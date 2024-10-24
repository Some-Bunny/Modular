using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class CounterProductivity : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CounterProductivity))
        {
            Name = "Counter Productivity",
            Description = "Exchange Rate",
            LongDescription = "Divides Clip Size and Reload Time by 2 (+1 per stack). Slightly increases fire rate. (+More fire rate per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("counerproduction_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("counerproduction_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Counter Productivity " + h.ReturnTierLabel();
            h.AdditionalWeightMultiplier = 0.75f;
            h.LabelDescription = "Divides Clip Size and Reload Time by 2 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ").\nSlightly increases fire rate  (" + StaticColorHexes.AddColorToLabelString("+More Fire Rate", StaticColorHexes.Light_Orange_Hex) + ").";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "CouterProduction",
                FireRate_Process = PFR,
                ClipSize_Process = ProcessClipSize,
                Reload_Process = ProcessReloadTime,
                ChargeSpeed_Process = PFR,

            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
        }

        public float ProcessReloadTime(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / (1 + this.ReturnStack(modulePrinterCore));
        }

        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip / (1 + this.ReturnStack(modulePrinterCore));
        }

        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinter);
            return f - (f - (f / (1 + (0.2f * stack))));
        }
    }
}

