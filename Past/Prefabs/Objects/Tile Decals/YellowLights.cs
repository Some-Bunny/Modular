using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class YellowLights
    {
        public static void InitYellowLightHorizontal()
        {
            GameObject obj = PrefabBuilder.BuildObject("Yellow_Light");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("ground_light_002"));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 50);
            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            AdditionalBraveLight braveLight = obj.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = obj.transform.position;
            braveLight.LightColor = Color.yellow;//WarpGateExitMDLR_Past
            braveLight.LightIntensity = 2f;
            braveLight.LightRadius = 15f;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Yellow_Light_Horizintal", obj);
        }
        public static void InitYellowLightVertical()
        {
            GameObject obj = PrefabBuilder.BuildObject("Yellow_Light");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("ground_light_001"));
            tk2d.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 50);
            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            AdditionalBraveLight braveLight = obj.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = obj.transform.position;
            braveLight.LightColor = Color.yellow;//WarpGateExitMDLR_Past
            braveLight.LightIntensity = 2f;
            braveLight.LightRadius = 15f;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Yellow_Light_Vertical", obj);
        }
    }//BG_Nonsense
}
