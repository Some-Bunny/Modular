using MonoMod.RuntimeDetour.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Code.Components.Projectile_Components
{



    public class CritCancel : MonoBehaviour { }
    public class CriticalHitComponent : MonoBehaviour
    {
        public static ParticleSystem critParticles;
        public static GameObject SpawnEffect;
        public static GameObject DestroyEffect;


        public PlayerController player;

        public Projectile Projectile;
        private ParticleSystem particleObject;

        public Action<Projectile, PlayerController> OnCritProc;
        public Action<Projectile, PlayerController> OnCritFailed;

        public Action<Projectile, PlayerController,AIActor> OnCritHitEnemy;
        public Action<Projectile, PlayerController, PhysicsEngine.Tile> OnCritHitWall;

        public Action<Projectile, PlayerController> OnCritDestroyed;

        public static void Init()
        {
            critParticles = ModularMod.Module.ModularAssetBundle.LoadAsset<GameObject>("CritParticle").GetComponent<ParticleSystem>();
            SpawnEffect = (PickupObjectDatabase.GetById(365) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapVertical.effects.First().effects.First().effect;
        }
        

        public float DamageMult;

        public List<CritContext> critContexts = new List<CritContext>()
        {

        };
        private float S = 0.2f;
        public bool Process(bool nextShotCrit)
        {
            S = Mathf.Max(0.1f, 0.5f * ConfigManager.ImportantVFXIntensity.Value);
            Projectile = Projectile ?? this.gameObject.GetComponent<Projectile>();
            if (Projectile == null) { return false; }
            player = (Projectile.Owner as PlayerController) ?? player;

            lastStoredPosition = Projectile.sprite.WorldCenter;
            float chance = 0;
            float Damage = 1.1f;

            bool isGuaranteed = nextShotCrit;
            foreach (var entry in critContexts)
            {
                if (entry.isGuaranteedCrit == true) { isGuaranteed = true; }


                chance += entry.CritChance;
                if (entry.CritChanceMultiplier != null)
                {
                    chance *= entry.CritChanceMultiplier.Value;
                }

                if (entry.GuaranteedCritCalc != null) { isGuaranteed = entry.GuaranteedCritCalc(chance); }

                Damage *= entry.CritDamageMultiplier ?? 1;
                Damage += entry.CritDamageAddon ?? 0;

            }
            foreach (var entry in critContexts)
            {
                if (entry.CritChanceCalc != null) { chance = entry.CritChanceCalc(chance); }
                if (entry.CritDamageCalc != null) { Damage = entry.CritDamageCalc(Damage); }

            }

            bool CanCrit = isGuaranteed == true | chance > UnityEngine.Random.value;

            if (CanCrit == true && this.Projectile.gameObject.GetComponent<CritCancel>() == null)
            {
                if (OnCritProc != null)
                {
                    OnCritProc(Projectile, player);
                }

                GameObject vfx = SpawnManager.SpawnVFX(SpawnEffect, true);
                vfx.transform.position = Projectile.sprite.WorldCenter;
                vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;
                vfx.transform.localScale *= 0.3f;
                UnityEngine.Object.Destroy(vfx, 1);

                AkSoundEngine.PostEvent("Play_CriticalHit", this.gameObject);
                Projectile.ignoreDamageCaps = true;
                Projectile.baseData.damage *= Damage;


                Projectile.baseData.speed *= 1.15f;
                Projectile.baseData.force *= 1.5f;

                Projectile.PoisonApplyChance *= 1.3f;
                Projectile.BleedApplyChance *= 1.3f;
                Projectile.CharmApplyChance *= 1.3f;
                Projectile.FireApplyChance *= 1.3f;
                Projectile.FreezeApplyChance *= 1.3f;
                Projectile.SpeedApplyChance *= 1.3f;
                Projectile.CheeseApplyChance *= 1.3f;

                Projectile.BlackPhantomDamageMultiplier *= 1.1f;
                Projectile.damagesWalls = true;
                Projectile.BossDamageMultiplier *= 1.05f;

                Projectile.UpdateSpeed();

                particleObject = UnityEngine.Object.Instantiate(critParticles).GetComponent<ParticleSystem>();
                Projectile.specRigidbody.OnPreRigidbodyCollision += (myRigidbody, myPixelCollider, otherRigidbody, otherPixelCollider) => {
                    HandleHit(Projectile, otherRigidbody);
                };

                Projectile.specRigidbody.OnPreTileCollision += (myRigidbody, myPixelCollider, Tile, TilePixelCollider) =>
                {
                    HandleHit(Projectile, null, Tile);
                };
                return true;
            }
            else
            {
                if (OnCritFailed != null)
                {
                    OnCritFailed(Projectile, player);
                }
                Destroy(this);
                return false;
            }
        }
        private bool CanVFX = true;
        public void C()
        {
            CanVFX = true;
        }

        public void Start()
        {
        }

        private void HandleHit(Projectile projectile, SpeculativeRigidbody otherBody, PhysicsEngine.Tile tile = null)
        {
            if (projectile)
            {
                if (ConfigManager.DoVisualEffect == true && CanVFX == true)
                {
                    CanVFX = false;
                    this.Invoke("C", 1.25f);
                    GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                    GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, Projectile.sprite.WorldCenter, Quaternion.identity);
                    blankObj.transform.localScale = Vector3.one * 0.35f;
                    Destroy(blankObj, 2f);
                }

                if (otherBody == null && tile != null && OnCritHitWall != null)
                {
                    OnCritHitWall(Projectile, player,  tile);
                }
                if (otherBody != null && otherBody.aiActor != null && !otherBody.healthHaver.IsDead && otherBody.aiActor.behaviorSpeculator && !otherBody.aiActor.IsHarmlessEnemy && OnCritHitEnemy != null)
                {
                    OnCritHitEnemy(Projectile, player, otherBody.aiActor);
                }
            }       
        }


        public void OnDestroy()
        {
            if (particleObject)
            {
                Destroy(particleObject, 5);
            }
            if (OnCritDestroyed != null)
            {
                OnCritDestroyed(Projectile, player);
            }
        }



        public void Update()
        {
            DistTick += (int)Vector2.Distance(Projectile.sprite.WorldCenter, lastStoredPosition) / 1.5f;
            for (int i = 0; i < DistTick; i++)
            {
                float t = (float)i / DistTick;
                if (S > UnityEngine.Random.value)
                {
                    Vector3 vector3 = Vector3.Lerp(Projectile.sprite.WorldCenter, lastStoredPosition, t);
                    ParticleSystem particleSystem = particleObject;
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                    {
                        position = vector3,
                        randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                    };
                    var emission = particleSystem.emission;
                    emission.enabled = false;
                    particleSystem.Emit(emitParams, 1);
                    lastStoredPosition = Projectile.sprite.WorldCenter;
                }
                DistTick--;
            }
        }
        public Vector2 lastStoredPosition;
        private float DistTick = 0;


        public struct CritContext
        {
            public float CritChance;
            public float? CritChanceMultiplier;
            public Func<float, float> CritChanceCalc;


            public float? CritDamageMultiplier;
            public float? CritDamageAddon;

            public Func<float, float> CritDamageCalc;

            public bool? isGuaranteedCrit;
            public Func<float, bool> GuaranteedCritCalc;
        }
    }
}
