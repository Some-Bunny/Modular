using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using SaveAPI;
using System;
using System.Collections;
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
            CooldownType = ItemBuilder.CooldownType.Timed,
            PostInitAction = PIA,
            
        };

        public static void PIA(PickupObject pickup)
        {
            pickup.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 1);
            pickup.CanBeDropped = false;
            //Alexandria.Misc.CustomActions.OnPostProcessItemSpawn += OnPostProcessItemSpawn;

            GameObject VFX = new GameObject("Scrapper_VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("scrapper_scrap_007"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("ScrapperAnimation").GetComponent<tk2dSpriteAnimation>();
            Tk2dSpriteAnimatorUtility.AddEventTriggersToAnimation(tk2dAnim, "start", new Dictionary<int, string>()
            {
                {8, "DoSparks"},
                {26, "SpitOutScrap"},

            });
            ScrapVFX = VFX;

            Sparkticle = Module.ModularAssetBundle.LoadAsset<GameObject>("Spark Particle System");
        }
        public static GameObject ScrapVFX;
        public static GameObject Sparkticle;

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

        private void DoSpawnVFX(tk2dBaseSprite sprite, int ScrapCount)
        {

            Vector3 position = sprite.WorldBottomLeft;

            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = sprite.gameObject.layer;

            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            gameObject.transform.position = position;

            tk2dSprite.SetSprite(sprite.sprite.Collection, sprite.sprite.spriteId);
            Destroy(sprite.gameObject);


            var Tk2dAnimator = UnityEngine.Object.Instantiate(ScrapVFX, position, Quaternion.identity).GetComponent<tk2dSpriteAnimator>();
            AkSoundEngine.PostEvent("Play_CHR_muncher_eat_01", Tk2dAnimator.gameObject);

            Tk2dAnimator.PlayAndDestroyObject("start");
            this.StartCoroutine(MoveScrap(gameObject, 0.5f, Tk2dAnimator.sprite.WorldTopLeft + new Vector2(0, 0.5f)));

            Tk2dAnimator.AnimationEventTriggered = (animator, clip, idX) =>
            {
                if (clip.GetFrame(idX).eventInfo.Contains("DoSparks"))
                {
                    AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_loop_01", Tk2dAnimator.gameObject);
                    this.StartCoroutine(MoveScrap(gameObject, 1f, Tk2dAnimator.sprite.WorldCenter - new Vector2(0, 0.25f), 1, 0, true));
                    var particle = UnityEngine.Object.Instantiate(Sparkticle, Tk2dAnimator.sprite.WorldCenter, Quaternion.identity);
                    particle.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                    Destroy(particle, 3);
                }
                if (clip.GetFrame(idX).eventInfo.Contains("SpitOutScrap"))
                {
                    AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", Tk2dAnimator.gameObject);
                    float f = BraveUtility.RandomAngle();
                    for (int e = 0; e < ScrapCount; e++)
                    {
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject, Tk2dAnimator.sprite.WorldTopCenter - new Vector2(0.125f, 0.375f), Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, ScrapCount, e), 1), ScrapCount > 1 ? 2 : 0, true, true);
                    }
                }
            };
        }

        public IEnumerator MoveScrap(GameObject spriteObject, float duration, Vector2 newPosition, float scaleOld = 1, float scaleNew = 1, bool Destroyed = false)
        {
            Vector2 oldPosition = spriteObject.transform.PositionVector2();
            float e = 0;
            while (e < duration)
            {
                e += BraveTime.DeltaTime;
                spriteObject.transform.position = Vector2.Lerp(oldPosition, newPosition, Toolbox.SinLerpTValue(e / duration));
                spriteObject.transform.localScale = Vector3.one * Mathf.Lerp(scaleOld, scaleNew, Toolbox.SinLerpTValue(e / duration));
                yield return null;
            }
            if (Destroyed == true)
            {
                Destroy(spriteObject);
            }
            yield break;
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
                                            DoSpawnVFX(debrisObject2.sprite, amount);
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
                                            DoSpawnVFX(debrisObject2.sprite, amount);
                                        }
                                    }
                                }
                                foreach (Component component in debrisObject2.GetComponentsInChildren<Component>())
                                {
                                    if (component is Gun a)
                                    {
                                        float f = BraveUtility.RandomAngle();
                                        int amount = ReturnAmountBasedOnTier(a.quality);
                                        DoSpawnVFX(a.sprite, amount);
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

