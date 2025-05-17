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
            Description = "Sonar",
            LongDescription = "Secret Rooms are revealed if you are near them. While powered, all secret rooms contain 2 (+1 per stack) extra pickups, starting from the next floor." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("radarscanner_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("radarscanner_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.85f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Radar Scanner " + h.ReturnTierLabel();
            h.LabelDescription = "Secret Rooms are revealed if you are near them.\nWhile powered, all secret rooms contain\n2 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") extra pickups\nstarting from the next floor.";
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.GENERATION);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.OverrideScrapCost = 8;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded += Instance_OnNewLevelFullyLoaded;
            modulePrinter.PlayerEnteredAnyRoom += ORC;
        }

        public void ORC(ModulePrinterCore modulePrinter, PlayerController player, RoomHandler room)
        {
            bool playedSound = false;
            foreach (var entry in room.connectedRooms)
            {
                if (entry.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET && entry.RevealedOnMap == false)
                {
                    if (playedSound == false && ConfigManager.DoVisualEffect == true)
                    {
                        
                        GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                        GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, player.sprite.WorldCenter, Quaternion.identity);
                        blankObj.transform.localScale = Vector3.one * 5;
                        AkSoundEngine.PostEvent("Play_Radar_Ping", player.gameObject);
                        playedSound = true;
                    }
                    entry.RevealedOnMap = true;
                    Minimap.Instance.RevealMinimapRoom(entry, true, true, entry == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                }
            }
        }

        private void Instance_OnNewLevelFullyLoaded()
        {
            AddGoodies();
        }


        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= Instance_OnNewLevelFullyLoaded;
            modulePrinter.PlayerEnteredAnyRoom -= ORC;
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
                        for (int q = 0; q < 1 + Stack(); q++)
                        {
                            var pickup = LootEngine.SpawnItem(GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable.SelectByWeight(false), roomHandler.GetBestRewardLocation(new IntVector2(1, 1), RoomHandler.RewardLocationStyle.Original).ToCenterVector3(0), Vector2.zero, 0);
                            pickup.GetComponent<PickupObject>().IgnoredByRat = true;
                            if (pickup.GetComponent<PickupObject>() is Scrap)
                            {
                                pickup.gameObject.SetActive(true);
                            }
                            if (!StaticReferenceManager.AllDebris.Contains(pickup))
                            {
                                StaticReferenceManager.AllDebris.Add(pickup);
                            }
                        }
                    }
                }
            }
        }
    } 
}

