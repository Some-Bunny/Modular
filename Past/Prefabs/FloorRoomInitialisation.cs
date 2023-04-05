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

namespace ModularMod
{
	public class FloorRoomInitialisation
    {
        public static PrototypeDungeonRoom StartRoom;
        public static PrototypeDungeonRoom FirstRoom;

        public static void InitCustomRooms()
        {
            StartRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/Debug/mdl_past_room_test.room").room;
            FirstRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/Debug/mdl_past_room_test.room").room;
            FirstRoom.overrideRoomVisualType = 2;
        }
        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }
	}
}
