using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class OverclockedMagazines : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(OverclockedMagazines))
        {
            Name = "Overclocked Magazines",
            Description = "BRRAP",
            LongDescription = "Multiplies Rate Of Fire and Clip Size by 3 (+1 per stack). Greatly increases spread and reduces damage." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("overclockedmagazine_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("overclockedmagazine_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Overclocked Magazines " + h.ReturnTierLabel();
            h.LabelDescription = "Multiplies Rate Of Fire and Clip Size by 3 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ").\nGreatly increases spread and reduces damage.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.AdditionalWeightMultiplier = 0.8f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 15;

            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.ChangeMuzzleFlash((PickupObjectDatabase.GetById(95) as Gun).muzzleFlashEffects);
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Machine_Gun",
                FireRate_Process = ProcessFireRate,
                ClipSize_Process = ProcessClipSize,
                Accuracy_Process = ProcessAccuracy,
            };
            modularGunController.statMods.Add(this.gunStatModifier);
            player.stats.RecalculateStats(player);
            printer.OnPostProcessProjectile += PPP;
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / (2 + (this.ReturnStack(modulePrinterCore)));
        }
        public int ProcessClipSize(int f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 2 + ((this.ReturnStack(modulePrinterCore)));
        }
        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 3;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 0.66f;
            p.baseData.speed *= 2;
            p.UpdateSpeed();
        }


        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            player.stats.RecalculateStats(player);

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.RevertMuzzleFlash();
            if (modularGunController.statMods.Contains(this.gunStatModifier)) { modularGunController.statMods.Remove(this.gunStatModifier); }
            player.stats.RecalculateStats(player);
        }
    }
}

