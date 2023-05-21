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

namespace ModularMod
{
    public class UIHooks
    {
        public static void Init()
        {
           new Hook(typeof(GameManager).GetMethod("Pause", BindingFlags.Instance | BindingFlags.Public), typeof(UIHooks).GetMethod("PauseHook", BindingFlags.Static | BindingFlags.Public));


                    
        }
        public static void PauseHook(Action<GameManager> orig, GameManager self)
        {
            if (StarterGunSelectUIController.Inst != null)
            {
                StarterGunSelectUIController.Inst.ToggleUI(false, null, true);
            }
            var scrapLabel = ScrapUIController.FindScrapUI(GameUIRoot.Instance);
            scrapLabel.isVisible = ScrapUIController.ScrapCounterVisible().First;
            scrapLabel.enabled = ScrapUIController.ScrapCounterVisible().First;

            orig(self);
        }     
    }
}
