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
    public class PredefinedCore : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ConfidenceCore))
        {
            Name = "Predefined Drive",
            Description = "Certain Fate",
            LongDescription = "While held, vastly reduces the choices when selecting modules, but grants 2 of a selected module.\n\nI've got this.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("halfchoices_drive"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            v.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(v.gameObject);
            v.CustomCost = 10;
            
            v.SetupUnlockOnCustomFlag(SaveAPI.CustomDungeonFlags.BEAT_LICH_AS_MODULAR, true);
            predefinedCoreID = v.PickupObjectId;
            AdditionalShopItemController.ShopItemContexts.Add(new AdditionalShopItemController.AdditionalShopItemContext()
            {
                Condition = Cond,
                itemID = ID,
                offset = Offset,
            });
            ChooseModuleController.PrimaryOptionsModifier += ReturnOptions;
            ChooseModuleController.ModuleUICarrier.OnModuleSelected += OnModuleDropped;
            ChooseModuleController.AngleSpawnModifier += Ret;
            ChooseModuleController.RadiusSpawnModifier += Radius;
            ChooseModuleController.PrimaryOptionsModifier += ReturnOptions;
        }

        public static float Radius(float angle)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(predefinedCoreID))
                {
                    angle += -1;
                }
            }
            return angle;
        }

        public static float Ret(float angle)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(predefinedCoreID))
                {
                    angle += BraveUtility.RandomAngle();
                }
            }
            return angle;
        }

        public static void OnModuleDropped(DefaultModule module, PlayerController c)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(predefinedCoreID))
                {
                    var extantModule = UnityEngine.Object.Instantiate<GameObject>(GlobalModuleStorage.ReturnModule(module).gameObject, module.sprite.WorldCenter, Quaternion.identity).GetComponent<DefaultModule>();
                    DebrisObject orAddComponent = extantModule.gameObject.GetOrAddComponent<DebrisObject>();
                    orAddComponent.shouldUseSRBMotion = true;
                    orAddComponent.angularVelocity = 0f;
                    orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                    orAddComponent.sprite.UpdateZDepth();
                    orAddComponent.Trigger(Vector3.up.WithZ(2f), 1, 1f);
                    extantModule.OnEnteredRange(c);
                }
            }
        }

        public static int ReturnOptions(int c, PickupObject.ItemQuality q)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.HasPickupID(predefinedCoreID))
                {
                    switch (q)
                    {
                        case ItemQuality.D:
                            return 1;
                        case ItemQuality.C:
                            return 1;
                        case  ItemQuality.B:
                            return 1;
                        case ItemQuality.A:
                            return 2;
                        case ItemQuality.S:
                            return 2;
                        default:
                            return 2;
                    }
                }
            }
            return c;
        }

        public override void Pickup(PlayerController player)
        {
            AdvancedGameStatsManager.Instance.SetStat(CustomTrackedStats.ENCOUNTERS_OF_PREDEFINED, 1);
            base.Pickup(player);
        }


        public static bool Cond(BaseShopController.AdditionalShopType shopType)
        {
            if (AdvancedGameStatsManager.Instance == null) { return false; }
            if (AdvancedGameStatsManager.Instance.GetSessionStatValue(CustomTrackedStats.ENCOUNTERS_OF_PREDEFINED) > 0) { return false; }
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BEAT_LICH_AS_MODULAR) == false) { return false; }
            if (shopType != BaseShopController.AdditionalShopType.NONE) { return false; }
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                if (player.PlayerHasCore() != null && !player.HasPickupID(predefinedCoreID))
                {
                    return true;
                }
            }
            return false;
        }
        public static Tuple<int, int> ID()
        {
            return new Tuple<int, int>(PredefinedCore.predefinedCoreID, 30);
        }
        public static Vector3 Offset(BaseShopController.AdditionalShopType shopType)
        {
            Vector3 help = new Vector3(0, 0);
            if (shopType == BaseShopController.AdditionalShopType.NONE) { help = new Vector3(6.125f, 1.625f); }
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
        public static int predefinedCoreID;
    }
}

