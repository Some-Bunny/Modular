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
    public class CycleEfficiency : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CycleEfficiency))
        {
            Name = "Cycle Efficiency",
            Description = "In And Out",
            LongDescription = "Reloading is 15% (+15% per stack hyperbolically) faster, and and increases rate of fire by 12.5% (+12.5% per stack hyperbolically)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("cycleup_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("cycleup_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Cycle Efficiency " + h.ReturnTierLabel();
            h.LabelDescription = "Reloading is 15% (" + StaticColorHexes.AddColorToLabelString("+15% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ") faster\nand increases rate of fire by 12.5% (" + StaticColorHexes.AddColorToLabelString("+12.5% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);

            h.AddModuleTag(BaseModuleTags.BASIC);


            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                Reload_Process = ProcessReloadTime,
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
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.125f * stack)));
        }

        public float ProcessReloadTime(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.15f * stack)));
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);    
            GlobalSparksDoer.DoSingleParticle(p.sprite.WorldCenter, Toolbox.GetUnitOnCircle(player.CurrentGun.CurrentAngle + UnityEngine.Random.Range(165, 195), 3 + (1f * stack)), null, 2, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            modularGunController.ProcessStats();
        }
    }
}

