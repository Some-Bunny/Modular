using Dungeonator;
using ModularMod.Code.Hooks;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModularMod
{
    public class BlessedMode_Modifier
    {
        public static void Init()
        {
            new Hook(typeof(PlayerController).GetMethod("ChangeToRandomGun", BindingFlags.Instance | BindingFlags.Public), typeof(BlessedMode_Modifier).GetMethod("ChangeToRandomGunHook"));
            new Hook(typeof(PlayerController).GetMethod("Update", BindingFlags.Instance | BindingFlags.Public), typeof(BlessedMode_Modifier).GetMethod("UpdateHook"));

        }


        public static DefaultModule ReturnSelectedModule(int tier)
        {
            var mod = GlobalModuleStorage.SelectTable(PickupObject.ItemQuality.B);
            return mod.ModularSelectByWeight(false, Func).GetComponent<DefaultModule>();
        }

        private static bool Func(DefaultModule defaultModule)
        {
            if (defaultModule.AppearsFromBlessedModeRoll == false) { return false; }
            return true;
        }

        public static int FloorMultiplier(Dungeon floor)
        {
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON) { return 0; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON) { return 0; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON) { return 1; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON) { return 1; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON) { return 2; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON) { return 1; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON) { return 1; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON) { return 2; }
            if (floor.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON) { return 2; }

            return 1;
        }
        public static void ChangeToRandomGunHook(Action<PlayerController> orig, PlayerController self)
        {
            if (self.PlayerHasCore() != null)
            {
                self.m_gunGameElapsed = 0f;
                self.m_gunGameDamageThreshold = 1250f + UnityEngine.Random.Range(0, 1250);
                if (self.inventory.GunLocked.Value)
                {
                    return;
                }
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL)
                {
                    return;
                }
                self.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_MagicFavor_Change") as GameObject, new Vector3(0f, -1f, 0f), true, false, false);
                var core = self.PlayerHasCore();
                core.RemoveTemporaryModules("BLESSED_MODE", true);
                int amount = UnityEngine.Random.Range(1, 2);
                amount += FloorMultiplier(GameManager.Instance.Dungeon);
                for (int i = 1; i < amount + 1; i++)
                {
                    int tier = UnityEngine.Random.Range(1, 11);
                    core.GiveTemporaryModule(ReturnSelectedModule(tier), "BLESSED_MODE", i, true);
                }
            }
            else
            {
                orig(self);
            }
        }

        [Obsolete]
        public static void UpdateHook(Action<PlayerController> orig, PlayerController self)
        {
            if (self.CharacterUsesRandomGuns == true && self.PlayerHasCore() != null)
            {
                typeof(GameActor).GetMethod("Update").InvokeNotOverride(self, null);
                /*
                 *         var method = typeof(GaneActor).GetMethod("Update");
                            var ftn = method.MethodHandle.GetFunctionPointer();
                            var func = (Action)Activator.CreateInstance(typeof(Action), self, ftn);
                            func.Invoke();
                */

                if (GameManager.Instance.IsPaused || GameManager.Instance.UnpausedThisFrame)
                {
                    return;
                }
                if (GameManager.Instance.IsLoadingLevel)
                {
                    return;
                }
                self.m_interactedThisFrame = false;
                if (self.IsPetting && (!self.spriteAnimator.IsPlaying("pet") || !self.m_pettingTarget || self.m_pettingTarget.m_pettingDoer != self || Vector2.Distance(self.specRigidbody.UnitCenter, self.m_pettingTarget.specRigidbody.UnitCenter) > 3f || self.IsDodgeRolling))
                {
                    self.ToggleGunRenderers(true, "petting");
                    self.ToggleHandRenderers(true, "petting");
                    self.m_pettingTarget = null;
                }
                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9)
                {
                    self.dx9counter += GameManager.INVARIANT_DELTA_TIME;
                    if (self.dx9counter > 5f)
                    {
                        self.dx9counter = 0f;
                        tk2dSprite[] componentsInChildren = self.GetComponentsInChildren<tk2dSprite>();
                        for (int i = 0; i < componentsInChildren.Length; i++)
                        {
                            componentsInChildren[i].ForceBuild();
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.F8))
                    {
                        tk2dBaseSprite[] array = UnityEngine.Object.FindObjectsOfType<tk2dBaseSprite>();
                        for (int j = 0; j < array.Length; j++)
                        {
                            if (array[j])
                            {
                                array[j].ForceBuild();
                            }
                        }
                        ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                        for (int k = 0; k < allProjectiles.Count; k++)
                        {
                            Projectile projectile = allProjectiles[k];
                            if (projectile && projectile.sprite)
                            {
                                projectile.sprite.ForceBuild();
                            }
                        }
                    }
                }
                if (self.healthHaver.IsDead && !self.IsGhost)
                {
                    return;
                }

                self.HandlePostDodgeRollTimer();
                self.m_activeActions = BraveInput.GetInstanceForPlayer(self.PlayerIDX).ActiveActions;
                if ((!self.AcceptingNonMotionInput || self.CurrentStoneGunTimer > 0f) && self.CurrentGun != null && self.CurrentGun.IsFiring && (!self.CurrentGun.IsCharging || (self.CurrentInputState != PlayerInputState.OnlyMovement && !GameManager.IsBossIntro)))
                {
                    self.CurrentGun.CeaseAttack(false, null);
                    if (self.CurrentSecondaryGun)
                    {
                        self.CurrentSecondaryGun.CeaseAttack(false, null);
                    }
                }
                if (self.inventory != null)
                {
                    self.inventory.FrameUpdate();
                }
                Projectile.UpdateEnemyBulletSpeedMultiplier();
                float num = Mathf.Clamp01(BraveTime.DeltaTime / 0.5f);
                if (num > 0f && num < 1f)
                {
                    Vector2 value = self.AverageVelocity * (1f - num) + self.specRigidbody.Velocity * num;
                    self.AverageVelocity = BraveMathCollege.ClampSafe(value, -20f, 20f);
                }
                if (self.m_isFalling)
                {
                    return;
                }
                if ((self.IsDodgeRolling || self.m_dodgeRollState == PlayerController.DodgeRollState.AdditionalDelay) && self.m_dodgeRollTimer >= self.rollStats.GetModifiedTime(self))
                {
                    if (self.DodgeRollIsBlink)
                    {
                        if (self.m_dodgeRollTimer > self.rollStats.GetModifiedTime(self) + 0.1f)
                        {
                            self.IsEthereal = false;
                            self.IsVisible = true;
                            self.ClearDodgeRollState();
                            self.previousMineCart = null;
                        }
                        else if (self.m_dodgeRollTimer > self.rollStats.GetModifiedTime(self))
                        {
                            self.EndBlinkDodge();
                        }
                    }
                    else
                    {
                        self.ClearDodgeRollState();
                        self.previousMineCart = null;
                    }
                }

                Delegate dodge = self.GetEventDelegate("OnIsRolling");
                if (self.IsDodgeRolling && dodge != null)
                {
                    dodge.DynamicInvoke(new object[] { self });
                }

                CellVisualData.CellFloorType cellFloorType = CellVisualData.CellFloorType.Stone;
                cellFloorType = GameManager.Instance.Dungeon.GetFloorTypeFromPosition(self.specRigidbody.UnitBottomCenter);
                if (self.m_prevFloorType == null || self.m_prevFloorType.Value != cellFloorType)
                {
                    self.m_prevFloorType = new CellVisualData.CellFloorType?(cellFloorType);
                    AkSoundEngine.SetSwitch("FS_Surfaces", cellFloorType.ToString(), self.gameObject);
                }
                self.m_playerCommandedDirection = Vector2.zero;
                self.IsFiring = false;
                if (!BraveUtility.isLoadingLevel && !GameManager.Instance.IsLoadingLevel)
                {
                    self.ProcessDebugInput();
                    if (GameUIRoot.Instance.MetalGearActive)
                    {
                        if (self.m_activeActions.GunDownAction.WasPressed || self.m_activeActions.GunUpAction.WasPressed)
                        {
                            self.m_gunChangePressedWhileMetalGeared = true;
                        }
                    }
                    else
                    {
                        self.m_gunChangePressedWhileMetalGeared = false;
                    }
                    if (self.AcceptingAnyInput)
                    {
                        try
                        {
                            self.m_playerCommandedDirection = self.HandlePlayerInput();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Caught PlayerController.HandlePlayerInput() exception. i={0}, ex={1}", self.exceptionTracker, ex.ToString()));
                        }
                    }
                    if (self.m_newFloorNoInput && self.m_playerCommandedDirection.magnitude > 0f)
                    {
                        self.m_newFloorNoInput = false;
                    }
                    if (self.usingForcedInput)
                    {
                        self.m_playerCommandedDirection = self.forcedInput;
                    }
                    if (self.m_playerCommandedDirection != Vector2.zero)
                    {
                        GameManager.Instance.platformInterface.ProcessDlcUnlocks();
                    }
                }
                if (self.IsDodgeRolling || self.m_dodgeRollState == PlayerController.DodgeRollState.AdditionalDelay)
                {
                    self.HandleContinueDodgeRoll();
                }
                if (PassiveItem.IsFlagSetForCharacter(self, typeof(HeavyBootsItem)))
                {
                    self.knockbackComponent = Vector2.zero;
                }
                if (self.IsDodgeRolling)
                {
                    if (self.usingForcedInput)
                    {
                        self.specRigidbody.Velocity = self.forcedInput.normalized * self.GetDodgeRollSpeed() + self.knockbackComponent + self.immutableKnockbackComponent;
                    }
                    else if (self.DodgeRollIsBlink)
                    {
                        self.specRigidbody.Velocity = Vector2.zero;
                    }
                    else
                    {
                        self.specRigidbody.Velocity = self.lockedDodgeRollDirection.normalized * self.GetDodgeRollSpeed() + self.knockbackComponent + self.immutableKnockbackComponent;
                    }
                }
                else
                {
                    float num2 = 1f;
                    if (!self.IsInCombat && GameManager.Options.IncreaseSpeedOutOfCombat)
                    {
                        bool flag = true;
                        List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
                        if (allEnemies != null)
                        {
                            for (int l = 0; l < allEnemies.Count; l++)
                            {
                                AIActor aiactor = allEnemies[l];
                                if (aiactor && aiactor.IsMimicEnemy && !aiactor.IsGone)
                                {
                                    float num3 = Vector2.Distance(aiactor.CenterPosition, self.CenterPosition);
                                    if (num3 < 40f)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            num2 *= 1.5f;
                        }
                    }
                    Vector2 voluntaryVel = self.m_playerCommandedDirection * self.stats.MovementSpeed * num2;
                    Vector2 involuntaryVel = self.knockbackComponent;
                    self.specRigidbody.Velocity = self.ApplyMovementModifiers(voluntaryVel, involuntaryVel) + self.immutableKnockbackComponent;
                }
                self.specRigidbody.Velocity += self.ImpartedVelocity;
                self.ImpartedVelocity = Vector2.zero;
                if (cellFloorType == CellVisualData.CellFloorType.Ice && !self.IsFlying && !PassiveItem.IsFlagSetForCharacter(self, typeof(HeavyBootsItem)))
                {
                    self.m_maxIceFactor = Mathf.Clamp01(self.m_maxIceFactor + BraveTime.DeltaTime * 4f);
                }
                else if (self.IsFlying && !PassiveItem.IsFlagSetForCharacter(self, typeof(HeavyBootsItem)))
                {
                    self.m_maxIceFactor = 0f;
                }
                else
                {
                    self.m_maxIceFactor = Mathf.Clamp01(self.m_maxIceFactor - BraveTime.DeltaTime * 1.5f);
                }
                if (self.m_maxIceFactor > 0f)
                {
                    float max = Mathf.Max(self.m_lastVelocity.magnitude, self.specRigidbody.Velocity.magnitude);
                    float t = 1f - Mathf.Clamp01(Mathf.Abs(Vector2.Angle(self.m_lastVelocity, self.specRigidbody.Velocity)) / 180f);
                    float num4 = Mathf.Lerp(1f / BraveTime.DeltaTime, Mathf.Lerp(0.5f, 1.5f, t), self.m_maxIceFactor);
                    if (self.m_lastVelocity.magnitude < 0.25f)
                    {
                        num4 = Mathf.Min(1f / BraveTime.DeltaTime, Mathf.Max(num4 * (1f / (30f * BraveTime.DeltaTime)), num4));
                    }
                    self.specRigidbody.Velocity = Vector2.Lerp(self.m_lastVelocity, self.specRigidbody.Velocity, num4 * BraveTime.DeltaTime);
                    self.specRigidbody.Velocity = self.specRigidbody.Velocity.normalized * Mathf.Clamp(self.specRigidbody.Velocity.magnitude, 0f, max);
                    if (float.IsNaN(self.specRigidbody.Velocity.x) || float.IsNaN(self.specRigidbody.Velocity.y))
                    {
                        self.specRigidbody.Velocity = Vector2.zero;
                        UnityEngine.Debug.Log(string.Concat(new object[]
                        {
                    self.m_lastVelocity,
                    "|",
                    self.m_lastVelocity.magnitude,
                    "| NaN correction"
                        }));
                    }
                    if (self.specRigidbody.Velocity.magnitude < self.c_iceVelocityMinClamp)
                    {
                        self.specRigidbody.Velocity = Vector2.zero;
                    }
                }
                if (self.ZeroVelocityThisFrame)
                {
                    self.specRigidbody.Velocity = Vector2.zero;
                    self.ZeroVelocityThisFrame = false;
                }
                self.HandleFlipping(self.m_currentGunAngle);
                self.HandleAnimations(self.m_playerCommandedDirection, self.m_currentGunAngle);
                if (!self.IsPrimaryPlayer)
                {
                    PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(self);
                    if (otherPlayer)
                    {
                        float num5 = -0.55f;
                        float heightOffGround = self.sprite.HeightOffGround;
                        float z = otherPlayer.sprite.transform.position.z;
                        float z2 = self.sprite.transform.position.z;
                        if (z == z2)
                        {
                            if (heightOffGround == num5)
                            {
                                self.sprite.HeightOffGround = num5 + 0.1f;
                            }
                            else if (heightOffGround == num5 + 0.1f)
                            {
                                self.sprite.HeightOffGround = num5;
                            }
                            self.sprite.UpdateZDepth();
                        }
                    }
                }
                if (self.IsSlidingOverSurface)
                {
                    if (self.sprite.HeightOffGround < 0f)
                    {
                        self.sprite.HeightOffGround = 1.5f;
                    }
                }
                else if (self.sprite.HeightOffGround > 0f)
                {
                    self.sprite.HeightOffGround = ((!self.IsPrimaryPlayer) ? -0.55f : -0.5f);
                }
                self.HandleAttachedSpriteDepth(self.m_currentGunAngle);
                self.HandleShellCasingDisplacement();
                self.HandlePitChecks();
                self.HandleRoomProcessing();
                self.HandleGunAttachPoint();
                self.CheckSpawnEmergencyCrate();
                self.CheckSpawnAlertArrows();
                bool flag2 = self.QueryGroundedFrame() && !self.IsFlying;
                if (!self.m_cachedGrounded && flag2 && !self.m_isFalling && self.IsVisible)
                {
                    GameManager.Instance.Dungeon.dungeonDustups.InstantiateLandDustup(self.specRigidbody.UnitCenter);
                }
                self.m_cachedGrounded = flag2;
                if (self.m_playerCommandedDirection != Vector2.zero)
                {
                    self.m_lastNonzeroCommandedDirection = self.m_playerCommandedDirection;
                }
                self.transform.position = self.transform.position.WithZ(self.transform.position.y - self.sprite.HeightOffGround);
                if (self.CurrentGun != null)
                {
                    self.CurrentGun.transform.position = self.CurrentGun.transform.position.WithZ(self.gunAttachPoint.position.z);
                }
                if (self.CurrentSecondaryGun != null && self.SecondaryGunPivot)
                {
                    self.CurrentSecondaryGun.transform.position = self.CurrentSecondaryGun.transform.position.WithZ(self.SecondaryGunPivot.position.z);
                }
                bool flag3 = self.m_capableOfStealing.UpdateTimers(BraveTime.DeltaTime);
                if (flag3)
                {
                    self.ForceRefreshInteractable = true;
                }
                if (self.m_superDuperAutoAimTimer > 0f)
                {
                    self.m_superDuperAutoAimTimer = Mathf.Max(0f, self.m_superDuperAutoAimTimer - BraveTime.DeltaTime);
                }
            }
            else
            {
                orig(self);
            }
        }




    }
}
