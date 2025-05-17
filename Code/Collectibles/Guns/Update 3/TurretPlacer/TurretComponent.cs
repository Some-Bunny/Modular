using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace ModularMod.Code.Collectibles.Guns.Update_3
{ 
    public class TurretComponent : MonoBehaviour
    {
        public static int MaxTurrets = 3;

        public float GetMaxTurrets()
        {
            int Amount = MaxTurrets;
            Amount += GlobalModuleStorage.PlayerActiveModuleCount(Owner, IteratedDesign.ID);
            int amount = (gunController != null ? gunController.gun.modifiedVolley.projectiles.Count() : 1);
            return (amount == 1 ? Amount : Amount - 1) * amount;
        }


        public static List<TurretComponent> turrets = new List<TurretComponent>();

        private Projectile projectile;
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            var player = (projectile.Owner as PlayerController);
            if (player == null) { return; }
            if (player.PlayerHasCore() == null) { return; }
            gunController = player.PlayerHasCore().ModularGunController;
            var sticky = this.projectile.gameObject.GetOrAddComponent<StickyProjectileModifier>();
            sticky.stickyContexts.Add(new StickyProjectileModifier.StickyContext() { CanStickToTerrain = true, });
            sticky.OnPreStick += OPS;
            sticky.OnStickToWall += OnStickToWall;
            Owner = player;
        }


        private ModularGunController gunController;
        private PlayerController Owner;

        public string PrimeAnimation = "turretactive";
        public Material materialToCopy;

        public static Projectile projectileToFire;
        public static Projectile projectileToFireAlt;

        public bool Procced = false;
        public bool Active = false;

        public void OPS(GameObject projectileObject, PlayerController player)
        {
            PreStopDirection = projectileObject.GetComponent<Projectile>().Direction;
        }

        private Vector2 PreStopDirection;

        public void OnStickToWall(GameObject gameObject, StickyProjectileModifier modifier, tk2dBaseSprite sprite, PlayerController owner, PhysicsEngine.Tile t)
        {
            if (gameObject == null) { return; }
            if (t == null) { Destroy(gameObject); return; }
            Animator = gameObject.GetComponentInChildren<tk2dSpriteAnimator>();
            if (Animator == null) { Destroy(gameObject); return; }
            if (Procced == true) { return; }
            turrets.Add(this);
            int Failsafe = 60;
            if (turrets.Count > GetMaxTurrets()) 
            {
                while (turrets.Count > GetMaxTurrets() && Failsafe > 0)
                {
                    if (turrets.First().gameObject != null)
                    {
                        Destroy(turrets.First().gameObject);
                    }
                     turrets.Remove(turrets.First());
                    Failsafe--;
                }
            }
            if (Failsafe == 0)
            {
                Debug.LogWarning("[MDL] TURRET FAILSAFE TRIPPED, THIS REALLY SHOULDN'T HAPPEN.");
            }

            Procced = true;



            Animator.Play(PrimeAnimation);
            Animator.sprite.usesOverrideMaterial = true;
            Animator.sprite.renderer.material = materialToCopy;
            Vector2 EndPosition;
            Vector2 vector2 = gameObject.transform.PositionVector2().ToNearestWall(out EndPosition, PreStopDirection.ToAngle());
            
            
            float ang = BraveMathCollege.GetNearestAngle((gameObject.transform.PositionVector2() - vector2).ToAngle(), new float[] { 0, 90, 180, 270 });
            
            gameObject.GetComponentInChildren<SpeculativeRigidbody>().Reinitialize();
            
            var obj = UnityEngine.Object.Instantiate(VFXStorage.LaserReticle, gameObject.transform.position, Quaternion.identity);
            var tiledsprite = obj.GetComponent<tk2dTiledSprite>();
            tiledsprite.transform.position = EndPosition;
            tiledsprite.transform.localRotation = Quaternion.Euler(0f, 0f, ang);      
            tiledsprite.dimensions = new Vector2(1, 1f);
            tiledsprite.UpdateZDepth();
            tiledsprite.ShouldDoTilt = false;

            Direction = Toolbox.GetUnitOnCircle(ang, 0.25f);
            LaserSightInst = tiledsprite;
            tiledsprite.StartCoroutine(LaserSight());
            Destroy(this.gameObject, 180);
            AkSoundEngine.PostEvent("Play_BOSS_mineflayer_trigger_01", this.gameObject);
            if (Owner.IsInCombat && Owner.CurrentRoom != null)
            {
                Owner.CurrentRoom.OnEnemiesCleared += RoomCleared;
            }
        }
        public IEnumerator LaserSight()
        {
            float e = 0;
            while (e < 1)
            {
                LaserSightInst.renderer.enabled = e % 0.25f > 0.125f;
                e += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_OBJ_metroid_roll_01", this.gameObject);
            LaserSightInst.renderer.enabled = true;
            Active = true;
            yield break;
        }


        public tk2dTiledSprite LaserSightInst;
        public Vector2 Direction;
        public tk2dSpriteAnimator Animator;
        public GameObject muzzleFlashPrefab = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
        public void RoomCleared()
        {
            Destroy(this.gameObject);
        }

        public void OnDestroy()
        {
            if (Owner != null)
            {
                if (Owner.CurrentRoom != null)
                {
                    Owner.CurrentRoom.OnEnemiesCleared -= RoomCleared;
                }
            }
            if (turrets.Contains(this)) { turrets.Remove(this); }
            if (LaserSightInst != null)
            {
                Destroy(LaserSightInst.gameObject);
            }
        }

        public void Update()
        {
            if (Reloads == 0) { Destroy(this.gameObject); }
            IncrementAllTimers();
            if (Procced == false || LaserSightInst == null) { return; }
            
            if (CanAttack() == true)
            {
                Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (SpeculativeRigidbody otherRigidbody) => otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets;
                float num9 = 0;
                int rayMask2 = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.BulletBreakable, CollisionLayer.EnemyCollider);
                RaycastResult raycastResult2;
                if (PhysicsEngine.Instance.Raycast(Animator.sprite.WorldCenter + Direction, Direction, 1000, out raycastResult2, true, true, rayMask2, null, false, rigidbodyExcluder, null))
                {
                    num9 = raycastResult2.Distance;
                    if (raycastResult2.OtherPixelCollider != null)
                    {
                        bool c = raycastResult2.OtherPixelCollider.CollisionLayer == CollisionLayer.EnemyHitBox || raycastResult2.OtherPixelCollider.CollisionLayer == CollisionLayer.EnemyCollider;
                        if (Active == true && c == true)
                        {
                            AkSoundEngine.PostEvent("Play_WPN_sniperrifle_shot_01", this.gameObject);

                            GameObject vfx = SpawnManager.SpawnVFX(muzzleFlashPrefab, true);
                            vfx.transform.position = Animator.sprite.WorldCenter;
                            vfx.transform.localRotation = Quaternion.Euler(0f, 0f, Direction.ToAngle());
                            vfx.GetComponent<tk2dBaseSprite>().HeightOffGround = 22;

                            Destroy(vfx, 2);

                            ResetAttackCooldown();

                            GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(gunController.isAlt== true ? projectileToFireAlt.gameObject : projectileToFire.gameObject, Animator.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (Direction.ToAngle() + UnityEngine.Random.Range(-Accuracy, Accuracy))), true);
                            Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                            if (component != null)
                            {
                                component.Owner = Owner;
                                component.Shooter = Owner.specRigidbody;
                                component.IgnoreTileCollisionsFor(0.5f);
                                Owner.DoPostProcessProjectile(component);
                            }

                            currentClip--;
                            if (currentClip == 0)
                            {
                                b = false;
                                ResetReload();
                                ResetClip();
                                Reloads--;
                            }
                        }
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult2);
                LaserSightInst.dimensions = new Vector2(num9 * 16, 1f);
            }
            LaserSightInst.transform.position = Animator.sprite.WorldCenter + Direction;
            LaserSightInst.renderer.enabled = !(currentReload > 0f || currentAttackCooldown > 0.05f);
        }
        public int Reloads = 15;
        public bool CanAttack()
        {
            if (currentAttackCooldown > 0 || currentReload > 0)
            {
                return false;
            }
            return true;
        }
        private bool b = false;
        public void IncrementAllTimers()
        {
            if (currentAttackCooldown > 0) { currentAttackCooldown -= BraveTime.DeltaTime; }
            else if (b == false)
            {
                b = true;
                AkSoundEngine.PostEvent("Play_OBJ_metroid_roll_01", this.gameObject);
            }
            if (currentReload > 0) { currentReload -= BraveTime.DeltaTime; }
        }


        private int currentClip = 10;
        private float currentAttackCooldown = 0.333f;
        private float currentReload = 0;

        public void ResetClip()
        {
            currentClip = Clip;
        }
        public void ResetReload()
        {
            currentReload = Reload / Owner.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);

        }

        public void ResetAttackCooldown()
        {
            currentAttackCooldown = AttackCooldown / Owner.stats.GetStatValue(PlayerStats.StatType.RateOfFire);
        }


        public float AttackCooldown 
        {
            get
            {
                return gunController.GetRateOfFire(0.333f);
            }
        }
        public int Clip
        {
            get
            {
                return (int)(gunController.GetClipSize(12) * Owner.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier));
            }
        }
        public float Accuracy
        {
            get
            {
                return gunController.GetAccuracy(13f) * Owner.stats.GetStatValue(PlayerStats.StatType.Accuracy);
            }
        }
        public float Reload
        {
            get
            {
                return gunController.GetReload(3f);
            }
        }
    }
}
