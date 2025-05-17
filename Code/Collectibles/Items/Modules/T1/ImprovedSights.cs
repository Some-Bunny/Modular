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
    public class ImprovedSights : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ImprovedSights))
        {
            Name = "Improved Sights",
            Description = "X Marks The Spot",
            LongDescription = "Improves Accuracy by 33% (+33% hyperbolically per stack). Increases shotspeed by 25% (+25% per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("improvedsights_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("improvedsights_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Improved Sights " + h.ReturnTierLabel();
            h.LabelDescription = "Improves Accuracy by 33% (" + StaticColorHexes.AddColorToLabelString("+33% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").\nIncreases shotspeed by 33% (" + StaticColorHexes.AddColorToLabelString("+33%", StaticColorHexes.Light_Orange_Hex) + ").";

            h.AddModuleTag(BaseModuleTags.BASIC);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Accuracy_Process = ProcessFireRate,
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
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.33f * stack)));
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.speed *= 1 + (0.33f * this.ReturnStack(modulePrinterCore));
            p.UpdateSpeed();
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            modularGunController.ProcessStats();
        }
    }
}

