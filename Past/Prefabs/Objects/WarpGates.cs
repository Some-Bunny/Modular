﻿using Alexandria.PrefabAPI;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ModularMod.Past.Prefabs.Objects
{
    public class WarpGates
    {
        private class WarpGatePoint : MonoBehaviour { public bool active = false; public void Start() { active = true; } }
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("WarpGate_Exit");
            obj.AddComponent<WarpGatePoint>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("WarpGateExitMDLR_Entrance", obj);


            GameObject obj1 = PrefabBuilder.BuildObject("WarpGate_Entrance");
            AdditionalBraveLight braveLight = obj1.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = obj1.transform.position + new Vector3(4, 0);
            braveLight.LightColor = Color.white;//WarpGateExitMDLR_Past
            braveLight.LightIntensity = 10f;
            braveLight.LightRadius = 40f;
            braveLight.LightAngle = 60;
            braveLight.UsesCone = true;
            braveLight.LightOrient = 90;
            obj1.AddComponent<WarpGateEntrancePoint>();

            obj1.CreateFastBody(CollisionLayer.Pickup, new IntVector2(48, 80), new IntVector2(-16, -16));
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("WarpGateExitMDLR_Past", obj1);
        }

        private class WarpGateEntrancePoint : MonoBehaviour 
        {
            private bool Triggered = false;
            public void Start()
            {
                AdditionalBraveLight braveLight = this.GetComponent<AdditionalBraveLight>();
                braveLight.transform.position = this.transform.position;

                var specBody = this.GetComponent<SpeculativeRigidbody>();
                if (specBody)
                {
                    specBody.OnPreRigidbodyCollision += (myBody, myCollider, otherbody, otherCollider) =>
                    {
                        var currentRoom = GameManager.Instance.Dungeon.data.rooms[1];
                            IntVector2 pos = currentRoom.GetCenterCell();
                        if (Triggered == false)
                        {
                            var player = otherbody.gameObject.GetComponent<PlayerController>();
                            Triggered = true;
                            if (player)
                            {
                                player.WarpToPoint(pos.ToCenterVector2() - new Vector2(20, 35), false, false);
                            }
                            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                            {
                                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(player);
                                if (otherPlayer)
                                {
                                    otherPlayer.ReuniteWithOtherPlayer(player, false);
                                }
                            }
                        }
                    };
                }
            }
        }
    }
}