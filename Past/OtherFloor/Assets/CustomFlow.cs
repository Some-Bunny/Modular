using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using MonoMod.RuntimeDetour;
using System.Reflection;
using MonoMod.Cil;

namespace ModularMod
{
    class SuperFlow : DungeonFlowHelper
    {
        public static DungeonFlow Default_Flow;

        
		public static void GenerateFlows()
		{
			try
			{
                BaseSharedInjectionData = ResourceManager.LoadAssetBundle("shared_auto_002").LoadAsset<SharedInjectionData>("Base Shared Injection Data");

                var MinesDungeonPrefab = PastDungeon.GetOrLoadByName_Orig("Base_Mines");

                DungeonFlow m_CachedFlow = ScriptableObject.CreateInstance<DungeonFlow>();

                DungeonFlowNode entranceNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, FloorRoomInitialisationChain.StartRoom);
                m_CachedFlow.AddNodeToFlow(entranceNode, null);
                m_CachedFlow.FirstNode = entranceNode;
                entranceNode.guidAsString = "ASS";
                entranceNode.priority = DungeonFlowNode.NodePriority.MANDATORY;
				entranceNode.isWarpWingEntrance = false;
				



                m_CachedFlow.name = "sadadsad";
                m_CachedFlow.fallbackRoomTable = MinesDungeonPrefab.PatternSettings.flows[0].fallbackRoomTable; //ModPrefabs.FloorNameRoomTable;
                m_CachedFlow.phantomRoomTable = null;
                m_CachedFlow.subtypeRestrictions = new List<DungeonFlowSubtypeRestriction>(0);
                m_CachedFlow.flowInjectionData = new List<ProceduralFlowModifierData>(0);
                m_CachedFlow.sharedInjectionData = new List<SharedInjectionData>() { };
                


                DungeonFlowNode WeezerNode = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Weezer_Room);
                m_CachedFlow.AddNodeToFlow(WeezerNode, entranceNode);

                DungeonFlowNode Node2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_2);
                m_CachedFlow.AddNodeToFlow(Node2, WeezerNode);

                DungeonFlowNode Node3 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.HUB, FloorRoomInitialisationChain.Room_3);
                m_CachedFlow.AddNodeToFlow(Node3, Node2);

                DungeonFlowNode Node1_1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_1_1);
                m_CachedFlow.AddNodeToFlow(Node1_1, Node3);

                DungeonFlowNode Node1_2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_1_2);
                m_CachedFlow.AddNodeToFlow(Node1_2, Node1_1);




                DungeonFlowNode Node1_3 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_1_3);
                m_CachedFlow.AddNodeToFlow(Node1_3, Node1_2);

                //DungeonFlowNode FUCK2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, FloorRoomInitialisationChain.Corridor);
                //m_CachedFlow.AddNodeToFlow(FUCK2, Node1_3);

                //DungeonFlowNode FUCK1 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.BOSS, FloorRoomInitialisationChain.BossRoom);
                //m_CachedFlow.AddNodeToFlow(FUCK1, null);




                DungeonFlowNode Node1_FuckYou = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_1_FuckYou);
                m_CachedFlow.AddNodeToFlow(Node1_FuckYou, Node1_3);


                DungeonFlowNode FUCK2 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Corridor);
                m_CachedFlow.AddNodeToFlow(FUCK2, Node1_FuckYou);

                DungeonFlowNode FUCK3 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.BOSS, FloorRoomInitialisationChain.BossRoom);
                m_CachedFlow.AddNodeToFlow(FUCK3, FUCK2);

                DungeonFlowNode Node4 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_4);
                m_CachedFlow.AddNodeToFlow(Node4, Node3);

                DungeonFlowNode Node5 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_5);
                m_CachedFlow.AddNodeToFlow(Node5, Node4);

                DungeonFlowNode Node6 = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Room_6);
                m_CachedFlow.AddNodeToFlow(Node6, Node5);


                //DungeonFlowNode corridor = GenerateDefaultNode(m_CachedFlow, PrototypeDungeonRoom.RoomCategory.NORMAL, FloorRoomInitialisationChain.Corridor);
                //m_CachedFlow.AddNodeToFlow(corridor, Node1_3);


                Default_Flow = m_CachedFlow;
                MinesDungeonPrefab = null;

                /*
                var data = PastDungeon.GetOrLoadByName_Orig("finalscenario_bullet");
                foreach(var thing in data.PatternSettings.flows[0].m_nodes)
                {
                    //thing.LogPropertiesAndFields();
                    Debug.Log("child node guids");
                    foreach (var childGUID in thing.childNodeGuids)
                    {
                        Debug.Log(childGUID);
                    }
                    Debug.Log("");
                    Debug.Log("subchainIdentifiers");
                    foreach (var childGUID in thing.subchainIdentifiers)
                    {
                        Debug.Log(childGUID);
                    }
                }
                data = null;
                */
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
