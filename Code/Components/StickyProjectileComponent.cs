using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class StickyProjectileModifier : MonoBehaviour
    {
        public Action<GameObject, PlayerController> OnPreStick;
        public Action<GameObject, StickyProjectileModifier, tk2dBaseSprite, PlayerController> OnStick;
        public Action<GameObject, StickyProjectileModifier, PlayerController> OnStickyDestroyed;

        public Projectile currentObject;
        public GameObject objectToLookOutFor;
        public Material materialToCopy;
        public PlayerController player;

        public List<StickyContext> stickyContexts = new List<StickyContext>();

        private bool StickToEnemies = false;
        private bool StickToTerrain = false;

        public void Start()
        {
            foreach (var contexts in stickyContexts)
            {
                if (contexts.CanStickEnemies == true) { StickToEnemies = true; }
                if (contexts.CanStickToTerrain == true) { StickToTerrain = true; }

            }
            currentObject = this.GetComponent<Projectile>();
            if (currentObject)
            {
                if (StickToTerrain == true)
                {
                    currentObject.specRigidbody.OnPreTileCollision += (myRigidbody, myPixelCollider, otherRigidbody, otherPixelCollider) =>
                    {
                        HandleHit(currentObject, null);
                    };
                }
                if (StickToEnemies == true)
                {
                    currentObject.specRigidbody.OnPreRigidbodyCollision += (myRigidbody, myPixelCollider, otherRigidbody, otherPixelCollider) => {
                        HandleHit(currentObject, otherRigidbody);
                    };
                }           
            }
        }
        private void HandleHit(Projectile projectile, SpeculativeRigidbody otherBody)
        {
            if (otherBody == null)
            {
                TransformToSticky(projectile, null);

            }
            else
            {
                if (otherBody.aiActor != null && !otherBody.healthHaver.IsDead && otherBody.aiActor.behaviorSpeculator && !otherBody.aiActor.IsHarmlessEnemy)
                {
                    if (base.GetComponent<PierceProjModifier>() != null)
                    {
                        if (base.GetComponent<PierceProjModifier>().penetration == 0)
                        { TransformToSticky(projectile, otherBody); }
                    }
                    else
                    { TransformToSticky(projectile, otherBody); }
                }
            }          
        }

        private void TransformToSticky(Projectile projectile, SpeculativeRigidbody otherBody)
        {
            if (projectile == null) { return; }
            if (OnPreStick != null)
            {
                if (projectile != null)
                {
                    OnPreStick(projectile.gameObject, player);
                }
            }
            projectile.DestroyMode = Projectile.ProjectileDestroyMode.DestroyComponent;
            objectToLookOutFor = projectile.gameObject;
            if (otherBody != null)
            {
                objectToLookOutFor.transform.parent = otherBody.transform;
                objectToLookOutFor.transform.position = otherBody.sprite.WorldCenter + Toolbox.GetUnitOnCircle(projectile.angularVelocity, Vector2.Distance(objectToLookOutFor.transform.position, otherBody.sprite.WorldCenter) * 0.2f);
            }
            player = projectile.Owner as PlayerController;
            if (OnStick != null)
            {
                OnStick(objectToLookOutFor, this, objectToLookOutFor.GetComponentInChildren<tk2dBaseSprite>(), player);
            }
        }

        public void OnDestroy()
        {
            if (objectToLookOutFor != null && this != null && player != null)
            {
                if (OnStickyDestroyed != null)
                {
                    OnStickyDestroyed(objectToLookOutFor, this, player);
                }
            }
        }
        public class StickyContext
        {
            public bool CanStickToTerrain = false;
            public bool CanStickEnemies = true;
        }
    }
}
