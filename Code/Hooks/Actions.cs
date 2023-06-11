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
            //new Hook(typeof(PlayerItem).GetMethod("Use", BindingFlags.Instance | BindingFlags.Public), typeof(Actions).GetMethod("PreUse"));
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
        public static System.Action<RoomHandler> OnReinforcementWave;
    }
}
