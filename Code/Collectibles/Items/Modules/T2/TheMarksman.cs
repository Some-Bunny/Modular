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
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class TheMarksman : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TheMarksman))
        {
            Name = "The Marksman",
            Description = "Version First",
            LongDescription = "Hitting an enemy grants a 10% damage boost, up to 10 (+5 per stack) consecutive hits. Missing resets the damage bonus." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("marksman_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("marksman_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "The Marksman " + h.ReturnTierLabel();
            h.LabelDescription = "Hitting an enemy grants a 10% damage boost,\nup to 10 (" + StaticColorHexes.AddColorToLabelString("+5", StaticColorHexes.Light_Orange_Hex) + ") consecutive hits.\nMissing resets the damage bonus.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 10;
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            GameObject VFX = new GameObject("Marksman_VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 30);

            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("marksmanhit_005"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("MarksmanAnimation").GetComponent<tk2dSpriteAnimation>();
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
            HitCap = (5 * this.ReturnStack(modulePrinter)) + 5;
        }

        public void OPEH(ModulePrinterCore modulePrinter, PlayerController player, AIActor enemy, Projectile p)
        {
            if (HitCap <= Hits) { return; }
            Hits++;
            var vfx = enemy.PlayEffectOnActor(HitOrMissVFX, new Vector3(0, 2));
            vfx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("marksman_hit");
            AkSoundEngine.PostEvent("Play_BOSS_Rat_Cheese_Jump_01", player.gameObject);
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 1 + (0.1f * Hits);
            p.OnDestruction += this.HandleProjectileDestruction;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnPreEnemyHit -= OPEH;
        }
        private void HandleProjectileDestruction(Projectile source)
        {
            if (source && source.PossibleSourceGun && !source.HasImpactedEnemy)
            {
                if (Hits > 0)
                {
                    AkSoundEngine.PostEvent("Play_BOSS_Punchout_Punch_Hit_01", source.PossibleSourceGun.CurrentOwner.gameObject);
                    var vfx = source.PossibleSourceGun.CurrentOwner.PlayEffectOnActor(HitOrMissVFX, new Vector3(0, 2));
                    vfx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("marksman_miss");                    
                    Hits = 0;
                }
            }
        }
        private int Hits = 0;
        private int HitCap = 10;
    }
}

