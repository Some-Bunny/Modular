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
            LongDescription = "Taking Damage temporarily grants guaranteed Critical Hits and a faster reload for 15 (+15 per stack) seconds. Taking damage also permanantly increases clip size and accuracy by 3.3% (+3.3% per stack).\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
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
            h.LabelDescription = "Taking Damage temporarily grants\nguaranteed "+StaticColorHexes.AddColorToLabelString("Critical Hits", StaticColorHexes.Light_Purple_Hex) +" and a faster reload for 15 ("+StaticColorHexes.AddColorToLabelString("+15")+") seconds.\nTaking damage also permanantly boosts\nclip size and accuracy by 3.3% ("+StaticColorHexes.AddColorToLabelString("+3.3%")+").";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.RETALIATION);
            h.AddModuleTag(BaseModuleTags.CRIT);

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

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.CritContext = new CriticalHitComponent.CritContext()
            {
                GuaranteedCritCalc = CritCalc,
            };
            modulePrinter.CritContexts.Add(this.CritContext);
            modulePrinter.OnFrameUpdate += OFU;
            modulePrinter.OnDamaged += OD;

            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Reload_Process = PFR,
                Accuracy_Process = ProcessAccuracyRate,
                ClipSize_Process = ProcessClipSize,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }
        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + (int)(clip * (0.033f * Hits) * this.ReturnStack(modulePrinterCore));
        }

        public float ProcessAccuracyRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + (0.033f * stack)* Hits)));
        }

        private int Hits = 0;
        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (IsGuaranteed == true) { return f / 3; }
            return f;
        }

        public void OD(ModulePrinterCore modulePrinter, PlayerController player)
        {
            Hits++;
            int stack = this.ReturnStack(modulePrinter);
            T = 15 * stack;
            AkSoundEngine.PostEvent("Play_BOSS_cyborg_eagle_01", player.gameObject);
            Exploder.DoDistortionWave(player.sprite.WorldCenter, 0.2f * ConfigManager.DistortionWaveMultiplier, 0.25f * ConfigManager.DistortionWaveMultiplier, 7f,0.75f);
            if (extantVFX != null) { return; }
            extantVFX = player.PlayEffectOnActor(FocusVFX, new Vector3(0, 0.625f));
            extantVFX.GetComponent<tk2dSpriteAnimator>().Play("Focus_Start");
            extantVFX.transform.localScale = Vector3.one / 2;
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

        public bool CritCalc(float baseChance)
        {
            if (IsGuaranteed == true) { return true; }
            return false;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.CritContexts.Remove(this.CritContext);
            modulePrinter.OnFrameUpdate -= OFU;
            modulePrinter.OnDamaged -= OD;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);

        }

        public bool IsGuaranteed
        {
            get
            {
                return T > 0;
            }
        }
    }
}

