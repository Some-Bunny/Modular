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
using HarmonyLib;
using MonoMod.Cil;
using Mono.Cecil.Cil;


namespace ModularMod
{
    public static class Hooks
    {
        public static void Init()
        {
            //new Hook(typeof(Gun).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("PickupHook"));
            //new Hook(typeof(Gun).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("UpdateHook"));

            //new Hook(typeof(Gun).GetMethod("OnEnteredRange", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("OnEnteredRangeHook"));

            new Hook(typeof(PlayerController).GetMethod("SetStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("SetStencilValHook"));
            new Hook(typeof(PlayerController).GetMethod("UpdateStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("UpdateStencilValHook"));
            //new Hook(typeof(PlayerStats).GetMethod("RebuildGunVolleys", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("RebuildGunVolleysHook"));
            //new Hook(typeof(AIActor).GetMethod("TeleportSomewhere", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("TeleportationImmunity"));


            JuneLib.ItemsCore.AddChangeSpawnItem(ReturnObj);
              
            new Hook(typeof(PickupObject).GetMethod("HandlePickupCurseParticles", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("HandlePickupCurseParticlesHook"));

            new Hook(typeof(BaseShopController).GetMethod("HandleEnter", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("HandleEnterHook"));

            //new Hook(typeof(Projectile).GetMethod("BeamCollision", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("FuckYou"));
        }

        [HarmonyPatch(typeof(LootEngine), nameof(LootEngine.GivePrefabToPlayer))]
        public class LootEngine_GivePrefabToPlayer
        {
            [HarmonyPrefix]
            private static bool Awake(GameObject item, PlayerController player)
            {
                Gun component = item.GetComponent<Gun>();
                if (player.IsModular() && component != null)
                {
                    EncounterTrackable component2 = component.GetComponent<EncounterTrackable>();
                    if (component2 != null)
                    {
                        component2.HandleEncounter();
                    }
                    player.inventory.AddGunToInventory(component, true);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.BeamCollision))]
        public class Projectile_BeamCollision
        {
            [HarmonyPrefix]
            private static bool Awake(Projectile __instance, Projectile currentProjectile)
            {
                var eventComp = __instance.projectile.GetComponent<BeamCollisionEvent>();
                if (eventComp != null)
                {
                    bool Destroyed = eventComp.DetermineDestroy != null ? eventComp.DetermineDestroy(__instance) : eventComp.WillBeDestroyed;
                    if (Destroyed == true)
                    {
                        __instance.DieInAir(false, true, true, false);
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.Pickup))]
        public class Gun_Pickup
        {
            [HarmonyPrefix]
            private static bool Awake(Gun __instance, PlayerController player)
            {
                if (player.PlayerHasCore() != null)
                {
                    var yes = __instance.gameObject.GetComponent<ChooseModuleController>();
                    if (yes == null)
                    {
                        yes = __instance.gameObject.AddComponent<ChooseModuleController>();
                        yes.isAlt = player.IsUsingAlternateCostume;
                    }
                    else
                    {
                        yes.Nudge(player);
                    }
                    return false;
                }
                for (int i = __instance.transform.childCount - 1; i > -1; i--)
                {
                    if (__instance.transform.GetChild(i).name.Contains("VFX_MODULABLE"))
                    {
                        UnityEngine.Object.Destroy(__instance.transform.GetChild(i).gameObject);
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Gun), nameof(Gun.Update))]
        public class Gun_Update
        {
            [HarmonyPrefix]
            private static void Update(Gun __instance)
            {
                if (__instance.CurrentOwner != null)
                {
                    for (int i = __instance.transform.childCount - 1; i > -1; i--)
                    {
                        if (__instance.transform.GetChild(i).name.Contains("VFX_MODULABLE"))
                        {
                            UnityEngine.Object.Destroy(__instance.transform.GetChild(i).gameObject);
                        }
                    }
                    var c = __instance.gameObject.GetComponent<ChooseModuleController>();
                    if (c != null)
                    {
                        c.DestroyAllOthers(false, true);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AIActor), nameof(AIActor.TeleportSomewhere))]
        public class AIActor_TeleportSomewhere
        {
            [HarmonyPrefix]
            private static bool Update(AIActor __instance)
            {
                if (__instance.GetComponent<TeleportationImmunity>() != null) { return false; }
                return true;
            }
        }









        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.RebuildGunVolleys))]
        public class PlayerStats_RebuildGunVolleys
        {
            [HarmonyPrefix]
            private static void RebuildGunVolleys(PlayerStats __instance, PlayerController owner)
            {
                GameManager.Instance.StartCoroutine(FrameDelay());
            }
            public static IEnumerator FrameDelay()
            {
                yield return null;
                if (OnRecalculateStats != null) { OnRecalculateStats(); }
                yield break;
            }
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
                            //Debug.Log(1);

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
                            //Debug.Log(2);
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
            
            if (self != null)
            {
                var attacher = self.gameObject.GetComponent<ShittyVFXAttacher>();
                var cmc = self.gameObject.GetComponent<ChooseModuleController>();

                if (attacher != null) { return; }

                if (GameManager.Instance != null && GameManager.Instance.AllPlayers != null)
                {

                    foreach (var player in GameManager.Instance.AllPlayers)
                    {
                        if (player != null)
                        {
                            var core = player.PlayerHasCore();
                            if (core != null && attacher == null && cmc == null && ItemSynergyController.ModularSynergy.isSynergyItem(self.PickupObjectId) == true)
                            {
                                var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                                p.gameObj = VFXStorage.VFX__Synergy;
                                p.wasUsingAltCostume = player.IsUsingAlternateCostume;
                            }
                            else if (self is Gun && core != null && attacher == null && cmc == null)
                            {
                                var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                                p.gameObj = VFXStorage.VFX_Modulable;
                                p.wasUsingAltCostume = player.IsUsingAlternateCostume;

                            }
                        }
                        
                    }
                }
            }    
            
        }
    }
}