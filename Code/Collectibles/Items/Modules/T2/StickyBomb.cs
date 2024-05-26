using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class StickyBomb : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(StickyBomb))
        {
            Name = "Sticky Bomb",
            Description = "KA-BLEWY",
            LongDescription = "Increases Accuracy by 20% (+20% hyperbolically per stack), projectiles now leave sticky bombs on enemies that\nexplode after 10 (-25% hyperbolically per stack) seconds. (+Sticky Bomb Damage per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("stickybombs_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("stickybombs_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Sticky Bomb " + h.ReturnTierLabel();
            h.LabelDescription = "Increases Accuracy by 20% (" + StaticColorHexes.AddColorToLabelString("+20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + "),\nprojectiles now leave sticky bombs on enemies that\nexplode after 10 (" + StaticColorHexes.AddColorToLabelString("-25% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ") seconds.\n(" + StaticColorHexes.AddColorToLabelString("+Sticky Bomb Damage", StaticColorHexes.Light_Orange_Hex) + ")";
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);

            h.AddModuleTag(BaseModuleTags.STICKY);

            h.AddToGlobalStorage();
            h.stickyContext = new StickyProjectileModifier.StickyContext()
            {
                CanStickToTerrain = false,
                CanStickEnemies = true
            };
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
            Data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            Data.damage = 8;

        }

        public static ExplosionData Data;
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.02f) { return; }
            p.baseData.speed *= 2f;
            p.pierceMinorBreakables = true;
            p.UpdateSpeed();

            var mod = p.gameObject.GetOrAddComponent<StickyProjectileModifier>();
            mod.stickyContexts.Add(new StickyProjectileModifier.StickyContext() { CanStickToTerrain = false, CanStickEnemies = true });
            mod.OnStick += H_S;
            mod.OnStickyDestroyed += H2;
            mod.OnPreStick += OPPS;
        }


        public void H_S(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            comp.StartCoroutine(DoTimer(stick, 12.5f - (12.5f - (12.5f / (1 + (0.25f * 1))))));
        }

        public static tk2dSpriteAnimation mineAnimation;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Mines",
                Accuracy_Process = PFR,
            };
            modulePrinter.stickyContexts.Add(this.stickyContext);

            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.OnProjectileStickAction += H;
            modulePrinter.OnStickyDestroyAction += H2;
            modulePrinter.OnPreProjectileStickAction += OPPS;

        }

        public void OPPS(GameObject p, PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", p.gameObject);
            //p.hitEffects.HandleProjectileDeathVFX(p.transform.position, 0, null, Vector2.zero, Vector2.zero);
            var sprite = p.GetComponentInChildren<tk2dBaseSprite>();
            if (sprite)
            {
                sprite.SetSprite(StaticCollections.Projectile_Collection ,StaticCollections.Projectile_Collection.GetSpriteIdByName("mine_idle_001"));
            }
        }

        public void H(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            comp.StartCoroutine(DoTimer(stick, 12.5f - (12.5f - (12.5f / (1 + (0.25f * this.Stack()))))));
        }

        public IEnumerator DoTimer(GameObject sticky, float DetTime = 5)
        {
            float e = 0;
            while (e < DetTime)
            {
                if (sticky == null) { yield break; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            if (sticky == null) 
            {
                yield break; 
            }
            AkSoundEngine.PostEvent("Play_BOSS_mineflayer_trigger_01", sticky.gameObject);
            sticky.GetComponentInChildren<tk2dBaseSprite>().SetSprite(sticky.GetComponentInChildren<tk2dBaseSprite>().Collection.GetSpriteIdByName("mine_idle_002"));
            e = 0;
            while (e < 1)
            {
                if (sticky == null) { yield break; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            Destroy(sticky);
        }


        public void H2(GameObject stick, StickyProjectileModifier comp, PlayerController p)
        {
            int stack = 1;
            if (Stored_Core != null)
            {
                stack = this.Stack();
            }
            var d = StaticExplosionDatas.CopyFields(Data);
            d.damage = 6 + (2 * stack);
            Exploder.Explode(stick.transform.position, d, Vector2.zero);


        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {

            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.stickyContexts.Remove(this.stickyContext);

            modulePrinter.OnProjectileStickAction -= H;
            modulePrinter.OnStickyDestroyAction -= H2;
            modulePrinter.OnPreProjectileStickAction -= OPPS;
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
        }

        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinter);
            return f - (f - (f / (1 + (0.20f * stack))));
        }
    }
}

