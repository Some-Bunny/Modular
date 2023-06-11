using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class ForbodingSign
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Forboding_Sign_Sticker");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;

            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("p-2"));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(StaticShaders.Default_Shader);
            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("ForbodingSignMDLR", obj);
        }
    }//BG_Nonsense
}
