using Alexandria.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class ChooseModuleController : MonoBehaviour
    {
        public static Func<int, int> AdditionalOptionsModifier;

        public int Count = 4;
        public Gun g;
        public bool isAlt = false;
        public Dictionary<tk2dTiledSprite, ModuleUICarrier> tk2DTiledSprites = new Dictionary<tk2dTiledSprite, ModuleUICarrier>();

        public Vector2 CalculateAdditionalOffset(float angle)
        {
            return Toolbox.GetUnitOnCircle(angle - 90, 0.5f);
        }

        public void Nudge(PlayerController p)
        {
            if (isNudgeable == false) { return; }
            AkSoundEngine.PostEvent("Play_ENM_rubber_bounce_01", g.gameObject);
            GameManager.Instance.StartCoroutine(NudgeToPlayer(p));
        }

        public IEnumerator NudgeToPlayer(PlayerController p)
        {
            isNudgeable = false;
            Vector2 modPosition = g.transform.PositionVector2();
            float elapsed = 0f;
            while (elapsed < 0.7f)
            {
                if (g == null) { yield break; }
                elapsed += BraveTime.DeltaTime;
                float t = elapsed / 1;
                if (elapsed > 0.35f) { elapsed = 1; }
                g.gameObject.transform.position = Vector3.Lerp(modPosition, p.SpriteBottomCenter, Toolbox.SinLerpTValueFull(t / 2));
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < 0.65f)
            {
                if (g == null) { yield break; }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            isNudgeable = true;
            yield break;
        }
        private bool isNudgeable = true;

        public DefaultModule SelectModule(GenericLootTable table)
        {

            var mod = table.SelectByWeightNoExclusions().GetComponent<DefaultModule>();
            switch (mod.Tier)
            {
                case DefaultModule.ModuleTier.Tier_1:
                    foreach (PlayerController p in GameManager.Instance.AllPlayers)
                    {
                        if (p.PlayerHasCore() != null & p.HasPickupID(815))
                        {
                            return GlobalModuleStorage.SelectTable(PickupObject.ItemQuality.B).SelectByWeightNoExclusions().GetComponent<DefaultModule>();
                        }
                    }
                    if (UnityEngine.Random.value < 0.00005f) { AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                    return mod;
                case DefaultModule.ModuleTier.Tier_2:
                    if (UnityEngine.Random.value < 0.0001f) { AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega); }
                    return mod;
                case DefaultModule.ModuleTier.Tier_3:
                    if (UnityEngine.Random.value < 0.000175f)
                    {
                        AkSoundEngine.PostEvent("Play_BOSS_queenship_emerge_01", g.gameObject); return GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_Omega);
                    }
                    return mod;
                default: return mod;
            }
        }
        public void AlterCount()
        {
            if (g.quality == PickupObject.ItemQuality.B | g.quality == PickupObject.ItemQuality.A)
            {
                Count++;
            }
            if (g.quality == PickupObject.ItemQuality.S)
            {
                Count += 2;
            }
        }


        public void Start()
        {

            g = this.GetComponent<Gun>();
            var obj = g.gameObject.GetComponent<ShittyVFXAttacher>();
            if (obj)
            {
                Destroy(obj);
            }
            AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_start_01", g.gameObject);

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
                    EndPosition = Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(Vector2.up.ToAngle() + (Arc * -1), Count, i), 2.5f), //, Count, i) , 2f),   
                    isUsingAlternate = isAlt
                });
            }
            foreach (var r in selectableModules)
            {
                r.Start();
                var Extant_Tether = UnityEngine.Object.Instantiate(VFXStorage.VFX_Tether_Modulable, g.sprite.WorldCenter, Quaternion.identity).GetComponent<tk2dTiledSprite>();
                Extant_Tether.dimensions = new Vector2(1, 16f);
                Extant_Tether.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Extant_Tether.GetComponent<tk2dSpriteAnimator>().Play(isAlt ? "chain_alt_start" : "chain_start");
                tk2DTiledSprites.Add(Extant_Tether, r);
            }


            g.StartCoroutine(LerpLight(g));
        }




        public void Update()
        {
            if (isBeingDestroyed == true) { return; }
            foreach (var entry in tk2DTiledSprites)
            {
                float angle = ((entry.Value.extantModule.transform.PositionVector2() - g.sprite.WorldCenter) + new Vector2(0.5f, 0.5f)).ToAngle();
                var vec = CalculateAdditionalOffset(angle);
                entry.Key.dimensions = new Vector2(Vector2.Distance(g.sprite.WorldCenter + CalculateAdditionalOffset(angle) + vec, entry.Value.extantModule.GetComponent<tk2dBaseSprite>().WorldCenter + vec) * 16, 16f);
                entry.Key.gameObject.transform.localRotation = Quaternion.Euler(0, 0, angle);
                entry.Key.gameObject.transform.position = g.sprite.WorldCenter + vec;
            }
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
            foreach (var entry in tk2DTiledSprites)
            {
                entry.Key.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(isAlt ? "chain_alt_break" : "chain_break");
            }

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
            private Vector2 Offset = new Vector2(-0.5f, -0.5f);
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
                    AkSoundEngine.PostEvent("Play_OBJ_metroid_roll_01", DefMod.gameObject);
                    DefMod.EnteredRange -= Entered;
                    DefMod.ExitedRange -= Exited;
                    DefMod.ChangeShader(StaticShaders.Default_Shader);
                    HasDropped = true;
                    controller.DestroyAllOthers();
                    DefMod.StartCoroutine(LerpLight(DefMod, 0, 7, 0, 3));
                    Destroy(this);
                    DefMod.StartCoroutine(this.DoMovementToPlayer(DefMod, p));
                    return false;
                }
                return (HasStoppedMoving);
            }

            private IEnumerator DoMovementToPlayer(DefaultModule self, PlayerController p)
            {
                HasStoppedMoving = false;
                self.PreInteractLogic -= PreInteract;
                Vector2 modPosition = self.transform.PositionVector2();
                float elapsed = 0f;
                while (elapsed < 0.75f)
                {
                    if (self == null) { yield break; }
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / 1;
                    self.gameObject.transform.position = Vector3.Lerp(modPosition, p.SpriteBottomCenter, Toolbox.SinLerpTValue(t));
                    yield return null;
                }
                DebrisObject orAddComponent = self.gameObject.GetOrAddComponent<DebrisObject>();
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.sprite.UpdateZDepth();
                orAddComponent.Trigger(Vector3.up.WithZ(2f), 1, 1f);
                self.OnEnteredRange(p);


                yield break;
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
                    self.gameObject.transform.position = Vector3.Lerp(controller.g.sprite.WorldCenter + Offset, (controller.g.sprite.WorldCenter + EndPosition) + Offset, Toolbox.SinLerpTValue(t));
                    yield return null;
                }
                self.EnteredRange += Entered;
                self.ExitedRange += Exited;
                HasStoppedMoving = true;
                while (this.controller)
                {
                    if (HasStoppedMoving == true && extantModule.gameObject && controller.g)
                    {
                        extantModule.gameObject.transform.position = Vector2.MoveTowards(extantModule.gameObject.transform.position, (controller.g.sprite.WorldCenter + EndPosition) + Offset, 0.04f);
                    }
                    yield return null;
                }
                yield break;
            }

            /*
            public void Update()
            {

                if (controller)
                {
                    Debug.Log("EX: " + (extantModule != null));
                    Debug.Log("EXOB: " + (extantModule.gameObject != null));
                    Debug.Log("CO: " + (controller != null));
                    Debug.Log("COG: " + (controller.g != null));
                    Debug.Log("COGSPR: " + (controller.g.sprite != null));

                    if (HasStoppedMoving == true && extantModule.gameObject && controller.g)
                    {
                        extantModule.gameObject.transform.position = Vector2.MoveTowards(extantModule.gameObject.transform.position, (controller.g.sprite.WorldCenter + EndPosition) + Offset, 0.075f);
                    }
                }

            }
            */
            public void Entered(DefaultModule DefMod)
            {
                AkSoundEngine.PostEvent("Play_UI_menu_select_01", DefMod.gameObject);
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
                //Extant_Tether.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(controller.isAlt ? "chain_alt_break" : "chain_break");
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
            public void NoOutline(DefaultModule DefMod) { }

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

}
