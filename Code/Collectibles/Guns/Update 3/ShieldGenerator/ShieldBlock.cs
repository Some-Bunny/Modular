using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod.Code.Collectibles.Guns.Update_3
{


    public class InvalidForCube : MonoBehaviour { }
    public class ShieldEater : MonoBehaviour
    {
        private Projectile projectile;
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.projectile.baseData.force = 0;


        }
    }


    public class ShieldBlock : MonoBehaviour
    {
        private Projectile projectile;
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.projectile.specRigidbody.OnPreRigidbodyCollision += OnPreCollision;
            this.projectile.baseData.force = 0;
            this.Player = this.projectile.Owner as PlayerController;
            if (CheckModule() == true)
            {
                projectile.baseData.speed *= 0.8f;
                projectile.UpdateSpeed();
                projectile.projectileHitHealth += 10;
            }    
        }
        public PlayerController Player;
        public float Range = 0.875f;
        public float e = 0;
        public float e1 = 0;

        public void Update()
        {
            if (e < 0.2f) { e += BraveTime.DeltaTime; }
            if (e1 < 1) { e1 += BraveTime.DeltaTime; }

            var t = ProjectilesPositions.Keys.ToList();
            var t1 = ProjectilesPositions.Values.ToList();
            for (int i = ProjectilesPositions.Count - 1; i > -1; i--)
            {
                if (t[i] == null)
                {
                    ProjectilesPositions.Remove(t[i]);
                }
                else
                {
                    t[i].gameObject.transform.position = this.projectile.sprite.WorldCenter + t1[i];
                    t[i].specRigidbody.Reinitialize();
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


        protected virtual void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            bool Iterated = CheckModule();
            if (otherRigidbody.projectile == null)
            {
                return;
            }
            var shield = otherRigidbody.projectile.GetComponent<ShieldEater>();
            var shieldBlock = otherRigidbody.projectile.GetComponent<ShieldBlock>();
            var invalid = otherRigidbody.projectile.GetComponent<InvalidForCube>();
            if (invalid != null)
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
            if (shieldBlock != null)
            {
                if (this.e1 > (Iterated == true? 0.65f : 0.95f))
                {
                    this.projectile.baseData.damage += (Iterated == true ? 2 : 1);
                    this.projectile.baseData.speed *= 1.3f;
                    this.projectile.UpdateSpeed();
                    this.projectile.ResetDistance();

                    this.e1 = 0;

                    shieldBlock.projectile.baseData.damage += 1;
                    shieldBlock.projectile.baseData.speed *= 1.3f;
                    shieldBlock.projectile.UpdateSpeed();
                    shieldBlock.projectile.ResetDistance();

                    shieldBlock.e1 = 0;

                    this.projectile.Direction = Toolbox.GetUnitOnCircleVec3(BraveUtility.RandomAngle(), 1);
                    shieldBlock.projectile.Direction = Toolbox.GetUnitOnCircleVec3(BraveUtility.RandomAngle(), 1);


                    AkSoundEngine.PostEvent("Play_ENM_bullat_tackle_01", this.gameObject);
                    if (ConfigManager.DoVisualEffect == true)
                    {
                        GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                        GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, this.projectile.sprite.WorldCenter, Quaternion.identity);
                        blankObj.transform.localScale = Vector3.one * 0.15f;
                        Destroy(blankObj, 2f);
                    }

                }

                PhysicsEngine.SkipCollision = true;
                return;
            }

            if ((otherRigidbody.projectile.Owner is PlayerController) && shield == null)// || fuck == true)
            {
                PhysicsEngine.SkipCollision = true;
                return;
            }
            bool isCube = shield != null;
            if ((otherRigidbody.projectile.Owner is AIActor) | isCube == true)
            {

                PhysicsEngine.SkipCollision = true;
                
                if (ProjectilesPositions.ContainsKey(otherRigidbody.projectile)) { return; }
                if (e < 0.2f) { e += BraveTime.DeltaTime; return; }
                e = 0f;
                if (this.projectile.projectileHitHealth < 1)
                {
                    this.projectile.DieInAir();
                    return;
                }
                this.projectile.projectileHitHealth -= isCube == true ? 3 : 1;
                otherRigidbody.projectile.RemoveBulletScriptControl();
                otherRigidbody.projectile.allowSelfShooting = false;


                if (otherRigidbody.projectile.Owner && otherRigidbody.projectile.Owner.specRigidbody)
                {
                    otherRigidbody.projectile.specRigidbody.DeregisterSpecificCollisionException(otherRigidbody.projectile.Owner.specRigidbody);
                }

                otherRigidbody.projectile.Owner = this.projectile.Owner;
                otherRigidbody.projectile.SetNewShooter(this.projectile.Owner.specRigidbody);

                otherRigidbody.projectile.collidesWithPlayer = false;
                otherRigidbody.projectile.collidesWithEnemies = true;

                SpawnManager.PoolManager.Remove(otherRigidbody.projectile.transform);

                otherRigidbody.projectile.baseData.speed = 0.1f;
                otherRigidbody.projectile.UpdateSpeed();
                otherRigidbody.projectile.baseData.force = 0;

                otherRigidbody.projectile.baseData.damage = otherRigidbody.projectile.IsBlackBullet ? 5f : 2.5f;


                otherRigidbody.projectile.UpdateCollisionMask();
                otherRigidbody.projectile.ResetDistance();
                otherRigidbody.projectile.Reflected();
                otherRigidbody.projectile.Reawaken();
                if (isCube == false)
                {
                    otherRigidbody.projectile.AdjustPlayerProjectileTint(isAlt ? new Color(0, 0, 1, 1) : new Color(0, 1, 1, 1), 10, 0.5f);
                }

                PierceProjModifier pierceProjModifier1 = otherRigidbody.projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                pierceProjModifier1.penetration = 10;
                otherRigidbody.projectile.gameObject.AddComponent<MaintainDamageOnPierce>();

                if (this.projectile.Owner is PlayerController)
                {
                    (this.projectile.Owner as PlayerController).DoPostProcessProjectile(otherRigidbody.projectile);
                }

                ProjectilesPositions.Add(otherRigidbody.projectile, Toolbox.GetUnitOnCircleVec3(otherRigidbody.projectile.Direction.ToAngle() - 180, (Range * this.projectile.AdditionalScaleMultiplier) * (isCube == true ? 0.75f : 1)));

                return;
            }
        }
        public bool isAlt = false;

        public void OnDestroy()
        {
            var t = ProjectilesPositions.Keys.ToList();

            AkSoundEngine.PostEvent("Play_ENM_bullat_tackle_01", this.gameObject);
            if (ConfigManager.DoVisualEffect == true)
            {
                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, this.projectile.sprite.WorldCenter, Quaternion.identity);
                blankObj.transform.localScale = Vector3.one * 0.15f;
                Destroy(blankObj, 2f);
            }
            for (int i = ProjectilesPositions.Count - 1; i > -1; i--)
            {
                if (t[i] != null)
                {
                    var proj = t[i];
                    proj.baseData.UsesCustomAccelerationCurve = true;
                    proj.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1, 1f, 0.5f);

                    proj.UpdateCollisionMask();
                    proj.ResetDistance();
                    proj.Reflected();
                    proj.Reawaken();
                    proj.Direction = Toolbox.GetUnitOnCircleVec3(BraveUtility.RandomAngle(), 1);
                    proj.baseData.speed = 25;
                    proj.UpdateSpeed();
                    ProjectilesPositions.Remove(proj);
                }
            }
        }

        private Dictionary<Projectile, Vector2> ProjectilesPositions = new Dictionary<Projectile, Vector2>();
    }
}
