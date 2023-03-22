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
                if (GUID == StaticGUIDs.Lich_Phase_3_GUID)
                {
                    foreach (PlayerController player in GameManager.Instance.AllPlayers)
                    {
                        if (player.PlayerHasCore() == true)
                        {
                            AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR, true);
                            if (player.PlayerHasCore().ReturnActiveTotal() < 5)
                            {
                                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BEAT_LICH_WITH_4_MODULES_OR_LESS, true);
                            }
                        }
                    }
                }
            }
        }
    }
}
