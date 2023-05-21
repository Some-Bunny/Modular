using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class WarningSticker
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Warning_Sticker");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;

            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("warning_sign"));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1);
            mat.SetFloat("_EmissivePower", 1);
            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("WarningStickerModularPast", obj);
        }
    }//BG_Nonsense
}
