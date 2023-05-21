using Alexandria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    class DungeonMaterialSetup
    {  
        public static void InitCustomDungeonMaterial()
        {
            var past_Marine = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");

            floorMaterial = ScriptableObject.CreateInstance<DungeonMaterial>();
            floorMaterial.supportsPits = true;
            floorMaterial.doPitAO = false;
            floorMaterial.useLighting = true;
            floorMaterial.supportsDiagonalWalls = false;
            floorMaterial.carpetIsMainFloor = false;
            floorMaterial.carpetGrids = new TileIndexGrid[0];
            floorMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
            floorMaterial.additionalPitBorderFlatGrid = TilesetToolbox.CreateBlankIndexGrid();
            floorMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
            

            floorMaterial.fallbackHorizontalTileMapEffects = new VFXComplex[] { (PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects.effects[0] };
            floorMaterial.fallbackVerticalTileMapEffects = new VFXComplex[] { (PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects.effects[0] };


            string[] pathsSmall = new string[]
            {
                "Planetside/Resources/FloorStuff/WallShards/wallshard1_001.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard1_002.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard1_003.png",

            };
            string[] pathsLarge = new string[]
            {
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_001.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_002.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_003.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_004.png",

            };
            floorMaterial.wallShards = past_Marine.roomMaterialDefinitions[0].wallShards;
            floorMaterial.bigWallShardDamageThreshold = 20;
            floorMaterial.bigWallShards = past_Marine.roomMaterialDefinitions[0].bigWallShards;
            floorMaterial.secretRoomWallShardCollections = past_Marine.roomMaterialDefinitions[0].secretRoomWallShardCollections;

            past_Marine = null;


            var ceilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();

            ceilingBorderGrid.topIndices = new TileIndexList { indices = new List<int> { 35 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.leftIndices = new TileIndexList { indices = new List<int> { 37 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.rightIndices = new TileIndexList { indices = new List<int> { 36 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomIndices = new TileIndexList { indices = new List<int> { 34 }, indexWeights = new List<float> { 1f } };


            ceilingBorderGrid.topLeftNubIndices = new TileIndexList { indices = new List<int> { 41 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topRightNubIndices = new TileIndexList { indices = new List<int> { 40 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 39 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 0 }, indexWeights = new List<float> { 1f } };


            floorMaterial.roomCeilingBorderGrid = ceilingBorderGrid;
            var pitBorderGridCave = TilesetToolbox.CreateBlankIndexGrid();
            pitBorderGridCave.topIndices = new TileIndexList { indices = new List<int> { 20,21,22,23 }, indexWeights = new List<float> { 1f,1,1,1 } };
            pitBorderGridCave.leftIndices = new TileIndexList { indices = new List<int> { 20, 21, 22, 23 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.rightIndices = new TileIndexList { indices = new List<int> { 20, 21, 22, 23 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.bottomIndices = new TileIndexList { indices = new List<int> { 20, 21, 22, 23 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };


            pitBorderGridCave.topLeftNubIndices = new TileIndexList { indices = new List<int> { 20, 21, 22, 23 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.topRightNubIndices = new TileIndexList { indices = new List<int> { 20, 21, 22, 23 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 20, 21, 22, 23 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 20, 21, 22, 23 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.CenterIndicesAreStrata = false;
            floorMaterial.roomFloorBorderGrid = pitBorderGridCave;


            /*
            var pitBorderGridCave = TilesetToolbox.CreateBlankIndexGrid();
            pitBorderGridCave.topLeftIndices = new TileIndexList { indices = new List<int> { 1 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topRightIndices = new TileIndexList { indices = new List<int> { 3 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.leftIndices = new TileIndexList { indices = new List<int> { 4 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.rightIndices = new TileIndexList { indices = new List<int> { 5 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomLeftIndices = new TileIndexList { indices = new List<int> { 6 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomIndices = new TileIndexList { indices = new List<int> { 7 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomRightIndices = new TileIndexList { indices = new List<int> { 8 }, indexWeights = new List<float> { 1f } };

            pitBorderGridCave.topRightNubIndices = new TileIndexList { indices = new List<int> { 9 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topLeftNubIndices = new TileIndexList { indices = new List<int> { 10 }, indexWeights = new List<float> { 1f } };

            pitBorderGridCave.horizontalIndices = new TileIndexList { indices = new List<int> { 11 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.verticalIndices = new TileIndexList { indices = new List<int> { 12 }, indexWeights = new List<float> { 1f } };


            pitBorderGridCave.topCapIndices = new TileIndexList { indices = new List<int> { 13 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomCapIndices = new TileIndexList { indices = new List<int> { 14 }, indexWeights = new List<float> { 1f } };
            */

            //pitBorderGridCave.allSidesIndices = new TileIndexList { indices = new List<int> { 10 }, indexWeights = new List<float> { 1f } };

            //
            //pitBorderGridCave.rightCapIndices = new TileIndexList { indices = new List<int> { 81 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.leftCapIndices = new TileIndexList { indices = new List<int> { 83 }, indexWeights = new List<float> { 1f } };



            //pitBorderGridCave.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 89 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.extendedSet = true;
            //pitBorderGridCave.centerIndices = new TileIndexList { indices = new List<int> { 0 }, indexWeights = new List<float> { 1f } };


            //pitBorderGridCave.topCenterLeftIndices = new TileIndexList { indices = new List<int> { 89 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.topCenterIndices = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };


            //pitBorderGridCave.topCenterRightIndices = new TileIndexList { indices = new List<int> { 91 }, indexWeights = new List<float> { 1f } };

            //pitBorderGridCave.thirdTopRowLeftIndices = new TileIndexList { indices = new List<int> { 92 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.thirdTopRowCenterIndices = new TileIndexList { indices = new List<int> { 93 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.thirdTopRowRightIndices = new TileIndexList { indices = new List<int> { 94 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.internalBottomLeftCenterIndices = new TileIndexList { indices = new List<int> { 95 }, indexWeights = new List<float> { 1f } };


            /*
            pitBorderGridCave.internalBottomCenterIndices = new TileIndexList { indices = new List<int> { 101 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.internalBottomRightCenterIndices = new TileIndexList { indices = new List<int> { 102 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubLeftIndices = new TileIndexList { indices = new List<int> { 103 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubRightIndices = new TileIndexList { indices = new List<int> { 104 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 105 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 106 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 107 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 108 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 109 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 110 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 111 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 112 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 113 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 114 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 115 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 116 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsTop = new TileIndexList { indices = new List<int> { 117 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsRight = new TileIndexList { indices = new List<int> { 118 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsBottom = new TileIndexList { indices = new List<int> { 119 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsLeft = new TileIndexList { indices = new List<int> { 120 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.quadNubs = new TileIndexList { indices = new List<int> { 121 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topRightWithNub = new TileIndexList { indices = new List<int> { 122 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topLeftWithNub = new TileIndexList { indices = new List<int> { 123 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomRightWithNub = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 125 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderNE = new TileIndexList { indices = new List<int> { 126 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderSE = new TileIndexList { indices = new List<int> { 127 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderSW = new TileIndexList { indices = new List<int> { 128 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderNW = new TileIndexList { indices = new List<int> { 129 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingNE = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingSE = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingSW = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingNW = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            */
            //pitBorderGridCave.CenterCheckerboard = false;
            //pitBorderGridCave.CheckerboardDimension = 0;

            //pitBorderGridCave.CenterIndicesAreStrata = false;
            //pitBorderGridCave.PitInternalSquareGrids = new List<TileIndexGrid>() { pitBorderGridCave };





            //pitBorderGridCave.PitInternalSquareOptions = new PitSquarePlacementOptions() { CanBeFlushBottom = true, CanBeFlushLeft = true, CanBeFlushRight = true, PitSquareChance = 1 };
            //pitBorderGridCave.PitBorderIsInternal = false;
            //pitBorderGridCave.PitBorderOverridesFloorTile = false;
            //pitBorderGridCave.CeilingBorderUsesDistancedCenters = false;
            //pitBorderGridCave.UsesRatChunkBorders = false;
            //pitBorderGridCave.RatChunkNormalSet = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.RatChunkBottomSet = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.PathFacewallStamp = ob;
            //pitBorderGridCave.PathSidewallStamp =;
            //pitBorderGridCave.PathPitPosts = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.PathPitPostsBL = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.PathPitPostsBR = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //floorMaterial.roomFloorBorderGrid = pitBorderGridCave;
            //floorMaterial.outerCeilingBorderGrid = pitBorderGridCave;

            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================

            //To make 2x2 squares
            /*
            var floorSquares = TilesetToolbox.CreateBlankIndexGrid();

            floorSquares.topLeftIndices = new TileIndexList { indices = new List<int> { 25 }, indexWeights = new List<float> { 1f } };
            floorSquares.topRightIndices = new TileIndexList { indices = new List<int> { 26 }, indexWeights = new List<float> { 1f } };
            floorSquares.bottomLeftIndices = new TileIndexList { indices = new List<int> { 27 }, indexWeights = new List<float> { 1f } };
            floorSquares.bottomRightIndices = new TileIndexList { indices = new List<int> { 28 }, indexWeights = new List<float> { 1f } };

            var floorSquares1 = TilesetToolbox.CreateBlankIndexGrid();
            floorSquares1.topLeftIndices = new TileIndexList { indices = new List<int> { 29 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.topRightIndices = new TileIndexList { indices = new List<int> { 30 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.bottomLeftIndices = new TileIndexList { indices = new List<int> { 31 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.bottomRightIndices = new TileIndexList { indices = new List<int> { 32 }, indexWeights = new List<float> { 0.5f } };

            floorMaterial.floorSquares = new TileIndexGrid[] { floorSquares, floorSquares1 };
            */
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================
            {





                /*
                ceilingBorderGrid.topLeftIndices = new TileIndexList { indices = new List<int> { 60 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topIndices = new TileIndexList { indices = new List<int> { 61 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topRightIndices = new TileIndexList { indices = new List<int> { 62 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.leftIndices = new TileIndexList { indices = new List<int> { 38, 40 }, indexWeights = new List<float> { 1f, 0.9f } };

                //ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 96 }, indexWeights = new List<float> { 1f } };
                //new TileIndexList { indices = new List<int> { 96, 97, 98, 99 }, indexWeights = new List<float> { 1f, 0.06f, 0.06f, 0.06f } };

                ceilingBorderGrid.rightIndices = new TileIndexList { indices = new List<int> { 39, 41 }, indexWeights = new List<float> { 1f, 0.9f } };


                ceilingBorderGrid.bottomLeftIndices = new TileIndexList { indices = new List<int> { 66 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomIndices = new TileIndexList { indices = new List<int> { 67 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightIndices = new TileIndexList { indices = new List<int> { 68 }, indexWeights = new List<float> { 1f } };

                ceilingBorderGrid.verticalIndices = new TileIndexList { indices = new List<int> { 105 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.horizontalIndices = new TileIndexList { indices = new List<int> { 106 }, indexWeights = new List<float> { 1f } };

                //Caps
                ceilingBorderGrid.topCapIndices = new TileIndexList { indices = new List<int> { 103 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.leftCapIndices = new TileIndexList { indices = new List<int> { 101 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.rightCapIndices = new TileIndexList { indices = new List<int> { 102 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomCapIndices = new TileIndexList { indices = new List<int> { 104 }, indexWeights = new List<float> { 1f } };

                //

                //========================================
                //Nubs
                ceilingBorderGrid.doubleNubsRight = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsLeft = new TileIndexList { indices = new List<int> { 81 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsTop = new TileIndexList { indices = new List<int> { 82 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsBottom = new TileIndexList { indices = new List<int> { 83 }, indexWeights = new List<float> { 1f } };

                ceilingBorderGrid.quadNubs = new TileIndexList { indices = new List<int> { 84 }, indexWeights = new List<float> { 1f } };

                ///Corner Nubs
                ceilingBorderGrid.topRightWithNub = new TileIndexList { indices = new List<int> { 108 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topLeftWithNub = new TileIndexList { indices = new List<int> { 107 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightWithNub = new TileIndexList { indices = new List<int> { 109 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 110 }, indexWeights = new List<float> { 1f } };
                //========================================

                ceilingBorderGrid.allSidesIndices = new TileIndexList { indices = new List<int> { 75 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topLeftNubIndices = new TileIndexList { indices = new List<int> { 76 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topRightNubIndices = new TileIndexList { indices = new List<int> { 77 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 78 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 79 }, indexWeights = new List<float> { 1f } };

                ceilingBorderGrid.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
                //


                ceilingBorderGrid.borderTopNubLeftIndices = new TileIndexList { indices = new List<int> { 117 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderTopNubRightIndices = new TileIndexList { indices = new List<int> { 118 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 119 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 115 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 116 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 120 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 111 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 112 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 122 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 113 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 114 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 121 }, indexWeights = new List<float> { 1f } };

                //ceilingBorderGrid.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };
                //ceilingBorderGrid.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 91 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.CheckerboardDimension = 1;
                

                ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 0 }, indexWeights = new List<float> { 1f } };
                */



            }


            /*
            var ouifryeouife = DungeonDatabase.GetOrLoadByName("Base_Cathedral");
            if (ouifryeouife == null) { ETGModConsole.Log("FUCK"); }
            foreach (var pain in ouifryeouife.roomMaterialDefinitions)
            {
                
                pain.roomCeilingBorderGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (roomCeilingBorderGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.roomFloorBorderGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (roomFloorBorderGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.additionalPitBorderFlatGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (additionalPitBorderFlatGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.bridgeGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (bridgeGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.pitBorderFlatGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (pitBorderFlatGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.exteriorFacadeBorderGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (exteriorFacadeBorderGrid)", true);
                Debug.Log("==========================\n\n==========================");
                
                pain.pitLayoutGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (pitLayoutGrid)", true);
                Debug.Log("==========================\n\n==========================");
            }
            ouifryeouife = null;
            */

        }
        public static DungeonMaterial floorMaterial;    
    }
    class DungeonMaterialSetupSecond
    {
        public static void InitCustomDungeonMaterial()
        {
            var past_Marine = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");

            floorMaterial = ScriptableObject.CreateInstance<DungeonMaterial>();
            floorMaterial.supportsPits = true;
            floorMaterial.doPitAO = false;
            floorMaterial.useLighting = true;
            floorMaterial.supportsDiagonalWalls = false;
            floorMaterial.carpetIsMainFloor = false;
            floorMaterial.carpetGrids = new TileIndexGrid[0];
            floorMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();
            floorMaterial.additionalPitBorderFlatGrid = TilesetToolbox.CreateBlankIndexGrid();
            floorMaterial.roomCeilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();


            floorMaterial.fallbackHorizontalTileMapEffects = new VFXComplex[] { (PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects.effects[0] };
            floorMaterial.fallbackVerticalTileMapEffects = new VFXComplex[] { (PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects.effects[0] };


            string[] pathsSmall = new string[]
            {
                "Planetside/Resources/FloorStuff/WallShards/wallshard1_001.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard1_002.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard1_003.png",

            };
            string[] pathsLarge = new string[]
            {
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_001.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_002.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_003.png",
                "Planetside/Resources/FloorStuff/WallShards/wallshard2_004.png",

            };
            floorMaterial.wallShards = past_Marine.roomMaterialDefinitions[0].wallShards;
            floorMaterial.bigWallShardDamageThreshold = 20;
            floorMaterial.bigWallShards = past_Marine.roomMaterialDefinitions[0].bigWallShards;
            floorMaterial.secretRoomWallShardCollections = past_Marine.roomMaterialDefinitions[0].secretRoomWallShardCollections;

            past_Marine = null;


            var ceilingBorderGrid = TilesetToolbox.CreateBlankIndexGrid();

            ceilingBorderGrid.topIndices = new TileIndexList { indices = new List<int> { 52 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.leftIndices = new TileIndexList { indices = new List<int> { 54 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.rightIndices = new TileIndexList { indices = new List<int> { 53 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomIndices = new TileIndexList { indices = new List<int> { 51 }, indexWeights = new List<float> { 1f } };


            ceilingBorderGrid.topLeftNubIndices = new TileIndexList { indices = new List<int> { 57}, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topRightNubIndices = new TileIndexList { indices = new List<int> { 58 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 55 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 56 }, indexWeights = new List<float> { 1f } };
            
            ceilingBorderGrid.bottomRightIndices = new TileIndexList { indices = new List<int> { 61 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.bottomLeftIndices = new TileIndexList { indices = new List<int> { 62 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topLeftIndices = new TileIndexList { indices = new List<int> { 63 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.topRightIndices = new TileIndexList { indices = new List<int> { 64 }, indexWeights = new List<float> { 1f } };

            ceilingBorderGrid.topRightIndices = new TileIndexList { indices = new List<int> { 64 }, indexWeights = new List<float> { 1f } };

            ceilingBorderGrid.rightCapIndices = new TileIndexList { indices = new List<int> { 65 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.leftCapIndices = new TileIndexList { indices = new List<int> { 66 }, indexWeights = new List<float> { 1f } };

            ceilingBorderGrid.horizontalIndices = new TileIndexList { indices = new List<int> { 67 }, indexWeights = new List<float> { 1f } };

            ceilingBorderGrid.borderTopNubRightIndices = new TileIndexList { indices = new List<int> { 68 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.borderTopNubLeftIndices = new TileIndexList { indices = new List<int> { 69 }, indexWeights = new List<float> { 1f } };
            ceilingBorderGrid.CenterIndicesAreStrata = false;
            ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 1 }, indexWeights = new List<float> { 1f } };

            floorMaterial.roomCeilingBorderGrid = ceilingBorderGrid;
            var pitBorderGridCave = TilesetToolbox.CreateBlankIndexGrid();
            pitBorderGridCave.topIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.leftIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.rightIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.bottomIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };


            pitBorderGridCave.topLeftNubIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.topRightNubIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 44, 45, 46, 47 }, indexWeights = new List<float> { 1f, 1, 1, 1 } };
            pitBorderGridCave.CenterIndicesAreStrata = false;
            floorMaterial.roomFloorBorderGrid = pitBorderGridCave;


            /*
            var pitBorderGridCave = TilesetToolbox.CreateBlankIndexGrid();
            pitBorderGridCave.topLeftIndices = new TileIndexList { indices = new List<int> { 1 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topRightIndices = new TileIndexList { indices = new List<int> { 3 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.leftIndices = new TileIndexList { indices = new List<int> { 4 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.rightIndices = new TileIndexList { indices = new List<int> { 5 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomLeftIndices = new TileIndexList { indices = new List<int> { 6 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomIndices = new TileIndexList { indices = new List<int> { 7 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomRightIndices = new TileIndexList { indices = new List<int> { 8 }, indexWeights = new List<float> { 1f } };

            pitBorderGridCave.topRightNubIndices = new TileIndexList { indices = new List<int> { 9 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topLeftNubIndices = new TileIndexList { indices = new List<int> { 10 }, indexWeights = new List<float> { 1f } };

            pitBorderGridCave.horizontalIndices = new TileIndexList { indices = new List<int> { 11 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.verticalIndices = new TileIndexList { indices = new List<int> { 12 }, indexWeights = new List<float> { 1f } };


            pitBorderGridCave.topCapIndices = new TileIndexList { indices = new List<int> { 13 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomCapIndices = new TileIndexList { indices = new List<int> { 14 }, indexWeights = new List<float> { 1f } };
            */

            //pitBorderGridCave.allSidesIndices = new TileIndexList { indices = new List<int> { 10 }, indexWeights = new List<float> { 1f } };

            //
            //pitBorderGridCave.rightCapIndices = new TileIndexList { indices = new List<int> { 81 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.leftCapIndices = new TileIndexList { indices = new List<int> { 83 }, indexWeights = new List<float> { 1f } };



            //pitBorderGridCave.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 89 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.extendedSet = true;
            //pitBorderGridCave.centerIndices = new TileIndexList { indices = new List<int> { 0 }, indexWeights = new List<float> { 1f } };


            //pitBorderGridCave.topCenterLeftIndices = new TileIndexList { indices = new List<int> { 89 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.topCenterIndices = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };


            //pitBorderGridCave.topCenterRightIndices = new TileIndexList { indices = new List<int> { 91 }, indexWeights = new List<float> { 1f } };

            //pitBorderGridCave.thirdTopRowLeftIndices = new TileIndexList { indices = new List<int> { 92 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.thirdTopRowCenterIndices = new TileIndexList { indices = new List<int> { 93 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.thirdTopRowRightIndices = new TileIndexList { indices = new List<int> { 94 }, indexWeights = new List<float> { 1f } };
            //pitBorderGridCave.internalBottomLeftCenterIndices = new TileIndexList { indices = new List<int> { 95 }, indexWeights = new List<float> { 1f } };


            /*
            pitBorderGridCave.internalBottomCenterIndices = new TileIndexList { indices = new List<int> { 101 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.internalBottomRightCenterIndices = new TileIndexList { indices = new List<int> { 102 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubLeftIndices = new TileIndexList { indices = new List<int> { 103 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubRightIndices = new TileIndexList { indices = new List<int> { 104 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 105 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 106 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 107 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 108 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 109 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 110 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 111 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 112 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 113 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 114 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 115 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 116 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsTop = new TileIndexList { indices = new List<int> { 117 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsRight = new TileIndexList { indices = new List<int> { 118 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsBottom = new TileIndexList { indices = new List<int> { 119 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.doubleNubsLeft = new TileIndexList { indices = new List<int> { 120 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.quadNubs = new TileIndexList { indices = new List<int> { 121 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topRightWithNub = new TileIndexList { indices = new List<int> { 122 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.topLeftWithNub = new TileIndexList { indices = new List<int> { 123 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomRightWithNub = new TileIndexList { indices = new List<int> { 124 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 125 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderNE = new TileIndexList { indices = new List<int> { 126 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderSE = new TileIndexList { indices = new List<int> { 127 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderSW = new TileIndexList { indices = new List<int> { 128 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalBorderNW = new TileIndexList { indices = new List<int> { 129 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingNE = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingSE = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingSW = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            pitBorderGridCave.diagonalCeilingNW = new TileIndexList { indices = new List<int> { 130 }, indexWeights = new List<float> { 1f } };
            */
            //pitBorderGridCave.CenterCheckerboard = false;
            //pitBorderGridCave.CheckerboardDimension = 0;

            //pitBorderGridCave.CenterIndicesAreStrata = false;
            //pitBorderGridCave.PitInternalSquareGrids = new List<TileIndexGrid>() { pitBorderGridCave };





            //pitBorderGridCave.PitInternalSquareOptions = new PitSquarePlacementOptions() { CanBeFlushBottom = true, CanBeFlushLeft = true, CanBeFlushRight = true, PitSquareChance = 1 };
            //pitBorderGridCave.PitBorderIsInternal = false;
            //pitBorderGridCave.PitBorderOverridesFloorTile = false;
            //pitBorderGridCave.CeilingBorderUsesDistancedCenters = false;
            //pitBorderGridCave.UsesRatChunkBorders = false;
            //pitBorderGridCave.RatChunkNormalSet = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.RatChunkBottomSet = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.PathFacewallStamp = ob;
            //pitBorderGridCave.PathSidewallStamp =;
            //pitBorderGridCave.PathPitPosts = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.PathPitPostsBL = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //pitBorderGridCave.PathPitPostsBR = new TileIndexList { indices = new List<int> { 38 }, indexWeights = new List<float> { 1f } }; ;
            //floorMaterial.roomFloorBorderGrid = pitBorderGridCave;
            //floorMaterial.outerCeilingBorderGrid = pitBorderGridCave;

            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================

            //To make 2x2 squares
            /*
            var floorSquares = TilesetToolbox.CreateBlankIndexGrid();

            floorSquares.topLeftIndices = new TileIndexList { indices = new List<int> { 25 }, indexWeights = new List<float> { 1f } };
            floorSquares.topRightIndices = new TileIndexList { indices = new List<int> { 26 }, indexWeights = new List<float> { 1f } };
            floorSquares.bottomLeftIndices = new TileIndexList { indices = new List<int> { 27 }, indexWeights = new List<float> { 1f } };
            floorSquares.bottomRightIndices = new TileIndexList { indices = new List<int> { 28 }, indexWeights = new List<float> { 1f } };

            var floorSquares1 = TilesetToolbox.CreateBlankIndexGrid();
            floorSquares1.topLeftIndices = new TileIndexList { indices = new List<int> { 29 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.topRightIndices = new TileIndexList { indices = new List<int> { 30 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.bottomLeftIndices = new TileIndexList { indices = new List<int> { 31 }, indexWeights = new List<float> { 0.5f } };
            floorSquares1.bottomRightIndices = new TileIndexList { indices = new List<int> { 32 }, indexWeights = new List<float> { 0.5f } };

            floorMaterial.floorSquares = new TileIndexGrid[] { floorSquares, floorSquares1 };
            */
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================
            //==============================================================================================================================================================================================
            {





                /*
                ceilingBorderGrid.topLeftIndices = new TileIndexList { indices = new List<int> { 60 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topIndices = new TileIndexList { indices = new List<int> { 61 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topRightIndices = new TileIndexList { indices = new List<int> { 62 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.leftIndices = new TileIndexList { indices = new List<int> { 38, 40 }, indexWeights = new List<float> { 1f, 0.9f } };

                //ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 96 }, indexWeights = new List<float> { 1f } };
                //new TileIndexList { indices = new List<int> { 96, 97, 98, 99 }, indexWeights = new List<float> { 1f, 0.06f, 0.06f, 0.06f } };

                ceilingBorderGrid.rightIndices = new TileIndexList { indices = new List<int> { 39, 41 }, indexWeights = new List<float> { 1f, 0.9f } };


                ceilingBorderGrid.bottomLeftIndices = new TileIndexList { indices = new List<int> { 66 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomIndices = new TileIndexList { indices = new List<int> { 67 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightIndices = new TileIndexList { indices = new List<int> { 68 }, indexWeights = new List<float> { 1f } };

                ceilingBorderGrid.verticalIndices = new TileIndexList { indices = new List<int> { 105 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.horizontalIndices = new TileIndexList { indices = new List<int> { 106 }, indexWeights = new List<float> { 1f } };

                //Caps
                ceilingBorderGrid.topCapIndices = new TileIndexList { indices = new List<int> { 103 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.leftCapIndices = new TileIndexList { indices = new List<int> { 101 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.rightCapIndices = new TileIndexList { indices = new List<int> { 102 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomCapIndices = new TileIndexList { indices = new List<int> { 104 }, indexWeights = new List<float> { 1f } };

                //

                //========================================
                //Nubs
                ceilingBorderGrid.doubleNubsRight = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsLeft = new TileIndexList { indices = new List<int> { 81 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsTop = new TileIndexList { indices = new List<int> { 82 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.doubleNubsBottom = new TileIndexList { indices = new List<int> { 83 }, indexWeights = new List<float> { 1f } };

                ceilingBorderGrid.quadNubs = new TileIndexList { indices = new List<int> { 84 }, indexWeights = new List<float> { 1f } };

                ///Corner Nubs
                ceilingBorderGrid.topRightWithNub = new TileIndexList { indices = new List<int> { 108 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topLeftWithNub = new TileIndexList { indices = new List<int> { 107 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightWithNub = new TileIndexList { indices = new List<int> { 109 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomLeftWithNub = new TileIndexList { indices = new List<int> { 110 }, indexWeights = new List<float> { 1f } };
                //========================================

                ceilingBorderGrid.allSidesIndices = new TileIndexList { indices = new List<int> { 75 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topLeftNubIndices = new TileIndexList { indices = new List<int> { 76 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.topRightNubIndices = new TileIndexList { indices = new List<int> { 77 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomLeftNubIndices = new TileIndexList { indices = new List<int> { 78 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.bottomRightNubIndices = new TileIndexList { indices = new List<int> { 79 }, indexWeights = new List<float> { 1f } };

                ceilingBorderGrid.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 80 }, indexWeights = new List<float> { 1f } };
                //


                ceilingBorderGrid.borderTopNubLeftIndices = new TileIndexList { indices = new List<int> { 117 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderTopNubRightIndices = new TileIndexList { indices = new List<int> { 118 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderTopNubBothIndices = new TileIndexList { indices = new List<int> { 119 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.borderBottomNubLeftIndices = new TileIndexList { indices = new List<int> { 115 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubRightIndices = new TileIndexList { indices = new List<int> { 116 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderBottomNubBothIndices = new TileIndexList { indices = new List<int> { 120 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.borderLeftNubTopIndices = new TileIndexList { indices = new List<int> { 111 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBottomIndices = new TileIndexList { indices = new List<int> { 112 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderLeftNubBothIndices = new TileIndexList { indices = new List<int> { 122 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.borderRightNubTopIndices = new TileIndexList { indices = new List<int> { 113 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubBottomIndices = new TileIndexList { indices = new List<int> { 114 }, indexWeights = new List<float> { 1f } };
                ceilingBorderGrid.borderRightNubBothIndices = new TileIndexList { indices = new List<int> { 121 }, indexWeights = new List<float> { 1f } };

                //ceilingBorderGrid.diagonalNubsTopLeftBottomRight = new TileIndexList { indices = new List<int> { 90 }, indexWeights = new List<float> { 1f } };
                //ceilingBorderGrid.diagonalNubsTopRightBottomLeft = new TileIndexList { indices = new List<int> { 91 }, indexWeights = new List<float> { 1f } };


                ceilingBorderGrid.CheckerboardDimension = 1;
                

                ceilingBorderGrid.centerIndices = new TileIndexList { indices = new List<int> { 0 }, indexWeights = new List<float> { 1f } };
                */



            }


            /*
            var ouifryeouife = DungeonDatabase.GetOrLoadByName("Base_Cathedral");
            if (ouifryeouife == null) { ETGModConsole.Log("FUCK"); }
            foreach (var pain in ouifryeouife.roomMaterialDefinitions)
            {
                
                pain.roomCeilingBorderGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (roomCeilingBorderGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.roomFloorBorderGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (roomFloorBorderGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.additionalPitBorderFlatGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (additionalPitBorderFlatGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.bridgeGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (bridgeGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.pitBorderFlatGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (pitBorderFlatGrid)", true);
                Debug.Log("==========================\n\n==========================");
                pain.exteriorFacadeBorderGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (exteriorFacadeBorderGrid)", true);
                Debug.Log("==========================\n\n==========================");
                
                pain.pitLayoutGrid.LogPropertiesAndFields<TileIndexGrid>(pain.name + " (pitLayoutGrid)", true);
                Debug.Log("==========================\n\n==========================");
            }
            ouifryeouife = null;
            */

        }
        public static DungeonMaterial floorMaterial;
    }
}
