using MonoMod.RuntimeDetour;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Code.Unlocks
{
    public class EnemyDeathUnlockController
    {
        public static void Start()
        {
            new Hook(typeof(HealthHaver).GetMethod("Die"), typeof(EnemyDeathUnlockController).GetMethod("OnHealthHaverDie"));
        }

        public static void OnHealthHaverDie(Action<HealthHaver, Vector2> orig, HealthHaver self, Vector2 finalDamageDir)
        {
            orig(self, finalDamageDir);
            if (self.aiActor != null)
            {
                UnlockChecklist(self.aiActor, self.aiActor.IsBlackPhantom, self.aiActor.EnemyGuid);
            }
        }

        public static void UnlockChecklist(AIActor enemy, bool IsJammed, string GUID)
        {
            if (enemy != null)
            {
                foreach (PlayerController player in GameManager.Instance.AllPlayers)
                {
                    if (player.PlayerHasCore() == true)
                    {
                        //OLD KING
                        if (StaticGUIDs.Old_King_GUID == GUID)
                        {
                            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR) == false)
                            {
                                Toolbox.NotifyCustom("You Unlocked:", "Apollo", StaticCollections.Gun_Collection.GetSpriteIdByName("apollo_idle_001"), StaticCollections.Gun_Collection);
                            }

                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR, true);
                        }
                        //RAT
                        if (StaticGUIDs.Resourceful_Rat_Mech_Boss_GUID == GUID)
                        {
                            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR) == false)
                            {
                                Toolbox.NotifyCustom("You Unlocked:", "Singularity Pulsar", StaticCollections.Gun_Collection.GetSpriteIdByName("gravgun_idle_001"), StaticCollections.Gun_Collection);
                            }
                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_RAT_AS_MODULAR, true);
                        }


                        //LICH
                        if (GUID == StaticGUIDs.Lich_Phase_3_GUID)
                        {
                            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR) == false)
                            {
                                Toolbox.NotifyCustom("You Unlocked:", "2 New Weapons!", StaticCollections.Gun_Collection.GetSpriteIdByName("thegreater"), StaticCollections.Gun_Collection);
                            }
                            GameStatsManager.Instance.SetCharacterSpecificFlag(ETGModCompatibility.ExtendEnum<PlayableCharacters>(Module.GUID, Module.Modular_Character_Data.nameShort), CharacterSpecificGungeonFlags.CLEARED_BULLET_HELL, true);
                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR, true);
                            if (player.PlayerHasCore().ReturnActiveTotal() <= 4)
                            {
                                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LICH_WITH_4_MODULES_OR_LESS, true);
                            }
                            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                            {
                                PlayerController playerController = GameManager.Instance.AllPlayers[i];
                                if (playerController)
                                {
                                    if (playerController.CharacterUsesRandomGuns)
                                    {
                                        if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BLESSED_MODE) == false)
                                        {
                                            Toolbox.NotifyCustom("You Unlocked:", "Barrier Builder", StaticCollections.Gun_Collection.GetSpriteIdByName("shieldgen_idle_004"), StaticCollections.Gun_Collection);
                                        }
                                        AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BLESSED_MODE, true);
                                    }
                                }
                            }



                        }
                        //THIRD FLOOR
                        if (StaticGUIDs.Cannonbalrog_GUID == GUID | StaticGUIDs.Mine_Flayer_GUID == GUID | StaticGUIDs.Treadnaught_GUID == GUID)
                        {
                            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_FLOOR_3) == false)
                            {
                                Toolbox.NotifyCustom("You Unlocked:", "3 New Weapons!", StaticCollections.Gun_Collection.GetSpriteIdByName("thestandardissue"), StaticCollections.Gun_Collection);
                            }
                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_FLOOR_3, true);
                        }
                        //1st floor no modules
                        if (StaticGUIDs.Bullet_King_GUID == GUID | StaticGUIDs.Smiley_GUID == GUID | StaticGUIDs.Shades_GUID == GUID | StaticGUIDs.Gatling_Gull_GUID == GUID)
                        {
                            {
                                if (player.PlayerHasCore().ReturnActiveTotal() == 0)
                                {
                                    AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.FIRST_FLOOR_NO_MODULES, true);
                                }
                            }
                        }
                        //Draguns
                        if (StaticGUIDs.Advanced_Dragun_GUID == GUID | StaticGUIDs.Dragun_GUID == GUID)
                        {
                            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR) == false)
                            {
                                Toolbox.NotifyCustom("You Unlocked:", "2 New Weapons!", StaticCollections.Gun_Collection.GetSpriteIdByName("thespecialedition"), StaticCollections.Gun_Collection);
                            }

                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR, true);
                            if (player.PlayerHasCore().ReturnActiveTotal() <= 3)
                            {
                                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS, true);
                            }
                            if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH | GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
                            {
                                if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BOSS_RUSH_AS_MODULAR) == false)
                                {
                                    Toolbox.NotifyCustom("You Unlocked:", "Flame Ejector", StaticCollections.Gun_Collection.GetSpriteIdByName("flamer_idle_001"), StaticCollections.Gun_Collection);
                                }
                                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BOSS_RUSH_AS_MODULAR, true);
                            }
                            if (ChallengeManager.CHALLENGE_MODE_ACTIVE == true)
                            {
                                if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.CHALLENGEMODE_DRAGUN) == false)
                                {
                                    Toolbox.NotifyCustom("You Unlocked:", "Fortifier", StaticCollections.Gun_Collection.GetSpriteIdByName("turretplace_reload_001"), StaticCollections.Gun_Collection);
                                }
                                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.CHALLENGEMODE_DRAGUN, true);
                            }
                            /*
                            if (ChallengeManager.CHALLENGE_MODE_ACTIVE == true)
                            {
                                if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.CHALLENGEMODE_DRAGUN) == false)
                                {
                                    Toolbox.NotifyCustom("You Unlocked:", "Barrier Builder", StaticCollections.Gun_Collection.GetSpriteIdByName("shieldgen_idle_004"), StaticCollections.Gun_Collection);
                                }
                                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.CHALLENGEMODE_DRAGUN, true);
                            }
                            */
                        }
                        //Advanced Dragun
                        if (StaticGUIDs.Advanced_Dragun_GUID == GUID)
                        {
                            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR) == false)
                            {
                                Toolbox.NotifyCustom("You Unlocked:", "Kinetic Payload", StaticCollections.Gun_Collection.GetSpriteIdByName("bigbomb_idle_004"), StaticCollections.Gun_Collection);
                            }
                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR, true);
                            if (player.PlayerHasCore().ReturnActiveTotal() <= 3)
                            {
                                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS, true);
                            }
                        }
                    }                  
                }
            }
        }
    }
}
