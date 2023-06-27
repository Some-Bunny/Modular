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
            //new Hook(typeof(Gun).GetMethod("OnEnteredRange", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("OnEnteredRangeHook"));

            new Hook(typeof(PlayerController).GetMethod("SetStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("SetStencilValHook"));
            new Hook(typeof(PlayerController).GetMethod("UpdateStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("UpdateStencilValHook"));
            new Hook(typeof(PlayerStats).GetMethod("RebuildGunVolleys", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("RebuildGunVolleysHook"));
            new Hook(typeof(AIActor).GetMethod("TeleportSomewhere", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("TeleportationImmunity"));


            JuneLib.ItemsCore.AddChangeSpawnItem(ReturnObj);
              
            new Hook(typeof(PickupObject).GetMethod("HandlePickupCurseParticles", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("HandlePickupCurseParticlesHook"));
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
                            pickup.gameObject.SetActive(true);

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
            if (player.HasPickupID(ModulePrinterCore.ModulePrinterCoreID) == true)
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
                var c = self.gameObject.GetComponent<ChooseModuleController>();
                if (c != null) { if (c.isBeingDestroyed == true) { return; } }
                orig(self, player);
            }
        }

        public static void OnEnteredRangeHook(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            orig(self, player);
            if (player.PlayerHasCore() != null && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null)
            {
                var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                p.mainPlayer = player;
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
                    if (self.m_itemControllers[i] && self.m_itemControllers[i].item && HPComp != null)
                    {
                        bool flga = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.PAST);

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
                    p.mainPlayer = player;
                }
                else if (self is Gun && player.PlayerHasCore() != null && self.gameObject.GetComponent<ShittyVFXAttacher>() == null && self.gameObject.GetComponent<ChooseModuleController>() == null)
                {
                    var p = self.gameObject.AddComponent<ShittyVFXAttacher>();
                    p.mainPlayer = player;
                }
            }          
        }
    }
}