using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace ModularMod
{
    public class DungeonFlowHelper
    {


        public static List<DungeonFlow> KnownFlows;

        public static DungeonFlow Foyer_Flow;

        // Default stuff to use with custom Flows
        public static SharedInjectionData BaseSharedInjectionData;
        public static SharedInjectionData GungeonInjectionData;
        public static SharedInjectionData SewersInjectionData;
        public static SharedInjectionData HollowsInjectionData;
        public static SharedInjectionData CastleInjectionData;

        public static ProceduralFlowModifierData SecretFloorNameEntranceInjector;

        public static DungeonFlowSubtypeRestriction BaseSubTypeRestrictions = new DungeonFlowSubtypeRestriction()
        {
            baseCategoryRestriction = PrototypeDungeonRoom.RoomCategory.NORMAL,
            normalSubcategoryRestriction = PrototypeDungeonRoom.RoomNormalSubCategory.TRAP,
            bossSubcategoryRestriction = PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS,
            specialSubcategoryRestriction = PrototypeDungeonRoom.RoomSpecialSubCategory.UNSPECIFIED_SPECIAL,
            secretSubcategoryRestriction = PrototypeDungeonRoom.RoomSecretSubCategory.UNSPECIFIED_SECRET,
            maximumRoomsOfSubtype = 1
        };

        // Custom Room Table for Keep Shared Injection Data 
        public static GenericRoomTable m_KeepEntranceRooms;

        // Generate a DungeonFlowNode with a default configuration
        public static DungeonFlowNode GenerateDefaultNode(DungeonFlow targetflow, PrototypeDungeonRoom.RoomCategory roomType, PrototypeDungeonRoom overrideRoom = null, GenericRoomTable overrideTable = null, bool oneWayLoopTarget = false, bool isWarpWingNode = false, string nodeGUID = null, DungeonFlowNode.NodePriority priority = DungeonFlowNode.NodePriority.MANDATORY, float percentChance = 1, bool handlesOwnWarping = true)
        {
            try
            {
                if (string.IsNullOrEmpty(nodeGUID)) { nodeGUID = Guid.NewGuid().ToString(); }

                DungeonFlowNode m_CachedNode = new DungeonFlowNode(targetflow)
                {
                    isSubchainStandin = false,
                    nodeType = DungeonFlowNode.ControlNodeType.ROOM,
                    roomCategory = roomType,
                    percentChance = percentChance,
                    priority = priority,
                    overrideExactRoom = overrideRoom,
                    overrideRoomTable = overrideTable,
                    capSubchain = false,
                    subchainIdentifier = string.Empty,
                    limitedCopiesOfSubchain = false,
                    maxCopiesOfSubchain = 1,
                    subchainIdentifiers = new List<string>(0),
                    receivesCaps = false,
                    isWarpWingEntrance = isWarpWingNode,
                    handlesOwnWarping = handlesOwnWarping,
                    forcedDoorType = DungeonFlowNode.ForcedDoorType.NONE,
                    loopForcedDoorType = DungeonFlowNode.ForcedDoorType.NONE,
                    nodeExpands = false,
                    initialChainPrototype = "n",
                    chainRules = new List<ChainRule>(0),
                    minChainLength = 3,
                    maxChainLength = 8,
                    minChildrenToBuild = 1,
                    maxChildrenToBuild = 1,
                    canBuildDuplicateChildren = false,
                    guidAsString = nodeGUID,
                    parentNodeGuid = string.Empty,
                    childNodeGuids = new List<string>(0),
                    loopTargetNodeGuid = string.Empty,
                    loopTargetIsOneWay = oneWayLoopTarget,
                    flow = targetflow
                };



                return m_CachedNode;
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
                return null;
            }
        }


        public static List<SharedInjectionData> RetrieveSharedInjectionDataListFromCurrentFloor()
        {
            Dungeon dungeon = GameManager.Instance.CurrentlyGeneratingDungeonPrefab;

            if (dungeon == null)
            {
                dungeon = GameManager.Instance.Dungeon;
                if (dungeon == null) { return new List<SharedInjectionData>(0); }

            }

            if (dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FINALGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON |
                dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
            {
                return new List<SharedInjectionData>(0);
            }

            List<SharedInjectionData> m_CachedInjectionDataList = new List<SharedInjectionData>(0);

            if (dungeon.PatternSettings != null && dungeon.PatternSettings.flows != null && dungeon.PatternSettings.flows.Count > 0)
            {
                if (dungeon.PatternSettings.flows[0].sharedInjectionData != null && dungeon.PatternSettings.flows[0].sharedInjectionData.Count > 0)
                {
                    m_CachedInjectionDataList = dungeon.PatternSettings.flows[0].sharedInjectionData;
                }
            }

            return m_CachedInjectionDataList;
        }


    }

    class CustomFlow : DungeonFlowHelper
    {
        public static DungeonFlow Default_Flow;

        
		public static void GenerateFlows()
		{
			try
			{
                BaseSharedInjectionData = ResourceManager.LoadAssetBundle("shared_auto_002").LoadAsset<SharedInjectionData>("Base Shared Injection Data");

                var MinesDungeonPrefab = PastDungeon.GetOrLoadByName_Orig("Base_Mines");

                DungeonFlow m_CachedFlow = ScriptableObject.CreateInstance<DungeonFlow>();
                DungeonFlowNode entranceNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, FloorRoomInitialisation.StartRoom);
                m_CachedFlow.AddNodeToFlow(entranceNode, null);
                m_CachedFlow.FirstNode = entranceNode;

                DungeonFlowNode firstroom = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisation.FirstRoom);
                m_CachedFlow.AddNodeToFlow(firstroom, entranceNode);
                

                m_CachedFlow.name = "sadadsad";
                m_CachedFlow.fallbackRoomTable = MinesDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable; //ModPrefabs.FloorNameRoomTable;
                m_CachedFlow.phantomRoomTable = null;
                m_CachedFlow.subtypeRestrictions = new List<DungeonFlowSubtypeRestriction>(0);
                m_CachedFlow.flowInjectionData = new List<ProceduralFlowModifierData>(0);
                m_CachedFlow.sharedInjectionData = new List<SharedInjectionData>() { };

                Default_Flow = m_CachedFlow;
                MinesDungeonPrefab = null;
                /*
				DungeonFlowNode entranceNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, ModRoomPrefabs.Mod_Entrance_Room);
				DungeonFlowNode exitNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.EXIT, ModRoomPrefabs.Mod_Exit_Room);
				DungeonFlowNode bossfoyerNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.SPECIAL, overrideTable: ModPrefabs.boss_foyertable);
				DungeonFlowNode bossNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.BOSS, ModRoomPrefabs.Mod_Boss);

				DungeonFlowNode FloorNameShopNode = GenerateDefaultNode(m_CachedFlow, ModPrefabs.shop02.category, overrideTable: ModPrefabs.shop_room_table);
				DungeonFlowNode FloorNameRewardNode_01 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.CONNECTOR, ModPrefabs.gungeon_rewardroom_1);
				DungeonFlowNode FloorNameRewardNode_02 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.CONNECTOR, ModPrefabs.gungeon_rewardroom_1);


				DungeonFlowNode FloorNameRoomNode_01 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, oneWayLoopTarget: true);
				DungeonFlowNode FloorNameRoomNode_02 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_04 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_05 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_06 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, oneWayLoopTarget: true);
				DungeonFlowNode FloorNameRoomNode_07 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_09 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_10 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_11 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB);
				DungeonFlowNode FloorNameRoomNode_12 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_13 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_14 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_16 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_17 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);
				DungeonFlowNode FloorNameRoomNode_18 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL);



				m_CachedFlow.Initialize();

				m_CachedFlow.AddNodeToFlow(entranceNode, null);
				// First Looping branch
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_16, entranceNode);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_01, FloorNameRoomNode_16);
				// Dead End
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_05, FloorNameRoomNode_01);
				// Start of Loop
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_02, FloorNameRoomNode_01);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_04, FloorNameRoomNode_02);
				m_CachedFlow.AddNodeToFlow(FloorNameRewardNode_01, FloorNameRoomNode_04);
				// Connect End of Loop to first in chain
				m_CachedFlow.LoopConnectNodes(FloorNameRewardNode_01, FloorNameRoomNode_01);

				// Second Looping branch
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_17, entranceNode);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_06, FloorNameRoomNode_17);
				// Dead End
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_10, FloorNameRoomNode_06);
				// Start of Loop
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_07, FloorNameRoomNode_06);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_09, FloorNameRoomNode_07);
				m_CachedFlow.AddNodeToFlow(FloorNameRewardNode_02, FloorNameRoomNode_09);
				// Connect End of Loop to first in chain
				m_CachedFlow.LoopConnectNodes(FloorNameRewardNode_02, FloorNameRoomNode_06);

				// Splitting path to Shop or Boss
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_18, entranceNode);
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_11, FloorNameRoomNode_18);
				// Path To Boss
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_12, FloorNameRoomNode_11);
				// Path to Shop
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_13, FloorNameRoomNode_11);
				m_CachedFlow.AddNodeToFlow(FloorNameShopNode, FloorNameRoomNode_13);
				// Dead End
				m_CachedFlow.AddNodeToFlow(FloorNameRoomNode_14, FloorNameRoomNode_11);


				m_CachedFlow.AddNodeToFlow(bossfoyerNode, FloorNameRoomNode_12);
				m_CachedFlow.AddNodeToFlow(bossNode, bossfoyerNode);
				m_CachedFlow.AddNodeToFlow(exitNode, bossNode);

				m_CachedFlow.FirstNode = entranceNode;
                */
            }
			catch (Exception e)
			{
				ETGModConsole.Log(e.ToString());
			}
		}   
    }
}
