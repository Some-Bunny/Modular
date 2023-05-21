using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class Scrap : PickupObject
    {
        public static ItemTemplate template = new ItemTemplate(typeof(Scrap))
        {
            Name = "Scrap",
            Description = "Damage Up",
            LongDescription = "Increases Damage by\n33% (+33% per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("gear_borderless"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public override void Pickup(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", player.gameObject);
            GameStatsManager.Instance.HandleEncounteredObjectRaw(this.encounterTrackable.EncounterGuid);
            player.BloopItemAboveHead(base.sprite, "");
            player.gameObject.GetComponent<ConsumableStorage>().AddConsumableAmount("Scrap", 1);
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public static void PostInit(PickupObject v)
        {
            v.CustomCost = 20;
            v.UsesCustomCost = true;
            v.gameObject.SetActive(false);
            SpeculativeRigidbody speculativeRigidbody = v.gameObject.AddComponent<SpeculativeRigidbody>();
            PixelCollider item = new PixelCollider
            {
                IsTrigger = true,
                ManualWidth = 12,
                ManualHeight = 12,
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.PlayerBlocker,
                ManualOffsetX = 0,
                ManualOffsetY = 0
            };
            speculativeRigidbody.PixelColliders = new List<PixelCollider>
            {
                item
            };

            var tk2dAnim = v.gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("ScrapAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;
            Scrap_ID = v.PickupObjectId;
        }

        public static int Scrap_ID;

        protected void Start()
        {
            try
            {
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
                var storedBody = base.gameObject.GetComponent<SpeculativeRigidbody>();
                SpeculativeRigidbody speculativeRigidbody = storedBody;
                SpeculativeRigidbody speculativeRigidbody2 = speculativeRigidbody;
                speculativeRigidbody2.OnTriggerCollision = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(speculativeRigidbody2.OnTriggerCollision, new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision));
            }
            catch (Exception ex)
            {
                ETGModConsole.Log(ex.Message, false);
            }
        }

        private void OnPreCollision(SpeculativeRigidbody otherRigidbody, SpeculativeRigidbody source, CollisionData collisionData)
        {
            if (!m_hasBeenPickedUp)
            {
                PlayerController component = otherRigidbody.GetComponent<PlayerController>();
                if (component != null)
                {
                    this.m_hasBeenPickedUp = true;
                    this.Pickup(component);
                }
            }
        }

        private bool m_hasBeenPickedUp;

    }
}

