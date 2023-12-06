using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    [HarmonyPatch(typeof(BasicBeamController), nameof(BasicBeamController.FindBeamTarget))]
    class BeamCollisionGarbage
    {
        [HarmonyPostfix]
        private static bool Postfix(bool __result, BasicBeamController __instance, Vector2 origin, Vector2 direction, float distance, int collisionMask, Vector2 targetPoint, Vector2 targetNormal, SpeculativeRigidbody hitRigidbody)
        {        
            if (hitRigidbody != null)
            {
                var commision = hitRigidbody.GetComponent<BeamCollisionEvent>();
                if (commision != null)
                {
                    return commision.isCollisionEvent;
                }
            }          
            return __result;
        }
    }
}
