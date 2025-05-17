using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;

namespace ModularMod
{
    public class RNGManipulation : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RNGManipulation))
        {
            Name = "RNG Manipulation",
            Description = "Just Really Lucky",
            LongDescription = "Grants vastly improved and more common room drops (+Loot chance and better loot drop odds per stack).\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("rngmanipulation_t2_module"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("rngmanipulation_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "RNG Manipulation " + h.ReturnTierLabel();
            h.LabelDescription = "Grants vastly improved and more common room drops.\n(" + StaticColorHexes.AddColorToLabelString("+Loot chance and better loot drop odds", StaticColorHexes.Light_Orange_Hex) + ").";
            h.EnergyConsumption = 1;
            h.AppearsInRainbowMode = false;

            h.AddModuleTag(BaseModuleTags.GENERATION);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.AdditionalWeightMultiplier = 0.8f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.OverrideScrapCost = 16;

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
            ValidRoomRewardContents = new Alexandria.RoomRewardAPI.ValidRoomRewardContents()
            {
                overrideItemPool = new List<Tuple<float, int>>() 
                {
                    new Tuple<float, int>(0.5f, 120),
                    new Tuple<float, int>(0.33f, 565),
                    new Tuple<float, int>(0.033f, 297),
                    new Tuple<float, int>(0.5f, 70),
                    new Tuple<float, int>(0.3f, 67),
                    new Tuple<float, int>(0.05f, 137),
                    new Tuple<float, int>(0.15f, Scrap.Scrap_ID),
                    new Tuple<float, int>(0.02f, CraftingCore.CraftingCoreID),

                }
            };
            h.StartCoroutine(FrameDelay());
        }
        public static IEnumerator FrameDelay()
        {
            yield return null;
            foreach (var t1_modules in GlobalModuleStorage.all_Tier_1_Modules)
            {
                ValidRoomRewardContents.overrideItemPool.Add(new Tuple<float, int>(0.0005f, t1_modules.PickupObjectId));
            }
            yield break;
        }

        public static Alexandria.RoomRewardAPI.ValidRoomRewardContents ValidRoomRewardContents;
        public static int ID;
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
        }
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents += OnDetermineContents;
            Alexandria.RoomRewardAPI.OnRoomClearItemDrop += OnRoomClearItemDrop;
        }
        public void OnRoomClearItemDrop(DebrisObject debrisObject, RoomHandler room)
        {
            AkSoundEngine.PostEvent("Play_OBJ_dice_bless_01", debrisObject.gameObject);
            if (ConfigManager.DoVisualEffect == true)
            {
                UnityEngine.Object.Instantiate(VFXStorage.TeleportDistortVFX, debrisObject.sprite.WorldCenter, Quaternion.identity);
            }
        }
        public void OnDetermineContents(RoomHandler room, Alexandria.RoomRewardAPI.ValidRoomRewardContents validRoomReward, float f)
        {
            validRoomReward.additionalRewardChance -= (float)((0.05f * Stack()));
            validRoomReward.overrideItemPool.AddRange(ReturnThing());
        }

        public List<Tuple<float, int>> ReturnThing()
        {
            var copy = new Alexandria.RoomRewardAPI.ValidRoomRewardContents();
            copy.overrideItemPool = new List<Tuple<float, int>>();
            copy.overrideItemPool.AddRange(ValidRoomRewardContents.overrideItemPool);
            foreach (var entry in copy.overrideItemPool)
            {
                entry.First *= Stack();
            }
            return copy.overrideItemPool;
        }





        public override void OnCoreDestruction(ModulePrinterCore modulePrinter, ModularGunController modularGunController)
        {
            Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents -= OnDetermineContents;
            Alexandria.RoomRewardAPI.OnRoomClearItemDrop -= OnRoomClearItemDrop;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents -= OnDetermineContents;
            Alexandria.RoomRewardAPI.OnRoomClearItemDrop -= OnRoomClearItemDrop;
        }
    }
}

