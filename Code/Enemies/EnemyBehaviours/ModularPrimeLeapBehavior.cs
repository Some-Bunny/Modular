using Dungeonator;
using FullInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ModularMod.ModularPrime;

namespace ModularMod.Code.Enemies.EnemyBehaviours
{
    //DO NOT FUCKING USE THIS BEHAVIOR IT IS SHIT AND JANK
    public class ModularPrimeLeapBehavior : BasicAttackBehavior
    {
        private bool shitFart;
        private bool ShowBulletScript()
        {
            return string.IsNullOrEmpty(this.BulletName);
        }

        private bool ShowBulletName()
        {
            return this.BulletScript == null || this.BulletScript.IsNull;
        }

        private bool ShowImmobileDuringStop()
        {
            return this.StopDuring != ModularPrimeLeapBehavior.StopType.None;
        }

        private bool ShowChargeTime()
        {
            return !string.IsNullOrEmpty(this.ChargeAnimation);
        }

        private bool ShowOverrideFireDirection()
        {
            return this.ShowBulletName() && this.ShouldOverrideFireDirection;
        }

        public bool IsBulletScript
        {
            get
            {
                return this.BulletScript != null && !string.IsNullOrEmpty(this.BulletScript.scriptTypeName);
            }
        }

        public bool IsSingleBullet
        {
            get
            {
                return !string.IsNullOrEmpty(this.BulletName);
            }
        }

        public override void Start()
        {
            base.Start();
            controller = this.m_aiActor.GetComponent<ModularPrimeController>();
            if (this.SpecifyAiAnimator)
            {
                this.m_aiAnimator = this.SpecifyAiAnimator;
            }
            if (!string.IsNullOrEmpty(this.TellAnimation))
            {
                tk2dSpriteAnimator spriteAnimator = this.m_aiAnimator.spriteAnimator;
                spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered));
                if (this.m_aiAnimator.ChildAnimator)
                {
                    tk2dSpriteAnimator spriteAnimator2 = this.m_aiAnimator.ChildAnimator.spriteAnimator;
                    spriteAnimator2.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator2.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered));
                }
            }
            GlobalMessageRadio.RegisterObjectToRadio(this.m_aiActor.gameObject, new List<string>() { "LANDYOUFUCK" }, OnRecieveMessage);
        }

        public void OnRecieveMessage(GameObject obj, string message)
        {
            STOPYOUCUNT = true;
        }

        private ModularPrimeController controller;

        public override void Upkeep()
        {
            base.Upkeep();
            if (this.state == ModularPrimeLeapBehavior.State.WaitingForCharge)
            {
                base.DecrementTimer(ref this.m_chargeTimer, false);
            }
        }

        private bool STOPYOUCUNT = false;

        public IEnumerator Takeoff()
        {
            FuckingDie = false;
            shitFart = false;
            cached_position = this.m_aiActor.transform.PositionVector2();

            cached_roomHandler = this.m_aiActor.GetAbsoluteParentRoom();
            this.m_aiActor.behaviorSpeculator.PreventMovement = true;

            this.m_cachedMovementSpeed = this.m_aiActor.MovementSpeed;
            this.m_aiActor.MovementSpeed = 0;

            controller.predictedPosition = cached_position;

            float elaWait = 0f;
            float duraWait = 1.1f;
            while (elaWait < duraWait)
            {
                if (STOPYOUCUNT == true) { this.m_aiActor.transform.position = cached_position;  yield break; }
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            this.m_aiActor.healthHaver.IsVulnerable = false;
            this.m_aiActor.specRigidbody.enabled = false;

            elaWait = 0f;
            duraWait = 1f;
            Vector3 pos = this.m_aiActor.transform.position;
            AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", this.m_aiActor.gameObject);
            AkSoundEngine.PostEvent("Play_BOSS_doormimic_jump_01", this.m_aiActor.gameObject);
            ExplosionData explosionData = new ExplosionData();
            explosionData.CopyFrom(StaticExplosionDatas.customDynamiteExplosion);
            explosionData.ignoreList = new List<SpeculativeRigidbody>() { this.m_aiActor.specRigidbody };
            Exploder.Explode(this.m_aiActor.sprite.WorldBottomCenter, explosionData, this.m_aiActor.sprite.WorldBottomCenter);
            Exploder.DoDistortionWave(this.m_aiActor.sprite.WorldBottomCenter, 2f * ConfigManager.DistortionWaveMultiplier, 0.5f * ConfigManager.DistortionWaveMultiplier, 30, 1f);


            while (elaWait < duraWait)
            {
                if (STOPYOUCUNT == true) { this.m_aiActor.transform.position = cached_position; yield break; }
                float t = Mathf.Min(elaWait, 1);
                float tRue = Toolbox.SinLerpTValue(t);
                this.m_aiActor.transform.position = Vector3.Lerp(cached_position, cached_position + new Vector2(0, 30), tRue);
                this.m_aiActor.specRigidbody.Reinitialize();
                elaWait += BraveTime.DeltaTime;

                yield return null;
            }
            this.m_aiActor.sprite.renderer.enabled = false;
            this.m_aiActor.StartCoroutine(DoTrack());
            yield break;
        }


        public RoomHandler cached_roomHandler;

        public Vector2 cached_position;

        private bool FuckingDie = false;

        public IEnumerator TakeOn()
        {
            if (FuckingDie == true) { yield break; }
            FuckingDie = true;
            this.m_aiActor.sprite.renderer.enabled = true;

            float elaWait = 0f;
            float duraWait = 0.5f;

            this.m_aiAnimator.Play(this.PostFireAnimation, AIAnimator.AnimatorState.StateEndType.Duration, 2, -1, false, null);

            AkSoundEngine.PostEvent("Play_ANM_Gull_Descend_01", GameManager.Instance.PrimaryPlayer.gameObject);
            AkSoundEngine.PostEvent("Play_ANM_Gull_Descend_01", GameManager.Instance.PrimaryPlayer.gameObject);
            Vector3 pos = cached_position + new Vector2(0, 50);
            while (elaWait < duraWait)
            {
                if (STOPYOUCUNT == true) { this.m_aiActor.transform.position = cached_position; yield break; }
                float t = (float)elaWait / (float)duraWait;
                Vector3 vector3 = Vector3.Lerp(pos - new Vector3(1, 1), cached_position - new Vector2(1, 1), t);
                this.m_aiActor.transform.position = vector3;
                this.m_aiActor.specRigidbody.Reinitialize();
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            controller.ResetPredictedPos();
            AkSoundEngine.PostEvent("Play_enm_mech_death_01", GameManager.Instance.PrimaryPlayer.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_elevator_arrive_01", GameManager.Instance.PrimaryPlayer.gameObject);
            ExplosionData explosionData = new ExplosionData();
            explosionData.CopyFrom(StaticExplosionDatas.customDynamiteExplosion);
            explosionData.ignoreList = new List<SpeculativeRigidbody>() { this.m_aiActor.specRigidbody };
            explosionData.doDestroyProjectiles = false;
            Exploder.Explode(this.m_aiActor.sprite.WorldBottomCenter, explosionData, this.m_aiActor.sprite.WorldBottomCenter);
            Exploder.DoDistortionWave(this.m_aiActor.sprite.WorldBottomCenter, 2f * ConfigManager.DistortionWaveMultiplier, 0.5f * ConfigManager.DistortionWaveMultiplier, 30, 1f);


            this.m_aiActor.behaviorSpeculator.PreventMovement = false;
            this.m_aiActor.healthHaver.IsVulnerable = true;
            this.m_aiActor.specRigidbody.enabled = true;
            this.m_aiActor.MovementSpeed = m_cachedMovementSpeed;
            this.Fire();

            elaWait = 0f;
            duraWait = 1f;
            while (elaWait < duraWait)
            {
                if (STOPYOUCUNT == true) { this.m_aiActor.transform.position = cached_position; yield break; }
                elaWait += BraveTime.DeltaTime;
                yield return null;
            }
            yield break;
        }


        private void M_aiActor_MovementModifiers(ref Vector2 volundaryVel, ref Vector2 involuntaryVel)
        {
            throw new NotImplementedException();
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
            {
                return behaviorResult;
            }
            if (!this.IsReady())
            {
                return BehaviorResult.Continue;
            }
            if (this.RequiresTarget && this.m_behaviorSpeculator.TargetRigidbody == null)
            {
                return BehaviorResult.Continue;
            }
            if (this.UseVfx && !string.IsNullOrEmpty(this.Vfx))
            {
                this.m_aiAnimator.PlayVfx(this.Vfx, null, null, null);
            }
            if (!this.m_gameObject.activeSelf)
            {
                this.m_gameObject.SetActive(true);
                this.m_beganInactive = true;
            }
            if (this.m_behaviorSpeculator.TargetRigidbody)
            {
                this.m_cachedTargetCenter = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
            }
            if (this.ClearGoop)
            {
                this.SetGoopClearing(true);
            }
            this.state = ModularPrimeLeapBehavior.State.Idle;
            if (!string.IsNullOrEmpty(this.ChargeAnimation))
            {

                this.m_aiActor.StartCoroutine(Takeoff());
                this.m_aiAnimator.PlayUntilFinished(this.ChargeAnimation, true, null, -1f, false);
                this.state = ModularPrimeLeapBehavior.State.WaitingForCharge;
            }
            else if (!string.IsNullOrEmpty(this.TellAnimation))
            {
                if (!string.IsNullOrEmpty(this.TellAnimation))
                {
                    this.m_aiAnimator.PlayUntilCancelled(this.TellAnimation, true, null, -1f, false);
                }
                else
                {
                    this.m_aiAnimator.PlayUntilFinished(this.TellAnimation, true, null, -1f, false);
                }
                this.state = ModularPrimeLeapBehavior.State.WaitingForTell;
                if (this.HideGun && this.m_aiShooter)
                {
                    this.m_aiShooter.ToggleGunAndHandRenderers(false, "ShootBulletScript");
                }
            }
            else
            {
            }
            if (this.MoveSpeedModifier != 1f)
            {
                this.m_cachedMovementSpeed = this.m_aiActor.MovementSpeed;
                this.m_aiActor.MovementSpeed *= this.MoveSpeedModifier;
            }
            if (this.LockFacingDirection)
            {
                this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
                this.m_aiAnimator.LockFacingDirection = true;
            }
            if (this.PreventTargetSwitching && this.m_aiActor)
            {
                this.m_aiActor.SuppressTargetSwitch = true;
            }
            this.m_updateEveryFrame = true;
            if (this.OverrideBaseAnims && this.m_aiAnimator)
            {
                if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
                {
                    this.m_aiAnimator.OverrideIdleAnimation = this.OverrideIdleAnim;
                }
                if (!string.IsNullOrEmpty(this.OverrideMoveAnim))
                {
                    this.m_aiAnimator.OverrideMoveAnimation = this.OverrideMoveAnim;
                }
            }
            if (this.StopDuring == ModularPrimeLeapBehavior.StopType.None || this.StopDuring == ModularPrimeLeapBehavior.StopType.TellOnly)
            {
                return BehaviorResult.RunContinuousInClass;
            }
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            base.ContinuousUpdate();
            if (this.m_behaviorSpeculator.TargetRigidbody)
            {
                this.m_cachedTargetCenter = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
            }
            if (this.state == ModularPrimeLeapBehavior.State.WaitingForCharge)
            {
                if ((this.ChargeTime > 0f && this.m_chargeTimer <= 0f) || (this.ChargeTime <= 0f && !this.m_aiAnimator.IsPlaying(this.ChargeAnimation)))
                {
                    if (!string.IsNullOrEmpty(this.TellAnimation))
                    {
                        this.m_aiAnimator.PlayUntilFinished(this.TellAnimation, true, null, -1f, false);
                        this.state = ModularPrimeLeapBehavior.State.WaitingForTell;
                    }
                    else
                    {
                    }
                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.state == ModularPrimeLeapBehavior.State.WaitingForTell)
            {
                if (this.LockFacingDirection && this.ContinueAimingDuringTell && !this.m_isAimLocked && this.m_behaviorSpeculator.TargetRigidbody)
                {
                    this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
                }
                if (!this.m_aiAnimator.IsPlaying(this.TellAnimation))
                {
                }
                return ContinuousBehaviorResult.Continue;
            }
            if (this.state == ModularPrimeLeapBehavior.State.Firing)
            {
                if (!this.JumpEnded)
                {
                    return ContinuousBehaviorResult.Continue;
                }
                tk2dSpriteAnimationClip.WrapMode wrapMode;
                if (!string.IsNullOrEmpty(this.TellAnimation) && this.m_aiAnimator.IsPlaying(this.TellAnimation) && this.m_aiAnimator.GetWrapType(this.TellAnimation, out wrapMode) && wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
                {
                    return ContinuousBehaviorResult.Continue;
                }
                if (!string.IsNullOrEmpty(this.FireAnimation) && this.m_aiAnimator.IsPlaying(this.FireAnimation) && this.m_aiAnimator.GetWrapType(this.FireAnimation, out wrapMode) && wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
                {
                    return ContinuousBehaviorResult.Continue;
                }
                return ContinuousBehaviorResult.Finished;
            }
            else
            {
                return ContinuousBehaviorResult.Finished;
            }
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.CeaseFire();
            this.m_aiActor.StartCoroutine(TakeOn());

            if (this.ClearGoop)
            {
                this.SetGoopClearing(false);
            }
            if (this.HideGun && this.m_aiShooter)
            {
                this.m_aiShooter.ToggleGunAndHandRenderers(true, "ShootBulletScript");
            }
            if (!string.IsNullOrEmpty(this.ChargeAnimation))
            {
                this.m_aiAnimator.EndAnimationIf(this.ChargeAnimation);
            }
            if (!string.IsNullOrEmpty(this.TellAnimation))
            {
                this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
            }
            if (!string.IsNullOrEmpty(this.FireAnimation))
            {
                this.m_aiAnimator.EndAnimationIf(this.FireAnimation);
            }
            if (this.UseVfx && !string.IsNullOrEmpty(this.Vfx))
            {
                this.m_aiAnimator.StopVfx(this.Vfx);
            }
            if (this.UseVfx && !string.IsNullOrEmpty(this.ChargeVfx))
            {
                this.m_aiAnimator.StopVfx(this.ChargeVfx);
            }
            if (this.UseVfx && !string.IsNullOrEmpty(this.TellVfx))
            {
                this.m_aiAnimator.StopVfx(this.TellVfx);
            }
            if (this.UseVfx && !string.IsNullOrEmpty(this.FireVfx))
            {
                this.m_aiAnimator.StopVfx(this.FireVfx);
            }
            if (this.EnabledDuringAttack != null)
            {
                for (int i = 0; i < this.EnabledDuringAttack.Length; i++)
                {
                    this.EnabledDuringAttack[i].SetActive(false);
                }
            }
            if (this.m_beganInactive)
            {
                this.m_aiAnimator.gameObject.SetActive(false);
                this.m_beganInactive = false;
            }
            if (this.MoveSpeedModifier != 1f)
            {
                this.m_aiActor.MovementSpeed = this.m_cachedMovementSpeed;
            }
            if (this.m_aiActor && this.StopDuring != ModularPrimeLeapBehavior.StopType.None && this.ImmobileDuringStop)
            {
                this.m_aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
            }
            if (this.LockFacingDirection)
            {
                this.m_aiAnimator.LockFacingDirection = false;
            }
            if (this.PreventTargetSwitching && this.m_aiActor)
            {
                this.m_aiActor.SuppressTargetSwitch = false;
            }
            if (this.OverrideBaseAnims && this.m_aiAnimator)
            {
                if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
                {
                    this.m_aiAnimator.OverrideIdleAnimation = null;
                }
                if (!string.IsNullOrEmpty(this.OverrideMoveAnim))
                {
                    this.m_aiAnimator.OverrideMoveAnimation = null;
                }
            }



            this.m_updateEveryFrame = false;
            this.state = ModularPrimeLeapBehavior.State.Idle;
            this.UpdateCooldowns();
        }

        public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
        {
            base.Init(gameObject, aiActor, aiShooter);
            this.m_specRigidbody = this.m_behaviorSpeculator.specRigidbody;
            this.m_bulletBank = this.m_behaviorSpeculator.bulletBank;
        }

        public override bool IsOverridable()
        {
            return !this.Uninterruptible;
        }

        private void Fire()
        {
            if (this.LockFacingDirection && this.ReaimOnFire && this.m_behaviorSpeculator.TargetRigidbody)
            {
                this.m_aiAnimator.FacingDirection = (this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
            }
            if (!string.IsNullOrEmpty(this.FireAnimation))
            {
                this.m_aiAnimator.EndAnimation();
                this.m_aiAnimator.PlayUntilFinished(this.FireAnimation, false, null, -1f, false);
            }
            if (this.UseVfx && !string.IsNullOrEmpty(this.FireVfx))
            {
                this.m_aiAnimator.PlayVfx(this.FireVfx, null, null, null);
            }
            this.SpawnProjectiles();
            if (this.EnabledDuringAttack != null)
            {
                for (int i = 0; i < this.EnabledDuringAttack.Length; i++)
                {
                    this.EnabledDuringAttack[i].SetActive(true);
                }
            }
            if (this.StopDuring == ModularPrimeLeapBehavior.StopType.TellOnly)
            {
                if (this.m_aiActor && this.ImmobileDuringStop)
                {
                    this.m_aiActor.knockbackDoer.SetImmobile(false, "ShootBulletScript");
                }
            }

            this.state = ModularPrimeLeapBehavior.State.Firing;
            if (this.HideGun && this.m_aiShooter)
            {
                this.m_aiShooter.ToggleGunAndHandRenderers(false, "ShootBulletScript");
            }
        }

        private void CeaseFire()
        {
            if (this.IsBulletScript && this.m_bulletSource && !this.m_bulletSource.IsEnded)
            {
                this.m_bulletSource.ForceStop();
            }
        }



        public override Vector2 GetOrigin(ShootBehavior.TargetAreaOrigin origin)
        {
            if (origin == ShootBehavior.TargetAreaOrigin.ShootPoint)
            {
                return this.ShootPoint.transform.position.XY();
            }
            return base.GetOrigin(origin);
        }

        private void SpawnProjectiles()
        {
            if (this.IsBulletScript)
            {
                if (!this.m_bulletSource)
                {
                    this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
                }
                this.m_bulletSource.BulletManager = this.m_bulletBank;
                this.m_bulletSource.BulletScript = this.BulletScript;
                this.m_bulletSource.Initialize();
                return;
            }
            if (this.IsSingleBullet)
            {
                AIBulletBank.Entry bullet = this.m_bulletBank.GetBullet(this.BulletName);
                GameObject bulletObject = bullet.BulletObject;
                Vector2 vector = this.m_cachedTargetCenter;
                if (this.m_behaviorSpeculator.TargetRigidbody)
                {
                    vector = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
                }
                float direction;
                if (this.ShouldOverrideFireDirection)
                {
                    direction = this.OverrideFireDirection;
                }
                else
                {
                    if (this.LeadAmount > 0f)
                    {
                        Vector2 value = this.ShootPoint.transform.position;
                        float? overrideProjectileSpeed = (!bullet.OverrideProjectile) ? null : new float?(bullet.ProjectileData.speed);
                        Projectile component = bulletObject.GetComponent<Projectile>();
                        Vector2 predictedTargetPosition = component.GetPredictedTargetPosition(vector, this.m_behaviorSpeculator.TargetVelocity, new Vector2?(value), overrideProjectileSpeed);
                        vector = Vector2.Lerp(vector, predictedTargetPosition, this.LeadAmount);
                    }
                    Vector2 vector2 = vector - this.ShootPoint.transform.position.XY();
                    direction = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
                }
                GameObject gameObject = this.m_bulletBank.CreateProjectileFromBank(this.ShootPoint.transform.position, direction, this.BulletName, null, false, true, false);
                if (this.m_bulletBank.OnProjectileCreatedWithSource != null)
                {
                    this.m_bulletBank.OnProjectileCreatedWithSource(this.ShootPoint.transform.name, gameObject.GetComponent<Projectile>());
                }
                ArcProjectile component2 = gameObject.GetComponent<ArcProjectile>();
                if (component2)
                {
                    component2.AdjustSpeedToHit(vector);
                }
            }
        }


        public float FlightTime = 3;
        public float TrackingSpeedMultiplier = 1;


        public IEnumerator DoTrack()
        {
            Vector3 startsPosition = cached_position;
            var obj = UnityEngine.Object.Instantiate(ModularPrime.VFXObject, startsPosition, Quaternion.Euler(0, 0, 0));
            obj.GetComponent<tk2dSpriteAnimator>().Play("targetreticle_target");
            obj.GetComponent<tk2dBaseSprite>().ShouldDoTilt = false;

            obj.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            obj.AddComponent<FakeCorridor.Fuck_You_Youre_No_Longer_Perpendicular>();
            float m = 1;
            float e = 0;
            while (e < FlightTime)
            {
                float gjkgj = e - (0.25f);
                float t = Mathf.Min(1, gjkgj / 4);
                Vector3 centerPoint = startsPosition;
                float a = (this.m_aiActor.bulletBank.PlayerPosition() - new Vector2(centerPoint.x, centerPoint.y)).ToAngle();
                float playerDist = (this.m_aiActor.bulletBank.PlayerPosition().ToVector3XUp(0) - startsPosition).magnitude;
                float dist = Mathf.Min(1, (Mathf.Min(12, playerDist) / 40));
                dist *= t;
                dist *= m;
                if (e > (FlightTime - 0.5f))
                {
                    float ads = e - (FlightTime - 0.5f);
                    float t2 = (float)ads / (float)0.5f;
                    m = Mathf.Lerp(1, 0, Toolbox.SinLerpTValue(t2));
                }
                startsPosition = centerPoint + BraveMathCollege.DegreesToVector(a, dist * TrackingSpeedMultiplier).ToVector3ZUp();
                startsPosition.WithZ(-10);
                obj.transform.position = startsPosition;
                cached_position = startsPosition;
                controller.predictedPosition = startsPosition;
                e += BraveTime.DeltaTime;
                yield return null;
            }
            cached_position = startsPosition;
            obj.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("targetreticle_break");
            shitFart = true;
            this.EndContinuousUpdate();
            yield break;
        }


        private void SetGoopClearing(bool value)
        {
            if (!this.ClearGoop || !this.m_aiActor || !this.m_aiActor.specRigidbody)
            {
                return;
            }
            if (value)
            {
                this.m_goopExceptionId = DeadlyDeadlyGoopManager.RegisterUngoopableCircle(this.m_aiActor.specRigidbody.UnitCenter, 2f);
            }
            else
            {
                if (this.m_goopExceptionId != -1)
                {
                    DeadlyDeadlyGoopManager.DeregisterUngoopableCircle(this.m_goopExceptionId);
                }
                this.m_goopExceptionId = -1;
            }
        }

        public bool JumpEnded
        {
            get
            {
                return shitFart;
            }
        }
        public bool BulletScriptEnded
        {
            get
            {
                if (this.IsBulletScript)
                {
                    return this.m_bulletSource.IsEnded;
                }
                return !this.IsSingleBullet || true;
            }
        }

        private ModularPrimeLeapBehavior.State state
        {
            get
            {
                return this.m_state;
            }
            set
            {
                if (this.m_state != value)
                {
                    this.EndState(this.m_state);
                    this.m_state = value;
                    this.BeginState(this.m_state);
                }
            }
        }

        private void BeginState(ModularPrimeLeapBehavior.State state)
        {
            if (state == ModularPrimeLeapBehavior.State.WaitingForCharge)
            {
                if (this.UseVfx && !string.IsNullOrEmpty(this.ChargeVfx))
                {
                    this.m_aiAnimator.PlayVfx(this.ChargeVfx, null, null, null);
                }

                this.m_chargeTimer = this.ChargeTime;
            }
            else if (state == ModularPrimeLeapBehavior.State.WaitingForTell)
            {
                if (this.UseVfx && !string.IsNullOrEmpty(this.TellVfx))
                {
                    this.m_aiAnimator.PlayVfx(this.TellVfx, null, null, null);
                }


                this.m_isAimLocked = false;
            }
        }

        private void EndState(ModularPrimeLeapBehavior.State state)
        {
            if (state == ModularPrimeLeapBehavior.State.WaitingForCharge)
            {
                if (this.UseVfx && !string.IsNullOrEmpty(this.ChargeVfx))
                {
                    this.m_aiAnimator.StopVfx(this.ChargeVfx);
                }
            }
            else if (state == ModularPrimeLeapBehavior.State.WaitingForTell)
            {
                if (this.UseVfx && !string.IsNullOrEmpty(this.TellVfx))
                {
                    this.m_aiAnimator.StopVfx(this.TellVfx);
                }
                if (this.OverrideBaseAnims)
                {
                    if (!string.IsNullOrEmpty(this.OverrideIdleAnim))
                    {
                        this.m_aiAnimator.OverrideIdleAnimation = this.OverrideIdleAnim;
                    }
                    if (!string.IsNullOrEmpty(this.OverrideMoveAnim))
                    {
                        this.m_aiAnimator.OverrideMoveAnimation = this.OverrideMoveAnim;
                    }
                    if (!string.IsNullOrEmpty(this.TellAnimation))
                    {
                        this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
                    }
                }
            }
            else if (state == ModularPrimeLeapBehavior.State.Firing && this.UseVfx && !string.IsNullOrEmpty(this.FireVfx))
            {
                this.m_aiAnimator.StopVfx(this.FireVfx);
            }
        }

        private void AnimEventTriggered(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
        {
            tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
            bool flag = this.state == ModularPrimeLeapBehavior.State.WaitingForTell;
            if (this.MultipleFireEvents)
            {
                flag |= (this.state == ModularPrimeLeapBehavior.State.Firing);
            }

            if (this.LockFacingDirection && this.ContinueAimingDuringTell && frame.eventInfo == "stopAiming")
            {
                this.m_isAimLocked = true;
            }
        }

        public GameObject ShootPoint;

        [InspectorShowIf("ShowBulletScript")]
        public BulletScriptSelector BulletScript;

        [InspectorShowIf("ShowBulletName")]
        public string BulletName;

        [InspectorShowIf("IsSingleBullet")]
        public float LeadAmount;

        public ModularPrimeLeapBehavior.StopType StopDuring;

        [InspectorShowIf("ShowImmobileDuringStop")]
        public bool ImmobileDuringStop;

        public float MoveSpeedModifier = 1f;

        public bool LockFacingDirection;

        [InspectorIndent]
        [InspectorShowIf("LockFacingDirection")]
        public bool ContinueAimingDuringTell;

        [InspectorIndent]
        [InspectorShowIf("LockFacingDirection")]
        public bool ReaimOnFire;

        public bool MultipleFireEvents;

        public bool RequiresTarget = true;

        public bool PreventTargetSwitching;

        public bool Uninterruptible;

        public bool ClearGoop;

        [InspectorIndent]
        [InspectorShowIf("ClearGoop")]
        public float ClearGoopRadius = 2f;

        public bool ShouldOverrideFireDirection;

        [InspectorIndent]
        [InspectorShowIf("ShowOverrideFireDirection")]
        public float OverrideFireDirection;

        [InspectorCategory("Visuals")]
        public AIAnimator SpecifyAiAnimator;

        [InspectorCategory("Visuals")]
        public string ChargeAnimation;

        [InspectorCategory("Visuals")]
        [InspectorShowIf("ShowChargeTime")]
        public float ChargeTime;

        [InspectorCategory("Visuals")]
        public string TellAnimation;

        [InspectorCategory("Visuals")]
        public string FireAnimation;

        // Token: 0x04003B2F RID: 15151
        [InspectorCategory("Visuals")]
        public string PostFireAnimation;

        // Token: 0x04003B30 RID: 15152
        [InspectorCategory("Visuals")]
        public bool HideGun = true;

        // Token: 0x04003B31 RID: 15153
        [InspectorCategory("Visuals")]
        public bool OverrideBaseAnims;

        // Token: 0x04003B32 RID: 15154
        [InspectorShowIf("OverrideBaseAnims")]
        [InspectorIndent]
        [InspectorCategory("Visuals")]
        public string OverrideIdleAnim;

        // Token: 0x04003B33 RID: 15155
        [InspectorIndent]
        [InspectorCategory("Visuals")]
        [InspectorShowIf("OverrideBaseAnims")]
        public string OverrideMoveAnim;

        // Token: 0x04003B34 RID: 15156
        [InspectorCategory("Visuals")]
        public bool UseVfx;

        // Token: 0x04003B35 RID: 15157
        [InspectorCategory("Visuals")]
        [InspectorShowIf("UseVfx")]
        [InspectorIndent]
        public string ChargeVfx;

        // Token: 0x04003B36 RID: 15158
        [InspectorShowIf("UseVfx")]
        [InspectorCategory("Visuals")]
        [InspectorIndent]
        public string TellVfx;

        // Token: 0x04003B37 RID: 15159
        [InspectorCategory("Visuals")]
        [InspectorShowIf("UseVfx")]
        [InspectorIndent]
        public string FireVfx;

        // Token: 0x04003B38 RID: 15160
        [InspectorIndent]
        [InspectorCategory("Visuals")]
        [InspectorShowIf("UseVfx")]
        public string Vfx;

        // Token: 0x04003B39 RID: 15161
        [InspectorCategory("Visuals")]
        public GameObject[] EnabledDuringAttack;

        // Token: 0x04003B3A RID: 15162
        private SpeculativeRigidbody m_specRigidbody;

        // Token: 0x04003B3B RID: 15163
        private AIBulletBank m_bulletBank;

        // Token: 0x04003B3C RID: 15164
        private BulletScriptSource m_bulletSource;

        // Token: 0x04003B3D RID: 15165
        private float m_chargeTimer;

        // Token: 0x04003B3E RID: 15166
        private bool m_beganInactive;

        // Token: 0x04003B3F RID: 15167
        private bool m_isAimLocked;

        // Token: 0x04003B40 RID: 15168
        private float m_cachedMovementSpeed;

        // Token: 0x04003B41 RID: 15169
        private Vector2 m_cachedTargetCenter;

        // Token: 0x04003B42 RID: 15170
        private int m_goopExceptionId = -1;

        private ModularPrimeLeapBehavior.State m_state;

        public enum StopType
        {
            None,
            Tell,
            Attack,
            Charge,
            TellOnly
        }

        private enum State
        {
            Idle,
            WaitingForCharge,
            WaitingForTell,
            Firing,
            WaitingForPostAnim
        }

        public enum TargetAreaOrigin
        {
            HitboxCenter,
            ShootPoint
        }

        public abstract class FiringAreaStyle
        {
            public abstract bool TargetInFiringArea(Vector2 origin, Vector2 targetCenter);

            public abstract void DrawDebugLines(Vector2 origin, Vector2 targetCenter, AIActor actor);

            public ShootBehavior.TargetAreaOrigin targetAreaOrigin;
        }

        public class ArcFiringArea : ShootBehavior.FiringAreaStyle
        {
            public override bool TargetInFiringArea(Vector2 origin, Vector2 targetCenter)
            {
                return BraveMathCollege.IsAngleWithinSweepArea((targetCenter - origin).ToAngle(), this.StartAngle, this.SweepAngle);
            }

            public override void DrawDebugLines(Vector2 origin, Vector2 targetCenter, AIActor actor)
            {
                BasicAttackBehavior.m_arcCount++;
            }

            public float StartAngle;

            public float SweepAngle;
        }

        public class RectFiringArea : ShootBehavior.FiringAreaStyle
        {

            private Vector2 offset
            {
                get
                {
                    Vector2 areaOriginOffset = this.AreaOriginOffset;
                    if (this.AreaDimensions.x < 0f)
                    {
                        areaOriginOffset.x += this.AreaDimensions.x;
                    }
                    if (this.AreaDimensions.y < 0f)
                    {
                        areaOriginOffset.y += this.AreaDimensions.y;
                    }
                    return areaOriginOffset;
                }
            }

            private Vector2 dimensions
            {
                get
                {
                    return new Vector2(Mathf.Abs(this.AreaDimensions.x), Mathf.Abs(this.AreaDimensions.y));
                }
            }

            public override bool TargetInFiringArea(Vector2 origin, Vector2 targetCenter)
            {
                origin += this.offset;
                return targetCenter.x >= origin.x && targetCenter.x <= origin.x + this.dimensions.x && targetCenter.y >= origin.y && targetCenter.y <= origin.y + this.dimensions.y;
            }

            public override void DrawDebugLines(Vector2 origin, Vector2 targetCenter, AIActor actor)
            {
                origin += this.offset;
            }

            public Vector2 AreaOriginOffset;

            public Vector2 AreaDimensions;
        }
    }

}
