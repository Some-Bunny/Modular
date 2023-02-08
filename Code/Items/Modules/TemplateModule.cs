using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
namespace ModularMod
{
    public class TestModuleProper : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TestModuleProper))
        {
            Name = "Damage Module",
            Description = "Damage Up",
            LongDescription = "Increases Damage by\n20% (+20% per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            SpriteResource = "ModularMod/Sprites/Items/Modules/Core/damage_tier1_module",
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Damage Module " + h.ReturnTierLabel();
            h.LabelDescription = "Increases Damage by\n20% (" + StaticColorHexes.AddColorToLabelString("+20%", StaticColorHexes.Light_Orange_Hex) + ")";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
        }
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + (0.2f * stack);
        }
    }
}
*/
