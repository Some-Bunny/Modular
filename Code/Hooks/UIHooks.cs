using Alexandria.PrefabAPI;
using MonoMod.RuntimeDetour;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Alexandria.CharacterAPI;
using static ModularMod.StarterGunSelectUIController;
using HarmonyLib;

namespace ModularMod
{
    public class UIHooks
    {
        public static void Init()
        {
           //new Hook(typeof(GameManager).GetMethod("Pause", BindingFlags.Instance | BindingFlags.Public), typeof(UIHooks).GetMethod("PauseHook", BindingFlags.Static | BindingFlags.Public));                   
        }

        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Pause))]
        public class Patch_GameManager_Pause
        {
            [HarmonyPrefix]
            private static void Awake(GameManager __instance)
            {
                if (StarterGunSelectUIController.Inst != null)
                {
                    StarterGunSelectUIController.Inst.ToggleUI(false, null, true);
                }
                CursorPatch.DisplayCursorOnController = false;
                var scrapLabel = ScrapUIController.FindScrapUI(GameUIRoot.Instance);
                scrapLabel.isVisible = ScrapUIController.ScrapCounterVisible().First;
                scrapLabel.enabled = ScrapUIController.ScrapCounterVisible().First;
                if (OnPaused != null) { OnPaused(); }
            }
        }

        /*
        public static void PauseHook(Action<GameManager> orig, GameManager self)
        {
            if (StarterGunSelectUIController.Inst != null)
            {
                StarterGunSelectUIController.Inst.ToggleUI(false, null, true);
            }
            CursorPatch.DisplayCursorOnController = false;
            var scrapLabel = ScrapUIController.FindScrapUI(GameUIRoot.Instance);
            scrapLabel.isVisible = ScrapUIController.ScrapCounterVisible().First;
            scrapLabel.enabled = ScrapUIController.ScrapCounterVisible().First;
            if (OnPaused != null) { OnPaused(); }
            orig(self);
        }
        */
        public static Action OnPaused;
    }
}
