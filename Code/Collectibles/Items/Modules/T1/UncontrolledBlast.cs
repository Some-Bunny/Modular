using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class UncontrolledBlast : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(UncontrolledBlast))
        {
            Name = "Uncontrolled Blast",
            Description = "Spray And Pray",
            LongDescription = "Massively boosts clip size and fire rate (+More Fire Rate per stack),but reduces damage and gives virtually uncontrollable spread. (+Even More Spread per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("uncontrolledblast_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("uncontrolledblast_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Uncontrolled Blast " + h.ReturnTierLabel();
            h.LabelDescription = "Massively boosts clip size and fire rate (" + StaticColorHexes.AddColorToLabelString("+More Fire Rate", StaticColorHexes.Light_Orange_Hex) + "),\nbut reduces damage and gives " + StaticColorHexes.AddColorToLabelString("virtually uncontrollable spread", StaticColorHexes.Red_Color_Hex) + ".\n(" + StaticColorHexes.AddColorToLabelString("+Even More Spread", StaticColorHexes.Light_Orange_Hex) + ")";
            h.OverrideScrapCost = 7;
            h.AdditionalWeightMultiplier = 0.2f;
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.IsSpecialModule = true;
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                ClipSize_Process = ProcessClipSize,
                Accuracy_Process = ProcessAccuracy,
                ChargeSpeed_Process = ProcessFireRate,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modularGunController.ProcessStats();
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }

        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / (1 + (this.ReturnStack(modulePrinterCore)));
        }
        public int ProcessClipSize(int f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return (int)(f * 2f);
        }

        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return (f + 20) * (4.5f * stack);
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 0.8f;
            p.baseData.speed *= 2;
            p.UpdateSpeed();
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            modularGunController.ProcessStats();
        }
    }
}

