using Alexandria.BreakableAPI;
using Alexandria.DungeonAPI;
using Alexandria.EnemyAPI;
using Alexandria.PrefabAPI;
using Dungeonator;
using ModularMod.Code.Hooks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class SniperTurretsController : MonoBehaviour
    {
        public int waveCount = 0;
        public enum ActivationState
        {
            INSTANT,
            ONE_WAVE,
            TWO_WAVE
        }

        public ActivationState state = ActivationState.INSTANT;
        public string ActivatioAnimation;
        private bool Entered = false;
        public void Start()
        {
            CurrentState = State.INACTIVE;
            shootPosition = this.GetComponent<tk2dBaseSprite>().WorldCenter;
            bulletBank = this.GetComponent<AIBulletBank>();
            currentRoom = this.transform.position.GetAbsoluteRoom();
            if (laserPointer == null)
            {
                laserPointer = SpawnManager.SpawnVFX((GameObject)BraveResources.Load(isProfessional == false ? "Global VFX/VFX_LaserSight_Enemy" : "Global VFX/VFX_LaserSight_Enemy_Green", ".prefab"), true);
                laserPointer.transform.position = shootPosition;
                laserPointer.transform.parent = this.gameObject.transform;
                laserPointerTiledSprite = laserPointer.GetComponent<tk2dTiledSprite>();
                laserPointerTiledSprite.HeightOffGround = 11;
                laserPointerTiledSprite.renderer.enabled = true;
                laserPointerTiledSprite.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle());
                laserPointerTiledSprite.dimensions = new Vector2(0, 1f);
                laserPointerTiledSprite.ForceRotationRebuild();
                laserPointerTiledSprite.UpdateZDepth();
            }
            Actions.OnReinforcementWave += (roomHandler) =>
            {
                if (roomHandler == currentRoom)
                {
                    if (waveCount > 0) { waveCount--; }
                }
            };
            currentRoom.Entered += CurrentRoom_Entered;
        }

        private void CurrentRoom_Entered(PlayerController p)
        {
            Entered = true;
        }

        private bool shouldBeActive()
        {
            if (CurrentState == State.INACTIVE) { return false; }
            if (CurrentState == State.ACTIVATING) { return false; }
            return true;
        }
        public void Update()
        {
            
            if (laserPointer != null && this.gameObject != null && shouldBeActive() == true)
            {
                DoRayCast();
            }
            else if (waveCount == 0 && CurrentState == State.INACTIVE && Entered == true)
            {
                CurrentState = State.ACTIVATING;
                this.StartCoroutine(Activate());
            }
        }

        public IEnumerator Activate()
        {
            float e = 0;
            this.GetComponent<tk2dSpriteAnimator>().Play(ActivatioAnimation);
            laserPointerTiledSprite.renderer.enabled = false;
            while (e < 1)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            DoRayCast();
            while (e < 1f)
            {
                bool enabled = e % 0.33f > 0.16f;
                if (this == null) { yield break; }
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                if (this.gameObject == null) { yield break; }
                laserPointerTiledSprite.renderer.enabled = enabled;
                e += BraveTime.DeltaTime;
                yield return null;
            }
            laserPointerTiledSprite.renderer.enabled = true;
            CurrentState = State.PRIMED;
            yield break;
        }


        public void DoRayCast()
        {
            float num9 = float.MaxValue;
            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable;
            CollisionLayer layer2 = CollisionLayer.PlayerHitBox;
            int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, layer2);
            RaycastResult raycastResult2;
            if (PhysicsEngine.Instance.Raycast(shootPosition, ReturnDirection(), 100, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
            {
                num9 = raycastResult2.Distance;
                if (raycastResult2.SpeculativeRigidbody && raycastResult2.OtherPixelCollider.CollisionLayer == layer2)
                {
                    if (CurrentState == State.PRIMED)
                    {
                        Shoot();
                        if (isProfessional == true)
                        {
                            SniperTurretsController[] turrets = UnityEngine.Object.FindObjectsOfType<SniperTurretsController>();
                            for (int i = 0; i < turrets.Count(); i++)
                            {
                                if (turrets[i].isProfessional && turrets[i].currentRoom == this.currentRoom && turrets[i].shouldBeActive() == true)
                                {
                                    turrets[i].Shoot();
                                }
                            }
                        }
                    }

                }
            }
            RaycastResult.Pool.Free(ref raycastResult2);
            laserPointerTiledSprite.dimensions = new Vector2((num9 / 0.0625f), 1f);
            laserPointerTiledSprite.ForceRotationRebuild();
            laserPointerTiledSprite.UpdateZDepth();
        }

        public void Shoot()
        {
            CurrentState = State.ABOUT_TO_FIRE;
            GameManager.Instance.StartCoroutine(StartShoot());
        }


        private IEnumerator StartShoot()
        {
            float elaWait = 0f;
            float duraWait = 1f;

            while (elaWait < duraWait)
            {
                bool enabled = elaWait % 0.33f > 0.16f;
                if (this == null) { yield break; }
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                if (this.gameObject == null) { yield break; }
                laserPointerTiledSprite.renderer.enabled = enabled;
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            if (this == null) { yield break; }
            if (laserPointerTiledSprite.gameObject == null) { yield break; }
            if (this.gameObject != null)
            {
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                laserPointerTiledSprite.renderer.enabled = false;
                AkSoundEngine.PostEvent("Play_WPN_sniperrifle_shot_01", this.gameObject);
                GameObject gameObject = SpawnManager.SpawnProjectile(this.bulletBank.Bullets[0].BulletObject, shootPosition, Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle()), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                component.baseData.speed = 50;
                component.UpdateSpeed();
                component.Shooter = this.GetComponent<SpeculativeRigidbody>();
                component.IgnoreTileCollisionsFor(0.03f);
                component.pierceMinorBreakables = true;
                component.collidesWithEnemies = false;
                SpawnManager.PoolManager.Remove(component.gameObject.transform);
                component.BulletScriptSettings.preventPooling = true;




                elaWait = 0f;
                duraWait = 2f;

                if (muzzleFlashPrefab != null)
                {
                    GameObject vfx = SpawnManager.SpawnVFX(muzzleFlashPrefab, true);
                    vfx.transform.position = shootPosition;
                    vfx.transform.localRotation = Quaternion.Euler(0f, 0f, ReturnDirection().ToAngle());
                    vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;

                    Destroy(vfx, 2);
                }
                CurrentState = State.POST_FIRE;
            }
            while (elaWait < duraWait)
            {
                if (this == null) { yield break; }
                if (laserPointerTiledSprite.gameObject == null) { yield break; }
                if (this.gameObject == null) { yield break; }
                if (elaWait > (duraWait - 0.5f))
                {
                    laserPointerTiledSprite.renderer.enabled = true;
                }
                elaWait += BraveTime.DeltaTime;
                CurrentState = State.PRE_PRIMED;
                yield return null;
            }
            CurrentState = State.PRIMED;
            yield break;
        }

        public Vector2 ReturnDirection()
        {
            return Toolbox.GetUnitOnCircle(DirectionToFire, 1);
        }

        public AIBulletBank bulletBank;
        public RoomHandler currentRoom;
        public GameObject muzzleFlashPrefab;

        public Vector2 shootPosition;
        public GameObject laserPointer;
        public tk2dTiledSprite laserPointerTiledSprite;
        public float SizeOffset = 1;
        public bool isProfessional;

        public float DirectionToFire;
        public State CurrentState;
        public enum State
        {
            PRIMED,
            ABOUT_TO_FIRE,
            POST_FIRE,
            PRE_PRIMED,
            INACTIVE,
            ACTIVATING
        }
    }
    public class SniperTurretsDefault
    {
        public static void InitDownward()
        {
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_down";

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretDown_0", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_1");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_down";
                t.waveCount = 1;  

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretDown_1", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_2");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_down";
                t.waveCount = 2;

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretDown_2", obj.gameObject);
            }

        }
        public static void InitLeft()
        {
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Left");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_left";
                t.DirectionToFire = Vector2.left.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretLeft_0", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Left_1");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_left";
                t.waveCount = 1;
                t.DirectionToFire = Vector2.left.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretLeft_1", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Left_2");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_left";
                t.waveCount = 2;
                t.DirectionToFire = Vector2.left.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretLeft_2", obj.gameObject);
            }

        }
        public static void InitRight()
        {
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Right");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_right";
                t.DirectionToFire = Vector2.right.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretRight_0", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Right_1");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_right";
                t.waveCount = 1;
                t.DirectionToFire = Vector2.right.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretRight_1", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Right_2");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_right";
                t.waveCount = 2;
                t.DirectionToFire = Vector2.right.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretRight_2", obj.gameObject);
            }

        }

    }

    public class SniperTurretsProfessional
    {
        public static void InitDownward()
        {
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_down_professional";
                t.isProfessional = true;

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretDownProfessional_0", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_1");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_down_professional";
                t.waveCount = 1;
                t.isProfessional = true;

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretDownProfessional_1", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_2");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_down_professional";
                t.waveCount = 2;
                t.isProfessional = true;

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretDownProfessional_2", obj.gameObject);
            }

        }
        public static void InitLeft()
        {
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_left_professional";
                t.isProfessional = true;
                t.DirectionToFire = Vector2.left.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretLeftProfessional_0", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_1");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_left_professional";
                t.waveCount = 1;
                t.isProfessional = true;
                t.DirectionToFire = Vector2.left.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretLeftProfessional_1", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_2");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_left_professional";
                t.waveCount = 2;
                t.isProfessional = true;
                t.DirectionToFire = Vector2.left.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretLeftProfessional_2", obj.gameObject);
            }

        }
        public static void InitRight()
        {
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_right_professional";
                t.isProfessional = true;
                t.DirectionToFire = Vector2.right.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretRightProfessional_0", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_1");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_right_professional";
                t.waveCount = 1;
                t.isProfessional = true;
                t.DirectionToFire = Vector2.right.ToAngle();

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretRightProfessional_1", obj.gameObject);
            }
            {
                AIBulletBank.Entry entrySniper = Toolbox.CopyBulletBankEntry(EnemyDatabase.GetOrLoadByGuid("31a3ea0c54a745e182e22ea54844a82d").bulletBank.GetBullet("sniper"), "sniperTurret");

                GameObject obj = PrefabBuilder.BuildObject("Sniper_Down_2");
                var tk2d = obj.AddComponent<tk2dSprite>();
                tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
                tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("lasersniper_down_spawn1"));
                var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();

                tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("LaserTurretAnimation").GetComponent<tk2dSpriteAnimation>();
                tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("preIlde");
                tk2dAnim.playAutomatically = true;


                //MajorBreakable sniperTurretDefaultaFront = BreakableAPIToolbox.GenerateMajorBreakable("sniperTurretDefaultaFront", idlePaths, 5, idlePaths, 18, 15000, true, 0, 0, 0, 0, true, null, null, true, null);
                AIBulletBank bulletBankLeft = obj.gameObject.AddComponent<AIBulletBank>();
                SniperTurretsController t = obj.gameObject.AddComponent<SniperTurretsController>();
                t.DirectionToFire = Vector2.down.ToAngle();
                t.muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
                t.ActivatioAnimation = "activate_right_professional";
                t.waveCount = 2;
                t.isProfessional = true;
                t.DirectionToFire = Vector2.right.ToAngle();
                

                bulletBankLeft.Bullets = new List<AIBulletBank.Entry>();
                bulletBankLeft.Bullets.Add(entrySniper);
                StaticReferences.customObjects.Add("laserTurretRightProfessional_2", obj.gameObject);
            }

        }
    }
}