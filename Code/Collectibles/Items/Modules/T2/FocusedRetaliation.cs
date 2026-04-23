using Alexandria.ItemAPI;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    
    public class FocusedRetaliation : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(FocusedRetaliation))
        {
            Name = "Focused Retaliation",
            Description = "(o)",
            LongDescription = "Enemies that damage you are Marked, taking 2x (+1x per stack) damage and have their damage caps ignored. Taking damage also grants temporary movement speed, reload speed and a permanent 5% (+5% per stack) accuracy and clip size boost.",
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("focusedretaliation_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("focusedretaliation_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Focused Retaliation " + h.ReturnTierLabel();
            h.LabelDescription = $"Enemies that damage you are Marked,\ntaking 2x ({StaticColorHexes.AddColorToLabelString("+1x")}) damage and have their damage caps ignored.\nTaking damage also grants temporary movement speed,\nreload speed and a permanent 5% ({StaticColorHexes.AddColorToLabelString("+5%")}) accuracy and clip size boost.";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.RETALIATION);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AdditionalWeightMultiplier = 0.5f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.IsUncraftable = true;
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();
            ID = h.PickupObjectId;

            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("fosucVFX_009"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = StaticCollections.Generic_VFX_Animation;


            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);
            FocusVFX = VFX;
        }
        public static int ID;
        public static GameObject FocusVFX;
        public static ProjectileImpactVFXPool HitEffect = Guns.Fightsabre.DefaultModule.projectiles[0].hitEffects;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate += OFU;
            modulePrinter.OnDamaged += OD;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Reload_Process = PFR,
                Accuracy_Process = ProcessAccuracyRate,
                ClipSize_Process = ProcessClipSize,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.Owner.OnHitByProjectile += OHBP;
            modulePrinter.VoluntaryMovement_Modifier += ModifySpeed;
            modulePrinter.OnPreEnemyHit += OPEH;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate -= OFU;
            modulePrinter.OnDamaged -= OD;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.Owner.OnHitByProjectile -= OHBP;
            modulePrinter.VoluntaryMovement_Modifier -= ModifySpeed;
            modulePrinter.OnPreEnemyHit -= OPEH;

        }
        #region Stat Changes

        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + (int)(clip * (0.05f * Hits) * this.ReturnStack(modulePrinterCore));
        }

        public float ProcessAccuracyRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + (0.05f * stack)* Hits)));
        }
        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (T > 0) { return f / 3; }
            return f;
        }

        public float ModifySpeed(Vector2 currentVelocity, ModulePrinterCore core, PlayerController player)
        {
            return (T > 0 ? 0.3f : 0);
        }
        #endregion

        public void OD(ModulePrinterCore modulePrinter, PlayerController player)
        {
            Hits++;
            int stack = this.ReturnStack(modulePrinter);
            T = 15 * stack;
            AkSoundEngine.PostEvent("Play_BOSS_cyborg_eagle_01", player.gameObject);
            Exploder.DoDistortionWave(player.sprite.WorldCenter, 0.2f * ConfigManager.DistortionWaveMultiplier, 0.25f * ConfigManager.DistortionWaveMultiplier, 7f,0.75f);
            if (extantVFX != null) { return; }
            extantVFX = player.SmarterPlayEffectOnActor(FocusVFX, new Vector3(0, 0.625f));
            extantVFX.GetComponent<tk2dSpriteAnimator>().Play("Focus_Start");
            extantVFX.transform.localScale = Vector3.one / 2;
        }

        public void OPEH(ModulePrinterCore modulePrinter, PlayerController player, AIActor aIActor, Projectile projectile)
        {
            if (aIActor != null && aIActor.GetEffect("mdl:marked") != null)
            {
                float damage = projectile.baseData.damage;
                projectile.baseData.damage *= 1 + this.ReturnStack(modulePrinter);
                projectile.StartCoroutine(FrameDelay(projectile, damage));
            }
        }

        public IEnumerator FrameDelay(Projectile p, float DmG)
        {
            bool ig = p.ignoreDamageCaps;
            ProjectileImpactVFXPool Ef = p.hitEffects;
            p.hitEffects = HitEffect;
            p.ignoreDamageCaps = true;
            yield return null;
            p.baseData.damage = DmG;
            p.hitEffects = Ef;
            p.ignoreDamageCaps = ig;
            yield break;
        }


        public GameObject extantVFX;
        private float T = 0;
        public void OFU(ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (T > 0)
            {
                T -= BraveTime.DeltaTime;
            }
            if (T <= 0 && extantVFX != null)
            {
                extantVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("Focus_End");
                extantVFX = null;
            }
        }



        public void OHBP(Projectile projectile, PlayerController playerController)
        {
            if (projectile.Owner != null && projectile.Owner is AIActor enemy)
            {
                projectile.Owner.ApplyEffect(new MarkedEffect()
                {
                    effectIdentifier = "mdl:marked",
                    AffectsEnemies = true,
                    resistanceType = EffectResistanceType.None,
                    duration = 10000f,
                    AppliesTint = true,
                    AppliesDeathTint = true,
                    OverheadVFX = null,//MarkedEffect.MarkedVFX,
                    PlaysVFXOnActor = false,
                    AffectsPlayers = false,
                    stackMode = GameActorEffect.EffectStackingMode.Refresh,
                });
                enemy.PlayEffectOnActor(MarkedEffect.MarkedVFX, new Vector3(0, 1.25f));

            }
        }
        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);
            data.Add(Hits);
        }
        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameSerialize(data);
            Hits = (int)data[0];
        }
        private int Hits = 0;
    }
}

