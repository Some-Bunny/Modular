using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ModularMod.Code.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using tk2dRuntime.TileMap;
using UnityEngine;
using static ETGMod;
using static ModularMod.LaserDiode.Brapp;
using static ModularMod.LaserDiode.PewPew;

namespace ModularMod
{
    public class EnergyShield : AIActor
    {
        private static GameObject prefab;
        public static readonly string guid = "EnergyShield_MDLR";
        private static GameObject VFX_Object;
        private static GameObject VFX_Object_Electric;
        private static GameObject VFX_Object_Tether;

        public static void BuildPrefab(string GUID)
        {
            if (prefab == null || !EnemyBuilder.Dictionary.ContainsKey(guid + "_(" + GUID + ")"))
            {
                prefab = EnemyBuilder.BuildPrefabBundle("Energy Shield", guid +"_(" + GUID + ")", StaticCollections.Enemy_Collection, "gunnermech_die_001", new IntVector2(0, 0), new IntVector2(8, 9), false, true);
                var companion = prefab.AddComponent<EnergyShieldBehavior>();
                companion.GUID_To_Shield = GUID;
                companion.aiActor.knockbackDoer.weight = 50000;
                companion.aiActor.MovementSpeed = 1f;
                companion.aiActor.CollisionDamage = 1f;
                companion.aiActor.HasShadow = false;
                companion.aiActor.IgnoreForRoomClear = false;
                companion.aiActor.aiAnimator.HitReactChance = 0f;
                companion.aiActor.specRigidbody.CollideWithOthers = true;
                companion.aiActor.specRigidbody.CollideWithTileMap = true;
                companion.aiActor.PreventFallingInPitsEver = false;
                companion.aiActor.healthHaver.ForceSetCurrentHealth(14f);
                companion.aiActor.healthHaver.minimumHealth = 1;
                companion.aiActor.CollisionKnockbackStrength = 0f;
                companion.aiActor.procedurallyOutlined = true;
                companion.aiActor.CanTargetPlayers = true;
                companion.aiActor.HasShadow = true;
                companion.aiActor.healthHaver.flashesOnDamage = true;
                companion.aiActor.healthHaver.PreventAllDamage = false;


                companion.aiActor.PathableTiles = CellTypes.FLOOR;

                companion.aiActor.ShadowObject = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").ShadowObject;
                companion.aiActor.healthHaver.SetHealthMaximum(14f, null, false);
                companion.aiActor.specRigidbody.PixelColliders.Clear();


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
                    ManualHeight = 16,
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
                    ManualHeight = 16,
                    ManualDiameter = 0,
                    ManualLeftX = 0,
                    ManualLeftY = 0,
                    ManualRightX = 0,
                    ManualRightY = 0,
                });


                //companion.aiActor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
                companion.aiAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>();
                companion.aiActor.PreventBlackPhantom = false;
                companion.aiActor.AssignedCurrencyToDrop = 0;

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

                var h = Module.ModularAssetBundle.LoadAsset<GameObject>("EnergyShieldAnimation").GetComponent<tk2dSpriteAnimation>();
                companion.aiActor.spriteAnimator.Library = h;
                companion.aiActor.spriteAnimator.library = h;
                companion.aiActor.aiAnimator.spriteAnimator = companion.aiActor.spriteAnimator;
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "death", new string[] { "death" }, new DirectionalAnimation.FlipType[0]);
                Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimator, "awake", new string[] { "awaken" }, new DirectionalAnimation.FlipType[0]);

                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "awaken", new Dictionary<int, string> {
                    { 6, "Play_ENM_fs_mimic_03" },
                    { 10, "Play_WPN_Life_Orb_Shot_01" },
                });


                Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(companion.spriteAnimator, "death", new Dictionary<int, string> {
                    { 1, "Play_OBJ_turret_set_01" },
                    { 4, "Play_WPN_blackhole_impact_01" },

                });
                

                companion.aiActor.AwakenAnimType = AwakenAnimationType.Awaken;
                companion.aiActor.reinforceType = ReinforceType.SkipVfx;

                var bs = prefab.GetComponent<BehaviorSpeculator>();
                prefab.GetComponent<ObjectVisibilityManager>();
                BehaviorSpeculator behaviorSpeculator = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").behaviorSpeculator;
                bs.OverrideBehaviors = behaviorSpeculator.OverrideBehaviors;
                bs.OtherBehaviors = behaviorSpeculator.OtherBehaviors;


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
                bs.MovementBehaviors = new List<MovementBehaviorBase>() { };
                bs.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
                {
                   
                    
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


                {
                    GameObject VFX = new GameObject("Shield VFX");
                    FakePrefab.DontDestroyOnLoad(VFX);
                    FakePrefab.MarkAsFakePrefab(VFX);
                    VFX.SetActive(false);
                    var tk2d = VFX.AddComponent<tk2dSprite>();
                    tk2d.Collection = StaticCollections.VFX_Collection;
                    tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("protecc"));
                    var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

                    tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("CoolioAnimation").GetComponent<tk2dSpriteAnimation>();
                    tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("start");
                    tk2dAnim.playAutomatically = true;
                    tk2d.usesOverrideMaterial = true;
                    tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                    tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    tk2d.renderer.material.SetFloat("_EmissivePower", 30);
                    tk2d.renderer.material.SetFloat("_EmissiveColorPower", 30);
                    tk2d.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));

                    ExpandReticleRiserEffect rRE = VFX.gameObject.AddComponent<ExpandReticleRiserEffect>();
                    rRE.RiserHeight = 2.5f;
                    rRE.RiseTime = 2;
                    rRE.NumRisers = 3;
                    rRE.UpdateSpriteDefinitions = true;
                    rRE.CurrentSpriteName = "protecc";
                    VFX_Object = VFX;
                }
                {
                    GameObject VFX = new GameObject("Electric Bubble");
                    FakePrefab.DontDestroyOnLoad(VFX);
                    FakePrefab.MarkAsFakePrefab(VFX);
                    VFX.SetActive(false);
                    var tk2d = VFX.AddComponent<tk2dSprite>();
                    tk2d.Collection = StaticCollections.VFX_Collection;
                    tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("bigenergyball_001"));
                    var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

                    tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("SuperChargeAnimation").GetComponent<tk2dSpriteAnimation>();
                    tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("start");
                    tk2dAnim.playAutomatically = true;
                    tk2d.usesOverrideMaterial = true;
                    tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                    tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    tk2d.renderer.material.SetFloat("_EmissivePower", 30);
                    tk2d.renderer.material.SetFloat("_EmissiveColorPower", 30);
                    tk2d.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
                    VFX_Object_Electric = VFX;

                }
                {
                    GameObject VFX = new GameObject("Electric Tether");
                    FakePrefab.DontDestroyOnLoad(VFX);
                    FakePrefab.MarkAsFakePrefab(VFX);
                    VFX.SetActive(false);
                    var tk2d = VFX.AddComponent<tk2dTiledSprite>();
                    tk2d.Collection = StaticCollections.VFX_Collection;
                    tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("tether_001"));
                    var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();
                    tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("TetherAnimation").GetComponent<tk2dSpriteAnimation>();
                    tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("idle");
                    tk2dAnim.playAutomatically = true;
                    tk2d.usesOverrideMaterial = true;
                    tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
                    tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    tk2d.renderer.material.SetFloat("_EmissivePower", 30);
                    tk2d.renderer.material.SetFloat("_EmissiveColorPower", 30);
                    tk2d.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                    VFX_Object_Tether = VFX;
                }

                Game.Enemies.Add("mdlr:energy_shield" + "_("+GUID+")", companion.aiActor);            
            }
        }




        public class EnergyShieldBehavior : BraveBehaviour
        {
            private RoomHandler room;
            public string GUID_To_Shield;
            private AIActor enemyShielded;
            public GlobalMessageRadio.MessageContainer container;
            private GameObject laser;
            private GameObject ZZap;
            private bool STOP = false;

            private static bool DebugCheck = false;
            private static bool DebugCheck2 = false;

            private bool ShouldCountForRoomProgress()
            {
                if (this.aiActor.State != ActorState.Normal) { return false; }
                    if (room == null) { if (DebugCheck == true) { Debug.Log(1); } return false; }
                if (room.remainingReinforcementLayers == null) { if (DebugCheck == true) { Debug.Log(2); } return false; }

                bool remainingReinforcements = true;
                if (room.remainingReinforcementLayers.Count == 0) { remainingReinforcements = false; }

                List<AIActor> EnemyList = GetTheseActiveEnemies(room, RoomHandler.ActiveEnemyType.All);

                for (int i = EnemyList.Count - 1; i > -1; i--)
                {
                    if (EnemyList[i].GetComponent<EnergyShieldProtection>() != null | EnemyList[i].GetComponent<EnergyShieldBehavior>() != null) { if (DebugCheck2 == true) { Debug.Log("D1"); } EnemyList.RemoveAt(i); }
                }

                if (EnemyList.Count == 0 && remainingReinforcements == true) { if (DebugCheck == true) { Debug.Log(3); } return false; }
                if (EnemyList.Count > 0 && remainingReinforcements == true) { if (DebugCheck == true) { Debug.Log(3.5f); } return true; }


                if (EnemyList.Count > 0 && remainingReinforcements == false) { if (DebugCheck == true) { Debug.Log(4); } return false; }
                if (EnemyList.Count == 0 && remainingReinforcements == false) { if (DebugCheck == true) { Debug.Log(5); } return false; }
                if (DebugCheck == true) { Debug.Log(6); }
                return false;
            }

            public List<AIActor> GetTheseActiveEnemies(RoomHandler room, RoomHandler.ActiveEnemyType type)
            {
                var outList = new List<AIActor>();
                if (room.activeEnemies == null)
                {
                    return outList;
                }
                if (type == RoomHandler.ActiveEnemyType.RoomClear)
                {
                    for (int i = 0; i < room.activeEnemies.Count; i++)
                    {
                        if (!room.activeEnemies[i].IgnoreForRoomClear)
                        {
                            outList.Add(room.activeEnemies[i]);
                        }
                    }
                }
                else
                {
                    outList.AddRange(room.activeEnemies);
                }
                return outList;
            }

            private void Start()
            {
                room = this.aiActor.GetAbsoluteParentRoom();
                this.aiActor.healthHaver.OnPreDeath += (vector) =>
                {
                   
                };
            }

            private Dictionary<float, AIActor> dist = new Dictionary<float, AIActor>();
            
            private IEnumerator WAIT()
            {
                float f = 0;
                while (f < 0.25f)
                {
                    this.aiActor.IgnoreForRoomClear = false;
                    f += BraveTime.DeltaTime; yield return null; }
                this.aiActor.healthHaver.minimumHealth = 0;
                this.aiActor.healthHaver.ApplyDamage(1000, Vector2.zero, "DIE");
                yield break;
            }


            public void Update()
            {
                if (this.aiActor == null) { return; }

                if (this.aiActor.healthHaver.GetCurrentHealth() == this.aiActor.healthHaver.minimumHealth && STOP != true)
                {
                    STOP = true;
                    if (enemyShielded != null && enemyShielded.GetComponent<EnergyShieldProtection>() != null)
                    {
                        enemyShielded.IgnoreForRoomClear = false;
                        Destroy(laser);
                        enemyShielded.GetComponent<EnergyShieldProtection>().KaBlewy();
                    }
                    if (ZZap != null)
                    {
                        ZZap.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("end");
                    }
                    var Obj = UnityEngine.Object.Instantiate(RobotMiniMecha.KaboomObject, this.sprite.WorldBottomLeft - new Vector2(0.5f, 0.125f), Quaternion.identity);
                    Obj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("kaboom");
                    this.aiActor.IgnoreForRoomClear = false;
                    this.StartCoroutine(WAIT());
                }

                bool clear = ShouldCountForRoomProgress();
                if (STOP == true) { return; }
                this.aiActor.IgnoreForRoomClear = clear;
                if (enemyShielded != null) { enemyShielded.IgnoreForRoomClear = clear; }
                if (this.aiActor.State == ActorState.Normal && enemyShielded == null)
                {
                    var EnemyList = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                    if (EnemyList != null) 
                    {
                        dist = new Dictionary<float, AIActor>();
                        for (int i = EnemyList.Count - 1; i > -1; i--)
                        {
                            if (EnemyList[i].EnemyGuid == GUID_To_Shield && EnemyList[i].State == ActorState.Normal &&  EnemyList[i].GetComponent<EnergyShieldProtection>() == null)
                            {
                                dist.Add(Vector2.Distance(this.transform.PositionVector2(), EnemyList[i].transform.PositionVector2()), EnemyList[i]);
                            }
                        }
                        if (dist.Count > 0)
                        {
                            var ai = dist[dist.Keys.Min()];
                            enemyShielded = dist[dist.Keys.Min()];
                            var enemy = enemyShielded.gameObject.AddComponent<EnergyShieldProtection>();
                            enemy.user = enemyShielded;
                            if (ZZap != null)
                            {
                                ZZap.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("end");
                            }
                            ZZap = UnityEngine.Object.Instantiate(EnergyShield.VFX_Object_Electric, this.sprite.WorldBottomLeft - new Vector2(0.5f, 0.125f), Quaternion.identity);
                            ZZap.transform.parent = enemy.transform;
                        }
                    }
                }
                else if (enemyShielded != null)
                {
                    if (laser == null)
                    {
                        laser = UnityEngine.Object.Instantiate(EnergyShield.VFX_Object_Tether, this.sprite.WorldCenter + new Vector2(0, 0.5f), Quaternion.identity);
                        tk2dTiledSprite a = laser.GetComponent<tk2dTiledSprite>();
                        a.dimensions = new Vector2(16 * Vector2.Distance(this.sprite.WorldCenter, enemyShielded.sprite.WorldCenter), 16f);
                        a.ShouldDoTilt = false;
                        laser.SetLayerRecursively(LayerMask.NameToLayer("FG_Nonsense"));

                    }
                    tk2dTiledSprite component2 = laser.GetComponent<tk2dTiledSprite>();
                    component2.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnAngle(enemyShielded));
                    component2.dimensions = new Vector2(16 * Vector2.Distance(this.sprite.WorldCenter, enemyShielded.sprite.WorldCenter), 16f);
                    component2.UpdateZDepth();
                    //component2.HeightOffGround = -2;
                    component2.transform.localPosition = new Vector3(0, 0).WithZ(1000);
                    laser.transform.position = this.sprite.WorldCenter;
                }
            }
            public float ReturnAngle(AIActor enemy)
            {
                if (enemy == null) { return 0; }
                return (enemy.sprite.WorldCenter -this.sprite.WorldCenter).ToAngle();
            }
            private class EnergyShieldProtection : MonoBehaviour
            {

                public void KaBlewy()
                {
                    user.healthHaver.AllDamageMultiplier = 1.35f;
                    VFX.GetComponent<ExpandReticleRiserEffect>().Stop();
                    VFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("end");
                    Destroy(this, 0f);
                }

                public GameObject sparkOctantVFX = (PickupObjectDatabase.GetById(105) as FortuneFavorItem).sparkOctantVFX;
                private GameObject VFX;
                public float pushRadius = 4f;
                public float secondRadius = 6f;
                public float finalRadius = 8f;
                public float pushStrength = 120f;
                public AIActor user;
                public void Start()
                {
                    user.healthHaver.AllDamageMultiplier = 0;
                    VFX = UnityEngine.Object.Instantiate(VFX_Object, user.sprite.WorldCenter, Quaternion.identity);
                    Vector3 a = user.sprite.WorldCenter.ToVector3ZUp(0f);
                    VFX.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(a + new Vector3(0,0), tk2dBaseSprite.Anchor.MiddleCenter);
                    VFX.transform.parent = user.transform;
                    VFX.GetComponent<tk2dBaseSprite>().HeightOffGround = 0.2f;
                    user.sprite.AttachRenderer(VFX.GetComponent<tk2dBaseSprite>());
                }
                public void Update()
                {
                    float innerRadiusSqrDistance = this.pushRadius * this.pushRadius;
                    float outerRadiusSqrDistance = this.secondRadius * this.secondRadius;
                    float finalRadiusSqrDistance = this.finalRadius * this.finalRadius;
                    float pushStrengthRadians = this.pushStrength * 0.017453292f;
                    List<Projectile> ensnaredProjectiles = new List<Projectile>();
                    List<Vector2> initialDirections = new List<Vector2>();
                    GameObject[] octantVFX = new GameObject[8];
                    Vector2 playerCenter = user.CenterPosition;
                    List<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles.ToList();
                    for (int i = 0; i < allProjectiles.Count(); i++)
                    {
                        Projectile projectile = allProjectiles[i];
                        if ((projectile.Owner is PlayerController))
                        {
                            Vector2 worldCenter = projectile.sprite.WorldCenter;
                            Vector2 vector = worldCenter - playerCenter;
                            float num = Vector2.SqrMagnitude(vector);
                            if (num < innerRadiusSqrDistance && !ensnaredProjectiles.Contains(projectile))
                            {
                                projectile.RemoveBulletScriptControl();
                                ensnaredProjectiles.Add(projectile);
                                initialDirections.Add(projectile.Direction);
                                int num2 = BraveMathCollege.VectorToOctant(vector);
                                if (octantVFX[num2] == null)
                                {
                                    AkSoundEngine.PostEvent("Play_WPN_beam_slash_01", user.gameObject);
                                    octantVFX[num2] = user.PlayEffectOnActor(this.sparkOctantVFX, Vector3.zero, true, true, false);
                                    octantVFX[num2].transform.rotation = Quaternion.Euler(0f, 0f, (float)(-45 + -45 * num2));
                                }
                            }
                        }
                    }
                    for (int j = 0; j < ensnaredProjectiles.Count; j++)
                    {
                        Projectile projectile2 = ensnaredProjectiles[j];
                        if (!projectile2)
                        {
                            ensnaredProjectiles.RemoveAt(j);
                            initialDirections.RemoveAt(j);
                            j--;
                        }
                        else
                        {
                            Vector2 worldCenter2 = projectile2.sprite.WorldCenter;
                            Vector2 a = playerCenter - worldCenter2;
                            float num3 = Vector2.SqrMagnitude(a);
                            if (num3 > finalRadiusSqrDistance)
                            {
                                ensnaredProjectiles.RemoveAt(j);
                                initialDirections.RemoveAt(j);
                                j--;
                            }
                            else if (num3 > outerRadiusSqrDistance)
                            {
                                projectile2.Direction = Vector3.RotateTowards(projectile2.Direction, initialDirections[j], pushStrengthRadians * BraveTime.DeltaTime * 0.5f, 0f).XY().normalized;
                            }
                            else
                            {
                                Vector2 v = a * -1f;
                                float num4 = 1f;
                                if (num3 / innerRadiusSqrDistance < 0.75f)
                                {
                                    num4 = 3f;
                                }
                                v = ((v.normalized + initialDirections[j].normalized) / 2f).normalized;
                                projectile2.Direction = Vector3.RotateTowards(projectile2.Direction, v, pushStrengthRadians * BraveTime.DeltaTime * num4, 0f).XY().normalized;
                            }
                        }
                    }
                    for (int k = 0; k < 8; k++)
                    {
                        if (octantVFX[k] != null && !octantVFX[k])
                        {
                            octantVFX[k] = null;
                        }
                    }
                }
            }

        }
    }
}