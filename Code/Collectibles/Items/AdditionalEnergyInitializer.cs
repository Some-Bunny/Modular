using Alexandria.ItemAPI;
using Alexandria.Misc;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static ModularMod.Hooks;

namespace ModularMod
{
    public class AdditionalEnergyInitializer
    {
        public static GameObject PowerUpVFX;
        public static void Init()
        {
            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("PowerUpVFX_007"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("PowerUpAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);
            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(tk2dAnim, "power_up", new Dictionary<int, string>()
            {
                {1, "Play_WPN_plasmacell_reload_01"},
            });
            PowerUpVFX = VFX;

            BasicStatPickup Heart1 = PickupObjectDatabase.GetById(421) as BasicStatPickup;
            Heart1.gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>();
            BasicStatPickup Heart2 = PickupObjectDatabase.GetById(422) as BasicStatPickup;
            Heart2.gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>();
            BasicStatPickup Heart3 = PickupObjectDatabase.GetById(423) as BasicStatPickup;
            Heart3.gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>();
            BasicStatPickup Heart4 = PickupObjectDatabase.GetById(424) as BasicStatPickup;
            Heart4.gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>();
            BasicStatPickup Heart5 = PickupObjectDatabase.GetById(425) as BasicStatPickup;
            Heart5.gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>();

            PickupObjectDatabase.GetById(313).gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>();
            PickupObjectDatabase.GetById(570).gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>().AdditionalEnergy = 2;
            PickupObjectDatabase.GetById(132).gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>().AdditionalEnergy = 2;

            PickupObjectDatabase.GetById(131).gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>().AdditionalEnergy = 1;
            PickupObjectDatabase.GetById(116).gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>().AdditionalEnergy = 1;
            PickupObjectDatabase.GetById(134).gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>().AdditionalEnergy = 1;


            new Hook(typeof(PassiveItem).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(AdditionalEnergyInitializer).GetMethod("PickupHook"));
            new Hook(typeof(PlayerItem).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(AdditionalEnergyInitializer).GetMethod("PickupHook_2"));


        }
        public static void PickupHook(Action<PassiveItem, PlayerController> orig, PassiveItem self, PlayerController player)
        {
            orig(self, player);
            if (self.gameObject.GetComponent<ModulePrinterCore.AdditionalItemEnergyComponent>() != null && player.HasPassiveItem(ModulePrinterCore.ModulePrinterCoreID))
            {
                var fx = player.PlayEffectOnActor(PowerUpVFX, new Vector3(0, 1.25f));
                fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("power_up");
            }
            if (self.gameObject.GetComponent<BasicStatPickup>() != null && player.HasPassiveItem(ModulePrinterCore.ModulePrinterCoreID))
            {
                if (self.gameObject.GetComponent<BasicStatPickup>().IsMasteryToken == true)
                {
                    var fx = player.PlayEffectOnActor(PowerUpVFX, new Vector3(0, 1.25f));
                    fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("power_up");
                }
            }
            var obj = self.gameObject.GetComponent<ShittyVFXAttacher>();
            if (obj)
            {
               UnityEngine.Object.Destroy(obj);
            }
        }
        public static void PickupHook_2(Action<PlayerItem, PlayerController> orig, PlayerItem self, PlayerController player)
        {
            orig(self, player);
            var obj = self.gameObject.GetComponent<ShittyVFXAttacher>();
            if (obj)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
    }
}
