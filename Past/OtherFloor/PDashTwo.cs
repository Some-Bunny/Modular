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
    public class PDashTwo
    {
        public static void Init()
        {
            DungeonMaterialSetupChain.InitCustomDungeonMaterial();
            DungeonStampDataSetupChain.InitCustomDungeonStampData();


            /*
            InitObjects.Init();
            TilesetSetup.InitCustomTileset(); //Init Tileset


            DungeonStampDataSetup.InitCustomDungeonStampData(); //Init Stamp Data

            FloorRoomInitialisation.InitCustomRooms(); //Init Rooms Before Flows
            CustomFlow.GenerateFlows(); //Flows
            */


            TilesetSetupTwo.InitCustomTileset();

            FloorRoomInitialisationChain.InitCustomRooms(); //Init Rooms Before Flows
            SuperFlow.GenerateFlows(); //Flows
            if (Module.Debug_Mode == true)
            {
                ETGModConsole.Commands.GetGroup("pstmdl").AddUnit("2", LoadFloor);
            }

            SmokeParticleSystem = ModularMod.Module.ModularAssetBundle.LoadAsset<GameObject>("Spooky Smoke").GetComponent<ParticleSystem>();
            InitCustomDungeon();
            new Hook(
                typeof(Dungeon).GetMethod("FloorReached", BindingFlags.Instance | BindingFlags.Public),
                typeof(PDashTwo).GetMethod("FloorReachedHook", BindingFlags.Static | BindingFlags.Public)
            );

            new Hook(
                typeof(DungeonFloorMusicController).GetMethod("ResetForNewFloor", BindingFlags.Instance | BindingFlags.Public),
                typeof(PDashTwo).GetMethod("ResetForNewFloorHook", BindingFlags.Static | BindingFlags.Public)
                );
        }

        public static void ResetForNewFloorHook(Action<DungeonFloorMusicController, Dungeon> orig, DungeonFloorMusicController self, Dungeon d)
        {
            self.m_overrideMusic = false;
            self.m_isVictoryState = false;
            self.m_lastMusicChangeTime = -1000f;
            GameManager.Instance.FlushMusicAudio();
            if (!string.IsNullOrEmpty(d.musicEventName))
            {
                self.m_cachedMusicEventCore = d.musicEventName;
            }
            else
            {
                self.m_cachedMusicEventCore = "Play_MUS_Dungeon_Theme_01";
            }
            self.m_coreMusicEventID = AkSoundEngine.PostEvent(self.m_cachedMusicEventCore, GameManager.Instance.gameObject, 33U, new AkCallbackManager.EventCallback(self.OnAkMusicEvent), null);
            Debug.LogWarning(string.Concat(new object[]
            {
            "Posting core music event: ",
            self.m_cachedMusicEventCore,
            " with playing ID: ",
            self.m_coreMusicEventID
            }));
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST && GameManager.Instance.PrimaryPlayer.characterIdentity != PlayableCharacters.Bullet && d.GetComponent<P_2ParticleController>() == null)
            {
                self.m_overrideMusic = true;
                AkSoundEngine.PostEvent("Play_MUS_Ending_State_01", GameManager.Instance.gameObject) ;
            }
            else
            {
                self.SwitchToState(DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO);
            }
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER && GameStatsManager.Instance.AnyPastBeaten())
            {
                AkSoundEngine.PostEvent("Play_MUS_State_Winner", GameManager.Instance.gameObject);
            }
        }

        public static void FloorReachedHook(Action<Dungeon> orig, Dungeon self)
        {
            if (self.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FINALGEON)
            {
                GameManager.Instance.RewardManager.FacelessChancePerFloor = 0;
            }
            orig(self);
        }


        public static ParticleSystem SmokeParticleSystem;

        private static void LoadFloor(string[] obj)
        {
            if (!GameManager.Instance.customFloors.Contains(PastDefinition)) 
            {
                GameManager.Instance.customFloors.Add(PastDefinition);
            }
            GameManager.Instance.LoadCustomLevel(PDashTwo.PastDefinition.dungeonSceneName);
        }
        public static void InitCustomDungeon()
        {
            getOrLoadByName_Hook = new Hook(
                typeof(DungeonDatabase).GetMethod("GetOrLoadByName", BindingFlags.Static | BindingFlags.Public),
                typeof(PDashTwo).GetMethod("GetOrLoadByNameHook", BindingFlags.Static | BindingFlags.Public)
            );
            AssetBundle braveResources = ResourceManager.LoadAssetBundle("brave_resources_001");
            GameManagerObject = braveResources.LoadAsset<GameObject>("_GameManager");

            PastDefinition = new GameLevelDefinition()
            {
                dungeonSceneName = "tt_modular_ultrahard", //this is the name we will use whenever we want to load our dungeons scene
                dungeonPrefabPath = "cringe_modular_past", //this is what we will use when we want to acess our dungeon prefab
                priceMultiplier = 0f, //multiplies how much things cost in the shop
                secretDoorHealthMultiplier = 1, //multiplies how much health secret room doors have, aka how many shots you will need to expose them
                enemyHealthMultiplier = 1f, //multiplies how much health enemies have
                damageCap = 300, // damage cap for regular enemies
                bossDpsCap = 78, // damage cap for bosses
                
                flowEntries = new List<DungeonFlowLevelEntry>(0),
                predefinedSeeds = new List<int>(0)
            };
            // sets the level definition of the GameLevelDefinition in GameManager.Instance.customFloors if it exists
            foreach (GameLevelDefinition levelDefinition in GameManager.Instance.customFloors)
            {
                if (levelDefinition.dungeonSceneName == "tt_modular_ultrahard") { PastDefinition = levelDefinition; }
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
            bool flag = name.ToLower() == "cringe_modular_past";
            Dungeon result;
            if (flag)
            {
                DebugTime.RecordStartTime();
                DebugTime.Log("AssetBundle.LoadAsset<Dungeon>({0})", new object[]
                {
                    name
                });
                result = PDashTwo.AbyssGeon(GetOrLoadByName_Orig("Base_ResourcefulRat"));
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
            var MarinePastPrefab = DungeonDatabase.GetOrLoadByName("Finalscenario_Soldier");
            var RnG = GetOrLoadByName_Orig("Base_Nakatomi");
            //var Hell = GetOrLoadByName_Orig("base_bullethell");


            //dungeon.musicEventName = Hell.musicEventName;
            dungeon.gameObject.name = "Base_Modular_Past_2";
            dungeon.contentSource = ContentSource.CONTENT_UPDATE_03;
            dungeon.DungeonSeed = 0;
            dungeon.DungeonFloorName = "Hegemony Mechanics Shipyard."; // what shows up At the top when floor is loaded
            dungeon.DungeonShortName = "Modulars Past Continued."; // no clue lol, just make it the same
            dungeon.DungeonFloorLevelTextOverride = "Pad-2"; // what shows up below the floorname
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.debugSettings = new DebugDungeonSettings()
            {
                RAPID_DEBUG_DUNGEON_ITERATION_SEEKER = false,
                RAPID_DEBUG_DUNGEON_ITERATION = false,
                RAPID_DEBUG_DUNGEON_COUNT = 5,
                
                GENERATION_VIEWER_MODE = false,
                FULL_MINIMAP_VISIBILITY = false,
                COOP_TEST = false,
                DISABLE_ENEMIES = false,
                DISABLE_LOOPS = false,
                DISABLE_SECRET_ROOM_COVERS = true,
                
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

            dungeon.lockedDoorObjects = MarinePastPrefab.lockedDoorObjects;
            dungeon.phantomBlockerDoorObjects = MarinePastPrefab.lockedDoorObjects;

            dungeon.decoSettings.standardRoomVisualSubtypes = intCollection;
            var deco = dungeon.decoSettings;
            deco.ambientLightColor = new Color(0.1f, 0.05f, 0.05f);
            deco.ambientLightColorTwo = new Color(0.1f, 0.05f, 0.05f);

            deco.generateLights = false;
            // new Color(0.1f, 0.05f, 0.05f);
            //new Color(0.25f, 0.02f, 0.05f)

            dungeon.tileIndices = new TileIndices()
            {
                tilesetId = GlobalDungeonData.ValidTilesets.FINALGEON,//(GlobalDungeonData.ValidTilesets)CustomValidTilesetsClass.CustomValidTilesets.MODULAR_PAST, //sets it to our floors CustomValidTileset
                dungeonCollection = TilesetSetupTwo.ExperimentalTilesetCollection,//TilesetToolbox.ReplaceDungeonCollection(Tileset),
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

            dungeon.roomMaterialDefinitions = new DungeonMaterial[] {
                DungeonMaterialSetupChain.floorMaterial,
                DungeonMaterialSetupChain.floorMaterial,
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
                    SuperFlow.Default_Flow
                },
                mandatoryExtraRooms = new List<ExtraIncludedRoomData>(0),
                optionalExtraRooms = new List<ExtraIncludedRoomData>(0),
                MAX_GENERATION_ATTEMPTS = 5,
                
                DEBUG_RENDER_CANVASES_SEPARATELY = false
            };

            dungeon.damageTypeEffectMatrix = MarinePastPrefab.damageTypeEffectMatrix;
            dungeon.stampData = DungeonStampDataSetupChain.FloorStampData;
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

            dungeon.PlaceDoors = true;

            /*
            foreach (var flows in MarinePastPrefab.PatternSettings.flows)
            {
                foreach (var nodes in flows.AllNodes)
                {
                    //Debug.Log(nodes.overrideExactRoom.name ?? "null");
                    for (int i = 0; i < nodes.overrideExactRoom.placedObjects.Count; i++)
                    {
                        var objects = nodes.overrideExactRoom.placedObjects[i];
                        if (objects.nonenemyBehaviour.gameObject)
                        {
                            //Debug.Log(objects.nonenemyBehaviour.gameObject.name);
                            //Debug.Log(i);
                            //Debug.Log("-");
                            //Debug.Log(objects.nonenemyBehaviour.gameObject.transform.childCount);
                            for (int e = 0; e < objects.nonenemyBehaviour.gameObject.transform.childCount; e++)
                            {
                                //dungeon.doorObjects = new DungeonPlaceable() {variantTiers = new List<DungeonPlaceableVariant>() {new DungeonPlaceableVariant(){a } } objects.nonenemyBehaviour.gameObject.transform.GetChild(e).gameObject;
                                //Debug.Log(child1.name);
                                //Debug.Log(e);
                                //Debug.Log("--");

                            }

                        }
                    }
                    //Debug.Log("====");

                }
            }
            */

            foreach (var tiers in RnG.alternateDoorObjectsNakatomi.variantTiers)
            {
                var sprite2 = tiers?.nonDatabasePlaceable?.GetComponentsInChildren<tk2dBaseSprite>();
                if (sprite2 != null)
                {
                    foreach (var sprite in sprite2)
                    {
                        sprite.usesOverrideMaterial = true;
                        Material mat = new Material(StaticShaders.Default_Shader);
                        sprite.renderer.material = mat;
                    }               
                }
            }


            dungeon.doorObjects = RnG.alternateDoorObjectsNakatomi;
            dungeon.oneWayDoorObjects = MarinePastPrefab.oneWayDoorObjects;
            dungeon.oneWayDoorPressurePlate = MarinePastPrefab.oneWayDoorPressurePlate;
            dungeon.phantomBlockerDoorObjects = MarinePastPrefab.phantomBlockerDoorObjects;
            dungeon.UsesWallWarpWingDoors = false;
            dungeon.gameObject.AddComponent<P_2ParticleController>();
            dungeon.BossMasteryTokenItemId = ModulePrinterCore.ModulePrinterCoreID;
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.CHARACTER_PAST;
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
            dungeon.StripPlayerOnArrival = false;
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
            MinesDungeonPrefab = null;
            MarinePastPrefab = null;
            RnG = null;
            //Hell = null;
            return dungeon;
        }

        public class P_2ParticleController : MonoBehaviour
        {
            private List<RoomHandler> allRooms;
            private ParticleSystem particleObject;

            private IEnumerator Start()
            {
                RecievedDamage = false;
                while (Dungeon.IsGenerating)
                {
                    yield return null;
                }
                allRooms = GameManager.Instance.Dungeon.data.rooms;
                particleObject = UnityEngine.Object.Instantiate(SmokeParticleSystem.gameObject).GetComponent<ParticleSystem>();
                particleObject.transform.parent = GameManager.Instance.Dungeon.transform;

                foreach (var player in GameManager.Instance.AllPlayers)
                {
                    player.WarpToPoint(player.transform.PositionVector2() - new Vector2(2, 10));
                    player.OnReceivedDamage += Player_OnReceivedDamage;
                }
            }

            private void Player_OnReceivedDamage(PlayerController obj)
            {
                foreach (var player in GameManager.Instance.AllPlayers)
                {
                    player.OnReceivedDamage -= Player_OnReceivedDamage;
                }
                RecievedDamage = true;
            }

            public static bool RecievedDamage = true;

            public void Update()
            {
                if (particleObject)
                {
                    foreach (var RoomHandler in allRooms)
                    {
                        if (UnityEngine.Random.value < 0.025f)
                        {
                            IntVector2 cell = RoomHandler.GetRandomAvailableCellDumb();
                            ParticleSystem particleSystem = particleObject;
                            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                            {
                                position = cell.ToCenterVector3(1),
                                randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                            };
                            var emission = particleSystem.emission;
                            emission.enabled = false;
                            particleSystem.gameObject.SetActive(true);
                            particleSystem.Emit(emitParams, 1);
                        }
                    }
                }
                
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




