using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class BifurificationBarrel : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BifurificationBarrel))
        {
            Name = "Bifurcated Barrel",
            Description = "Two-For-One",
            LongDescription = "Increases rate of fire by 20% (+20% hyperbolically per stack), clip size by 20% (+20% per stack), but makes you fire in a V-formation.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("bifurificationbarrel_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("bifurificationbarrel_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Bifurcated Barrel " + h.ReturnTierLabel();
            h.LabelDescription = "Increases rate of fire by 20% (" + StaticColorHexes.AddColorToLabelString("+20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + "),\nclip size by 20% (" + StaticColorHexes.AddColorToLabelString("+20%", StaticColorHexes.Light_Orange_Hex) + "),\nbut makes you fire in a V-formation.";
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.OverrideScrapCost = 10;
            h.AdditionalWeightMultiplier = 0.75f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.IsUncraftable = true;

            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RegisterAction(Stats_AdditionalVolleyModifiers);
            modulePrinter.OnPostProcessProjectile += PPP;

            player.stats.AdditionalVolleyModifiers += Stats_AdditionalVolleyModifiers;
            player.stats.RecalculateStats(player);

            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                ChargeSpeed_Process = ProcessFireRate,
                ClipSize_Process = ProcessClipSize
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.DeregisterAction(Stats_AdditionalVolleyModifiers);
            player.stats.AdditionalVolleyModifiers -= Stats_AdditionalVolleyModifiers;
            player.stats.RecalculateStats(player);
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.speed *= 1.15f;
            p.UpdateSpeed();
        }
        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + ((modularGunController.Base_Clip_Size / 5) * modulePrinterCore.ReturnStack(this.LabelName));
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.2f * stack)));
        }
        private void Stats_AdditionalVolleyModifiers(ProjectileVolleyData obj)
        {
            Toolbox.ModifyVolley(obj, Stored_Core.Owner, 1, 54, 3, 3, 0, null, 1);
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            player.stats.RecalculateStats(player);
        }
    }
}

