using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker.Actions;
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
    public class Node : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "Node_MDLR";
        public static GameObject KaboomObject;

        public static void BuildPrefab()
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Node", guid, StaticCollections.Enemy_Collection, "node_frontback_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<MechaBehavior>();
                companion.aiActor.knockbackDoer.weight = 35;
                companion.aiActor.MovementSpeed = 0.1f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 0f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(12f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.PathableTiles = CellTypes.FLOOR;
                companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
                companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
                companion.aiActor.AssignedCurrencyToDrop = 0;

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(12f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();

                ImprovedAfterImage image = companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
                image.dashColor = new Color(1, 0.1f, 0.1f);
                image.spawnShadows = true;


                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 0,
                    ManualOffsetY = 1,
                    ManualWidth = 16,
                    ManualHeight = 15,
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
                    ManualOffsetX = 0,
                    ManualOffsetY = 1,
                    ManualWidth = 16,
                    ManualHeight = 15,
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
                        "idle_frontback",
                        "idle_frontleftbackright",
                        
                        "idle_leftright",
                        "idle_frontrightbackleft",
                        
                        "idle_frontback",
                        "idle_frontleftbackright",
                        
                        "idle_leftright",
                        "idle_frontrightbackleft"
                    },
                    Prefix = "idle"
                };
                aiAnimator.MoveAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.EightWay,
                    Flipped = new DirectionalAnimation.FlipType[8],
                    AnimNames = new string[]
                    {
                        "idle_frontback",
                        "idle_frontleftbackright",

                        "idle_leftright",
                        "idle_frontrightbackleft",

                        "idle_frontback",
                        "idle_frontleftbackright",

                        "idle_leftright",
                        "idle_frontrightbackleft"
                    },
                    Prefix = "move"
                };

                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("NodeEnemyAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "charge", new string[] {                         
                        "charge_frontback",
                        "charge_frontleftbackright",

                        "charge_leftright",
                        "charge_frontrightbackleft",

                        "charge_frontback",
                        "charge_frontleftbackright",

                        "charge_leftright",
                        "charge_frontrightbackleft" }, 
                        new DirectionalAnimation.FlipType[8], DirectionalAnimation.DirectionType.EightWay);

                
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "death", new Dictionary<int, string> {
                    { 0, "Play_DragunGrenade" },
                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_frontback", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" },});
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_frontleftbackright", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" }, });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_leftright", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" }, });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_frontrightbackleft", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" }, });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_frontback", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" }, });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_frontleftbackright", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" }, });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_leftright", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" }, });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge_frontrightbackleft", new Dictionary<int, string> { { 0, "Play_ENM_bullet_dash_01" }, });



                companion.aiActor.AwakenAnimType = AwakenAnimationType.Spawn;


                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

                companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;


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
                bs.MovementBehaviors = new List<MovementBehaviorBase>() {
                new SeekTargetBehavior() {
                    StopWhenInRange = true,
                    CustomRange = 4,
                    LineOfSight = true,
                    ReturnToSpawn = true,
                    SpawnTetherDistance = 0,
                    PathInterval = 0.5f,
                    SpecifyRange = false,
                    MinActiveRange = 1,
                    MaxActiveRange = 10
                } 
                };


                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
                {
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Behavior = new CustomDashBehavior()
                        {
					dashAnim = "charge",
					ShootPoint = m_CachedGunAttachPoint,
                    dashDistance = 9f,
                    dashTime = 1f,
                    AmountOfDashes = 1,
                    enableShadowTrail = false,
                    Cooldown = 3,
                    dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                    warpDashAnimLength = true,
                    hideShadow = true,
                    fireAtDashStart = true,
                    InitialCooldown = 1f,
                    AttackCooldown = 1f,
                    bulletScript = new CustomBulletScriptSelector(typeof(ChargeShot)),
                    RequiresLineOfSight = false,
                        }
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1,
                        Behavior = new CustomDashBehavior()
                        {
                    dashAnim = "charge",
                    ShootPoint = m_CachedGunAttachPoint,
                    dashDistance = 9f,
                    dashTime = 1f,
                    AmountOfDashes = 2,
                    enableShadowTrail = false,
                    Cooldown = 0.7f,
                    dashDirection = DashBehavior.DashDirection.KindaTowardTarget,
                    warpDashAnimLength = true,
                    hideShadow = true,
                    fireAtDashStart = true,
                    InitialCooldown = 1f,
                    AttackCooldown = 0.5f,
                    RequiresLineOfSight = false,
                        }
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.5f,
                        Behavior = new CustomDashBehavior()
                        {
                    dashAnim = "charge",
                    ShootPoint = m_CachedGunAttachPoint,
                    dashDistance = 9f,
                    dashTime = 1f,
                    AmountOfDashes = 2,
                    enableShadowTrail = false,
                    Cooldown = 3,
                    dashDirection = DashBehavior.DashDirection.Random,
                    warpDashAnimLength = true,
                    hideShadow = true,
                    fireAtDashStart = true,
                    InitialCooldown = 1f,
                    AttackCooldown = 0.75f,
                    
                    RequiresLineOfSight = false,
                        }
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                         Probability = 0.8f,
                        Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Gunge)),
                        LeadAmount = 0f,
                        AttackCooldown = 2f,
                        Cooldown = 6f,
                        CooldownVariance = 1,
                        InitialCooldown = 3f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                    },
                    NickName = "Pewpew Attack"
                    
                    }
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
                mat2.SetColor("_EmissiveColor", new Color32(255, 211, 214, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;


                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));


                AIBulletBank.Entry sentryEntry = EnemyBuildingTools.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").bulletBank.GetBullet("default"), 
                "desparation", 
                null,
                (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects
                , false);
                
                
                Projectile proj = sentryEntry.BulletObject.GetComponent<Projectile>();
                //proj.shouldRotate = true;
                //proj.sprite.SetSprite(StaticCollections.Projectile_Collection, "node_desparation_001");
                //proj.specRigidbody.Reinitialize();
                /*
                var projAnimator = proj.transform.Find("Sprite").gameObject.GetOrAddComponent<tk2dSpriteAnimator>();

                projAnimator.Library = StaticCollections.Projectile_Animation;
                projAnimator.defaultClipId = projAnimator.Library.GetClipIdByName("node_desparation");
                projAnimator.playAutomatically = true;
                */

                proj.sprite.usesOverrideMaterial = true;
                proj.hitEffects.alwaysUseMidair = true;
                proj.hitEffects.overrideMidairDeathVFX = StaticExplosionDatas.explosiveRoundsExplosion.effect;
                proj.BulletScriptSettings = new BulletScriptSettings() { preventPooling = true };
                Material mat3 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat3.mainTexture = proj.sprite.renderer.material.mainTexture;
                mat3.SetColor("_EmissiveColor", new Color32(255, 211, 214, 255));
                mat3.SetFloat("_EmissiveColorPower", 1.55f);
                mat3.SetFloat("_EmissivePower", 100);
                mat3.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                proj.sprite.renderer.material = mat3;

                var tro = proj.gameObject.AddChild("trail object");
                //tro.layer = 10;
                tro.transform.position = proj.sprite.WorldCenter;// + new Vector2(.25f, 0.3125f);
                tro.transform.localPosition = proj.sprite.WorldCenter;// + new Vector2(.25f, 0.3125f);

                TrailRenderer tr = tro.AddComponent<TrailRenderer>();
                tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                tr.receiveShadows = false;
                var asd = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
                asd.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                asd.SetFloat("_EmissivePower", 30);
                asd.SetFloat("_EmissiveColorPower", 30);
                tr.material = asd;
                tr.minVertexDistance = 0.01f;
                tr.numCapVertices = 20;

                //======
                UnityEngine.Color color = UnityEngine.Color.red;
                tr.startColor = color;
                tr.endColor = color * 0.7f;
                //======
                tr.time = 0.3f;
                //======
                tr.startWidth = 0.75f;
                tr.endWidth = 0f;
                tr.autodestruct = false;

                var rend = proj.gameObject.AddComponent<ProjectileTrailRendererController>();
                rend.trailRenderer = tr;
                rend.desiredLength = 4;

                proj.gameObject.AddComponent<DetachTrailFromParent>();
                //proj.transform.Find("Sprite").gameObject.CreateFastBody(CollisionLayer.Projectile, new IntVector2(16, 16), new IntVector2(0, 0));

               companion.aiActor.bulletBank.Bullets.Add(sentryEntry);





                GameObject VFX = new GameObject("VFX");
                FakePrefab.DontDestroyOnLoad(VFX);
                FakePrefab.MarkAsFakePrefab(VFX);
                var tk2d = VFX.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.VFX_Collection;
                tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("nodecharge_010"));
                var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();
                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("NodeChargeAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("charge");
                tk2dAnim.playAutomatically = true;
                tk2d.usesOverrideMaterial = true;
                tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                tk2d.renderer.material.SetFloat("_EmissivePower", 60);
                tk2d.renderer.material.SetFloat("_EmissiveColorPower", 6);

                KaboomObject = VFX;

                companion.aiActor.aiAnimator.OtherVFX = new List<AIAnimator.NamedVFXPool>()
                {
                    new AIAnimator.NamedVFXPool()
                    {
                        name = "chargeUp",
                        anchorTransform = companion.transform,
                        vfxPool = new VFXPool()
                        {
                            effects = new VFXComplex[]
                            {
                                new VFXComplex()
                                {
                                    effects = new VFXObject[]
                                    {
                                        new VFXObject()
                                        {
                                            effect = VFX
                                        },
                                        
                                    },
                                    
                                }
                            }                                
                        }
                    }
                };

                companion.healthHaver.spawnBulletScript = true;
                companion.healthHaver.chanceToSpawnBulletScript = 1f;
                companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
                companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(DIE));

                Game.Enemies.Add("mdlr:node", companion.aiActor);


                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
                companion.encounterTrackable.journalData = new JournalEntry();
                companion.encounterTrackable.EncounterGuid = "mdlr:node";
                companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                companion.encounterTrackable.journalData.SuppressKnownState = true;
                companion.encounterTrackable.journalData.IsEnemy = true;
                companion.encounterTrackable.journalData.SuppressInAmmonomicon = true;
                companion.encounterTrackable.ProxyEncounterGuid = "";
                companion.encounterTrackable.journalData.AmmonomiconSprite = "";
                companion.encounterTrackable.journalData.enemyPortraitSprite = null;//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\hmprimesheet.png");
                Module.Strings.Enemies.Set("#NODE_DRONE_NAME", "Micro Drone");
                companion.encounterTrackable.journalData.PrimaryDisplayName = "#NODE_DRONE_NAME";
                companion.encounterTrackable.journalData.NotificationPanelDescription = "#MODULARPRIME_SD";
                companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#MODULARPRIME_LD";
            }
        }




        public class MechaBehavior : BraveBehaviour
        {
            private void Start()
            {
                this.aiActor.healthHaver.OnPreDeath += (vector) =>
                {
                };
            }
        }

        public class DIE : Script
        {
            public override IEnumerator Top()
            {
                base.Fire(new Direction(UnityEngine.Random.Range(-30, 30), DirectionType.Aim, -1f), new Speed(2, SpeedType.Absolute), new Rotater());
                yield break;
            }
            public class Rotater : Bullet
            {
                public Rotater() : base("desparation", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    base.PostWwiseEvent("Play_WPN_chargelaser_shot_01", null);

                    Exploder.DoDistortionWave(this.Projectile.sprite.WorldCenter, 3 * ConfigManager.DistortionWaveMultiplier, 0.3f * ConfigManager.DistortionWaveMultiplier, 10, 0.5f);
                    this.ChangeSpeed(new Brave.BulletScript.Speed(20, SpeedType.Absolute), 120);
                    yield return base.Wait(15);
                    for (int i = 0; i < 120; i++)
                    {
                        float aim = base.GetAimDirection(1f, 10);
                        float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
                        if (Mathf.Abs(delta) > 100f)
                        {
                            yield break;
                        }
                        this.Direction += Mathf.MoveTowards(0f, delta, 1f);
                        yield return base.Wait(1);
                    }
                    yield return base.Wait(90);
                    base.Vanish(false);
                    yield break;
                }
                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    Exploder.DoDistortionWave(this.Projectile.sprite.WorldCenter, 3 * ConfigManager.DistortionWaveMultiplier, 0.3f * ConfigManager.DistortionWaveMultiplier, 10, 0.2f);

                    AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", this.Projectile.gameObject);
                    float f = BraveUtility.RandomAngle();
                    for (int i = 0; i < 12; i++)
                    {
                        base.Fire(new Direction(f + (i * 30), DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new Bullet("poundSmall"));
                        base.Fire(new Direction(f + (i * 30), DirectionType.Absolute, -1f), new Speed(13, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 9, 120));
                    }

                }
              
            }
           
           
        }

        public class Gunge : Script
        {
            public override IEnumerator Top()
            {
                this.PostWwiseEvent("Play_ENM_kali_charge_01", null);
                var obj = UnityEngine.Object.Instantiate(KaboomObject, this.BulletBank.aiActor.sprite.WorldCenter - new Vector2(1,1), Quaternion.identity);
                obj.transform.parent = this.BulletBank.aiActor.transform;
                obj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("charge");
                yield return base.Wait(75);
                for (int e = 0; e < 3; e++)
                {
                    float angle = (this.GetPredictedTargetPosition(1, 10) - this.BulletBank.transform.PositionVector2()).ToAngle();
                    base.Fire(new Direction(angle + 30, DirectionType.Absolute, -1f), new Speed(10, SpeedType.Absolute), new TinyBullet());
                    base.Fire(new Direction(angle, DirectionType.Absolute, -1f), new Speed(15, SpeedType.Absolute), new TinyBullet());
                    base.Fire(new Direction(angle - 30, DirectionType.Absolute, -1f), new Speed(10, SpeedType.Absolute), new TinyBullet());
                    this.BulletBank.aiActor.knockbackDoer.ApplyKnockback(Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 2), 25);
                    yield return base.Wait(40);

                }




                yield break;
            }

           
            public class TinyBullet : Bullet
            {
                public TinyBullet() : base("directedFire", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    yield break;
                }
            }
            public class SwarmShots : Bullet
            {

                public SwarmShots() : base("self", false, false, false){ }
                public override IEnumerator Top()
                {
                    yield break;
                }

            }
        }

        public class ChargeShot : Script
        {
            public override IEnumerator Top()
            {
                
                float angle = (this.GetPredictedTargetPosition(1, 10) - this.BulletBank.transform.PositionVector2()).ToAngle();
                for (int i = -1; i < 2; i++)
                {
                    base.Fire(new Direction(angle + (i * 20), DirectionType.Absolute, -1f), new Speed(5, SpeedType.Absolute), new SpeedChangingBullet("directedFire", 12, 90));
                }
                yield break;
            }


           
        }
    }
}
