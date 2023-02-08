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
    public class PerfectionistMomentum : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PerfectionistMomentum))
        {
            Name = "Perfectionist Momentum",
            Description = "Keeping It Up",
            LongDescription = "Grants a 3.3% Fire rate and Damage upgrade for every room cleared without taking damage, capped at 10 (+10 per stack) rooms.\nBonuses reset when you take damage." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("perfectmomentum_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("perfectmomentum_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Perfectionist Momentum " + h.ReturnTierLabel();
            h.LabelDescription = "Grants a 3.3% Fire Rate and Damage upgrade for\nevery room cleared without taking damage,\ncapped at 10 (" + StaticColorHexes.AddColorToLabelString("+10", StaticColorHexes.Light_Orange_Hex) + ") rooms.\n"+ StaticColorHexes.AddColorToLabelString("Bonuses reset when you take damage.", StaticColorHexes.Dark_Red_Hex);
            h.AddToGlobalStorage();
            h.AdditionalWeightMultiplier = 0.8f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);

            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("momentumplus_004"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("MomentumPlusAnimation").GetComponent<tk2dSpriteAnimation>();
            MomentumPlusObject = VFX;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public static GameObject MomentumPlusObject;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnRoomCleared += ORC;
            modulePrinter.OnDamaged += OnDamaged;

            mod = new ModuleGunStatModifier()
            {
                Name = "Momentum",
                FireRate_Process = PFR
            };
            modularGunController.statMods.Add(mod);
        }

        public ModuleGunStatModifier mod;

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnRoomCleared -= ORC;
            modulePrinter.OnDamaged -= OnDamaged;
            if (modularGunController.statMods.Contains(mod)) 
            {
                modularGunController.statMods.Remove(mod);
            }
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            RoomPerfectCountCap = 10 * this.ReturnStack(modulePrinter);
        }
        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            return f - (f - (f / (1 + 0.033f * RoomPerfectCount)));
        }

        public void ORC(ModulePrinterCore modulePrinterCore, PlayerController player, RoomHandler room)
        {
            if (room.PlayerHasTakenDamageInThisRoom == true) { return; }
            if (RoomPerfectCount < RoomPerfectCountCap)
            {
                var fx = player.PlayEffectOnActor(MomentumPlusObject, new Vector3(0, 2.25f));
                fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
                AkSoundEngine.PostEvent("Play_BOSS_energy_shield_01", player.gameObject);
                modulePrinterCore.ModularGunController.ProcessStats();
                RoomPerfectCount++;
            }
        }
        public void OnDamaged(ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (RoomPerfectCount == 0) { return; }
            if (player.IsInCombat == false) { return; }
            modulePrinterCore.ModularGunController.ProcessStats();
            var fx = player.PlayEffectOnActor(MomentumPlusObject, new Vector3(0, 2.25f));
            AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Lash_01", player.gameObject);
            fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("break");
            RoomPerfectCount = 0;
        }
        public int RoomPerfectCount = 0;
        public int RoomPerfectCountCap = 10;

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= 1 + (0.033f * RoomPerfectCount);
        }
    }
}

