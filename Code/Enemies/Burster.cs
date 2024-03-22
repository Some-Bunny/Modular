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
using static ModularMod.Node;

namespace ModularMod
{
    public class Burster : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "Burster_MDLR";

        public static void BuildPrefab(bool isVertical)
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Burster", guid + (isVertical ? "_Vertical" : "_Horizontal"), StaticCollections.Enemy_Collection, "burster_idle_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<LaserDiodeBehavior>();
                companion.aiActor.knockbackDoer.weight = 7500;
                companion.aiActor.MovementSpeed = 0.8f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(36f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
                companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
                companion.aiActor.AssignedCurrencyToDrop = 0;
                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(36f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();

                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("4d37ce3d666b4ddda8039929225b7ede");
                ExplodeOnDeath explodeOnDeath = companion.gameObject.AddComponent<ExplodeOnDeath>();
                ExplosionData explosionData = orLoadByGuid.GetComponent<ExplodeOnDeath>().explosionData;
                explodeOnDeath.explosionData = explosionData;
                explodeOnDeath.explosionData.damage = 5;

                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 5,
                    ManualOffsetY = 4,
                    ManualWidth = 18,
                    ManualHeight = 13,
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
                    ManualOffsetY = 4,
                    ManualWidth = 18,
                    ManualHeight = 13,
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
                    Type = DirectionalAnimation.DirectionType.Single,
                    Flipped = new DirectionalAnimation.FlipType[1],
                    AnimNames = new string[]
                    {
                        "idle"
                    },
                    Prefix = "idle"
                };
                aiAnimator.MoveAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.Single,
                    Flipped = new DirectionalAnimation.FlipType[1],
                    AnimNames = new string[]
                    {
                        "idle"
                    },
                    Prefix = "idle"
                };

                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("BursterBaddieAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "charge", new string[] { "charge" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "fire", new string[] { "fire" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "death", new Dictionary<int, string> {
                    {0, "Play_BOSS_RatMech_Kneel_01" },
                });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "charge", new Dictionary<int, string> {
                    {0, "Play_WPN_elephantgun_reload_01" }
                });

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Default;


                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldCenter;
                GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;

                AIActor shelleton = EnemyDatabase.GetOrLoadByGuid("21dd14e5ca2a4a388adab5b11b69a1e1");
                AIBeamShooter component = shelleton.GetComponent<AIBeamShooter>();
                AIBeamShooter2 beamShooter1 = Toolbox.AddAIBeamShooter2(companion.aiActor, shootpoint.transform, "middle_small_beam", component.beamProjectile, component.beamModule);
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
                new BursterMoveBehavior() {
                    maxMoveSpeed = (isVertical ? new Vector2(1.25f, 1.25f): new Vector2(1.25f, 1.25f)),
                    moveAcceleration = (isVertical ? new Vector2(0, 2): new Vector2(2, 0))
                } 
                };

                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
                {
                    
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(PewPew)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 2.25f,
                        CooldownVariance = 0.75f,
                        InitialCooldown = 0f,
                        TellAnimation = "charge",
                        PostFireAnimation = "fire",
                        RequiresLineOfSight = false,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                    },
                    NickName = "Pewpew Attack"
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
                mat2.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;


                companion.healthHaver.spawnBulletScript = true;
                companion.healthHaver.chanceToSpawnBulletScript = 1f;
                companion.healthHaver.bulletScriptType = HealthHaver.BulletScriptType.OnPreDeath;
                companion.healthHaver.bulletScript = new CustomBulletScriptSelector(typeof(DeathPew));

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundLarge"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                Game.Enemies.Add("mdlr:burster" + (isVertical ? "_vertical" : "_horizontal"), companion.aiActor);

                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
                companion.encounterTrackable.journalData = new JournalEntry();
                companion.encounterTrackable.EncounterGuid = "mdlr:burster" + (isVertical ? "_vertical" : "_horizontal");
                companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                companion.encounterTrackable.journalData.SuppressKnownState = true;
                companion.encounterTrackable.journalData.IsEnemy = true;
                companion.encounterTrackable.journalData.SuppressInAmmonomicon = true;
                companion.encounterTrackable.ProxyEncounterGuid = "";
                companion.encounterTrackable.journalData.AmmonomiconSprite = "";
                companion.encounterTrackable.journalData.enemyPortraitSprite = null;//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\hmprimesheet.png");
                Module.Strings.Enemies.Set("#BURSTER_NAME", "Burster Drone");
                companion.encounterTrackable.journalData.PrimaryDisplayName = "#BURSTER_NAME";
                companion.encounterTrackable.journalData.NotificationPanelDescription = "#MODULARPRIME_SD";
                companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#MODULARPRIME_LD";
            }
        }

        public class LaserDiodeBehavior : BraveBehaviour
        {
            private void Start()
            {
                
            }
        }



        public class PewPew : Script
        {
            public override IEnumerator Top()
            {
                this.PostWwiseEvent("Play_WPN_elephantgun_shot_01", null);
                for (int i = 0; i < 8; i++)
                {
                    base.Fire(new Direction((45 * i) -4, DirectionType.Absolute, -1f), new Speed(7.5f, SpeedType.Absolute), new Bullet("poundSmall"));
                    base.Fire(new Direction((45 * i), DirectionType.Absolute, -1f), new Speed(7.5f, SpeedType.Absolute), new Bullet("poundSmall"));
                    base.Fire(new Direction((45 * i) + 4, DirectionType.Absolute, -1f), new Speed(7.5f, SpeedType.Absolute), new Bullet("poundSmall"));
                }
                yield break;
            }
            
        }

        public class DeathPew : Script
        {
            public override IEnumerator Top()
            {
                this.PostWwiseEvent("Play_WPN_elephantgun_shot_01", null);

                for (int t = 0; t < 3; t++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        base.Fire(new Direction((45 * i), DirectionType.Absolute, -1f), new Speed(4f + (1f * t), SpeedType.Absolute), new DeathBurst());
                        base.Fire(new Direction((45 * i) + 1, DirectionType.Absolute, -1f), new Speed(3f + (1f * t), SpeedType.Absolute), new DeathBurst());
                        base.Fire(new Direction((45 * i) - 1, DirectionType.Absolute, -1f), new Speed(3f + (1f * t), SpeedType.Absolute), new DeathBurst());
                    }
                }            
                yield break;
            }
            public class DeathBurst : Bullet
            {
                public DeathBurst() : base("poundSmall", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(0.1f, SpeedType.Absolute), 240);
                    yield return this.Wait(600);
                    base.Vanish(false);

                    yield break;
                }
            }
        }
    }



    public class BursterMoveBehavior : MovementBehaviorBase
    {
        public override void Start()
        {
            base.Start();
            this.m_updateEveryFrame = true;
            this.m_centerX = new float?(this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.x);
            this.m_centerY = new float?(this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.y);

        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.m_aiActor.BehaviorOverridesVelocity = true;
        }

        public override BehaviorResult Update()
        {
            if (!this.m_aiActor.TargetRigidbody)
            {
                return BehaviorResult.Continue;
            }


            Vector2 unitCenter = this.m_aiActor.TargetRigidbody.UnitCenter;
            Vector2 zero = Vector2.zero;
            
            if (this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.x < this.m_centerX.Value - 2f)
            {
                zero.x = 1f;
            }
            else if (this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.x > this.m_centerX.Value + 2f)
            {
                zero.x = -1f;
            }

            if (this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.y < this.m_centerY.Value - 2f)
            {
                zero.y = 1f;
            }
            else if (this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.y > this.m_centerY.Value + 2f)
            {
                zero.y = 1f;
            }
            
            bool useRamingSpeedX = false;
            bool useRamingSpeedY= false;

            if (unitCenter.x < this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitLeft)
            {
                useRamingSpeedX = true;
                zero.x = -1f;
            }
            else if (unitCenter.x > this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitRight)
            {
                useRamingSpeedX = true;
                zero.x = 1f;
            }


            if (unitCenter.y < this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitBottom)
            {
                useRamingSpeedY = true;
                zero.y = -1f;
            }
            else if (unitCenter.y > this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitTop)
            {
                useRamingSpeedY = true;
                zero.y = 1f;
            }

            this.m_aiActor.BehaviorVelocity.x = this.RamMoveTowards(this.m_aiActor.BehaviorVelocity.x, zero.x * this.maxMoveSpeed.x, this.moveAcceleration.x * this.m_deltaTime, useRamingSpeedX);

            this.m_aiActor.BehaviorVelocity.y = this.RamMoveTowards(this.m_aiActor.BehaviorVelocity.y, zero.y * this.maxMoveSpeed.y, this.moveAcceleration.y * this.m_deltaTime, useRamingSpeedY);
            return BehaviorResult.Continue;
        }

        private float RamMoveTowards(float current, float target, float maxDelta, bool useRamingSpeed)
        {
            float num = target;
            float num2 = maxDelta;
            if (useRamingSpeed)
            {
                num = target * this.ramMultiplier;
                num2 = maxDelta * this.ramMultiplier;
            }
            if ((num < 0f && (current < num || current >= 0f)) || (num > 0f && (current > num || current <= 0f)))
            {
                num2 = maxDelta * this.ramMultiplier;
            }
            if (Mathf.Abs(num - current) <= num2)
            {
                return num;
            }
            return current + Mathf.Sign(num - current) * num2;
        }

        public Vector2 maxMoveSpeed = new Vector2(3f, 3f);

        public Vector2 moveAcceleration = new Vector2(2f, 0f);

        public float ramMultiplier = 3f;

        private float? m_centerX;
        private float? m_centerY;

    }
}

