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
    public class Slapper : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "Slapper_MDLR";

        public static void BuildPrefab()
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Slapper", guid, StaticCollections.Enemy_Collection, "slapper_idle_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<SlapperBehavior>();
                companion.aiActor.knockbackDoer.weight = 1500000;
                companion.aiActor.MovementSpeed = 0f;
                companion.aiActor.healthHaver.PreventAllDamage = false;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(9f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.PathableTiles = CellTypes.PIT | CellTypes.FLOOR;
                companion.gameObject.GetOrAddComponent<TeleportationImmunity>();

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(9f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();


                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 3,
                    ManualOffsetY = 1,
                    ManualWidth = 10,
                    ManualHeight = 9,
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
                    ManualOffsetX = 3,
                    ManualOffsetY = 1,
                    ManualWidth = 10,
                    ManualHeight = 9,
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


                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("SlapperAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;
                companion.aiActor.AssignedCurrencyToDrop = 0;


                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "awaken", new string[] { "awaken" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "slap", new string[] { "slap" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "pre_slap", new string[] { "pre_slap" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "die", new string[] { "die" }, new DirectionalAnimation.FlipType[0]);

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "awaken", new Dictionary<int, string> {
                    {7, "Play_BOSS_mineflayer_cute_01" },
                    {9, "Play_OBJ_mine_set_01" }
                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "pre_slap", new Dictionary<int, string> {
                    {7, "Play_BOSS_cyborg_charge_01" },
                });

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "die", new Dictionary<int, string> {
                    {7, "Play_ENM_rubber_blast_01" },
                });

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
                companion.aiActor.reinforceType = ReinforceType.SkipVfx;


                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;

                var shootpoint = new GameObject("fuck");
                shootpoint.transform.parent = companion.transform;
                shootpoint.transform.position = companion.sprite.WorldBottomLeft + new Vector2(0.5f, 0.3125f);
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
                        BulletScript = new CustomBulletScriptSelector(typeof(LaserAttack)),
                        LeadAmount = 0f,
                        AttackCooldown = 0f,
                        Cooldown = 0f,
                        CooldownVariance = 0,
                        InitialCooldown = 0,
                        //TellAnimation = "laserdiode_preattack1",
                        //PostFireAnimation = "laserdiode_postattack1",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                    },
                    NickName = "Main_Attack"
                    },//0
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                    Probability = 0f,
                    Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(NormalAttack)),
                        LeadAmount = 0f,
                        AttackCooldown = 0.5f,
                        Cooldown = 0f,
                        CooldownVariance = 0,
                        InitialCooldown = 0f,
                        TellAnimation = "pre_slap",
                        PostFireAnimation = "slap",
                        RequiresLineOfSight = true,
                        MultipleFireEvents = true,
                        Uninterruptible = false,
                        ChargeTime = 1.4f,
                    },
                    NickName = "Slap"
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
                mat2.SetColor("_EmissiveColor", new Color32(255, 221, 206, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;

                companion.aiActor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"));
                Game.Enemies.Add("mdlr:slapper", companion.aiActor);


                if (companion.GetComponent<EncounterTrackable>() != null)
                {
                    UnityEngine.Object.Destroy(companion.GetComponent<EncounterTrackable>());
                }
                companion.encounterTrackable = companion.gameObject.AddComponent<EncounterTrackable>();
                companion.encounterTrackable.journalData = new JournalEntry();
                companion.encounterTrackable.EncounterGuid = "mdlr:slapper";
                companion.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
                companion.encounterTrackable.journalData.SuppressKnownState = true;
                companion.encounterTrackable.journalData.IsEnemy = true;
                companion.encounterTrackable.journalData.SuppressInAmmonomicon = true;
                companion.encounterTrackable.ProxyEncounterGuid = "";
                companion.encounterTrackable.journalData.AmmonomiconSprite = "";
                companion.encounterTrackable.journalData.enemyPortraitSprite = null;//ItemAPI.ResourceExtractor.GetTextureFromResource("Planetside\\Resources\\Ammocom\\hmprimesheet.png");
                Module.Strings.Enemies.Set("#SLAPPER_NAME", "Reciever");
                companion.encounterTrackable.journalData.PrimaryDisplayName = "#SLAPPER_NAME";
                companion.encounterTrackable.journalData.NotificationPanelDescription = "#MODULARPRIME_SD";
                companion.encounterTrackable.journalData.AmmonomiconFullEntry = "#MODULARPRIME_LD";
            }
        }




        public class SlapperBehavior : BraveBehaviour
        {
            public GlobalMessageRadio.MessageContainer container;
            public bool isCaller = false;

            private void Start()
            {
                container = GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "SlapperSync" }, OnMessageRecieved);
                this.aiActor.healthHaver.OnPreDeath += (v2) =>
                {
                    Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 0.1f,0.5f, 10, 0.5f);
                };
            }
            public void OnMessageRecieved(GameObject obj, string message)
            {
                if (this.aiActor.isActiveAndEnabled == true)
                {
                    if (message == "SlapperSync")
                    {
                        this.behaviorSpeculator.AttackCooldown = 0f;

                        for (int i = 0; i < this.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors.Count; i++)
                        {
                            var aa = (this.behaviorSpeculator.AttackBehaviorGroup.AttackBehaviors[i]);
                            var behavior = aa.Behavior as BasicAttackBehavior;
                            if (aa.NickName == "Slap")
                            {
                                behavior.Cooldown = 0;
                                behavior.MinRange = -1f;
                                behavior.Range = 1000f;
                                behavior.RequiresLineOfSight = false;
                                aa.Probability = 1;
                            }
                            else
                            {
                                aa.Probability = 0;
                            }
                        }
                    }
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
                    if (aa.NickName == "Slap")
                    {
                        aa.Probability = 0;
                    }
                    else
                    {
                        aa.Probability = 1;
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    base.Fire(new Direction(i * 60, DirectionType.Aim, -1f), new Speed(9f, SpeedType.Absolute), new TinyBullet());
                }
                yield break;
            }
            public class TinyBullet : Bullet
            {
                public TinyBullet() : base("reversible", false, false, false)
                {

                }
                public override IEnumerator Top()
                {
                    this.Projectile.spriteAnimator.Play();
                    yield return this.Wait(30);
                    base.ChangeSpeed(new Brave.BulletScript.Speed(4, SpeedType.Absolute), 90);
                    yield break;
                }
            }
        }
        public class LaserAttack : Script
        {
            public override IEnumerator Top()
            {
                GlobalMessageRadio.BroadcastMessage("SlapperSync");
                yield break;
            }
        }
    }
}

