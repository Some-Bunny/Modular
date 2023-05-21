using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    class DungeonStampDataSetupChain
    {
        
        public static void InitCustomDungeonStampData()
        {
            FloorStampData = ScriptableObject.CreateInstance<DungeonTileStampData>();
            FloorStampData.name = "Modular_Past_StampData_2";
            FloorStampData.spriteStampWeight = 1f;
            FloorStampData.objectStampWeight = 1f;
            FloorStampData.tileStampWeight = 1f;

            FloorStampData.objectStamps = new ObjectStampData[0];
            FloorStampData.spriteStamps = new SpriteStampData[0];

            FloorStampData.stamps = new TileStampData[]
            {
                
                

            };
        }
        public static DungeonTileStampData FloorStampData;
    }
}
