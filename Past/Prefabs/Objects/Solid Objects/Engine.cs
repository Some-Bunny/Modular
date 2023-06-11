using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.Past.Prefabs.Objects.BigFuckOffDoor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class Engine
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Engine_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("engine_001"));
            
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat.SetFloat("_EmissiveColorPower", 8);
            mat.SetFloat("_EmissivePower", 3);
            tk2d.renderer.material = mat;

            var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("EngineAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;
            obj.AddComponent<EngineBehavior>();

            obj.CreateFastBody(new IntVector2(24, 28), new IntVector2(1, -4));
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Engine_Decor_MDLR", obj);
        }

        public class EngineBehavior : MonoBehaviour
        {
            public void Start()
            {
                GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "ShutDown" }, OnRecieveMessage);
            }
            public void OnRecieveMessage(GameObject obj, string message)
            {
                this.GetComponent<tk2dSpriteAnimator>().Play("bangbang");
            }
        }
    }
}
