using Alexandria.PrefabAPI;
using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class MiscDecoratives
    {
        public static void Init()
        {
            //obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            CreateObject("MLDR_LoaderSuit", "loader_suit", "A Loader Suit for moving very heavy cargo, and missing its winch arm.\nWill not be used for its intended purpose.", new IntVector2(22, 17), new IntVector2(15, 0));

            CreateObject("MLDR_ShippingContainer_Closed", "shipping cratedone", "A locked shipping crate.\nNo other fate for those inside...", new IntVector2(98, 206), new IntVector2(0, -4));
            var obj = CreateObject("MLDR_Loader", "big_load", "A collosal loader, for transporting very heavy containers. Unfortunately, no way to ride it.", new IntVector2(0, 0), new IntVector2(0, 0));
            obj.CreateFastBody(new IntVector2(25, 27), new IntVector2(83, -4));
            obj.CreateFastBody(new IntVector2(25, 27), new IntVector2(83, 121));

            obj.CreateFastBody(new IntVector2(25, 27), new IntVector2(1, 38));
            obj.CreateFastBody(new IntVector2(25, 27), new IntVector2(166, 38));


            var talkPoint = PrefabBuilder.BuildObject("Talkpoint");
            talkPoint.transform.parent = obj.transform;
            talkPoint.transform.localPosition += new Vector3(0, 0);
            var tk2d = talkPoint.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("bigload_shadow"));
            tk2d.usesOverrideMaterial = true;
            tk2d.hasOffScreenCachedUpdate = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);
            talkPoint.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));

            CreateObject("MLDR_PowerCellsSmall", "power_cell_stack_1", "A rack of Modular Power Cells.", new IntVector2(33, 36), new IntVector2(1, -4));
            CreateObject("MLDR_PowerCellsStack", "power_cell_stack_2", "A large rack of Modular Power Cells. High Voltage!", new IntVector2(33, 54), new IntVector2(1, -4));

            Alexandria.DungeonAPI.StaticReferences.customPlaceables.Add("TrolleysMDLR",
            Toolbox.GenerateDungeonPlaceable(
            new Dictionary<GameObject, float>()
            {
              { CreateObject("Trolley_Empty", "trolleys1", "An empty trolley.", new IntVector2(18, 13), new IntVector2(0, -4)), 1 },
              { CreateObject("Trolley_Modules", "trolleys2", "A trolley, with stacks of modules tied haphazardly to it. All Unprogrammed, of no use.", new IntVector2(18, 13), new IntVector2(0, -4)), 0.8f },
              { CreateObject("Trolley_Drive", "trolleys3", "A trolley with a singular, unmarked drive on it. Likely due to a rush job.", new IntVector2(18, 13), new IntVector2(0, -4)), 0.5f },
              { CreateObject("Trolley_Scrap", "trolleys4", "A trolley with some bits of Scrap on it. Likely left like that during the evacuation.", new IntVector2(18, 13), new IntVector2(0, -4)), 0.7f },
              { CreateObject("Trolley_Crowbar", "trolleys5", "A trolley with a singular crowbar on it. 3rd edition, unusually enough.", new IntVector2(18, 13), new IntVector2(0, -4)), 0.2f },
            }));

        }


        public static GameObject CreateObject(string name, string spriteName, string Dialogue, IntVector2 size, IntVector2 offset)
        {
            GameObject obj = PrefabBuilder.BuildObject(name);
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName(spriteName));

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 0);
            mat.SetFloat("_EmissivePower", 0);
            tk2d.renderer.material = mat;

            obj.CreateFastBody(size, offset);
            var controller = tk2d.gameObject.AddComponent<QuickInterractableController>();
            Module.Strings.Core.Set("#MDLR_MISC_"+name, Dialogue);
            controller.Interact_String = "#MDLR_MISC_" + name;
            controller.talkPoint = obj.transform;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add(name, obj);

            return obj;
        }
       
    }
}
