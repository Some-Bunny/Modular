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
    public class TheScavenger : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TheScavenger))
        {
            Name = "The Scavenger",
            Description = "Pointy",
            LongDescription = "Grants 1 (+1 per stack) additional scrap when scrapping items or pickups. Grants 1 (+1 per stack) shots to your clip and +5% movement speed per Scrap.\nVery slightly increase Scrap drops from room clear." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("scavenger_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("scavenger_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "The Scavenger " + h.ReturnTierLabel();
            h.LabelDescription = "Grants 1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") additional scrap when scrapping items or pickups.\nGrants 1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") shots to your clip\nand +5% movement speed per Scrap.\nVery slightly increase Scrap drops from room clear.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.AdditionalWeightMultiplier *= 0.75f;

            ID = h.PickupObjectId;
            ValidRoomRewardContents = new Alexandria.RoomRewardAPI.ValidRoomRewardContents()
            {
                overrideItemPool = new List<Tuple<float, int>>()
                {
                    new Tuple<float, int>(0.1f, Scrap.Scrap_ID),
                    new Tuple<float, int>(0.01f, CraftingCore.CraftingCoreID),

                }
            };
        }
        public static int ID;
        public static Projectile LanceBeam;
        public static Alexandria.RoomRewardAPI.ValidRoomRewardContents ValidRoomRewardContents;




        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.ModifyScrapContext += MSC;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Post_Calculation_ClipSize_Process = ProcessClipSize
            };
            modularGunController.statMods.Add(this.gunStatModifier);
            modulePrinter.VoluntaryMovement_Modifier += MovementMod;
            Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents += OnDetermineContents;
        }
        public Vector2 MovementMod(Vector2 currentVel, ModulePrinterCore core, PlayerController p)
        {
            return currentVel *= 1 + (0.05f * (p.GetComponent<ConsumableStorage>().GetConsumableOfName("Scrap")));
        }


        public int MSC(int amountOfScrap, ModulePrinterCore core, PlayerController player, Scrapper scrapper)
        {
            return amountOfScrap += this.ReturnStack(core);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (modularGunController && gunStatModifier != null && modularGunController.statMods.Contains(this.gunStatModifier)) { modularGunController.statMods.Remove(this.gunStatModifier); }
            modulePrinter.ModifyScrapContext -= MSC;
            modulePrinter.VoluntaryMovement_Modifier -= MovementMod;
            Alexandria.RoomRewardAPI.OnRoomRewardDetermineContents -= OnDetermineContents;


        }
        public void OnDetermineContents(RoomHandler room, Alexandria.RoomRewardAPI.ValidRoomRewardContents validRoomReward, float f)
        {
            validRoomReward.additionalRewardChance -= (float)(0.01f + (0.01f * Stack()));
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

        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + (player.GetComponent<ConsumableStorage>().GetConsumableOfName("Scrap") * this.ReturnStack(modulePrinterCore));
        }
    }
}

