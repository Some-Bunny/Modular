using Alexandria.ItemAPI;
using Dungeonator;
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
    public class RoguesUltimatum : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RoguesUltimatum))
        {
            Name = "Rogues Ultimatum",
            Description = "Devourer",
            LongDescription = "Acts as 1 (+1 per stack) of every module you will own." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T4_Collection,
            ManualSpriteID = StaticCollections.Module_T4_Collection.GetSpriteIdByName("ROGUE_ULTIMATUM"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            //h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("swarmer_t3_module_alt");
            h.Tier = ModuleTier.Tier_Omega;
            h.LabelName = "ROGUES ULTIMATUM " + h.ReturnTierLabel();
            h.LabelDescription = "GRANTS A DAMAGE AND FIRE RATE BOOST\nBASED OFF HOW MUCH ENERGY YOU HAVE REMAINING.\n(" + StaticColorHexes.AddColorToLabelString("+DAMAGE AND RATE OF FIRE", StaticColorHexes.Light_Orange_Hex) + ")";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.red);
            h.AdditionalWeightMultiplier = 1f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.Label_Background_Color_Override = new Color32(255, 10, 10, 255);
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
            printer.OnPostProcessProjectile += PPP;
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / ReturnStatMult();
        }
        public float ReturnStatMult()
        {
            return 1 + ((Stored_Core.ReturnRemainingPower() / 7.5f) * this.ReturnStack(Stored_Core));
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            float mult = ReturnStatMult();
            p.baseData.damage *= mult;
            p.baseData.speed *= mult;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (modularGunController && gunStatModifier != null && modularGunController.statMods.Contains(this.gunStatModifier)) { modularGunController.statMods.Remove(this.gunStatModifier); }
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
    }
}

