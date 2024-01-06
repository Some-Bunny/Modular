using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class TheSwarm : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TheSwarm))
        {
            Name = "The Swarm",
            Description = "Devourer",
            LongDescription = "Massively reduces damage. Projectiles gain massively increased lifetime, piercing and homing. (+Bounces, Pierces and stronger Homing per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("swarmer_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("swarmer_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "The Swarm " + h.ReturnTierLabel();
            h.LabelDescription = "Massively reduces damage.\nProjectiles gain massively increased lifetime,\npiercing and homing. (" + StaticColorHexes.AddColorToLabelString("+Bounces, Pierces and stronger Homing", StaticColorHexes.Light_Orange_Hex) + ")";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);
            h.AdditionalWeightMultiplier = 0.8f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.05f) { return; }
            int stack = 1;
            p.baseData.damage *= 0.2f;
            var aaaa = p.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
            aaaa.damageMultOnPierce *= 1.05f;
            aaaa.AmountOfPiercesBeforeFalloff = 10 + (stack * 5);
            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += (stack + 1);
            p.baseData.range *= 6;
            p.baseData.speed *= 0.6f;
            p.UpdateSpeed();

            ImprovedAfterImage yes = p.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.4f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0f, 0.3f, 1f, 1f);

            HomingModifier HomingMod = p.gameObject.GetOrAddComponent<HomingModifier>();
            HomingMod.AngularVelocity = 360 + (stack * 120);
            HomingMod.HomingRadius = 10 + (stack * 3.33f);
        }


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 0.2f;
            var aaaa = p.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
            aaaa.damageMultOnPierce *= 1.05f;
            aaaa.AmountOfPiercesBeforeFalloff = 10 + (stack*5);
            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += (stack + 1);
            p.baseData.range *= 6;
            p.baseData.speed *= 0.6f;
            p.UpdateSpeed();

            ImprovedAfterImage yes = p.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.4f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0f, 0.3f, 1f, 1f);

            HomingModifier HomingMod = p.gameObject.GetOrAddComponent<HomingModifier>();
            HomingMod.AngularVelocity = 360 + (stack * 120);
            HomingMod.HomingRadius = 10 + (stack * 3.33f);
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
    }
}

