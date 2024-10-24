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
    public class BypassedRecoil : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BypassedRecoil))
        {
            Name = "Disabled Dampeners",
            Description = "Recoil Up",
            LongDescription = "Increases Rate Of Fire by\n30% (+30% per stack hyperbolically), and reduces reload time by 15% (+15% per stack hyperbolically) but disables recoil dampeners." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("nodampener_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("nodampener_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Disabled Dampeners " + h.ReturnTierLabel();
            h.AdditionalWeightMultiplier = 0.7f;
            h.LabelDescription = "Increases Rate Of Fire by 30% (" + StaticColorHexes.AddColorToLabelString("+30% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")\nand reduces reload time by 15% (" + StaticColorHexes.AddColorToLabelString("+15% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")\nbut disables your weapons recoil dampener.";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            h.OverrideScrapCost = 3;

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
                ChargeSpeed_Process = ProcessFireRate,
                Reload_Process = ProcessReloadTime
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            modularGunController.ProcessStats();
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.3f * stack)));
        }
        public float ProcessReloadTime(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.15f * stack)));
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            player.knockbackDoer.ApplyKnockback(Toolbox.GetUnitOnCircle(player.CurrentGun.CurrentAngle - 180, 1), (20 * stack) * (1 + (p.baseData.damage / 60)));
            p.baseData.speed *= 1 + (0.2f * stack);
            p.baseData.force *= 1 + (0.2f * stack);
        }
    }
}

