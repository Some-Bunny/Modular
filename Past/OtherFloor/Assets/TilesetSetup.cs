using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    class TilesetSetupTwo
    {
        public static void InitCustomTileset()
        {

            ExperimentalTilesetCollection = StaticCollections.DoFastSetup(Module.ModularAssetBundle, "PastTileset2Collection", "tileset2 material.mat");//Module.ModularAssetBundle.LoadAsset<GameObject>("ExperimentalCollection").GetComponent<tk2dSpriteCollectionData>();
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

                ExperimentalTilesetCollection.spriteDefinitions[0].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.HighObstacle, true); //CENTER INDICES on wall borders NEEDS TO HAVE COLLISION

                ExperimentalTilesetCollection.spriteDefinitions[20].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FLOOR_TILE, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[21].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_EW, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[22].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.DOOR_FEET_NS, 1f, 2, 0, -1, false, true);

                ExperimentalTilesetCollection.spriteDefinitions[35].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[35].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);

                ExperimentalTilesetCollection.spriteDefinitions[36].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_RIGHTCORNER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[36].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);

                ExperimentalTilesetCollection.spriteDefinitions[37].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER_LEFTCORNER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[37].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box, CollisionLayer.LowObstacle);

                ExperimentalTilesetCollection.spriteDefinitions[38].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[38].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

                ExperimentalTilesetCollection.spriteDefinitions[39].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_RIGHTCORNER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[39].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);

                ExperimentalTilesetCollection.spriteDefinitions[40].metadata.SetupTileMetaData(TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER_LEFTCORNER, 1f, 2, 0, -1, false, true);
                ExperimentalTilesetCollection.spriteDefinitions[40].SetupTilesetSpriteDefForceCollision(new Vector3[] { new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0.1f) }, tk2dSpriteDefinition.ColliderType.Box);
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
