using System;
using System.Collections.Generic;
using UnityEngine;
using Dungeonator;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace ModularMod
{
    public class DungeonHooks
    {
        public static void Init()
        {
            //================================
            new Hook(
            typeof(RoomHandler).GetMethod("AddProceduralTeleporterToRoom", BindingFlags.Instance | BindingFlags.Public),
            typeof(DungeonHooks).GetMethod("AddProceduralTeleporterToRoomHook", BindingFlags.Static | BindingFlags.Public)
            );
            //================================

            //================================
            new Hook(typeof(DungeonData).GetMethod("FloodFillDungeonInterior", BindingFlags.Instance | BindingFlags.NonPublic), typeof(DungeonHooks).GetMethod("FloodFillDungeonInteriorHook"));
            //================================

            //================================
            new Hook(typeof(RoomHandler).GetMethod("CheckCellArea", BindingFlags.Instance | BindingFlags.NonPublic), typeof(DungeonHooks).GetMethod("CheckCellAreaHook"));
            //================================
        }
        public static void AddProceduralTeleporterToRoomHook(Action<RoomHandler> orig, RoomHandler roomHandler)
        {
            //yes, this is a really cheap and shit way of preventing teleporters on **specificaly** my floor but it should work
            if (GameManager.Instance.Dungeon.BossMasteryTokenItemId == ModulePrinterCore.ModulePrinterCoreID) { return; }
            orig(roomHandler);
        }
        public static bool CheckCellAreaHook(Func<RoomHandler, IntVector2, IntVector2, bool> orig, RoomHandler self, IntVector2 basePosition, IntVector2 objDimensions)
        {
            DungeonData data = GameManager.Instance.Dungeon.data;
            bool result = true;
            for (int i = basePosition.x; i < basePosition.x + objDimensions.x; i++)
            {
                for (int j = basePosition.y; j < basePosition.y + objDimensions.y; j++)
                {
                    CellData cellData = data.cellData[i][j];
                    if (cellData != null)
                    {
                        if (!cellData.IsPassable)
                        {
                            return false;
                        }
                    }

                }
            }
            return result;
        }
        public static void FloodFillDungeonInteriorHook(Action<DungeonData> orig, DungeonData self)
        {
            Stack<CellData> stack = new Stack<CellData>();
            for (int i = 0; i < self.rooms.Count; i++)
            {
                if (self.rooms[i] == self.Entrance || self.rooms[i].IsStartOfWarpWing)
                {
                    //Debug.Log(self.rooms[i].GetRoomName());
                    try
                    {
                        stack.Push(self[self.rooms[i].GetRandomAvailableCellDumb()]);
                    }
                    catch (Exception ex)
                    {
                        //ETGModConsole.Log("[ExpandTheGungeon] Warning: Exception caught at DungeonData.FloodFillDungeonInterior!");
                        Debug.LogException(ex);
                    }
                }
            }
            try
            {
                while (stack.Count > 0)
                {
                    CellData cellData = stack.Pop();
                    if (cellData.type != CellType.WALL)
                    {
                        List<CellData> cellNeighbors = self.GetCellNeighbors(cellData, false);
                        cellData.isGridConnected = true;
                        for (int j = 0; j < cellNeighbors.Count; j++)
                        {
                            if (cellNeighbors[j] != null && cellNeighbors[j].type != CellType.WALL && !cellNeighbors[j].isGridConnected)
                            {
                                stack.Push(cellNeighbors[j]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("[Modular] Warning: Exception caught at DungeonData.FloodFillDungeonInterior!");
                Debug.LogException(ex);
            }

        }
    }
}
