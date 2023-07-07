using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class STRING_EMPTY : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(STRING_EMPTY))
        {
            Name = "STRING_EMPTY",
            Description = "One For Each Finger",
            LongDescription = "Slightly reduces Rate Of Fire, and reduces Damage by 33%. Shoot 5 times the projectiles (+2 Projectiles per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("error_mod_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("error_mod_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "String.EMPTY " +StaticColorHexes.AddColorToLabelString("sprite Tier_3_Label", StaticColorHexes.Blue_Color_Hex);
            h.LabelDescription = "[Primary_Description_Placeholder]\n[secondaryEffect_Placeholder]\n"+StaticColorHexes.AddColorToLabelString("NullReferenceException: (999+)\nObject not set to an instance of an object.\nDefaultModule.ReturnTeritaryEffectText()\nTileBreaker.ReturnDescriptionLabel()", StaticColorHexes.Red_Color_Hex);
            h.SetTag("modular_module");
            h.AddColorLight(Color.white);
            h.AdditionalWeightMultiplier = 0.4f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 15;
            h.IsUncraftable = true;
            h.powerConsumptionData.OverridePowerDescriptionLabel = "Uses DefaultModule.PowerConsumption(ModuleQuality.Tier_3, -1)\n(" + StaticColorHexes.AddColorToLabelString("DefaultModule.AdditionalStackPowerConsumption(ModuleQuality.Tier_3, -1)", StaticColorHexes.Orange_Hex) + ")";
            h.EnergyConsumption = 0;
            h.AddToGlobalStorage();


            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            DummySpriteObject = VFX;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static GameObject DummySpriteObject;

        public override bool CanBeDisabled(ModulePrinterCore modulePrinter, ModularGunController modularGunController)
        {
            AkSoundEngine.PostEvent("Play_Glitch_Y", modulePrinter.gameObject);
            return false;
        }

        public override void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_Glitch_Y", player.gameObject);
            modulePrinter.PowerModule(this);
            var fx = player.PlayEffectOnActor(EmergencyResponse.WarnVFX, new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)));
            fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("warn");
            var sprite = fx.GetComponent<tk2dBaseSprite>();
            sprite.usesOverrideMaterial = true;
            Material material = sprite.renderer.material;
            material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
            material.SetFloat("_GlitchInterval", 0.1f);
            material.SetFloat("_DispProbability", 0.6f);
            material.SetFloat("_DispIntensity", 0.024f);
            material.SetFloat("_ColorProbability", 0.7f);
            material.SetFloat("_ColorIntensity", 0.1f);
        }
    

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {

        }
    }
}

