using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria;
using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Planetside;
using UnityEngine;

namespace ModularMod
{
    public static class Toolbox
    {
        public static void ProcessAnimations(this CustomCharacterData self, Dictionary<string, int> dict)
        {
            var lib = self.animator.Library;
            foreach (var entry in dict)
            { lib.GetClipByName(entry.Key).fps = entry.Value;}
        }
        public static void AddGlowShaderToGun(this Gun self, Color32 glowColor, int glowstrength, int colorGlowStrength = 0)
        {
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", glowColor);
            mat.SetFloat("_EmissiveColorPower", colorGlowStrength);
            mat.SetFloat("_EmissivePower", glowstrength);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
            MeshRenderer component = self.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;
        }

        public static VFXPool MakeObjectIntoVFX(GameObject obj)
        {
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.All;
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            vfObj.effect = obj;
            complex.effects = new VFXObject[] { vfObj };
            pool.effects = new VFXComplex[] { complex };
            return pool;
        }

        public static GameObject GenerateTransformPoint(GameObject attacher, Vector2 attachpoint, string name = "shootPoint")
        {
            GameObject shootpoint = new GameObject(name);
            shootpoint.transform.parent = attacher.transform;
            shootpoint.transform.position = attachpoint;
            return attacher.transform.Find(name).gameObject;
        }


        public static float SinLerpTValue(float t)
        {
            return Mathf.Sin(t * (Mathf.PI / 2));
        }

        public static float SinLerpTValueFull(float t)
        {
            return Mathf.Sin(t * (Mathf.PI));
        }

        public static float SubdivideArc(float startAngle, float sweepAngle, int numBullets, int i, bool offset = false)
        {
            return startAngle + Mathf.Lerp(0f, sweepAngle, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)(numBullets - 1));
        }

        public static float SubdivideCircle(float startAngle, int numBullets, int i, float direction = 1f, bool offset = false)
        {
            return startAngle + direction * Mathf.Lerp(0f, 360f, ((float)i + ((!offset) ? 0f : 0.5f)) / (float)numBullets);
        }
        public static Vector2 GetUnitOnCircle(float angleDegrees, float radius)
        {

            // initialize calculation variables
            float _x = 0;
            float _y = 0;
            float angleRadians = 0;
            Vector2 _returnVector;

            // convert degrees to radians
            angleRadians = angleDegrees * Mathf.PI / 180.0f;

            // get the 2D dimensional coordinates
            _x = radius * Mathf.Cos(angleRadians);
            _y = radius * Mathf.Sin(angleRadians);

            // derive the 2D vector
            _returnVector = new Vector2(_x, _y);

            // return the vector info
            return _returnVector;
        }

        public static ModifiedDefaultLabelManager GenerateText(Transform trans, Vector2 offset, float time, string Text, Color32 color, bool Autotrigger = true)
        {
            var labelToSet = UnityEngine.Object.Instantiate(DefaultModule.LabelController).gameObject.GetComponent<ModifiedDefaultLabelManager>();
            labelToSet.label.Text = Text;
            if (Autotrigger == true)
            {
                labelToSet.Trigger_CustomTime(trans, offset, time);
            }
            labelToSet.label.backgroundColor = color;

            GameUIRoot.Instance.m_manager.AddControl(labelToSet.panel);
            dfLabel componentInChildren = labelToSet.gameObject.GetComponentInChildren<dfLabel>();
            componentInChildren.ColorizeSymbols = false;
            componentInChildren.ProcessMarkup = true;
            return labelToSet;
        }

        public static void AddColorLight(this DefaultModule self, Color color)
        {
            AdditionalBraveLight braveLight = self.gameObject.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = self.sprite.WorldCenter;
            braveLight.LightColor = color;
            braveLight.LightIntensity = 0f;
            braveLight.LightRadius = 0f;
            self.BraveLight = braveLight;
            
        }

        public static void NotifyCustom(string header, string text, int spriteID, tk2dSpriteCollectionData CollectionData, UINotificationController.NotificationColor color = UINotificationController.NotificationColor.SILVER, bool forceSingleLine = false)
        {
            GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, CollectionData, spriteID, color, false, forceSingleLine);
        }

        public static Vector3 RandomPositionOnSprite(tk2dBaseSprite targetSprite, float zOffset = 0f)
        {
            if (targetSprite == null) { return Vector3.one; }
            Vector3 vector = targetSprite.WorldBottomLeft.ToVector3ZisY(zOffset);
            Vector3 vector2 = targetSprite.WorldTopRight.ToVector3ZisY(zOffset);
            float num = (vector2.y - vector.y) * (vector2.x - vector.x);
            float num2 = 25f * num;
            int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
            int num4 = num3;
            Vector3 minPosition = vector;
            Vector3 maxPosition = vector2;
            return new Vector3(UnityEngine.Random.Range(minPosition.x, maxPosition.x), UnityEngine.Random.Range(minPosition.y, maxPosition.y));
        }


        public static T LoadAssetFromAnywhere<T>(string path) where T : UnityEngine.Object
        {
            T t = default(T);
            foreach (string path2 in Toolbox.BundlePrereqs)
            {
                try
                {
                    t = ResourceManager.LoadAssetBundle(path2).LoadAsset<T>(path);
                }
                catch
                {
                }
                bool flag2 = t != null;
                if (flag2)
                {
                    break;
                }
            }
            return t;
        }
        private static string[] BundlePrereqs = new string[] {"brave_resources_001",
                "dungeon_scene_001",
                "encounters_base_001",
                "enemies_base_001",
                "flows_base_001",
                "foyer_001",
                "foyer_002",
                "foyer_003",
                "shared_auto_001",
                "shared_auto_002",
                "shared_base_001",
                "dungeons/base_bullethell",
                "dungeons/base_castle",
                "dungeons/base_catacombs",
                "dungeons/base_cathedral",
                "dungeons/base_forge",
                "dungeons/base_foyer",
                "dungeons/base_gungeon",
                "dungeons/base_mines",
                "dungeons/base_nakatomi",
                "dungeons/base_resourcefulrat",
                "dungeons/base_sewer",
                "dungeons/base_tutorial",
                "dungeons/finalscenario_bullet",
                "dungeons/finalscenario_convict",
                "dungeons/finalscenario_coop",
                "dungeons/finalscenario_guide",
                "dungeons/finalscenario_pilot",
                "dungeons/finalscenario_robot",
                "dungeons/finalscenario_soldier"};
    }
}
