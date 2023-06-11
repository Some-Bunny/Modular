using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.FakeCorridor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class BigVaultObject
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("BigVaultDoor_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("vault"));
            tk2d.sprite.usesOverrideMaterial = true;

            tk2d.usesOverrideMaterial = true;
            tk2d.hasOffScreenCachedUpdate = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);
            tk2d.renderer.material = mat;


            obj.CreateFastBody(new IntVector2(80, 96), new IntVector2(16, 16));
            obj.CreateFastBody(new IntVector2(80, 96), new IntVector2(160, 16));
            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();

            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("BigVault_MDLR", obj);
        }


    }//BG_Nonsense
}
