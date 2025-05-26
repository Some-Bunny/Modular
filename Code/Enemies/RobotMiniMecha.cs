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
    public class RobotMiniMecha : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "RobotMiniMecha_MDLR";
        public static GameObject KaboomObject;

        public static void BuildPrefab()
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Mini Mech Robot", guid, StaticCollections.Enemy_Collection, "gunnermech_die_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<MechaBehavior>();
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
                companion.aiActor.healthHaver.ForceSetCurrentHealth(52f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.PathableTiles = CellTypes.FLOOR;

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(52f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();


                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 2,
                    ManualOffsetY = 1,
                    ManualWidth = 15,
                    ManualHeight = 18,
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
                    ManualOffsetX = 2,
                    ManualOffsetY = 1,
                    ManualWidth = 15,
                    ManualHeight = 18,
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
                        "idle_back_right",
                        "idle_front_right",
                        "idle_front",
                        "idle_front_left",
                        "idle_back_left"
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
                        "move_back_right",
                        "move_front_right",
                        "move_front",
                        "move_front_left",
                        "move_back_left"
                    },
                    Prefix = "move"
                };

                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("MechaAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;
                companion.aiActor.AssignedCurrencyToDrop = 0;

                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "awake", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);

                
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "death", new Dictionary<int, string> {
                    { 1, "Play_ENM_plasma_burst_01" },
                });
                

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;


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
                    Probability = 1f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(PewPew)),
                        LeadAmount = 0f,
                        AttackCooldown = 1f,
                        Cooldown = 4f,
                        CooldownVariance = 1,
                        InitialCooldown = 0f,

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
                mat2.SetColor("_EmissiveColor", new Color32(121, 234, 255, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;



                AIBulletBank.Entry sentryEntry = EnemyBuildingTools.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80").bulletBank.GetBullet("directedfire"), "TurretBurst", "Play_TurretShot",
                (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects
                , false);

                companion.aiActor.bulletBank.Bullets.Add(sentryEntry);

                Game.Enemies.Add("mdlr:mini_mech", companion.aiActor);


                KaboomObject = new GameObject("Explode_VFX");
                FakePrefab.DontDestroyOnLoad(KaboomObject);
                FakePrefab.MarkAsFakePrefab(KaboomObject);
                var tk2d = KaboomObject.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.VFX_Collection;
                tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("deathsplosion_001"));
                var tk2dAnim = KaboomObject.AddComponent<tk2dSpriteAnimator>();
                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("DeathsplosionAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("kaboom");
                tk2dAnim.playAutomatically = true;
                tk2d.usesOverrideMaterial = true;
                tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                tk2d.renderer.material.SetFloat("_EmissivePower", 30);
                tk2d.renderer.material.SetFloat("_EmissiveColorPower", 30);

                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
                companion.encounterTrackable.journalData = new JournalEntry();
                companion.encounterTrackable.EncounterGuid = "mdlr:mini_mech";
                companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                companion.encounterTrackable.journalData.SuppressKnownState = true;
                companion.encounterTrackable.journalData.IsEnemy = true;
                companion.encounterTrackable.journalData.SuppressInAmmonomicon = true;
                companion.encounterTrackable.ProxyEncounterGuid = "";
                companion.encounterTrackable.journalData.AmmonomiconSprite = "";
                companion.encounterTrackable.journalData.enemyPortraitSprite = null;//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\hmprimesheet.png");
                Module.Strings.Enemies.Set("#MINIMEGH_NAME", "Modular Mini-Mech");
                companion.encounterTrackable.journalData.PrimaryDisplayName = "#MINIMEGH_NAME";
                companion.encounterTrackable.journalData.NotificationPanelDescription = "#MODULARPRIME_SD";
                companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#MODULARPRIME_LD";
                StaticReferenceManager.AllHealthHavers.Remove(companion.healthHaver);

            }
        }




        public class MechaBehavior : BraveBehaviour
        {
            public GlobalMessageRadio.MessageContainer container;
            private void Start()
            {
                this.aiActor.healthHaver.OnPreDeath += (vector) =>
                {
                    var Obj = this.aiActor.PlayEffectOnActor(KaboomObject, new Vector3(0,0));
                };
            }
        }

        public class PewPew : Script
        {
            public override IEnumerator Top()
            {
                float angle = (this.GetPredictedTargetPosition(1, 10) - this.BulletBank.transform.PositionVector2()).ToAngle();
                this.BulletBank.aiActor.aiAnimator.LockFacingDirection = true;
                for (int i = 0; i < 300; i++)
                {
                    angle = Mathf.MoveTowardsAngle(angle, (this.GetPredictedTargetPositionExact(1, 25) - this.BulletBank.transform.PositionVector2()).ToAngle(), Mathf.Max(1.4f, 45 - i));
                    this.BulletBank.aiActor.aiAnimator.FacingDirection = angle;
                    base.Fire(new Direction(angle + UnityEngine.Random.Range(-6 - (i / 90), 6 + (i / 90)), DirectionType.Absolute, -1f), new Speed(6 + (i / 10), SpeedType.Absolute), new Bullet("TurretBurst"));
                    yield return this.Wait(Mathf.Max(4, 30 - i));
                }
                this.BulletBank.aiActor.aiAnimator.LockFacingDirection = false;
                yield break;
            }
           
        }
    }
}
