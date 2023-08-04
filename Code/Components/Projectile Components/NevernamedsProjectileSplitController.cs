using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModularMod
{
    public class ProjectileSplitController : MonoBehaviour
    {
        public ProjectileSplitController()
        {
            distanceTillSplit = 7.5f;
            splitAngles = 35;
            amtToSplitTo = 0;
            dmgMultAfterSplit = 0.66f;
            sizeMultAfterSplit = 0.8f;
            removeComponentAfterUse = true;
            chanceToSplit = 1;
            SplitsOnDestroy = false;
        }
        private void Start()
        {
            parentProjectile = base.GetComponent<Projectile>();
            parentOwner = parentProjectile.ProjectilePlayerOwner();
            parentProjectile.OnDestruction += ParentProjectile_OnDestruction;
        }

        private void ParentProjectile_OnDestruction(Projectile obj)
        {
            if (SplitsOnDestroy == false) { return; }
            SplitProjectile();
        }

        private void Update()
        {
            if (parentProjectile != null && distanceBasedSplit && !hasSplit)
            {
                if (parentProjectile.GetElapsedDistance() > distanceTillSplit)
                {
                    SplitProjectile();
                }
            }
        }


        private void SplitProjectile()
        {
            if (UnityEngine.Random.value <= chanceToSplit)
            {          
                float Addon = HasRandomizedSplitAngle ? BraveUtility.RandomAngle() : 0;
                float ProjectileInterval = splitAngles / ((float)amtToSplitTo - 1);
                float currentAngle = parentProjectile.Direction.ToAngle();
                float startAngle = currentAngle + (splitAngles * 0.5f);
                for (int i = 0; i < amtToSplitTo; i++)
                {
                    float finalAngle  = isPurelyRandomizedSplitAngle ? BraveUtility.RandomAngle() : startAngle - (ProjectileInterval * i);
                    GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(parentProjectile.gameObject, parentProjectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, finalAngle + Addon), true);
                    Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                    if (component != null)
                    {
                        component.Owner = parentOwner;
                        component.Shooter = parentOwner.specRigidbody;
                        component.baseData.damage *= dmgMultAfterSplit;
                        component.baseData.speed *= speedMultAfterSplit;
                        component.UpdateSpeed();
                        component.RuntimeUpdateScale(sizeMultAfterSplit);
                        ProjectileSplitController split2 = component.gameObject.GetComponent<ProjectileSplitController>();
                        if (split2 && removeComponentAfterUse)
                        {
                            UnityEngine.Object.Destroy(split2);
                        }
                    }
                }
                if (parentProjectile)
                {
                    UnityEngine.Object.Destroy(parentProjectile.gameObject);
                }
            }
            hasSplit = true;
        }
        private Projectile parentProjectile;
        private PlayerController parentOwner;
        private bool hasSplit;

        //Publics
        public bool distanceBasedSplit;
        public float distanceTillSplit;
        public bool splitOnEnemy;
        public float splitAngles;
        public int amtToSplitTo;
        public float chanceToSplit;

        public bool HasRandomizedSplitAngle = false;
        public bool isPurelyRandomizedSplitAngle = false;

        public float dmgMultAfterSplit;
        public float sizeMultAfterSplit;
        public float speedMultAfterSplit = 1;

        public bool removeComponentAfterUse;

        public bool SplitsOnDestroy;

    }
}
