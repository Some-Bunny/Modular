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
    public class BlockOfCheese : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BlockOfCheese))
        {
            Name = "Block Of Cheese",
            Description = "Smells Great!",
            LongDescription = "A chunk of finely aged cheese.\n\nYou can't eat it.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("cheese"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {

            CheeseID = v.PickupObjectId;
            v.CanBeDropped = true;
            v.CustomCost = 40;
            var entry = CustomSynergies.Add("Resourcefuller Indeed", new List<string>() 
            {
                "mdl:block_of_cheese",
                "partially_eaten_cheese",
                "resourceful_sack",
                "rat_boots"
            });
            entry.bonusSynergies = new List<CustomSynergyType>() { CustomSynergyType.RESOURCEFUL_RAT };
            FakePrefab.MakeFakePrefab(v.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(v.gameObject);
            v.gameObject.SetActive(false);
        }
        public static int CheeseID;
    }
}

