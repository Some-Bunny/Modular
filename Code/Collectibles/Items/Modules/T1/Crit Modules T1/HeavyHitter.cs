using Alexandria.ItemAPI;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    /*
    public class HeavyHitter : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HeavyHitter))
        {
            Name = "Heavy Hitter",
            Description = "Charge A Punch",
            LongDescription = "Critical Hits deal 100% (+100% per stack) more damage, but divides Crit Chance by 2 (+1 per stack). Crit Chance cannot go below 5%." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("heavyhitter_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("heavyhitter_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.45f;

            h.LabelName = "Heavy Hitter" + h.ReturnTierLabel();
            h.LabelDescription = StaticColorHexes.AddColorToLabelString("Critical Hits", StaticColorHexes.Light_Purple_Hex) + 
                " deal +200% (" + StaticColorHexes.AddColorToLabelString("+100%", StaticColorHexes.Light_Orange_Hex) + ") more damage,\n"+
                "but divides "+ StaticColorHexes.AddColorToLabelString("Crit Chance", StaticColorHexes.Light_Purple_Hex) + " by 2 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ").\n"+ 
                StaticColorHexes.AddColorToLabelString("Crit Chance", StaticColorHexes.Light_Purple_Hex) + " cannot go below 5%.";
            
            h.AddModuleTag(BaseModuleTags.CRIT);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;            
            ID = h.PickupObjectId;
        }
        public static int ID;

        public float CritDamageCalc(float baseChance)
        {
            return baseChance * this.ReturnStack(Stored_Core) + 1;
        }
        public float CritCalc(float baseChance)
        {
            if (baseChance <= 0) { return 0.05f; }
            float f = baseChance / this.ReturnStack(Stored_Core) + 1;
            f = Mathf.Max(0.05f, f);
            return f;
        }
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.CritContext = new CriticalHitComponent.CritContext()
            {
                CritChanceCalc = CritCalc,
                CritDamageCalc = CritDamageCalc,
            };
            modulePrinter.CritContexts.Add(this.CritContext);
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.CritContexts.Remove(this.CritContext);
        }
    }
    */
}

