using Alexandria.PrefabAPI;
using Brave.BulletScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class ShippingContainer
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("ShippingContainer");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("shippingcontainer"));

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 0);
            mat.SetFloat("_EmissivePower", 0);
            tk2d.renderer.material = mat;

            obj.CreateFastBody(new IntVector2(98, 177), new IntVector2(0, 12));
            obj.CreateFastBody(new IntVector2(36, 32), new IntVector2(0, 0));

            var controller = tk2d.gameObject.AddComponent<QuickInterractableController>();
            Module.Strings.Core.Set("#MDLR_Container_", "A shipping container marked to ship to Gunymede.\nI'll be returning one way or another.");
            controller.Interact_String = "#MDLR_Container_";
            controller.talkPoint = obj.transform;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("ShippingContainerMDLR", obj);
        }
    }
}
