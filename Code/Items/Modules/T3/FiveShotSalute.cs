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
    public class FiveShotSalute : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(FiveShotSalute))
        {
            Name = "Five-Shot Salute",
            Description = "One For Each Finger",
            LongDescription = "Halves Rate Of Fire, and reduces Damage by 33%. Shoot 5 times the projectiles (+1 Projectile per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("fiveshot_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("fiveshot_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Five-Shot Salute " + h.ReturnTierLabel();
            h.LabelDescription = "Halves Rate Of Fire, and reduces Damage by 33%.\nShoot 5 times the projectiles (" + StaticColorHexes.AddColorToLabelString("+1 Projectile", StaticColorHexes.Light_Orange_Hex) + ").";
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
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
            };
            modularGunController.statMods.Add(this.gunStatModifier);
            player.stats.AdditionalVolleyModifiers += Stats_AdditionalVolleyModifiers;
            player.stats.RecalculateStats(player);
            printer.OnPostProcessProjectile += PPP;
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 2;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 0.66f;
        }
        private void Stats_AdditionalVolleyModifiers(ProjectileVolleyData obj)
        {
            GunVolleyModificationItem.AddDuplicateOfBaseModule(obj, this.Stored_Core.Owner, 3 + this.ReturnStack(Stored_Core), 18, 2);
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            player.stats.RecalculateStats(player);

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (modularGunController.statMods.Contains(this.gunStatModifier)) { modularGunController.statMods.Remove(this.gunStatModifier); }
            player.stats.AdditionalVolleyModifiers -= Stats_AdditionalVolleyModifiers;
            player.stats.RecalculateStats(player);
        }
    }
}

