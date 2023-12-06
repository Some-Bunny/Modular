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
using System.ComponentModel;
using static tk2dSpriteCollectionDefinition;

namespace ModularMod
{
    public class ItemSynergyController
    {
        public static void Init()
        {
            new Hook(typeof(OurPowersCombinedItem).GetMethod("GetDamageContribution", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ItemSynergyController).GetMethod("GetDamageContributionHook"));
            new Hook(typeof(MagazineRackItem).GetMethod("DoEffect", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ItemSynergyController).GetMethod("DoEffectHook"));
            
            
            new Hook(typeof(SprenOrbitalItem).GetMethod("HandleTransformationDuration", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ItemSynergyController).GetMethod("HandleTransformationDurationHook"));
            new Hook(typeof(SprenOrbitalItem).GetMethod("DetransformSpren", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ItemSynergyController).GetMethod("DetransformSprenHook"));

            new Hook(typeof(TripleTapEffect).GetMethod("HandleProjectileDestruction", BindingFlags.Instance | BindingFlags.NonPublic), typeof(ItemSynergyController).GetMethod("YurkeyModularHook"));

            ChooseModuleController.AdditionalOptionsModifier += ReturnAdditionalOptions;
            ChooseModuleController.ModifyOmegaModuleChance += OmegaChance;
            ChooseModuleController.OnModuleSelectGunDestroyed += OGEE;
            //ChooseModuleController.CarrierModifier += CarrierModifier;

            Scrapper.OnAnythingScrapped += OGEE_2;

        }

        public static void YurkeyModularHook(Action<TripleTapEffect, Projectile> orig, TripleTapEffect self, Projectile source)
        {
            if (self.m_player.PlayerHasCore() != null)
            {
                if (source.PlayerProjectileSourceGameTimeslice == -1f)
                {
                    return;
                }
                if (!self.m_slicesFired.ContainsKey(source.PlayerProjectileSourceGameTimeslice))
                {
                    return;
                }
                if (self.m_player && source)
                {
                    if (source.HasImpactedEnemy)
                    {
                        self.m_slicesFired.Remove(source.PlayerProjectileSourceGameTimeslice);
                        if (self.m_player.HasActiveBonusSynergy(CustomSynergyType.GET_IT_ITS_BOWLING, false))
                        {
                            self.m_shotCounter = Mathf.Min(self.RequiredSequentialShots, self.m_shotCounter + source.NumberHealthHaversHit);
                        }
                        else
                        {
                            self.m_shotCounter++;
                        }
                        if (self.m_shotCounter >= self.RequiredSequentialShots)
                        {
                            self.m_shotCounter -= self.RequiredSequentialShots;
                            self.m_player.PlayerHasCore().NextShotCrit = true;
                        }
                    }
                    else
                    {
                        self.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] = self.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] - 1;
                        if (self.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] == 0)
                        {
                            self.m_shotCounter = 0;
                        }
                    }
                }
            }
            else
            {
                orig(self, source);
            }
        }



        public static List<ChooseModuleController.ModuleUICarrier> CarrierModifier(List<ChooseModuleController.ModuleUICarrier> moduleUICarriers, ChooseModuleController chooseModuleController)
        {
            if (chooseModuleController.g.PickupObjectId == 90)
            {
                moduleUICarriers.Add(new ChooseModuleController.ModuleUICarrier() 
                {
                    defaultModule = PickupObjectDatabase.GetById(BeholsterEye.ID).GetComponent<DefaultModule>(),
                    controller = chooseModuleController,
                    isUsingAlternate = chooseModuleController.isAlt
                });
                moduleUICarriers.Shuffle();
            }
            if (chooseModuleController.g.PickupObjectId == 27)
            {
                moduleUICarriers.Add(new ChooseModuleController.ModuleUICarrier()
                {
                    defaultModule = PickupObjectDatabase.GetById(DuckHunter.ID).GetComponent<DefaultModule>(),
                    controller = chooseModuleController,
                    isUsingAlternate = chooseModuleController.isAlt
                });
                moduleUICarriers.Shuffle();
            }
            if (chooseModuleController.g.PickupObjectId == 577)
            {
                moduleUICarriers.Add(new ChooseModuleController.ModuleUICarrier()
                {
                    defaultModule = PickupObjectDatabase.GetById(CarpalTunnel.ID).GetComponent<DefaultModule>(),
                    controller = chooseModuleController,
                    isUsingAlternate = chooseModuleController.isAlt
                });
                moduleUICarriers.Shuffle();
            }
            if (chooseModuleController.g.PickupObjectId == 748)
            {
                moduleUICarriers.Add(new ChooseModuleController.ModuleUICarrier()
                {
                    defaultModule = PickupObjectDatabase.GetById(BrilliantSun.ID).GetComponent<DefaultModule>(),
                    controller = chooseModuleController,
                    isUsingAlternate = chooseModuleController.isAlt
                });
                moduleUICarriers.Shuffle();
            }
            if (chooseModuleController.g.PickupObjectId == 198)
            {
                moduleUICarriers.Add(new ChooseModuleController.ModuleUICarrier()
                {
                    defaultModule = PickupObjectDatabase.GetById(WillingSpirit.ID).GetComponent<DefaultModule>(),
                    controller = chooseModuleController,
                    isUsingAlternate = chooseModuleController.isAlt
                });
                moduleUICarriers.Shuffle();
            }
            if (chooseModuleController.g.PickupObjectId == 149)
            {
                moduleUICarriers.Add(new ChooseModuleController.ModuleUICarrier()
                {
                    defaultModule = PickupObjectDatabase.GetById(MusicBox.ID).GetComponent<DefaultModule>(),
                    controller = chooseModuleController,
                    isUsingAlternate = chooseModuleController.isAlt
                });
                moduleUICarriers.Shuffle();
            }
            return moduleUICarriers;    
        }
        
        public static void OGEE(Gun g)
        {
            if (g.PickupObjectId == 514)
            {
                GameManager.Instance.RewardManager.SpawnTotallyRandomChest(new IntVector2((int)g.transform.position.x, (int)g.transform.position.y));
            }
            if (g.PickupObjectId == 599 | g.PickupObjectId == 333)
            {
                AkSoundEngine.PostEvent("Play_ENM_blobulord_splash_01", g.gameObject);
                float f = BraveUtility.RandomAngle();
                for (int i = 0; i < 3; i++)
                {
                    var o = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(333) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, g.sprite.WorldCenter, Quaternion.Euler(0, 0, f + (120 * i)));
                    UnityEngine.Object.Destroy(o, 0.7f);
                }
                var q = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(449) as TeleporterPrototypeItem).TelefragVFXPrefab, g.sprite.WorldCenter, Quaternion.identity);
            }
            if (g.PickupObjectId == 520 | g.PickupObjectId == 337)
            {
                AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", g.gameObject);
                AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", g.gameObject);
                GameObject gameObject = new GameObject("silencer");
                var silencer = (PickupObjectDatabase.GetById(224) as SilencerItem);
                SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
                silencerInstance.TriggerSilencer(g.sprite.WorldCenter, silencer.silencerSpeed, silencer.silencerRadius, silencer.silencerVFXPrefab, silencer.distortionIntensity, silencer.distortionRadius, silencer.pushForce, silencer.pushRadius, silencer.knockbackForce, silencer.knockbackRadius, silencer.additionalTimeAtMaxRadius, GameManager.Instance.PrimaryPlayer, true, false);
            }
            if (g.PickupObjectId == 95)
            {
                float f = BraveUtility.RandomAngle();
                for (int i = 0; i < 3; i++)
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(67).gameObject, g.sprite.WorldTopCenter, Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, 3, i), 1), 2, true, true);
                }
            }
            if (g.PickupObjectId == 626)
            {
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(BlockOfCheese.CheeseID).gameObject, g.sprite.WorldTopCenter,Vector2.zero, 2, true, true);
            }
        }
        public static void OGEE_2(PickupObject g)
        {
            if (g.PickupObjectId == 514)
            {
                GameManager.Instance.RewardManager.SpawnTotallyRandomChest(new IntVector2((int)g.transform.position.x, (int)g.transform.position.y));
            }
            if (g.PickupObjectId == 599 | g.PickupObjectId == 333)
            {
                AkSoundEngine.PostEvent("Play_ENM_blobulord_splash_01", g.gameObject);
                float f = BraveUtility.RandomAngle();
                for (int i = 0; i < 3; i++)
                {
                    var o = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(333) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, g.sprite.WorldCenter, Quaternion.Euler(0, 0, f + (120 * i)));
                    UnityEngine.Object.Destroy(o, 0.7f);
                }
                var q = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(449) as TeleporterPrototypeItem).TelefragVFXPrefab, g.sprite.WorldCenter, Quaternion.identity);

            }
            if (g.PickupObjectId == 520 | g.PickupObjectId == 337)
            {
                AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", g.gameObject);
                AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", g.gameObject);
                GameObject gameObject = new GameObject("silencer");
                var silencer = (PickupObjectDatabase.GetById(224) as SilencerItem);
                SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
                silencerInstance.TriggerSilencer(g.sprite.WorldCenter, silencer.silencerSpeed, silencer.silencerRadius, silencer.silencerVFXPrefab, silencer.distortionIntensity, silencer.distortionRadius, silencer.pushForce, silencer.pushRadius, silencer.knockbackForce, silencer.knockbackRadius, silencer.additionalTimeAtMaxRadius, GameManager.Instance.PrimaryPlayer, true, false);
            }
            if (g.PickupObjectId == 95)
            {
                float f = BraveUtility.RandomAngle();
                for (int i = 0; i < 3; i++)
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(67).gameObject, g.sprite.WorldTopCenter, Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, 3, i), 1), 2, true, true);
                }
            }
        }


        public static IEnumerator HandleTransformationDurationHook(Func<SprenOrbitalItem, IEnumerator> orig, SprenOrbitalItem self)
        {
            tk2dSpriteAnimator extantAnimator = self.m_extantOrbital.GetComponentInChildren<tk2dSpriteAnimator>();
            extantAnimator.Play(self.GunChangeAnimation);
            PlayerOrbitalFollower follower = self.m_extantOrbital.GetComponent<PlayerOrbitalFollower>();
            if (follower)
            {
                follower.OverridePosition = true;
            }
            float elapsed = 0f;
            extantAnimator.sprite.HeightOffGround = 5f;
            while (elapsed < 1f)
            {
                elapsed += BraveTime.DeltaTime;
                if (follower && self.m_player)
                {
                    follower.OverrideTargetPosition = self.m_player.CenterPosition;
                }
                yield return null;
            }
            extantAnimator.Play(self.GunChangeMoreAnimation);
            while (extantAnimator.IsPlaying(self.GunChangeMoreAnimation))
            {
                if (follower && self.m_player)
                {
                    follower.OverrideTargetPosition = self.m_player.CenterPosition;
                }
                yield return null;
            }
            if (follower)
            {
                follower.ToggleRenderer(false);
            }
            self.m_player.inventory.GunChangeForgiveness = true;
            self.m_transformation = SprenOrbitalItem.SprenTransformationState.TRANSFORMED;

            Dictionary<DefaultModule, int> m = new Dictionary<DefaultModule, int>();

            if (self.m_player.PlayerHasCore() != null)
            {
                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Wizard_Cast_01", self.gameObject);
                var help_1 = GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_1);
                VFXStorage.DoFancyFlashOfModules(3, self.m_owner, help_1);
                self.m_player.PlayerHasCore().GiveTemporaryModule(help_1, "Sprun", 3);
                m.Add(help_1, 3);
                yield return null;
                var help_2 = GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_2);
                VFXStorage.DoFancyFlashOfModules(2, self.m_owner, help_2);
                self.m_player.PlayerHasCore().GiveTemporaryModule(help_2, "Sprun", 2);
                m.Add(help_2, 2);

                yield return null;
                var help_3 = GlobalModuleStorage.ReturnRandomModule(DefaultModule.ModuleTier.Tier_3);
                VFXStorage.DoFancyFlashOfModules(1, self.m_owner, help_3);
                self.m_player.PlayerHasCore().GiveTemporaryModule(help_3, "Sprun", 1);
                m.Add(help_3, 1);
            }
            else
            {
                Gun limitGun = PickupObjectDatabase.GetById(self.LimitGunId) as Gun;
                self.m_extantGun = self.m_player.inventory.AddGunToInventory(limitGun, true);
                self.m_extantGun.CanBeDropped = false;
                self.m_extantGun.CanBeSold = false;
                self.m_player.inventory.GunLocked.SetOverride("spren gun", true, null);
            }
          
            elapsed = 0f;
            while (elapsed < self.LimitDuration)
            {
                if (follower && self.m_player)
                {
                    follower.OverrideTargetPosition = self.m_player.CenterPosition;
                }
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            if (follower)
            {
                follower.ToggleRenderer(true);
            }
            if (extantAnimator)
            {
                extantAnimator.PlayForDuration(self.BackchangeAnimation, -1f, self.IdleAnimation, false);
            }
            while (extantAnimator.IsPlaying(self.BackchangeAnimation))
            {
                if (follower && self.m_player)
                {
                    follower.OverrideTargetPosition = self.m_player.CenterPosition;
                }
                yield return null;
            }
            if (self.m_player.PlayerHasCore() != null)
            {
                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Wizard_Kick_01", self.gameObject);
                self.m_player.PlayerHasCore().RemoveTemporaryModules("Sprun", true);
            }
            follower.OverridePosition = false;
            self.DetransformSpren();
            yield break;
        }

        public static void DetransformSprenHook(Action<SprenOrbitalItem> orig, SprenOrbitalItem self)
        {
            if (self.m_transformation != SprenOrbitalItem.SprenTransformationState.TRANSFORMED)
            {
                return;
            }
            if (!self || !self.m_player || !self.m_extantGun)
            {
                return;
            }
            self.m_transformation = SprenOrbitalItem.SprenTransformationState.NORMAL;
            if (self.m_player)
            {
                if (self.m_player.PlayerHasCore() != null)
                {

                }
                else
                {
                    if (!GameManager.Instance.IsLoadingLevel && !Dungeon.IsGenerating)
                    {
                        Minimap.Instance.ToggleMinimap(false, false);
                    }
                    self.m_player.inventory.GunLocked.RemoveOverride("spren gun");
                    self.m_player.inventory.DestroyGun(self.m_extantGun);
                    self.m_extantGun = null;
                }

            }
            self.m_player.inventory.GunChangeForgiveness = false;
        }
        public static float OmegaChance(PickupObject.ItemQuality q, DefaultModule.ModuleTier tier, float count)
        {
            foreach (PlayerController p in GameManager.Instance.AllPlayers)
            {
                if (p.PlayerHasCore() != null && p.HasPassiveItem(815)) { return count *= 5; }
            }
            return count;
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
            if (user.PlayerHasCore() != null)
            {

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





        public class ModularSynergy
        {
            public class SynergyMarker : MonoBehaviour { }
            public static List<ModularSynergy> synergizing_Items = new List<ModularSynergy>();

            public ModularSynergy(string syn, string consoleName)
            {
                item_Id = Gungeon.Game.Items[consoleName].PickupObjectId;
                synergy_Name = syn;
                var obj = PickupObjectDatabase.GetById(item_Id);
                obj.gameObject.AddComponent<SynergyMarker>();
                Alexandria.ItemAPI.CustomSynergies.Add(syn, new List<string> { "mdl:modular_printer_core" }, new List<string> { consoleName }, true);

            }

            public static string Get_Synergy_Name(int ID)
            {
                foreach (var entry in synergizing_Items)
                {
                    if (entry.item_Id == ID) { return entry.synergy_Name; }
                }
                return "ERROR";
            }

            public static bool isSynergyItem(int ID)
            {
                foreach (var entry in synergizing_Items)
                {
                    if (entry.item_Id == ID) { return true; }
                }
                return false;
            }


            public bool ModuleSynergyIsAvailable(PlayerController p)
            {
                return this.PlayerHasPickup(p, item_Id);
            }

            public bool PlayerHasPickup(PlayerController p, int pickupID)
            {
                if (p && p.inventory != null && p.inventory.AllGuns != null)
                {
                    for (int i = 0; i < p.inventory.AllGuns.Count; i++)
                    {
                        if (p.inventory.AllGuns[i].PickupObjectId == pickupID && p.PlayerHasCore() != null)
                        {
                            return true;
                        }
                    }
                }
                if (p)
                {
                    for (int j = 0; j < p.activeItems.Count; j++)
                    {
                        if (p.activeItems[j].PickupObjectId == pickupID && p.PlayerHasCore() != null)
                        {
                            return true;
                        }
                    }
                    for (int k = 0; k < p.passiveItems.Count; k++)
                    {
                        Debug.Log("fuck5");
                        if (p.passiveItems[k].PickupObjectId == pickupID && p.PlayerHasCore() != null)
                        {
                            return true;
                        }
                    }
                    if (pickupID == GlobalItemIds.Map && p.EverHadMap)
                    {
                        return true;
                    }
                }
                return false;
            }
            public int item_Id;
            public string synergy_Name;
        }      
    }
}
