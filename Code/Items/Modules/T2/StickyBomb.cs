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
            Description = "Exchange Rate",
            LongDescription = "Increases Accuracy by 20% (+20% hyperbolically per stack), projectiles now leave sticky bombs on enemies that\nexplode after 5 (-15% hyperbolically per stack) seconds." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
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
            h.LabelDescription = "Increases Accuracy by 20% (" + StaticColorHexes.AddColorToLabelString("+20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + "),\nprojectiles now leave sticky bombs on enemies that\nexplode after 5 ("+ StaticColorHexes.AddColorToLabelString("-15% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ") seconds.";
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.EnergyConsumption = 2;
            h.AddToGlobalStorage();

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static tk2dSpriteAnimation mineAnimation;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Mines",
                Accuracy_Process = PFR,
            };
            modularGunController.statMods.Add(this.gunStatModifier);
            modulePrinter.OnProjectileStickAction += H;
            modulePrinter.OnStickyDestroyAction += H2;
            modulePrinter.OnPreProjectileStickAction += OPPS;

        }

        public void OPPS(Projectile p, PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", p.gameObject);
            p.hitEffects.HandleProjectileDeathVFX(p.transform.position, 0, null, Vector2.zero, Vector2.zero);
            p.sprite.SetSprite(p.sprite.Collection.GetSpriteIdByName("mine_idle_001"));

        }

        public void H(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            comp.StartCoroutine(DoTimer(stick, 5 - (5 - (5 / (1 + (0.15f * this.Stack()))))));
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
            Exploder.Explode(stick.transform.position, StaticExplosionDatas.explosiveRoundsExplosion, Vector2.zero);
        }





        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (modularGunController.statMods.Contains(this.gunStatModifier)) 
            {
                modularGunController.statMods.Remove(this.gunStatModifier);
            }
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

