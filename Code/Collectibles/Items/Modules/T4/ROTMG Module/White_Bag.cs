using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class White_Bag : PickupObject, IPlayerInteractable
    {
        public static ItemTemplate template = new ItemTemplate(typeof(White_Bag))
        {
            Name = "White Bag",
            Description = "Mad",
            LongDescription = "The White Bag...",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("the_bag"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            v.CustomCost = 15;
            v.UsesCustomCost = true;
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
            //BraveOutlineSprite
            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("warning_003"));
            tk2d.usesOverrideMaterial = true;
            Material mat = tk2d.renderer.material;
            mat.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetColor("_OverrideColor", new Color(0.02f, 0.4f, 1, 0.6f));
            VFX.CreateFastBody(CollisionLayer.Pickup, new IntVector2(0, 0), new IntVector2(0, 0));
            VFX_Weapon = VFX;

            ID = v.PickupObjectId;
            GameObject roomIcon = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(roomIcon);
            FakePrefab.MarkAsFakePrefab(roomIcon);
            var tk2d_2 = roomIcon.AddComponent<tk2dSprite>();
            tk2d_2.Collection = StaticCollections.Item_Collection;
            tk2d_2.SetSprite(StaticCollections.Item_Collection.GetSpriteIdByName("whitebag_room_icon"));
            (v as White_Bag).minimapIcon = roomIcon;
        }

        public GameObject minimapIcon;
        private GameObject extant_minimapIcon;
        private RoomHandler m_minimapIconRoom;

        public static GameObject VFX_Weapon;

        public override void Pickup(PlayerController player)
        {

            this.GetRidOfMinimapIcon();

            AkSoundEngine.PostEvent("Play_UI_map_open_01", player.gameObject);
            var stuffy = UnityEngine.Object.Instantiate(VFX_Weapon, this.transform.position, Quaternion.identity);
            stuffy.SetActive(true);
            stuffy.GetOrAddComponent<WeaponPickup>().Start();

            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), this.transform.position, Quaternion.identity);
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            gameObject.transform.localScale *= 2.5f;
            tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
            if (component2 != null)
            {
                component2.sprite.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 2);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 2f);
                component2.sprite.renderer.material.SetColor("_OverrideColor", Color.white);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.white);
            }
            Destroy(gameObject, 1.5f);


            GameStatsManager.Instance.HandleEncounteredObjectRaw(this.encounterTrackable.EncounterGuid);
            UnityEngine.Object.Destroy(base.gameObject);
        }


        public static int ID;

        protected void Start()
        {
            try
            {
                this.gameObject.AddComponent<SquishyBounceWiggler>();
                this.gameObject.SetActive(true);
                if (!RoomHandler.unassignedInteractableObjects.Contains(this))
                {
                    RoomHandler.unassignedInteractableObjects.Add(this);
                }
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
                //AkSoundEngine.PostEvent("Play_The_Bag", this.gameObject);
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"), this.transform.position, Quaternion.identity);
                tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
                gameObject.transform.localScale *= 2.5f;
                tk2dSpriteAnimator component2 = component.GetComponent<tk2dSpriteAnimator>();
                if (component2 != null)
                {
                    component2.sprite.usesOverrideMaterial = true;
                    component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                    component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                    component2.sprite.renderer.material.SetFloat("_EmissivePower", 2);
                    component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 2f);
                    component2.sprite.renderer.material.SetColor("_OverrideColor", Color.white);
                    component2.sprite.renderer.material.SetColor("_EmissiveColor", Color.white);
                }
                Destroy(gameObject, 1.5f);

                if (this.minimapIcon != null)
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

        private void GetRidOfMinimapIcon()
        {
            if (this.extant_minimapIcon != null)
            {
                Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.extant_minimapIcon);
                this.extant_minimapIcon = null;
            }
        }


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

        public override void OnDestroy()
        {
            GetRidOfMinimapIcon();
            base.OnDestroy();
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
            this.Pickup(interactor);
        }

        public virtual void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();


        }

        public virtual void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }
    }
}
