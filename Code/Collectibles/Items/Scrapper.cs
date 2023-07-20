using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using ModularMod.Code.Hooks;
using MonoMod.RuntimeDetour;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
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
            LongDescription = "Allows you to open an interface, letting you see and power your modules.\n\nPressing reload on a full clip switches modes, the other which allows for the scrapping of Items and Pickups into Scrap. Scrap can be repurposed into upgrades.\n\nIn a perfect world, this device would break down waste materials on construction sites, and turn them into suitable materials for printing useful tools. But this is not a perfect world.",
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

            Up_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonUp"), "Up_B_UI_INV");
            Down_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonDown"), "Down_B_UI_INV");
            Left_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonLeft"), "Left_B_UI_INV"); 
            Right_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonRight"), "Right_B_UI_INV");     
            Close_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("Cancel"), "Close_B_UI_INV");

            Googly_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("GooglyMoogly"), "GooglyMoogly_B_UI_INV");
            Power_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("Power"), "Power_B_UI_INV");


            UpBright_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonUpBright"), "UpBright_B_UI_INV");
            DownBright_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonDownBright"), "DownBright_B_UI_INV");
            LeftBright_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonLeftBright"), "LeftBright_B_UI_INV");
            RightBright_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("ButtonRightBright"), "RightBright_B_UI_INV");
            CloseBright_Button_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("CanceBrightl"), "CloseBright_B_UI_INV");

            Clock_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("Clock"), "Clock_B_UI_INV");

            Googly_Bright_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("GooglyMooglyBright"), "GooglyMooglyBright_B_UI_INV");

            T1BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_1"), "T1B_B_UI_INV");
            T2BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_2"), "T2B_B_UI_INV");
            T3BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_3"), "T3B_B_UI_INV");
            T4BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_4"), "T4B_B_UI_INV");

            Info_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("Info"), "Info_B_UI_INV");
            Info_Bright_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("Info_Hover"), "Info_B_UI_INV");

            ID = pickup.PickupObjectId;

            new Hook(typeof(HealthPickup).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public), typeof(Scrapper).GetMethod("Start_HP"));
            new Hook(typeof(KeyBulletPickup).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Scrapper).GetMethod("Start_Key"));
            new Hook(typeof(AmmoPickup).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Scrapper).GetMethod("Start_Ammo"));
            new Hook(typeof(SilencerItem).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic), typeof(Scrapper).GetMethod("Start_Blank"));
            Actions.OnActiveItemDropped += OnThisActiveDropped;
        }


        public static void OnThisActiveDropped(PlayerItem active, PlayerController player)
        {
            if (active.PickupObjectId == Scrapper.ID)
            {
                active.Pickup(player);          
                (active as Scrapper).SetMode(Mode.COMPUTER);        
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

        public static string Up_Button_UI_String;
        public static string Down_Button_UI_String;
        public static string Left_Button_UI_String;
        public static string Right_Button_UI_String;
        public static string Close_Button_UI_String;

        public static string Googly_UI_String;
        public static string Power_UI_String;

        public static string UpBright_Button_UI_String;
        public static string DownBright_Button_UI_String;
        public static string LeftBright_Button_UI_String;
        public static string RightBright_Button_UI_String;
        public static string CloseBright_Button_UI_String;

        public static string Googly_Bright_UI_String;

        public static string Clock_UI_String;

        public static string Info_UI_String;
        public static string Info_Bright_UI_String;


        public static string T1BS;
        public static string T2BS;
        public static string T3BS;
        public static string T4BS;


        public static string ReturnButtonString(ButtonUI moduleTier)
        {
            switch (moduleTier)
            {
                case ButtonUI.UP:
                    return "[sprite \"" + Up_Button_UI_String + "\"]";
                case ButtonUI.DOWN:
                    return "[sprite \"" + Down_Button_UI_String + "\"]";
                case ButtonUI.LEFT:
                    return "[sprite \"" + Left_Button_UI_String + "\"]";
                case ButtonUI.RIGHT:
                    return "[sprite \"" + Right_Button_UI_String + "\"]";
                case ButtonUI.CLOSE:
                    return "[sprite \"" + Close_Button_UI_String + "\"]";
                case ButtonUI.GOOGLY:
                    return "[sprite \"" + Googly_UI_String + "\"]";
                case ButtonUI.POWER:
                    return "[sprite \"" + "Power_B_UI_INV" + "\"]";
                case ButtonUI.CLOCK:
                    return "[sprite \"" + Clock_UI_String + "\"]";
                case ButtonUI.INFO:
                    return "[sprite \"" + Info_UI_String + "\"]";
                default: return "|[sprite \"" + Close_Button_UI_String + "\"]"; ;
            }
        }
        public static string ReturnButtonStringBright(ButtonUIBright moduleTier)
        {
            switch (moduleTier)
            {
                case ButtonUIBright.UP_BRIGHT:
                    return "[sprite \"" + UpBright_Button_UI_String + "\"]";
                case ButtonUIBright.DOWN_BRIGHT:
                    return "[sprite \"" + DownBright_Button_UI_String + "\"]";
                case ButtonUIBright.LEFT_BRIGHT:
                    return "[sprite \"" + LeftBright_Button_UI_String + "\"]";
                case ButtonUIBright.RIGHT_BRIGHT:
                    return "[sprite \"" + RightBright_Button_UI_String + "\"]";
                case ButtonUIBright.CLOSE_BRIGHT:
                    return "[sprite \"" + CloseBright_Button_UI_String + "\"]";
                case ButtonUIBright.GOOGLY_BRIGHT:
                    return "[sprite \"" + Googly_UI_String + "\"]";
                case ButtonUIBright.T1B:
                    return "[sprite \"" + T1BS + "\"]";
                case ButtonUIBright.T2B:
                    return "[sprite \"" + T2BS + "\"]";
                case ButtonUIBright.T3B:
                    return "[sprite \"" + T3BS + "\"]";
                case ButtonUIBright.T4B:
                    return "[sprite \"" + T4BS + "\"]";
                case ButtonUIBright.INFO_BRIGHT:
                    return "[sprite \"" + Info_Bright_UI_String + "\"]";
                default: return "[sprite \"" + Close_Button_UI_String + "\"]"; ;
            }
        }

        public enum ButtonUI
        {
            UP, DOWN, LEFT, RIGHT, CLOSE, GOOGLY, POWER, CLOCK, INFO
        }
        public enum ButtonUIBright
        {
            UP_BRIGHT, DOWN_BRIGHT, LEFT_BRIGHT, RIGHT_BRIGHT, CLOSE_BRIGHT, GOOGLY_BRIGHT,
            T1B, T2B, T3B, T4B, INFO_BRIGHT
        }

        public static GameObject ScrapVFX;
        public static GameObject Sparkticle;


        public static int ReturnPickupScrapValue(PickupObject obj)
        {
            int c = -1;
            if (obj is IounStoneOrbitalItem) {c = 2; }
            if (obj is HealthPickup) { c = (int)((obj as HealthPickup).healAmount * 2) + (obj as HealthPickup).armorAmount; }
            if (obj is AmmoPickup)  { c = Amo((obj as AmmoPickup).mode);}
            if (obj is KeyBulletPickup) { c = 1 + (obj as KeyBulletPickup).numberKeyBullets + ((obj as KeyBulletPickup).IsRatKey ? 5 : 0); }
            if (obj is SilencerItem) { c = 2; }
            if (obj is PassiveItem) { c = (obj as PassiveItem).Owner ? -1 : ReturnAmountBasedOnTier(obj.quality); }
            if (obj is PlayerItem) { c = (obj as PlayerItem).LastOwner  ? -1 : ReturnAmountBasedOnTier(obj.quality); }
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

        public override void Update()
        {
            if (base.LastOwner != null && Inited == false)
            {
                Inited = true;
                SetMode(Mode.COMPUTER);
                base.LastOwner.OnReloadPressed += ReloadPressed;
                base.LastOwner.startingActiveItemIds.Add(this.PickupObjectId);
            }
            base.Update();
        }

        public override void OnPreDrop(PlayerController user)
        {
            Inited = false;
            base.LastOwner.OnReloadPressed -= ReloadPressed;
            base.LastOwner = null;
            base.OnPreDrop(user);
        }

        private bool Inited = false;

        public void SwitchMode()
        {
            AkSoundEngine.PostEvent("Play_OBJ_metacoin_collect_01", base.gameObject);
            if (currentMode == Mode.SCRAP)
            {
                this.sprite.SetSprite(StaticCollections.Item_Collection.GetSpriteIdByName("computer_core"));
                currentMode = Mode.COMPUTER;
            }
            else
            {
                this.sprite.SetSprite(StaticCollections.Item_Collection.GetSpriteIdByName("directive_scrap"));
                currentMode = Mode.SCRAP;
            }
        }
        public void SetMode(Mode mode)
        {
            currentMode = mode;
            if (currentMode == Mode.SCRAP)
            {
                this.sprite.SetSprite(StaticCollections.Item_Collection.GetSpriteIdByName("directive_scrap"));
            }
            else
            {
                this.sprite.SetSprite(StaticCollections.Item_Collection.GetSpriteIdByName("computer_core"));
            }
        }



        public Mode currentMode = Mode.SCRAP;

        public enum Mode
        {
            SCRAP,
            COMPUTER
        }



        public override bool CanBeUsed(PlayerController user)
        {
            if (!user)
            {
                return false;
            }
            else
            {
                if (currentMode == Mode.SCRAP)
                {
                    for (int i = allPickups.Count - 1; i > -1; i--)
                    {
                        PickupObject pickup = allPickups[i];
                        if (pickup == null) { allPickups.RemoveAt(i); } else 
                        {
                            float sqrMagnitude = (user.CenterPosition - pickup.transform.position.XY()).sqrMagnitude;
                            if (sqrMagnitude <= 25f)
                            {
                                if (Mathf.Sqrt(sqrMagnitude) < 2f)
                                {
                                    int scrap = ReturnPickupScrapValue(pickup);
                                    if (scrap > 0)
                                    {
                                        return true;
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
                            if (debrisObject && debrisObject.IsPickupObject)
                            {

                                float sqrMagnitude = (user.CenterPosition - debrisObject.transform.position.XY()).sqrMagnitude;
                                if (sqrMagnitude <= 25f)
                                {
                                    if (Mathf.Sqrt(sqrMagnitude) < 2f)
                                    {
                                        int scrap = ReturnPickupScrapValue(debrisObject.GetComponent<PickupObject>());
                                        if (scrap > 0)
                                        {
                                            
                                            if (!allPickups.Contains(debrisObject.GetComponent<PickupObject>())) { allPickups.Add(debrisObject.GetComponent<PickupObject>()); }
                                            return true;
                                        }
                                        foreach (Component component in debrisObject.GetComponentsInChildren<Component>())
                                        {
                                            if (component is Gun)
                                            {
                                                Gun peepee = (component as Gun);
                                                int scrapGun = ReturnPickupScrapValue(peepee);
                                                if (scrapGun > 0)
                                                {

                                                    if (!allPickups.Contains(peepee)) { allPickups.Add(peepee); }
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }           
                            }
                        }
                    }
                   
                }
                else
                {
                    return user.IsInCombat == true ? false : true;
                }
                return false;
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
                        var thing = LootEngine.SpawnItem(PickupObjectDatabase.GetById(Scrap.Scrap_ID).gameObject, Tk2dAnimator.sprite.WorldTopCenter - new Vector2(0.125f, 0.375f), Toolbox.GetUnitOnCircle(Toolbox.SubdivideCircle(f, ScrapCount, e), 1), ScrapCount > 1 ? 2 : 0, true, true);
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


        public override void DoEffect(PlayerController user)
        {
            if (currentMode == Mode.SCRAP)
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
                                    DoSpawnVFX(pickup.sprite, scrap);
                                }
                            }
                        }
                    }          
                }
            }
            else
            {
                if (extant_Inventory_Controller != null)
                {
                    extant_Inventory_Controller.ObliterateUI();
                    extant_Inventory_Controller = null;
                }
                extant_Inventory_Controller = ScriptableObject.CreateInstance<ModuleInventoryController>();
                extant_Inventory_Controller.DoQuickStart(user);
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
            }
        }




        public ModifiedDefaultLabelManager extant_Inventory_button;
        public ModifiedDefaultLabelManager extant_craft_button;
        public ModifiedDefaultLabelManager extant_close_button;


        public ModuleInventoryController extant_Inventory_Controller;
        public ModuleCrafingController extant_Crafting_Controller;

    }
}

