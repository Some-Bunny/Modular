using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.Past.Prefabs.Objects.BigFuckOffDoor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class Spaceship
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Spaceship_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("spaceship_closed"));
            
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(0, 206, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material = mat;

            var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("SpaceshipAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;
            obj.AddComponent<BigAssSpaceship>();

            obj.CreateFastBody(new IntVector2(202, 105), new IntVector2(11, 16));
            obj.CreateFastBody(new IntVector2(162, 63), new IntVector2(31, -121));
            obj.CreateFastBody(new IntVector2(144, 64), new IntVector2(40, 184));

            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("GiantSpaceShip_MDLR", obj);
        }

        public class BigAssSpaceship : MonoBehaviour
        {
            public void Start()
            {
                GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "DoTakeOff" }, OnRecieveMessage);
            }
            public void OnRecieveMessage(GameObject obj, string message)
            {
                this.GetComponent<tk2dSpriteAnimator>().Play("idle_closed");
            }
        }
    }
}
