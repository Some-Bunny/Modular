using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class Scrapper : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(Scrapper))
        {
            Name = "Scrapper",
            Description = "Game Breaking",
            LongDescription = "Allows for the scrapping of Guns, Items and Pickups into Scrap. Scrap can be traded away for upgrades.\n\nIn a perfect world, this device would break down waste materials on construction sites, and turn them into suitable materials for printing useful tools. But this is not a perfect world.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("directive_scrap"),
            Quality = ItemQuality.SPECIAL,
            
            Cooldown = 1,
            CooldownType = ItemBuilder.CooldownType.PerRoom,
            PostInitAction = PIA,
            
        };

        public static void PIA(PickupObject pickup)
        {
            pickup.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 1);
            pickup.CanBeDropped = false;
            Alexandria.Misc.CustomActions.OnPostProcessItemSpawn += OnPostProcessItemSpawn;
        }

        public static void OnPostProcessItemSpawn(DebrisObject debrisObject)
        {

        }

        public static List<Tuple<string, int>> tuples = new List<Tuple<string, int>>()
        {
            new Tuple<string, int>("IounStoneOrbitalItem", 3),
            new Tuple<string, int>("HealthPickup", 1),
            new Tuple<string, int>("AmmoPickup", 3),
            new Tuple<string, int>("KeyBulletPickup", 3),
            new Tuple<string, int>("SilencerItem", 2),
        };

        public static List<Tuple<string, int>> tuplesTypes = new List<Tuple<string, int>>()
        {
            new Tuple<string, int>("PlayerItem", 8),
            new Tuple<string, int>("PassiveItem", 8),
        };


        public int ReturnAmountBasedOnTier(PickupObject.ItemQuality itemQuality)
        {
            switch(itemQuality)
            {
                case ItemQuality.D:
                    return 2;
                case ItemQuality.C:
                    return 4;
                case ItemQuality.B:
                    return 6;
                default:
                    return 6;
            }
        }



        public override void Start()
        {
            
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }




        public override bool CanBeUsed(PlayerController user)
        {
            if (!user)
            {
                return false;
            }
            else
            {
                RoomHandler room = user.GetAbsoluteParentRoom();
                if (room != null)
                {
                    room.GetCenterCell();
                    {
                        List<DebrisObject> allDebris = StaticReferenceManager.AllDebris;
                        if (allDebris != null)
                        {
                            for (int i = 0; i < allDebris.Count; i++)
                            {
                                DebrisObject debrisObject = allDebris[i];
                                if (debrisObject && debrisObject.IsPickupObject)
                                {
                                    float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
                                    if (sqrMagnitude <= 25f)
                                    {
                                        if (Mathf.Sqrt(sqrMagnitude) < 2f)
                                        {
                                            foreach (Component component in debrisObject.GetComponents<Component>())
                                            {
                                                foreach (var entry in tuples)
                                                {
                                                    if (component.GetType().ToString() == entry.First)
                                                    {
                                                        return true;
                                                    }
                                                }
                                            }
                                            foreach (Component component in debrisObject.GetComponents<Component>())
                                            {
                                                foreach (var entry in tuplesTypes)
                                                {
                                                    if (component.GetType().GetBaseType().ToString() == entry.First)
                                                    {
                                                        return true;
                                                    }
                                                }
                                            }
                                            foreach (Component component in debrisObject.GetComponentsInChildren<Component>())
                                            {
                                                if (component is Gun)
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        IPlayerInteractable lastInteractable = user.GetLastInteractable();
                        if (lastInteractable is HeartDispenser)
                        {
                            HeartDispenser exists = lastInteractable as HeartDispenser;
                            if (exists && HeartDispenser.CurrentHalfHeartsStored > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
                
                return false;
            }
        }

        public override void DoEffect(PlayerController user)
        {
            IPlayerInteractable lastInteractable = user.GetLastInteractable();
            if (lastInteractable != null)
            {
                if (lastInteractable is HeartDispenser)
                {
                    HeartDispenser exists = lastInteractable as HeartDispenser;
                    if (exists && HeartDispenser.CurrentHalfHeartsStored > 0)
                    {
                        if (HeartDispenser.CurrentHalfHeartsStored > 1)
                        {
                            HeartDispenser.CurrentHalfHeartsStored -= 2;
                        }
                        else
                        {
                            HeartDispenser.CurrentHalfHeartsStored--;
                        }
                        return;
                    }
                }
            }

            if (StaticReferenceManager.AllDebris != null)
            {
                //float num = float.MaxValue;
                for (int i = 0; i < StaticReferenceManager.AllDebris.Count; i++)
                {
                    DebrisObject debrisObject2 = StaticReferenceManager.AllDebris[i];
                    bool isPickupObject = debrisObject2.IsPickupObject;
                    if (isPickupObject)
                    {
                        float sqrMagnitude = (user.CenterPosition - debrisObject2.transform.position.XY()).sqrMagnitude;
                        if (sqrMagnitude <= 25f)
                        {
                            if (Mathf.Sqrt(sqrMagnitude) < 2f)
                            {
                                foreach (Component component in debrisObject2.GetComponents<Component>())
                                {
                                    foreach (var entry in tuples)
                                    {
                                        if (component.GetType().ToString() == entry.First)
                                        {
                                            int amount = entry.Second;
                                            float f = BraveUtility.RandomAngle();
                                            for (int e = 0; e < entry.Second; e++)
                                            {
                                                LootEngine.SpawnItem(PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject, debrisObject2.sprite.WorldCenter, Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, entry.Second, e), 1), entry.Second > 1 ? 2 : 1);
                                            }
                                            Destroy(debrisObject2.gameObject);
                                            return;
                                        }
                                    }
                                }
                                foreach (Component component in debrisObject2.GetComponents<Component>())
                                {
                                    foreach (var entry in tuplesTypes)
                                    {
                                        if (component.GetType().GetBaseType().ToString() == entry.First)
                                        {
                                            int amount = ReturnAmountBasedOnTier((debrisObject2.GetComponent(entry.First) as PickupObject).quality);
                                            float f = BraveUtility.RandomAngle();
                                            for (int e = 0; e < amount; e++)
                                            {
                                                LootEngine.SpawnItem(PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject, debrisObject2.sprite.WorldCenter, Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, amount, e), 1), amount > 1 ? 2 : 1);
                                            }
                                            Destroy(debrisObject2.gameObject);
                                        }
                                    }
                                }
                                foreach (Component component in debrisObject2.GetComponentsInChildren<Component>())
                                {
                                    if (component is Gun a)
                                    {
                                        float f = BraveUtility.RandomAngle();
                                        int amount = ReturnAmountBasedOnTier(a.quality);
                                        for (int e = 0; e < amount; e++)
                                        {
                                            LootEngine.SpawnItem(PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject, debrisObject2.sprite.WorldCenter, Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, amount, e), 1), 2);
                                        }
                                        Destroy(debrisObject2.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

