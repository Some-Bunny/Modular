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
            LongDescription = "Reduces Rate Of Fire and Damage by 30%. Shoot 5 (+2 per stack) times the projectiles." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
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
            h.LabelDescription = "Reduces Rate Of Fire and Damage by 30%.\nShoot 5 (" + StaticColorHexes.AddColorToLabelString("+2", StaticColorHexes.Light_Orange_Hex) + ") times the projectiles.";
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
            printer.RegisterAction(Stats_AdditionalVolleyModifiers);
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                ChargeSpeed_Process = ProcessFireRate,
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            player.stats.AdditionalVolleyModifiers += Stats_AdditionalVolleyModifiers;
            player.stats.RecalculateStats(player);
            printer.OnPostProcessProjectile += PPP;
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 1.3f;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 0.7f;
        }
        private void Stats_AdditionalVolleyModifiers(ProjectileVolleyData obj)
        {
            Toolbox.ModifyVolley(obj, Stored_Core.Owner, 2 + (this.ReturnStack(Stored_Core) * 2), 16, 2);
        }



        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            player.stats.RecalculateStats(player);

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.DeregisterAction(Stats_AdditionalVolleyModifiers);
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            player.stats.AdditionalVolleyModifiers -= Stats_AdditionalVolleyModifiers;
            player.stats.RecalculateStats(player);
        }
    }
}

