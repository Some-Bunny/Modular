using Alexandria.ItemAPI;
using Gungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class VFXStorage
    {
        public static void AssignVFX()
        {
            RadialRing = (GameObject)ResourceCache.Acquire("Global VFX/HeatIndicator");
            TeleportDistortVFX = (PickupObjectDatabase.GetById(573) as ChestTeleporterItem).TeleportVFX;
            TeleportVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");
            RelodestoneContinuousSuckVFX = (PickupObjectDatabase.GetById(536) as RelodestoneItem).ContinuousVFX;
            GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
            foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
            {
                if (item is SkyRocket laser)
                {
                    DragunBoulderLandVFX = laser.ExplosionData.effect;
                    WarningImpactVFX = laser.LandingTargetSprite;

                }
            }
            HealingSparklesVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001");
            FriendlyElectricLinkVFX = (PickupObjectDatabase.GetById(298) as ComplexProjectileModifier).ChainLightningVFX;



            var machoBrace = PickupObjectDatabase.GetById(665) as MachoBraceItem;
            MachoBraceDustupVFX = machoBrace.DustUpVFX;
            MachoBraceBurstVFX = machoBrace.BurstVFX;
            AssetBundle bundle = ResourceManager.LoadAssetBundle("brave_resources_001");
            LaserReticle = bundle.LoadAsset("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;
            bundle = null;



            var gunModulableLib = Module.ModularAssetBundle.LoadAsset<GameObject>("GunBuildAndWhatNotAnimation").GetComponent<tk2dSpriteAnimation>();
            GameObject Tether_VFX = new GameObject("Electric Build Tether");
            FakePrefab.DontDestroyOnLoad(Tether_VFX);
            FakePrefab.MarkAsFakePrefab(Tether_VFX);
            Tether_VFX.SetActive(false);
            var tk2d = Tether_VFX.AddComponent<tk2dTiledSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("chain_idle_001"));
            var tk2dAnim = Tether_VFX.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = gunModulableLib;
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("chain_start");
            tk2dAnim.playAutomatically = true;
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 30);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 30);
            tk2d.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            //tk2d.IsPerpendicular = false;
            //tk2d.ShouldDoTilt = false;
            VFX_Tether_Modulable = Tether_VFX;

            GameObject VFX = new GameObject("VFX_MODULABLE");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            VFX.SetActive(false);
            var tk2d2 = VFX.AddComponent<tk2dSprite>();
            tk2d2.Collection = StaticCollections.VFX_Collection;
            tk2d2.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("rotating_gear_idle_001"));
            var tk2dAnim_2 = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim_2.Library = gunModulableLib;
            tk2dAnim_2.defaultClipId = tk2dAnim.Library.GetClipIdByName("start");
            tk2dAnim_2.playAutomatically = true;
            tk2d2.usesOverrideMaterial = true;
            tk2d2.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d2.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d2.renderer.material.SetFloat("_EmissivePower", 30);
            tk2d2.renderer.material.SetFloat("_EmissiveColorPower", 30);
            tk2d2.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            VFX_Modulable = VFX;


            var SynergyLib = Module.ModularAssetBundle.LoadAsset<GameObject>("SynergyAnimation").GetComponent<tk2dSpriteAnimation>();
            GameObject VFX_Synergy = new GameObject("VFX_SYNERGY");
            FakePrefab.DontDestroyOnLoad(VFX_Synergy);
            FakePrefab.MarkAsFakePrefab(VFX_Synergy);
            VFX_Synergy.SetActive(false);
            var tk2d3 = VFX_Synergy.AddComponent<tk2dSprite>();
            tk2d3.Collection = StaticCollections.VFX_Collection;
            tk2d3.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("synergy_arrow_idle_001"));
            var tk2dAnim_3 = VFX_Synergy.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim_3.Library = SynergyLib;
            tk2dAnim_3.defaultClipId = tk2dAnim.Library.GetClipIdByName("start");
            tk2dAnim_3.playAutomatically = true;
            tk2d3.usesOverrideMaterial = true;
            tk2d3.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d3.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d3.renderer.material.SetFloat("_EmissivePower", 30);
            tk2d3.renderer.material.SetFloat("_EmissiveColorPower", 30);
            tk2d3.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            VFX__Synergy = VFX_Synergy;

            GameObject VFX_Popup = new GameObject("VFX_Popup");
            FakePrefab.MarkAsFakePrefab(VFX_Popup);
            FakePrefab.DontDestroyOnLoad(VFX_Popup);
            var tk2d3_1 = VFX_Popup.AddComponent<tk2dSprite>();
            tk2d3_1.Collection = StaticCollections.Module_T1_Collection;

            AdditionalBraveLight braveLight = VFX_Popup.gameObject.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = tk2d3_1.sprite.WorldCenter;
            braveLight.LightColor = Color.white;
            braveLight.LightIntensity = 0f;
            braveLight.LightRadius = 0f;

            tk2d3_1.usesOverrideMaterial = true;
            tk2d3_1.renderer.material.shader = Shader.Find("Brave/Internal/SimpleAlphaFadeUnlit");
            tk2d3_1.renderer.material.SetFloat("_Fade", 0f);

            VFX_SpriteAppear = VFX_Popup;


            var scarf = PickupObjectDatabase.GetById(436) as BlinkPassiveItem;
            ScarfObject = scarf.ScarfPrefab;
        }
        public static GameObject VFX_SpriteAppear;

        public static GameObject VFX__Synergy;


        public static ScarfAttachmentDoer ScarfObject;


        public static GameObject VFX_Modulable;
        public static GameObject VFX_Tether_Modulable;

        public static GameObject LaserReticle;

        public static GameObject RadialRing;
        public static GameObject TeleportDistortVFX;
        public static GameObject TeleportVFX;
        public static GameObject RelodestoneContinuousSuckVFX;
        public static GameObject DragunBoulderLandVFX;
        public static GameObject HealingSparklesVFX;
        public static GameObject FriendlyElectricLinkVFX;
        public static GameObject MachoBraceDustupVFX;
        public static GameObject MachoBraceBurstVFX;
        public static GameObject WarningImpactVFX;



        public static void DoFancyFlashOfModules(int Amount, PlayerController player, DefaultModule Module_To_Display)
        {
            for (int r = 0; r < Amount; r++)
            {
                GameManager.Instance.StartCoroutine(DoFlashyVFX(player, Module_To_Display));
            }
        }

        public static void DoFancyDestroyOfModules(int Amount, PlayerController player, DefaultModule Module_To_Display)
        {
            for (int r = 0; r < Amount; r++)
            {
                GameManager.Instance.StartCoroutine(DoFlashyVFX_Destroy(player, Module_To_Display));
            }
        }

        private static IEnumerator DoFlashyVFX(PlayerController player, DefaultModule properties)
        {
            Vector2 playerPos = player.sprite.WorldCenter;

            var VFX_Object = UnityEngine.Object.Instantiate(VFX_SpriteAppear, playerPos, Quaternion.identity).GetComponent<tk2dBaseSprite>();

            VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(properties).sprite.collection, GlobalModuleStorage.ReturnModule(properties).sprite.spriteId);

            var light = VFX_Object.GetComponent<AdditionalBraveLight>();
            light.LightColor = GlobalModuleStorage.ReturnModule(properties).BraveLight.LightColor;

            Vector2 offset = Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2f, 5.0f));
            float e = 0;
            while (e < 1)
            {
                float t = Toolbox.SinLerpTValue(e);

                VFX_Object.transform.position = Vector2.Lerp(playerPos, playerPos + offset, t);
                VFX_Object.renderer.material.SetFloat("_Fade", t);
                light.LightIntensity = Mathf.Lerp(0, 2.5f, t);
                light.LightRadius = Mathf.Lerp(0, 2, t);
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            while (e < 1)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            e = 0;
            Vector2 p = VFX_Object.transform.PositionVector2();
            float d = UnityEngine.Random.Range(0.7f, 1.5f);
            while (e < d)
            {
                float t = Toolbox.SinLerpTValue(e / d);
                VFX_Object.transform.position = Vector2.Lerp(p, player.sprite.WorldCenter, t);
                light.LightIntensity = Mathf.Lerp(2.5f, 1, t);
                light.LightRadius = Mathf.Lerp(0, 2, t);
                VFX_Object.renderer.material.SetFloat("_Fade", 1 - t);

                e += BraveTime.DeltaTime;
                yield return null;
            }
            LootEngine.DoDefaultSynergyPoof(player.sprite.WorldCenter);
            UnityEngine.Object.Destroy(VFX_Object.gameObject);
            yield break;
        }

        private static IEnumerator DoFlashyVFX_Destroy(PlayerController player, DefaultModule properties)
        {
            Vector2 playerPos = player.sprite.WorldCenter;

            var VFX_Object = UnityEngine.Object.Instantiate(VFX_SpriteAppear, playerPos, Quaternion.identity).GetComponent<tk2dBaseSprite>();
            VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(properties).sprite.collection, GlobalModuleStorage.ReturnModule(properties).sprite.spriteId);
            var light = VFX_Object.GetComponent<AdditionalBraveLight>();
            light.LightColor = properties.BraveLight.LightColor;

            Vector2 offset = Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(2f, 5.0f));
            float e = 0;
            while (e < 2)
            {
                float t = Toolbox.SinLerpTValueFull(e / 2);
                float t1 = Toolbox.SinLerpTValue(e / 2);

                VFX_Object.transform.position = Vector2.Lerp(playerPos, playerPos + offset, t1);
                VFX_Object.renderer.material.SetFloat("_Fade", t);
                light.LightIntensity = Mathf.Lerp(0, 2.5f, t);
                light.LightRadius = Mathf.Lerp(0, 2, t);
                e += BraveTime.DeltaTime;
                yield return null;
            }
            UnityEngine.Object.Destroy(VFX_Object.gameObject);
            yield break;
        }

    }
}
