﻿using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;


namespace ModularMod
{
    public class MonetaryValue : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MonetaryValue))
        {
            Name = "Monetary Value",
            Description = "Investment",
            LongDescription = "Gain 0.5% (+0.5% per stack) damage for each casing you have. Your casing value is increased by 25% (+25% per stack) every floor." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("moneyvalue_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("moneyvalue_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.9f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Monetary Value " + h.ReturnTierLabel();
            h.LabelDescription = "Gain 0.5% damage (" + StaticColorHexes.AddColorToLabelString("+0.5%", StaticColorHexes.Light_Orange_Hex) + ") \nfor each casing you have.\nYour casing value is increased by 25% (" + StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + ") every floor.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            h.OverrideScrapCost = 7;
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.3f) { return; }
            p.baseData.damage *= 1 + ((player.carriedConsumables.Currency / 2000));
        }
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnNewFloorStarted += ONFS;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnNewFloorStarted -= ONFS;
        }
        public void ONFS(ModulePrinterCore modulePrinter, PlayerController player)
        {
            player.carriedConsumables.Currency += player.carriedConsumables.Currency * (int)(0.25f * this.ReturnStack(modulePrinter));

        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 1 + ((player.carriedConsumables.Currency / 2000)*this.ReturnStack(modulePrinterCore));
        }
    } 
}
