﻿using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class RecyclingNanites : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RecyclingNanites))
        {
            Name = "Recycling Nanites",
            Description = "Repurposed for something better",
            LongDescription = "While enabled, taking damage increases damage by\n+7.5% (+7.5% per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("recycler_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("recycler_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.75f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Recycling Nanites " + h.ReturnTierLabel();
            h.LabelDescription = "While enabled, taking damage increases damage by \n+7.5% (" + StaticColorHexes.AddColorToLabelString("+7.5%", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.RETALIATION);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.OverrideScrapCost = 3;

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnDamaged += OD;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnDamaged -= OD;
        }
        public void OD(ModulePrinterCore p, PlayerController player)
        {
            DamageTaken++;
            player.PlayEffectOnActor(VFXStorage.MachoBraceBurstVFX, new Vector3(0, 0));
            AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", player.gameObject);
        }
        private int DamageTaken = 0;
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + ((0.075f * stack) * DamageTaken);
        }
    } 
}

