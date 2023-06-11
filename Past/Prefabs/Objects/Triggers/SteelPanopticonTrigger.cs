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
    public class SteelPanopticonTrigger
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("BossTrigger-Panopticon");
            obj.CreateFastBody(CollisionLayer.Pickup, new IntVector2(676, 64), new IntVector2(0, 0));
            obj.AddComponent<Customtrigger>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("BossTrigger_Panopticon", obj);
        }

        public class Customtrigger : BraveBehaviour
        {
            public bool HasTriggered { get; set; }


            public RoomHandler ParentRoom { get; set; }

            public void Start()
            {
                HasTriggered = false;
                var specBody = this.GetComponent<SpeculativeRigidbody>();
                if (specBody)
                {
                    specBody.OnPreRigidbodyCollision += (myBody, myCollider, otherbody, otherCollider) =>
                    {
                        var currentRoom = GameManager.Instance.Dungeon.data.rooms[1];
                        IntVector2 pos = currentRoom.GetCenterCell();
                        if (HasTriggered == false)
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
                                
                                List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
                                for (int i = 0; i < allHealthHavers.Count; i++)
                                {
                                    if (allHealthHavers[i].IsBoss)
                                    {
                                        SteelPanopticonEngager component2 = allHealthHavers[i].GetComponent<SteelPanopticonEngager>();
                                        if (component2)
                                        {
                                            component2.DoDestroy();
                                        }
                                        Destroy(this.gameObject);
                                    }
                                }
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
