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
            obj.CreateFastBody(CollisionLayer.Pickup, new IntVector2(32, 64), new IntVector2(0, 0));
            obj.AddComponent<CustomtriggerSpaceShip>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("Trigger_SpaceShip", obj);
        }

        public class CustomtriggerSpaceShip : BraveBehaviour
        {
            
            public bool HasTriggered { get; set; }


            public RoomHandler ParentRoom { get; set; }

            public void Start()
            {
                AllowedToLeave = false;
                HasTriggered = false;
                var specBody = this.GetComponent<SpeculativeRigidbody>();
                if (specBody)
                {
                    specBody.OnPreRigidbodyCollision += (myBody, myCollider, otherbody, otherCollider) =>
                    {
                        var currentRoom = GameManager.Instance.Dungeon.data.rooms[1];
                        IntVector2 pos = currentRoom.GetCenterCell();
                        if (HasTriggered == false && AllowedToLeave == true)
                        {
                            var player = otherbody.gameObject.GetComponent<PlayerController>();
                            if (player)
                            {
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

            public override void OnDestroy()
            {
                base.OnDestroy();
            }
        }
    }
}
