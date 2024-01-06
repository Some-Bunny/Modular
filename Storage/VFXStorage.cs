using Alexandria.ItemAPI;
using Brave.BulletScript;
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
            TelefragVFX = (PickupObjectDatabase.GetById(449) as TeleporterPrototypeItem).TelefragVFXPrefab;

            //TeleportDistortVFX = (PickupObjectDatabase.GetById(573) as ChestTeleporterItem).;


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

            AIAnimator aiAnimator = EnemyDatabase.GetOrLoadByGuid("6c43fddfd401456c916089fdd1c99b1c").aiAnimator;
            List<AIAnimator.NamedVFXPool> namedVFX = aiAnimator.OtherVFX;
            foreach (AIAnimator.NamedVFXPool pool in namedVFX)
            {
                if (pool.name == "mergo")
                {
                    foreach (VFXComplex vFXComplex in pool.vfxPool.effects)
                    {
                        foreach (VFXObject vFXObject in vFXComplex.effects)
                        {
                            VFXObject myVFX = VFXStorage.CopyFields<VFXObject>(vFXObject);
                            HighPriestClapVFX = new VFXPool();
                            HighPriestClapVFX.type = VFXPoolType.Single;
                            HighPriestClapVFX.effects = new VFXComplex[] { new VFXComplex() { effects = new VFXObject[] { myVFX } } };

                            VFXObject myVFX2 = VFXStorage.CopyFields<VFXObject>(vFXObject);
                            GameObject yas = FakePrefab.Clone(myVFX2.effect);
                            myVFX2.effect = yas;

                           
                        }
                    }
                }
            }

            MourningStarLaser = FakePrefab.Clone((PickupObjectDatabase.GetById(515) as Gun).DefaultModule.projectiles[0].bleedEffect.vfxExplosion);
            var mourningStarComp = MourningStarLaser.AddComponent<MourningStarVFXController>();
            var hODC = MourningStarLaser.GetComponent<HammerOfDawnController>();
            mourningStarComp.BeamSections = hODC.BeamSections;
            mourningStarComp.BurstSprite = hODC.BurstSprite;
            mourningStarComp.SectionStartAnimation = hODC.SectionStartAnimation;
            mourningStarComp.SectionAnimation = hODC.SectionAnimation;
            mourningStarComp.SectionEndAnimation = hODC.SectionEndAnimation;
            mourningStarComp.CapAnimation = hODC.CapAnimation;
            mourningStarComp.CapEndAnimation = hODC.CapEndAnimation;
            mourningStarComp.InitialImpactVFX = hODC.InitialImpactVFX;
            UnityEngine.Object.Destroy(hODC);

            SpiratTeleportVFX = EnemyDatabase.GetOrLoadByGuid("56fb939a434140308b8f257f0f447829").bulletBank.GetBullet("rogue").BulletObject.GetComponent<TeleportProjModifier>().teleportVfx;
        }
        public static VFXPool SpiratTeleportVFX;

        public static VFXObject CopyFields<T>(VFXObject sample2) where T : VFXObject
        {
            VFXObject sample = new VFXObject();
            sample.alignment = sample2.alignment;
            sample.attached = sample2.attached;
            sample.destructible = sample2.destructible;
            sample.effect = sample2.effect;
            sample.orphaned = sample2.orphaned;
            sample.persistsOnDeath = sample2.persistsOnDeath;
            sample.usesZHeight = sample2.usesZHeight;
            return sample;
        }

        public static VFXPool HighPriestClapVFX;


        public static GameObject VFX_SpriteAppear;

        public static GameObject VFX__Synergy;


        public static ScarfAttachmentDoer ScarfObject;


        public static GameObject VFX_Modulable;
        public static GameObject VFX_Tether_Modulable;

        public static GameObject LaserReticle;

        public static GameObject RadialRing;
        public static GameObject TeleportDistortVFX;
        public static GameObject TeleportVFX;
        public static GameObject TelefragVFX;

        
        public static GameObject RelodestoneContinuousSuckVFX;
        public static GameObject DragunBoulderLandVFX;
        public static GameObject HealingSparklesVFX;
        public static GameObject FriendlyElectricLinkVFX;
        public static GameObject MachoBraceDustupVFX;
        public static GameObject MachoBraceBurstVFX;
        public static GameObject WarningImpactVFX;

        public static GameObject MourningStarLaser;



        public static void DoFancyFlashOfModules(int Amount, PlayerController player, DefaultModule Module_To_Display)
        {
            E_C.Add(new EffectContainer(player, Module_To_Display, Amount));
            if (isDoingFlashyVFX == true)
            {
                return;
            }
            GameManager.Instance.StartCoroutine(DoFlashyVFX());
        }



        public static void DoFancyDestroyOfModules(int Amount, PlayerController player, DefaultModule Module_To_Display)
        {
            DE_C.Add(new EffectContainer(player, Module_To_Display, Amount));
            if (isDoingFlashyVFX_Destroy == true)
            {
                return;
            }
            GameManager.Instance.StartCoroutine(DoFlashyVFX_Destroy());
        }
        private static List<EffectContainer> E_C = new List<EffectContainer>();
        private static List<EffectContainer> DE_C = new List<EffectContainer>();

        private class EffectContainer
        {
            public EffectContainer(PlayerController p, DefaultModule m, int a = 1)
            {
                player = p;
                defaultModule = m;
                Amount = a;
            }

            public PlayerController player;
            public DefaultModule defaultModule;
            public int Amount;
        }

        private static bool isDoingFlashyVFX = false;
        private static bool isDoingFlashyVFX_Destroy = false;

        private static IEnumerator DoFlashyVFX()
        {
            isDoingFlashyVFX = true;

            EffectContainer effectContainer = E_C.Last();


            PlayerController playerPos = effectContainer.player;

            Dictionary<tk2dBaseSprite, Vector2> sprites = new Dictionary<tk2dBaseSprite, Vector2>();



            for (int i = 0; i < effectContainer.Amount; i++)
            {
                var VFX_Object = UnityEngine.Object.Instantiate(VFX_SpriteAppear, playerPos.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dBaseSprite>();
                VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(effectContainer.defaultModule).sprite.collection, GlobalModuleStorage.ReturnModule(effectContainer.defaultModule).sprite.spriteId);
                var light = VFX_Object.GetComponent<AdditionalBraveLight>();
                light.LightColor = Color.white;//properties.BraveLight.LightColor;
                sprites.Add(VFX_Object, Toolbox.GetUnitOnCircle(Toolbox.SubdivideRange(-85, 85, effectContainer.Amount, i, true), 3f));
            }

            bool b = false;

            float e = 0;
            while (e < 2)
            {

                if (e > 0.5f && b == false)
                {
                    b = true;
                    E_C.Remove(effectContainer);
                    if (E_C.Count() > 0)
                    {
                        GameManager.Instance.StartCoroutine(DoFlashyVFX());
                    }
                }

                float t = Toolbox.SinLerpTValueFull(e / 2);
                float t1 = Toolbox.SinLerpTValue(e / 1.5f);
                foreach (var VFX_Object in sprites)
                {
                    VFX_Object.Key.transform.position = Vector2.Lerp(playerPos.sprite.WorldCenter - new Vector2(0.5f, 0.5f) + VFX_Object.Value, playerPos.sprite.WorldCenter - new Vector2(0.5f, 0.5f), t1);
                    VFX_Object.Key.renderer.material.SetFloat("_Fade", t);
                    var light = VFX_Object.Key.GetComponent<AdditionalBraveLight>();
                    light.LightIntensity = Mathf.Lerp(0, 2.5f, t);
                    light.LightRadius = Mathf.Lerp(0, 2, t);
                }
                e += BraveTime.DeltaTime;
                yield return null;
            }

            for (int i = sprites.Count() - 1; i > -1; i--)
            {
                UnityEngine.Object.Destroy(sprites.Last().Key.gameObject);
                LootEngine.DoDefaultSynergyPoof(playerPos.sprite.WorldCenter);
            }
            if (E_C.Count() > 0)
            {
                yield break;
            }
            isDoingFlashyVFX = false;
            yield break;
        }

        private static IEnumerator DoFlashyVFX_Destroy()
        {

            isDoingFlashyVFX_Destroy = true;
            EffectContainer effectContainer = DE_C.Last();


            PlayerController playerPos = effectContainer.player;

            Dictionary<tk2dBaseSprite, Vector2> sprites = new Dictionary<tk2dBaseSprite, Vector2>();
            
            

            for (int i = 0; i < effectContainer.Amount; i++)
            {
                var VFX_Object = UnityEngine.Object.Instantiate(VFX_SpriteAppear, playerPos.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dBaseSprite>();
                VFX_Object.SetSprite(GlobalModuleStorage.ReturnModule(effectContainer.defaultModule).sprite.collection, GlobalModuleStorage.ReturnModule(effectContainer.defaultModule).sprite.spriteId);
                var light = VFX_Object.GetComponent<AdditionalBraveLight>();
                light.LightColor = Color.white;//properties.BraveLight.LightColor;
                sprites.Add(VFX_Object, Toolbox.GetUnitOnCircle(Toolbox.SubdivideRange(-85, 85, effectContainer.Amount, i, true) + 180, 3f));
            }

            bool b = false;

            float e = 0;
            while (e < 2)
            {
                
                if (e > 0.5f && b == false)
                {
                    b = true;
                    DE_C.Remove(effectContainer);
                    if (DE_C.Count() > 0)
                    {
                        GameManager.Instance.StartCoroutine(DoFlashyVFX_Destroy());
                    }
                }
                
                float t = Toolbox.SinLerpTValueFull(e / 2);
                float t1 = Toolbox.SinLerpTValue(e / 1.5f);
                foreach (var VFX_Object in sprites)
                {
                    VFX_Object.Key.transform.position = Vector2.Lerp(playerPos.sprite.WorldCenter - new Vector2(0.5f, 0.5f), playerPos.sprite.WorldCenter - new Vector2(0.5f, 0.5f) + VFX_Object.Value, t1);
                    VFX_Object.Key.renderer.material.SetFloat("_Fade", t);
                    var light = VFX_Object.Key.GetComponent<AdditionalBraveLight>();
                    light.LightIntensity = Mathf.Lerp(0, 2.5f, t);
                    light.LightRadius = Mathf.Lerp(0, 2, t);
                }
                e += BraveTime.DeltaTime;
                yield return null;
            }

            for (int i = sprites.Count() - 1; i > -1; i--)
            {
                UnityEngine.Object.Destroy(sprites.Last().Key.gameObject);
            }
            if (DE_C.Count() > 0)
            {
                yield break;
            }
                isDoingFlashyVFX_Destroy = false;
            yield break;
        }


        public class MourningStarVFXController : BraveBehaviour
        {
            public static MourningStarVFXController SpawnMourningStar(Vector2 position, float lifeTime = -1, Transform parent = null)
            {
                var h = UnityEngine.Object.Instantiate<GameObject>(VFXStorage.MourningStarLaser, position, Quaternion.identity, parent).GetComponent<VFXStorage.MourningStarVFXController>();
                if (lifeTime != -1 && lifeTime > 0) { h.Invoke("Dissipate", lifeTime); }
                return h;
            }


            private void Start()
            {
                isbeingTossed = false;
                TimeExtant = 0;
                for (int i = 0; i < this.BeamSections.Count; i++)
                {
                    tk2dSpriteAnimator spriteAnimator = this.BeamSections[i].spriteAnimator;
                    if (spriteAnimator)
                    {
                        spriteAnimator.alwaysUpdateOffscreen = true;
                        spriteAnimator.PlayForDuration(this.SectionStartAnimation, -1f, this.SectionAnimation, false);
                        if (DoesSound == true)
                        {
                            AkSoundEngine.PostEvent("Play_WPN_dawnhammer_loop_01", base.gameObject);
                            AkSoundEngine.PostEvent("Play_State_Volume_Lower_01", base.gameObject);
                        }
                    }
                }
                if (OnBeamStart != null) { OnBeamStart(this.gameObject); }
                base.spriteAnimator.alwaysUpdateOffscreen = true;
                this.BurstSprite.UpdateZDepth();
                base.sprite.renderer.enabled = false;
            }

            public void Update()
            {
                if (isbeingTossed == true) { return; }
                TimeExtant += BraveTime.DeltaTime;
                base.sprite.UpdateZDepth();
                for (int i = 0; i < this.BeamSections.Count; i++)
                {
                    this.BeamSections[i].UpdateZDepth();
                }
                this.BurstSprite.UpdateZDepth();
                if (!this.BurstSprite.renderer.enabled)
                {
                    base.sprite.renderer.enabled = true;
                    base.spriteAnimator.Play(this.CapAnimation);
                }
                if (DoesEmbers == true)
                {
                    if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH)
                    {
                        int num4 = (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH) ? 50 : 125;
                        this.m_particleCounter += BraveTime.DeltaTime * (float)num4;
                        if (this.m_particleCounter > 1f)
                        {
                            GlobalSparksDoer.DoRadialParticleBurst(Mathf.FloorToInt(this.m_particleCounter), base.sprite.WorldBottomLeft, base.sprite.WorldTopRight, 30f, 2f, 1f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                            this.m_particleCounter %= 1f;
                        }
                    }
                }


                if (OnBeamUpdate != null) { OnBeamUpdate(this.gameObject, TimeExtant); }

            }

            public void Dissipate()
            {
                isbeingTossed = true;
                if (OnBeamDie != null) { OnBeamDie(this.gameObject); }
                base.sprite.renderer.enabled = true;
                ParticleSystem componentInChildren = base.GetComponentInChildren<ParticleSystem>();
                if (componentInChildren)
                {
                    BraveUtility.EnableEmission(componentInChildren, false);
                }
                for (int i = 0; i < this.BeamSections.Count; i++)
                {
                    this.BeamSections[i].spriteAnimator.Play(this.SectionEndAnimation);
                }
                base.spriteAnimator.PlayAndDestroyObject(this.CapEndAnimation, null);
                UnityEngine.Object.Destroy(base.gameObject, 1f);
                if (DoesSound == true)
                {
                    AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_State_Volume_Lower_01", base.gameObject);
                }
            }

            public override void OnDestroy()
            {
                if (DoesSound == true)
                {
                    AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", base.gameObject);
                    AkSoundEngine.PostEvent("Stop_State_Volume_Lower_01", base.gameObject);
                }
                base.OnDestroy();
            }

            public Action<GameObject> OnBeamStart;
            public Action<GameObject, float> OnBeamUpdate;
            public Action<GameObject> OnBeamDie;

            private float TimeExtant;

            public float TimeAlive() { return TimeExtant; }

            public bool DoesSound = true;
            public bool DoesEmbers = true;

            private bool isbeingTossed;

            public List<tk2dSprite> BeamSections;
            public tk2dSprite BurstSprite;
            public GameObject InitialImpactVFX;

            public string SectionStartAnimation;
            public string SectionAnimation;
            public string SectionEndAnimation;
            public string CapAnimation;
            public string CapEndAnimation;

            private float m_particleCounter;
        }


    }
}
