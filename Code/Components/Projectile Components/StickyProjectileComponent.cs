using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Alexandria.Misc.CustomActions;

namespace ModularMod
{
    public class StickyProjectileModifier : MonoBehaviour
    {
        public Action<GameObject, PlayerController> OnPreStick;
        public Action<GameObject, StickyProjectileModifier, tk2dBaseSprite, PlayerController> OnStick;
        public Action<GameObject, StickyProjectileModifier, tk2dBaseSprite, PlayerController, PhysicsEngine.Tile> OnStickToWall;

        public Action<GameObject, StickyProjectileModifier, PlayerController> OnStickyDestroyed;
        public static Action<GameObject> OnStickyProjectileStuck;


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
                    currentObject.specRigidbody.OnPreTileCollision += (myRigidbody, myPixelCollider, Tile, TilePixelCollider) =>
                    {
                        HandleHit(currentObject, null, Tile);
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
        private void HandleHit(Projectile projectile, SpeculativeRigidbody otherBody, PhysicsEngine.Tile tile = null)
        {
            if (otherBody == null)
            {
                if (projectile)
                {
                    if (projectile.GetComponent<BounceProjModifier>() == null)
                    {
                        TransformToSticky(projectile, null, tile);
                    }
                    else if (projectile.GetComponent<BounceProjModifier>().numberOfBounces == 0)
                    {
                        TransformToSticky(projectile, null, tile);
                    }
                }
               
            }
            else
            {
                if (projectile)
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
        }

        private void TransformToSticky(Projectile projectile, SpeculativeRigidbody otherBody, PhysicsEngine.Tile tile = null)
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
            if (OnStickToWall != null && tile != null)
            {
                OnStickToWall(objectToLookOutFor, this, objectToLookOutFor.GetComponentInChildren<tk2dBaseSprite>(), player, tile);
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
