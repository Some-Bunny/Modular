using Alexandria.PrefabAPI;
using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.FakeCorridor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class BigFuckOffDoorAuto
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("BigFuckOffDoor_MDLR");
            var tk2d = obj.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Past_Decorative_Object_Collection;
            tk2d.SetSprite(StaticCollections.Past_Decorative_Object_Collection.GetSpriteIdByName("big_fucking_door_open_001"));

            tk2d.sprite.usesOverrideMaterial = true;

            tk2d.usesOverrideMaterial = true;
            tk2d.hasOffScreenCachedUpdate = true;
            Material mat = new Material(StaticShaders.FloorTileMaterial_Transparency);
            mat.SetTexture("_MainTex", tk2d.renderer.material.mainTexture);
            tk2d.renderer.material = mat;

            obj.AddComponent<Fuck_You_Youre_No_Longer_Perpendicular>();
            obj.AddComponent<BigDoorBehaviorAuto>();

            var tk2dAnim = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("BigDoorAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.Library.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;

            //obj.CreateFastBody(new IntVector2(32, 53), new IntVector2(0, 8));
            //obj.CreateFastBody(new IntVector2(32, 53), new IntVector2(32, 8));

            AddChild("Left_Door_Body" ,new Vector2(0, 0.5f), obj);
            AddChild("Right_Door_Body", new Vector2(2, 0.5f), obj);

            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("BigFuckOffDoor_Auto_MDLR", obj);
        }

        public static void AddChild(string name ,Vector2 Offset, GameObject parent)
        {
            GameObject obj = PrefabBuilder.BuildObject(name);
            obj.transform.parent = parent.transform;
            obj.transform.localPosition = Offset;
            obj.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
            obj.CreateFastBody(new IntVector2(32, 53), new IntVector2(0, 0));
        }

        public class BigDoorBehaviorAuto : MonoBehaviour
        {
            private RoomHandler thisRoom;
            private bool Trigger = false;

            private SpeculativeRigidbody leftDoor;
            private SpeculativeRigidbody rightDoor;


            public void Start()
            {
                Trigger = true;
                thisRoom = this.transform.position.GetAbsoluteRoom();
                leftDoor = this.transform.Find("Left_Door_Body").gameObject.GetComponent<SpeculativeRigidbody>();
                rightDoor = this.transform.Find("Right_Door_Body").gameObject.GetComponent<SpeculativeRigidbody>();

            }

            public void Update()
            {
                if (GameManager.Instance.BestActivePlayer.sprite.WorldCenter == null) { return; }
                if (Trigger == true && Vector2.Distance(GameManager.Instance.BestActivePlayer.sprite.WorldCenter, this.gameObject.transform.PositionVector2()) < 6)
                {
                    AkSoundEngine.PostEvent("Play_OBJ_moondoor_close_01", GameManager.Instance.BestActivePlayer.gameObject);
                    AkSoundEngine.PostEvent("Play_OBJ_moondoor_close_01", GameManager.Instance.BestActivePlayer.gameObject);

                    Trigger = !Trigger;
                    this.StartCoroutine(DoOpen());
                    this.GetComponent<tk2dSpriteAnimator>().Play("open");
                }
            }


            public void OnRecieveMessage(GameObject obj, string message){Trigger = true;}

            public IEnumerator DoOpen()
            {
                float e = 0;
                while (e < 1.8f) { e += BraveTime.DeltaTime; yield return null; }
                e = 0;
                Vector2 lP = leftDoor.transform.localPosition;
                Vector2 rP = rightDoor.transform.localPosition;


                while (e < 3.2f) 
                {
                    float t = e / 3.2f;
                    leftDoor.gameObject.transform.localPosition = Vector2.Lerp(lP, lP + new Vector2(-2, 0), t);
                    rightDoor.gameObject.transform.localPosition = Vector2.Lerp(rP, rP + new Vector2(2, 0), t);
                    leftDoor.Reinitialize();
                    rightDoor.Reinitialize();
                    e += BraveTime.DeltaTime;
                    yield return null; 
                }
                yield break;
            }
        }
    }
}
