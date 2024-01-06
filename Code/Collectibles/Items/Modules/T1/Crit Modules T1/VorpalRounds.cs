using Alexandria.ItemAPI;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    /*
    public class VorpalRounds : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(VorpalRounds))
        {
            Name = "Vorpal Rounds",
            Description = "Pack A Punch",
            LongDescription = "Enables Critical Hits. 10% (+5% per stack) Crit Chance. 35% (+35% per stack) Crit Damage." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("vorpalrounds_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("vorpalrounds_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.55f;
            h.LabelName = "Vorpal Rounds" + h.ReturnTierLabel();
            h.LabelDescription = StaticColorHexes.AddColorToLabelString("Enables Critical Hits", StaticColorHexes.Light_Purple_Hex) + 
                ".\n+10% (" + StaticColorHexes.AddColorToLabelString("+10%", StaticColorHexes.Light_Orange_Hex) + ") "+ StaticColorHexes.AddColorToLabelString("Crit Chance", StaticColorHexes.Light_Purple_Hex)+
                ".\n+35% (" + StaticColorHexes.AddColorToLabelString("+35%", StaticColorHexes.Light_Orange_Hex) + ") " + StaticColorHexes.AddColorToLabelString("Crit Damage", StaticColorHexes.Light_Purple_Hex)+".";
            
            h.AddModuleTag(BaseModuleTags.CRIT);
            
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;            
            ID = h.PickupObjectId;
        }
        public static int ID;

        public float CritCalc(float baseChance)
        {
            return baseChance += 0.1f * this.ReturnStack(Stored_Core);
        }

        public float CritDamageCalc(float baseChance)
        {
            return baseChance += 0.35f * this.ReturnStack(Stored_Core);
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

