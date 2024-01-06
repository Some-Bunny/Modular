using Alexandria.ItemAPI;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    /*
    public class LuckyShot : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LuckyShot))
        {
            Name = "Lucky Shot",
            Description = "Lucky",
            LongDescription = "+50% (+50% per stack) Clip Size. Guarantees 2 (+1 per stack) Critical Shot every clip. +25% (+25% per stack) Crit Damage.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("luckyshot_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("luckyshot_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Lucky Shot " + h.ReturnTierLabel();
            h.LabelDescription = "+50% (" + StaticColorHexes.AddColorToLabelString("+50%") + ") Clip Size.\nGuarantees 2 (" + StaticColorHexes.AddColorToLabelString("+1") + ") " + StaticColorHexes.AddColorToLabelString("Critical Shots", StaticColorHexes.Light_Purple_Hex) + " every clip.\n+25% (" + StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + ") " + StaticColorHexes.AddColorToLabelString("Crit Damage", StaticColorHexes.Light_Purple_Hex) + ".";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.CRIT);
            h.AddModuleTag(BaseModuleTags.BASIC);

            h.AdditionalWeightMultiplier = 0.45f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.AddToGlobalStorage();
            ID = h.PickupObjectId;
        }
        public static int ID;
        public float CritCalc(float baseChance)
        {
            if (Procs == 0) { return baseChance; }
            return baseChance += (1 - Stored_Core.Owner.CurrentGun.PercentageOfClipLeft(+1));
        }

        public float CritDamageCalc(float baseChance)
        {
            return baseChance += 0.25f * this.ReturnStack(Stored_Core);
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            Procs = 2;
            this.CritContext = new CriticalHitComponent.CritContext()
            {
                CritChanceCalc = CritCalc,
                CritDamageCalc = CritDamageCalc,
            };
            modulePrinter.CritContexts.Add(this.CritContext);
            modulePrinter.OnCritProjectileRolled += CritRolled;
            modulePrinter.OnGunReloaded += OGR;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                ClipSize_Process = ProcessClipSize
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }
        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + ((modularGunController.Base_Clip_Size / 2) * modulePrinterCore.ReturnStack(this.LabelName));
        }
        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            Procs = this.ReturnStack(modulePrinterCore) + 1;
        }
        public void CritRolled(Projectile p, PlayerController player)
        {
            if (Procs == 0) { return; }
            Procs--;
        }
        public int Procs = 1;
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.CritContexts.Remove(this.CritContext);
            modulePrinter.OnCritProjectileRolled -= CritRolled;
            modulePrinter.OnGunReloaded -= OGR;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
    }
    */
}

