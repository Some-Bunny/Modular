using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class RedLight
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Red_Light");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("red_light_001"));
            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 0, 0, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 50);
            tk2d.renderer.material = mat;
            obj.SetLayerRecursively(LayerMask.NameToLayer("BG_Nonsense"));
            AdditionalBraveLight braveLight = obj.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = obj.transform.position;
            braveLight.LightColor = Color.red;//WarpGateExitMDLR_Past
            braveLight.LightIntensity = 5f;
            braveLight.LightRadius = 0;
            obj.AddComponent<RedLightController>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Red_Light_Horizintal", obj);
        }
      
        public class RedLightController : MonoBehaviour
        {
            public void Start()
            {
                var roomHandler = this.transform.position.GetAbsoluteRoom();
                if (roomHandler != null)
                {
                    roomHandler.Entered += RoomHandler_Entered;

                    
                }
            }

            private void RoomHandler_Entered(PlayerController p)
            {
                this.GetComponent<AdditionalBraveLight>().LightRadius = 10;
                this.GetComponent<tk2dBaseSprite>().SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("red_light_002"));
            }
        }

    }//BG_Nonsense
}
