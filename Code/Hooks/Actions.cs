using Dungeonator;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace ModularMod.Code.Hooks
{
    public class Actions
    {
        public static void Init()
        {
            new Hook(typeof(RoomHandler).GetMethod("TriggerReinforcementLayer", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("TriggerReinforcementLayerHook"));
            new Hook(typeof(PlayerItem).GetMethod("Drop", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("DropHook"));
            new Hook(typeof(FloorRewardManifest).GetMethod("GetNextBossReward", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("BossDropHook"));
            new Hook(typeof(RewardManager).GetMethod("GetRewardObjectBossStyle", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("GetRewardObjectBossStyleHook"));
            new Hook(typeof(RewardManager).GetMethod("IsBossRewardForcedGun", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("ModifyBossForceGunHook"));

        }

        public static GameObject GetRewardObjectBossStyleHook(System.Func<RewardManager, PlayerController, GameObject> orig, RewardManager self, PlayerController player)
        {
            FloorRewardData currentRewardData = self.CurrentRewardData;
            bool flag;
            if (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON && GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER && player && player.inventory != null && player.inventory.GunCountModified <= 3)
            {
                flag = (UnityEngine.Random.value > 0.2f);
            }
            else if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER && player && player.inventory != null && player.inventory.GunCountModified <= 2)
            {
                flag = (UnityEngine.Random.value > 0.3f);
            }
            else
            {
                flag = (UnityEngine.Random.value > self.ItemVsGunChanceBossReward);
            }
            if (self.IsBossRewardForcedGun())
            {
                flag = true;
            }
            if ((GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH) && !GameManager.Instance.Dungeon.HasGivenBossrushGun)
            {
                GameManager.Instance.Dungeon.HasGivenBossrushGun = true;
                flag = true;
            }
            if (ModifyForceGun != null)
            {
                flag = ModifyForceGun(flag);
            }
            if (flag)
            {
                PickupObject.ItemQuality randomBossTargetQuality = currentRewardData.GetRandomBossTargetQuality(null);
                return self.GetItemForPlayer(player, self.GunsLootTable, randomBossTargetQuality, null, false, null, false, null, false, RewardManager.RewardSource.BOSS_PEDESTAL);
            }
            return self.GetRewardItemDaveStyle(player, true);
        }

        public static bool ModifyBossForceGunHook(System.Func<RewardManager, bool> orig, RewardManager self)
        {
            if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.BOSSRUSH)
            {
                bool flag = true;
                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    if (GameManager.Instance.AllPlayers[i] && (GameManager.Instance.AllPlayers[i].HasReceivedNewGunThisFloor || GameManager.Instance.AllPlayers[i].CharacterUsesRandomGuns))
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    Debug.LogWarning("Potentially Force Drop GUN");
                    if (ModifyForceGun != null)
                    {
                        return ModifyForceGun(flag);
                    }
                    return true;
                }
            }
            return false;
        }

        public static PickupObject BossDropHook(System.Func<FloorRewardManifest, bool, PickupObject> orig, FloorRewardManifest self, bool forceGun)
        {
            if (forceGun)
            {
                self.m_bossGunIndex++;

                return ModifyBossDrop != null ? ModifyBossDrop(self.PregeneratedBossRewardsGunsOnly[self.m_bossGunIndex - 1]) : self.PregeneratedBossRewardsGunsOnly[self.m_bossGunIndex - 1];
            }
            self.m_bossIndex++;

            return ModifyBossDrop != null ? ModifyBossDrop(self.PregeneratedBossRewardsGunsOnly[self.m_bossGunIndex - 1]) : self.PregeneratedBossRewards[self.m_bossIndex - 1];
        }

        public static DebrisObject DropHook(System.Func<PlayerItem, PlayerController, float, DebrisObject> orig, PlayerItem self, PlayerController player, float overrideForce = 4f)
        {
            var debris = orig(self, player, overrideForce);
            if (OnActiveItemDropped != null) { OnActiveItemDropped(debris.GetComponent<PlayerItem>(), player); }
            return debris;
        }
        public static bool PreUse(System.Func<PlayerItem, PlayerController, Single, bool> orig, PlayerItem self, PlayerController user, out Single flot)
        {
            flot = -1;
            return orig(self, user, -1);
        }

        public static bool TriggerReinforcementLayerHook(Func<RoomHandler, int, bool, bool, int, int, bool, bool> orig, RoomHandler self, int index, bool removeLayer = true, bool disableDrops = false, int specifyObjectIndex = -1, int specifyObjectCount = -1, bool instant = false)
        {
            try
            {
                if (OnReinforcementWave != null && self != null) { OnReinforcementWave(self); }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return orig(self, index, removeLayer, disableDrops, specifyObjectIndex, specifyObjectCount, instant);
        }
        public static System.Func<PickupObject,PickupObject> ModifyBossDrop;
        public static System.Func<bool, bool> ModifyForceGun;

        public static System.Action<RoomHandler> OnReinforcementWave;
        public static System.Action<PlayerItem, PlayerController> OnActiveItemDropped;

    }
}
