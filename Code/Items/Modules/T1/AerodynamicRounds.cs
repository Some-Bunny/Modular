using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class AerodynamicRounds : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(AerodynamicRounds))
        {
            Name = "Aerodynamic Carvings",
            Description = "Whizz Up",
            LongDescription = "Increases Damage by\n15% (+15% per stack) and increases projectile speed by 30% (+30% per stack) " + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("aerdynamic_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("aerdynamic_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.7f;
            h.LabelName = "Aerodynamic Carvings" + h.ReturnTierLabel();
            h.LabelDescription = "Increases Damage by 10% (" + StaticColorHexes.AddColorToLabelString("+10%", StaticColorHexes.Light_Orange_Hex) + ")\nand player bullet speed by 30% (" + StaticColorHexes.AddColorToLabelString("+30%", StaticColorHexes.Light_Orange_Hex) + ")";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + (0.1f * stack);
            p.baseData.speed *= 1 + (0.3f * stack);
            p.UpdateSpeed();
        }
    }
}

