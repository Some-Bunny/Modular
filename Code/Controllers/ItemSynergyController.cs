using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria;
using Alexandria.PrefabAPI;
using Alexandria.ItemAPI;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using static UnityEngine.ParticleSystem;
using ModularMod.Code.Hooks;
using MonoMod.RuntimeDetour;
using System.Reflection;
using DaikonForge.Tween;

namespace ModularMod
{
    public class ItemSynergyController
    {
        public static void Init()
        {
            new Hook(typeof(OurPowersCombinedItem).GetMethod("GetDamageContribution", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ItemSynergyController).GetMethod("GetDamageContributionHook"));
            new Hook(typeof(MagazineRackItem).GetMethod("DoEffect", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ItemSynergyController).GetMethod("DoEffectHook"));

            //Alexandria.Misc.CustomActions.OnRunStart += OnRunStart;
            Hooks.ChooseModuleController.AdditionalOptionsModifier += ReturnAdditionalOptions;
        }

        public static int ReturnAdditionalOptions(int count)
        {
            foreach (PlayerController p in GameManager.Instance.AllPlayers)
            {
                if (p.PlayerHasCore() != null && p.HasPassiveItem(815)) { return count += 2; }
            }
            return count;
        }

        public static void DoEffectHook(Action<MagazineRackItem, PlayerController> orig, MagazineRackItem self, PlayerController user)
        {
            if (user.PlayerHasCore() != null){

                AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", user.gameObject);
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("Global Prefabs/HoveringGun") as GameObject, user.CenterPosition.ToVector3ZisY(0f), Quaternion.identity);
                HoveringGunController hover = gameObject.GetComponent<HoveringGunController>();
                hover.ConsumesTargetGunAmmo = false;
                hover.ChanceToConsumeTargetGunAmmo = 0f;
                hover.Position = HoveringGunController.HoverPosition.CIRCULATE;
                hover.Aim = HoveringGunController.AimType.PLAYER_AIM;
                hover.Trigger = HoveringGunController.FireType.ON_FIRED_GUN;
                hover.CooldownTime = 0.5f;
                hover.ShootDuration = 0.3f;
                hover.OnlyOnEmptyReload = false;
                hover.Initialize(PickupObjectDatabase.GetRandomGun(), user);
                UnityEngine.Object.Destroy(hover.gameObject, 15);

            }
            else
            {
                orig(self, user);
            }
        }
        public static float GetDamageContributionHook(Func<OurPowersCombinedItem, float> orig, OurPowersCombinedItem self)
        {
            if (self.Owner == null) { return 1; }
            if (self.Owner.PlayerHasCore() != null)
            {
                return self.Owner.PlayerHasCore().ReturnActiveTotal() * 2f;
            }
            return orig(self);
        }
    }
}
