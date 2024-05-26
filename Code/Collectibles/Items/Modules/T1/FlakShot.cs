using Alexandria.ItemAPI;
using DaikonForge.Tween;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class FlakShot : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(FlakShot))
        {
            Name = "Flak Shot",
            Description = "Better Than 1",
            LongDescription = "Reduces Damage by 30%. Projectiles split into 2 weaker projectiles that inherit all of the parent projectiles effects. (Split Projectile Damage per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("flakshot_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("flakshot_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.7f;
            h.LabelName = "Flak Shot" + h.ReturnTierLabel();
            h.LabelDescription = "Reduces Damage by 30%. Projectiles split into 2\nweaker projectiles that inherit all of the\nparent projectiles effects.\n("+StaticColorHexes.AddColorToLabelString("+Split Projectile Damage", StaticColorHexes.Light_Orange_Hex)+")";

            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.OverrideScrapCost = 5;
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (p.gameObject.GetComponent<ProjectileSplitController>() != null) { return; }
            if (UnityEngine.Random.value > 0.03f) { return; }
            int stack = 1;
            p.baseData.damage *= 0.7f;
            var split = p.gameObject.AddComponent<ProjectileSplitController>();
            split.dmgMultAfterSplit = 0.2f + (0.15f * stack);
            split.speedMultAfterSplit = 0.7f + (0.1f * stack);
            split.amtToSplitTo = 2;
            split.SplitsOnDestroy = true;
            split.isPurelyRandomizedSplitAngle = true;
            split.removeComponentAfterUse = true;
            split.sizeMultAfterSplit = 0.75f;
            split.DestroysProjectileOnSplit = false;
            split.DoesSplitPostProcess = true;

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
            if (p.gameObject.GetComponent<ProjectileSplitController>() != null) { return; }
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 0.7f;
            var split = p.gameObject.AddComponent<ProjectileSplitController>();
            split.dmgMultAfterSplit = 0.2f + (0.15f * stack);
            split.speedMultAfterSplit = 0.7f + (0.1f * stack);
            split.amtToSplitTo = 2;
            split.SplitsOnDestroy = true;
            split.isPurelyRandomizedSplitAngle = true;
            split.removeComponentAfterUse = true;
            split.sizeMultAfterSplit = 0.75f;
            split.DestroysProjectileOnSplit = false;
            split.DoesSplitPostProcess = true;
        }
    }
}

