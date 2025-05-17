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
    public class PlusOne : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PlusOne))
        {
            Name = "Plus One",
            Description = "One Better",
            LongDescription = "Improves projectiles by exactly +1 (+1 per stack) damage." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("plusone_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("plusone_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.66f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Plus One " + h.ReturnTierLabel();
            h.LabelDescription = "Improves projectiles by\nexactly +1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") damage.";


            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.1f) { return; }
            p.StartCoroutine(this.FrameDelay(p, modulePrinterCore, 1));
            p.HasDefaultTint = true;
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.StartCoroutine(this.FrameDelay(p, modulePrinterCore, null));
            p.HasDefaultTint = true;
            p.AdjustPlayerProjectileTint(Color.yellow, 1);
        }
        public IEnumerator FrameDelay(Projectile p, ModulePrinterCore modulePrinterCore, int? overrideStack)
        {
            yield return null;
            int stack = overrideStack ?? this.ReturnStack(modulePrinterCore);
            float f_1 = p.baseData.damage * 1 + (0.5f * stack);
            float f_2 = p.baseData.damage + stack;
            p.baseData.damage = Mathf.Min(f_1, f_2);
            yield break;
        }

    } 
}

