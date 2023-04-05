using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class PowerCell : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PowerCell))
        {
            Name = "Power Cell",
            Description = "Power Up!",
            LongDescription = "Increases the amount of Stored Energy by 1.\n\nA handy power cell to keep the Modular going, along with all of their installed tech.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("powerCell"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public override void Pickup(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_OBJ_power_up_01", player.gameObject);
            base.Pickup(player);
        }

        public override void Update()
        {
        }

        public static void PostInit(PickupObject v)
        {
            v.CanBeDropped = false;
            v.gameObject.AddComponent<ModulePrinterCore.AdditionalItemEnergyComponent>().AdditionalEnergy = 1;
            PowerCellID = v.PickupObjectId;
        }
        public static int PowerCellID;
    }
}

