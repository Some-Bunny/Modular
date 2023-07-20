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
        public static PrototypeDungeonRoom BossRoom;

        public static void InitCustomRooms()
        {
            StartRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/ships_co.room").room;
            StartRoom.customAmbientLight = new Color(0.3f, 0.3f, 0.3f, 1);
            StartRoom.usesCustomAmbientLight = true;

            FirstRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/MegaRoom.room").room;
            FirstRoom.overrideRoomVisualType = 2;
            FirstRoom.customAmbientLight = new Color(0.7f, 0.7f, 0.7f, 1);
            FirstRoom.usesCustomAmbientLight = true;
            BossRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/past_boss_room.room").room;
            BossRoom.overrideRoomVisualType = 2;
        }
        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }
	}
}
