using Alexandria.ItemAPI;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class CLEANSLATE : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CLEANSLATE))
        {
            Name = "CLEAN SLATE",
            Description = "Devourer",
            LongDescription = "Acts as 1 (+1 per stack) of every module you will own." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T4_Collection,
            ManualSpriteID = StaticCollections.Module_T4_Collection.GetSpriteIdByName("CLEANSLATE"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            //h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("swarmer_t3_module_alt");
            h.Tier = ModuleTier.Tier_Omega;
            h.LabelName = "CLEAN SLATE " + h.ReturnTierLabel();
            h.LabelDescription = "ONCE POWERED, FORFEITS ANY\nPOWERED MODULES IN EXCHANGE FOR ITEMS.";
            h.powerConsumptionData = new PowerConsumptionData()
            {
                FirstStack = 0,
                AdditionalStacks = 0,
                OverridePowerDescriptionLabel = "USES NO POWER.",
                OverridePowerManagement = null,
            };
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.red);
            h.AdditionalWeightMultiplier = 1f;
            h.Offset_LabelDescription = new Vector2(0.25f, -0.5f);
            h.Offset_LabelName = new Vector2(0.25f, 2.25f);
            h.Label_Background_Color_Override = new Color32(255, 10, 10, 100);
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnAnyModulePowered += OAMP;

            int l = 0;
            for (int i = printer.ModuleContainers.Count - 1; i > -1; i--)
            {
                var moduleContainer = printer.ModuleContainers[i];
                var shitfuck = printer.ModuleContainers.Where(self => self.LabelName != this.LabelName && self.Count > 0);
                if (shitfuck.Count() > 0)
                {
                    var mod = BraveUtility.RandomElement<ModulePrinterCore.ModuleContainer>(shitfuck.ToList()).defaultModule;
                    int count = printer.RemoveModule(mod, 1000);
                    l += count;
                    for (int sad  = 0; sad < count; sad++)
                    {
                        GameManager.Instance.StartCoroutine(DoDrop(l * 0.2f, mod,  player));
                    }
                }
            }

        }

        public IEnumerator DoDrop(float waitTime, DefaultModule module, PlayerController p)
        {
            yield return new WaitForSeconds(waitTime);
            VFXStorage.DoFancyDestroyOfModules(1, p, module);

            Vector2 pos = p.sprite.WorldCenter;
            VFXStorage.HighPriestClapVFX.SpawnAtPosition(pos);
            yield return new WaitForSeconds(1f);
            PickupObject pickupObject = LootEngine.GetItemOfTypeAndQuality<PickupObject>(ReturnRandomQuality(), GameManager.Instance.RewardManager.ItemsLootTable, false);
            LootEngine.SpawnItem(pickupObject.gameObject, pos - new Vector2(0.5f, 0.5f), Vector2.up, 0f, true, false, false);

            yield break;
        }

        public ItemQuality ReturnRandomQuality()
        {
            int i = UnityEngine.Random.Range(1, 9);
            switch (i)
            {
                case 1:
                    return PickupObject.ItemQuality.D;
                case 2:
                    return PickupObject.ItemQuality.D;
                case 3:
                    return PickupObject.ItemQuality.D;
                case 4:
                    return PickupObject.ItemQuality.C;
                case 5:
                    return PickupObject.ItemQuality.C;
                case 6:
                    return PickupObject.ItemQuality.B;
                case 7:
                    return PickupObject.ItemQuality.A;
                case 8:
                    return PickupObject.ItemQuality.S;
                default:
                    return PickupObject.ItemQuality.D;
            }
        }


        public void OAMP(ModulePrinterCore printer, DefaultModule module, int i)
        {
            int l = 0;

            if (module != this)
            {
                int count = printer.RemoveModule(module, 1000);
                l += count;
                for (int sad = 0; sad < count; sad++)
                {
                    GameManager.Instance.StartCoroutine(DoDrop(l * 0.2f, module, printer.Owner));
                }
            }
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnAnyModulePowered -= OAMP;
        }
    }
}

