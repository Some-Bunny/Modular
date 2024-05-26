using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class LiquidMetal : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LiquidMetal))
        {
            Name = "Liquid Metal",
            Description = "Pierce Up",
            LongDescription = "Adds 1 (+1 per stack) Pierce, but reduces projectile speed by 20% (-20% per stack hyperbolically) " + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("liquidmetal_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static VFXPool GhostVFX;
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("liquidmetal_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.9f;
            h.LabelName = "Liquid Metal" + h.ReturnTierLabel();
            h.LabelDescription = "Adds 1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ")Pierce \nbut reduces projectile speed by 20% (" + StaticColorHexes.AddColorToLabelString("-20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            GhostVFX = (PickupObjectDatabase.GetById(228) as Gun).DefaultModule.projectiles[0].hitEffects.enemy;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.05f) { return; }
            int stack = 1;
            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += stack;
            p.baseData.speed *= 1 - (1 - (1 / (1 + 0.2f * stack))); //this formula fucking sucks lmao
            p.UpdateSpeed();
            p.hitEffects.enemy = GhostVFX;
            p.hitEffects.deathEnemy = GhostVFX;
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
            PierceProjModifier bounceProjModifier =  p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += stack;
            p.baseData.speed *= 1-(1 - (1 / (1+0.2f * stack))); //this formula fucking sucks lmao
            p.UpdateSpeed();
            p.hitEffects.enemy = GhostVFX;
            p.hitEffects.deathEnemy = GhostVFX;
        }
    }
}

