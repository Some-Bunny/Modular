using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    class TilesetSetup
    {
        public static void InitCustomTileset()
        {

            ExperimentalTilesetCollection = StaticCollections.DoFastSetup(Module.ModularAssetBundle, "PastTilesetCollection", "pasttileset material.mat");//Module.ModularAssetBundle.LoadAsset<GameObject>("ExperimentalCollection").GetComponent<tk2dSpriteCollectionData>();
                                                                                                                                                          //ExperimentalTilesetCollection.gameObject.layer = 22;            
            Texture texture = ExperimentalTilesetCollection.materials[0].GetTexture("_MainTex"); //gets the main texture of the pre-built collection material
            texture.filterMode = FilterMode.Point;


            //==== Copies the collection materials of an existing floor
            var orLoadByName = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");

            Material transparentMat = new Material(orLoadByName.tileIndices.dungeonCollection.materials[1]);
            transparentMat.SetTexture("_MainTex", texture);


            Material defaultMat = new Material(orLoadByName.tileIndices.dungeonCollection.materials[0]);
            defaultMat.SetTexture("_MainTex", texture);
            //====

            orLoadByName = null;
            ExperimentalTilesetCollection.materials = new Material[] { defaultMat, transparentMat };
            ExperimentalTilesetCollection.materialInsts = new Material[] { defaultMat, transparentMat };

            ExperimentalTilesetCollection.textures = new Texture[]
            {
                        texture
            };
           

            for (int me = 1; me < ExperimentalTilesetCollection.Count; me++)
            {
                ExperimentalTilesetCollection.SetMaterial(me, 1);
            }
            for (int me = 1; me <  ExperimentalTilesetCollection.Count; me++)
            {
                 ExperimentalTilesetCollection.spriteDefinitions[me].metadata = new TilesetIndexMetadata();
                 ExperimentalTilesetCollection.spriteDefinitions[me].metadata.SetupTileMetaData((TilesetIndexMetadata.TilesetFlagType)0, 0f, 1, 0, -1, false, true);
            }

            // ExperimentalTilesetCollection.SetMaterial(5, 2);
            // ExperimentalTilesetCollection.SetMaterial(6, 2);


            try
            {



                ExperimentalTilesetCollection.spriteDefinitions[34].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[35].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[36].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[37].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[38].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[39].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[40].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[41].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);

                ExperimentalTilesetCollection.SetMaterial(34, 0);
                ExperimentalTilesetCollection.SetMaterial(35, 0);
                ExperimentalTilesetCollection.SetMaterial(36, 0);
                ExperimentalTilesetCollection.SetMaterial(37, 0);
                ExperimentalTilesetCollection.SetMaterial(38, 0);
                ExperimentalTilesetCollection.SetMaterial(39, 0);
                ExperimentalTilesetCollection.SetMaterial(40, 0);
                ExperimentalTilesetCollection.SetMaterial(41, 0);



                ExperimentalTilesetCollection.spriteDefinitions[0].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true); //CENTER INDICES on wall borders NEEDS TO HAVE COLLISION

                // ExperimentalTilesetCollection.spriteDefinitions[17].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.CHEST_HIGH_WALL, 1f, 1, 0, -1, false, true);
                // ExperimentalTilesetCollection.spriteDefinitions[18].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DECAL_TILE, 1f, 1, 0, -1, false, true);



                // ExperimentalTilesetCollection.spriteDefinitions[40].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_TOP_NE, 1f, 1, 0, -1, false, true);
                // ExperimentalTilesetCollection.spriteDefinitions[41].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_TOP_NW, 1f, 1, 0, -1, false, true);
                // ExperimentalTilesetCollection.spriteDefinitions[44].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NE, 1f, 1, 0, -1, false, true);
                // ExperimentalTilesetCollection.spriteDefinitions[45].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DIAGONAL_FACEWALL_UPPER_NW, 1f, 1, 0, -1, false, true);


                //=====TOP WALL
                ExperimentalTilesetCollection.spriteDefinitions[31].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
                ExperimentalTilesetCollection.spriteDefinitions[33].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

                ExperimentalTilesetCollection.spriteDefinitions[31].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 1f, 1, 0, -1, false, true);//DONE
                //ExperimentalTilesetCollection.spriteDefinitions[13].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTCORNER, 1f, 1, 0, -1, false, true);
                //ExperimentalTilesetCollection.spriteDefinitions[13].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTEDGE, 1f, 1, 0, -1, false, true);


                ExperimentalTilesetCollection.spriteDefinitions[32].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

                //ExperimentalTilesetCollection.spriteDefinitions[12].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTCORNER, 1f, 1, 0, -1, false, true);
                //ExperimentalTilesetCollection.spriteDefinitions[12].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTEDGE, 1f, 1, 0, -1, false, true);

                //====
                //ExperimentalTilesetCollection.spriteDefinitions[7].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.CHEST_HIGH_WALL, 1f, 1, 0, -1, false, true);


                //===Bottom Wall
                ExperimentalTilesetCollection.spriteDefinitions[24].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
                ExperimentalTilesetCollection.spriteDefinitions[25].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);
                ExperimentalTilesetCollection.spriteDefinitions[26].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);


                ExperimentalTilesetCollection.spriteDefinitions[24].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 1, 0, -1, false, true);
                //ExperimentalTilesetCollection.spriteDefinitions[5].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER, 1f, 1, 0, -1, false, true);
                //ExperimentalTilesetCollection.spriteDefinitions[6].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTEDGE, 1f, 1, 0, -1, false, true);
                //ExperimentalTilesetCollection.spriteDefinitions[5].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER, 1f, 1, 0, -1, false, true);
                //ExperimentalTilesetCollection.spriteDefinitions[6].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTEDGE, 1f, 1, 0, -1, false, true);
                //====

                //4,5,6


                ExperimentalTilesetCollection.spriteDefinitions[42].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_NS, 1f, 1, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[43].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_EW, 1f, 1, 0, -1, false, true);


                ExperimentalTilesetCollection.spriteDefinitions[20].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[21].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[22].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[23].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 1, 0, -1, false, true);



                //Loading Bay ==================================================

                ExperimentalTilesetCollection.spriteDefinitions[44].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[45].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[46].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[47].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 2, 0, -1, false, true);

                ExperimentalTilesetCollection.spriteDefinitions[59].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_NS, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[60].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_EW, 1f, 2, 0, -1, false, true);

                ExperimentalTilesetCollection.spriteDefinitions[1].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true); //CENTER INDICES on wall borders NEEDS TO HAVE COLLISION

                ExperimentalTilesetCollection.spriteDefinitions[48].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[48].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);

                ExperimentalTilesetCollection.spriteDefinitions[50].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[50].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

                ExperimentalTilesetCollection.spriteDefinitions[53].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[54].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);


                ExperimentalTilesetCollection.spriteDefinitions[54].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[54].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);


                ExperimentalTilesetCollection.spriteDefinitions[61].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[62].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[63].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[64].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[65].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[66].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[67].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[68].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);
                ExperimentalTilesetCollection.spriteDefinitions[69].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true);


                //ExperimentalTilesetCollection.spriteDefinitions[49].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 2, 0, -1, false, true);//DONE

                ExperimentalTilesetCollection.SetMaterial(51, 0);
                ExperimentalTilesetCollection.SetMaterial(52, 0);
                ExperimentalTilesetCollection.SetMaterial(53, 0);
                ExperimentalTilesetCollection.SetMaterial(54, 0);
                ExperimentalTilesetCollection.SetMaterial(55, 0);
                ExperimentalTilesetCollection.SetMaterial(56, 0);
                ExperimentalTilesetCollection.SetMaterial(57, 0);
                ExperimentalTilesetCollection.SetMaterial(58, 0);







                //==============================================================

                //Borders


            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }

        }
        //public static DungeonMaterial abyssMaterial;
        public static tk2dSpriteCollectionData ExperimentalTilesetCollection;
    }
}
