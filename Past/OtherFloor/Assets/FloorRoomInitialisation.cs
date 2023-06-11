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
        public static PrototypeDungeonRoom Room_5;
        public static PrototypeDungeonRoom Room_6;

        public static PrototypeDungeonRoom Room_1_1;
        public static PrototypeDungeonRoom Room_1_2;
        public static PrototypeDungeonRoom Room_1_3;
        public static PrototypeDungeonRoom Room_1_FuckYou;

        public static PrototypeDungeonRoom Corridor;
        public static PrototypeDungeonRoom BossRoom;


        public static PrototypeDungeonRoom TestRoom;

        public static void InitCustomRooms()
        {

            //var Test = Alexandria.DungeonAPI.RoomFactory.BuildFromRoomFileWithoutTexture("ModularMod/testRoom.newroom").room;
            //Alexandria.DungeonAPI.DungeonHandler.debugFlow = true;
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
            var shitfuck = new PrototypeRoomObjectLayer() { placedObjects = new List<PrototypePlacedObjectData>() { }, delayTime = 0, reinforcementTriggerCondition = RoomEventTriggerCondition.ENEMY_BEHAVIOR };
            var hkjsadha = Room_4.additionalObjectLayers[2]; 
            Room_4.additionalObjectLayers[2] = shitfuck;
            Room_4.additionalObjectLayers.Add(hkjsadha);
            foreach (var wave in Room_4.additionalObjectLayers)
            {
                wave.reinforcementTriggerCondition = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            }
            Room_4.additionalObjectLayers[2].reinforcementTriggerCondition = RoomEventTriggerCondition.NPC_TRIGGER_C;
            Room_4.additionalObjectLayers[3].reinforcementTriggerCondition = RoomEventTriggerCondition.NPC_TRIGGER_C;

            Room_5 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/Room_5.room").room;
            Room_5.usesCustomAmbientLight = true;
            Room_5.customAmbientLight = new Color(0.7f, 0.7f, 0.7f);
            //Room_5.additionalObjectLayers[0].reinforcementTriggerCondition = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            //Room_5.additionalObjectLayers[1].reinforcementTriggerCondition = RoomEventTriggerCondition.ON_THREE_QUARTERS_ENEMY_HP_DEPLETED;

            Room_6 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/Room_6.room").room;
            Room_6.usesCustomAmbientLight = true;
            Room_6.customAmbientLight = new Color(0.6f, 0.2f, 0.2f);

            Room_1_1 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/corridor_entrance_room_.room").room;
            Room_1_1.usesCustomAmbientLight = true;
            Room_1_1.customAmbientLight = new Color(0.4f, 0.4f, 0.4f);

            Room_1_2 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/Room_7.room").room;
            Room_1_2.usesCustomAmbientLight = true;
            Room_1_2.customAmbientLight = new Color(0.7f, 0.5f, 0.5f);

            Room_1_3 = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/Room_8.room").room;
            Room_1_3.usesCustomAmbientLight = true;
            Room_1_3.customAmbientLight = new Color(0.3f, 0.3f, 0.3f);

            Room_1_FuckYou = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/mega_fuck_you_room.room").room;
            Room_1_FuckYou.usesCustomAmbientLight = true;
            Room_1_FuckYou.customAmbientLight = new Color(0.6f, 0.6f, 0.6f);
            foreach (var wave in Room_1_FuckYou.additionalObjectLayers)
            {
                wave.reinforcementTriggerCondition = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            }
            //var fuckYouYouStupidCunt = Room_1_FuckYou.additionalObjectLayers[0];
            //Room_1_FuckYou.additionalObjectLayers[0] = Room_1_FuckYou.additionalObjectLayers[1];
            //Room_1_FuckYou.additionalObjectLayers[1] = fuckYouYouStupidCunt;
            //Room_1_FuckYou.additionalObjectLayers[0].reinforcementTriggerCondition = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
            //Room_1_FuckYou.additionalObjectLayers[0].reinforcementTriggerCondition = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;


            Corridor = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/huge_fuckoff_corridor.room").room;
            Corridor.usesCustomAmbientLight = true;
            Corridor.customAmbientLight = new Color(0.2f, 0.05f, 0.05f);

            Corridor.UseCustomMusic = true;
            Corridor.UseCustomMusicState = false;
            Corridor.CustomMusicEvent = "Play_MUS_Dungeon_State_Winner";
            Corridor.UseCustomMusicSwitch = true;

            Corridor.UseCustomMusicSwitch = true;
            Corridor.CustomMusicSwitch = "Play_ChoirOfTheSilentMachine";

            BossRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/STEEL_PANOPTICON.room").room;
            BossRoom.usesCustomAmbientLight = true;
            BossRoom.customAmbientLight = new Color(0.6f, 0.6f, 0.6f);


            BossRoom.UseCustomMusic = true;
            BossRoom.UseCustomMusicState = false;
            BossRoom.CustomMusicEvent = "Play_MUS_Dungeon_State_Winner";
            BossRoom.UseCustomMusicSwitch = true;

            BossRoom.UseCustomMusicSwitch = true;
            BossRoom.CustomMusicSwitch = "Play_ChoirOfTheSilentMachine";

            TestRoom = Alexandria.DungeonAPI.RoomFactory.BuildFromResource("ModularMod/Past/Rooms/PastDashTwo/fyck you dodgeroll.room").room;
        }
        public static WeightedRoom GenerateWeightedRoom(PrototypeDungeonRoom Room, float Weight = 1, bool LimitedCopies = true, int MaxCopies = 1, DungeonPrerequisite[] AdditionalPrerequisites = null)
        {
            if (Room == null) { return null; }
            if (AdditionalPrerequisites == null) { AdditionalPrerequisites = new DungeonPrerequisite[0]; }
            return new WeightedRoom() { room = Room, weight = Weight, limitedCopies = LimitedCopies, maxCopies = MaxCopies, additionalPrerequisites = AdditionalPrerequisites };
        }
	}
}
