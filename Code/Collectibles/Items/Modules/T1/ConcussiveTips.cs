using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class ConcussiveTips : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ConcussiveTips))
        {
            Name = "Concussive Tips",
            Description = "Hit Harder, Differently",
            LongDescription = "Increases knockback force by 50% (+50% per stack), boss damage by 25% (+25% per stack), and grants a small chance to stun enemies." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("conc_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("conc_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Concussive Tips " + h.ReturnTierLabel();
            h.LabelDescription = "Increases Knockback force by 50% (" + StaticColorHexes.AddColorToLabelString("+50%", StaticColorHexes.Light_Orange_Hex) + "),\nboss damage by 25% (" + StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + ")\nand adds a small chance to stun enemies.";
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
            p.BossDamageMultiplier *=  1 + (0.25f * stack);
            p.baseData.force *= 1 + (0.5f * stack);
            p.StunApplyChance = 1 - (1 / (1 + 0.625f * stack));
            p.AppliesStun = true;
            p.AppliedStunDuration = 1.5f + (0.5f * stack);
        }
    }
}

