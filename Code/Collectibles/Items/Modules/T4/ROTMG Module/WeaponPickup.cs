using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ModularMod.LootMadness;
using UnityEngine;

namespace ModularMod
{
    public class WeaponPickup : BraveBehaviour, IPlayerInteractable
    {
        public void Start()
        {
            DebrisObject orAddComponent = this.gameObject.GetOrAddComponent<DebrisObject>();
            orAddComponent.shouldUseSRBMotion = true;
            orAddComponent.angularVelocity = 0f;
            orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
            orAddComponent.sprite.UpdateZDepth();
            orAddComponent.Trigger(Vector3.up.WithZ(0.5f), 1, 1f);
            modifiedRealm = new ModifiedRealmWeapon();
            modifiedRealm.Startup();
            this.sprite.renderer.material.SetColor("_OverrideColor", modifiedRealm.color);
            this.sprite.SetSprite(modifiedRealm.SpriteData, modifiedRealm.SpriteID);
            var room = this.transform.position.GetAbsoluteRoom();
            if (room != null) { room.RegisterInteractable(this); }
            this.gameObject.transform.localScale *= modifiedRealm.ReturnTotalSizeMult();

            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }
        public ModifiedRealmWeapon modifiedRealm;
        public ModifiedDefaultLabelManager PrimaryLabel;

        public virtual string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public virtual float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public virtual float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public virtual void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);

            this.modifiedRealm.Pickup(interactor);

            if (PrimaryLabel != null) { Destroy(PrimaryLabel.gameObject); }

            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), this.transform.position, Quaternion.identity);
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            component.HeightOffGround = 35f;
            component.UpdateZDepth();
            gameObject.transform.localScale *= 1 + modifiedRealm.ReturnTotalSizeMult();
            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
            if (component2 != null)
            {
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 2);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 2f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", modifiedRealm.color);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", modifiedRealm.color);
            }
            Destroy(gameObject, 1.5f);
            Destroy(this.gameObject);
        }


        public virtual void OnEnteredRange(PlayerController interactor)
        {
            if (PrimaryLabel != null) { Destroy(PrimaryLabel.gameObject); }
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            if (PrimaryLabel == null) { PrimaryLabel = Toolbox.GenerateText(this.transform, new Vector2(-0.5f, -0.25f), 0.5f, modifiedRealm.Name, new Color(0.07f, 0.07f, 0.07f, 0.7f)); }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }


        public void Update()
        {
            for (int i = this.transform.childCount - 1; i > -1; i--)
            {
                if (this.transform.GetChild(i).name == "BraveOutlineSprite")
                {
                    var ch = this.transform.GetChild(i);

                    ch.transform.localPosition = ch.transform.localPosition.WithZ(15);
                    ch.GetComponent<tk2dBaseSprite>().scale = this.transform.localScale;

                }
            }
        }

        public virtual void OnExitRange(PlayerController interactor)
        {
            if (PrimaryLabel != null) { PrimaryLabel.Inv(); PrimaryLabel = null; }
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }
    }
}
