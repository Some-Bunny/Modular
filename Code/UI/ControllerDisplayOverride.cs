using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModularMod
{

    [HarmonyPatch(typeof(GameCursorController))]
    [HarmonyPatch("showMouseCursor", MethodType.Getter)]
    class CursorPatch
    {
        public static bool DisplayCursorOnController = true;

        [HarmonyPrefix]
        public static bool Postfix(ref bool __result, GameCursorController __instance)
        {
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                if (!BraveInput.GetInstanceForPlayer(0).HasMouse() && !BraveInput.GetInstanceForPlayer(1).HasMouse())
                {
                    return DisplayCursorOnController;
                }
            }
            else if (!BraveInput.GetInstanceForPlayer(0).HasMouse())
            {
                return DisplayCursorOnController;
            }
            return true;
        }
    }
}
