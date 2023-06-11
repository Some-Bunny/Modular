using Alexandria.BreakableAPI;
using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.PrefabAPI;
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
using static ModularMod.GlobalMessageRadio;
using static ModularMod.Nopticon.NopticonBehavior;

namespace ModularMod
{
    public class Nopticon : AIActor
    {
        public class NodeMinionController : BraveBehaviour
        {
            public AIActor Parent;
            private GameObject laser;

            public void Start()
            {
                bodyPartController = this.GetComponent<AdvancedBodyPartController>();
                bodyPartController.healthHaver.SetHealthMaximum(5);
                bodyPartController.healthHaver.ForceSetCurrentHealth(5);
                //Parent = bodyPartController.MainBody;

                bodyPartController.OnBodyPartPreDeath += (obj1, obj2, obj3) =>
                {
                    isFuckingDying = true;
                    if (laser)
                    {
                        Destroy(laser);
                    }
                    //SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, false);
                    var l = bodyPartController.MainBody.gameObject.GetComponent<NopticonBehavior>().SpawnedNodes;
                    bodyPartController.MainBody.gameObject.GetComponent<NopticonBehavior>().OhFuck();
                    if (l.Contains(this)) { l.Remove(this); }
                    foreach (var nodes in l)
                    {
                        nodes.healthHaver.FullHeal();
                    }


                    GameObject breakVFX = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(156) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects[0].effects[0].effect, base.sprite.WorldCenter, Quaternion.identity);
                    tk2dBaseSprite component = breakVFX.GetComponent<tk2dBaseSprite>();
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
                    Destroy(breakVFX, 2);
                    AkSoundEngine.PostEvent("Play_enm_mech_step_01", this.gameObject);

                };

                GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() {"Cast_Basic", "Cast_Burst", "Cast_Scatter", "Cast_Direct", "Cast_Crazy" }, OnMessageRecieved);
            }
            private bool isFuckingDying = false;

            public void Update()
            {
                if (isFuckingDying == true) { return; }
                if (laser == null)
                {
                    laser = UnityEngine.Object.Instantiate(Nopticon.VFX_Object_Tether, this.sprite.WorldCenter + new Vector2(0, 0.5f), Quaternion.identity);
                    tk2dTiledSprite a = laser.GetComponent<tk2dTiledSprite>();
                    a.dimensions = new Vector2(16 * Vector2.Distance(this.sprite.WorldCenter, bodyPartController.MainBody.sprite.WorldCenter + new Vector2(-0.125f, -0.125f)), 2f);
                    a.ShouldDoTilt = false;
                    laser.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));

                }
                tk2dTiledSprite component2 = laser.GetComponent<tk2dTiledSprite>();
                component2.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnAngle(bodyPartController.MainBody));
                component2.dimensions = new Vector2(16 * Vector2.Distance(this.sprite.WorldCenter, bodyPartController.MainBody.sprite.WorldCenter + new Vector2(-0.125f, -0.125f)), 2f);
                component2.UpdateZDepth();
                component2.transform.localPosition = new Vector3(0, 0).WithZ(1000);
                laser.transform.position = this.sprite.WorldCenter;
            }
            public float ReturnAngle(AIActor enemy)
            {
                if (enemy == null) { return 0; }
                return (enemy.sprite.WorldCenter - this.sprite.WorldCenter).ToAngle();
            }
            public override void OnDestroy()
            {
                base.OnDestroy();
            }


            public void MoveToPosition(Vector2 pos, float time = 1)
            {
                this.StartCoroutine(MoveTo(this.transform.PositionVector2(), pos, time));   
            }
            public void MoveToPositionInstant(Vector2 pos)
            {
                this.transform.position = pos;
            }

            public void ToggleOverrideAim(bool e, float facing = 0)
            {
                this.aiAnimator.LockFacingDirection = e;
                if (e == true)
                {
                    this.aiAnimator.FacingDirection = facing;
                }
            }

            public int ReturnNodeCount()
            {
                return bodyPartController.MainBody.GetComponent<NopticonBehavior>().SpawnedNodes.Count();
            }

            private IEnumerator MoveTo(Vector2 oldpos, Vector2 pos, float time = 1)
            {
                float ela = 0;
                float t = 0;
                while (ela < time)
                {
                    ela += BraveTime.DeltaTime;
                    t = ela / time;
                    this.transform.position = Vector2.Lerp(oldpos, pos, Toolbox.SinLerpTValue(t));
                    yield return null;
                }
                yield break;
            }


            public void ForceMessage(string message)
            {
                if (message == null) { return; }
                OnMessageRecieved(this.bodyPartController.MainBody.aiActor.gameObject, message);
            }

            public void OnMessageRecieved(GameObject obj, string message)
            {
                if (obj != this.bodyPartController.MainBody.aiActor.gameObject) { return; }
                if (message.Contains("Cast_"))
                {
                    BulletScriptSource bulletScriptSource = gameObject.GetOrAddComponent<BulletScriptSource>();
                    bulletScriptSource.BulletManager = base.GetComponent<AIBulletBank>();
                    bulletScriptSource.BulletScript = ReturnScript(message);
                    bulletScriptSource.Initialize();
                }
            }

            public static CustomBulletScriptSelector ReturnScript(string s)
            {
                if (s == ("Cast_Burst")) { return new CustomBulletScriptSelector(typeof(Shot)); }
                if (s == ("Cast_Basic")) { return new CustomBulletScriptSelector(typeof(BasicBitchShot)); }
                if (s == ("Cast_Scatter")) { return new CustomBulletScriptSelector(typeof(BlastBlast)); }
                if (s == ("Cast_Direct")) { return new CustomBulletScriptSelector(typeof(LineBlast)); }
                return new CustomBulletScriptSelector(typeof(Shot));
            }

            public class Shot : Script
            {
                public int Return()
                {
                    return this.BulletBank.GetComponent<NodeMinionController>().ReturnNodeCount();
                }

                public override IEnumerator Top()
                {
                    int Rep = Return() > 6 ? 1 : 7 - Return();

                    for (int i = 0; i < 6; i++)
                    {
                        float aim = (this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter - this.BulletBank.GetComponent<NodeMinionController>().bodyPartController.MainBody.sprite.WorldCenter).ToAngle();
                        this.PostWwiseEvent("Play_WPN_looper_shot_01", null);
                        for (int e = 0; e < Rep; e++)
                        {
                            base.Fire(Offset.OverridePosition(this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter), new Direction(aim + ((360/Rep)* e), DirectionType.Absolute, -1f), new Speed(1f + i, SpeedType.Absolute), new SpeedChangingBullet("poundLarge", 15, 120));
                        }
                    }
                    yield break;
                }
            }

            public class LineBlast : Script
            {
                public int Return()
                {
                    return this.BulletBank.GetComponent<NodeMinionController>().ReturnNodeCount();
                }

                public override IEnumerator Top()
                {
                    this.PostWwiseEvent("Play_WPN_grasshopper_impact_01", (PickupObjectDatabase.GetById(180) as Gun).gunSwitchGroup);
                    base.Fire(Offset.OverridePosition(this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter), new Direction(this.BulletBank.aiAnimator.FacingDirection, DirectionType.Absolute, -1f), new Speed(0f, SpeedType.Absolute), new SpeedChangingBullet("default", 35, 90));
                    yield break;
                }
            }


            public class BlastBlast : Script
            {
                public int Return()
                {
                    return this.BulletBank.GetComponent<NodeMinionController>().ReturnNodeCount();
                }
                public override IEnumerator Top()
                {
                    for (int i = 0; i < 120 / Return(); i++)
                    {
                        this.PostWwiseEvent("Play_ITM_Crisis_Stone_Impact_01",null);
                        float aim = (this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter - this.BulletBank.GetComponent<NodeMinionController>().bodyPartController.MainBody.sprite.WorldCenter).ToAngle();
                        base.Fire(Offset.OverridePosition(this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter), new Direction(aim, DirectionType.Absolute, -1f), new Speed(3f, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 15, 120));
                        yield return this.Wait(Return() * 2);
                    }
                    yield break;
                }
            }
            public class BasicBitchShot : Script
            {
                public int Return()
                {
                    return this.BulletBank.GetComponent<NodeMinionController>().ReturnNodeCount();
                }
                public override IEnumerator Top()
                {
                    int Rep = Return() > 6 ? 1 : 7 - Return();
                    for (int e = 0; e < Rep; e++)
                    {
                        this.PostWwiseEvent("Play_WPN_plasmacell_shot_01", (PickupObjectDatabase.GetById(156) as Gun).gunSwitchGroup);
                        for (int i = 0; i < 3; i++)
                        {
                            base.Fire(Offset.OverridePosition(this.BulletBank.GetComponent<tk2dBaseSprite>().WorldCenter), new Direction(0, DirectionType.Aim, -1f), new Speed(3f + i, SpeedType.Absolute), new SpeedChangingBullet("poundSmall", 20, 100));
                        }
                        yield return this.Wait(Return() * 6);
                    }


                    yield break;
                }
            }
            public AdvancedBodyPartController bodyPartController;
        }

        private static GameObject VFX_Object_Tether;

        public static void BuildNode()
        {
            GameObject Obj = PrefabBuilder.BuildObject("Nopticon Eye");
            var tk2d = Obj.AddComponent<tk2dSprite>();

            tk2d.Collection = StaticCollections.Enemy_Collection;
            tk2d.SetSprite(StaticCollections.Enemy_Collection.GetSpriteIdByName("towernode_idle_001"));
            var tk2dAnim = Obj.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.sprite.usesOverrideMaterial = true;
            Material Handmat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            Handmat.mainTexture = tk2dAnim.sprite.renderer.material.mainTexture;
            Handmat.SetColor("_EmissiveColor", new Color32(255, 215, 205, 255));
            Handmat.SetFloat("_EmissiveColorPower", 1.55f);
            Handmat.SetFloat("_EmissivePower", 50);
            Handmat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            tk2dAnim.sprite.renderer.material = Handmat;


            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("TowerNodeAnimation").GetComponent<tk2dSpriteAnimation>();
            AIAnimator aiAnimatorBody = Obj.AddComponent<AIAnimator>();
            aiAnimatorBody.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.EightWay,
                Flipped = new DirectionalAnimation.FlipType[8],
                AnimNames = new string[]
                {
                        "idle_u",
                        "idle_ur",
                        "idle_r",
                        "idle_dr",
                        "idle_d",
                        "idle_dl",
                        "idle_l",
                        "idle_ul"
                }
            };

            AdvancedBodyPartController bodyPart = Obj.AddComponent<AdvancedBodyPartController>();
            bodyPart.Name = "Nopticon Node";
            bodyPart.Render = true;
            bodyPart.OverrideFacingDirection = true;
            bodyPart.faceTarget = true;
            bodyPart.faceTargetTurnSpeed = 120;

            Obj.AddComponent<NodeMinionController>();

            ImprovedAfterImage image = Obj.gameObject.AddComponent<ImprovedAfterImage>();
            image.dashColor = new Color(2, 0.1f, 0.1f);
            image.spawnShadows = true;
            image.shadowTimeDelay = 0.05f;

            SpeculativeRigidbody body = Obj.AddComponent<SpeculativeRigidbody>();

            body.CollideWithOthers = true;
            body.CollideWithTileMap = false;

            Obj.GetComponent<tk2dBaseSprite>().OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;

            body.PixelColliders = new List<PixelCollider>();
            body.PixelColliders.Add(new PixelCollider
            {

                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.EnemyHitBox,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 0,
                ManualOffsetY = 0,
                ManualWidth = 7,
                ManualHeight = 8,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,

            });
            HealthHaver healthHaver = Obj.AddComponent<HealthHaver>();
            healthHaver.SetHealthMaximum(10);
            healthHaver.ForceSetCurrentHealth(10);
            healthHaver.flashesOnDamage = true;
            Obj.GetOrAddComponent<GameActor>();

            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
            var bulletb = Obj.gameObject.AddComponent<AIBulletBank>();
            bulletb.Bullets = new List<AIBulletBank.Entry>()
            {
                EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"),
                EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundLarge"),
                EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("poundSmall"),
                EnemyDatabase.GetOrLoadByGuid("1bc2a07ef87741be90c37096910843ab").bulletBank.GetBullet("reversible"),
                EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Robot_Cylinder_GUID).bulletBank.GetBullet("default"),

            };

            /*
            foreach (var thing in EnemyDatabase.GetOrLoadByGuid(StaticGUIDs.Robot_Cylinder_GUID).bulletBank.Bullets)
            {
                ETGModConsole.Log(thing.Name);
            }
            */
            //SpriteOutlineManager.AddOutlineToSprite(tk2d.sprite, Color.green);
            //SpriteOutlineManager.AddOutlineToSprite(tk2d.sprite, Color.green, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            //SpriteOutlineManager.ToggleOutlineRenderers(tk2d.sprite, true);
            node = Obj;

            GameObject VFX = new GameObject("Red_Line Tether");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            VFX.SetActive(false);
            var tk2d2 = VFX.AddComponent<tk2dTiledSprite>();
            tk2d2.Collection = StaticCollections.VFX_Collection;
            tk2d2.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("beamlaserthing1"));
            var tk2dAnim2 = VFX.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim2.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("TetherAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim2.defaultClipId = tk2dAnim2.Library.GetClipIdByName("idle_red");
            tk2dAnim2.playAutomatically = true;
            tk2d2.usesOverrideMaterial = true;
            tk2d2.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d2.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d2.renderer.material.SetFloat("_EmissivePower", 50);
            tk2d2.renderer.material.SetFloat("_EmissiveColorPower", 50);
            tk2d2.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            VFX_Object_Tether = VFX;

        }
        private static GameObject node;


        private static GameObject prefab;
        public static readonly string guid = "Nopticon_MDLR";

        public static void BuildPrefab(int nodeCount)
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Nopticon", guid + "(" + nodeCount.ToString()+")", StaticCollections.Enemy_Collection, "towernopticon_death_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<NopticonBehavior>();
                companion.NodeCount = nodeCount;

                companion.aiActor.knockbackDoer.weight = 50000;
                companion.aiActor.MovementSpeed = 1f;
                companion.aiActor.healthHaver.PreventAllDamage = true;
                companion.aiActor.healthHaver.flashesOnDamage = false;

                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(90f);
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.PathableTiles = CellTypes.FLOOR;

                companion.aiActor.healthHaver.SetHealthMaximum(90f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();
                companion.aiActor.healthHaver.persistsOnDeath = true;


                EnemyBuildingTools.AddShadowToAIActor(companion.aiActor, EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject, new Vector2(0.75f, 0.25f), "shadowPos");


                companion.aiActor.specRigidbody.PixelColliders.Add(new PixelCollider
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                    CollisionLayer = CollisionLayer.EnemyCollider,
                    IsTrigger = false,
                    BagleUseFirstFrameOnly = false,
                    SpecifyBagelFrame = string.Empty,
                    BagelColliderNumber = 0,
                    ManualOffsetX = 1,
                    ManualOffsetY = 1,
                    ManualWidth = 16,
                    ManualHeight = 35,
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
                    ManualOffsetX = 1,
                    ManualOffsetY = 1,
                    ManualWidth = 16,
                    ManualHeight = 35,
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


                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("NopticonAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);


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
                        BulletScript = new CustomBulletScriptSelector(typeof(Cast_Burst)),
                        AttackCooldown = 1f,
                        Cooldown = 3f,
                        CooldownVariance = 0.5f,
                        InitialCooldown = 1f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = true,
                        Uninterruptible = true,
                    },
                    NickName = "Cast_Burst"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1.5f,
                        Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Cast_Singles)),
                        AttackCooldown = 2f,
                        Cooldown = 5f,
                        CooldownVariance = 0.5f,
                        InitialCooldown = 1f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = true,
                        Uninterruptible = true,
                    },
                    NickName = "Cast_Basic"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 1f,
                        Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Cast_Beans)),
                        AttackCooldown = 2f,
                        Cooldown = 5f,
                        CooldownVariance = 0.5f,
                        InitialCooldown = 1f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = true,
                        Uninterruptible = true,
                    },
                    NickName = "Cast_Beans"
                    },
                    new AttackBehaviorGroup.AttackGroupItem()
                    {
                        Probability = 0.7f,
                        Behavior = new ShootBehavior() {
                        ShootPoint = m_CachedGunAttachPoint,
                        BulletScript = new CustomBulletScriptSelector(typeof(Cast_Scatter)),
                        AttackCooldown = 2f,
                        Cooldown = 7f,
                        CooldownVariance = 0.5f,
                        InitialCooldown = 1f,
                        RequiresLineOfSight = false,
                        MultipleFireEvents = true,
                        Uninterruptible = true,
                    },
                    NickName = "Cast_Scatter"
                    },
                };
                //Cast_Scatter
                bs.InstantFirstTick = behaviorSpeculator.InstantFirstTick;
                bs.TickInterval = behaviorSpeculator.TickInterval;
                bs.PostAwakenDelay = behaviorSpeculator.PostAwakenDelay;
                bs.RemoveDelayOnReinforce = behaviorSpeculator.RemoveDelayOnReinforce;
                bs.OverrideStartingFacingDirection = behaviorSpeculator.OverrideStartingFacingDirection;
                bs.StartingFacingDirection = behaviorSpeculator.StartingFacingDirection;
                bs.SkipTimingDifferentiator = behaviorSpeculator.SkipTimingDifferentiator;

                
                Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat2.mainTexture = companion.aiActor.sprite.renderer.material.mainTexture;
                mat2.SetColor("_EmissiveColor", new Color32(255, 215, 205, 255));
                mat2.SetFloat("_EmissiveColorPower", 1.55f);
                mat2.SetFloat("_EmissivePower", 100);
                mat2.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
                companion.sprite.renderer.material = mat2;



                Game.Enemies.Add("mdlr:nopticon" +"(" + nodeCount.ToString() + ")", companion.aiActor);

                GameObject VFX = new GameObject("VFX");
                FakePrefab.DontDestroyOnLoad(VFX);
                FakePrefab.MarkAsFakePrefab(VFX);
                var tk2d = VFX.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.VFX_Collection;
                tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("redwarn1"));
                var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("RedWarnAnimation").GetComponent<tk2dSpriteAnimation>();


                tk2d.usesOverrideMaterial = true;
                tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                tk2d.renderer.material.SetFloat("_EmissivePower", 10);
                tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);
                RedWarn = VFX;
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(tk2dAnim, "bigbeep", new Dictionary<int, string>()
                {
                    {0, "Play_ENM_hammer_target_01"},
                    {2, "Play_ENM_hammer_target_01"},
                    {4, "Play_ENM_hammer_target_01"},
                });
                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(tk2dAnim, "beepbeep", new Dictionary<int, string>()
                {
                    {0, "Play_OBJ_metroid_roll_01"},
                    {2, "Play_OBJ_metroid_roll_01"},
                    {4, "Play_OBJ_metroid_roll_01"},
                });
            }

        }
        public static GameObject RedWarn;





        public class NopticonBehavior : BraveBehaviour
        {
            public int NodeCount = 2;
            public List<NodeMinionController> SpawnedNodes = new List<NodeMinionController>();

            public bool IsNodedUp = false;

            private void Start()
            {
                for (int i = 0; i < NodeCount; i++)
                {
                    var pp = UnityEngine.Object.Instantiate<GameObject>(node, base.aiActor.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + Toolbox.GetUnitOnCircle((360 / NodeCount) * i, 2), Quaternion.identity, null).GetComponent<AdvancedBodyPartController>();
                    SpawnedNodes.Add(pp.GetComponent<NodeMinionController>());
                    pp.MainBody = base.aiActor;
                    pp.gameObject.transform.parent = base.aiActor.transform;
                }
                IsNodedUp = true;
                this.aiActor.healthHaver.OnPreDeath += (vector2) =>
                {
                    Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 1f, 0.75f, 50, 1);
                    Vector3 vector = this.aiActor.sprite.WorldBottomCenter;
                    AkSoundEngine.PostEvent("Play_BOSS_queenship_explode_01", this.gameObject);
                    GameObject teleportVFX = (PickupObjectDatabase.GetById(573) as ChestTeleporterItem).TeleportVFX;
                    SpawnManager.SpawnVFX(teleportVFX, vector, Quaternion.identity, true);

                };
                this.aiActor.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            }

            private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
            {
                Debug.Log("yep, thats damage");
            }

            public void OhFuck()
            {
                Exploder.DoDistortionWave(base.aiActor.sprite.WorldCenter, 0.1f, 0.25f, 25, 0.5f);
                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Wizard_Death_01", this.gameObject);
                //this.behaviorSpeculator.Interrupt();

            }
            

            public void Update()
            {
                
                if (SpawnedNodes.Count == 0 && IsNodedUp == true)
                {
                    IsNodedUp = false;
                    //Debug.Log("Ow :(");
                    this.aiActor.healthHaver.PreventAllDamage = false;
                    this.aiActor.healthHaver.ApplyDamage(9999, Vector2.zero, "Critical Shit");
                }
                else if (SpawnedNodes.Count > 0)
                {
                    //Debug.Log(90 * ((float)SpawnedNodes.Count / (float)NodeCount));
                    this.healthHaver.currentHealth = 90 * ((float)SpawnedNodes.Count / (float)NodeCount);
                }
            }
            public class Cast_Scatter : Script
            {
                public override IEnumerator Top()
                {
                    var sadassda = UnityEngine.Object.Instantiate(RedWarn, this.BulletBank.aiActor.sprite.WorldTopCenter + new Vector2(-0.125f, 0.125f), Quaternion.identity);
                    sadassda.transform.parent = this.BulletBank.aiActor.transform;
                    sadassda.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("bigbeep");
                    yield return this.Wait(30);
                    var NoP = this.BulletBank.aiActor.GetComponent<NopticonBehavior>();
                    NoP.SpawnedNodes.Shuffle();
                    float rng = BraveUtility.RandomAngle();
                    for (int i = 0; i < NoP.SpawnedNodes.Count; i++)
                    {
                        var node = NoP.SpawnedNodes[i];
                        node.MoveToPosition(node.bodyPartController.MainBody.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + Toolbox.GetUnitOnCircle(((360 / NoP.SpawnedNodes.Count) * i) + rng, 2));
                    }
                    yield return this.Wait(60);
                    GlobalMessageRadio.BroadcastMessageToOthers(this.BulletBank.aiActor.gameObject, "Cast_Scatter");
                    bool b = BraveUtility.RandomBool();
                    for (int e = 0; e < 240; e++)
                    {
                        for (int i = 0; i < NoP.SpawnedNodes.Count; i++)
                        {
                            int coc = NoP.SpawnedNodes.Count() > 6 ? 1 : 7 - NoP.SpawnedNodes.Count();
                            var node = NoP.SpawnedNodes[i];
                            node.MoveToPositionInstant(node.bodyPartController.MainBody.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + Toolbox.GetUnitOnCircle((((360 / NoP.SpawnedNodes.Count) * i) + (b == true ? +(e* coc) : (e * coc))), 2));
                        }
                        yield return this.Wait(1);
                    }
                    yield return this.Wait(30);
                    yield break;
                }
            }
            public class Cast_Burst : Script
            {
                public override IEnumerator Top()
                {
                    var sadassda = UnityEngine.Object.Instantiate(RedWarn, this.BulletBank.aiActor.sprite.WorldTopCenter + new Vector2(-0.125f, 0.125f), Quaternion.identity);
                    sadassda.transform.parent = this.BulletBank.aiActor.transform;
                    sadassda.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("bigbeep");
                    yield return this.Wait(30);
                    var NoP = this.BulletBank.aiActor.GetComponent<NopticonBehavior>();

                    for (int f = 0; f < Mathf.Max(1, 7 - NoP.SpawnedNodes.Count); f++)
                    {
                        NoP.SpawnedNodes.Shuffle();
                        float rng = BraveUtility.RandomAngle();
                        float rng_Val_2 = UnityEngine.Random.Range(1.5f, 3);
                        for (int i = 0; i < NoP.SpawnedNodes.Count; i++)
                        {
                            var node = NoP.SpawnedNodes[i];
                            node.MoveToPosition(node.bodyPartController.MainBody.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + (new Vector2(-0.125f, -0.125f)) + Toolbox.GetUnitOnCircle(((360 / NoP.SpawnedNodes.Count) * i) + rng, 1));
                        }
                        yield return this.Wait(60);
                        GlobalMessageRadio.BroadcastMessageToOthers(this.BulletBank.aiActor.gameObject, "Cast_Burst");
                    }
                    yield return this.Wait(30);
                    yield break;
                }
            }

            public class Cast_Beans : Script
            {
                public override IEnumerator Top()
                {
                    var sadassda = UnityEngine.Object.Instantiate(RedWarn, this.BulletBank.aiActor.sprite.WorldTopCenter + new Vector2(-0.125f, 0.125f), Quaternion.identity);
                    sadassda.transform.parent = this.BulletBank.aiActor.transform;
                    sadassda.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("bigbeep");
                    yield return this.Wait(30);
                    var NoP = this.BulletBank.aiActor.GetComponent<NopticonBehavior>();


                    int h = Mathf.Max(1, 7 - NoP.SpawnedNodes.Count);

                    for (int a = 0; a < h; a++)
                    {
                        float divider = Mathf.Min(1f, h / 6);
                        NoP.SpawnedNodes.Shuffle();
                        float adfsa = (this.GetPredictedTargetPositionExact(1, 30) - this.BulletBank.transform.PositionVector2()).ToAngle();

                        var vec1 = Toolbox.GetUnitOnCircle(adfsa - 45, 3);
                        var vec2 = Toolbox.GetUnitOnCircle(adfsa + 45, 3);
                        for (int i = 0; i < NoP.SpawnedNodes.Count; i++)
                        {
                            float t = (float)i / (float)NoP.SpawnedNodes.Count;
                            var node = NoP.SpawnedNodes[i];
                            node.ToggleOverrideAim(true, adfsa);
                            node.MoveToPosition(node.bodyPartController.MainBody.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + Vector2.Lerp(vec1, vec2, t), 0.5f);
                        }
                        yield return this.Wait(40);
                        GlobalMessageRadio.BroadcastMessageToOthers(this.BulletBank.aiActor.gameObject, "Cast_Direct");
                    }          
                    for (int i = 0; i < NoP.SpawnedNodes.Count; i++)
                    {
                        var node = NoP.SpawnedNodes[i];
                        node.ToggleOverrideAim(false);
                    }
                    yield return this.Wait(30);
                   
                    yield break;
                }
            }

            public class Cast_Singles : Script
            {
                public override IEnumerator Top()
                {
                    var sadassda = UnityEngine.Object.Instantiate(RedWarn, this.BulletBank.aiActor.sprite.WorldTopCenter + new Vector2(-0.125f, 0.125f), Quaternion.identity);
                    sadassda.transform.parent = this.BulletBank.aiActor.transform;
                    sadassda.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("bigbeep");
                    yield return this.Wait(30);

                    var NoP = this.BulletBank.aiActor.GetComponent<NopticonBehavior>();
                    float rng = BraveUtility.RandomAngle();
                    NoP.SpawnedNodes.Shuffle();
                    for (int i = 0; i < NoP.SpawnedNodes.Count; i++)
                    {
                        var node = NoP.SpawnedNodes[i];
                        node.MoveToPosition(node.bodyPartController.MainBody.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + Toolbox.GetUnitOnCircle(((360 / NoP.SpawnedNodes.Count) * i) + rng, 1), 0.5f);
                    }
                    yield return this.Wait(45);
                    for (int i = 0; i < NoP.SpawnedNodes.Count; i++)
                    {
                        var Node = NoP.SpawnedNodes[i];
                        if (Node != null)
                        {
                            var fx = UnityEngine.Object.Instantiate(RedWarn, Node.transform.position + new Vector3(0, 0.5f), Quaternion.identity);
                            fx.transform.parent = Node.transform;
                            fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("beepbeep");

                            float aim = (GetPredictedTargetPosition(0, 1000) - Node.sprite.WorldCenter).ToAngle();
                            Node.MoveToPosition(Node.bodyPartController.MainBody.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + Toolbox.GetUnitOnCircle(aim, Vector2.Distance(GetPredictedTargetPosition(0, 1000), Node.sprite.WorldCenter) / 1.5f), 0.5f);
                            yield return this.Wait(45);
                        }
                        if (Node != null)
                        {
                            Node.ForceMessage("Cast_Basic");
                            yield return this.Wait(15);
                        }
                        Node?.MoveToPosition(Node.bodyPartController.MainBody.sprite.WorldCenter + (new Vector2(-0.125f, -0.125f)) + Toolbox.GetUnitOnCircle(((360 / NoP.SpawnedNodes.Count) * i) + rng, 1), 0.5f);
                    }
                    yield return this.Wait(30);
                    yield break;
                }
            }
        }
    }
}
