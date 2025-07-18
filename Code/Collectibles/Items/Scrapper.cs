﻿using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using ModularMod.Code.Components.Misc_Components;
using ModularMod.Code.Hooks;
using MonoMod.RuntimeDetour;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static ModularMod.DefaultModule;
using static ModularMod.ModulePrinterCore;


namespace ModularMod
{
    public class Scrapper : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(Scrapper))
        {
            Name = "Module Computer Core",
            Description = "Game Making",
            LongDescription = "Allows you to open an interface, letting you see and power your modules.\n\nHolding the 'Reload' button switches modes, the other which allows for the scrapping of Items and Pickups into Scrap. Scrap can be repurposed into upgrades.\n\nIn a perfect world, this device would break down waste materials on construction sites, and turn them into suitable materials for printing useful tools. But this is not a perfect world.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("computer_core"),
            Quality = ItemQuality.SPECIAL,         
            Cooldown = 1,
            CooldownType = ItemBuilder.CooldownType.Timed,
            PostInitAction = PIA,         
        };

        public static void PIA(PickupObject pickup)
        {
            pickup.AddPassiveStatModifier(PlayerStats.StatType.AdditionalItemCapacity, 1);
            pickup.CanBeDropped = false;
            (pickup as PlayerItem).m_baseSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("computer_core");
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


            ID = pickup.PickupObjectId;

            new Hook(typeof(HealthPickup).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public), typeof(Scrapper).GetMethod("Start_HP"));
            new Hook(typeof(KeyBulletPickup).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Scrapper).GetMethod("Start_Key"));
            new Hook(typeof(AmmoPickup).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Scrapper).GetMethod("Start_Ammo"));
            new Hook(typeof(SilencerItem).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Scrapper).GetMethod("Start_Blank"));
            Actions.OnActiveItemDropped += OnThisActiveDropped;

            GameObject ReloadHoldEffect_ = new GameObject("SwitchModeTuah");
            FakePrefab.DontDestroyOnLoad(ReloadHoldEffect_);
            FakePrefab.MarkAsFakePrefab(ReloadHoldEffect_);
            var tk2d_ = ReloadHoldEffect_.AddComponent<tk2dSprite>();
            tk2d_.Collection = StaticCollections.VFX_Collection;
            tk2d_.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("modularswitchmode_007"));
            var tk2dAnim_ = ReloadHoldEffect_.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim_.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("GenericVFXAnimation").GetComponent<tk2dSpriteAnimation>();

            tk2d_.usesOverrideMaterial = true;
            tk2d_.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d_.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d_.renderer.material.SetFloat("_EmissivePower", 4);
            tk2d_.renderer.material.SetFloat("_EmissiveColorPower", 4);

            ReloadHoldEffect = ReloadHoldEffect_;
        }
        public static GameObject ReloadHoldEffect;

        public static void OnThisActiveDropped(PlayerItem active, PlayerController player)
        {
            if (active.PickupObjectId == Scrapper.ID)
            {
                active.Pickup(player);          
                (active as Scrapper).SetMode("COMPUTER");        
            }
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        public static void Start_HP(Action<HealthPickup> orig, HealthPickup self)
        {
            orig(self);
            if (!allPickups.Contains(self)) { allPickups.Add(self); }
        }

        public static void Start_Key(Action<KeyBulletPickup> orig, KeyBulletPickup self)
        {
            orig(self);
            if (!allPickups.Contains(self)) { allPickups.Add(self); }
        }
        public static void Start_Ammo(Action<AmmoPickup> orig, AmmoPickup self)
        {
            orig(self);
            if (!allPickups.Contains(self)) { allPickups.Add(self); }
        }
        public static void Start_Blank(Action<SilencerItem> orig, SilencerItem self)
        {
            orig(self);
            if (!allPickups.Contains(self)) { allPickups.Add(self); }
        }


        public static List<PickupObject> allPickups = new List<PickupObject>();

        public static int ID;

        public static GameObject ScrapVFX;
        public static GameObject Sparkticle;


        public static int ReturnPickupScrapValue(PickupObject obj)
        {
            int c = -1;
            if (obj == null) { return c; }

            if (obj is HealthPickup) { c = (int)((obj as HealthPickup).healAmount * 2) + (obj as HealthPickup).armorAmount; }
            if (obj is AmmoPickup)  { c = Amo((obj as AmmoPickup).mode);}
            if (obj is KeyBulletPickup) { c = 1 + (obj as KeyBulletPickup).numberKeyBullets + ((obj as KeyBulletPickup).IsRatKey ? 5 : 0); }
            if (obj is SilencerItem) { c = 2; }
            if (obj is PassiveItem) { c = (obj as PassiveItem).Owner ? -1 : ReturnAmountBasedOnTier(obj.quality); }
            if (obj is PlayerItem) { c = (obj as PlayerItem).LastOwner  ? -1 : ReturnAmountBasedOnTier(obj.quality); }
            if (obj.PickupObjectId == 565) { c = 1; }
            if (obj.PickupObjectId == 127) { c = 1; }
            if (obj.PickupObjectId == 148) { c = 1; }
            if (obj.PickupObjectId == 641) { c = 5; }

            if (OverrideCustomScrapCheck != null) 
            {
                c = OverrideCustomScrapCheck(obj, c);
            }
            return c;
        }
        private static int Amo(AmmoPickup.AmmoPickupMode a)
        {
            switch (a)
            {
                case AmmoPickup.AmmoPickupMode.FULL_AMMO:
                    return 2;
                case AmmoPickup.AmmoPickupMode.SPREAD_AMMO:
                    return 1;
                case AmmoPickup.AmmoPickupMode.ONE_CLIP:
                    return 1;
            }
            return -1;
        }
        public static Func<PickupObject, int, int> OverrideCustomScrapCheck;


        public override void Start()
        {

        }

        public static int ReturnAmountBasedOnTier(PickupObject.ItemQuality itemQuality)
        {
            switch(itemQuality)
            {
                case ItemQuality.COMMON:
                    return 1;
                case ItemQuality.D:
                    return 1;
                case ItemQuality.C:
                    return 1;
                case ItemQuality.B:
                    return 2;
                default:
                    return 2;
            }
        }





        public void ReloadPressed(PlayerController p, Gun g)
        {
            if (g.ClipCapacity > g.ClipShotsRemaining) { return; }
            if (p.CurrentItem != this) { return; }
            SwitchMode();
        }
        private GameObject EffectInst;
        private float TimerToSwitch = 0;

        public override void Update()
        {
            if (base.LastOwner != null)
            {
                if (Inited == false)
                {
                    Inited = true;
                    SetMode("COMPUTER");
                    //base.LastOwner.OnReloadPressed += ReloadPressed;
                    base.LastOwner.OnNewFloorLoaded += ONFE;
                    ONFE(base.LastOwner);
                    base.LastOwner.startingActiveItemIds.Add(this.PickupObjectId);
                }
                bool wasPressed = LastOwner.m_activeActions.ReloadAction.State;
                if (wasPressed && LastOwner.CurrentItem == this)
                {
                    if (EffectInst == null && TimerToSwitch > 0.25f)
                    {
                        EffectInst = LastOwner.PlayEffectOnActor(ReloadHoldEffect, new Vector3(0, 0.75f));
                        EffectInst.gameObject.layer = 22;
                        EffectInst.GetComponent<tk2dSpriteAnimator>().Play(this.LastOwner.IsUsingAlternateCostume ? "moduleswitchmodealt" : "moduleswitchmode");

                    }
                    TimerToSwitch += Time.deltaTime;
                    if (TimerToSwitch >= 1.25f)
                    {
                        if (EffectInst != null)
                        {
                            Destroy(EffectInst);
                            EffectInst = null;
                        }

                        if (LastOwner.CurrentItem != this) { return; }
                        SwitchMode();
                        TimerToSwitch = 0;
                    }
                }
                else
                {
                    if (EffectInst != null)
                    {
                        Destroy(EffectInst);
                        EffectInst = null;
                    }
                    TimerToSwitch = 0;
                }
            }
            base.Update();
        }

        private IEnumerator Wait()
        {
            yield return null;
            if (GameManager.Instance.Dungeon.data.Entrance.hierarchyParent == null) { yield break; }
            gunberMunchers = new List<GunberMuncherController>();

            GunberMuncherController[] muncher = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<GunberMuncherController>(true);
            if (muncher != null && muncher.Length > 0)
            {
                foreach (GunberMuncherController shope in muncher)
                {
                    gunberMunchers.Add(shope);
                }
            }
            yield break;
        }


        public void ONFE(PlayerController playerController)
        {
            playerController.StartCoroutine(Wait());
        }

        public List<GunberMuncherController> gunberMunchers;

        public override void OnPreDrop(PlayerController user)
        {
            Inited = false;
            //base.LastOwner.OnReloadPressed -= ReloadPressed;
            base.LastOwner = null;
            base.OnPreDrop(user);
        }

        private bool Inited = false;

        public void SwitchMode()
        {
            AkSoundEngine.PostEvent("Play_OBJ_metacoin_collect_01", base.gameObject);
            Mode++;
            if (Mode == currentModes.Count())
            {
                Mode = 0;
            }
            var ActiveMode = currentModes[Mode];
            currentMode = ActiveMode.Mode_Name;
            this.sprite.SetSprite(ActiveMode.spriteCollection.GetSpriteIdByName(ActiveMode.Sprite_Name));
        }
        public void SetMode(string mode)
        {
            foreach (var entry in currentModes)
            {
                if (mode == entry.Mode_Name)
                {
                    Mode = currentModes.FindIndex(self => self.Mode_Name == entry.Mode_Name);
                    currentMode = entry.Mode_Name;
                    this.sprite.SetSprite(entry.spriteCollection.GetSpriteIdByName(entry.Sprite_Name));
                }
            }
        }

        public void AddMode(ActiveItemMode mode)
        {
            var m = currentModes.Where(self => self.Mode_Name == mode.Mode_Name);
            if (m.Count() > 0) { return; }
            currentModes.Add(mode);
        }

        public void RemoveMode(ActiveItemMode mode)
        {
            var m = currentModes.Where(self => self.Mode_Name == mode.Mode_Name);
            if (m.Count() == 0) { return; }
            if (mode.Removable == false) { return; }
            currentModes.Remove(mode);
            if (currentMode == mode.Mode_Name) { SwitchMode(); }

        }


        private int Mode = 0;

        public string currentMode = "SCRAP";
        public struct ActiveItemMode
        {
            public tk2dSpriteCollectionData spriteCollection;
            public string Sprite_Name;
            public string Mode_Name;
            public bool Removable;
            public Func<PlayerController, Scrapper, bool> CanBeUsed;
            public Action<PlayerController, Scrapper> OnUsed;
        }

        public List<ActiveItemMode> currentModes = new List<ActiveItemMode>()
        {
            new ActiveItemMode()
            {
                Mode_Name = "COMPUTER",
                spriteCollection = StaticCollections.Item_Collection,
                Sprite_Name = "computer_core",
                Removable = false,
                CanBeUsed = ComputerCanBeUsed,
                OnUsed = DoComputerUse,
            },
            new ActiveItemMode()
            {
                Mode_Name = "SCRAP",
                spriteCollection = StaticCollections.Item_Collection,
                Sprite_Name = "directive_scrap",
                Removable = false,
                CanBeUsed = ScrapCanBeUsed,
                OnUsed = DoEffectScrap,
            },
        };
        public static bool ScrapCanBeUsed(PlayerController user, Scrapper scrapper)
        {
            

            for (int i = allPickups.Count - 1; i > -1; i--)
            {
                PickupObject pickup = allPickups[i];
                if (pickup == null) { allPickups.RemoveAt(i); }
                else
                {
                    float sqrMagnitude = (user.CenterPosition - pickup.transform.position.XY()).sqrMagnitude;
                    if (sqrMagnitude <= 25f)
                    {
                        if (Mathf.Sqrt(sqrMagnitude) < 2f)
                        {
                            int scrap = ReturnPickupScrapValue(pickup);
                            if (scrap > 0)
                            {
                                if (pickup is PassiveItem passive)
                                {
                                    if (passive.Owner == null)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            List<DebrisObject> allDebris = StaticReferenceManager.AllDebris;
            if (allDebris != null)
            {
                for (int e = 0; e < allDebris.Count; e++)
                {
                    DebrisObject debrisObject = allDebris[e];
                    float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
                    if (debrisObject && debrisObject.IsPickupObject && sqrMagnitude <= 25f && Mathf.Sqrt(sqrMagnitude) < 2f)
                    {
                        int scrap = ReturnPickupScrapValue(debrisObject.GetComponent<PickupObject>());
                        if (scrap > 0)
                        {

                            if (!allPickups.Contains(debrisObject.GetComponent<PickupObject>())) { allPickups.Add(debrisObject.GetComponent<PickupObject>()); }
                            return true;
                        }
                        Gun g  = debrisObject.GetComponentInChildren<Gun>();
                        if (g != null)
                        {
                            int scrapGun = ReturnPickupScrapValue(g);
                            if (scrapGun > 0 && g.CurrentOwner == null)
                            {
                                if (!allPickups.Contains(g)) { allPickups.Add(g); }
                                return false;
                            }
                        }
                    }
                }
            }

            for (int i = scrapper.gunberMunchers.Count - 1; i > -1; i--)
            {
                GunberMuncherController shope = scrapper.gunberMunchers[i];
                if (Vector2.Distance(shope.sprite.WorldCenter, user.sprite.WorldCenter) < 3)
                {
                    return true;
                }
            }
            return false;
        }
       

        public static bool ComputerCanBeUsed(PlayerController user, Scrapper scrapper)
        {
            return user.IsInCombat == true ? false : true;
        }

        public static void DoEffectScrap(PlayerController user, Scrapper scrapper)
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

            for (int i = allPickups.Count - 1; i > -1; i--)
            {
                PickupObject pickup = allPickups[i];
                if (pickup == null) { allPickups.RemoveAt(i); }
                else
                {

                    float sqrMagnitude = (user.CenterPosition - pickup.transform.position.XY()).sqrMagnitude;
                    if (sqrMagnitude <= 25f)
                    {
                        if (Mathf.Sqrt(sqrMagnitude) < 2f)
                        {
                            int scrap = ReturnPickupScrapValue(pickup);
                            if (scrap > 0)
                            {
                                if (pickup is PassiveItem passive)
                                {
                                    if (passive.Owner == null)
                                    {
                                        if (OnAnythingScrapped != null) { OnAnythingScrapped(pickup); }
                                        scrapper.DoSpawnVFX(pickup.sprite, scrap);
                                    }
                                }
                                else
                                {
                                    if (pickup is Gun)
                                    {
                                        Destroy(pickup.GetComponentInParent<DebrisObject>());
                                    }
                                    if (OnAnythingScrapped != null) { OnAnythingScrapped(pickup); }
                                    scrapper.DoSpawnVFX(pickup.sprite, scrap);
                                }
                            }
                        }
                    }
                }
            }
            for (int i = scrapper.gunberMunchers.Count - 1; i > -1; i--)
            {
                GunberMuncherController shope  = scrapper.gunberMunchers[i];
                if (Vector2.Distance(shope.sprite.WorldCenter, user.sprite.WorldCenter) < 3)
                {

                    var silliness = shope.gameObject.GetOrAddComponent<GunMuncherGoSilly>();
                    silliness.scrapper = scrapper;
                    silliness.muncherController = shope;
                    silliness.DoJump();
                }
            }

            
        }
        public static void DoComputerUse(PlayerController user, Scrapper scrapper)
        {
            if (scrapper.extant_Inventory_Controller != null)
            {
                scrapper.extant_Inventory_Controller.ObliterateUI();
                scrapper.extant_Inventory_Controller = null;
            }
            scrapper.extant_Inventory_Controller = ScriptableObject.CreateInstance<ModuleInventoryController>();
            scrapper.extant_Inventory_Controller.DoQuickStart(user);
        }


        public override bool CanBeUsed(PlayerController user)
        {
            if (!user)
            {
                return false;
            }
            else
            {
                if(currentModes[Mode].CanBeUsed != null)
                {
                    return currentModes[Mode].CanBeUsed(user, this);
                }
                else
                {
                    return true;
                }
            }
        }

        private void DoSpawnVFX(tk2dBaseSprite sprite, int ScrapCount)
        {
            var core = LastOwner.PlayerHasCore();
            if (core != null && core.ModifyScrapContext != null)
            {
                ScrapCount = core.ModifyScrapContext(ScrapCount, core, LastOwner, this);
            }
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
                    if (LastOwner.PlayerHasCore() != null) 
                    {
                        if (LastOwner.PlayerHasCore().OnScrapped != null)
                        {
                            LastOwner.PlayerHasCore().OnScrapped(ScrapCount, LastOwner.PlayerHasCore(), LastOwner, this);
                        }
                    }
                    AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", Tk2dAnimator.gameObject);
                    float f = BraveUtility.RandomAngle();
                    for (int e = 0; e < ScrapCount; e++)
                    {
                        var thing = LootEngine.SpawnItem(PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject, Tk2dAnimator.sprite.WorldTopCenter - new Vector2(0.125f, 0.375f), Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, ScrapCount, e), 1), ScrapCount > 1 ? 1.25f : 0, true, true);
                        thing.gameObject.SetActive(true);
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

        public static Action<PickupObject> OnAnythingScrapped;
        public override void DoEffect(PlayerController user)
        {
            if (currentModes[Mode].OnUsed != null)
            {
                currentModes[Mode].OnUsed(user, this);
            }
            else
            {
                //Debug.LogWarning("SOMEONE FORGOT TO MAKE THEIR ACTIVE MODE USABLE, YA DANGUS!");
            }

            /*
            if (currentMode == "SCRAP")
            {
                
            }
            else
            {

                /*
                if (extant_craft_button != null) { Destroy(extant_craft_button.gameObject); }
                if (extant_Inventory_button != null) { Destroy(extant_Inventory_button.gameObject); }
                if (extant_close_button != null) { Destroy(extant_close_button.gameObject); }

                if (extant_craft_button != null) { Destroy(extant_craft_button.gameObject); }
                if (extant_Inventory_Controller != null) { extant_Inventory_Controller.ObliterateUI(); 
                    extant_Inventory_Controller = null; }
                
                if (extant_Crafting_Controller != null)
                {
                    extant_Crafting_Controller.ObliterateUI();
                    extant_Crafting_Controller = null;
                }

                Color32 cl = user.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);


              


                //INVENTORY
                extant_Inventory_button = Toolbox.GenerateText(user.transform, new Vector2(1.5f, 1.5f), 0.66f, "Inventory", cl);

                extant_Inventory_button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (extant_Inventory_button) { Destroy(extant_Inventory_button.gameObject); }
                    if (extant_craft_button) { Destroy(extant_craft_button.gameObject); }
                    if (extant_close_button != null) { Destroy(extant_close_button.gameObject); }

                    extant_Inventory_Controller = ScriptableObject.CreateInstance<ModuleInventoryController>();
                    extant_Inventory_Controller.DoQuickStart(user);
                    //Inventory Code Here
                };

                extant_Inventory_button.MouseHover = (label, boolean) =>
                {
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(155, 155, 155, 155);
                    label.Invalidate();
                };

                //CRAFT
                /*
                extant_craft_button = Toolbox.GenerateText(user.transform, new Vector2(1.5f, 0.5f), 0.66f, "Crafting", cl);
                extant_craft_button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (extant_Inventory_button.gameObject) { Destroy(extant_Inventory_button.gameObject); }
                    if (extant_craft_button.gameObject) { Destroy(extant_craft_button.gameObject); }
                    if (extant_close_button != null) { Destroy(extant_close_button.gameObject); }

                    extant_Crafting_Controller = ScriptableObject.CreateInstance<ModuleCrafingController>();
                    extant_Crafting_Controller.DoQuickStart(user);
                    //Craft Code Here
                };
                extant_craft_button.MouseHover = (label, boolean) =>
                {
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(155, 155, 155, 155);
                    label.Invalidate();
                };
                */
            //CLOSE
            /*
            extant_close_button = Toolbox.GenerateText(user.transform, new Vector2(1.5f, -0.5f), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE), cl);
            extant_close_button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                if (extant_Inventory_button) { extant_Inventory_button.Inv(); }
                if (extant_craft_button) { extant_craft_button.Inv(); }
                if (extant_close_button) { extant_close_button.Inv(); }
            };  
            extant_close_button.MouseHover = (label, boolean) =>
            {
                label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(155, 155, 155, 155);
                label.Invalidate();
            };
            */
            //}

        }




        public ModifiedDefaultLabelManager extant_Inventory_button;
        public ModifiedDefaultLabelManager extant_craft_button;
        public ModifiedDefaultLabelManager extant_close_button;


        public ModuleInventoryController extant_Inventory_Controller;
        public ModuleCrafingController extant_Crafting_Controller;
    }
}

