﻿using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class LineUp : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LineUp))
        {
            Name = "Line Up",
            Description = "Knock 'Em Down",
            LongDescription = "Adds 1 Pierce, Reduce Damage by 20% (-20% per stack hyperbolically) But each enemy pierced increases projectile damage by 1.75x (+0.75x per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("lineup_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("lineup_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Line Up " + h.ReturnTierLabel();
            h.LabelDescription = "Adds 1 Pierce, Reduce Damage by 15% (" + StaticColorHexes.AddColorToLabelString("-15% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")\n" +
                StaticColorHexes.AddColorToLabelString("But", StaticColorHexes.Dark_Red_Hex) + " each enemy pierced increases\nprojectile damage by 1.75x " +
                StaticColorHexes.AddColorToLabelString("+0.75x", StaticColorHexes.Light_Orange_Hex) + ".";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            PierceImpact = (PickupObjectDatabase.GetById(545) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static GameObject PierceImpact;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 - (1 - (1 / (1 + 0.15f * stack)));

            var aaaa = p.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
            aaaa.damageMultOnPierce = 1 + (0.75f * stack);
            aaaa.AmountOfPiercesBeforeFalloff = 2 + stack;
            aaaa.OnPierce += OP;
            p.baseData.range += 3;

            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += stack;
        }
        public void OP(Projectile p, SpeculativeRigidbody speculativeRigidbody)
        {
            var VFX = UnityEngine.Object.Instantiate(PierceImpact, p.sprite.WorldCenter - new Vector2(1.5f, 0), Quaternion.identity);
            Destroy(VFX, 2);
        }
    }
}
