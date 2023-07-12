using Dungeonator;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModularMod.Code.Controllers
{
    public class AdditionalShopItemController
    {
        public static void Init()
        {
            new Hook(typeof(BaseShopController).GetMethod("DoSetup", BindingFlags.Instance | BindingFlags.NonPublic), typeof(AdditionalShopItemController).GetMethod("DoSetupHook"));
            /*
            ShopItemContexts.Add(new AdditionalShopItemContext()
            {
                Condition = Cond,
                itemID = ID,
                offset = Offset
            });
            */
        }

        public static bool Cond(BaseShopController.AdditionalShopType shopType)
        {
            return shopType == BaseShopController.AdditionalShopType.NONE || shopType == BaseShopController.AdditionalShopType.BLACKSMITH;//GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON;
        }
        public static int ID()
        {
            return ModulePrinterCore.ModulePrinterCoreID;
        }
        public static Vector3 Offset(BaseShopController.AdditionalShopType shopType)
        {
            Vector3 help = new Vector3(0, 0);
            if (shopType == BaseShopController.AdditionalShopType.NONE) { help = new Vector3(0.875f, 1.5f); }
            if (shopType == BaseShopController.AdditionalShopType.BLACKSMITH) { help = new Vector3(2.875f, -8.625f); }
            return help;
        }
        public static void DoSetupHook(Action<BaseShopController> orig, BaseShopController self)
        {
            orig(self);
            foreach (var entry in ShopItemContexts)
            {
                if (entry.Condition != null && entry.Condition(self.baseShopType) == true && entry.itemID != null && entry.offset != null)
                {
                    GameObject gunObj = PickupObjectDatabase.GetById(entry.itemID().First).gameObject;

                    GameObject gameObject8 = new GameObject("Additional Shop Item(" + gunObj.name + ")");
                    Transform transform4 = gameObject8.transform;

                    GameObject transObj = new GameObject();
                    transObj.transform.position = self.transform.position + entry.offset(self.baseShopType);  //PlanetsideReflectionHelper.ReflectGetField<Transform[]>(typeof(BaseShopController), "spawnPositionsGroup2", self).Last().position + new Vector3(0, -3.125f);
                    transform4.position = transObj.transform.position;
                    transform4.parent = transObj.transform;

                    EncounterTrackable component9 = gunObj.GetComponent<EncounterTrackable>();
                    if (component9 != null)
                    {
                        GameManager.Instance.ExtantShopTrackableGuids.Add(component9.EncounterGuid);
                    }
                    GameLevelDefinition lastLoadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
                    float num4 = (lastLoadedLevelDefinition == null) ? 1f : lastLoadedLevelDefinition.priceMultiplier;

                    ShopItemController shopItemController2 = gameObject8.AddComponent<ShopItemController>();
                    self.AssignItemFacing(transObj.transform, shopItemController2);
                    self.m_room.RegisterInteractable(shopItemController2);
                    
                    if (entry.itemID().Second != -1)
                    {
                        shopItemController2.OverridePrice = ((int?)(entry.itemID().Second * num4));
                    }
                    shopItemController2.Initialize(gunObj.GetComponent<PickupObject>(), self);
                    self.m_itemControllers.Add(shopItemController2);
                    self.m_shopItems.Add(gunObj);
                    self.m_room.RegisterInteractable(shopItemController2);


                }

            }
        }
        public static List<AdditionalShopItemContext> ShopItemContexts = new List<AdditionalShopItemContext>();
        public class AdditionalShopItemContext
        {
            public Func<BaseShopController.AdditionalShopType, bool> Condition;
            public Func<Tuple<int, int>> itemID;
            public Func<BaseShopController.AdditionalShopType,Vector3> offset;
        }
    }
}
