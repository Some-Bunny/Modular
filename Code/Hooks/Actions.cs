using Dungeonator;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using MonoMod.Cil;
using Mono.Cecil.Cil;
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
            //new Hook(typeof(RoomHandler).GetMethod("TriggerReinforcementLayer", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("TriggerReinforcementLayerHook"));
            //new Hook(typeof(PlayerItem).GetMethod("Drop", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("DropHook"));
            //new Hook(typeof(FloorRewardManifest).GetMethod("GetNextBossReward", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("BossDropHook"));
            //new Hook(typeof(RewardManager).GetMethod("GetRewardObjectBossStyle", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("GetRewardObjectBossStyleHook"));
            //new Hook(typeof(RewardManager).GetMethod("IsBossRewardForcedGun", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("ModifyBossForceGunHook"));

        }

        [HarmonyPatch(typeof(RewardManager), nameof(RewardManager.GetRewardObjectBossStyle))]
        public class Patch_RewardManager_GetRewardObjectBossStyle
        {
            [HarmonyPrefix]
            private static bool Awake(RewardManager __instance, PlayerController player, ref GameObject __result)
            {
                if (player.HasPassiveItem(ConfidenceCore.ConfidenceCoreID))
                {
                    __result = GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.SelectByWeight();
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(RewardManager), nameof(RewardManager.IsBossRewardForcedGun))]
        public class Patch_RewardManager_IsBossRewardForcedGun
        {
            [HarmonyPostfix]
            private static void Awake(RewardManager __instance,  ref bool __result)
            {
                if (ModifyForceGun != null)
                {
                    foreach (var entry in ModifyForceGun.GetInvocationList())
                    {
                        __result = (bool)entry.DynamicInvoke(__result);
                    }
                }
            }
        }



        [HarmonyPatch(typeof(FloorRewardManifest), nameof(FloorRewardManifest.GetNextBossReward))]
        public class Patch_FloorRewardManifest_GetNextBossReward
        {
            [HarmonyPostfix]
            private static void Awake(FloorRewardManifest __instance, bool forceGun, ref PickupObject __result)
            {
                if (ModifyBossDrop != null)
                {
                    foreach (var entry in ModifyBossDrop.GetInvocationList())
                    {
                        __result = (PickupObject)entry.DynamicInvoke(__result);
                    }
                }
            }
        }
        [HarmonyPatch]
        private static class PlayerItem_Drop
        {
            [HarmonyPatch(typeof(PlayerItem), nameof(PlayerItem.Drop))]
            [HarmonyILManipulator]
            private static void Bomk(ILContext il)
            {
                ILCursor cursor = new ILCursor(il);

                if (!cursor.TryGotoNext(MoveType.Before,
                    instr => instr.MatchLdarg(1),
                    instr => instr.MatchLdfld<PlayerController>("stats"),
                    instr => instr.MatchLdarg(1)))
                    return;


                cursor.Emit(OpCodes.Ldarg, 0);
                cursor.Emit(OpCodes.Ldloc, 3);
                cursor.Emit(OpCodes.Call, typeof(PlayerItem_Drop).GetMethod("ActiveItemDropped", BindingFlags.Static | BindingFlags.NonPublic));
            }
            private static void ActiveItemDropped(PlayerItem chestBehavior, PlayerController player)
            {
                //Debug.Log("DSKJNSDAFJKNLDFAS");
                if (OnActiveItemDropped != null) { OnActiveItemDropped(chestBehavior, player); }
            }
        }

        /*
        [HarmonyPatch(typeof(PlayerItem), nameof(PlayerItem.Drop))]
        public class Patch_PlayerItem_Drop
        {
            [HarmonyPostfix]
            private static void Awake(PlayerItem __instance,  ref DebrisObject __result)
            {

                if (OnActiveItemDropped != null) { OnActiveItemDropped(__result.GetComponent<PlayerItem>(), player); }
            }
        }
        */


        /*
        public static bool PreUse(System.Func<PlayerItem, PlayerController, Single, bool> orig, PlayerItem self, PlayerController user, out Single flot)
        {
            flot = -1;
            return orig(self, user, -1);
        }
        */

        [HarmonyPatch(typeof(RoomHandler), nameof(RoomHandler.TriggerReinforcementLayer))]
        public class Patch_RoomHandler_TriggerReinforcementLayer
        {
            [HarmonyPostfix]
            private static void Awake(RoomHandler __instance)
            {
                if (OnReinforcementWave != null && __instance != null) 
                {
                    OnReinforcementWave(__instance);
                }
            }
        }

        public static System.Func<PickupObject,PickupObject> ModifyBossDrop;
        public static System.Func<bool, bool> ModifyForceGun;

        public static System.Action<RoomHandler> OnReinforcementWave;
        public static System.Action<PlayerItem, PlayerController> OnActiveItemDropped;

    }
}
