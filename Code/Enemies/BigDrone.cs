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
using static ModularMod.LaserDiode;
using static ModularMod.LaserDiode.Brapp;
using static ModularMod.LaserDiode.PewPew;
using static ModularMod.Node;
using static ModularMod.Sentry;

namespace ModularMod
{
    public class BigDrone : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "BigDrone_MDLR";

        public static void BuildPrefab()
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Big Drone", guid, StaticCollections.Enemy_Collection, "superdrone_idle_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<MechaBehavior>();
                companion.aiActor.knockbackDoer.weight = 150;
                companion.aiActor.MovementSpeed = 2f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(70f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                //companion.healthHaver.invulnerabilityPeriod = 1;
                //companion.healthHaver.usesInvulnerabilityPeriod = true;

                companion.aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(70f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();


                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 6,
                    ManualOffsetY = 4,
                    ManualWidth = 26,
                    ManualHeight = 17,
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
                    ManualOffsetX = 6,
                    ManualOffsetY = 4,
                    ManualWidth = 26,
                    ManualHeight = 17,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0,
                });


                ImprovedAfterImage image = companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
                image.dashColor = new Color(0, 2, 2);
                image.spawnShadows = false;
                image.shadowTimeDelay = 0.05f;

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
                    Prefix = "move"
                };

                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("BigDroneAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "preraise", new string[] { "preraise" }, new DirectionalAnimation.FlipType[1]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "raise", new string[] { "raise" }, new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "prepdash", new string[] { "prepdash" }, new DirectionalAnimation.FlipType[1]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "dash", new string[] { "dash" }, new DirectionalAnimation.FlipType[1]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "postdash", new string[] { "postdash" }, new DirectionalAnimation.FlipType[1]);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "prepmissile", new string[] { "prepmissile" }, new DirectionalAnimation.FlipType[1]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "missile", new string[] { "missile" }, new DirectionalAnimation.FlipType[1]);

                //m_ENM_cube_dash_01
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "prepdash", new Dictionary<int, string> {
                    { 0, "Play_BOSS_cyborg_charge_01" },
                                        { 5, "Play_ENM_cube_dash_01" },

                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "death", new Dictionary<int, string> {
                    { 0, "Play_BOSS_RatMech_Hop_01" },
                    { 2, "Play_OBJ_lightning_flash_01" },
                    { 8, "Play_OBJ_lightning_flash_01" },

                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "preraise", new Dictionary<int, string> {
                    { 0, "Play_BOSS_RatMech_Shutter_01" },
                });



                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "prepmissile", new Dictionary<int, string> {
                    { 1, "Play_BOSS_RatMech_Squat_01" },
                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "missile", new Dictionary<int, string> {
                    { 0, "Play_BOSS_RatMech_Missile_01" },
                    { 3, "Play_BOSS_RatMech_Missile_01" },
                });


                //Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "awake", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);


                /*

                */

                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("4d37ce3d666b4ddda8039929225b7ede");
                ExplodeOnDeath explodeOnDeath = companion.gameObject.AddComponent<ExplodeOnDeath>();
                ExplosionData explosionData = orLoadByGuid.GetComponent<ExplodeOnDeath>().explosionData;
                explodeOnDeath.explosionData = explosionData;
                explodeOnDeath.explosionData.damage = 15;

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Default;


                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

                //companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;


                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldBottomLeft + new Vector2(1.1875f, 0.75f);
                GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;

                AIActor shelleton = EnemyDatabase.GetOrLoadByGuid("21dd14e5ca2a4a388adab5b11b69a1e1");
                AIBeamShooter component = shelleton.GetComponent<AIBeamShooter>();

                AIBeamShooter2 beamShooter1 = Toolbox.AddAIBeamShooter2(companion.aiActor, shootpoint.transform, "middle_small_beam", component.beamProjectile, component.beamModule, 0);
                AIBeamShooter2 beamShooter2 = Toolbox.AddAIBeamShooter2(companion.aiActor, shootpoint.transform, "middle_small_beam", component.beamProjectile, component.beamModule, 90);
                AIBeamShooter2 beamShooter3 = Toolbox.AddAIBeamShooter2(companion.aiActor, shootpoint.transform, "middle_small_beam", component.beamProjectile, component.beamModule, 180);
                AIBeamShooter2 beamShooter4 = Toolbox.AddAIBeamShooter2(companion.aiActor, shootpoint.transform, "middle_small_beam", component.beamProjectile, component.beamModule, 270);


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
                    Probability = 0.5f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Rockets)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 1.5f,
                        CooldownVariance = 0.5f,
                        InitialCooldown = 1f,
                        ChargeTime = 1,
                        ChargeAnimation = "prepmissile",
                        FireAnimation = "missile",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        MaxUsages = 1,
                        StopDuring = ShootBehavior.StopType.Attack
                    },
                    NickName = "Rockets"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1.5f,
                        Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(GunGun)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 3f,
                        CooldownVariance = 0.5f,
                        InitialCooldown = 4f,
                        ChargeTime = 1,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Attack
                    },
                    NickName = "So Much Gun"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                        Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Shotgun)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 5f,
                        CooldownVariance = 2f,
                        InitialCooldown = 1f,
                        ChargeTime = 0,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Attack
                    },
                    NickName = "So Much Gun"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1,
                        Behavior = new CustomDashBehavior()
                        {
                        dashAnim = "dash",
                        chargeAnim = "prepdash",
                        postDashSpeed = 0,
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 15f,
                        dashTime = 0.75f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        Cooldown = 5f,
                        dashDirection = DashBehavior.DashDirection.KindaTowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = false,
                        fireAtDashStart = true,
                        InitialCooldown = 1f,
                        AttackCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        bulletScript = new CustomBulletScriptSelector(typeof(Dash_)),
                        }
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1,
                        Behavior = new CustomDashBehavior()
                        {
                        dashAnim = "dash",
                        chargeAnim = "prepdash",
                        postDashSpeed = 0,
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 9f,
                        dashTime = 0.5f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        Cooldown = 2f,
                        dashDirection = DashBehavior.DashDirection.TowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = false,
                        fireAtDashStart = true,
                        InitialCooldown = 1f,
                        AttackCooldown = 0.5f,
                        RequiresLineOfSight = false,
                        //bulletScript = new CustomBulletScriptSelector(typeof(Dash_)),
                        }
                    },
                    /*
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(PewPew)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 10f,
                        CooldownVariance = 3,
                        InitialCooldown = 0.5f,
                        //TellAnimation = "laserdiode_preattack1",
                        //PostFireAnimation = "laserdiode_postattack1",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                    },
                    NickName = "MainLaserAttack"
                    },//0
                    */
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.7f,
                    Behavior = new CustomBeholsterLaserBehavior() {
                    InitialCooldown = 1f,
                    firingTime = 7f,
                    AttackCooldown = 1f,
                    Cooldown = 6,
                    CooldownVariance = 0,
                    RequiresLineOfSight = false,
                    UsesCustomAngle = true,
                    MaxUsages = 2,
                    RampHeight = 14,
                    firingType = CustomBeholsterLaserBehavior.FiringType.ONLY_NORTHANGLEVARIANCE,
                    chargeTime = 1f,
                    UsesBaseSounds = false,
                    AdditionalHeightOffset = 11,
                    EnemyChargeSound = "Play_BOSS_omegaBeam_charge_01",
                    LaserFiringSound = "Play_ENM_deathray_shot_01",
                    StopLaserFiringSound = "Stop_ENM_deathray_loop_01",
                    ChargeAnimation = "preraise",
                    FireAnimation = "raise",
                    //PostFireAnimation = "postattack_2",
                    hurtsOtherHealthhavers = false,
                    beamSelection = ShootBeamBehavior.BeamSelection.Specify,
                    specificBeamShooters = new List<AIBeamShooter2>() { beamShooter1,beamShooter2,beamShooter3,beamShooter4 },
                    MinRange = 4,
                    Range = 10,
                    trackingType = CustomBeholsterLaserBehavior.TrackingType.ConstantTurn,
                    DoesSpeedLerp = true,
                    DoesReverseSpeedLerp = true,
                    InitialStartingSpeed = 0,
                    TimeToStayAtZeroSpeedAt = 1f,
                    TimeToReachFullSpeed = 1f,
                    TimeToReachEndingSpeed = 1,
                    EndingSpeed = 0,
                    LocksFacingDirection = false,
                    maxTurnRate = 45,
                    maxUnitForCatchUp = 2f,
                    minDegreesForCatchUp = 60,
                    minUnitForCatchUp = 0.1f,
                    minUnitForOvershoot = 1,
                    turnRateAcceleration = 1,
                    unitCatchUpSpeed = 1,
                    unitOvershootSpeed = 1,
                    unitOvershootTime = 0.25f,
                    degreeCatchUpSpeed = 90,
                    useDegreeCatchUp = companion.transform,
                    useUnitCatchUp = true,
                    useUnitOvershoot = true,
                    //GlobalCooldown = 0,
                    ShootPoint = m_CachedGunAttachPoint.transform,
                    StopDuring = CustomBeholsterLaserBehavior.StopType.All,
                    BulletScript = new CustomBulletScriptSelector(typeof(PewPew)),
                    },
                    NickName = "LASER"
                    },//0 
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
                companion.aiActor.AssignedCurrencyToDrop = 0;


                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("missile"));

                Game.Enemies.Add("mdlr:big_drone", companion.aiActor);

            }
        }




        public class MechaBehavior : BraveBehaviour
        {
            public GlobalMessageRadio.MessageContainer container;
            private void Start()
            {
                this.aiActor.healthHaver.OnPreDeath += (vector) =>
                {
                };
            }
        }

        public class Rockets : Script
        {
            public override IEnumerator Top()
            {
                bool b = BraveUtility.RandomBool();
                for (int e = 0; e < 2; e++)
                {
                    b = !b;
                    base.Fire(new Direction(b == true ? -20 : 20, DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new Rocket());
                    yield return this.Wait(60);
                }
                yield break;
            }
            public class Rocket : Bullet
            {
                public Rocket() : base("missile", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    this.PostWwiseEvent("Play_BOSS_RatMech_Missile_01", null);
                    this.PostWwiseEvent("Play_WPN_YariRocketLauncher_Shot_01", null);
                    this.PostWwiseEvent("Play_BOSS_RatMech_Missile_01", null);
                    this.PostWwiseEvent("Play_WPN_YariRocketLauncher_Shot_01", null);
                    this.Projectile.BulletScriptSettings = new BulletScriptSettings() { surviveRigidbodyCollisions = true };
                    this.Projectile.spriteAnimator.Play();
                    for (int i = 0; i < 360; i++)
                    {
                        float aim = base.GetAimDirection(1f, 10);
                        float delta = BraveMathCollege.ClampAngle180(aim - this.Direction);
                        this.Direction += Mathf.MoveTowards(0f, delta, 2f);

                        float Distance = Vector2.Distance(this.GetPredictedTargetPosition(1, 100), this.Position);
                        this.Speed = Distance > 12 ? Mathf.Min(12, Mathf.Max(3, 24 - (Distance * 2))) : Mathf.Max(6, Distance);
                        yield return base.Wait(1);
                    }
                    yield break;
                }
                public override void OnBulletDestruction(Bullet.DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
                {
                    if (preventSpawningProjectiles)
                    {
                        return;
                    }
                    base.PostWwiseEvent("Play_WPN_smallrocket_impact_01", null);
                    base.PostWwiseEvent("Play_WPN_smallrocket_impact_01", null);
                }
            }
        }

        public class GunGun : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < 10; i++)
                {
                    float aimDirection = base.GetAimDirection(1f, 30);
                    base.Fire(new Direction( aimDirection, DirectionType.Absolute, -1f), new Speed((i * 1.5f) + 7.5f, SpeedType.Absolute), new Bullet("directedfire"));
                    yield return this.Wait(5);
                }
                yield break;
            }
        }

        public class Shotgun : Script
        {
            public override IEnumerator Top()
            {
                float aimDirection = base.GetAimDirection(1f, 7f);
                for (int j = -4; j <= 4; j++)
                {
                    base.Fire(new Direction(aimDirection + (float)(j * 3), DirectionType.Absolute, -1f), new Speed(9f - (float)Mathf.Abs(j) * 0.8f, SpeedType.Absolute), new TinyBullet(false));
                }
                yield break;
            }
            public class TinyBullet : Bullet
            {
                public TinyBullet(bool fart) : base("reversible", false, false, false)
                {
                    Fart = fart;
                }
                public override IEnumerator Top()
                {
                    this.Projectile.spriteAnimator.Play();
                    if (Fart == true)
                    {
                        this.ChangeSpeed(new Brave.BulletScript.Speed(0, SpeedType.Absolute), 90); yield return this.Wait(420); base.Vanish(false);
                    }
                    yield break;
                }
                private bool Fart;
            }
        }

        public class Dash_ : Script
        {
            public override IEnumerator Top()
            {
                float aimDirection = base.GetAimDirection(1f, 14f);
                this.PostWwiseEvent("Play_BOSS_RatMech_Hop_01", null);
                for (int i = 0; i < 20; i++)
                {
                    for (int e = 0; e < 3; e++)
                    {
                        base.Fire(new Direction((120 * e) + aimDirection, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new Rotater());
                    }
                    yield return this.Wait(3);
                }
                yield break;
            }
            public class Rotater : Bullet
            {
                public Rotater() : base("poundSmall", false, false, false)
                {
                }
                public override IEnumerator Top()
                {
                    yield return this.Wait(30);
                    this.ChangeSpeed(new Brave.BulletScript.Speed(20, SpeedType.Absolute), 120);
                    yield break;
                }
            }
        }


        public class PewPew : Script
        {
            public override IEnumerator Top()
            {
                this.PostWwiseEvent("Play_BOSS_RatMech_Hop_01", null);
                for (int j = 0; j < 16; j++)
                {
                    base.Fire(new Direction(45 * j + (j <= 7 ? 22.5f : 0), DirectionType.Absolute, -1f), new Speed(j <= 7 ? 8 : 5, SpeedType.Absolute), new TinyBullet(true));
                }
                yield return this.Wait(60);
                for (int e = 0; e < 4; e++)
                {
                    float aimDirection = base.GetAimDirection(1f, 14f);
                    this.PostWwiseEvent("Play_ENM_bulletking_shot_01", null);
                    for (int j = -2; j <= 2; j++)
                    {
                        base.Fire(new Direction(aimDirection + (float)(j * 4), DirectionType.Absolute, -1f), new Speed(9f - (float)Mathf.Abs(j) * 0.5f, SpeedType.Absolute), new TinyBullet(false));
                    }
                    yield return this.Wait(75);
                }
                    

                yield break;
            }
            
            public class TinyBullet : Bullet
            {
                public TinyBullet(bool fart) : base("reversible", false, false, false)
                {
                    Fart = fart;
                }
                public override IEnumerator Top()
                {
                    this.Projectile.spriteAnimator.Play();
                    if (Fart == true)
                    {
                        this.ChangeSpeed(new Brave.BulletScript.Speed(0, SpeedType.Absolute), 90); yield return this.Wait(420); base.Vanish(false);
                    }
                    yield break;
                }
                private bool Fart;
            }
        }
    }
}
