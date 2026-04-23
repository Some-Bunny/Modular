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
    public class MomentumNullifier : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MomentumNullifier))
        {
            Name = "Momentum Nullifier",
            Description = "Nope",
            LongDescription = "Converts 33% (+33% per stack) of your projectiles force into damage and chance to stun.\nNullifies all projectile knockback after.",
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("momentumnullifier_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("momentumnullifier_t2_alt_module");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Momentum Nullifier " + h.ReturnTierLabel();
            h.LabelDescription = $"Converts 33% ({StaticColorHexes.AddColorToLabelString("+33%")}) of your projectiles force into damage and chance to stun.\nNullifies all projectile knockback after.";

            h.AddModuleTag(BaseModuleTags.TRADE_OFF);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.OverrideScrapCost = 9;
            h.AdditionalWeightMultiplier = 0.7f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectileOneFrameDelay += OPPOFD;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectileOneFrameDelay -= OPPOFD;
        }


        public void OPPOFD(ModulePrinterCore modulePrinterCore, Projectile projectile, float a, PlayerController playerController, bool b)
        {
            float force = projectile.baseData.force * (0.333f * this.ReturnStack(modulePrinterCore));
            projectile.baseData.damage += force;
            projectile.baseData.force = 0;
            projectile.AppliesStun = true;
            projectile.StunApplyChance = force *= 0.02f;
            projectile.AppliedStunDuration = force * 0.333f;
        }

    }
}

