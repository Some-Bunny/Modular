using System.Reflection;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Dungeonator;
using Gungeon;
using System.Collections;
using Brave.BulletScript;
using System;
using System.Collections.Generic;
using Planetside;
using static SpawnEnemyOnDeath;
using Alexandria.Misc;
using UnityEngine.UI;

namespace ModularMod
{
    public static class Hooks
    {
        public class  ChooseModuleController : MonoBehaviour
        {
            public static Func<int, int> AdditionalOptionsModifier;

            public int Count = 3;
            public Gun g;
            public bool isAlt = false;

            public DefaultModule SelectModule(GenericLootTable table)
            {
                var mod = table.SelectByWeightNoExclusions().GetComponent<DefaultModule>();
                switch (mod.Tier)
                {
                    case DefaultModule.ModuleTier.Tier_1:
                        if (UnityEngine.Random.value < 0.001) { return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                        return mod;
                    case DefaultModule.ModuleTier.Tier_2:
                        if (UnityEngine.Random.value < 0.0025) { return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                        return mod;
                    case DefaultModule.ModuleTier.Tier_3:
                        if (UnityEngine.Random.value < 0.0075) { return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                        return mod;
                    default: return mod;
                }
            }

            public void AlterCount()
            {
                if (g.quality == PickupObject.ItemQuality.B | g.quality == PickupObject.ItemQuality.A) { Count = 4; }
                if (g.quality == PickupObject.ItemQuality.S) { Count = 5; }
            }


            public void Start()
            {
                g = this.GetComponent<Gun>();
                var light = g.gameObject.GetOrAddComponent<AdditionalBraveLight>();
                light.LightColor = TierColor();
                AlterCount();
                selectableModules = new List<ModuleUICarrier>();
                GenericLootTable tableToUse = GlobalModuleStorage.SelectTable(g.quality);

                if (AdditionalOptionsModifier != null) { Count = AdditionalOptionsModifier(Count); }

                float Arc = 30 + (Count * 15);

                for (int i = 0; i < Count; i++)
                {
                    selectableModules.Add(new ModuleUICarrier()
                    {
                        controller = this,
                        defaultModule = SelectModule(tableToUse),
                        EndPosition = Toolbox.GetUnitOnCircle(Toolbox.SubdivideArc(Vector2.up.ToAngle() + (Arc * -1), Arc * 2, Count, i) , 2f),   
                        isUsingAlternate = isAlt
                    });
                }
                foreach(var r in selectableModules)
                {
                    r.Start();
                }
                g.StartCoroutine(LerpLight(g));
            }
            private IEnumerator LerpLight(Gun g)
            {
                bool emergtencyCheck = false;
                var light = g.gameObject.GetOrAddComponent<AdditionalBraveLight>();
                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    if (g == null) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / 1f;
                    if (emergtencyCheck == false)
                    {
                        light.LightIntensity = Mathf.Lerp(0, 10, t);
                        light.LightRadius = Mathf.Lerp(0, 1.5f, t);
                    }
                    yield return null;
                }
                yield break;
            }


            public void DestroyAllOthers()
            {
                for (int i = 0; i < selectableModules.Count; i++)
                {
                    if (selectableModules[i].extantModule)
                    {
                        if (selectableModules[i].HasDropped == false)
                        {
                            selectableModules[i].DoDestroy(selectableModules[i].extantModule.GetComponent<DefaultModule>());
                        }
                    }
                }
                g.StartCoroutine(I_DoDestroy(g));
            }

            public Color TierColor()
            {
                switch (g.quality)
                {
                    case PickupObject.ItemQuality.D:
                        return new Color(0.6f, 0.3f, 0);
                    case PickupObject.ItemQuality.C:
                        return Color.blue;
                    case PickupObject.ItemQuality.B:
                        return new Color(0.4f, 0.8f, 0.09f);
                    case PickupObject.ItemQuality.A:
                        return Color.red;
                    case PickupObject.ItemQuality.S:
                        return Color.white;
                    default: return Color.cyan;
                }     
            }

            public bool isBeingDestroyed = false;
            private IEnumerator I_DoDestroy(Gun g)
            {
                isBeingDestroyed = true;
                bool emergtencyCheck = false;
                var light = g.gameObject.GetOrAddComponent<AdditionalBraveLight>();
                light.LightColor = TierColor();
                g.sprite.renderer.material.shader = StaticShaders.Displacer_Beast_Shader;
                g.sprite.renderer.material.SetTexture("_MainTex", g.sprite.renderer.material.mainTexture);
                float elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    if (g == null) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / 0.5f;
                    g.sprite.renderer.material.SetFloat("_BurnAmount", t);
                    if (emergtencyCheck == false)
                    {
                        light.LightIntensity = Mathf.Lerp(10, 50, t);
                        light.LightRadius = Mathf.Lerp(1.5f, 0, t);
                    }
                    SpriteOutlineManager.RemoveOutlineFromSprite(g.sprite, false);
                    yield return null;
                }
                Destroy(g.gameObject);
                yield break;
            }


            private void OnDestroy()
            {
                for (int i = 0; i < selectableModules.Count; i++)
                {
                    if (selectableModules[i].extantModule)
                    {
                        if (selectableModules[i].HasDropped == false && selectableModules[i].BeingDestroyed == false)
                        {
                            selectableModules[i].DoDestroy(selectableModules[i].extantModule.GetComponent<DefaultModule>());
                        }
                    }
                }
            }


            public List<ModuleUICarrier> selectableModules = new List<ModuleUICarrier>();

            public class ModuleUICarrier : MonoBehaviour
            {
                public void Start()
                {
                    HasStoppedMoving = false;
                    extantModule = UnityEngine.Object.Instantiate<GameObject>(defaultModule.gameObject, controller.g.sprite.WorldCenter, Quaternion.identity);
                    var DefMod = extantModule.GetComponent<DefaultModule>();
                    DefMod.ChangeShader(StaticShaders.Hologram_Shader);
                    DefMod.sprite.renderer.material.SetFloat("_IsGreen", isUsingAlternate == true ? 1 : 0);

                    DefMod.PreInteractLogic += PreInteract;
                    DefMod.StartCoroutine(this.DoMovement(1f, DefMod));
                    DefMod.StartCoroutine(LerpLight(DefMod, 4, 0, 2, 0));
                }

                public bool HasDropped = false;
                public bool BeingDestroyed = false;


                public bool PreInteract(DefaultModule DefMod, PlayerController p)
                {
                    if (HasStoppedMoving == true && HasDropped == false)
                    {
                        DebrisObject orAddComponent = DefMod.gameObject.GetOrAddComponent<DebrisObject>();
                        orAddComponent.shouldUseSRBMotion = true;
                        orAddComponent.angularVelocity = 0f;
                        orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                        orAddComponent.sprite.UpdateZDepth();
                        orAddComponent.Trigger(Vector3.up.WithZ(2f), 1, 1f);
                        DefMod.OnEnteredRange(p);
                        DefMod.EnteredRange -= Entered;
                        DefMod.ExitedRange -= Exited;
                        DefMod.ChangeShader(StaticShaders.Default_Shader);
                        HasDropped = true;
                        controller.DestroyAllOthers();
                        DefMod.StartCoroutine(LerpLight(DefMod, 0, 7, 0, 3));
                        Destroy(this);
                        return false;
                    }
                    return (HasStoppedMoving);
                }
                private bool HasStoppedMoving;

                private IEnumerator LerpLight(DefaultModule self, float to, float From, float radTo, float radFrom)
                {
                    if (self.BraveLight == null) { yield break; }
                    float elapsed = 0f;
                    while (elapsed < 0.5f)
                    {
                        elapsed += BraveTime.DeltaTime;
                        float t = elapsed / 0.5f;
                        self.BraveLight.LightIntensity = Mathf.Lerp(From, to, t);
                        self.BraveLight.LightRadius = Mathf.Lerp(radFrom, radTo, t);
                        yield return null;
                    }
                    yield break;
                }


                private IEnumerator DoMovement(float duration, DefaultModule self)
                {
                    float elapsed = 0f;
                    while (elapsed < duration)
                    {
                        elapsed += BraveTime.DeltaTime;
                        float t = elapsed / duration;
                        self.gameObject.transform.position = Vector3.Lerp(controller.g.sprite.WorldCenter, controller.g.sprite.WorldCenter + EndPosition, Toolbox.SinLerpTValue(t));
                        yield return null;
                    }
                    self.EnteredRange += Entered;
                    self.ExitedRange += Exited;
                    HasStoppedMoving = true;
                    yield break;
                }

                public void Entered(DefaultModule DefMod)
                {
                    DefMod.StartCoroutine(LerpLight(DefMod, 7, 4, 3, 2));
                    DefMod.ChangeShader(StaticShaders.Default_Shader);
                }
                public void Exited(DefaultModule DefMod)
                {
                    DefMod.StartCoroutine(LerpLight(DefMod, 4, 7, 2, 3));
                    DefMod.ChangeShader(StaticShaders.Hologram_Shader);
                    DefMod.sprite.renderer.material.SetFloat("_IsGreen", isUsingAlternate == true ? 1 : 0);

                }

                public void DoDestroy(DefaultModule DefMod)
                {
                    DefMod.StartCoroutine(I_DoDestroy(DefMod));
                }

                private IEnumerator I_DoDestroy(DefaultModule DefMod)
                {
                    DefMod.OverrideCanDisplayText(false);
                    bool emergtencyCheck = false;
                    if (DefMod.BraveLight == null) { emergtencyCheck = true; }

                    BeingDestroyed = true;

                    DefMod.PreInteractLogic -= PreInteract;
                    DefMod.PreInteractLogic += PreInteractOverride;
                    DefMod.OverrideEnteredRangeOutline = NoOutline;
                    DefMod.OverrideExitedRangeOutline = NoOutline;
                    DefMod.EnteredRange -= Entered;
                    DefMod.ExitedRange -= Exited;


                    float i = emergtencyCheck == false ? DefMod.BraveLight.LightIntensity : 0;
                    DefMod.ChangeShader(StaticShaders.Displacer_Beast_Shader);
                    DefMod.sprite.renderer.material.SetTexture("_MainTex", DefMod.sprite.renderer.material.mainTexture);     
                    float elapsed = 0f;
                    while (elapsed < 0.66f)
                    {
                        elapsed += BraveTime.DeltaTime;
                        float t = elapsed / 0.66f;
                        DefMod.sprite.renderer.material.SetFloat("_BurnAmount", t);
                        if (emergtencyCheck == false)
                        {
                            DefMod.BraveLight.LightIntensity = Mathf.Lerp(i, 50, t);
                            DefMod.BraveLight.LightRadius = Mathf.Lerp(2, 0, t);
                        }
                        SpriteOutlineManager.RemoveOutlineFromSprite(DefMod.sprite, false);
                        yield return null;
                    }
                    Destroy(DefMod.gameObject);
                    yield break;
                }
                public bool PreInteractOverride(DefaultModule DefMod, PlayerController p)
                {
                    return false;
                }
                public void NoOutline(DefaultModule DefMod){ }

                public void OnDestroy()
                {
                    if (controller)
                    {
                        if (controller.selectableModules.Contains(this)) { controller.selectableModules.Remove(this); }
                    }
                }
                public Vector2 EndPosition;
                public ChooseModuleController controller;
                public GameObject extantModule;
                public DefaultModule defaultModule;
                public bool isUsingAlternate = false;

            }
        }
        public static void Init()
        {
            new Hook(typeof(Gun).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public), typeof(Hooks).GetMethod("PickupHook"));
            new Hook(typeof(PlayerController).GetMethod("SetStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("SetStencilValHook"));
            new Hook(typeof(PlayerController).GetMethod("UpdateStencilVal", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Hooks).GetMethod("UpdateStencilValHook"));

        }

        public static void PickupHook(Action<Gun, PlayerController> orig, Gun self, PlayerController player)
        {
            if (player.HasPickupID(ModulePrinterCore.ModulePrinterCoreID) == true)
            {
                var yes = self.gameObject.GetOrAddComponent<ChooseModuleController>();
                yes.isAlt = player.IsUsingAlternateCostume;
            }
            else
            {
                var c = self.gameObject.GetComponent<ChooseModuleController>();
                if (c != null) { if (c.isBeingDestroyed == true) { return; } }
                orig(self, player);
            }
        }
        public static bool Stencility_Enabled = true;
        public static void SetStencilValHook(Action<PlayerController, int> orig, PlayerController player, int i)
        {
            if (player.sprite.renderer.material.shader == StaticShaders.TransparencyShader) { return; }
            if (Stencility_Enabled == false) { return; }
            orig(player, i);
        }
        public static void UpdateStencilValHook(Action<PlayerController> orig, PlayerController player)
        {
            if (player.sprite.renderer.material.shader == StaticShaders.TransparencyShader) { return; }
            if (Stencility_Enabled == false) { return; }
            orig(player);
        }
    }
}