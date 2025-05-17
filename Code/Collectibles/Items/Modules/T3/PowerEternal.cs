using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class PowerEternal : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PowerEternal))
        {
            Name = "Power Eternal",
            Description = "Must Be Sated",
            LongDescription = "Small chance to gain a Power Cell upon slaying an enemy. (+Higher Chance per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("powereternal_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("powereternal_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Power Eternal " + h.ReturnTierLabel();
            h.LabelDescription = "Small chance to gain a Power Cell upon slaying an enemy.\n(" + StaticColorHexes.AddColorToLabelString("+Increased Chance", StaticColorHexes.Light_Orange_Hex) + ")";

            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AdditionalWeightMultiplier = 0.7f;
            h.EnergyConsumption = 5;
            h.IsUncraftable = true;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);
            ID = h.PickupObjectId;
        }
        public static int ID;



        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnKilledEnemy += OKE;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnKilledEnemy -= OKE;
        }

        public void OKE(ModulePrinterCore printer, PlayerController player, AIActor enemy)
        {
            if (UnityEngine.Random.value < 0.004f + (this.ReturnStack(printer) * 0.004f))
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(PowerCell.PowerCellID).gameObject, Vector3.zero, Quaternion.identity);
                PickupObject component3 = gameObject.GetComponent<PickupObject>();
                if (component3 != null)
                {
                    component3.CanBeDropped = false;
                    component3.Pickup(player);
                }
            }
        }
    }
}

