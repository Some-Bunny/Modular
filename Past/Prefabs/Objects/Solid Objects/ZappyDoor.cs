using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.Past.Prefabs.Objects.BigFuckOffDoor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class ZappyDoor
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("ZappyDoor_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("bigzappydoor_destroy_001"));
            
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 206, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 20);
            mat.SetFloat("_EmissivePower", 20);
            tk2d.renderer.material = mat;

            var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("ZappyDoorAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;
            obj.AddComponent<EngineBehavior>();

            obj.CreateFastBody(new IntVector2(24, 112), new IntVector2(-4, -8));
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("ZappyDoor_MDLR", obj);
        }

        public class EngineBehavior : MonoBehaviour
        {
            public void Start()
            {
                this.transform.position -= new Vector3(0, 1);
                GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "PastWin" }, OnRecieveMessage);
            }
            public void OnRecieveMessage(GameObject obj, string message)
            {
                this.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("destroy");
            }
        }
    }
}
