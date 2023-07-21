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
    public class VoltaicTethers : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(VoltaicTethers))
        {
            Name = "Voltaic Tethers",
            Description = "Loaded",
            LongDescription = "Massively decreases Accuracy, and doubles Reload Time. Projectiles will stick to walls and tether electricity to players and other nearby tether nodes. (+Tether Range and Damage per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("voltaictethers_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("voltaictethers_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Voltaic Tethers " + h.ReturnTierLabel();
            h.LabelDescription = "Massively decreases Accuracy, and doubles Reload Time.\nProjectiles will stick to walls and tether\nelectricity to players and other nearby tether nodes.\n(" + StaticColorHexes.AddColorToLabelString("+Tether Range and Damage", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.AdditionalWeightMultiplier = 0.8f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 15;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.02f) { return; }
            p.baseData.speed *= 0.5f;
            p.UpdateSpeed();
            p.pierceMinorBreakables = true;
            int stack = 1;
            var tethers = p.gameObject.AddComponent<VoltaicTetherComponent>();
            tethers.DPS = 15 * stack;
            tethers.PylonRange = 5 * stack;
            tethers.PlayerRange = 10 * stack;

            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += 2;

            var mod = p.gameObject.GetOrAddComponent<StickyProjectileModifier>();
            mod.stickyContexts.Add(new StickyProjectileModifier.StickyContext() { CanStickToTerrain = true });
            mod.OnStick += H;
            mod.OnStickyDestroyed += H2;
        }


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Voltaic",
                Reload_Process = ProcessFireRate,
                Accuracy_Process = ProcessAccuracy,
            };

            this.stickyContext = new StickyProjectileModifier.StickyContext()
            {
                CanStickToTerrain = true,
                CanStickEnemies = false
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            printer.stickyContexts.Add(this.stickyContext);

            printer.OnPostProcessProjectile += PPP;

            printer.OnProjectileStickAction += H;
            printer.OnStickyDestroyAction += H2;
        }

        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * (2 + (this.ReturnStack(modulePrinterCore)));
        }
        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 3;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.speed *= 0.5f;
            p.UpdateSpeed();
            p.pierceMinorBreakables = true;
            int stack = this.ReturnStack(modulePrinterCore);
            var tethers = p.gameObject.AddComponent<VoltaicTetherComponent>();
            tethers.DPS = 15 * stack;
            tethers.PylonRange = 5 * stack;
            tethers.PlayerRange = 10 * stack;

            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += 2;
        }

        public void H(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            var tethers = stick.GetComponent<VoltaicTetherComponent>();
            if (tethers) { tethers.PylonRange *= 2.5f; }
            Destroy(stick, 20);
        }

        public void H2(GameObject stick, StickyProjectileModifier comp, PlayerController p)
        {
            var obj = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.overrideMidairDeathVFX, stick.transform.position, Quaternion.identity);
            Destroy(obj, 2);
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup) { }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.OnProjectileStickAction -= H;
            modulePrinter.OnStickyDestroyAction -= H2;
            if (modulePrinter.stickyContexts.Contains(this.stickyContext))
            {
                modulePrinter.stickyContexts.Remove(this.stickyContext);
            }
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
    }
}

