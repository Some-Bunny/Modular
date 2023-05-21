using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using Alexandria.DungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;

namespace ModularMod
{
	public class FloorRoomInitialisationChain
    {
        public static PrototypeDungeonRoom StartRoom;
        public static PrototypeDungeonRoom FirstRoom;

        public static PrototypeDungeonRoom Weezer_Room;
        public static PrototypeDungeonRoom Room_2;
        public static PrototypeDungeonRoom Room_3;
        public static PrototypeDungeonRoom Room_4;

        public static PrototypeDungeonRoom BossRoom;

        public static void InitCustomRooms()
        {
            //Play_ChoirOfTheSilentMachine
            StartRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/pad2_entrance.room").room;
            StartRoom.UseCustomMusic = true;
            StartRoom.UseCustomMusicState = false;
            StartRoom.CustomMusicEvent = "Play_MUS_Dungeon_State_Winner";
            StartRoom.UseCustomMusicSwitch = true;

            StartRoom.UseCustomMusicSwitch = true;
            StartRoom.CustomMusicSwitch = "Play_ChoirOfTheSilentMachine";

            Weezer_Room = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/hell.room").room;
            foreach (var wave in Weezer_Room.additionalObjectLayers)
            {
                wave.reinforcementTriggerCondition = RoomEventTriggerCondition.ON_HALF_ENEMY_HP_DEPLETED;
            }

            Room_2 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/Room2.room").room;
            Room_2.usesCustomAmbientLight = true;
            Room_2.customAmbientLight = new Color(0.7f, 0.7f, 0.7f);
            foreach (var wave in Room_2.additionalObjectLayers)
            {
                wave.reinforcementTriggerCondition = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            }


            Room_3 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/Room3.room").room;
            Room_3.usesCustomAmbientLight = true;
            Room_3.customAmbientLight = new Color(0.7f, 0.7f, 0.7f);
            foreach (var wave in Room_3.additionalObjectLayers)
            {
                wave.reinforcementTriggerCondition = RoomEventTriggerCondition.ON_NINETY_PERCENT_ENEMY_HP_DEPLETED;
            }


            Room_4 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/Room4_2.room").room;
            Room_4.usesCustomAmbientLight = true;
            Room_4.customAmbientLight = new Color(0.8f, 0.5f, 0.5f);
            foreach (var wave in Room_3.additionalObjectLayers)
            {
                wave.reinforcementTriggerCondition = RoomEventTriggerCondition.ON_NINETY_PERCENT_ENEMY_HP_DEPLETED;
            }
        }
        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }
	}
}
