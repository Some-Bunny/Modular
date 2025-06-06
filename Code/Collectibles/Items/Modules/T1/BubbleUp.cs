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
    public class BubbleUp : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BubbleUp))
        {
            Name = "Bubble Up",
            Description = "Pop!",
            LongDescription = "Increases Fire Rate by 20% (+20% hyperbolically per stack). Projectiles now stick to terrain and enemies, expanding into large bubbles and bursting, doing massive knockback. (+Burst Force and Damage per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("bubbleup_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("bubbleup_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Bubble Up " + h.ReturnTierLabel();
            h.LabelDescription = "Increases Fire Rate by 25% (" + StaticColorHexes.AddColorToLabelString("+25% hyperbolically") + ").\nProjectiles now stick to terrain and enemies,\nexpanding into large bubbles and bursting,\ndoing massive knockback. ("  + StaticColorHexes.AddColorToLabelString("+Burst Force and Damage") + ").";
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);

            h.AddModuleTag(BaseModuleTags.STICKY);

            h.AddToGlobalStorage();
            h.stickyContext = new StickyProjectileModifier.StickyContext()
            {
                CanStickToTerrain = true,
                CanStickEnemies = true
            };
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
            Data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            Data.effect = null;
            Data.force = 150;
            Data.pushRadius = 3.5f;
            Data.forceUseThisRadius = true;
            Data.damage = 1;
            GameObject VFX = new GameObject("Bubble");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 1);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 1);

            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("momentumplus_004"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = StaticCollections.Projectile_Animation;//Module.ModularAssetBundle.LoadAsset<GameObject>("MomentumPlusAnimation").GetComponent<tk2dSpriteAnimation>();
            Bubble = VFX;
        }
        public static GameObject Bubble;
        public static ExplosionData Data;
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.02f) { return; }
            
            var mod = p.gameObject.GetOrAddComponent<StickyProjectileModifier>();
            mod.stickyContexts.Add(new StickyProjectileModifier.StickyContext() { CanStickToTerrain = true, CanStickEnemies = true });
            mod.OnStick += H_S;
            mod.OnStickyDestroyed += H2;            
        }


        public void H_S(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            comp.StartCoroutine(DoTimer(stick,3));
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Stuck",
                FireRate_Process = PFR,
                
            };
            this.stickyContext = new StickyProjectileModifier.StickyContext()
            {
                CanStickToTerrain = true,
                CanStickEnemies = true
            };
            modulePrinter.stickyContexts.Add(this.stickyContext);

            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.OnProjectileStickAction += H;
            modulePrinter.OnStickyDestroyAction += H2;
            modulePrinter.OnPostProcessProjectile += PPP;

        }
        public void H(GameObject stick, StickyProjectileModifier comp, tk2dBaseSprite sprite, PlayerController p)
        {
            if (sprite)
            {
                sprite.renderer.enabled = false;
            }
            var bubble = UnityEngine.Object.Instantiate(Bubble, stick.transform);
            bubble.transform.Rotate(0, 0, UnityEngine.Random.value * 360);
            bubble.GetComponentInChildren<tk2dSpriteAnimator>().PlayAndDisableObject("bubble");
            comp.StartCoroutine(DoTimer(stick, 3));
        }

        public IEnumerator DoTimer(GameObject sticky, float DetTime = 5)
        {
            AkSoundEngine.PostEvent("Play_ENM_lizard_bubble_01", sticky.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_lizard_bubble_01", sticky.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_lizard_bubble_01", sticky.gameObject);

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
            Destroy(sticky);
            yield break;
        }


        public void H2(GameObject stick, StickyProjectileModifier comp, PlayerController p)
        {
            int stack = 1;
            if (Stored_Core != null)
            {
                stack = this.Stack();
            }
            var d = StaticExplosionDatas.CopyFields(Data);
            d.damage = 4f * stack;
            d.force = 75 * stack;
            d.ignoreList = new List<SpeculativeRigidbody>() { p.specRigidbody };
            Exploder.Explode(stick.transform.position, d, Vector2.zero);
            AkSoundEngine.PostEvent("Play_BOSS_Rat_Cheese_Burst_01", stick.gameObject);

            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, stick.transform.position, Quaternion.identity);
            blankObj.transform.localScale = Vector3.one * 0.3f;
            Destroy(blankObj, 2f);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (modularGunController.statMods.Contains(this.gunStatModifier)) 
            {
                modularGunController.statMods.Remove(this.gunStatModifier);
            }
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.stickyContexts.Remove(this.stickyContext);

            modulePrinter.OnProjectileStickAction -= H;
            modulePrinter.OnStickyDestroyAction -= H2;
            modulePrinter.OnPostProcessProjectile -= PPP;

        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.speed *= 0.7f;
            p.baseData.force *= 0.5f;
            p.UpdateSpeed();
        }

        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinter);
            return f - (f - (f / (1 + (0.25f * stack))));
        }
    }
}

