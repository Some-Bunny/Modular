using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class MetalFences
    {
        public static void Init()
        {
            GenerateFence("Fence_Horizontal_MDLR", "metal_fence_002", new IntVector2(16, 4), new IntVector2(0, 4));
            GenerateFence("Fence_Vertical_MDLR", "metal_fence_001", new IntVector2(4, 16), new IntVector2(6, 0));

            GenerateFence("Fence_Corner_T_L_MDLR", "metal_fence_003", new IntVector2(10, 12), new IntVector2(0, 4));
            GenerateFence("Fence_Corner_T_R_MDLR", "metal_fence_004", new IntVector2(10, 12), new IntVector2(6, 4));

            GenerateFence("Fence_Corner_B_R_MDLR", "metal_fence_005", new IntVector2(10, 10), new IntVector2(6, 0));
            GenerateFence("Fence_Corner_B_L_MDLR", "metal_fence_006", new IntVector2(10, 10), new IntVector2(0, 0));

            GenerateFence("Fence_End1_MDLR", "fence_end_RH", new IntVector2(6, 8), new IntVector2(0, 2));
            GenerateFence("Fence_End2_MDLR", "fence_end_LH", new IntVector2(6, 8), new IntVector2(10, 2));

            GenerateFence("Fence_End3_MDLR", "fence_end_VB", new IntVector2(4, 6), new IntVector2(6, 0));
            GenerateFence("Fence_End4_MDLR", "fence_end_VT", new IntVector2(4, 8), new IntVector2(6, 8));
        }


        private static void GenerateFence(string placeName ,string spriteName, IntVector2 colliderX_Y, IntVector2 offsetX_Y)
        {
            GameObject obj = PrefabBuilder.BuildObject(placeName + "_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(spriteName));
            tk2d.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat.SetFloat("_EmissiveColorPower", 8);
            mat.SetFloat("_EmissivePower", 3);
            tk2d.renderer.material = mat;
            
            obj.CreateFastBody(CollisionLayer.LowObstacle, colliderX_Y, offsetX_Y);
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add(placeName, obj);
        }

    }//BG_Nonsense
}
