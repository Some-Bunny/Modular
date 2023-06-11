using Alexandria.PrefabAPI;
using Dungeonator;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.Past.Prefabs.Objects.BigFuckOffDoor;

namespace ModularMod.Past.Prefabs.Objects
{
    public class TriggerEncounter
    {
        public static void Init()
        {
            GameObject obj = PrefabBuilder.BuildObject("CombatTrigger_MDLR");
            obj.AddComponent<CombatTriggerBehavior>();
            Alexandria.DungeonAPI.StaticReferences.customObjects.Add("CombatTrigger_MDLR", obj);
        }

        public class CombatTriggerBehavior : MonoBehaviour
        {
            private RoomHandler thisRoom;
            private bool Trigger = false;
            public void Start()
            {
                GlobalMessageRadio.RegisterObjectToRadio(this.gameObject, new List<string>() { "ShutDown" }, OnRecieveMessage);
                thisRoom = this.transform.position.GetAbsoluteRoom();
                thisRoom.Entered += ThisRoom_Entered;
            }
            public void OnRecieveMessage(GameObject obj, string message)
            {
                Trigger = true;
            }
            private void ThisRoom_Entered(PlayerController p)
            {
                if (Trigger == true)
                {
                    thisRoom.SealRoom();
                    Trigger = !Trigger;
                    for (int i = 0; i < 2; i++)
                    {
                        thisRoom.TriggerNextReinforcementLayer();
                    }
                }
            }
        }
    }
}
