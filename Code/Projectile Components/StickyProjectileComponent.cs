using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class StickyProjectileModifier : MonoBehaviour
    {
        public Action<Projectile, PlayerController> OnPreStick;
        public Action<GameObject, StickyProjectileModifier, tk2dBaseSprite, PlayerController> OnStick;
        public Action<GameObject, StickyProjectileModifier, PlayerController> OnStickyDestroyed;

        public Projectile currentObject;
        public GameObject objectToLookOutFor;
        public Material materialToCopy;
        public PlayerController player;

        public void Start()
        {
            currentObject = this.GetComponent<Projectile>();
            if (currentObject)
            {
                currentObject.OnHitEnemy += HandleHit;
            }
        }
        private void HandleHit(Projectile projectile, SpeculativeRigidbody otherBody, bool fatal)
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

        private void TransformToSticky(Projectile projectile, SpeculativeRigidbody otherBody)
        {
            if (OnPreStick != null)
            {
                OnPreStick(projectile, player);
            }
            projectile.DestroyMode = Projectile.ProjectileDestroyMode.DestroyComponent;
            objectToLookOutFor = projectile.gameObject;
            objectToLookOutFor.transform.parent = otherBody.transform;
            objectToLookOutFor.transform.position = otherBody.sprite.WorldCenter + Toolbox.GetUnitOnCircle(projectile.angularVelocity, Vector2.Distance(objectToLookOutFor.transform.position, otherBody.sprite.WorldCenter) * 0.2f);
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
    }
}
