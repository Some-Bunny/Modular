using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using ModularMod.Code.Controllers;
using ModularMod.Code.Hooks;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class ConfidenceCore : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ConfidenceCore))
        {
            Name = "Confidence Drive",
            Description = "Self Boost",
            LongDescription = "Grants 1 Coolness. While held, bosses no longer drop guns. Breaks when removed.\n\nI've got this.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("confidence_drive"),
            Quality = ItemQuality.D,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            v.CustomCost = 10;
            
            v.AddPassiveStatModifier(PlayerStats.StatType.Coolness, 1, StatModifier.ModifyMethod.ADDITIVE);
            v.SetupUnlockOnCustomFlag(SaveAPI.CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR, true);
            ConfidenceCoreID = v.PickupObjectId;
            AdditionalShopItemController.ShopItemContexts.Add(new AdditionalShopItemController.AdditionalShopItemContext()
            {
                Condition = Cond,
                itemID = ID,
                offset = Offset,

            });
            Actions.ModifyBossDrop += Modify;
            Actions.ModifyForceGun += Bool;

        }

        public override void Pickup(PlayerController player)
        {
            AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.ENCOUNTERS_OF_COFIDENCE, 1);
            base.Pickup(player);
        }

        public static bool Bool(bool b)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(ConfidenceCoreID))
                {
                    return false;
                }
            }
            return b;
        }
        public static PickupObject Modify(PickupObject obj)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(ConfidenceCoreID))
                {
                    int c = 10;
                    while (obj is Gun && c > 0)
                    {
                        c--;
                        obj = GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.SelectByWeight().GetComponent<PickupObject>();
                    }
                    return obj;
                }
            }
            return obj;
        }

        public static bool Cond(BaseShopController.AdditionalShopType shopType)
        {
            if (GameManager.Instance == null) { return false; }
            if (GameStatsManager.Instance.IsInSession == false) { return false; }
            if (GameManager.Instance.Dungeon == null) { return false; }
            if (GameManager.Instance.AllPlayers.Count() == 0) { return false; }


            if (AdvancedGameStatsManager.Instance.GetSessionStatValue(CustomTrackedStats.ENCOUNTERS_OF_COFIDENCE) > 0) { return false; }
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR) == false) { return false; }
            if (shopType != BaseShopController.AdditionalShopType.NONE) { return false; }
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.PlayerHasCore() != null && !player.HasPickupID(ConfidenceCoreID))
                {
                    return true;
                }
            }
            return false;
        }
        public static Tuple<int, int> ID()
        {
            return new Tuple<int, int>(ConfidenceCore.ConfidenceCoreID, 15);
        }
        public static Vector3 Offset(BaseShopController.AdditionalShopType shopType)
        {
            Vector3 help = new Vector3(0, 0);
            if (shopType == BaseShopController.AdditionalShopType.NONE) { help = new Vector3(0.875f, 1.625f); }
            return help;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            var DebrisObject = base.Drop(player);
            DebrisObject.OnTouchedGround += (obj) =>
            {
                var blast = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
                blast.ignoreList = new List<SpeculativeRigidbody>() { player.specRigidbody };
                Exploder.Explode(obj.sprite.WorldCenter, blast, obj.sprite.WorldCenter);
                Destroy(obj.gameObject);
            };
            return DebrisObject;
        }

        public static int ConfidenceCoreID;
    }
}

