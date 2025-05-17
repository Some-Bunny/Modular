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
    public class PlatedRage : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PlatedRage))
        {
            Name = "Plated Rage",
            Description = "Armored Core",
            LongDescription = "Increases Damage by\n3.33% (+3.33% per stack) for each piece of armor held." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("platedrage_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("platedrage_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Plated Rage " + h.ReturnTierLabel();
            h.LabelDescription = "Increases Damage by 3.33% (" + StaticColorHexes.AddColorToLabelString("+3.33%", StaticColorHexes.Light_Orange_Hex) + ")\nfor each piece of armor held.";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.CONDITIONAL);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.1f) { return; }
            p.baseData.damage *= 1 + ((0.0333f) * player.healthHaver.Armor);

        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + ((0.0333f * stack) * player.healthHaver.Armor);
        }
    }
}

