using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class EmergencyResponse : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(EmergencyResponse))
        {
            Name = "Emergency Response",
            Description = "Fight Up",
            LongDescription = "Taking damage grants a 2x damage up and 2x fire rate up that slowly degrades over 15 (+7.5 per stack) seconds." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("emergencymod_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("emergencymod_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.8f;
            h.LabelName = "Emergency Response" + h.ReturnTierLabel();
            h.LabelDescription = "Taking damage grants a double damage\nand double fire rate boost\nthat degrades over 15 (" + StaticColorHexes.AddColorToLabelString("+7.5", StaticColorHexes.Light_Orange_Hex) + ") seconds.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("warning_003"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();
           
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("WarningVFXAnimation").GetComponent<tk2dSpriteAnimation>();


            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);

            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(tk2dAnim, "warn", new Dictionary<int, string>()
            {
                {4, "Play_ENM_hammer_target_01"},
                {6, "Play_ENM_hammer_target_01"},
                {8, "Play_ENM_hammer_target_01"},
            });
            WarnVFX = VFX;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static GameObject WarnVFX;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnDamaged += OD;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
            };
            modularGunController.statMods.Add(this.gunStatModifier);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnDamaged -= OD;
            if (modularGunController && gunStatModifier != null && modularGunController.statMods.Contains(this.gunStatModifier)) { modularGunController.statMods.Remove(this.gunStatModifier); }
        }

        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / DamageMult;
        }

        public void OD(ModulePrinterCore modulePrinter, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinter);
            player.StartCoroutine(DoDamageUp(stack, player, modulePrinter.ModularGunController));
            var fx = player.PlayEffectOnActor(WarnVFX, new Vector3(0, 1.625f));
            fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("warn");
        }
        public IEnumerator DoDamageUp(int stack, PlayerController player, ModularGunController modularGunController)
        {
            Active = true;
            yield return null;
            Active = false;
            float e = 0;
            float d = 7.5f + (7.5f * stack);
            while (e < d)
            {
                e += BraveTime.DeltaTime;
                DamageMult = Mathf.Lerp(2, 1, e / d);
                modularGunController.ProcessStats();
                if (Active == true) { yield break; }
                if (UnityEngine.Random.value < (e / d))
                {
                    GlobalSparksDoer.DoSingleParticle(Toolbox.RandomPositionOnSprite(player.sprite), Toolbox.GetUnitOnCircle(UnityEngine.Random.Range(-180, 180), Mathf.Lerp(6, 0, e / d)), null, 2, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                }
                yield return null;
            }
            Active = false;
            yield break;
        }

        private bool Active = false;
        public float DamageMult = 1;

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= DamageMult;
            p.baseData.speed *= DamageMult;
            p.UpdateSpeed();
        }
    }
}

