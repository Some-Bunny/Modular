﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Code.Collectibles.Guns.Gravity_Pulsar
{
    internal class GravityPulsarLargeProjectile : MonoBehaviour
    {
        public GravityPulsarLargeProjectile()
        {
            this.damageRadius = 7;
            this.gravitationalForce = 25;
            this.radius = 40;
            this.radiusSquared = radius * radius;
        }
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.Player = projectile.Owner as PlayerController;
            if (this.projectile != null)
            {
                AkSoundEngine.PostEvent("Play_WPN_blackhole_loop_01", base.gameObject);
                Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 5f * ConfigManager.DistortionWaveMultiplier, 0.2f * ConfigManager.DistortionWaveMultiplier, 20, 0.5f);

                this.m_distortMaterial = new Material(ShaderCache.Acquire("Brave/Internal/DistortionRadius"));
                this.m_distortMaterial.SetFloat("_Strength", 0f);
                this.m_distortMaterial.SetFloat("_TimePulse", 0.2f);
                this.m_distortMaterial.SetFloat("_RadiusFactor", 0.5f);
                this.m_distortMaterial.SetVector("_WaveCenter", this.GetCenterPointInScreenUV(projectile.sprite.WorldCenter));
                Pixelator.Instance.RegisterAdditionalRenderPass(this.m_distortMaterial);
                var stick =  this.gameObject.GetComponent<StickyProjectileModifier>();
                if (stick != null)
                {
                    stick.OnPreStick += OPS;
                }
            }
        }

        public bool CheckModule()
        {
            if (GlobalModuleStorage.PlayerHasActiveModule(Player, IteratedDesign.ID))
            {
                return true;
            }
            return false;
        }


        public void OPS(GameObject proj, PlayerController player)
        {
            this.projectile.spriteAnimator.Stop();
            this.projectile.sprite.SetSprite(StaticCollections.Projectile_Collection, StaticCollections.Projectile_Collection.GetSpriteIdByName("fwoomp_start_002"));
            AkSoundEngine.PostEvent("Stop_WPN_blackhole_loop_01", base.gameObject);
            Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 5f * ConfigManager.DistortionWaveMultiplier, 0.2f * ConfigManager.DistortionWaveMultiplier, 20, 0.5f);

            if (Pixelator.Instance != null && this.m_distortMaterial != null)
            {
                Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_distortMaterial);
            }
            Destroy(this);
        }

        private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, 0f, 0f);
        }
        private Material m_distortMaterial;


        public void OnDestroy()
        {
            AkSoundEngine.PostEvent("Stop_WPN_blackhole_loop_01", base.gameObject);
            Exploder.DoDistortionWave(projectile.sprite.WorldCenter, 5f * ConfigManager.DistortionWaveMultiplier, 0.2f * ConfigManager.DistortionWaveMultiplier, 20, 0.5f);

            if (Pixelator.Instance != null && this.m_distortMaterial != null)
            {
                Pixelator.Instance.DeregisterAdditionalRenderPass(this.m_distortMaterial);
            }
        }
        public float e = 0;
        public void Update()
        {
            if (e < 1) { e += BraveTime.DeltaTime; this.m_distortMaterial.SetFloat("_Strength", 4 * e);            }
            if (this.projectile != null)
            {
                if (this.m_distortMaterial != null)
                {
                    this.m_distortMaterial.SetVector("_WaveCenter", this.GetCenterPointInScreenUV(projectile.sprite.WorldCenter));
                }

                for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
                {
                    if (StaticReferenceManager.AllProjectiles[i].gameObject.activeSelf)
                    {
                        if (StaticReferenceManager.AllProjectiles[i].enabled)
                        {
                            this.AdjustRigidbodyVelocity(StaticReferenceManager.AllProjectiles[i].specRigidbody);
                        }
                    }
                }
            }
        }

        public float damageRadius;
        public float gravitationalForce;
        public float radius;
        private float radiusSquared;


        private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other)
        {
            Vector2 a = other.UnitCenter - this.projectile.specRigidbody.UnitCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num < this.radiusSquared)
            {
                float g = this.gravitationalForce;
                Vector2 velocity = other.Velocity;
                Projectile projectile = other.projectile;
                if (projectile)
                {
                    if (other.GetComponent<BlackHoleDoer>() != null)
                    {
                        return false;
                    }
                    if (other.GetComponent<GravityPulsarSmallProjectile>() == null)
                    {
                        return false;
                    }
                    if (other.GetComponent<GravityPulsarLargeProjectile>() != null)
                    {
                        return false;
                    }
                    if (velocity == Vector2.zero)
                    {
                        return false;
                    }
                    g = this.gravitationalForce;
                }

                if (projectile.Owner is PlayerController)
                {
                    Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g);
                    float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                    Vector2 b = frameAccelerationForRigidbody * d;
                    Vector2 vector = velocity + b;
                    if (BraveTime.DeltaTime > 0.02f)
                    {
                        vector *= 0.02f / BraveTime.DeltaTime;
                    }
                    other.Velocity = vector;
                    if (projectile != null)
                    {

                        if (vector != Vector2.zero)
                        {
                            projectile.Direction = vector.normalized;
                            projectile.Speed = Mathf.Max(CheckModule() ? 10 : 13f, vector.magnitude);
                            other.Velocity = projectile.Direction * projectile.Speed;
                            if (projectile.shouldRotate && (vector.x != 0f || vector.y != 0f))
                            {
                                float num2 = BraveMathCollege.Atan2Degrees(projectile.Direction);
                                if (!float.IsNaN(num2) && !float.IsInfinity(num2))
                                {
                                    Quaternion rotation = Quaternion.Euler(0f, 0f, num2);
                                    if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y))
                                    {
                                        projectile.transform.rotation = rotation;
                                    }
                                }
                            }
                        }
                    }
                }
               
                return true;
            }
            return false;
        }
        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
        {
            Vector2 zero = Vector2.zero;
            float num = Mathf.Clamp01(1f - currentDistance / this.radius);
            float d = g * num * num;
            Vector2 normalized = (this.projectile.specRigidbody.UnitCenter - unitCenter).normalized;
            return (normalized * d) * 9;
        }

        private PlayerController Player;
        private Projectile projectile;
    }
    internal class GravityPulsarSmallProjectile : MonoBehaviour { }
}
