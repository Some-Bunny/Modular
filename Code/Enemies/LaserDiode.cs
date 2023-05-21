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
    public class LaserDiode : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "LaserDiode_MDLR";

        public static void BuildPrefab()
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Laser Diode", guid, StaticCollections.Enemy_Collection, "laser_diode_idle_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<LaserDiodeBehavior>();
                companion.aiActor.knockbackDoer.weight = 50;
                companion.aiActor.MovementSpeed = 0.8f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(33f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.SetIsFlying(true, "Gamemode: Creative");
                companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(33f, null, false);
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
                    ManualOffsetX = 0,
                    ManualOffsetY = 7,
                    ManualWidth = 20,
                    ManualHeight = 26,
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
                    ManualOffsetY = 7,
                    ManualWidth = 20,
                    ManualHeight = 26,
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
                        "laserdiode_idle"
                    },
                    Prefix = "laserdiode_idle"
                };
                aiAnimator.MoveAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.Single,
                    Flipped = new DirectionalAnimation.FlipType[1],
                    AnimNames = new string[]
                    {
                        "laserdiode_idle"
                    },
                    Prefix = "laserdiode_idle"
                };

                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserDiodeAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "pre_attack_1", new string[] { "laserdiode_preattack1" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "attack_1", new string[] { "laserdiode_attack1" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "postattack_1", new string[] { "laserdiode_postattack1" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "pre_attack_2", new string[] { "laserdiode_preattack2" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "attack_2", new string[] { "laserdiode_attack2" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "postattack_2", new string[] { "laserdiode_postattack2" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "death", new Dictionary<int, string> {
                    { 0, "Play_ENM_hammer_target_01" },
                    { 2, "Play_ENM_hammer_target_01" },
                    { 4, "Play_ENM_hammer_target_01" },
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
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(LaserAttack)),
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
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(PewPew)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 4f,
                        CooldownVariance = 1,
                        InitialCooldown = 0f,
                        TellAnimation = "laserdiode_preattack1",
                        PostFireAnimation = "laserdiode_postattack1",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                    },
                    NickName = "Pewpew Attack"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Brapp)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 3f,
                        CooldownVariance = 3,
                        InitialCooldown = 0f,
                        TellAnimation = "pre_attack_2",
                        FireAnimation = "attack_2",
                        PostFireAnimation = "postattack_2",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                    },
                    NickName = "Brapp Attack"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0f,
                    Behavior = new CustomBeholsterLaserBehavior() {
                    InitialCooldown = 0f,
                    firingTime = 3.5f,
                    AttackCooldown = 0f,
                    Cooldown = 0,
                    CooldownVariance = 0,
                    RequiresLineOfSight = true,
                    UsesCustomAngle = true,
                    RampHeight = 14,
                    firingType = CustomBeholsterLaserBehavior.FiringType.TOWARDS_PLAYER,
                    chargeTime = 1f,
                    UsesBaseSounds = false,
                    AdditionalHeightOffset = 11,
                    EnemyChargeSound = "Play_BOSS_omegaBeam_charge_01",
                    LaserFiringSound = "Play_ENM_deathray_shot_01",
                    StopLaserFiringSound = "Stop_ENM_deathray_loop_01",
                    ChargeAnimation = "pre_attack_2",
                    FireAnimation = "attack_2",
                    PostFireAnimation = "postattack_2",
                    hurtsOtherHealthhavers = false,
                    beamSelection = ShootBeamBehavior.BeamSelection.Specify,
                    specificBeamShooters = new List<AIBeamShooter2>() { beamShooter1 },
                    trackingType = CustomBeholsterLaserBehavior.TrackingType.Follow,
                    DoesSpeedLerp = true,
                    InitialStartingSpeed = 0,
                    TimeToStayAtZeroSpeedAt = 0.5f,
                    TimeToReachFullSpeed = 1f,
                    LocksFacingDirection = false,
                    maxTurnRate = 1,
                    maxUnitForCatchUp = 2f,
                    minDegreesForCatchUp = 45,
                    minUnitForCatchUp = 0.1f,
                    minUnitForOvershoot = 1,
                    turnRateAcceleration = 1,
                    unitCatchUpSpeed = 1,
                    unitOvershootSpeed = 1,
                    unitOvershootTime = 0.25f,
                    degreeCatchUpSpeed = 180,
                    useDegreeCatchUp = companion.transform,
                    useUnitCatchUp = true,
                    useUnitOvershoot = true,
                    GlobalCooldown = 0,
                    ShootPoint = m_CachedGunAttachPoint.transform,
                    BulletScript = new CustomBulletScriptSelector(typeof(NormalAttack)),
                    Bap = false
                    },
                    NickName = "Laser Attack Dummy"
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
                mat2.SetColor("_EmissiveColor", new Color32(255, 211, 214, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;


                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundLarge"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                Game.Enemies.Add("mdlr:laser_diode", companion.aiActor);
            }
        }




        public class LaserDiodeBehavior : BraveBehaviour
        {
            public GlobalMessageRadio.MessageContainer container;
            public bool isCaller = false;

            private void Start()
            {
                container = GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "LaserDiodeLaserSync", "DeathStun" }, OnMessageRecieved);
                this.aiActor.healthHaver.OnPreDeath += (v2) =>
                {
                    GlobalMessageRadio.BroadcastMessage("DeathStun");
                };
            }
            public void OnMessageRecieved(GameObject obj, string message)
            {
                if (this.aiActor.isActiveAndEnabled == true)
                {
                    if (message == "LaserDiodeLaserSync")
                    {
                        this.behaviorSpeculator.Interrupt();
                        this.behaviorSpeculator.AttackCooldown = 0f;

                        for (int i = 0; i < this.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Count; i++)
                        {
                            var aa = (this.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors[i]);
                            var behavior = aa.Behavior as BasicAttackBehavior;
                            if (aa.NickName == "Laser Attack Dummy")
                            {
                                behavior.Cooldown = 0;
                                behavior.MinRange = -1f;
                                behavior.Range = 1000f;
                                behavior.RequiresLineOfSight = false;
                                (behavior as CustomBeholsterLaserBehavior).Bap = true;
                                aa.Probability = 1;
                            }
                            else if (aa.NickName == "MainLaserAttack")
                            {
                                behavior.Cooldown += 9;
                                aa.Probability = 0;
                            }
                            else
                            {
                                aa.Probability = 0;
                            }
                        }
                    }
                    if (message == "DeathStun")
                    {
                        this.aiActor.behaviorSpeculator.Stun(1.5f);
                        this.behaviorSpeculator.Interrupt();
                        //this.aiActor.knockbackDoer.ApplyKnockback(Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 2), 15);
                    }
                }
                    
            }
        }

        public class Brapp : Script
        {
            public override IEnumerator Top()
            {
                bool b = BraveUtility.RandomBool();
                for (int i = 0; i < 4; i++)
                {
                    this.PostWwiseEvent("Play_BOSS_RatMech_Hop_01", null);
                    for (int e = 0; e < 4; e++)
                    {
                        base.Fire(new Direction((90 * e) + (i * 20), DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new Rotater(b));
                    }
                    yield return this.Wait(45);
                }
                yield break;
            }
            public class Rotater : Bullet
            {
                public Rotater(bool rotation) : base("poundSmall", false, false, false)
                {
                    Rot = rotation;
                }
                public override IEnumerator Top()
                {
                    this.ChangeDirection(new Brave.BulletScript.Direction(Rot ? 120 : -120, DirectionType.Relative), 240);
                    yield break;
                }
                private bool Rot;
            }
        }
        public class PewPew : Script
        {
            public override IEnumerator Top()
            {
                
                for (int i = 0; i < 4; i++)
                {
                    this.PostWwiseEvent("Play_BOSS_lichC_zap_01", null);
                    base.Fire(new Direction(UnityEngine.Random.Range(-45, 45), DirectionType.Aim, -1f), new Speed(7, SpeedType.Absolute), new BasicBullet());
                    yield return this.Wait(45);
                }
                yield break;
            }
            public class BasicBullet : Bullet
            {
                public BasicBullet() : base("poundLarge", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.ChangeDirection(new Brave.BulletScript.Direction(BraveUtility.RandomBool() == true ? 60 : -60, DirectionType.Relative), 120);
                    while (this.IsEnded == false || this.Destroyed == false)
                    {
                        base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(0, SpeedType.Absolute), new TinyBullet());
                        yield return this.Wait(1);
                    }
                    yield break;
                }
            }
            public class TinyBullet : Bullet
            {
                public TinyBullet() : base("poundSmall", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    yield return this.Wait(20);
                    base.Vanish(true);
                    yield break;
                }
            }
        }

        public class NormalAttack : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < this.BulletBank.aiActor.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Count; i++)
                {
                    var aa = (this.BulletBank.aiActor.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors[i]);
                    var behavior = aa.Behavior as BasicAttackBehavior;
                    if (aa.NickName == "Laser Attack Dummy")
                    {
                        aa.Probability = 0;
                        (behavior as CustomBeholsterLaserBehavior).Bap = false;
                    }
                    else
                    {
                        aa.Probability = 1;
                    }
                }
                if (this.BulletBank.aiActor.GetComponent<LaserDiodeBehavior>().isCaller == true)
                {
                    this.BulletBank.aiActor.GetComponent<LaserDiodeBehavior>().isCaller = false;
                    yield return this.Wait(30);
                    for (int f = 0; f < 2; f++)
                    {
                        bool b = BraveUtility.RandomBool();
                        for (int e = 0; e < 4; e++)
                        {
                            base.Fire(new Direction((90 * e), DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new Rotater(b));
                        }
                        b = !b;
                        for (int e = 0; e < 4; e++)
                        {
                            base.Fire(new Direction((90 * e), DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new Rotater(b));
                        }
                        yield return this.Wait(75);
                    }
                }
                


                yield break;
            }
        }
        public class LaserAttack : Script
        {
            public override IEnumerator Top()
            {
                this.BulletBank.aiActor.GetComponent<LaserDiodeBehavior>().isCaller = true;
                GlobalMessageRadio.BroadcastMessage("LaserDiodeLaserSync");
                yield break;
            }
        }
    }
}

