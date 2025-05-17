using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace ModularMod
{
    public class IteratedDesign : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(IteratedDesign))
        {
            Name = "Iterated Design",
            Description = "Simply Better",
            LongDescription = "Grants a boost to some stats. (+Increased Stats per stack). Grants benefits unique to your current gun. (+Better Benefits per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("iterateddesign_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("iterateddesign_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Iterated Design " + h.ReturnTierLabel();
            h.LabelDescription = "Grants a boost to some stats. (" + StaticColorHexes.AddColorToLabelString("+Increased Stats", StaticColorHexes.Light_Orange_Hex) + ").\nGrants benefits unique to your current gun. ("+StaticColorHexes.AddColorToLabelString("+Better Benefits")+").";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.EnergyConsumption = 2;
            h.AdditionalWeightMultiplier = 0.7f;
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);
            ID = h.PickupObjectId;
        }
        public static int ID;

        public static int PlayerHasIteratedDesign(PlayerController player)
        {
            int c = GlobalModuleStorage.PlayerActiveModuleCount(player, IteratedDesign.ID);
            if (c > 0)
            {
                return c;
            }
            return 0;
        }

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnPostProcessProjectile += PPP;
            /*
            this.CritContext = new CriticalHitComponent.CritContext()
            {
                CritChanceCalc = CritCalc,
                CritDamageCalc = CritDamageCalc,
            };
            printer.CritContexts.Add(this.CritContext);
            */
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                ChargeSpeed_Process = ProcessFireRate,
                Accuracy_Process = ProcessAccuracy,
                Reload_Process = ProcessReload,
                ClipSize_Process = ProcessClipSize,
                Post_Calculation_ClipSize_Process = ProcessClipSizePostCalc,
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
        }
        public float CritCalc(float baseChance)
        {
            return baseChance;
        }

        public float CritDamageCalc(float baseChance)
        {
            return baseChance += 0.25f * this.ReturnStack(Stored_Core);
        }
        public int ProcessClipSizePostCalc(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            if (SpecialProcessGunSpecificClipPostCalc != null) { clip = SpecialProcessGunSpecificClipPostCalc(clip, stack, modulePrinterCore, player); }
            return clip;
        }
        public static Func<int, int, ModulePrinterCore, PlayerController, int> SpecialProcessGunSpecificClipPostCalc;
        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            if (SpecialProcessGunSpecificClip != null) { clip = SpecialProcessGunSpecificClip(clip, stack, modulePrinterCore, player); }
            return clip;
        }
        public static Func<int, int,ModulePrinterCore, PlayerController, int> SpecialProcessGunSpecificClip;

        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            if (SpecialProcessGunSpecificFireRate != null) { f = SpecialProcessGunSpecificFireRate(f, stack, modulePrinterCore, player); }
            return f * 1;
        }
        public static Func<float, int,  ModulePrinterCore, PlayerController, float> SpecialProcessGunSpecificFireRate;

        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            if (SpecialProcessGunSpecificAccuracy != null) { f = SpecialProcessGunSpecificAccuracy(f, stack, modulePrinterCore, player); }
            return f;
        }
        public static Func<float, int, ModulePrinterCore, PlayerController, float> SpecialProcessGunSpecificAccuracy;
        public float ProcessReload(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            if (SpecialProcessGunSpecificReload != null) { f = SpecialProcessGunSpecificReload(f, stack, modulePrinterCore, player); }
            return f;
        }
        public static Func<float, int, ModulePrinterCore, PlayerController, float> SpecialProcessGunSpecificReload;

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + (0.125f * stack);
            p.baseData.speed *= 1 + (0.15f * stack);
            p.baseData.force *= 2 * stack;
            p.baseData.range *= 2 * stack;

            p.AppliedStunDuration *= 1 + (0.3f * stack);

            p.BleedApplyChance *= 1 + (0.3f * stack);
            p.CharmApplyChance *= 1 + (0.3f * stack);
            p.CheeseApplyChance *= 1 + (0.3f * stack);
            p.FireApplyChance *= 1 + (0.3f * stack);
            p.FreezeApplyChance *= 1 + (0.3f * stack);
            p.PoisonApplyChance *= 1 + (0.3f * stack);
            p.SpeedApplyChance *= 1 + (0.3f * stack);
            p.StunApplyChance *= 1 + (0.3f * stack);

            p.BlackPhantomDamageMultiplier *= 1 + (0.125f * stack);

            p.UpdateSpeed();
            if (SpecialProcessGunSpecific != null)
            {
                SpecialProcessGunSpecific(modulePrinterCore, p, stack, player);
            }
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            //modulePrinter.CritContexts.Remove(this.CritContext);
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
        public static Action<ModulePrinterCore, Projectile, int, PlayerController> SpecialProcessGunSpecific;
    }
}

