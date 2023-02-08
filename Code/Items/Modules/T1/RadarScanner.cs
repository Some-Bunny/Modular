using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;


namespace ModularMod
{
    public class RadarScanner : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RadarScanner))
        {
            Name = "Radar Scanner",
            Description = "Investment",
            LongDescription = "Secret Rooms are revealed on the map. All secret rooms contain 2 (+1 per stack) extra pickups." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("radarscanner_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("radarscanner_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.9f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Radar Scanner " + h.ReturnTierLabel();
            h.LabelDescription = "Secret Rooms are revealed on the map.\nAll secret rooms contain 2(" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ")\nextra pickups.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded += Instance_OnNewLevelFullyLoaded;
            RevealSecretRooms();
        }

        private void Instance_OnNewLevelFullyLoaded()
        {
            RevealSecretRooms();
            AddGoodies();
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= Instance_OnNewLevelFullyLoaded;
        }

        public void RevealSecretRooms()
        {
            for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
            {
                RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[i];
                if (roomHandler.connectedRooms.Count != 0)
                {
                    if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
                    {
                        roomHandler.RevealedOnMap = true;
                        Minimap.Instance.RevealMinimapRoom(roomHandler, true, true, false);
                    }
                }
            }
        }

        public void AddGoodies()
        {
            for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
            {
                RoomHandler roomHandler = GameManager.Instance.Dungeon.data.rooms[i];
                if (roomHandler.connectedRooms.Count != 0)
                {
                    if (roomHandler.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
                    {
                        for (int q = 0; q < Stack() + 1; q++)
                        {
                            var pickup = LootEngine.SpawnItem(GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable.SelectByWeight(false), roomHandler.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.Original).ToCenterVector3(0), Vector2.zero, 0);
                            pickup.GetComponent<PickupObject>().IgnoredByRat = true;
                        }
                    }
                }
            }
        }
    } 
}

