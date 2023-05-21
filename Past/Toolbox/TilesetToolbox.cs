using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public static class TilesetToolbox
    {

        public static tk2dSpriteCollectionData ReplaceDungeonCollection(tk2dSpriteCollectionData sourceCollection, Texture2D spriteSheet = null, List<string> spriteList = null)
        {
            if (sourceCollection == null) { return null; }
            tk2dSpriteCollectionData collectionData = UnityEngine.Object.Instantiate(sourceCollection);
            tk2dSpriteDefinition[] spriteDefinietions = new tk2dSpriteDefinition[collectionData.spriteDefinitions.Length];
            for (int i = 0; i < collectionData.spriteDefinitions.Length; i++) { spriteDefinietions[i] = collectionData.spriteDefinitions[i].Copy(); }
            collectionData.spriteDefinitions = spriteDefinietions;
            if (spriteSheet != null)
            {
                Material[] materials = sourceCollection.materials;
                Material[] newMaterials = new Material[materials.Length];
                if (materials != null)
                {
                    for (int i = 0; i < materials.Length; i++)
                    {
                        newMaterials[i] = materials[i].Copy(spriteSheet);
                    }
                    collectionData.materials = newMaterials;
                    foreach (Material material2 in collectionData.materials)
                    {
                        foreach (tk2dSpriteDefinition spriteDefinition in collectionData.spriteDefinitions)
                        {
                            bool flag3 = material2 != null && spriteDefinition.material.name.Equals(material2.name);
                            if (flag3)
                            {
                                spriteDefinition.material = material2;
                                spriteDefinition.materialInst = new Material(material2);
                            }
                        }
                    }
                }
            }
            else if (spriteList != null)
            {
                RuntimeAtlasPage runtimeAtlasPage = new RuntimeAtlasPage(0, 0, TextureFormat.RGBA32, 2);
                for (int i = 0; i < spriteList.Count; i++)
                {
                    Texture2D texture2D = ResourceExtractor.GetTextureFromResource(spriteList[i]);
                    if (!texture2D)
                    {
                        Debug.Log("[BuildDungeonCollection] Null Texture found at index: " + i);
                        goto IL_EXIT;
                    }
                    float X = (texture2D.width / 16f);
                    float Y = (texture2D.height / 16f);
                    // tk2dSpriteDefinition spriteData = collectionData.GetSpriteDefinition(i.ToString());
                    tk2dSpriteDefinition spriteData = collectionData.spriteDefinitions[i];
                    if (spriteData != null)
                    {
                        if (spriteData.boundsDataCenter != Vector3.zero)
                        {
                            try
                            {
                                // Debug.Log("[BuildDungeonCollection] Pack Existing Atlas Element at index: " + i);
                                RuntimeAtlasSegment runtimeAtlasSegment = runtimeAtlasPage.Pack(texture2D, false);
                                spriteData.materialInst.mainTexture = runtimeAtlasSegment.texture;
                                spriteData.uvs = runtimeAtlasSegment.uvs;
                                spriteData.extractRegion = true;
                                spriteData.position0 = Vector3.zero;
                                spriteData.position1 = new Vector3(X, 0, 0);
                                spriteData.position2 = new Vector3(0, Y, 0);
                                spriteData.position3 = new Vector3(X, Y, 0);
                                spriteData.boundsDataCenter = new Vector2((X / 2), (Y / 2));
                                spriteData.untrimmedBoundsDataCenter = spriteData.boundsDataCenter;
                                spriteData.boundsDataExtents = new Vector2(X, Y);
                                spriteData.untrimmedBoundsDataExtents = spriteData.boundsDataExtents;
                            }
                            catch (Exception)
                            {
                                Debug.Log("[BuildDungeonCollection] Exception caught at index: " + i);
                            }
                        }
                        else
                        {
                            // Debug.Log("Test 3. Replace Existing Atlas Element at index: " + i);
                            try
                            {
                                ETGMod.ReplaceTexture(spriteData, texture2D, true);
                            }
                            catch (Exception)
                            {
                                Debug.Log("[BuildDungeonCollection] Exception caught at index: " + i);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("[BuildDungeonCollection] SpriteData is null at index: " + i);
                    }
                IL_EXIT:;
                }
                runtimeAtlasPage.Apply();
            }
            else
            {
                Debug.Log("[BuildDungeonCollection] SpriteList is null!");
            }
            return collectionData;
        }

        public static Material Copy(this Material orig, Texture2D textureOverride = null, Shader shaderOverride = null)
        {
            Material m_NewMaterial = new Material(orig.shader)
            {
                name = orig.name,
                shaderKeywords = orig.shaderKeywords,
                globalIlluminationFlags = orig.globalIlluminationFlags,
                enableInstancing = orig.enableInstancing,
                doubleSidedGI = orig.doubleSidedGI,
                mainTextureOffset = orig.mainTextureOffset,
                mainTextureScale = orig.mainTextureScale,
                renderQueue = orig.renderQueue,
                color = orig.color,
                hideFlags = orig.hideFlags
            };
            if (textureOverride != null)
            {
                m_NewMaterial.mainTexture = textureOverride;
            }
            else
            {
                m_NewMaterial.mainTexture = orig.mainTexture;
            }

            if (shaderOverride != null)
            {
                m_NewMaterial.shader = shaderOverride;
            }
            else
            {
                m_NewMaterial.shader = orig.shader;
            }
            return m_NewMaterial;
        }

        public static tk2dSpriteDefinition Copy(this tk2dSpriteDefinition orig)
        {
            tk2dSpriteDefinition m_newSpriteCollection = new tk2dSpriteDefinition();

            m_newSpriteCollection.boundsDataCenter = orig.boundsDataCenter;
            m_newSpriteCollection.boundsDataExtents = orig.boundsDataExtents;
            m_newSpriteCollection.colliderConvex = orig.colliderConvex;
            m_newSpriteCollection.colliderSmoothSphereCollisions = orig.colliderSmoothSphereCollisions;
            m_newSpriteCollection.colliderType = orig.colliderType;
            m_newSpriteCollection.colliderVertices = orig.colliderVertices;
            m_newSpriteCollection.collisionLayer = orig.collisionLayer;
            m_newSpriteCollection.complexGeometry = orig.complexGeometry;
            m_newSpriteCollection.extractRegion = orig.extractRegion;
            m_newSpriteCollection.flipped = orig.flipped;
            m_newSpriteCollection.indices = orig.indices;
            if (orig.material != null) { m_newSpriteCollection.material = new Material(orig.material); }
            m_newSpriteCollection.materialId = orig.materialId;
            if (orig.materialInst != null) { m_newSpriteCollection.materialInst = new Material(orig.materialInst); }
            m_newSpriteCollection.metadata = orig.metadata;
            m_newSpriteCollection.name = orig.name;
            m_newSpriteCollection.normals = orig.normals;
            m_newSpriteCollection.physicsEngine = orig.physicsEngine;
            m_newSpriteCollection.position0 = orig.position0;
            m_newSpriteCollection.position1 = orig.position1;
            m_newSpriteCollection.position2 = orig.position2;
            m_newSpriteCollection.position3 = orig.position3;
            m_newSpriteCollection.regionH = orig.regionH;
            m_newSpriteCollection.regionW = orig.regionW;
            m_newSpriteCollection.regionX = orig.regionX;
            m_newSpriteCollection.regionY = orig.regionY;
            m_newSpriteCollection.tangents = orig.tangents;
            m_newSpriteCollection.texelSize = orig.texelSize;
            m_newSpriteCollection.untrimmedBoundsDataCenter = orig.untrimmedBoundsDataCenter;
            m_newSpriteCollection.untrimmedBoundsDataExtents = orig.untrimmedBoundsDataExtents;
            m_newSpriteCollection.uvs = orig.uvs;

            return m_newSpriteCollection;
        }


        public static void SetupFloorSquare(ref DungeonMaterial material, TileIndexGrid[] tileGrid, float density = 0.2f)
        {
            material.floorSquares = tileGrid;
            material.floorSquareDensity = density;
        }

        public enum TileType
        {
            WallTop,
            WallBottom,
            //WallRightTop,
            //WallLeftTop,
            //WallRightBottom,
            //WallLeftBottom,
        }

        public static void SetWallCollistion(this tk2dSpriteDefinition def, TileType type)
        {

            def.colliderType = tk2dSpriteDefinition.ColliderType.Box;
            switch (type)
            {
                case TileType.WallTop:
                    def.colliderVertices = new Vector3[] { new Vector3(0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.1f) };
                    def.collisionLayer = CollisionLayer.HighObstacle;
                    break;
                case TileType.WallBottom:
                    def.colliderVertices = new Vector3[] { new Vector3(0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.1f) };
                    def.collisionLayer = CollisionLayer.LowObstacle;
                    break;
            }
        }


        public static FacewallIndexGridDefinition CreateBlankFacewallIndexGridDefinitionIndexGrid(TileIndexGrid gridToUse)
        {
            var indexGrid = new FacewallIndexGridDefinition();
            indexGrid.canAcceptFloorDecoration = true;
            indexGrid.canAcceptWallDecoration = true;
            indexGrid.canBePlacedInExits = true;
            indexGrid.canExistInCorners = true;

            indexGrid.forcedStampMatchingStyle = DungeonTileStampData.IntermediaryMatchingStyle.ANY;
            indexGrid.grid = gridToUse;
            indexGrid.hasIntermediaries = false;
            indexGrid.maxIntermediaryBuffer = 1;
            indexGrid.maxIntermediaryLength = 1;

            indexGrid.maxWidth = 100;
            indexGrid.middleSectionSequential = true;
            indexGrid.minIntermediaryBuffer = 100;
            indexGrid.minIntermediaryLength = 100;
            indexGrid.minWidth = 100;
            indexGrid.perTileFailureRate = 0;
            indexGrid.topsMatchBottoms = false;

            return indexGrid;
        }


        public static TileIndexGrid CreateBlankIndexGrid()
        {
            var indexGrid = ScriptableObject.CreateInstance<TileIndexGrid>();
            var yes = new TileIndexList { indexWeights = new List<float> { 0.1f }, indices = new List<int> { -1 } };

            indexGrid.topLeftIndices = yes;
            indexGrid.topIndices = yes;
            indexGrid.topRightIndices = yes;
            indexGrid.leftIndices = yes;
            indexGrid.centerIndices = yes;
            indexGrid.rightIndices = yes;
            indexGrid.bottomLeftIndices = yes;
            indexGrid.bottomIndices = yes;
            indexGrid.bottomRightIndices = yes;
            indexGrid.horizontalIndices = yes;
            indexGrid.verticalIndices = yes;
            indexGrid.topCapIndices = yes;
            indexGrid.rightCapIndices = yes;
            indexGrid.bottomCapIndices = yes;
            indexGrid.leftCapIndices = yes;
            indexGrid.allSidesIndices = yes;
            indexGrid.topLeftNubIndices = yes;
            indexGrid.topRightNubIndices = yes;
            indexGrid.bottomLeftNubIndices = yes;
            indexGrid.bottomRightNubIndices = yes;

            indexGrid.extendedSet = false;

            indexGrid.topCenterLeftIndices = yes;
            indexGrid.topCenterIndices = yes;
            indexGrid.topCenterRightIndices = yes;
            indexGrid.thirdTopRowLeftIndices = yes;
            indexGrid.thirdTopRowCenterIndices = yes;
            indexGrid.thirdTopRowRightIndices = yes;
            indexGrid.internalBottomLeftCenterIndices = yes;
            indexGrid.internalBottomCenterIndices = yes;
            indexGrid.internalBottomRightCenterIndices = yes;

            indexGrid.borderTopNubLeftIndices = yes;
            indexGrid.borderTopNubRightIndices = yes;
            indexGrid.borderTopNubBothIndices = yes;
            indexGrid.borderRightNubTopIndices = yes;
            indexGrid.borderRightNubBottomIndices = yes;
            indexGrid.borderRightNubBothIndices = yes;
            indexGrid.borderBottomNubLeftIndices = yes;
            indexGrid.borderBottomNubRightIndices = yes;
            indexGrid.borderBottomNubBothIndices = yes;
            indexGrid.borderLeftNubTopIndices = yes;
            indexGrid.borderLeftNubBottomIndices = yes;
            indexGrid.borderLeftNubBothIndices = yes;
            indexGrid.diagonalNubsTopLeftBottomRight = yes;
            indexGrid.diagonalNubsTopRightBottomLeft = yes;
            indexGrid.doubleNubsTop = yes;
            indexGrid.doubleNubsRight = yes;
            indexGrid.doubleNubsBottom = yes;
            indexGrid.doubleNubsLeft = yes;
            indexGrid.quadNubs = yes;
            indexGrid.topRightWithNub = yes;
            indexGrid.topLeftWithNub = yes;
            indexGrid.bottomRightWithNub = yes;
            indexGrid.bottomLeftWithNub = yes;

            indexGrid.diagonalBorderNE = yes;
            indexGrid.diagonalBorderSE = yes;
            indexGrid.diagonalBorderSW = yes;
            indexGrid.diagonalBorderNW = yes;
            indexGrid.diagonalCeilingNE = yes;
            indexGrid.diagonalCeilingSE = yes;
            indexGrid.diagonalCeilingSW = yes;
            indexGrid.diagonalCeilingNW = yes;

            indexGrid.CenterCheckerboard = false;
            indexGrid.CheckerboardDimension = 1;
            indexGrid.CenterIndicesAreStrata = false;

            indexGrid.PitInternalSquareGrids = new List<TileIndexGrid>();

            indexGrid.PitInternalSquareOptions = new PitSquarePlacementOptions { CanBeFlushBottom = false, CanBeFlushLeft = false, CanBeFlushRight = false, PitSquareChance = -1 };

            indexGrid.PitBorderIsInternal = false;

            indexGrid.PitBorderOverridesFloorTile = false;

            indexGrid.CeilingBorderUsesDistancedCenters = false;

            indexGrid.UsesRatChunkBorders = false;
            indexGrid.RatChunkNormalSet = yes;
            indexGrid.RatChunkBottomSet = yes;

            indexGrid.PathFacewallStamp = null;
            indexGrid.PathSidewallStamp = null;

            indexGrid.PathPitPosts = yes;
            indexGrid.PathPitPostsBL = yes;
            indexGrid.PathPitPostsBR = yes;

            indexGrid.PathStubNorth = null;
            indexGrid.PathStubEast = null;
            indexGrid.PathStubSouth = null;
            indexGrid.PathStubWest = null;


            return indexGrid;
        }



        public static void SetupTilesetSpriteDef(this tk2dSpriteDefinition def, bool wall = false, bool lower = false)
        {
            try
            {
                def.boundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
                def.boundsDataExtents = new Vector3(1f, 1f, 0f);
                def.untrimmedBoundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
                def.untrimmedBoundsDataExtents = new Vector3(1f, 1f, 0f);
                def.texelSize = new Vector2(0.625f, 0.625f);
                def.position0 = new Vector3(0f, 0f, 0f);
                def.position1 = new Vector3(1f, 0f, 0f);
                def.position2 = new Vector3(0f, 1f, 0f);
                def.position3 = new Vector3(1f, 1f, 0f);
                def.regionH = 16;
                def.regionW = 16;
                if (wall == true)
                {
                    def.colliderType = tk2dSpriteDefinition.ColliderType.Box;
                    def.collisionLayer = (lower ? CollisionLayer.LowObstacle : CollisionLayer.HighObstacle);
                    def.colliderVertices = new Vector3[]
                    {
                    new Vector3(0f, 1f, -1f),
                    new Vector3(0f, 1f, 1f),
                    new Vector3(0f, 0f, -1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, -1f),
                    new Vector3(1f, 0f, 1f),
                    new Vector3(1f, 1f, -1f),
                    new Vector3(1f, 1f, 1f)
                    };
                }
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }

        }
        public static void SetupTilesetSpriteDefForceCollision(this tk2dSpriteDefinition def, Vector3[] vector3s, tk2dSpriteDefinition.ColliderType type = tk2dSpriteDefinition.ColliderType.None, CollisionLayer collisionLayer = CollisionLayer.HighObstacle, bool ForceDefaultvertices = false)
        {
            try
            {
                def.boundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
                def.boundsDataExtents = new Vector3(1f, 1f, 0f);
                def.untrimmedBoundsDataCenter = new Vector3(0.5f, 0.5f, 0f);
                def.untrimmedBoundsDataExtents = new Vector3(1f, 1f, 0f);
                def.texelSize = new Vector2(0.625f, 0.625f);
                def.position0 = new Vector3(0f, 0f, 0f);
                def.position1 = new Vector3(1f, 0f, 0f);
                def.position2 = new Vector3(0f, 1f, 0f);
                def.position3 = new Vector3(1f, 1f, 0f);
                def.regionH = 16;
                def.regionW = 16;
                def.colliderType = type;
                def.collisionLayer = collisionLayer;
                if (ForceDefaultvertices == true)
                {
                    def.colliderVertices = new Vector3[]
                    {
                    new Vector3(0f, 1f, -1f),
                    new Vector3(0f, 1f, 1f),
                    new Vector3(0f, 0f, -1f),
                    new Vector3(0f, 0f, 1f),
                    new Vector3(1f, 0f, -1f),
                    new Vector3(1f, 0f, 1f),
                    new Vector3(1f, 1f, -1f),
                    new Vector3(1f, 1f, 1f)
                    };
                }
                else if (vector3s == null)
                {
                    def.colliderVertices = new Vector3[0];
                }
                else
                {
                    def.colliderVertices = vector3s;
                }
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }

        }
        public static void SetMaterial(this tk2dSpriteCollectionData collection, int spriteId, int matNum, bool textureChange = false)
        {
            if (textureChange == true)
            {
                collection.materials[matNum].mainTexture = collection.spriteDefinitions[spriteId].material.mainTexture;
            }
            collection.spriteDefinitions[spriteId].material = collection.materials[matNum];
            collection.spriteDefinitions[spriteId].materialId = matNum;
        }

        public static void SetupTileMetaData(this TilesetIndexMetadata metadata, TilesetIndexMetadata.TilesetFlagType type, float weight = 1f, int dungeonRoomSubType = 0, int dungeonRoomSubType2 = -1, int dungeonRoomSubType3 = -1, bool animated = false, bool preventStamps = true)
        {
            metadata.type = type;
            metadata.weight = weight;
            metadata.dungeonRoomSubType = dungeonRoomSubType;
            metadata.secondRoomSubType = dungeonRoomSubType2;
            metadata.thirdRoomSubType = dungeonRoomSubType3;
            metadata.usesAnimSequence = animated;
            metadata.usesNeighborDependencies = false;
            metadata.preventWallStamping = preventStamps;
            metadata.usesPerTileVFX = false;
            metadata.tileVFXPlaystyle = TilesetIndexMetadata.VFXPlaystyle.CONTINUOUS;
            metadata.tileVFXChance = 0f;
            metadata.tileVFXPrefab = null;
            metadata.tileVFXOffset = Vector2.zero;
            metadata.tileVFXDelayTime = 1f;
            metadata.tileVFXDelayVariance = 0f;
            metadata.tileVFXAnimFrame = 0;
        }

        public static void LogPropertiesAndFields<T>(this T obj, string header = "", bool debug = false)
        {

            Debug.Log(header);
            Debug.Log("=======================");
            if (obj == null) { Debug.Log("LogPropertiesAndFields: Null object"); return; }
            Type type = obj.GetType();
            Debug.Log($"Type: {type}");
            PropertyInfo[] pinfos = type.GetProperties();
            Debug.Log($"{typeof(T)} Properties: ");
            foreach (var pinfo in pinfos)
            {
                try
                {
                    var value = pinfo.GetValue(obj, null);
                    string valueString = value.ToString();
                    bool isList = obj?.GetType().GetGenericTypeDefinition() == typeof(List<>);
                    if (isList)
                    {
                        var list = value as List<object>;
                        valueString = $"List[{list.Count}]";
                        foreach (var subval in list)
                        {
                            valueString += "\n\t\t" + subval.ToString();
                        }
                    }

                   

                    Debug.Log($"\t{pinfo.Name}: {valueString}");
                }
                catch { }
            }
            Debug.Log($"{typeof(T)} Fields: ");
            FieldInfo[] finfos = type.GetFields();
            foreach (var finfo in finfos)
            {


                //Debug.Log($"\t{finfo.Name}: {finfo.GetValue(obj)}");


                
                var value = finfo.GetValue(obj);
                if (value != null)
                {
                    bool isTileIndex = value.ToString() == "TileIndexList";
                    string valueString = value.ToString();
                    if (isTileIndex)
                    {
                        var list = value as TileIndexList;
                        valueString = $"List[";
                        foreach (var subval in list.indices)
                        {
                            valueString += "\t\t" + subval.ToString();
                        }
                    }
                    Debug.Log($"\t{finfo.Name}: {valueString}"+"]");
                }
                

            }
        }

    }

}
