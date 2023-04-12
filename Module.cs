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

namespace ModularMod
{
    [BepInDependency("etgmodding.etg.mtgapi")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Module : BaseUnityPlugin
    {
        public const string GUID = "somebunny.etg.modularcharacter";
        public const string NAME = "Modular Custom Character";
        public const string VERSION = "0.0.0";
        public const string TEXT_COLOR = "#79eaff";

        public static string FilePathFolder;

        public static AssetBundle ModularAssetBundle;
        public static CustomCharacterData Modular_Character_Data;

        public void Start(){ ETGModMainBehaviour.WaitForGameManagerStart(GMStart); }

        public void Awake()
        {
            //Start up SaveAPI
            SaveAPIManager.Setup("mdl");
        }

        public void GMStart(GameManager g)
        {
            //==== Setup important file path stuff ====//
            FilePathFolder = this.FolderPath();
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
            Hooks.Init();
            EnemyDeathUnlockController.Start();
            MultiActiveReloadManager.SetupHooks();
            CustomClipAmmoTypeToolbox.Init();
            

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



           


            //==== Build Custom Character ====//
            ToolsCharApi.EnableDebugLogging = true;
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
                         "",
                         Module.ModularAssetBundle.LoadAsset<Texture2D>("modular_bosscard_001")); //Past ID String
            //Certain aspects of character gets modified here
            Modular_Character_Data = data;
            var doer = data.idleDoer;
            doer.phases = new CharacterSelectIdlePhase[]
            {
                new CharacterSelectIdlePhase() { outAnimation= "error"},
                new CharacterSelectIdlePhase(){ outAnimation = "tummy"}
            };


            PastDungeon.Init();

            this.StartCoroutine(Delayedstarthandler());
            Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);
        }

        public IEnumerator Delayedstarthandler()
        {
            //==== Quick And Easy Commands Setup ====//
            ETGModConsole.Commands.AddGroup("mdl", args =>
            {
            });
            ETGModConsole.Commands.GetGroup("mdl").AddUnit("toggle_test_unlock", ForceEnableMixedFloor);

            ETGModConsole.Commands.GetGroup("mdl").AddUnit("locktoggle", ToggleLocks);


            if (this.OnFrameDelay != null) 
            {
                OnFrameDelay();
            }

            yield return null;
            Module.Modular = ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, "Modular");
            GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST, true);
            GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME, true);
            yield break;
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

        public Action OnFrameDelay;

        public static void Log(string text, string color="FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }



        public static PlayableCharacters Modular;
    }

}
