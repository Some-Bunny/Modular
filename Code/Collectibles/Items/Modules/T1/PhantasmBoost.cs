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
    public class PhantasmBoost : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PhantasmBoost))
        {
            Name = "Phantasm Boost",
            Description = "Cold As The Grave",
            LongDescription = "Deal an additional 50% (+50% per stack) damage to Jammed enemies. Reload 15% (+15% hyperbolically per stack) faster after every kill. Kill count resets AFTER reloading. Projectiles will now be able to travel through internal walls and pierce debris objects." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("gravecooling_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("gravecooling_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Phantasm Boost " + h.ReturnTierLabel();
            h.LabelDescription = "Deal an additional 50% ("+ StaticColorHexes.AddColorToLabelString("+50%", StaticColorHexes.Light_Orange_Hex) + ") more damage to Jammed enemies.\nReload 20% (" + StaticColorHexes.AddColorToLabelString("+20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ") faster after every kill.\nBonus resets AFTER reloading.\nProjectiles will now be able to travel through internal walls and pierce debris objects.";
            h.AdditionalWeightMultiplier = 0.66f;
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;

            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("spooky3"));

            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = StaticCollections.Generic_VFX_Animation;

            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);

            WarnVFX = VFX;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }
        public static int ID;
        public static GameObject WarnVFX;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.2f) { return; }
            p.PenetratesInternalWalls = true;
            p.pierceMinorBreakables = true;
            p.Awake();
            int stack = 1;
            p.BlackPhantomDamageMultiplier *= 1f + (0.5f * stack);
            p.CurseSparks = true;
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Reload_Process = ProcessReloadTime
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modularGunController.ProcessStats();

            modulePrinter.OnKilledEnemy += OKE;
            modulePrinter.OnGunReloaded += OGR;
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", player.gameObject);
            player.StartCoroutine(DoWait(player));
        }

        private IEnumerator DoWait(PlayerController p)
        {
            while (p.CurrentGun.IsReloading)
            {
                yield return null;
            }
            Kills = 0;
            yield break;
        }

        public void OKE(ModulePrinterCore modulePrinter, PlayerController player, AIActor enemy)
        {
            if (enemy.IsNormalEnemy) { Kills++; 
                var fx = player.PlayEffectOnActor(WarnVFX, new Vector3(0, 1.625f));
                fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("ghostboop");
            }
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnKilledEnemy -= OKE;
            modulePrinter.OnGunReloaded -= OGR;
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
        public float ProcessReloadTime(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + (0.20f * stack) * Kills)));
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            modularGunController.ProcessStats();
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.BlackPhantomDamageMultiplier *= 1f + (0.5f * stack);
            p.CurseSparks = true;
            p.PenetratesInternalWalls = true;
            p.pierceMinorBreakables = true;
            p.Awake();
        }
        public int Kills = 0;
    }
}

