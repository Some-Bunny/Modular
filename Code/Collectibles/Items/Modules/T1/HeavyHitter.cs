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
    
    public class HeavyHitter : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HeavyHitter))
        {
            Name = "Heavy Hitter",
            Description = "Bonk Damage",
            LongDescription = "Deal 300% (+300% per stack) more knockback. Increases damage by 12.5% (+12.5% per stack).",
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("heavyhitter_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("heavyhitter_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.45f;

            h.LabelName = "Heavy Hitter" + h.ReturnTierLabel();
            h.LabelDescription = $"Deal 300% ({StaticColorHexes.AddColorToLabelString("+300%")}) more knockback.\nIncreases damage by 12.5% ({StaticColorHexes.AddColorToLabelString("+12.5%")}).";
            
            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;            
            ID = h.PickupObjectId;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.03f) { return; }
            p.baseData.damage *= 1.125f;
            p.baseData.speed *= 1.2f;
            p.baseData.force *= 3;
            p.UpdateSpeed();
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
            p.baseData.damage *= 1 + (0.125f * stack);
            p.baseData.speed *= 1.2f;
            p.baseData.force *= (3f * stack);

            p.UpdateSpeed();
        }
    }
    
}

