﻿using System;
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

namespace ModularMod
{
    public class CrateSpawnController
    {
        public static void Init()
        {
            Crate = PrefabBuilder.BuildObject("Crate_Interactable");
            var tk2d = Crate.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Crate_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("crate_idle_001"));
            var tk2dAnim = Crate.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = StaticCollections.Crate_Animation;

            tk2dAnim.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2dAnim.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(121, 234, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            tk2dAnim.renderer.material = mat;
            

            SpeculativeRigidbody specBody = Crate.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(1, -12), new IntVector2(32, 20));
            specBody.PixelColliders.Clear();
            specBody.CollideWithTileMap = false;
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.HighObstacle,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 0,
                ManualOffsetY = -4,
                ManualWidth = 32,
                ManualHeight = 20,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.BeamBlocker,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 0,
                ManualOffsetY = -4,
                ManualWidth = 32,
                ManualHeight = 20,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.BulletBlocker,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 0,
                ManualOffsetY = -4,
                ManualWidth = 32,
                ManualHeight = 20,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            Crate.AddComponent<CrateController>();

            SmokeObject = Module.ModularAssetBundle.LoadAsset<GameObject>("SmokeParticle");

            Alexandria.Misc.CustomActions.OnRunStart += OnRunStart;
        }
        public static GameObject Crate;
        public static GameObject SmokeObject;

        public static void OnRunStart(PlayerController player1, PlayerController player2, GameManager.GameMode mode)
        {
            GameManager.Instance.StartCoroutine(DoFall());
        }

        public static IEnumerator DoFall()
        {
            float e = 0;
            while (e < 1f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }

            bool b = false;
            foreach (PlayerController p in GameManager.Instance.AllPlayers)
            {
                if (p.PlayerHasCore() != null && b == false)
                {
                    b = !b;
                    if (CanSpawnCrate() == true)
                    {
                        var newCrate = UnityEngine.Object.Instantiate(Crate, p.transform.position + new Vector3(-5, -3), Quaternion.identity).GetComponent<CrateController>();
                        newCrate.player = p;
                        //Do Spawn Code Here
                    }
                }
            }
            yield return null;
        }


        public static bool CanSpawnCrate()
        {
            var Manager = SaveAPI.AdvancedGameStatsManager.Instance;
            if (Manager == null) { return false; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_FLOOR_3) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_LICH_AS_MODULAR) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_DRAGUN_WITH_3_ACTIVE_MODULES_OR_LESS) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_LICH_WITH_4_MODULES_OR_LESS) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_RAT_AS_MODULAR) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.LEAD_GOD_AS_MODULAR) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BOSS_RUSH_AS_MODULAR) == true) { return true; }

            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BLESSED_MODE) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.CHALLENGEMODE_DRAGUN) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.CHALLENGEMODE_LICH) == true) { return true; }
            if (Manager.GetFlag(SaveAPI.CustomDungeonFlags.BEAT_ADVANCED_DRAGUN_AS_MODULAR) == true) { return true; }


            return false;
        }


        public class CrateController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
        {
            private RoomHandler parentRoom;
            private GameObject instanceMinimapIcon;
            public PlayerController player;

            public bool UsesAltSkin()
            {
                if (player == null) { return false; }
                return player.IsUsingAlternateCostume;
            }

            public void Start()
            {
                if (UsesAltSkin() == true)
                {
                    this.sprite.renderer.material.SetColor("_EmissiveColor", new Color32(0, 255, 54, 255));
                }
                this.StartCoroutine(DoFall());

                /*
                if (SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(SaveAPI.CustomDungeonFlags.CRATE_DROP) == false)
                {
                    SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.CRATE_DROP, true);
                    this.StartCoroutine(DoFall());
                    //Fall code here
                }
                else
                {
                    ConfigureOnPlacement(this.transform.position.GetAbsoluteRoom());
                    this.spriteAnimator.Play(UsesAltSkin() == true ? "cratealt_idle" : "crate_idle");
                }
                */
            }

            public void Update()
            {
                if (player != null && this.parentRoom != null)
                {
                    if (player.CurrentRoom != this.parentRoom)
                    {
                        if (this.parentRoom.IsRegistered(this))
                        {
                            this.parentRoom.DeregisterInteractable(this);
                            this.spriteAnimator.Play(UsesAltSkin() == true ? "cratealt_superclose" : "crate_superclose");
                        }
                    }
                }
            }


            public IEnumerator DoFall()
            {
                this.specRigidbody.enabled = false;
                this.sprite.renderer.enabled = true;
                Vector3 landPosition = this.sprite.WorldCenter;
                Vector3 startPosition = this.transform.position += new Vector3(12f, 30);

                float e = 0;
                while (e < 1.5f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                this.spriteAnimator.Play(UsesAltSkin() == true ? "cratealt_fall" : "crate_fall");

                GameObject eff = UnityEngine.Object.Instantiate<GameObject>(VFXStorage.WarningImpactVFX, landPosition, Quaternion.identity);
                Destroy(eff, 1f);
                tk2dSpriteAnimator component2 = eff.GetComponent<tk2dSpriteAnimator>();
                ParticleSystem particleSystem = UnityEngine.Object.Instantiate(SmokeObject).GetComponent<ParticleSystem>();
                particleSystem.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Whistle_01", this.gameObject);

                if (component2 != null)
                {
                    component2.ignoreTimeScale = true;
                    component2.AlwaysIgnoreTimeScale = true;
                    component2.AnimateDuringBossIntros = true;
                    component2.alwaysUpdateOffscreen = true;
                    component2.playAutomatically = true;
                }

                while (e < 1f)
                {
                    this.specRigidbody.Reinitialize();
                    e += BraveTime.DeltaTime;
                    Vector2 pos = Vector3.Lerp(startPosition, landPosition, e);
                    this.transform.position = pos;
                    particleSystem.gameObject.transform.position = this.sprite.WorldCenter;
                    yield return null;
                }

                Destroy(particleSystem, 60);
                this.sprite.renderer.enabled = true;

                Exploder.Explode(landPosition, StaticExplosionDatas.customDynamiteExplosion, landPosition);
                this.specRigidbody.enabled = true;
                this.specRigidbody.Reinitialize();
                this.spriteAnimator.Stop();
                this.spriteAnimator.Play(UsesAltSkin() == true ? "cratealt_idle" : "crate_idle");
                ConfigureOnPlacement(GameManager.Instance.Dungeon.data.Entrance);


                yield break;
            }



            public void ConfigureOnPlacement(RoomHandler room)
            {
                this.parentRoom = room;
                room.OptionalDoorTopDecorable = (ResourceCache.Acquire("Global Prefabs/Shrine_Lantern") as GameObject);
                if (!room.IsOnCriticalPath && room.connectedRooms.Count == 1)
                {
                    room.ShouldAttemptProceduralLock = true;
                    room.AttemptProceduralLockChance = 0;
                }
                this.parentRoom.RegisterInteractable(this);
                this.RegisterMinimapIcon();
            }
            public void RegisterMinimapIcon()
            {
                this.instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.parentRoom, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
            }

            public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
            {
                shouldBeFlipped = false;
                return string.Empty;
            }

            public float GetDistanceToPoint(Vector2 point)
            {
                if (base.sprite == null)
                {
                    return 100f;
                }
                Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
                return Vector2.Distance(point, v) / 1.5f;
            }

            public float GetOverrideMaxDistance()
            {
                return -1f;
            }
            public void Interact(PlayerController interactor)
            {
                this.spriteAnimator.Play(UsesAltSkin() == true ? "cratealt_open" : "crate_open");
                var ui = StarterGunSelectUIController.GenerateUI();
                ui.ToggleUI(null, interactor);
                ui.OnClosed += () =>
                {
                    this.spriteAnimator.Play(UsesAltSkin() == true ? "cratealt_close" : "crate_close");
                };
                ui.OnUsed += () =>
                {
                    this.parentRoom.DeregisterInteractable(this);
                    this.spriteAnimator.Play(UsesAltSkin() == true ? "cratealt_superclose" : "crate_superclose");
                };
            }
            public void OnEnteredRange(PlayerController interactor)
            {
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
            }

            public void OnExitRange(PlayerController interactor)
            {
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
            }
        }
    }
}
