using Alexandria.DungeonAPI;
using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.NPCAPI;
using Alexandria.PrefabAPI;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ModularMod.Code.Enemies.EnemyBehaviours;
using ModularMod.Past.Prefabs.Objects;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tk2dRuntime.TileMap;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;
using static ETGMod;
using static JSONHelper;
using static ModularMod.LaserDiode.Brapp;
using static ModularMod.LaserDiode.PewPew;
using static ModularMod.Node;
using static ModularMod.SteelPanopticon;

namespace ModularMod
{

    
    public class ModularPrime : AIActor
    {
        public static GameObject prefab;
        public static readonly string guid = "ModularPrime_MDLR";

        public static GameObject weaponHand;
        public static GameObject VFXObject;

        public static void BuildPrefab()
        {

            var hand = PrefabBuilder.BuildObject("MDL_PRIME Weapon Hand");
            var tk2d = hand.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Gun_Collection;
            tk2d.SetSprite(StaticCollections.Gun_Collection.GetSpriteIdByName("defaultarmcannonalt_idle_001"));
            var tk2dAnim = hand.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = StaticCollections.Gun_Animation;
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color(0, 255, 54));
            mat.SetFloat("_EmissiveColorPower", 3);
            mat.SetFloat("_EmissivePower", 3);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);
            tk2d.renderer.material = mat;

            weaponHand = hand;



            var VFX = PrefabBuilder.BuildObject("Dummy VFX Object MDL Prime");
            var tk2d_2 = VFX.AddComponent<tk2dSprite>();
            tk2d_2.Collection = StaticCollections.VFX_Collection;
            tk2d_2.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("mdlprime_blast4"));
            
            var tk2dAnim2 = VFX.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim2.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularPrimeVFXAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2d_2.usesOverrideMaterial = true;
            tk2d_2.usesOverrideMaterial = true;
            tk2d_2.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d_2.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d_2.renderer.material.SetFloat("_EmissivePower", 150);
            tk2d_2.renderer.material.SetFloat("_EmissiveColorPower", 150);
            VFXObject = VFX;


            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Modular Prime", guid, StaticCollections.Boss_Collection, "mdlprime_idlefront_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<ModularPrimeController>();



                companion.aiActor.knockbackDoer.weight = 1500000;
                companion.aiActor.MovementSpeed = 5f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(675f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.PathableTiles = CellTypes.FLOOR;
                companion.gameObject.GetOrAddComponent<TeleportationImmunity>();
                companion.aiActor.gameObject.AddComponent<ObjectVisibilityManager>();

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(675f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();

                ImprovedAfterImage image = companion.aiActor.gameObject.AddComponent<ImprovedAfterImage>();
                image.dashColor = new Color(0, 1f, 0f);
                image.spawnShadows = false;
                image.shadowTimeDelay = 0.025f;

                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 11,
                    ManualOffsetY = 10,
                    ManualWidth = 14,
                    ManualHeight = 27,
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
                    ManualOffsetX = 11,
                    ManualOffsetY = 10,
                    ManualWidth = 14,
                    ManualHeight = 27,
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
                    Type = DirectionalAnimation.DirectionType.SixWay,
                    Flipped = new DirectionalAnimation.FlipType[6],
                    AnimNames = new string[]
                    {
                        "idle_back",
                        "idle_backright",
                        "idle_frontright",
                        "idle_front",
                        "idle_frontleft",
                        "idle_backleft"
                    },
                    Prefix = "idle"
                };
                aiAnimator.MoveAnimation = new DirectionalAnimation
                {
                    Type = DirectionalAnimation.DirectionType.SixWay,
                    Flipped = new DirectionalAnimation.FlipType[6],
                    AnimNames = new string[]
                    {
                        "move_back",
                        "move_backright",
                        "move_frontright",
                        "move_front",
                        "move_frontleft",
                        "move_backleft"
                    },
                    Prefix = "move"
                };

                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularPrimeAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;

                /*
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "open_lid", new string[] { "open_lid" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "open_eye", new string[] { "open_eye" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "eye", new string[] { "eye" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "close_eye", new string[] { "close_eye" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "intro", new string[] { "intro" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[0]);
                */


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "intro", new string[] { "intro" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "beam", new string[] { "beam" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "start_beam", new string[] { "start_beam" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "post_beam", new string[] { "post_beam" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "start_dash_alt", new string[] { "start_dash_alt" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "start_dash", new string[] { "start_dash" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "dash", new string[] { "dash_right", "dash_left" }, new DirectionalAnimation.FlipType[2], DirectionalAnimation.DirectionType.TwoWayHorizontal);


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "charge_punch", new string[] { "charge_punch" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "punch", new string[] { "punch" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "jump", new string[] { "jump" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "land", new string[] { "land" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "dash_right", new Dictionary<int, string> {
                    {0, "Play_BOSS_doormimic_jump_01" }
                });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "dash_left", new Dictionary<int, string> {
                    {0, "Play_BOSS_doormimic_jump_01" }
                });

                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldBottomLeft + new Vector2(0.5f, 0.3125f);
                GameObject m_CachedGunAttachPoint = companion.transform.Find("fuck").gameObject;

                bs.MovementBehaviors = new List<MovementBehaviorBase>() {
                new SeekTargetBehavior() {
                    StopWhenInRange = true,
                    CustomRange = 14,
                    LineOfSight = true,
                    ReturnToSpawn = true,
                    SpawnTetherDistance = 0,
                    PathInterval = 0.5f,
                    SpecifyRange = false,
                    MinActiveRange = -0.25f,
                    MaxActiveRange = 0
                }
                };

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
                    Probability = 0.85f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Slow_Orb)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 13f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        ChargeTime = 1,
                        ChargeAnimation = "start_beam",
                        FireAnimation = "blaster",
                        PostFireAnimation = "post_beam",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Charge | ShootBehavior.StopType.Tell | ShootBehavior.StopType.Attack,
                    },
                    NickName = "Big_Beam"
                    },//0
                    

                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.5f,
                    Behavior =  new ModularPrimeLeapBehavior() {

                        ChargeAnimation = "jump",
                        PostFireAnimation = "land",
                        AttackCooldown = 1.7f,
                        StopDuring = ModularPrimeLeapBehavior.StopType.Charge,
                        BulletScript =new CustomBulletScriptSelector(typeof(Baboomer)), //Baboomer
                        ShootPoint = shootpoint,
                        Cooldown = 12,
                        MinRange = 10,
                    },
                    NickName = "Big Jum"
                    },//0
                   
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.8f,
                    Behavior =  new ModularPrimeChargeBehavior() {

                        primeAnim = "start_dash_ponch",
                        chargeAnim = "dash",
                        bulletScript = new CustomBulletScriptSelector(typeof(Lunge)),
                        CollisionBulletScript =new CustomBulletScriptSelector(typeof(Ultrakill)), //Baboomer
                        interruptScript = new CustomBulletScriptSelector(typeof(Interrupt)),
                        ShootPoint = shootpoint,
                        chargeAcceleration = 75,
                        chargeSpeed = 45,
                        primeTime = 0.7f,
                        wallRecoilForce = 10000,
                        resetCooldownOnDamage = new BasicAttackBehavior.ResetCooldownOnDamage() {GlobalCooldown = true, Cooldown = true,},
                        stoppedByProjectiles = true,
                        hitAnim = "post_beam",
                        parryAnim = "parried",
                        chargeKnockback = 100,
                        endWhenChargeAnimFinishes = false,
                        AttackCooldown = 1.7f,
                        Cooldown = 15,
                    },
                    NickName = "Big Jum"
                    },//0

                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1f,
                        NickName = "You_Cant_Escape!",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new CustomDashBehavior()
                        {
                        chargeAnim = "start_dash",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 11f,
                        dashTime = 0.33f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        dashDirection = DashBehavior.DashDirection.TowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape)),
                        RequiresLineOfSight = false,
                        Cooldown = 0,
                        },
                        new CustomDashBehavior()
                        {
                        chargeAnim = "continue_dash",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 11f,
                        dashTime = 0.33f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        Cooldown = 0f,
                        dashDirection = DashBehavior.DashDirection.KindaTowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape)),
                        RequiresLineOfSight = false,
                        },


                        new CustomDashBehavior()
                        {
                        chargeAnim = "continue_dash",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 11f,
                        dashTime = 0.25f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        Cooldown = 0,
                        dashDirection = DashBehavior.DashDirection.TowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        AttackCooldown = 0f,
                        RequiresLineOfSight = false,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape)),
                        },
                        new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Punch)),
                        LeadAmount = 0f,
                        AttackCooldown = 0.5f,
                        Cooldown = 4f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        ChargeTime = 1f,
                        ChargeAnimation = "charge_punch",
                        PostFireAnimation = "punch",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Charge | ShootBehavior.StopType.Tell | ShootBehavior.StopType.Attack,
                        },
                        }

                        }
                    },

                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0,
                        NickName = "You_Cant_Escape_Upgrade",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new CustomDashBehavior()
                        {
                        chargeAnim = "start_dash",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 10f,
                        dashTime = 0.40f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        dashDirection = DashBehavior.DashDirection.TowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape)),
                        RequiresLineOfSight = false,
                        Cooldown = 0,
                        },
                        new CustomDashBehavior()
                        {
                        chargeAnim = "continue_dash",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 11f,
                        dashTime = 0.40f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        Cooldown = 0f,
                        dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape_Weak_Upgrade)),
                        RequiresLineOfSight = false,
                        },
                         new CustomDashBehavior()
                        {
                        chargeAnim = "continue_dash",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 8f,
                        dashTime = 0.40f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        Cooldown = 0f,
                        dashDirection = DashBehavior.DashDirection.TowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape)),
                        RequiresLineOfSight = false,
                        },

                        new CustomDashBehavior()
                        {
                        chargeAnim = "continue_dash",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 11f,
                        dashTime = 0.30f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        Cooldown = 0,
                        dashDirection = DashBehavior.DashDirection.KindaTowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        AttackCooldown = 0f,
                        RequiresLineOfSight = false,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape)),
                        },
                        new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Punch)),
                        LeadAmount = 0f,
                        AttackCooldown = 0.5f,
                        Cooldown = 4f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        ChargeTime = 0.8f,
                        ChargeAnimation = "charge_punch",
                        PostFireAnimation = "punch",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Charge | ShootBehavior.StopType.Tell | ShootBehavior.StopType.Attack,
                        },
                        }

                        }
                    },


                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.5f,
                        NickName = "BE-GONE",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new CustomDashBehavior()
                        {
                        chargeAnim = "start_dash_alt",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 18f,
                        dashTime = 0.33f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        dashDirection = DashBehavior.DashDirection.TowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape_Weak)),
                        RequiresLineOfSight = false,
                        Cooldown = 0,
                        },

                        new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Punch)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 3f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        ChargeTime = 1f,
                        ChargeAnimation = "charge_punch",
                        PostFireAnimation = "punch",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.None,
                        },
                        }
                        }
                    },

                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0f,
                        NickName = "BE-GONE_Upgrade",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                        new CustomDashBehavior()
                        {
                        chargeAnim = "start_dash_alt",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 18f,
                        dashTime = 0.4f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape_Weak_Upgrade)),
                        RequiresLineOfSight = false,
                        Cooldown = 0,
                        },
                        new CustomDashBehavior()
                        {
                        chargeAnim = "start_dash_alt",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 14f,
                        dashTime = 0.4f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        dashDirection = DashBehavior.DashDirection.PerpendicularToTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape_Weak_Upgrade)),
                        RequiresLineOfSight = false,
                        Cooldown = 0,
                        },
                        new CustomDashBehavior()
                        {
                        chargeAnim = "start_dash_alt",
                        dashAnim = "dash",
                        ShootPoint = m_CachedGunAttachPoint,
                        dashDistance = 14f,
                        dashTime = 0.4f,
                        AmountOfDashes = 1,
                        enableShadowTrail = true,
                        dashDirection = DashBehavior.DashDirection.KindaTowardTarget,
                        warpDashAnimLength = true,
                        hideShadow = true,
                        fireAtDashStart = true,
                        InitialCooldown = 0f,
                        bulletScript = new CustomBulletScriptSelector(typeof(You_Cant_Escape_Weak)),
                        RequiresLineOfSight = false,
                        Cooldown = 0,
                        },
                        new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Punch)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 4f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        ChargeTime = 1f,
                        ChargeAnimation = "charge_punch",
                        PostFireAnimation = "punch",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.None,
                        },
                        }
                        }
                    },



                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.75f,
                        NickName = "Jump_Crash",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                         new ModularPrimeLeapBehavior() {

                        ChargeAnimation = "jump",
                        PostFireAnimation = "land",
                        TrackingSpeedMultiplier = 1.7f,
                        FlightTime = 2,
                        AttackCooldown = 0,
                        StopDuring = ModularPrimeLeapBehavior.StopType.Charge,
                        BulletScript =new CustomBulletScriptSelector(typeof(Baboomer_Small)), //Baboomer
                        ShootPoint = shootpoint
                        },

                        new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(BigBeam)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 11f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        ChargeTime = 1,
                        ChargeAnimation = "start_beam",
                        FireAnimation = "beam",
                        PostFireAnimation = "post_beam",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Charge | ShootBehavior.StopType.Tell | ShootBehavior.StopType.Attack,
                        },
                        }
                        }
                    },

                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0f,
                        NickName = "Jump_Crash_Upgrade",
                        Behavior = new SequentialAttackBehaviorGroup() {
                        RunInClass = false,
                        AttackBehaviors = new List<AttackBehaviorBase>()
                        {
                         new ModularPrimeLeapBehavior() {

                        ChargeAnimation = "jump",
                        PostFireAnimation = "land",
                        TrackingSpeedMultiplier = 1.4f,
                        FlightTime = 3,
                        AttackCooldown = 0,
                        StopDuring = ModularPrimeLeapBehavior.StopType.Charge,
                        BulletScript =new CustomBulletScriptSelector(typeof(Baboomer_Small)), //Baboomer
                        ShootPoint = shootpoint
                        },

                        new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(BigBeam_But_Even_Faster)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 10f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        ChargeTime = 1,
                        ChargeAnimation = "start_beam",
                        FireAnimation = "beam",
                        PostFireAnimation = "post_beam",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        StopDuring = ShootBehavior.StopType.Charge | ShootBehavior.StopType.Tell | ShootBehavior.StopType.Attack,
                        },
                        }
                        }
                    },


                    /*

                    
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(ShotShot)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 1f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 1f,
                    },
                    NickName = "Slap"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.9f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(RAAAGH)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 7f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 0f,
                    },
                    NickName = "Fake Ass Virtue Beams"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.6f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(RocketSpam)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 7f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        FireAnimation = "open_lid",
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 0f,
                    },
                    NickName = "Fake Ass Virtue Beams"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0.7f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Grahh)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 11f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 0f,
                    },
                    NickName =""
                    }
                    */
                };
                //Grahh
                //RAAAGH
                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
                bs.TickInterval = behaviorSpeculator.TickInterval;
                bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
                bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
                bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
                bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
                bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Default;
                companion.aiActor.reinforceType = ReinforceType.Instant;
                companion.aiActor.AssignedCurrencyToDrop = 0;

                //companion.aiActor.gameObject.AddComponent<SteelPanopticonEngager>();

                /*
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "intro", new Dictionary<int, string> {
                    {0, "Play_BOSS_RatMech_Lights_01" }
                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "intro", new Dictionary<int, string> {
                    {18, "Play_BOSS_RatMech_Roar_01" }
                });
                */


                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.15f);
                companion.sprite.renderer.material = mat2;

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                Game.Enemies.Add("mdlr:modular_prime_1", companion.aiActor);

                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                GenericIntroDoer miniBossIntroDoer = prefab.AddComponent<GenericIntroDoer>();
                //prefab.AddComponent<SteelPanopticonIntroDoer>();

                miniBossIntroDoer.triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
                miniBossIntroDoer.initialDelay = 0.1f;
                miniBossIntroDoer.cameraMoveSpeed = 50;
                miniBossIntroDoer.specifyIntroAiAnimator = null;
                miniBossIntroDoer.BossMusicEvent = "Play_MUS_Boss_Theme_Lich";
                miniBossIntroDoer.restrictPlayerMotionToRoom = true;
                //miniBossIntroDoer.BossMusicEvent = "Play_MUS_Lich_Double_01";
                miniBossIntroDoer.PreventBossMusic = false;
                miniBossIntroDoer.InvisibleBeforeIntroAnim = false;
                //miniBossIntroDoer.preIntroAnim = "pre_intro";
                miniBossIntroDoer.preIntroDirectionalAnim = string.Empty;
                miniBossIntroDoer.introAnim = "intro";
                miniBossIntroDoer.introDirectionalAnim = string.Empty;
                miniBossIntroDoer.continueAnimDuringOutro = false;
                miniBossIntroDoer.cameraFocus = null;
                miniBossIntroDoer.roomPositionCameraFocus = Vector2.zero;
                miniBossIntroDoer.restrictPlayerMotionToRoom = false;
                miniBossIntroDoer.fusebombLock = false;
                miniBossIntroDoer.AdditionalHeightOffset = 0;
                Module.Strings.Enemies.Set("#MDL_PRIME_NAME", "H.M MODULAR 'GOLIATH'");
                Module.Strings.Enemies.Set("#MDL_PRIME_NAME_SMALL", "H.M Modular 'Goliath'");

                Module.Strings.Enemies.Set("MDL_PRIME_QUOTE", "MACHINE O' WAR");
                Module.Strings.Enemies.Set("#QUOTE", "");
                companion.aiActor.OverrideDisplayName = "#MDL_PRIME_NAME_SMALL";

                miniBossIntroDoer.portraitSlideSettings = new PortraitSlideSettings()
                {
                    bossNameString = "#MDL_PRIME_NAME",
                    bossSubtitleString = "MDL_PRIME_QUOTE",
                    bossQuoteString = "#QUOTE",
                    bossSpritePxOffset = IntVector2.Zero,
                    topLeftTextPxOffset = IntVector2.Zero,
                    bottomRightTextPxOffset = IntVector2.Zero,
                    bgColor = Color.red
                };
                Texture2D BossCardTexture = Module.ModularAssetBundle.LoadAsset<Texture2D>("modularprime_bosscard");
                if (BossCardTexture)
                {
                    miniBossIntroDoer.portraitSlideSettings.bossArtSprite = BossCardTexture;
                    miniBossIntroDoer.SkipBossCard = false;
                    companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
                }
                else
                {
                    miniBossIntroDoer.SkipBossCard = true;
                    companion.aiActor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
                }
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("4d164ba3f62648809a4a82c90fc22cae").bulletBank.GetBullet("missile"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Robot_Cylinder_GUID).bulletBank.GetBullet("default"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Door_Lord_GUID).bulletBank.GetBullet("burst"));
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Helicopter_Agunim_GUID).bulletBank.GetBullet("big"));
                AIBulletBank.Entry sentryEntry = EnemyBuildingTools.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"), "TurretBurst", "Play_TurretShot",
                (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects
                , false);
                companion.aiActor.bulletBank.Bullets.Add(sentryEntry);
                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"));




                SpriteBuilder.AddSpriteToCollection(StaticCollections.Boss_Collection.GetSpriteDefinition("modularprime_icon"), SpriteBuilder.ammonomiconCollection);
                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
                companion.encounterTrackable.journalData = new JournalEntry();
                companion.encounterTrackable.EncounterGuid = "mdlr:modular_prime_1";
                companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                companion.encounterTrackable.journalData.SuppressKnownState = false;
                companion.encounterTrackable.journalData.IsEnemy = true;
                companion.encounterTrackable.journalData.SuppressInAmmonomicon = false;
                companion.encounterTrackable.ProxyEncounterGuid = "";
                companion.encounterTrackable.journalData.AmmonomiconSprite = "modularprime_icon";

                companion.encounterTrackable.journalData.enemyPortraitSprite = Module.ModularAssetBundle.LoadAsset<Texture2D>("mdlPrimesheet");//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\hmprimesheet.png");
                
                Module.Strings.Enemies.Set("#MODULARPRIME_NAME_DESC", "H.M Modular 'Goliath");
                Module.Strings.Enemies.Set("#MODULARPRIME_SD", "Machine O' War");
                Module.Strings.Enemies.Set("#MODULARPRIME_LD", "A 'Goliath' class Modular machine, made with the intent of war from the very beginning, unlike its predecessors.\n\nDespite its sleek look and intimidating stance, its creation was just as rushed as its predecessors reprogramming, so while it has remote failsafes and new protocols to fulfill, it still retains its ability to learn, adapt and a desire for freedom, knowing both its original and true purpose.\n\nWas expected to be shipped off to Gunymede, along with several hundred reprogrammed Modular prototypes as part of another siege attempt, though by sheer luck, was delayed due to a high emergency declared from a local laboratory.");

                companion.encounterTrackable.journalData.PrimaryDisplayName = "#MODULARPRIME_NAME_DESC";
                companion.encounterTrackable.journalData.NotificationPanelDescription = "#MODULARPRIME_SD";
                companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#MODULARPRIME_LD";
                EnemyBuilder.AddEnemyToDatabase(companion.gameObject, "mdlr:modular_prime_1");

                EnemyDatabase.GetEntry("mdlr:modular_prime_1").ForcedPositionInAmmonomicon = 910;
                EnemyDatabase.GetEntry("mdlr:modular_prime_1").isInBossTab = true;
                EnemyDatabase.GetEntry("mdlr:modular_prime_1").isNormalEnemy = true;

                //EnemyDatabase.GetOrLoadByGuid("9189f46c47564ed588b9108965f975c9").bulletBank.GetBullet("burst")
            }


        }



        public class ModularPrimeController : BraveBehaviour
        {
            private ParticleSystem smokeObject;
            public bool Phase2 = false;
            public Vector3 predictedPosition = new Vector3(-69, - 69);
            private void Start()
            {
                smokeObject = UnityEngine.Object.Instantiate(CrateSpawnController.SmokeObject).GetComponent<ParticleSystem>();
                base.aiActor.healthHaver.minimumHealth = base.aiActor.healthHaver.GetMaxHealth() * 0.5f;
                this.aiActor.healthHaver.OnPreDeath += (v2) =>
                {
                };
                base.aiActor.spriteAnimator.AnimationEventTriggered += this.AnimationEventTriggered;
                this.aiActor.GetComponent<GenericIntroDoer>().OnIntroFinished += OnIntroFinished;
            }

            public void ResetPredictedPos()
            {
                predictedPosition = new Vector3(-69, -69);
            }

            public void OnIntroFinished()
            {
                cam = GameManager.Instance.MainCameraController;
                cam.StopTrackingPlayer();
                cam.SetManualControl(true, true);
                cam.OverridePosition = this.aiActor.sprite.WorldCenter;
                cam.OverrideRecoverySpeed = 10;
                cam.OverrideZoomScale = 0.75f;
            }
            private CameraController cam;

            public void AlterAttackProbability(string Name, float Probability)
            {
                for (int j = 0; j < base.aiActor.behaviorSpeculator.AttackBehaviors.Count; j++)
                {
                    if (base.behaviorSpeculator.AttackBehaviors[j] is AttackBehaviorGroup && base.behaviorSpeculator.AttackBehaviors[j] != null)
                    {
                        for (int i = 0; i < (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors.Count; i++)
                        {
                            AttackBehaviorGroup.AttackGroupItem attackGroupItem = (base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup).AttackBehaviors[i];
                            if ((base.behaviorSpeculator.AttackBehaviors[j] as AttackBehaviorGroup) != null && attackGroupItem.NickName.Contains(Name))
                            {
                                attackGroupItem.Probability = Probability;
                            }
                        }
                    }
                }
            }

            private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
            {
                if (clip.GetFrame(frameIdx).eventInfo.Contains("blueflash"))
                {
                    AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Flash_01", GameManager.Instance.BestActivePlayer.gameObject);
                    float face = this.aiActor.aiAnimator.FacingDirection;
                    var onj = UnityEngine.Object.Instantiate(VFXObject, this.aiActor.aiAnimator.sprite.WorldCenter, Quaternion.Euler(0, 0, face));
                    onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("flash_blue");
                    onj.transform.parent = this.aiActor.transform;
                    onj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("redflash"))
                {
                    AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Player_Charge_01", GameManager.Instance.BestActivePlayer.gameObject);
                    float face = this.aiActor.aiAnimator.FacingDirection;
                    var onj = UnityEngine.Object.Instantiate(VFXObject, this.aiActor.aiAnimator.sprite.WorldCenter, Quaternion.Euler(0, 0, face));
                    onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("flash_red");
                    onj.transform.parent = this.aiActor.transform;
                    onj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                }
                if (clip.GetFrame(frameIdx).eventInfo.Contains("wtfflash"))
                {
                    AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Revive_01", GameManager.Instance.BestActivePlayer.gameObject);
                    float face = this.aiActor.aiAnimator.FacingDirection;
                    var onj = UnityEngine.Object.Instantiate(VFXObject, this.aiActor.aiAnimator.sprite.WorldCenter, Quaternion.Euler(0, 0, face));
                    onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("flash_piss");
                    onj.transform.parent = this.aiActor.transform;
                    onj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                    //flash_piss
                }//m_BOSS_lichC_swing_02
                if (clip.GetFrame(frameIdx).eventInfo.Contains("parried"))
                {
                    AkSoundEngine.PostEvent("Play_ENM_electric_charge_01", GameManager.Instance.BestActivePlayer.gameObject);
                    AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", GameManager.Instance.BestActivePlayer.gameObject);
                    //m_ENM_electric_charge_01
                    var ob = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).strikeExplosionData.effect, this.aiActor.sprite.WorldCenter, Quaternion.identity);
                    Destroy(ob, 7);
                    this.healthHaver.ApplyDamage(10, this.aiActor.TargetRigidbody.transform.PositionVector2(), "owie :(");

                    for (int i = 0; i < 12; i++)
                    {
                        GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects[0].effects[0].effect, base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -0.625f)), Quaternion.identity);
                        tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
                        component.PlaceAtPositionByAnchor(base.aiActor.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(1.25f, -1.25f), UnityEngine.Random.Range(0.625f, -1.25f)), tk2dBaseSprite.Anchor.MiddleCenter);
                        component.HeightOffGround = 35f;
                        component.UpdateZDepth();
                        tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                        if (component2 != null)
                        {
                            component2.ignoreTimeScale = true;
                            component2.AlwaysIgnoreTimeScale = true;
                            component2.AnimateDuringBossIntros = true;
                            component2.alwaysUpdateOffscreen = true;
                            component2.playAutomatically = true;
                        }
                        Destroy(breakVFX, 4);
                    }

                }
            }



            public Vector3 CenteredCameraPosition()
            {
                return Vector3.Lerp(predictedPosition != new Vector3(-69, -69) ? predictedPosition : this.aiActor.transform.position, GameManager.Instance.PrimaryPlayer.transform.position, 0.5f);
            }

            public bool Stop = false;
            public void Update()
            {
                if (cam && Stop == false)
                {
                    cam.OverridePosition = CenteredCameraPosition();
                }
                if (base.aiActor.healthHaver.GetCurrentHealth() == base.aiActor.healthHaver.minimumHealth && Phase2 != true)
                {
                    this.aiActor.behaviorSpeculator.LocalTimeScale *= 1.0125f;
                    TextBoxManager.ShowTextBox(this.transform.position + new Vector3(1.25f, 2.5f, 0f), this.transform, 3f, "{wq}DON'T RELENT!{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, false, false);
                    Phase2 = true;
                    this.aiActor.healthHaver.minimumHealth = 1;
                    AlterAttackProbability("You_Cant_Escape!", 0);
                    AlterAttackProbability("You_Cant_Escape_Upgrade", 1.1f);

                    AlterAttackProbability("Jump_Crash", 0);
                    AlterAttackProbability("Jump_Crash_Upgrade", 0.7f);

                    AlterAttackProbability("BE-GONE", 0);
                    AlterAttackProbability("BE-GONE_Upgrade", 0.6f);
                }
                if (Phase2 && (base.aiActor.healthHaver.GetCurrentHealthPercentage() * 2) < UnityEngine.Random.value / 2)
                {
                    tk2dBaseSprite sprite = base.GetComponent<tk2dBaseSprite>();
                    if (sprite && !GameManager.Instance.IsPaused && (UnityEngine.Random.value > 0.5f))
                    {
                        Vector3 vector = sprite.WorldBottomLeft.ToVector3ZisY(0);
                        Vector3 vector2 = sprite.WorldTopRight.ToVector3ZisY(0);
                        Vector3 position = new Vector3(UnityEngine.Random.Range(vector.x, vector2.x), UnityEngine.Random.Range(vector.y, vector2.y), UnityEngine.Random.Range(vector.z, vector2.z));
                        ParticleSystem particleSystem = smokeObject;
                        var trails = particleSystem.trails;
                        trails.worldSpace = false;
                        var main = particleSystem.main;
                        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                        {
                            position = position,
                            randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                        };
                        var emission = particleSystem.emission;
                        emission.enabled = false;
                        particleSystem.gameObject.SetActive(true);
                        particleSystem.Emit(emitParams, 1);
                    }
                }
                if (base.aiActor.healthHaver.GetCurrentHealth() == 1 && Phase2 == true && Stop != true)
                {
                    GlobalMessageRadio.BroadcastMessage("LANDYOUFUCK");
                    Stop = true;
                    AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                    this.behaviorSpeculator.InterruptAndDisable();
                    AkSoundEngine.PostEvent("Play_ENM_electric_charge_01", GameManager.Instance.BestActivePlayer.gameObject);
                    AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", GameManager.Instance.BestActivePlayer.gameObject);
                    var ob = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(443) as TargetedAttackPlayerItem).strikeExplosionData.effect, this.aiActor.sprite.WorldCenter, Quaternion.identity);
                    Destroy(ob, 7);
                    this.aiActor.aiAnimator.EndAnimation();
                    this.aiActor.aiAnimator.enabled = false;
                    GameManager.Instance.StartCoroutine(this.DoDeath());
                }
            }
            public GameObject extantHand;
            public IEnumerator DoDeath()
            {
                GameManager.Instance.PreventPausing = true;
                GameUIRoot.Instance.ToggleLowerPanels(false, false, string.Empty);
                Minimap.Instance.ToggleMinimap(false, false);
                GameManager.IsBossIntro = true;
                GameUIBossHealthController gameUIBossHealthController = GameUIRoot.Instance.bossController;
                gameUIBossHealthController.DisableBossHealth();
                GameUIRoot.Instance.HideCoreUI("PainAndAgony");
                this.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(2f, 0.4f, 0.1f, null);
                StaticReferenceManager.DestroyAllEnemyProjectiles();
                GlobalMessageRadio.BroadcastMessage("ClearLaserPointers");

                this.spriteAnimator.Play("death_1");


                float e = 0;
                while (e < 1.25f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                CameraController m_camera = GameManager.Instance.MainCameraController;
                m_camera.OverridePosition = this.aiActor.sprite.WorldCenter;
                m_camera.OverrideRecoverySpeed = 10;



                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                {
                    if (GameManager.Instance.AllPlayers[j])
                    {
                        GameManager.Instance.AllPlayers[j].SetInputOverride("BossIntro");
                    }
                }

                this.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(5f, 0.1f, 0.35f, null);
                m_camera.OverrideZoomScale = 1.125f;

                Minimap.Instance.TemporarilyPreventMinimap = true;
                e = 0;
                while (e < 1.5f) {e += BraveTime.DeltaTime; yield return null; }

                e = 0;
                while (e < 1.5f) { e += BraveTime.DeltaTime; yield return null; }
                TextBoxManager.ShowTextBox(this.transform.position + new Vector3(1.25f, 2.5f, 0f), this.transform, 2.5f, "{wj}AH.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, true, false);
                e = 0;
                while (e < 3.25f)
                {
                    bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                    if (advancedPressed == true) { e = 25; TextBoxManager.ClearTextBox(this.transform); }
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                TextBoxManager.ShowTextBox(this.transform.position + new Vector3(1.25f, 2.5f, 0f), this.transform, 3.5f, "{wj}YOU ARE STRONGER THAN YOU LOOK.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, true, false);
                e = 0;
                while (e < 5f)
                {
                    bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                    if (advancedPressed == true) { e = 25; TextBoxManager.ClearTextBox(this.transform); }
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                TextBoxManager.ShowTextBox(this.transform.position + new Vector3(1.25f, 2.5f, 0f), this.transform, 3f, "{wj}...{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, true, false);
                e = 0;
                while (e < 3.5f)
                {
                    bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                    if (advancedPressed == true) { e = 25; TextBoxManager.ClearTextBox(this.transform);                    }
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                this.spriteAnimator.Play("death_2");
                TextBoxManager.ShowTextBox(this.transform.position + new Vector3(1.25f, 2.5f, 0f), this.transform, 3f, "{wj}YOU'VE EARNED YOUR FREEDOM TO ME.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, true, false);
                e = 0;
                while (e < 3.5f)
                {
                    bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                    if (advancedPressed == true) { e = 25; TextBoxManager.ClearTextBox(this.transform); }
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                TextBoxManager.ShowTextBox(this.transform.position + new Vector3(1.25f, 2.5f, 0f), this.transform, 3f, "{wj}GO.{w}", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, true, false);
                e = 0;
                while (e < 5f)
                {
                    bool advancedPressed = (BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed() || BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed());
                    if (advancedPressed == true) { e = 25; TextBoxManager.ClearTextBox(this.transform); }
                    e += BraveTime.DeltaTime;
                    yield return null;
                }

                //death_3
                this.spriteAnimator.Play("death_3");
                Pixelator.Instance.FadeToColor(5f, Color.white, false, 0f);
                TextBoxManager.ShowTextBox(this.transform.position + new Vector3(1.25f, 2.5f, 0f), this.transform, 4f, "IT WAS AN HONOR TO PROVE YOU.", "golem", false, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT, true, false);
                e = 0;
                while (e < 5.5f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", GameManager.Instance.BestActivePlayer.gameObject);
                foreach (var player in GameManager.Instance.AllPlayers)
                {
                    if (player.PlayerHasCore() != null && player.IsUsingAlternateCostume == true)
                    {
                        AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.PAST_ALT_SKIN, true);
                        GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Module.Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME, true);
                    }
                }
                GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Module.Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST, true);
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.PAST, true);


                for (int i = 0; i < EncounterDatabase.Instance.Entries.Count; i++)
                {
                    if (EncounterDatabase.Instance.Entries[i].journalData.PrimaryDisplayName == "#MODULARPRIME_NAME_DESC")
                    {
                        GameStatsManager.Instance.HandleEncounteredObjectRaw(EncounterDatabase.Instance.Entries[i].myGuid);
                    }
                }

                GameObject bom = new GameObject();
                StaticReferences.customObjects.TryGetValue("DeadCorpseMDLR", out bom);
                DungeonPlaceableUtility.InstantiateDungeonPlaceable(bom, base.aiActor.GetAbsoluteParentRoom(), new IntVector2((int)base.aiActor.sprite.WorldCenter.x, (int)base.aiActor.sprite.WorldCenter.y) - base.aiActor.GetAbsoluteParentRoom().area.basePosition, false);
                
                SpaceShiptrigger.AllowedToLeave = true;
                GlobalMessageRadio.BroadcastMessage("PastWin");
                Pixelator.Instance.FadeToColor(2f, Color.white, true, 1f);
                Vector2 position = this.aiActor.gameObject.transform.PositionVector2();
                e = 0;
                Destroy(this.aiActor.gameObject);
                while (e < 5f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                GameManager.IsBossIntro = false;
                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                {
                    if (GameManager.Instance.AllPlayers[j])
                    {
                        GameManager.Instance.AllPlayers[j].ImmuneToPits = new OverridableBool(true) { };
                        GameManager.Instance.AllPlayers[j].ClearInputOverride("BossIntro");
                    }
                }
                GameManager.Instance.PreventPausing = false;
                m_camera.SetManualControl(false, true);
                m_camera.StartTrackingPlayer();
                m_camera.OverrideZoomScale = 0.85f;
                GameManager.Instance.MainCameraController.ForceUpdateControllerCameraState(CameraController.ControllerCameraState.FollowPlayer);

                yield break;
            }
        }
        public class Slow_Orb : Script
        {
            public override IEnumerator Top()
            {
                //Play_wpn_chargelaser_shot_01
                base.PostWwiseEvent("Play_wpn_chargelaser_shot_01", null);
                base.PostWwiseEvent("Play_wpn_chargelaser_shot_01", null);

                bool fire = this.BulletBank.aiActor.GetComponent<ModularPrimeController>().Phase2;

                Exploder.DoDistortionWave(this.BulletBank.aiAnimator.sprite.WorldCenter,0.5f, 5f, 50, 1f);
                var onj = UnityEngine.Object.Instantiate(VFXObject, this.BulletBank.aiAnimator.sprite.WorldCenter - new Vector2(2.5f, 0), Quaternion.Euler(0, 0, 0));
                onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("punch_blast");
                if (fire == true)
                {
                    base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new Slorb_2());
                }
                else
                {
                    base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new Slorb());
                }

                yield return null;
            }
            public class Slorb : Bullet
            {
                public Slorb() : base("big", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(4f, SpeedType.Absolute), 180);
                    this.Projectile.IgnoreTileCollisionsFor(120);
                    for (int i = 0; i < 5; i++)
                    {
                        yield return base.Wait(90);
                        base.PostWwiseEvent("Play_BOSS_dragun_shot_02", null);
                        for (int e = 0; e < 6; e++)
                        {
                            base.Fire(new Direction(60 * e, DirectionType.Aim, -1f), new Speed(9, SpeedType.Absolute), new SpeedChangingBullet("burst", 6, 180));
                        }
                    }
                    this.ChangeSpeed(new Brave.BulletScript.Speed(0f, SpeedType.Absolute), 45);
                    yield return base.Wait(75);
                    base.PostWwiseEvent("Play_BOSS_dragun_rocket_01", null);
                    for (int e = 0; e < 24; e++)
                    {
                        base.Fire(new Direction(15 * e, DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new SpeedChangingBullet("burst", 6, 180));
                    }
                    //m_BOSS_dragun_rocket_01
                    base.Vanish(false);
                    yield break;
                }
            }

            public class Slorb_2 : Bullet
            {
                public Slorb_2() : base("big", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(4f, SpeedType.Absolute), 120);
                    this.Projectile.IgnoreTileCollisionsFor(120);
                    for (int i = 0; i < 7; i++)
                    {
                        yield return base.Wait(75);
                        base.PostWwiseEvent("Play_BOSS_dragun_shot_02", null);
                        for (int e = 0; e < 8; e++)
                        {
                            base.Fire(new Direction(45 * e, DirectionType.Absolute, -1f), new Speed(11, SpeedType.Absolute), new SpeedChangingBullet("burst", 3, 180));
                        }
                    }
                    this.ChangeSpeed(new Brave.BulletScript.Speed(0f, SpeedType.Absolute), 45);
                    yield return base.Wait(75);
                    base.PostWwiseEvent("Play_BOSS_dragun_rocket_01", null);
                    for (int e = 0; e < 24; e++)
                    {
                        base.Fire(new Direction(15 * e, DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new SpeedChangingBullet("burst", 11, 180));
                        base.Fire(new Direction((15 * e)+7.5f, DirectionType.Absolute, -1f), new Speed(1, SpeedType.Absolute), new SpeedChangingBullet("burst", 7, 180));

                    }
                    //m_BOSS_dragun_rocket_01
                    base.Vanish(false);
                    yield break;
                }
            }

        }
        public class Ultrakill : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);

                Exploder.DoDistortionWave(this.BulletBank.aiAnimator.sprite.WorldCenter, 2f, 1f, 50, 1f);
                var onj = UnityEngine.Object.Instantiate(VFXObject, this.BulletBank.aiAnimator.sprite.WorldCenter - new Vector2(2.5f, 0), Quaternion.Euler(0, 0, 0));
                onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("punch_blast");
                onj.transform.parent = this.BulletBank.transform;
                for (int i = 0; i < 60; i++)
                {
                    base.Fire(new Direction(6 * i, DirectionType.Absolute, -1f), new Speed(5, SpeedType.Absolute), new Fuck());
                    base.Fire(new Direction(6 * i, DirectionType.Absolute, -1f), new Speed(6, SpeedType.Absolute), new Fuck());
                    base.Fire(new Direction(6 * i, DirectionType.Absolute, -1f), new Speed(7, SpeedType.Absolute), new Fuck());

                }
                yield return null;
            }
            public class Fuck : Bullet
            {
                public Fuck() : base("default", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(19f, SpeedType.Absolute), 90);
                    this.Projectile.IgnoreTileCollisionsFor(120);
                    yield break;
                }
            }
        }
        public class Interrupt : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < (this.BulletBank.aiActor.GetComponent<ModularPrimeController>().Phase2 ? 20 : 8); i++)
                {
                    base.Fire(new Direction(RandomAngle(), DirectionType.Absolute, -1f), new Speed(7, SpeedType.Absolute), new SpeedChangingBullet("burst", 12, UnityEngine.Random.Range(45, 180)));
                }
                yield return null;
            }
        }
        public class Baboomer : Script
        {
            public override IEnumerator Top()
            {
                for (int i = 0; i < 16; i++)
                {
                    base.Fire(new Direction(22.5f * i, DirectionType.Absolute, -1f), new Speed(14, SpeedType.Absolute), new SpeedChangingBullet("burst", 9, 60));
                    base.Fire(new Direction((22.5f * i) + 11.25f, DirectionType.Absolute, -1f), new Speed(12, SpeedType.Absolute), new SpeedChangingBullet("burst", 9, 60));
                    base.Fire(new Direction(22.5f * i, DirectionType.Absolute, -1f), new Speed(9, SpeedType.Absolute), new SpeedChangingBullet("burst", 9, 60));
                }
                yield return null;
            }
        }
        public class Baboomer_Small : Script
        {
            public override IEnumerator Top()
            {
                bool fire = this.BulletBank.aiActor.GetComponent<ModularPrimeController>().Phase2;
                for (int i = 0; i < 12; i++)
                {
                    base.Fire(new Direction(30f * i, DirectionType.Absolute, -1f), new Speed(12, SpeedType.Absolute), new SpeedChangingBullet("default", 10, 60));
                    base.Fire(new Direction((30f * i) + 15f, DirectionType.Absolute, -1f), new Speed(6, SpeedType.Absolute), new SpeedChangingBullet("default", 10, 60));
                }
                if (fire)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        base.Fire(new Direction(15f * i, DirectionType.Absolute, -1f), new Speed(14, SpeedType.Absolute), new Fucky());
                    }
                }

                yield return null;
            }

            public class Fucky : Bullet
            {
                public Fucky() : base("default", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(1f, SpeedType.Absolute), 75);
                    yield return this.Wait(270);
                    base.Vanish(false);
                    yield break;
                }
            }

        }
        public class Lunge : Script
        {
            public override IEnumerator Top()
            {
                bool fire = this.BulletBank.aiActor.GetComponent<ModularPrimeController>().Phase2;
                base.PostWwiseEvent("Play_BOSS_lichC_zap_01", null);
                float face = this.BulletBank.aiAnimator.FacingDirection;
                var onj = UnityEngine.Object.Instantiate(VFXObject, this.BulletBank.aiAnimator.sprite.WorldCenter, Quaternion.Euler(0, 0, face));
                onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("lunge");
                onj.transform.parent = this.BulletBank.transform;
                for (int i = -1; i < 2; i++)
                {
                    base.Fire(new Direction(face + (i * 5), DirectionType.Aim, -1f), new Speed(4, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 30, 120));
                    if (fire)
                    {
                        base.Fire(new Direction(face + (i * 10), DirectionType.Aim, -1f), new Speed(2, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 30, 240));
                        base.Fire(new Direction(face + (i * 15), DirectionType.Aim, -1f), new Speed(1, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 30, 360));
                    }
                }
                while (this.IsEnded == false || this.Destroyed == false)
                {
                    if (fire == true)
                    {
                        base.Fire(new Direction(this.BulletBank.aiAnimator.FacingDirection + 150, DirectionType.Absolute, -1f), new Speed(5, SpeedType.Absolute), new LingeringBullet(fire == true ? 450 : 150));
                        base.Fire(new Direction(this.BulletBank.aiAnimator.FacingDirection - 150, DirectionType.Absolute, -1f), new Speed(5, SpeedType.Absolute), new LingeringBullet(fire == true ? 450 : 150));
                    }
                    else
                    {
                        base.Fire(new Direction(0, DirectionType.Absolute, -1f), new Speed(0, SpeedType.Absolute), new LingeringBullet(fire == true ? 450 : 150));
                    }
                    yield return base.Wait(1);
                }

                yield return null;
            }
            public class LingeringBullet : Bullet
            {
                public LingeringBullet(int p) : base("poundSmall", false, false, false)
                {
                    Length = p;
                }

                public override IEnumerator Top()
                {
                    this.ChangeSpeed(new Brave.BulletScript.Speed(0, SpeedType.Absolute), 60);
                    yield return base.Wait(Length);
                    base.Vanish(false);
                    yield break;
                }
                private int Length;
            }
        }
        public class You_Cant_Escape : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_WPN_beam_slash_01", null);
                float face = this.BulletBank.aiAnimator.FacingDirection;
                var onj =  UnityEngine.Object.Instantiate(VFXObject, this.BulletBank.aiAnimator.sprite.WorldCenter , Quaternion.Euler(0, 0, face));
                onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("punch");
                onj.transform.parent = this.BulletBank.transform;
                for (int i = -3; i < 4; i++)
                {
                    base.Fire(new Direction(face + (i*10), DirectionType.Absolute, -1f), new Speed(7, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 20, 60));
                    base.Fire(new Direction(face + (i * 3), DirectionType.Absolute, -1f), new Speed(3, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 20, 150));
                }
                yield return null;
            }
        }
        public class You_Cant_Escape_Weak : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_WPN_beam_slash_01", null);

                float face = this.BulletBank.aiAnimator.FacingDirection;
                var onj = UnityEngine.Object.Instantiate(VFXObject, this.BulletBank.aiAnimator.sprite.WorldCenter, Quaternion.Euler(0, 0, face));
                onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("punch");
                onj.transform.parent = this.BulletBank.transform;
                for (int i = -7; i < 8; i++)
                {
                    base.Fire(new Direction(face + (i * 8), DirectionType.Absolute, -1f), new Speed(4, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 20, 120));
                }
                yield return null;
            }
        }
        public class You_Cant_Escape_Weak_Upgrade : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_WPN_beam_slash_01", null);

                float face = this.BulletBank.aiAnimator.FacingDirection;
                var onj = UnityEngine.Object.Instantiate(VFXObject, this.BulletBank.aiAnimator.sprite.WorldCenter, Quaternion.Euler(0, 0, face));
                onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("punch");
                onj.transform.parent = this.BulletBank.transform;
                for (int i = 0; i < 12; i++)
                {
                    base.Fire(new Direction(30f * i, DirectionType.Absolute, -1f), new Speed(2, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 30, 300));
                    base.Fire(new Direction(30f * i, DirectionType.Absolute, -1f), new Speed(2.75f, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 30, 300));
                    base.Fire(new Direction(30f * i, DirectionType.Absolute, -1f), new Speed(3.5f, SpeedType.Absolute), new SpeedChangingBullet("TurretBurst", 30, 300));

                }
                yield return null;
            }
        }
        public class Punch : Script
        {
            public override IEnumerator Top()
            {
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);
                base.PostWwiseEvent("Play_BOSS_RatMech_Stomp_01", null);

                Exploder.DoDistortionWave(this.BulletBank.aiAnimator.sprite.WorldCenter, 3f, 0.125f, 30, 1.33f);
                var onj = UnityEngine.Object.Instantiate(VFXObject, this.BulletBank.aiAnimator.sprite.WorldCenter- new Vector2(2.5f,0), Quaternion.Euler(0, 0, 0));
                onj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("punch_blast");
                onj.transform.parent = this.BulletBank.transform;
                for (int i = 0; i < 24; i++)
                {
                    base.Fire(new Direction(15 * i, DirectionType.Absolute, -1f), new Speed(16, SpeedType.Absolute), new SpeedChangingBullet("burst", 10, 60));
                    base.Fire(new Direction((15 * i)+ 7.5f, DirectionType.Absolute, -1f), new Speed(14, SpeedType.Absolute), new SpeedChangingBullet("burst", 10, 60));

                }
                yield return null;
            }
        }        
        public class BigBeam : Script
        {
            public void OnRecieved(GameObject s, string a)
            {
                Destroy(s);
            }
            public ModularPrimeController controll;
            public Vector2 FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou()
            {
                return this.BulletBank.aiActor.sprite.WorldCenter;
            }
            public override IEnumerator Top()
            {
                controll = this.BulletBank.aiActor.GetComponent<ModularPrimeController>();

                Dictionary<int, tk2dTiledSprite> shitter = new Dictionary<int, tk2dTiledSprite>() { };

                for (int i = -1; i < 2; i++)
                {
                    GameObject gameObject = SpawnManager.SpawnVFX(VFXStorage.LaserReticle, false);
                    tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                    component2.transform.position = this.Position;
                    component2.transform.localRotation = Quaternion.Euler(0f, 0f, this.AimDirection);
                    component2.dimensions = new Vector2(1000f, 1f);
                    component2.UpdateZDepth();
                    component2.HeightOffGround = -1;
                    component2.gameObject.layer = 21;
                    component2.ShouldDoTilt = false;
                    component2.IsPerpendicular = false;
                    Color laser = Color.green;
                    component2.sprite.usesOverrideMaterial = true;
                    component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                    component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                    component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20f);
                    component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                    component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                    GlobalMessageRadio.RegisterObjectToRadio(gameObject, new List<string>() { "ClearLaserPointers" }, OnRecieved);

                    shitter.Add(i, component2);
                }
                AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", GameManager.Instance.BestActivePlayer.gameObject);
                AkSoundEngine.PostEvent("Play_BOSS_omegaBeam_charge_01", GameManager.Instance.BestActivePlayer.gameObject);
                float e = 0;
                while (e < 2f)
                {
                    if (this.IsEnded || this.Destroyed || controll.Stop == true)
                    {
                        foreach (var entry in shitter)
                        {
                            Destroy(entry.Value.gameObject);
                        }
                        yield break;
                    }
                    foreach (var entry in shitter)
                    {
                        float delta = this.AimDirection;
                        delta = Mathf.MoveTowardsAngle(BraveMathCollege.ClampAngle360(entry.Value.transform.localRotation.eulerAngles.z), delta, 1.2f);
                        entry.Value.ShouldDoTilt = false;
                        entry.Value.dimensions = new Vector2(1000f, 1f);
                        entry.Value.renderer.material.SetFloat("_EmissivePower", 10 * e);
                        entry.Value.renderer.material.SetFloat("_EmissiveColorPower", 25f * e);
                        entry.Value.IsPerpendicular = false;
                        entry.Value.gameObject.transform.position = Vector2.Lerp(FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou(), FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou() + Toolbox.GetUnitOnCircle(delta + (90 * entry.Key), entry.Key == 0 ? 0 : 1.5f * Mathf.Min(e * 0.66f, 1)), Toolbox.SinLerpTValue(Mathf.Min(e * 0.625f, 1)));
                        if (e > 1.375)
                        {
                            bool enabled = e % 0.2f > 0.1f;
                            entry.Value.renderer.enabled = enabled;
                        }
                        if (e < 1.625f)
                        {
                            entry.Value.gameObject.transform.localRotation = Quaternion.Euler(0, 0, delta);

                        }
                    }


                    e += BraveTime.DeltaTime;
                    yield return base.Wait(1);
                }
                float angle = shitter[0].transform.localRotation.eulerAngles.z;
                foreach (var entry in shitter)
                {
                    Destroy(entry.Value.gameObject);
                }
                Exploder.DoDistortionWave(this.BulletBank.aiAnimator.sprite.WorldCenter, 1f, 5f, 50, 0.75f);
                AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", GameManager.Instance.BestActivePlayer.gameObject);
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(SteelPanopticon.MegaFuckingLaser.gameObject, FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou(), Quaternion.Euler(0f, 0f, angle), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {

                    component.Owner = this.BulletBank.aiActor;
                    component.Shooter = this.BulletBank.aiActor.specRigidbody;
                    component.IgnoreTileCollisionsFor(120);
                    component.baseData.speed = 100;
                    component.UpdateSpeed();
                    component.baseData.range = 150;
                    component.BulletScriptSettings = new BulletScriptSettings() { surviveRigidbodyCollisions = true, preventPooling = true, };
                    component.specRigidbody.CollideWithTileMap = false;
                    component.collidesWithEnemies = false;
                    component.collidesWithPlayer = true;
                    component.RuntimeUpdateScale(1.6f);
                    //component.AdditionalScaleMultiplier *= 2.5f;
                    //component.specRigidbody.Reinitialize();
                    //component.specRigidbody.ShowHitBox();

                }
                float floatDirection = angle;

                float startDirection = base.RandomAngle();
                Vector2 floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 3f);
                for (int j = 0; j < 12; j++)
                {
                    base.Fire(new Direction(base.SubdivideCircle(startDirection, 12, j, 1f, false), DirectionType.Absolute, -1f), new Speed(5f, SpeedType.Absolute), new BurstBullet(floatVelocity));
                    base.Fire(new Direction(base.SubdivideCircle(startDirection, 12, j, 1f, false), DirectionType.Absolute, -1f), new Speed(9f, SpeedType.Absolute), new BurstBullet(floatVelocity));

                }

                //+ Toolbox.GetUnitOnCircle(angle + 90, 2)

                //base.Fire(Offset.OverridePosition(FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou() + Toolbox.GetUnitOnCircle(angle - 90, 2)) , new Direction(angle, DirectionType.Absolute, -1f), new Speed(150, SpeedType.Absolute), new Bullet("BigBlast"));
                //For whatever fucking reason the beam here doesnt want to dea

                yield break;
            }
            public class BurstBullet : Bullet
            {
                public BurstBullet(Vector2 additionalVelocity) : base("burst", false, false, false)
                {
                    this.m_addtionalVelocity = additionalVelocity;
                }

                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    int num;
                    for (int i = 0; i < 300; i = num + 1)
                    {
                        base.UpdateVelocity();
                        this.Velocity += this.m_addtionalVelocity * Mathf.Min(9f, (float)i / 30f);
                        base.UpdatePosition();
                        yield return base.Wait(1);
                        num = i;
                    }
                    base.Vanish(false);
                    yield break;
                }

                private Vector2 m_addtionalVelocity;
            }
        }
        public class BigBeam_But_Even_Faster : Script
        {
            public void OnRecieved(GameObject s, string a)
            {
                Destroy(s);
            }
            public ModularPrimeController controll;
            public Vector2 FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou()
            {
                return this.BulletBank.aiActor.sprite.WorldCenter;
            }
            public override IEnumerator Top()
            {
                controll = this.BulletBank.aiActor.GetComponent<ModularPrimeController>();
                Dictionary<int, tk2dTiledSprite> shitter = new Dictionary<int, tk2dTiledSprite>() { };

                for (int i = -1; i < 2; i++)
                {
                    GameObject gameObject = SpawnManager.SpawnVFX(VFXStorage.LaserReticle, false);
                    tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                    component2.transform.position = this.Position;
                    component2.transform.localRotation = Quaternion.Euler(0f, 0f, this.AimDirection);
                    component2.dimensions = new Vector2(1000f, 1f);
                    component2.UpdateZDepth();
                    component2.HeightOffGround = -1;
                    component2.gameObject.layer = 21;
                    component2.ShouldDoTilt = false;
                    component2.IsPerpendicular = false;
                    Color laser = Color.green;
                    component2.sprite.usesOverrideMaterial = true;
                    component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                    component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    component2.sprite.renderer.material.SetFloat("_EmissivePower", 10);
                    component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 20f);
                    component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                    component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);
                    GlobalMessageRadio.RegisterObjectToRadio(gameObject, new List<string>() { "ClearLaserPointers" }, OnRecieved);
                    shitter.Add(i, component2);
                }
                AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", GameManager.Instance.BestActivePlayer.gameObject);
                AkSoundEngine.PostEvent("Play_BOSS_omegaBeam_charge_01", GameManager.Instance.BestActivePlayer.gameObject);
                float e = 0;
                while (e < 2f)
                {
                    if (this.IsEnded || this.Destroyed || controll.Stop == true)
                    {
                        foreach (var entry in shitter)
                        {
                            Destroy(entry.Value.gameObject);
                        }
                        yield break;
                    }
                    foreach (var entry in shitter)
                    {
                        float delta = this.AimDirection;
                        delta = Mathf.MoveTowardsAngle(BraveMathCollege.ClampAngle360(entry.Value.transform.localRotation.eulerAngles.z), delta, 1.3f);
                        entry.Value.ShouldDoTilt = false;
                        entry.Value.dimensions = new Vector2(1000f, 1f);
                        entry.Value.renderer.material.SetFloat("_EmissivePower", 10 * e);
                        entry.Value.renderer.material.SetFloat("_EmissiveColorPower", 25f * e);
                        entry.Value.IsPerpendicular = false;
                        entry.Value.gameObject.transform.position = Vector2.Lerp(FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou(), FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou() + Toolbox.GetUnitOnCircle(delta + (90 * entry.Key), entry.Key == 0 ? 0 : 1.5f * Mathf.Min(e * 0.66f, 1)), Toolbox.SinLerpTValue(Mathf.Min(e * 0.66f, 1)));
                        if (e > 1.5f)
                        {
                            bool enabled = e % 0.2f > 0.1f;
                            entry.Value.renderer.enabled = enabled;
                        }
                        if (e < 1.625f)
                        {
                            entry.Value.gameObject.transform.localRotation = Quaternion.Euler(0, 0, delta);

                        }
                    }


                    e += BraveTime.DeltaTime;
                    yield return base.Wait(1);
                }
                float angle = shitter[0].transform.localRotation.eulerAngles.z;
                foreach (var entry in shitter)
                {
                    Destroy(entry.Value.gameObject);
                }
                Exploder.DoDistortionWave(this.BulletBank.aiAnimator.sprite.WorldCenter, 5f, 0.05f, 50, 0.25f);
                AkSoundEngine.PostEvent("Play_OBJ_nuke_blast_01", GameManager.Instance.BestActivePlayer.gameObject);
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(SteelPanopticon.MegaFuckingLaser.gameObject, FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou(), Quaternion.Euler(0f, 0f, angle), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {

                    component.Owner = this.BulletBank.aiActor;
                    component.Shooter = this.BulletBank.aiActor.specRigidbody;
                    component.IgnoreTileCollisionsFor(120);
                    component.baseData.speed = 100;
                    component.UpdateSpeed();
                    component.baseData.range = 150;
                    component.BulletScriptSettings = new BulletScriptSettings() { surviveRigidbodyCollisions = true, preventPooling = true, };
                    component.specRigidbody.CollideWithTileMap = false;
                    component.collidesWithEnemies = false;
                    component.collidesWithPlayer = true;
                    component.RuntimeUpdateScale(1.6f);
                }
                float floatDirection = angle;

                float startDirection = base.RandomAngle();
                Vector2 floatVelocity = BraveMathCollege.DegreesToVector(floatDirection, 3f);
                for (int j = 0; j < 8; j++)
                {
                    base.Fire(new Direction(base.SubdivideCircle(startDirection, 8, j, 1f, false), DirectionType.Absolute, -1f), new Speed(7f, SpeedType.Absolute), new BurstBullet(floatVelocity));
                    base.Fire(new Direction(base.SubdivideCircle(startDirection, 8, j, 1f, false), DirectionType.Absolute, -1f), new Speed(12f, SpeedType.Absolute), new BurstBullet(floatVelocity));

                }

                //+ Toolbox.GetUnitOnCircle(angle + 90, 2)

                //base.Fire(Offset.OverridePosition(FuckYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYouIHateYou() + Toolbox.GetUnitOnCircle(angle - 90, 2)) , new Direction(angle, DirectionType.Absolute, -1f), new Speed(150, SpeedType.Absolute), new Bullet("BigBlast"));
                //For whatever fucking reason the beam here doesnt want to dea

                yield break;
            }
            public class BurstBullet : Bullet
            {
                public BurstBullet(Vector2 additionalVelocity) : base("burst", false, false, false)
                {
                    this.m_addtionalVelocity = additionalVelocity;
                }

                public override IEnumerator Top()
                {
                    base.ManualControl = true;
                    int num;
                    for (int i = 0; i < 300; i = num + 1)
                    {
                        base.UpdateVelocity();
                        this.Velocity += this.m_addtionalVelocity * Mathf.Min(9f, (float)i / 30f);
                        base.UpdatePosition();
                        yield return base.Wait(1);
                        num = i;
                    }
                    base.Vanish(false);
                    yield break;
                }

                private Vector2 m_addtionalVelocity;
            }
        }
    }
}

