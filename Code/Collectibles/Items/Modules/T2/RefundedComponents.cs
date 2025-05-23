﻿using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class RefundedComponents : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RefundedComponents))
        {
            Name = "Refunded Components",
            Description = "Exchange Rate",
            LongDescription = "Missed shots are refunded back into your clip. Gain a 25% boost to damage when you miss, up to 10 (+10 per stack) misses.\nLanding a hit will use all stored damage." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("refundstuff_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("refundstuff_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Refunded Components" + h.ReturnTierLabel();
            h.LabelDescription = "Missed shots are refunded back into your clip.\nGain a 33% boost to damage when you miss, up to 10 " + StaticColorHexes.AddColorToLabelString("+10", StaticColorHexes.Light_Orange_Hex) + " misses.\nLanding a hit will use all stored damage.";

            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            GameObject VFX = new GameObject("HitOrMiss_VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);

            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("hit_dat_004"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();


            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("RefundAnimation").GetComponent<tk2dSpriteAnimation>();
            HitOrMissVFX = VFX;

            ID = h.PickupObjectId;
        }
        public static int ID;
        private static GameObject HitOrMissVFX;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnPreEnemyHit += OPEH;
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            MissCap = 10 * this.ReturnStack(modulePrinter);
        }

        public void OPEH(ModulePrinterCore modulePrinter, PlayerController player, AIActor enemy, Projectile p)
        {
            if (Misses == 0) { return; }
            if (player != null)
            {
                var vfx = player.SmarterPlayEffectOnActor(HitOrMissVFX, new Vector3(0, 2));
               
                if (vfx != null)
                {
                    vfx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("vfx_hit");
                }
                AkSoundEngine.PostEvent("Play_OBJ_box_uncover_01", player.gameObject);
            }
            if (p != null)
            {
                float mult = 1 + ((0.33f * Misses));
                float mult_ = 1 + ((0.5f * Misses));

                p.baseData.damage *= mult;
                p.baseData.force *= mult_;
                if (coroutine == null)
                {
                    coroutine = GameManager.Instance.StartCoroutine(DoWait());
                }
            }
        }
        private Coroutine coroutine; 
        public IEnumerator DoWait()
        {
            yield return new WaitForSeconds(0.075f);
            Misses = 0;
            coroutine = null;
            yield break;
        }



        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.OnDestruction += this.HandleProjectileDestruction;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnPreEnemyHit -= OPEH;
        }
        public int MissCap = 10;

        private void HandleProjectileDestruction(Projectile source)
        {
            if (source && source.PossibleSourceGun && !source.HasImpactedEnemy)
            {
                source.PossibleSourceGun.MoveBulletsIntoClip(1);
                if (Misses < MissCap)
                {
                    AkSoundEngine.PostEvent("Play_OBJ_compass_point_01", source.PossibleSourceGun.CurrentOwner.gameObject);

                    var vfx = source.PossibleSourceGun.CurrentOwner.SmarterPlayEffectOnActor(HitOrMissVFX, new Vector3(0, 2));
                    vfx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("vfx_miss");
                    Misses++;
                }
            }
        }
        private int Misses = 0;
    }
}

