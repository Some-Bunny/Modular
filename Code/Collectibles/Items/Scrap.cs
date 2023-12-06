using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
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
        public static void PostInit(PickupObject v)
        {
            v.CustomCost = 15;
            v.UsesCustomCost = true;
            Scrap_ID = v.PickupObjectId;
            v.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(v.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(v.gameObject);

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
            //tk2dAnim.playAutomatically = true;

            GameObject roomIcon = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(roomIcon);
            FakePrefab.MarkAsFakePrefab(roomIcon);
            var tk2d = roomIcon.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Item_Collection;
            tk2d.SetSprite(StaticCollections.Item_Collection.GetSpriteIdByName("scrap_room_icon"));
            (v as Scrap).minimapIcon = roomIcon;
        }

        public GameObject minimapIcon;
        private GameObject extant_minimapIcon;
        private RoomHandler m_minimapIconRoom;

        public override void Pickup(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", player.gameObject);
            GameStatsManager.Instance.HandleEncounteredObjectRaw(this.encounterTrackable.EncounterGuid);
            if (base.spriteAnimator)
            {
                base.spriteAnimator.StopAndResetFrame();
            }
            player.BloopItemAboveHead(base.sprite, "scrap_idle_001");
            GlobalConsumableStorage.AddConsumableAmount("Scrap", 1);
            this.GetRidOfMinimapIcon();
            UnityEngine.Object.Destroy(base.gameObject);
        }
        private void GetRidOfMinimapIcon()
        {
            if (this.extant_minimapIcon != null)
            {
                Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.extant_minimapIcon);
                this.extant_minimapIcon = null;
            }
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
                if (this.minimapIcon != null && !this.m_hasBeenPickedUp && this.gameObject.activeSelf == true)
                {
                    this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                    this.extant_minimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon, false);
                }
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

        public override void OnDestroy()
        {
            GetRidOfMinimapIcon();
            base.OnDestroy();
        }

        public void Update()
        {
            if (base.spriteAnimator != null && base.spriteAnimator.DefaultClip != null)
            {
                base.spriteAnimator.SetFrame(Mathf.FloorToInt(Time.time * base.spriteAnimator.DefaultClip.fps % (float)base.spriteAnimator.DefaultClip.frames.Length));
            }
            var gf = this.GetComponent<SquishyBounceWiggler>();
            if (gf) { Destroy(gf); }
        }

        private bool m_hasBeenPickedUp;

    }
}

