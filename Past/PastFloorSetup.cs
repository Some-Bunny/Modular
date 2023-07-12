using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using Alexandria.DungeonAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using ModularMod.Past.Prefabs.Objects;
using System.Collections;

namespace ModularMod
{
    public class PastDungeon
    {
        public static void Init()
        {
            InitObjects.Init();
            TilesetSetup.InitCustomTileset(); //Init Tileset
            DungeonMaterialSetup.InitCustomDungeonMaterial(); //Init Dungeon Material
            DungeonMaterialSetupSecond.InitCustomDungeonMaterial();

            DungeonStampDataSetup.InitCustomDungeonStampData(); //Init Stamp Data

            FloorRoomInitialisation.InitCustomRooms(); //Init Rooms Before Flows
            CustomFlow.GenerateFlows(); //Flows


            ETGModConsole.Commands.AddGroup("pstmdl", args =>
            {
            });
            if (Module.Debug_Mode == true)
            {
                ETGModConsole.Commands.GetGroup("pstmdl").AddUnit("load", LoadFloor);
            }
            InitCustomDungeon();

           
        }
        private static void LoadFloor(string[] obj)
        {
            if (!GameManager.Instance.customFloors.Contains(PastDefinition))
            {
                GameManager.Instance.customFloors.Add(PastDefinition);
            }
            GameManager.Instance.LoadCustomLevel(PastDungeon.PastDefinition.dungeonSceneName);
        }
        public static void InitCustomDungeon()
        {
            getOrLoadByName_Hook = new Hook(
                typeof(DungeonDatabase).GetMethod("GetOrLoadByName", BindingFlags.Static | BindingFlags.Public),
                typeof(PastDungeon).GetMethod("GetOrLoadByNameHook", BindingFlags.Static | BindingFlags.Public)
            );
            AssetBundle braveResources = ResourceManager.LoadAssetBundle("brave_resources_001");
            GameManagerObject = braveResources.LoadAsset<GameObject>("_GameManager");

            PastDefinition = new GameLevelDefinition()
            {
                dungeonSceneName = "tt_modular_past", //this is the name we will use whenever we want to load our dungeons scene
                dungeonPrefabPath = "based_modular_past", //this is what we will use when we want to acess our dungeon prefab
                priceMultiplier = 1.5f, //multiplies how much things cost in the shop
                secretDoorHealthMultiplier = 1, //multiplies how much health secret room doors have, aka how many shots you will need to expose them
                enemyHealthMultiplier = 1.6f, //multiplies how much health enemies have
                damageCap = 300, // damage cap for regular enemies
                bossDpsCap = 78, // damage cap for bosses
                flowEntries = new List<DungeonFlowLevelEntry>(0),
                predefinedSeeds = new List<int>(0)
            };
            // sets the level definition of the GameLevelDefinition in GameManager.Instance.customFloors if it exists
            foreach (GameLevelDefinition levelDefinition in GameManager.Instance.customFloors)
            {
                if (levelDefinition.dungeonSceneName == "tt_modular_past") { PastDefinition = levelDefinition; }
            }

            GameManager.Instance.customFloors.Add(PastDefinition);
            GameManagerObject.GetComponent<GameManager>().customFloors.Add(PastDefinition);
        }


        public static GameLevelDefinition PastDefinition;
        public static GameObject GameManagerObject;
        public static tk2dSpriteCollectionData Tileset;



        public static Dungeon GetOrLoadByNameHook(Func<string, Dungeon> orig, string name)
        {
            if (!GameManager.Instance.customFloors.Contains(PastDefinition))
            {
                GameManager.Instance.customFloors.Add(PastDefinition);
            }
            bool flag = name.ToLower() == "based_modular_past";
            Dungeon result;
            if (flag)
            {
                DebugTime.RecordStartTime();
                DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[]
                {
                    name
                });
                result = PastDungeon.AbyssGeon(GetOrLoadByName_Orig("Base_ResourcefulRat"));
            }
            else
            {
                result = orig(name);
            }

            return result;
            /*
            if (name.ToLower() == "tt_modular_past")//THIS STRING MUST BE THE NAME OF YOUR FLOORS DUNGEON PREFAB PATH
            { 
                return PastDungeon.AbyssGeon(GetOrLoadByName_Orig("Base_ResourcefulRat"));
            }
            else
            {
                return orig(name);
            }
            */
        }

        public static Hook getOrLoadByName_Hook;
       
        public static Dungeon AbyssGeon(Dungeon dungeon)
        {
            var MinesDungeonPrefab = GetOrLoadByName_Orig("Base_Mines");
            //var CatacombsPrefab = GetOrLoadByName_Orig("Base_Catacombs");
            //var RatDungeonPrefab = GetOrLoadByName_Orig("Base_ResourcefulRat");
            var MarinePastPrefab = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");

            dungeon.gameObject.name = "Base_Modular_Past";
            dungeon.contentSource = ContentSource.CONTENT_UPDATE_03;
            dungeon.DungeonSeed = 0;
            dungeon.DungeonFloorName = "Hegemony Mechanics Loading Bay."; // what shows up At the top when floor is loaded
            dungeon.DungeonShortName = "Modulars Past."; // no clue lol, just make it the same
            dungeon.DungeonFloorLevelTextOverride = "17 Years Prior..."; // what shows up below the floorname
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.debugSettings = new DebugDungeonSettings()
            {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 5,
                
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = true,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = true,
                DISABLE_SECRET_ROOM_COVERS = false,
                DISABLE_OUTLINES = false,
                WALLS_ARE_PITS = false
            };
            dungeon.ForceRegenerationOfCharacters = false;
            dungeon.ActuallyGenerateTilemap = true;

            WeightedInt weightedInt = new WeightedInt();
            weightedInt.value = 1;
            weightedInt.weight = 1;
            weightedInt.additionalPrerequisites = new DungeonPrerequisite[0];
            weightedInt.annotation = "why";
            WeightedIntCollection intCollection = new WeightedIntCollection();
            intCollection.elements = new WeightedInt[] { weightedInt };
            dungeon.decoSettings.standardRoomVisualSubtypes = intCollection;
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.CHARACTER_PAST;

            var deco = dungeon.decoSettings;
            
            deco.ambientLightColor = new Color(0.05f, 0.05f, 0.05f);
            deco.ambientLightColorTwo = new Color(0.05f, 0.05f, 0.05f);

            deco.generateLights = false;



            dungeon.tileIndices = new TileIndices()
            {
                tilesetId = (GlobalDungeonData.ValidTilesets)CustomValidTilesetsClass.CustomValidTilesets.MODULAR_PAST, //sets it to our floors CustomValidTileset
                dungeonCollection = TilesetSetup.ExperimentalTilesetCollection,//TilesetToolbox.ReplaceDungeonCollection(Tileset),
                dungeonCollectionSupportsDiagonalWalls = false,
                aoTileIndices = new AOTileIndices() { },//RatDungeonPrefab.tileIndices.aoTileIndices,
                placeBorders = true,
                placePits = true,

                chestHighWallIndices = new List<TileIndexVariant>() {
                    new TileIndexVariant() {
                        index = 41,
                        likelihood = 0.5f,
                        overrideLayerIndex = 0,
                        overrideIndex = 0
                    }
                },
                decalIndexGrid = null,
                //patternIndexGrid = TilesetToolbox.CreateBlankIndexGrid(),//pitBorderGridCave,//RatDungeonPrefab.tileIndices.patternIndexGrid,
                //globalSecondBorderTiles = new List<int>(99),
                edgeDecorationTiles = null,
                     
            };
            dungeon.tileIndices.dungeonCollection.name = "ENV_AbyssFloor_Collection";

            dungeon.roomMaterialDefinitions = new DungeonMaterial[] {
                DungeonMaterialSetup.floorMaterial,
                DungeonMaterialSetup.floorMaterial,
                DungeonMaterialSetupSecond.floorMaterial
            };
            
            dungeon.dungeonWingDefinitions = new DungeonWingDefinition[0];

            //This section can be used to take parts from other floors and use them as our own.
            //we can make the running dust from one floor our own, the tables from another our own, 
            //we can use all of the stuff from the same floor, or if you want, you can make your own.
            dungeon.pathGridDefinitions = new List<TileIndexGrid>() { MinesDungeonPrefab.pathGridDefinitions[0] };



            dungeon.dungeonDustups = new DustUpVFX()
            {
                runDustup = MinesDungeonPrefab.dungeonDustups.runDustup,
                waterDustup = MinesDungeonPrefab.dungeonDustups.waterDustup,
                additionalWaterDustup = MinesDungeonPrefab.dungeonDustups.additionalWaterDustup,
                rollNorthDustup = MinesDungeonPrefab.dungeonDustups.rollNorthDustup,
                rollNorthEastDustup = MinesDungeonPrefab.dungeonDustups.rollNorthEastDustup,
                rollEastDustup = MinesDungeonPrefab.dungeonDustups.rollEastDustup,
                rollSouthEastDustup = MinesDungeonPrefab.dungeonDustups.rollSouthEastDustup,
                rollSouthDustup = MinesDungeonPrefab.dungeonDustups.rollSouthDustup,
                rollSouthWestDustup = MinesDungeonPrefab.dungeonDustups.rollSouthWestDustup,
                rollWestDustup = MinesDungeonPrefab.dungeonDustups.rollWestDustup,
                rollNorthWestDustup = MinesDungeonPrefab.dungeonDustups.rollNorthWestDustup,
                rollLandDustup = MinesDungeonPrefab.dungeonDustups.rollLandDustup
            };
            dungeon.PatternSettings = new SemioticDungeonGenSettings()
            {
                flows = new List<DungeonFlow>()
                {
                    CustomFlow.Default_Flow
                },
                mandatoryExtraRooms = new List<ExtraIncludedRoomData>(0),
                optionalExtraRooms = new List<ExtraIncludedRoomData>(0),
                MAX_GENERATION_ATTEMPTS = 5,
                
                DEBUG_RENDER_CANVASES_SEPARATELY = false
            };

            dungeon.damageTypeEffectMatrix = MinesDungeonPrefab.damageTypeEffectMatrix;
            dungeon.stampData = DungeonStampDataSetup.FloorStampData;
            dungeon.UsesCustomFloorIdea = false;
            dungeon.FloorIdea = new RobotDaveIdea()
            {
                ValidEasyEnemyPlaceables = new DungeonPlaceable[0],
                ValidHardEnemyPlaceables = new DungeonPlaceable[0],
                UseWallSawblades = false,
                UseRollingLogsVertical = true,
                UseRollingLogsHorizontal = true,
                UseFloorPitTraps = false,
                UseFloorFlameTraps = true,
                UseFloorSpikeTraps = true,
                UseFloorConveyorBelts = true,
                UseCaveIns = true,
                UseAlarmMushrooms = false,
                UseChandeliers = true,
                UseMineCarts = false,
                CanIncludePits = false
            };
            dungeon.StripPlayerOnArrival = true;
            dungeon.musicEventName = MarinePastPrefab.musicEventName;
            /*
            //more variable we can copy from other floors, or make our own
            dungeon.PlaceDoors = false;
            dungeon.doorObjects = AbyssFloorDoor.AbyssDoor;
            dungeon.oneWayDoorObjects = MinesDungeonPrefab.oneWayDoorObjects;
            dungeon.oneWayDoorPressurePlate = MinesDungeonPrefab.oneWayDoorPressurePlate;
            dungeon.phantomBlockerDoorObjects = MinesDungeonPrefab.phantomBlockerDoorObjects;
            dungeon.UsesWallWarpWingDoors = false;
            
            //(PickupObjectDatabase.GetById(DiamondChamber.DiamondChamberID) as PickupObject,


            GenericLootTable table = LootTableTools.CreateLootTable();
            table.AddItemsToPool(new Dictionary<int, float>() { { DiamondChamber.DiamondChamberID, 1 } });
            dungeon.baseChestContents = table;
            dungeon.baseChestContents.defaultItemDrops = new WeightedGameObjectCollection() { elements = new List<WeightedGameObject>(0)};

            dungeon.SecretRoomSimpleTriggersFacewall = new List<GameObject>() { CatacombsPrefab.SecretRoomSimpleTriggersFacewall[0] };
            dungeon.SecretRoomSimpleTriggersSidewall = new List<GameObject>() { CatacombsPrefab.SecretRoomSimpleTriggersSidewall[0] };
            dungeon.SecretRoomComplexTriggers = new List<ComplexSecretRoomTrigger>(0);
            dungeon.SecretRoomDoorSparkVFX = CatacombsPrefab.SecretRoomDoorSparkVFX;
            dungeon.SecretRoomHorizontalPoofVFX = CatacombsPrefab.SecretRoomHorizontalPoofVFX;
            dungeon.SecretRoomVerticalPoofVFX = CatacombsPrefab.SecretRoomVerticalPoofVFX;
            dungeon.sharedSettingsPrefab = CatacombsPrefab.sharedSettingsPrefab;
            dungeon.NormalRatGUID = string.Empty;
            dungeon.BossMasteryTokenItemId = NetheriteChamber.ChaamberID;
            dungeon.UsesOverrideTertiaryBossSets = false;
            dungeon.OverrideTertiaryRewardSets = new List<TertiaryBossRewardSet>(0);
            dungeon.defaultPlayerPrefab = MinesDungeonPrefab.defaultPlayerPrefab;
            dungeon.SuppressEmergencyCrates = false;
            dungeon.SetTutorialFlag = false;
            dungeon.PlayerIsLight = true;
            dungeon.PlayerLightColor = Color.cyan;
            dungeon.PlayerLightIntensity = 2;
            dungeon.PlayerLightRadius = 3;
            dungeon.PrefabsToAutoSpawn = new GameObject[0];

            //include this for custom floor audio
            dungeon.musicEventName = "Play_PSOG_MUS_DEEP_01";
            
            */

            //CatacombsPrefab = null;
            //RatDungeonPrefab = null;
            MinesDungeonPrefab = null;
            MarinePastPrefab = null;

            dungeon.gameObject.AddComponent<PastFloorOneController>();

            return dungeon;
        }

        private class PastFloorOneController : MonoBehaviour
        {

            public IEnumerator Start()
            {
                while (Dungeon.IsGenerating)
                {
                    yield return null;
                }
                foreach (var player in GameManager.Instance.AllPlayers)
                {
                    if (player.characterIdentity == ModularMod.Module.Modular)
                    {
                        try
                        {
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(ModulePrinterCore.ModulePrinterCoreID).gameObject, Vector3.zero, Quaternion.identity);
                            PickupObject component3 = gameObject.GetComponent<PickupObject>();
                            if (component3 != null)
                            {
                                component3.CanBeDropped = false;
                                component3.Pickup(player);
                            }
                            player.inventory.AddGunToInventory(PickupObjectDatabase.GetById(DefaultArmCannon.ID) as Gun, true);
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }
                    else
                    {
                        var g = player.inventory.AddGunToInventory(PickupObjectDatabase.GetById(56) as Gun, true);
                        g.InfiniteAmmo = true;
                        g.CanBeDropped = false;
                    }
                }
                Pixelator.Instance.FadeToBlack(0.25f, true, 0.05f);
                Pixelator.Instance.TriggerPastFadeIn();
                yield return new WaitForSeconds(0.4f);
                Pixelator.Instance.SetOcclusionDirty();

                GameManager.Instance.PrimaryPlayer.SetInputOverride("past");
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    GameManager.Instance.SecondaryPlayer.SetInputOverride("past");
                }
                yield return new WaitForSeconds(2f);

                TextBoxManager.ShowTextBox(GameManager.Instance.PrimaryPlayer.transform.position + new Vector3(1.25f, 2.5f, 0f), GameManager.Instance.PrimaryPlayer.transform, 4f, GameManager.Instance.PrimaryPlayer.IsUsingAlternateCostume == true ? "No time to waste here.\n I remember this as clear as day.": "Here it is.\nBack here again.", "golem", false, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, true, false);
                yield return new WaitForSeconds(1f);
                GameManager.Instance.PrimaryPlayer.ClearInputOverride("past");
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    GameManager.Instance.SecondaryPlayer.ClearInputOverride("past");
                }


                yield break;
            }
        }
        public static Dungeon GetOrLoadByName_Orig(string name)
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("dungeons/" + name.ToLower());
            DebugTime.RecordStartTime();
            Dungeon component = assetBundle.LoadAsset<GameObject>(name).GetComponent<Dungeon>();
            DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[]
            {
                name
            });
            return component;
        }
    }
}




