using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class HeavyweightRounds : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HeavyweightRounds))
        {
            Name = "Heavyweight Rounds",
            Description = "Plonk",
            LongDescription = "Increases Damage by 50% (+50%), and bullet scale by 25% (+25%) but reduces player bullet speed by 50% (+50% hyperbolically per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("heavyrounds_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("heavyrounds_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.7f;
            h.LabelName = "Heavyweight Rounds" + h.ReturnTierLabel();
            h.LabelDescription = "Increases Damage by 50% (" + StaticColorHexes.AddColorToLabelString("+50%", StaticColorHexes.Light_Orange_Hex) + "),\nand bullet scale by 25% (" + StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + ")\nbut reduces player bullet speed by 50% (" + StaticColorHexes.AddColorToLabelString("+50% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").";
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
            p.baseData.damage *= 1 + (0.5f * stack);
            p.AdditionalScaleMultiplier *= 1 + (0.25f * stack);
            p.baseData.speed *= 1 - (1 - (1 / (1 + 0.5f * stack)));
            p.UpdateSpeed();
        }
    }
}

