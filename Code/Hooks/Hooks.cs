using System.Reflection;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using Gungeon;
using System.Collections;
using Brave.BulletScript;
using System;
using System.Collections.Generic;
using Planetside;
using static SpawnEnemyOnDeath;
using Alexandria.Misc;
using UnityEngine.UI;
using FullInspector;
using Alexandria.NPCAPI;
using Alexandria.ItemAPI;
using static BossFinalRogueLaunchShips1;
using SaveAPI;

namespace ModularMod
{
    public static class Hooks
    {
        public static void Init()
        {
            new Hook(typeof(Gun).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("PickupHook"));
            new Hook(typeof(Gun).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("UpdateHook"));

            //new Hook(typeof(Gun).GetMethod("OnEnteredRange", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("OnEnteredRangeHook"));

            new Hook(typeof(PlayerController).GetMethod("SetStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("SetStencilValHook"));
            new Hook(typeof(PlayerController).GetMethod("UpdateStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("UpdateStencilValHook"));
            new Hook(typeof(PlayerStats).GetMethod("RebuildGunVolleys", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("RebuildGunVolleysHook"));
            new Hook(typeof(AIActor).GetMethod("TeleportSomewhere", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("TeleportationImmunity"));


            JuneLib.ItemsCore.AddChangeSpawnItem(ReturnObj);
              
            new Hook(typeof(PickupObject).GetMethod("HandlePickupCurseParticles", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("HandlePickupCurseParticlesHook"));

            new Hook(typeof(BaseShopController).GetMethod("HandleEnter", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("HandleEnterHook"));

            new Hook(typeof(Projectile).GetMethod("BeamCollision", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("FuckYou"));
            //new Hook(typeof(BasicBeamController).GetMethod("FindBeamTarget", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("FuckYouToo"));
        }

        public static bool FuckYouToo(MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers.Func<BasicBeamController, Vector2, Vector2, float, int, Vector2, Vector2, SpeculativeRigidbody, PixelCollider, List<PointcastResult>, System.Func<SpeculativeRigidbody, bool>, 
            SpeculativeRigidbody[], bool> orig, BasicBeamController self, Vector2 origin, Vector2 direction, float distance, int collisionMask, Vector2 targetPoint, Vector2 targetNormal, SpeculativeRigidbody hitRigidbody, PixelCollider hitPixelCollider, List<PointcastResult> boneCollisions, Func<SpeculativeRigidbody, bool> rigidbodyExcluder = null, params SpeculativeRigidbody[] ignoreRigidbodies)
        {
            bool flag = false;
            targetPoint = new Vector2(-1f, -1f);
            targetNormal = new Vector2(0f, 0f);
            hitRigidbody = null;
            hitPixelCollider = null;
            if (self.collisionType == BasicBeamController.BeamCollisionType.Rectangle)
            {
                if (!self.specRigidbody)
                {
                    self.specRigidbody = self.gameObject.AddComponent<SpeculativeRigidbody>();
                    self.specRigidbody.CollideWithTileMap = false;
                    self.specRigidbody.CollideWithOthers = true;
                    PixelCollider pixelCollider = new PixelCollider();
                    pixelCollider.Enabled = false;
                    pixelCollider.CollisionLayer = CollisionLayer.PlayerBlocker;
                    pixelCollider.Enabled = true;
                    pixelCollider.IsTrigger = true;
                    pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
                    pixelCollider.ManualOffsetX = 0;
                    pixelCollider.ManualOffsetY = self.collisionWidth / -2;
                    pixelCollider.ManualWidth = self.collisionLength;
                    pixelCollider.ManualHeight = self.collisionWidth;
                    self.specRigidbody.PixelColliders = new List<PixelCollider>(1);
                    self.specRigidbody.PixelColliders.Add(pixelCollider);
                    self.specRigidbody.Initialize();
                }
                if (self.m_cachedRectangleOrigin != origin || self.m_cachedRectangleDirection != direction)
                {
                    self.specRigidbody.Position = new Position(origin);
                    self.specRigidbody.PrimaryPixelCollider.SetRotationAndScale(direction.ToAngle(), Vector2.one);
                    self.specRigidbody.UpdateColliderPositions();
                    self.m_cachedRectangleOrigin = origin;
                    self.m_cachedRectangleDirection = direction;
                }
                int num = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox);
                if ((collisionMask & num) == num)
                {
                    self.specRigidbody.PrimaryPixelCollider.CollisionLayerIgnoreOverride &= ~CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider);
                }
                else
                {
                    self.specRigidbody.PrimaryPixelCollider.CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider);
                }
                List<CollisionData> list = new List<CollisionData>();
                self.specRigidbody.PrimaryPixelCollider.Enabled = true;
                flag = PhysicsEngine.Instance.OverlapCast(self.specRigidbody, list, false, true, null, null, false, null, null, ignoreRigidbodies);
                self.specRigidbody.PrimaryPixelCollider.Enabled = false;
                boneCollisions = new List<PointcastResult>();
                if (!flag)
                {
                    return false;
                }
                targetNormal = list[0].Normal;
                targetPoint = list[0].Contact;
                hitRigidbody = list[0].OtherRigidbody;
                hitPixelCollider = list[0].OtherPixelCollider;
            }
            else if (self.UsesBones)
            {
                float num2 = -self.collisionRadius * self.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
                float num3 = self.collisionRadius * self.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
                int num4 = Mathf.Max(2, Mathf.CeilToInt((num3 - num2) / 0.25f));
                int ignoreTileBoneCount;
                List<IntVector2> points = self.GeneratePixelCloud(num2, num3, (float)num4, out ignoreTileBoneCount);
                List<IntVector2> lastFramePoints = self.GenerateLastPixelCloud(num2, num3, (float)num4);
                if (!PhysicsEngine.Instance.Pointcast(points, lastFramePoints, num4, out boneCollisions, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, rigidbodyExcluder, ignoreTileBoneCount, ignoreRigidbodies))
                {
                    return false;
                }
                PointcastResult pointcastResult = boneCollisions[0];
                for (int i = 0; i < boneCollisions.Count; i++)
                {
                    if (boneCollisions[i].hitDirection == HitDirection.Forward && boneCollisions[i].boneIndex > 0)
                    {
                        pointcastResult = boneCollisions[i];
                        break;
                    }
                }
                targetPoint = pointcastResult.hitResult.Contact;
                targetNormal = pointcastResult.hitResult.Normal;
                hitRigidbody = pointcastResult.hitResult.SpeculativeRigidbody;
                hitPixelCollider = pointcastResult.hitResult.OtherPixelCollider;
            }
            else
            {
                float num5 = -self.collisionRadius * self.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
                float num6 = self.collisionRadius * self.m_projectileScale * PhysicsEngine.Instance.PixelUnitWidth;
                int num7 = Mathf.Max(2, Mathf.CeilToInt((num6 - num5) / 0.25f));
                RaycastResult raycastResult = null;
                for (int j = 0; j < num7; j++)
                {
                    float y = Mathf.Lerp(num5, num6, (float)j / (float)(num7 - 1));
                    Vector2 unitOrigin = origin + new Vector2(0f, y).Rotate(direction.ToAngle());
                    RaycastResult raycastResult2;
                    if (PhysicsEngine.Instance.RaycastWithIgnores(unitOrigin, direction.normalized, distance, out raycastResult2, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, rigidbodyExcluder, ignoreRigidbodies))
                    {
                        flag = true;
                        if (raycastResult == null || raycastResult2.Distance < raycastResult.Distance)
                        {
                            RaycastResult.Pool.Free(ref raycastResult);
                            raycastResult = raycastResult2;
                        }
                        else
                        {
                            RaycastResult.Pool.Free(ref raycastResult2);
                        }
                    }
                }
                boneCollisions = new List<PointcastResult>();
                if (!flag)
                {
                    return false;
                }
                targetNormal = raycastResult.Normal;
                targetPoint = origin + BraveMathCollege.DegreesToVector(direction.ToAngle(), raycastResult.Distance);
                hitRigidbody = raycastResult.SpeculativeRigidbody;
                hitPixelCollider = raycastResult.OtherPixelCollider;
                RaycastResult.Pool.Free(ref raycastResult);
            }
            if (hitRigidbody == null)
            {
                return true;
            }
            if (hitRigidbody.minorBreakable && !hitRigidbody.minorBreakable.OnlyBrokenByCode)
            {
                hitRigidbody.minorBreakable.Break(direction);
            }
            DebrisObject component = hitRigidbody.GetComponent<DebrisObject>();
            if (component)
            {
                component.Trigger(direction, 0.5f, 1f);
            }
            TorchController component2 = hitRigidbody.GetComponent<TorchController>();
            if (component2)
            {
                component2.BeamCollision(self.projectile);
            }
            if (hitRigidbody.projectile && hitRigidbody.projectile.collidesWithProjectiles)
            {
                hitRigidbody.projectile.BeamCollision(self.projectile);
                var pain = hitRigidbody.projectile.gameObject.GetComponent<BeamCollisionEvent>();
                if (pain != null)
                {
                    return pain.isCollisionEvent;
                }
            }


            return true;
        }

        public static void FuckYou(Action<Projectile, Projectile> orig, Projectile self, Projectile currentProjectile)
        {
            var eventComp = self.projectile.GetComponent<BeamCollisionEvent>();
            if (eventComp != null)
            {
                bool Destroyed = eventComp.DetermineDestroy != null ? eventComp.DetermineDestroy(self) : eventComp.WillBeDestroyed;
                if (Destroyed == true)
                {
                    self.DieInAir(false, true, true, false);
                }
            }
            else
            {
                orig(self, currentProjectile);
            }
        }



        public static void UpdateHook(Action<Gun> orig, Gun self)
        {
            orig(self);
            if (self.CurrentOwner != null)
            {
                for (int i = self.transform.childCount - 1; i > -1; i--)
                {
                    if (self.transform.GetChild(i).name.Contains("VFX_MODULABLE"))
                    {
                        UnityEngine.Object.Destroy(self.transform.GetChild(i).gameObject);
                    }
                }
                var c = self.gameObject.GetComponent<ChooseModuleController>();
                if (c != null)
                {
                    c.DestroyAllOthers(false, true);
                }
            }
        }
        public static void TeleportationImmunity(Action<AIActor, IntVector2?, bool> orig, AIActor self, IntVector2? overrideClearance = null, bool keepClose = false)
        {
            if (self.GetComponent<TeleportationImmunity>() != null) { return; }
            orig(self, overrideClearance, keepClose);
        }




        public static void RebuildGunVolleysHook(Action<PlayerStats, PlayerController> orig, PlayerStats self, PlayerController p)
        {
            orig(self, p);
            GameManager.Instance.StartCoroutine(FrameDelay());
        }
        public static IEnumerator FrameDelay()
        {
            yield return null;
            if (OnRecalculateStats != null) { OnRecalculateStats(); }
            yield break;
        }
        public static Action OnRecalculateStats;

        public static GameObject ReturnObj(PickupObject pickup)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.PlayerHasCore() != null) 
                {
                    var HPComp = pickup.GetComponent<HealthPickup>();
                    if (HPComp != null)
                    {
                        bool flga = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.PAST);
                        if (HPComp.healAmount == 0.5f)
                        {
                            pickup = UnityEngine.Random.value < 0.02f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID) : PickupObjectDatabase.GetById(Scrap.Scrap_ID);

                        }
                        if (HPComp.healAmount == 1f)
                        {
                            pickup = UnityEngine.Random.value < 0.035f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID) : PickupObjectDatabase.GetById(Scrap.Scrap_ID);
                        }
                    }
                }
            }
            return pickup.gameObject;
        }

        public static void PickupHook(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            if (player.PlayerHasCore() != null)
            {
                var yes = self.gameObject.GetComponent<ChooseModuleController>();
                if (yes == null)
                {
                    yes = self.gameObject.AddComponent<ChooseModuleController>();
                    yes.isAlt = player.IsUsingAlternateCostume;

                }
                else
                {
                    yes.Nudge(player);
                }
            }
            else
            {
                orig(self, player);
                for (int i = self.transform.childCount - 1; i > -1; i--)
                {
                    if (self.transform.GetChild(i).name.Contains("VFX_MODULABLE"))
                    {
                        UnityEngine.Object.Destroy(self.transform.GetChild(i).gameObject);
                    }
                }
            }
        }




        public static void OnEnteredRangeHook(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            orig(self, player);
            if (player.PlayerHasCore() != null && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null)
            {
                var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                p.wasUsingAltCostume = player.IsUsingAlternateCostume;
            }
        }


        //AwakeHook
        public static bool Stencility_Enabled = true;
        public static void SetStencilValHook(Action<PlayerController, int> orig, PlayerController player, int i)
        {
            if (player.sprite.renderer.material.shader == StaticShaders.TransparencyShader) { return; }
            if (Stencility_Enabled == false) { return; }
            orig(player, i);
        }
        public static void UpdateStencilValHook(Action<PlayerController> orig, PlayerController player)
        {
            if (player.sprite.renderer.material.shader == StaticShaders.TransparencyShader) { return; }
            if (Stencility_Enabled == false) { return; }
            orig(player);
        }

        public static void HandleEnterHook(Action<BaseShopController, PlayerController> orig, BaseShopController self, PlayerController p)
        {
            if (!self.m_hasBeenEntered && self.baseShopType == BaseShopController.AdditionalShopType.NONE)
            {
                foreach (PlayerController p1 in GameManager.Instance.AllPlayers)
                {
                    if (p1.PlayerHasCore() == true)
                    {
                        ReinitializeHPTOModules(self);
                    }
                }
            }
            orig(self, p);
            
        }

        public static void ReinitializeHPTOModules(BaseShopController self)
        {
            if (self.baseShopType == BaseShopController.AdditionalShopType.NONE)
            {
                for (int i = 0; i < self.m_itemControllers.Count; i++)
                {
                    var HPComp = self.m_itemControllers[i].item.GetComponent<HealthPickup>();
                    var AmmoComp = self.m_itemControllers[i].item.GetComponent<AmmoPickup>();

                    if (self.m_itemControllers[i] && self.m_itemControllers[i].item)
                    {
                        bool flga = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.PAST);
                        if (HPComp != null)
                        {
                            Debug.Log(1);

                            if (HPComp.healAmount == 0.5f)
                            {
                                var g = UnityEngine.Random.value < 0.025f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                                self.m_shopItems[i] = g;
                                self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                            }
                            if (HPComp.healAmount == 1f)
                            {
                                var g = UnityEngine.Random.value < 0.0625f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                                self.m_shopItems[i] = g;
                                self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                            }
                        }
                        if (AmmoComp != null)
                        {
                            Debug.Log(2);
                            if (AmmoComp.mode == AmmoPickup.AmmoPickupMode.SPREAD_AMMO)
                            {
                                var g = UnityEngine.Random.value < 0.025f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                                self.m_shopItems[i] = g;
                                self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                            }
                            if (AmmoComp.mode == AmmoPickup.AmmoPickupMode.FULL_AMMO)
                            {
                                var g = UnityEngine.Random.value < 0.0625f && flga == true ? PickupObjectDatabase.GetById(CraftingCore.CraftingCoreID).gameObject : PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject;
                                self.m_shopItems[i] = g;
                                self.m_itemControllers[i].Initialize(g.GetComponent<PickupObject>(), self);
                            }
                        }
                    }
                }
            }
        }

        public static void HandlePickupCurseParticlesHook(Action<PickupObject> orig, PickupObject self)
        {
            orig(self);
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.PlayerHasCore() != null && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null && ItemSynergyController.ModularSynergy.isSynergyItem(self.PickupObjectId) == true)
                {
                    var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                    p.gameObj = VFXStorage.VFX__Synergy;
                    p.wasUsingAltCostume = player.IsUsingAlternateCostume;
                }
                else if (self is Gun && player.PlayerHasCore() != null && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null)
                {
                    var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                    p.wasUsingAltCostume = player.IsUsingAlternateCostume;
                }
            }          
        }
    }
}