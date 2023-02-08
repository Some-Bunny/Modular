using Alexandria.ItemAPI;
using Gungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class VFXStorage
    {
        public static void AssignVFX()
        {
            RadialRing = (GameObject)ResourceCache.Acquire("Global VFX/HeatIndicator");
            TeleportDistortVFX = (PickupObjectDatabase.GetById(573) as ChestTeleporterItem).TeleportVFX;
            TeleportVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");
            RelodestoneContinuousSuckVFX = (PickupObjectDatabase.GetById(536) as RelodestoneItem).ContinuousVFX;
            GameObject dragunBoulder = EnemyDatabase.GetOrLoadByGuid("05b8afe0b6cc4fffa9dc6036fa24c8ec").GetComponent<DraGunController>().skyBoulder;
            foreach (Component item in dragunBoulder.GetComponentsInChildren(typeof(Component)))
            {
                if (item is SkyRocket laser)
                {
                    DragunBoulderLandVFX = laser.ExplosionData.effect;
                }
            }
            HealingSparklesVFX = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001");
            FriendlyElectricLinkVFX = (PickupObjectDatabase.GetById(298) as ComplexProjectileModifier).ChainLightningVFX;
            var machoBrace = PickupObjectDatabase.GetById(665) as MachoBraceItem;
            MachoBraceDustupVFX = machoBrace.DustUpVFX;
            MachoBraceBurstVFX = machoBrace.BurstVFX;
        }

        public static GameObject RadialRing;
        public static GameObject TeleportDistortVFX;
        public static GameObject TeleportVFX;
        public static GameObject RelodestoneContinuousSuckVFX;
        public static GameObject DragunBoulderLandVFX;
        public static GameObject HealingSparklesVFX;
        public static GameObject FriendlyElectricLinkVFX;
        public static GameObject MachoBraceDustupVFX;
        public static GameObject MachoBraceBurstVFX;
    }
}
