using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace ModularMod
{
    public class TemporaryShield : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TemporaryShield))
        {
            Name = "Temporary Shield",
            Description = "Block It",
            LongDescription = "Prevents 1 (+1 per stack) instances of damage per floor. Blocking damage activates on-hit effects. Increases the probability of certain modules appearing." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("tempshield_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("tempshield_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.6f;
            h.LabelName = "Temporary Shield" + h.ReturnTierLabel();
            h.LabelDescription = "Prevents 1 ("+ StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex)+") instances of damage per floor.\nBlocking damage activates on-hit effects.\n"+StaticColorHexes.AddColorToLabelString("Increases the probability of certain modules appearing.", StaticColorHexes.Pink_Hex);

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.RETALIATION);

            h.OverrideScrapCost = 5;
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            ID = h.PickupObjectId;

            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("temparmor_006"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("GenericVFXAnimation").GetComponent<tk2dSpriteAnimation>();

            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 4);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 4);

            ShieldVFX = VFX;
        }
        public static int ID;
        public static GameObject ShieldVFX;
        public GameObject Inst;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnNewFloorStarted += ONFS;
            GlobalModuleStorage.AlterModuleWeight += ModuleWeight;
            player.healthHaver.ModifyDamage += ModifyIncomingDamage;
            Inst = player.PlayEffectOnActor(ShieldVFX, new Vector3(0, -1f));
            Inst.GetComponent<tk2dSpriteAnimator>().Play("shield_start");
        }

        private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if (Blocks > 0)
            {
                AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Trigger_01", source.gameObject);

                Delegate dodge = source.GetEventDelegate("OnDamaged");
                if (dodge != null)
                {
                    dodge.DynamicInvoke(new object[] { 0, 100, CoreDamageTypes.Void, DamageCategory.Normal, Vector2.zero });
                }
                Delegate dodge2 = source.m_player.GetEventDelegate("OnReceivedDamage");
                if (dodge2 != null)
                {
                    dodge2.DynamicInvoke(new object[] { source.m_player });
                }
                Blocks--;

                if (Inst != null && Blocks == 0)
                {
                    Inst.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("shield_break");
                    AkSoundEngine.PostEvent("Play_OBJ_boulder_break_01", source.gameObject);

                }

                args.InitialDamage = 0;
                args.ModifiedDamage = 0;
                source.IsVulnerable = false;
                UnityEngine.Object.Instantiate(VFXStorage.TeleportDistortVFX, source.sprite.WorldCenter, Quaternion.identity);
                source.StartCoroutine(StartInv(source));
            }
        }

        public IEnumerator StartInv(HealthHaver h)
        {

            h.m_player.sprite.usesOverrideMaterial = true;
            h.m_player.SetOverrideShader(ShaderCache.Acquire("Brave/ItemSpecific/MetalSkinShader"));


            yield return new WaitForSeconds(1);
            AkSoundEngine.PostEvent("Play_OBJ_metalskin_end_01", h.gameObject);
            h.m_player.ClearOverrideShader();
            h.IsVulnerable = true;
            yield break;
        }


        public float ModuleWeight(DefaultModule module, float f)
        {
            if (module.ContainsTag(BaseModuleTags.RETALIATION) || module.ContainsTag(BaseModuleTags.DEFENSIVE)) { return f *= 1.5f; }
            return f;
        }


        public void ONFS(ModulePrinterCore core, PlayerController p)
        {
            Blocks = this.ReturnStack(core);
            if (Inst == null)
            {
                Inst = p.PlayEffectOnActor(ShieldVFX, new Vector3(0, -2f));
                Inst.GetComponent<tk2dSpriteAnimator>().Play("shield_start");
            }
        }

        public int Blocks = 1;

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.healthHaver.ModifyDamage -= ModifyIncomingDamage;
            modulePrinter.OnNewFloorStarted -= ONFS;
            GlobalModuleStorage.AlterModuleWeight -= ModuleWeight;
            if (Inst != null)
            {
                Inst.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("shield_break");
            }
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + (0.15f * stack);
            p.baseData.speed *= 1 + (0.3f * stack);
            p.UpdateSpeed();
        }
    }
}

