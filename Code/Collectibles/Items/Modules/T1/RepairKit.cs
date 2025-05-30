﻿using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class RepairKit : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RepairKit))
        {
            Name = "Repair Kit",
            Description = "Up Keep",
            LongDescription = "Increases Damage by\n12.5% (+12.5% per stack). Restores 2 Armor on pickup." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("repairtool_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("repairtool_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.7f;
            h.LabelName = "Repair Kit " + h.ReturnTierLabel();
            h.LabelDescription = "Increases Damage by 12.5% (" + StaticColorHexes.AddColorToLabelString("+12.5%", StaticColorHexes.Light_Orange_Hex) + ").\nRestores 2 Armor on Pickup.";
            h.EnergyConsumption = 0.5f;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.OverrideScrapCost = 6;
            h.IsUncraftable = true;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);


            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.PlayEffectOnActor(VFXStorage.HealingSparklesVFX, new Vector3(0, 0));
            AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", player.gameObject);
            player.healthHaver.Armor += 2;
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {

        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage += (0.5f * stack);
            p.baseData.damage *= 1 + (0.1f * stack);
        }
    }
}

