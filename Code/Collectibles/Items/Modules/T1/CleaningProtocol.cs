using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class CleaningProtocol : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CleaningProtocol))
        {
            Name = "Cleaning Protocol",
            Description = "BRUSH",
            LongDescription = "Deal 50% (+50% per stack) more damage to Jammed enemies.\nAll enemies take an additional 25% (+25% per stack) damage multiplier from various effects.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("cleaner_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("cleaner_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Cleaning Protocol " + h.ReturnTierLabel();
            h.LabelDescription = "Deal 50% (" + StaticColorHexes.AddColorToLabelString("+50%", StaticColorHexes.Light_Orange_Hex) + ") more damage to Jammed enemies.\nAll enemies take an additional 25% (" + StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + "\ndamage multiplier from various effects.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.1f) { return; }
            int stack = 1;
            p.BlackPhantomDamageMultiplier *= 1f + (0.5f * stack);
            p.CurseSparks = true;
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            ETGMod.AIActor.OnPreStart += OPEH;
        }

        public void OPEH(AIActor enemy)
        {
            if (enemy.CanTargetPlayers)
            {
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Fire, damageMultiplier = 1 + (0.25f + this.ReturnStack(Stored_Core))});
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Electric, damageMultiplier = 1 + (0.25f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Ice, damageMultiplier = 1 + (0.25f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Magic, damageMultiplier = 1 + (0.25f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Poison, damageMultiplier = 1 + (0.25f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Void, damageMultiplier = 1 + (0.25f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Water, damageMultiplier = 1 + (0.25f + this.ReturnStack(Stored_Core)) });
            }
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            ETGMod.AIActor.OnPreStart -= OPEH;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {                
            int stack = this.ReturnStack(modulePrinterCore);
            p.BlackPhantomDamageMultiplier *= 1f + (0.5f * stack);
            p.CurseSparks = true;     
        }
    }
}

