using Alexandria.BreakableAPI;
using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using HutongGames.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tk2dRuntime.TileMap;
using UnityEngine;
using static ETGMod;
using static ModularMod.LaserDiode.Brapp;
using static ModularMod.LaserDiode.PewPew;

namespace ModularMod
{
    public class Sentry : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "SentryTurret_MDLR";

        public static void BuildPrefab()
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Sentry", guid, StaticCollections.Enemy_Collection, "gunnermech_die_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<TurretBehavior>();
                companion.aiActor.knockbackDoer.weight = 50000;
                companion.aiActor.MovementSpeed = 1f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(40f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.PathableTiles = CellTypes.FLOOR;
                companion.gameObject.GetOrAddComponent<TeleportationImmunity>();
                companion.aiActor.AssignedCurrencyToDrop = 0;

                companion.aiActor.healthHaver.SetHealthMaximum(40f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();

                EnemyBuildingTools.AddShadowToAIActor(companion.aiActor, EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject, new Vector2(0.75f, 0.25f), "shadowPos");


                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 5,
                    ManualOffsetY = 3,
                    ManualWidth = 14,
                    ManualHeight = 20,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0
                });
                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {

                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyHitBox,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 5,
                    ManualOffsetY = 3,
                    ManualWidth = 14,
                    ManualHeight = 20,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0,
                });


                //companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
                companion.aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>();
                companion.aiActor.PreventBlackPhantom = false;
                AIAnimator aiAnimator = companion.aiAnimator;
                aiAnimator.IdleAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.EightWay,
                    Flipped = new DirectionalAnimation.FlipType[8],
                    AnimNames = new string[]
                    {
                        "idle_up",
                        "idle_right_up",
                        "idle_right",
                        "idle_right_down",
                        "idle_down",
                        "idle_left_down",
                        "idle_left",
                        "idle_left_up"
                    },
                    Prefix = "idle"
                };


                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("SentryAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "awake", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "charge", new string[] {
                        "charge_up",
                        "charge_right_up",
                        "charge_right",
                        "charge_down_right",
                        "charge_down",
                        "charge_down_left",
                        "charge_left",
                        "charge_left_up"
                }, new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);


                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "death", new Dictionary<int, string> {
                    { 0, "Play_ENM_plasma_burst_01" },
                });
                

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
                companion.aiActor.reinforceType = ReinforceType.SkipVfx;


                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

                //companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;


                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldCenter;
                GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;

                bs.TargetBehaviors = new List<TargetBehaviorBase>
                {
                new TargetPlayerBehavior
                {
                    Radius = 35f,
                    LineOfSight = true,
                    ObjectPermanence = true,
                    SearchInterval = 0.25f,
                    PauseOnTargetSwitch = false,
                    PauseTime = 0.25f,
                }
                };




                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
                {
                   
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(MinionShotPredictive)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 1.5f,
                        CooldownVariance = 0.5f,
                        InitialCooldown = 1f,
                        FireAnimation = "charge",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Attack
                    },
                    NickName = "So Much Gun"
                    },
                   
                };

                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
                bs.TickInterval = behaviorSpeculator.TickInterval;
                bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
                bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
                bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
                bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
                bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

                
                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;

                AIBulletBank.Entry sentryEntry = EnemyBuildingTools.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"), "SentryShot", "Play_SentryRailgun",
                (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects
                , false);
                Projectile proj = sentryEntry.BulletObject.GetComponent<Projectile>();
                proj.sprite.renderer.enabled = false;
                var trailController = proj.AddTrailToProjectileBundle(StaticCollections.Beam_Collection, "sentry_hitscan_001", StaticCollections.Beam_Animation, "sentryshot", new Vector2(1, 1), new Vector2(0,0));
                var sprite = trailController.GetComponent<tk2dTiledSprite>();
                trailController.transform.parent = proj.gameObject.transform;
                sprite.usesOverrideMaterial = true;
                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                sprite.renderer.material.SetFloat("_EmissivePower", 500);
                sprite.renderer.material.SetFloat("_EmissiveColorPower", 5);
                proj.BulletScriptSettings = new BulletScriptSettings() { preventPooling = true };
                FuckYou = proj.gameObject;
                companion.aiActor.bulletBank.Bullets.Add(sentryEntry);

                DebrisObject tatter1 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_001", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter2 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_002", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter3 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_003", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter4 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_004", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter5 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_001", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter6 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_006", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter7 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_007", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter8 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_008", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter9 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_009", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter10 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_010", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);
                DebrisObject tatter11 = GenerateDebrisObjectBundle(StaticCollections.Enemy_Collection, "sentry_debris_011", true, 0.33f, 3, 240, 180, null, 0.4f, null, null, 0);

                ShardCluster tatterCluster = BreakableAPIToolbox.GenerateShardCluster(new DebrisObject[] 
                {
                    tatter1,
                    tatter2,
                    tatter3,
                    tatter4,
                    tatter5,
                    tatter6,
                    tatter7,
                    tatter8,
                    tatter9,
                    tatter10,
                    tatter11,
                }, 2f, 1.1f, 11, 17, 1.2f);
                SpawnShardsOnDeath taters = companion.aiActor.gameObject.AddComponent<SpawnShardsOnDeath>();
                taters.deathType = OnDeathBehavior.DeathType.Death;
                taters.breakStyle = MinorBreakable.BreakStyle.BURST;
                taters.verticalSpeed = 3f;
                taters.heightOffGround = 1f;
                taters.shardClusters = new ShardCluster[] { tatterCluster };

                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("4d37ce3d666b4ddda8039929225b7ede");
                ExplodeOnDeath explodeOnDeath = companion.gameObject.AddComponent<ExplodeOnDeath>();
                ExplosionData explosionData = orLoadByGuid.GetComponent<ExplodeOnDeath>().explosionData;
                explodeOnDeath.explosionData = explosionData;
                explodeOnDeath.explosionData.damage = 0;

                Game.Enemies.Add("mdlr:sentry_turret", companion.aiActor);
            }
        }

        public static DebrisObject GenerateDebrisObjectBundle(tk2dSpriteCollectionData data, string spritename, bool debrisObjectsCanRotate = true, float LifeSpanMin = 0.33f, float LifeSpanMax = 2f, float AngularVelocity = 540, float AngularVelocityVariance = 180f, tk2dSprite shadowSprite = null, float Mass = 1, string AudioEventName = null, GameObject BounceVFX = null, int DebrisBounceCount = 0, bool DoesGoopOnRest = false, GoopDefinition GoopType = null, float GoopRadius = 1f, bool usesWorldShader = true)
        {
            GameObject debrisObject =new GameObject("debris");
            FakePrefab.MarkAsFakePrefab(debrisObject);
            tk2dSprite tk2dsprite = debrisObject.AddComponent<tk2dSprite>();
            tk2dsprite.collection = data;
            tk2dsprite.SetSprite(data.GetSpriteIdByName(spritename));

            DebrisObject DebrisObj = debrisObject.AddComponent<DebrisObject>();
            DebrisObj.canRotate = debrisObjectsCanRotate;
            DebrisObj.lifespanMin = LifeSpanMin;
            DebrisObj.lifespanMax = LifeSpanMax;
            DebrisObj.bounceCount = DebrisBounceCount;
            DebrisObj.angularVelocity = AngularVelocity;
            DebrisObj.angularVelocityVariance = AngularVelocityVariance;
            if (AudioEventName != null) { DebrisObj.audioEventName = AudioEventName; }
            if (BounceVFX != null) { DebrisObj.optionalBounceVFX = BounceVFX; }
            DebrisObj.sprite = tk2dsprite;
            DebrisObj.DoesGoopOnRest = DoesGoopOnRest;
            if (GoopType != null) { DebrisObj.AssignedGoop = GoopType; } else if (GoopType == null && DebrisObj.DoesGoopOnRest == true) { DebrisObj.DoesGoopOnRest = false; }
            DebrisObj.GoopRadius = GoopRadius;
            if (shadowSprite != null) { DebrisObj.shadowSprite = shadowSprite; }
            DebrisObj.inertialMass = Mass;
            if (usesWorldShader == true)
            {
                //DebrisObj.sprite.renderer.material.shader = worldShader;
                //DebrisObj.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            }
            return DebrisObj;
        }


        public static GameObject FuckYou;



        public class TurretBehavior : BraveBehaviour
        {
            public float AttackFireRate = 3.5f;
            private void Start()
            {
                this.aiActor.healthHaver.OnPreDeath += (vector) =>
                {
                    var Obj = UnityEngine.Object.Instantiate(RobotMiniMecha.KaboomObject, this.aiActor.sprite.WorldCenter, Quaternion.identity);
                    Obj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("kaboom");
                };
            }
        }

        public class MinionShotPredictive : Script
        {
            public virtual bool IsBlue
            {
                get
                {
                    return false;
                }
            }

            public override IEnumerator Top()
            {
                float Angle = base.AimDirection;
                GameObject gameObject = SpawnManager.SpawnVFX(VFXStorage.LaserReticle, false);

                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                component2.transform.position = new Vector3(this.Position.x, this.Position.y, 99999);
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
                component2.dimensions = new Vector2(1000f, 1f);
                component2.UpdateZDepth();
                component2.HeightOffGround = -1;
                component2.gameObject.layer = 21;
                Color laser = IsBlue == true ? new Color(0, 0.25f, 1f) : new Color(1, 0.85f, 0.7f);
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 50);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 40f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

                GameManager.Instance.StartCoroutine(FlashReticles(component2, this));
                yield return this.Wait((this.BulletBank.aiActor.GetComponent<TurretBehavior>().AttackFireRate * 60) + 60);
                yield break;
            }
            private IEnumerator FlashReticles(tk2dTiledSprite tiledspriteObject, MinionShotPredictive parent)
            {
                tk2dTiledSprite tiledsprite = tiledspriteObject.GetComponent<tk2dTiledSprite>();
                float elapsed = 0;
                float elapsed2 = 0;

                float Time = this.BulletBank.aiActor.GetComponent<TurretBehavior>().AttackFireRate;
                float f = 0;

                float Interval = 0.5f;


                bool lightSHow = false;

                while (elapsed < Time)
                {
                    if (elapsed2 > Interval)
                    {
                        base.PostWwiseEvent("Play_TurretBeep", null);
                        elapsed2 = 0;
                        lightSHow = !lightSHow;
                        Color colorLaser = lightSHow == true ? Color.red : new Color(1, 1f, 1f);
                        tiledsprite.sprite.renderer.material.SetColor("_OverrideColor", colorLaser);
                        tiledsprite.sprite.renderer.material.SetColor("_EmissiveColor", colorLaser);
                    }
                    if (elapsed < (Time * 0.2f))
                    {
                        Interval = 0.5f;
                    }
                    if (elapsed > (Time * 0.2f) && elapsed < (Time * 0.4f))
                    {
                        Interval = 0.4f;

                    }
                    if (elapsed > (Time * 0.4f) && elapsed < (Time * 0.6f))
                    {
                        Interval = 0.3f;

                    }
                    if (elapsed > (Time * 0.6f) && elapsed < (Time * 0.8f))
                    {
                        Interval = 0.2f;

                    }
                    if (elapsed > (Time * 0.8f) && (elapsed < Time))
                    {
                        Interval = 0.1f;

                    }

                    float t = (float)elapsed / (float)Time;
                    if (this == null)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }
                    if (this.BulletBank == null)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }
                    if (this.BulletBank.gameObject == null)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }

                    if (tiledspriteObject != null)
                    {
                        tiledsprite.transform.position = this.BulletBank.sprite.WorldCenter;

                        Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition(), this.BulletManager.PlayerVelocity(), this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter, 90f);
                        float CentreAngle = (predictedPosition - this.Position).ToAngle();
                        f = CentreAngle;
                        tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, CentreAngle);
                        tiledsprite.dimensions = new Vector2(Vector2.Distance(this.Position, this.BulletManager.PlayerPosition()) * 160, 1f);
                        tiledsprite.UpdateZDepth();
                    }
                    elapsed += BraveTime.DeltaTime;
                    elapsed2 += BraveTime.DeltaTime;

                    yield return null;
                }
                elapsed = 0;
                Time = 0.25f;
                while (elapsed < Time)
                {
                    if (this == null)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }
                    if (this.BulletBank == null)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }
                    if (parent.IsEnded || parent.Destroyed)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }
                    if (this.BulletBank.gameObject == null)
                    {
                        UnityEngine.Object.Destroy(tiledspriteObject.gameObject);
                        yield break;
                    }

                    if (tiledspriteObject != null)
                    {
                        tiledsprite.transform.position = this.BulletBank.sprite.WorldCenter;
                        tiledsprite.dimensions = new Vector2(Vector2.Distance(this.Position, this.BulletManager.PlayerPosition()) * 160, 1f);
                        tiledsprite.HeightOffGround = -2;
                        tiledsprite.renderer.gameObject.layer = 23;
                        tiledsprite.UpdateZDepth();
                        bool enabled = elapsed % 0.25f > 0.125f;
                        tiledsprite.sprite.renderer.enabled = enabled;
                    }
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                UnityEngine.Object.Destroy(tiledspriteObject.gameObject);

                if (Time > 1.5f)
                {
                    this.BulletBank.aiActor.GetComponent<TurretBehavior>().AttackFireRate -= 0.5f;
                }
                if (base.BulletBank != null)
                {
                    //Play_SentryRailgun
                    base.PostWwiseEvent("Play_SentryRailgun", null);
                    base.Fire(new Direction(f, DirectionType.Absolute, -1f), new Speed(250, SpeedType.Absolute), new HitScan());

                }
                yield break;
            }
            public class HitScan : Bullet
            {
                public HitScan() : base("SentryShot", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    yield break;
                }
            }
        }
    }
}
