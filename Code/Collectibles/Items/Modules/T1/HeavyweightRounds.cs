﻿using Alexandria.ItemAPI;
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
            LongDescription = "Increases Damage by 33% (+33%), and bullet size by 25% (+25%) but reduces player bullet speed by 50% (+50% hyperbolically per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
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
            h.LabelDescription = "Increases Damage by 33% (" + StaticColorHexes.AddColorToLabelString("+33%", StaticColorHexes.Light_Orange_Hex) + "),\nand bullet size by 25% (" + StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + ")\nbut reduces player bullet speed by 50% (" + StaticColorHexes.AddColorToLabelString("+50% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.075f) { return; }
            p.baseData.damage *= 1.33f;
            p.AdditionalScaleMultiplier *= 1.25f;
            p.baseData.speed *= 0.5f;
            p.baseData.force *= 0.75f;
            p.UpdateSpeed();
        }


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + (0.33f * stack);
            p.AdditionalScaleMultiplier *= 1 + (0.25f * stack);
            p.baseData.speed *= 1 - (1 - (1 / (1 + 0.5f * stack)));
            p.baseData.force *= 1 - (1 - (1 / (1 + 0.25f * stack)));
            p.UpdateSpeed();
        }
    }
}

