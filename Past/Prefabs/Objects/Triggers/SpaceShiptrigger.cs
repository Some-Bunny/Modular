using Alexandria.PrefabAPI;
using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Past.Prefabs.Objects
{
    public class SpaceShiptrigger
    {
        public static bool AllowedToLeave =false;
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("Trigger_SpaceShip");
            obj.CreateFastBody(CollisionLayer.Pickup, new IntVector2(48, 64), new IntVector2(0, 0));
            obj.AddComponent<CustomtriggerSpaceShip>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Trigger_SpaceShip", obj);
        }

        public class CustomtriggerSpaceShip : BraveBehaviour
        {
            
            public bool HasTriggered { get; set; }


            public RoomHandler ParentRoom { get; set; }

            public void Start()
            {
                HasTriggered = true;
                GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "PastWin" }, OnRecieveMessage);
                var specBody = this.GetComponent<SpeculativeRigidbody>();
                if (specBody)
                {
                    specBody.OnPreRigidbodyCollision += (myBody, myCollider, otherbody, otherCollider) =>
                    {
                        if (HasTriggered == false)
                        {
                            var player = otherbody.gameObject.GetComponent<PlayerController>();
                            if (player)
                            {
                                GameManager.IsBossIntro = false;
                                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                                {
                                    if (GameManager.Instance.AllPlayers[j])
                                    {
                                        GameManager.Instance.AllPlayers[j].SetInputOverride("goodbye!");
                                    }
                                }
                                GlobalMessageRadio.BroadcastMessage("DoTakeOff");
                                //GameManager.Instance.StartCoroutine(EnterTheGungeon());
                                HasTriggered = true;
                                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(player);
                                if (otherPlayer)
                                {
                                    otherPlayer.ReuniteWithOtherPlayer(player, false);
                                }
                                Destroy(this.gameObject);
                            }
                        }
                    };
                }

            }

          

            public void OnRecieveMessage(GameObject obj, string message)
            {
                HasTriggered = false;
            }
            public override void OnDestroy()
            {
                base.OnDestroy();
            }
        }
    }
}
