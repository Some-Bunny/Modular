using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.CharacterAPI;
using System.Collections;
using Planetside;
using Alexandria.ItemAPI;
using JuneLib;
using JuneLib.Items;
using HarmonyLib;
using SaveAPI;
using HutongGames.PlayMaker.Actions;
using SoundAPI;
using ModularMod.Code.Unlocks;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Alexandria.EnemyAPI;
using ModularMod.Code.Hooks;
using Dungeonator;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using ModularMod.Code.Controllers;

namespace ModularMod
{
    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Module : BaseUnityPlugin
    {
        public const string GUID = "somebunny.etg.modularcharacter";
        public const string NAME = "Modular Custom Character";
        public const string VERSION = "1.1.9";
        public const string TEXT_COLOR = "#79eaff";


        public static AssetBundle ModularAssetBundle;

        public static CustomCharacterData Modular_Character_Data;

        public static AdvancedStringDB Strings;

        private static bool SoundTest = false;
        public static bool Debug_Mode = false;
        private static bool DialogueTest = false;

        public void Start(){ ETGModMainBehaviour.WaitForGameManagerStart(GMStart); }

        public void Awake()
        {
            //Start up SaveAPI
            SaveAPIManager.Setup("mdl");
            new Harmony(GUID).PatchAll();
        }

        private void GameManager_Awake(System.Action<GameManager> orig, GameManager self)
        {
            orig(self);
            PastDungeon.InitCustomDungeon();
        }
        public static string FilePathFolder;

        public void GMStart(GameManager g)
        {
            FilePathFolder = this.FolderPath();


            Hook hook = new Hook(typeof(GameManager).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Module).GetMethod("GameManager_Awake", BindingFlags.Instance | BindingFlags.NonPublic), typeof(GameManager));

            Strings = new AdvancedStringDB();


            //==== Setup important file path stuff ====//
            //====//

            //==== Initialise Assetbundle ====/
            Module.ModularAssetBundle = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("modular_bundle");
            StaticCollections.InitialiseCollections();
            //====//


            //==== Setup Static Stuff ====//
            LightningController.Init();
            VFXStorage.AssignVFX();
            StaticTextures.InitTextures();
            StaticShaders.GetDisplacerMat();
            DefaultModule.DoQuickSetup();

            EnemyBuilder.Init();
            BossBuilder.Init();
            ExpandDungeonMusicAPI.InitHooks();

            //============================//

            //==== Initialise SoundAPI and Soundbanks ====//
            SoundManager.Init();
            SoundManager.LoadBankFromModProject("ModularMod/Modular_Soundbank");
            //=============================//

            //==== Initialise Auto-Item Initialiser ====//
            JuneLib.PrefixHandler.AddPrefixForAssembly("mdl");
            ItemTemplateManager.Init();
            //==========================================//

            //Hooks
            DungeonHooks.Init();
            Hooks.Init();
            Actions.Init();
            EnemyDeathUnlockController.Start();
            MultiActiveReloadManager.SetupHooks();
            CustomClipAmmoTypeToolbox.Init();
            BlessedMode_Modifier.Init();
            AdditionalShopItemController.Init();
            StuffedToy.Init();

            SpecialCharactersController.Init();

            //Items
            ModulePrinterCore.Init();

            //Starters
            DefaultArmCannon.Init();
            DefaultArmCannonAlt.Init();
            ModularPeaShooter.Init();
            ModularPeaShooterAlt.Init();
            ScatterBlast.Init();
            ScatterBlastAlt.Init();
            TriBurst.Init();
            TriBurstAlt.Init();
            ChargeBlaster.Init();
            ChargeBlasterAlt.Init();
            PresicionRifle.Init();
            PresicionRifleAlt.Init();
            Suppressor.Init();
            SuppressorAlt.Init();
            TheHammer.Init();
            TheHammerAlt.Init();
            Apollo.Init();
            Artemis.Init();
            LightLance.Init();
            LightLanceAlt.Init();

            GravityPulsar.Init();
            GravityPulsarAlt.Init();

            //Test Items
            Flowder.Init();
            //TestModule.Init();
            //TestModule.Init2();
            //TestModule.Init3();

            //==== UI Stuff ====//
            StarterGunSelectUIController.Init();
            ScrapUIController.Init();
            AdditionalEnergyInitializer.Init();
            UIHooks.Init();
            //==================//

            //==== Set up loot tables ====//
            GlobalModuleStorage.InitialiseLootTables();
            //============================//


            CrateSpawnController.Init();
            ItemSynergyController.Init();

            //Enemies
            LaserDiode.BuildPrefab();
            RobotMiniMecha.BuildPrefab();
            Sentry.BuildPrefab();
            Node.BuildPrefab();
            Burster.BuildPrefab(true);
            Burster.BuildPrefab(false);
            BigDrone.BuildPrefab();

            Nopticon.BuildNode();
            Nopticon.BuildPrefab(4);
            Nopticon.BuildPrefab(8);
            Nopticon.BuildPrefab(12);
            Nopticon.BuildPrefab(16);

            EnergyShield.BuildPrefab(LaserDiode.guid);
            EnergyShield.BuildPrefab(Sentry.guid);
            EnergyShield.BuildPrefab(RobotMiniMecha.guid);

            EnergyShield.BuildPrefab(Burster.guid + "_Vertical");
            EnergyShield.BuildPrefab(Burster.guid + "_Horizontal");

            Slapper.BuildPrefab();

            SteelPanopticon.BuildPrefab();
            ModularPrime.BuildPrefab();

            //==== Build Custom Character ====//
            ToolsCharApi.EnableDebugLogging = false;
            var data = Loader.BuildCharacterBundle(
                         "ModularMod/Sprites/Modular",
                         StaticCollections.Modular_Character_Collection,
                         Module.ModularAssetBundle.LoadAsset<GameObject>("ModularSpriteAnimation").GetComponent<tk2dSpriteAnimation>(),
                         StaticCollections.Modular_Character_Alt_Collection,
                         Module.ModularAssetBundle.LoadAsset<GameObject>("Modular_Alt_Animation").GetComponent<tk2dSpriteAnimation>(),
                         "somebunny.etg.modularcharacter",
                         new Vector3(30.125f, 29.5f),
                         true,
                         new Vector3(30.625f, 28.5f),
                         false,
                         false,
                         true,
                         true, //Sprites used by paradox
                         true, //Glows
                         new GlowMatDoer(new Color32(121, 234, 255, 255), 14, 10), //Glow Mat
                         new GlowMatDoer(new Color32(0, 255, 54, 255), 5, 3), //Alt Skin Glow Mat
                         0, //Hegemony Cost
                         true, //HasPast
                         "tt_modular_past",
                         Module.ModularAssetBundle.LoadAsset<Texture2D>("modular_bosscard_001")); //Past ID String
            //Certain aspects of character gets modified here
            Modular_Character_Data = data;
            Modular_Character_Data.pastWinPic = Module.ModularAssetBundle.LoadAsset<Texture2D>("win_pic_001");
            
            var doer = data.idleDoer;
            doer.phases = new CharacterSelectIdlePhase[]
            {
                new CharacterSelectIdlePhase() { outAnimation= "error"},
                new CharacterSelectIdlePhase(){ outAnimation = "tummy"}
            };
            foreach (var entry in data.punchoutSprites)
            {
                Debug.Log(entry.Value.name);
            }

            PastDungeon.Init();
            PDashTwo.Init();

            this.StartCoroutine(Delayedstarthandler());

            ConsoleMagic.LogButCool($"{NAME} v{VERSION} started successfully.", Module.ModularAssetBundle.LoadAsset<Texture2D>("modular_Tex_Icon"));
            //Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);

            if (SoundTest == true)
            {
                new Hook(typeof(AkSoundEngine).GetMethods().Single((MethodInfo m) => m.Name == "PostEvent" && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType == typeof(string)), typeof(Module).GetMethod("PostEventHook", BindingFlags.Static | BindingFlags.Public));
            }
            if (DialogueTest == true)
            {
                new Hook(typeof(TextBoxManager).GetMethod("SetText", BindingFlags.Instance | BindingFlags.Public), typeof(Module).GetMethod("SetTextHook"));
            }
        }

        public static void SetTextHook(Action<TextBoxManager, string, Vector3, bool, TextBoxManager.BoxSlideOrientation, bool , bool , bool > orig, TextBoxManager self, string text, Vector3 worldPosition, bool instant = true, TextBoxManager.BoxSlideOrientation slideOrientation = TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, bool showContinueText = true, bool UseAlienLanguage = false, bool clampThoughtBubble = false)
        {
            Debug.Log(text);
            orig(self, text, worldPosition, instant, slideOrientation, showContinueText, UseAlienLanguage, clampThoughtBubble);
        }
        public static uint PostEventHook(System.Func<string, GameObject, uint> orig, string name, GameObject obj)
        {
            if (name != null)
            {
                ETGModConsole.Log(name, true);
                Debug.Log(name);

            }
            return orig(name, obj);
        }
        public IEnumerator Delayedstarthandler()
        {
            //==== Quick And Easy Commands Setup ====//
            ETGModConsole.Commands.AddGroup("mdl", args =>
            {
            });
            //ETGModConsole.Commands.GetGroup("mdl").AddUnit("toggle_test_unlock", ForceEnableMixedFloor);

            //ETGModConsole.Commands.GetGroup("mdl").AddUnit("locktoggle", ToggleLocks);

            if (Debug_Mode == true)
            {
                ETGModConsole.Commands.GetGroup("mdl").AddUnit("cratetoggle", Crate);


                ETGModConsole.Commands.GetGroup("mdl").AddUnit("lock_all", Lock);
                ETGModConsole.Commands.GetGroup("mdl").AddUnit("unlock_all", Unlock);
            }


            if (this.OnFrameDelay != null) 
            {
                OnFrameDelay();
            }

            yield return null;
            Module.Modular = ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, "Modular");
            GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.NONE, true);

            bool Shit = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.PAST);
            bool GodDamnit = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.PAST_ALT_SKIN);
            GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST, Shit);
            GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME, GodDamnit);

            //Debug.Log("Past Kill: " + GameStatsManager.Instance.GetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST));
            //Debug.Log("Skin Past Kill: " + GameStatsManager.Instance.GetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME));

            //GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST, !Test_Copy);
            //GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME, !Test_Copy);

            yield break;
        }

        public static void Crate(string[] s)
        {
            SaveAPIManager.SetFlag(CustomDungeonFlags.CRATE_DROP, !SaveAPIManager.GetFlag(CustomDungeonFlags.CRATE_DROP));
            ETGModConsole.Log("Crate is now set to : " + SaveAPIManager.GetFlag(CustomDungeonFlags.CRATE_DROP));
        }

        public static void ForceEnableMixedFloor(string[] s)
        {
            SaveAPIManager.SetFlag(CustomDungeonFlags.TEST_UNLOCK, !SaveAPIManager.GetFlag(CustomDungeonFlags.TEST_UNLOCK));
            ETGModConsole.Log("Unlock is now set to : " + SaveAPIManager.GetFlag(CustomDungeonFlags.TEST_UNLOCK));
        }



        public static void ToggleLocks(string[] s)
        {
            SaveAPIManager.SetFlag(CustomDungeonFlags.TEST_UNLOCK, !SaveAPIManager.GetFlag(CustomDungeonFlags.TEST_UNLOCK));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_FLOOR_3, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_FLOOR_3));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_LICH_WITH_4_MODULES_OR_LESS, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_LICH_WITH_4_MODULES_OR_LESS));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_RAT_AS_MODULAR, !SaveAPIManager.GetFlag(CustomDungeonFlags.BEAT_RAT_AS_MODULAR));
            SaveAPIManager.SetFlag(CustomDungeonFlags.BOSS_RUSH_AS_MODULAR, !SaveAPIManager.GetFlag(CustomDungeonFlags.BOSS_RUSH_AS_MODULAR));
            SaveAPIManager.SetFlag(CustomDungeonFlags.LEAD_GOD_AS_MODULAR, !SaveAPIManager.GetFlag(CustomDungeonFlags.LEAD_GOD_AS_MODULAR));
            SaveAPIManager.SetFlag(CustomDungeonFlags.FIRST_FLOOR_NO_MODULES, !SaveAPIManager.GetFlag(CustomDungeonFlags.FIRST_FLOOR_NO_MODULES));

            ETGModConsole.Log("Unlocks are now set to : " + SaveAPIManager.GetFlag(CustomDungeonFlags.TEST_UNLOCK));
        }


        public static void Unlock(string[] s)
        {
            bool b = true;
            SaveAPIManager.SetFlag(CustomDungeonFlags.TEST_UNLOCK, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_FLOOR_3, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_LICH_WITH_4_MODULES_OR_LESS, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_RAT_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BOSS_RUSH_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.LEAD_GOD_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.FIRST_FLOOR_NO_MODULES, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.PAST, b);

            ETGModConsole.Log("Unlocks are now set to : " +b);
        }

        public static void Lock(string[] s)
        {
            bool b = false;
            SaveAPIManager.SetFlag(CustomDungeonFlags.TEST_UNLOCK, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_FLOOR_3, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_LICH_WITH_4_MODULES_OR_LESS, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BEAT_RAT_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.BOSS_RUSH_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.LEAD_GOD_AS_MODULAR, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.FIRST_FLOOR_NO_MODULES, b);
            SaveAPIManager.SetFlag(CustomDungeonFlags.PAST, b);

            ETGModConsole.Log("Unlocks are now set to : " + b);
        }

        public Action OnFrameDelay;

        public static void Log(string text, string color="FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }



        public static PlayableCharacters Modular;
    }

}
