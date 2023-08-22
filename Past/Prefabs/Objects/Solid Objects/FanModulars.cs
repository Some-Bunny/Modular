using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class FanModulars
    {
        public static void Init()
        {
            //obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            CreateObject("NN_Modular", "NN_modular", "Modular Version 0.314.\nOriginally intended for research.", new IntVector2(17, 21), new IntVector2(0, -4));
            CreateObject("_X_Modular", "broken_modular", "Modular Version 0.7.\nA release candidate version, until it was\nunceremoniously dropped, and broken.\nMissing its AI drive, and has a cracked screen.", new IntVector2(16, 22), new IntVector2(0, -4));

            CreateObject("Lynceus_Modular", "ModularSpiderHelloBunnyYoureAwesome", "Modular Version 0.49.\nReconnaissance machine. Equipped for all terrain. I would be jealous, if I could be jealous.", new IntVector2(18, 17), new IntVector2(0, -4));
            CreateObject("Ski_Modular", "Modular_Deco_LightWeight_Base_Ver0.512", "Modular Version 0.512.\nVery lightweight, and scarily tall when upright.", new IntVector2(28, 18), new IntVector2(0, -4));

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
            Module.Strings.Core.Set("#MDLR_MACHINE_"+name, Dialogue);
            controller.Interact_String = "#MDLR_MACHINE_" + name;
            controller.talkPoint = obj.transform;
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add(name, obj);

            return obj;
        }
       
    }
}
