using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem;


namespace ModularMod
{
    public class CraftingCore : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CraftingCore))
        {
            Name = "Module Crafting Core",
            Description = "Game Making",
            LongDescription = "Allows for crafting modules out of scrap. Single Use.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("craftingcore"),
            Quality = ItemQuality.SPECIAL,
            Cooldown = 1,
            CooldownType = ItemBuilder.CooldownType.Timed,
            PostInitAction = PIA,
        };

        public static void PIA(PickupObject pickup)
        {

            var tk2dAnim = pickup.gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("CrafterAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;
            pickup.CustomCost = 45;
            pickup.UsesCustomCost = true;
            pickup.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 1);
            var active = (pickup as PlayerItem);
            active.canStack = true;
            active.numberOfUses = 1;
            active.UsesNumberOfUsesBeforeCooldown = true;

            CraftingCoreID = pickup.PickupObjectId;
        }

        public override void Update()
        {
            if (this.m_pickedUp)
            {
                if (this.LastOwner == null)
                {
                    this.LastOwner = base.GetComponentInParent<PlayerController>();
                }
                if (this.remainingTimeCooldown > 0f && (PlayerItem.AllowDamageCooldownOnActive || !this.IsCurrentlyActive))
                {
                    this.remainingTimeCooldown = Mathf.Max(0f, this.remainingTimeCooldown - BraveTime.DeltaTime);
                }
                if (this.IsCurrentlyActive)
                {
                    this.m_activeElapsed += BraveTime.DeltaTime * this.m_adjustedTimeScale;
                    if (!string.IsNullOrEmpty(this.OnActivatedSprite))
                    {
                        base.sprite.SetSprite(this.OnActivatedSprite);
                    }
                }
            }
        }


        public override bool CanBeUsed(PlayerController user)
        {
            if (!user)
            {
                return false;
            }
            else
            {
                return user.IsInCombat == true ? false : true;
            }
        }


        public override void DoEffect(PlayerController user)
        {
            this.numberOfUses++;
            if (user.PlayerHasComputerCore())
            {
                var cc = user.PlayerHasComputerCore();
                if (cc.extant_Inventory_button) { Destroy(cc.extant_Inventory_button.gameObject); }
                if (cc.extant_craft_button) { Destroy(cc.extant_craft_button.gameObject); }
                if (cc.extant_close_button != null) { Destroy(cc.extant_close_button.gameObject); }
                cc.extant_Crafting_Controller = ScriptableObject.CreateInstance<ModuleCrafingController>();
                cc.extant_Crafting_Controller.DoQuickStart(user);

                cc.extant_Crafting_Controller.OnCrafted += () =>
                {
                    this.numberOfUses--;
                    if (this.numberOfUses < 1)
                    {
                        cc.extant_Crafting_Controller.Nuke();
                        cc.extant_Crafting_Controller.ObliterateUI();
                        Destroy(cc.extant_Crafting_Controller);
                        user.RemoveActiveItemAt(user.activeItems.FindIndex(EndsWithSaurus));
                    }
                };
            }
        }

        private bool EndsWithSaurus(PlayerItem s)
        {
            if (s == this)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public override void DoActiveEffect(PlayerController user)
        {
            ETGModConsole.Log(this.numberOfUses);
            base.DoActiveEffect(user);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        public static int CraftingCoreID;
    }
}

