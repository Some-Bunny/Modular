using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            Googly_Bright_UI_String = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("GooglyMooglyBright"), "GooglyMooglyBright_B_UI_INV");

            T1BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_1"), "T1B_B_UI_INV");
            T2BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_2"), "T2B_B_UI_INV");
            T3BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_3"), "T3B_B_UI_INV");
            T4BS = AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>("tier_label_4"), "T4B_B_UI_INV");
            ID = pickup.PickupObjectId;
        }
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
                default: return "[sprite \"" + Close_Button_UI_String + "\"]"; ;
            }
        }

        public enum ButtonUI
        {
            UP, DOWN, LEFT, RIGHT, CLOSE, GOOGLY, POWER,
        }
        public enum ButtonUIBright
        {
            UP_BRIGHT, DOWN_BRIGHT, LEFT_BRIGHT, RIGHT_BRIGHT, CLOSE_BRIGHT, GOOGLY_BRIGHT,
            T1B, T2B, T3B, T4B
        }

        public static GameObject ScrapVFX;
        public static GameObject Sparkticle;

        public static List<Tuple<string, int>> tuples = new List<Tuple<string, int>>()
        {
            new Tuple<string, int>("IounStoneOrbitalItem", 2),
            new Tuple<string, int>("HealthPickup", 1),
            new Tuple<string, int>("AmmoPickup", 2),
            new Tuple<string, int>("KeyBulletPickup", 2),
            new Tuple<string, int>("SilencerItem", 2),
        };

        public static List<Tuple<string, int>> tuplesTypes = new List<Tuple<string, int>>()
        {
            new Tuple<string, int>("PlayerItem", 8),
            new Tuple<string, int>("PassiveItem", 8),
        };

        public override void Start()
        {

        }

        public int ReturnAmountBasedOnTier(PickupObject.ItemQuality itemQuality)
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

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public void ReloadPressed(PlayerController p, Gun g)
        {
            if (g.ClipCapacity > g.ClipShotsRemaining) { return; }
            if (p.CurrentItem != this) { return; }
            SwitchMode();
        }

        public override void Update()
        {
            base.Update();
            if (base.LastOwner != null && Inited == false)
            {
                Inited = !Inited;
                SetMode(Mode.COMPUTER);
                base.LastOwner.OnReloadPressed += ReloadPressed;
            }
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
                                                /*
                                                foreach (Component component in debrisObject.GetComponentsInChildren<Component>())
                                                {
                                                    if (component is Gun)
                                                    {
                                                        return true;
                                                    }
                                                }
                                                */
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
                                    /*
                                    foreach (Component component in debrisObject2.GetComponentsInChildren<Component>())
                                    {
                                        if (component is Gun a)
                                        {
                                            float f = BraveUtility.RandomAngle();
                                            int amount = ReturnAmountBasedOnTier(a.quality);
                                            DoSpawnVFX(a.sprite, amount);
                                        }
                                    }
                                    */
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

    public class ModuleInventoryController : ScriptableObject
    {
        private PlayerController player;
        private int Scale = 9;

        public void DoQuickStart(PlayerController p)
        {
            AkSoundEngine.PostEvent("Play_UI_menu_pause_01", p.gameObject);
            player = p;
            Core = ReturnCore(p);
            CurrentMode = Mode.DEF;
            if (Core == null) { return; }
            IsNone = Core.ModuleContainers.Count == 0;
            ToggleControl(true);
            GameManager.Instance.PreventPausing = true;
            p.StartCoroutine(DoDelays(p));
        }
        public IEnumerator DoFade(bool active)
        {
            GameManager.Instance.MainCameraController.SetManualControl(true, true);
            CameraController mainCameraController = GameManager.Instance.MainCameraController;
            mainCameraController.OverridePosition = active == true ? this.player.sprite.WorldCenter + new Vector2(7, -1.5f) : this.player.sprite.WorldCenter;

            float f = 0;
            while (f < 0.35f)
            {
                float q = f / 0.35f;
                f += GameManager.INVARIANT_DELTA_TIME;
                if (active == true)
                {
                    mainCameraController.SetZoomScaleImmediate(Mathf.Lerp(1, 1.8f, q));
                    Pixelator.Instance.fade = Mathf.Lerp(1f, 0.3f, q);

                }
                else
                {
                    Pixelator.Instance.fade = Mathf.Lerp(0.3f, 1f, q);
                    mainCameraController.SetZoomScaleImmediate(Mathf.Lerp(1.8f, 1, q));

                }
                yield return null;
            }
            if (active == false)
            {
                GameManager.Instance.MainCameraController.SetManualControl(false, true);
            }
            else
            {
                BraveTime.SetTimeScaleMultiplier(0, GameManager.Instance.gameObject);
            }
            yield break;
        }
        private void ToggleControl(bool active)
        {
            GameManager.Instance.StartCoroutine(DoFade(active));
            Minimap.Instance.TemporarilyPreventMinimap = active;
            if (active == true)
            {
                if (!GameManager.Instance.MainCameraController.ManualControl)
                {
                    GameManager.Instance.MainCameraController.OverridePosition = GameManager.Instance.MainCameraController.transform.position;
                    GameManager.Instance.MainCameraController.SetManualControl(true, true);
                }
                GameUIRoot.Instance.ForceClearReload(-1);
                GameUIRoot.Instance.notificationController.ForceHide();
                GameUIRoot.Instance.levelNameUI.BanishLevelNameText();
                Pixelator.Instance.FadeColor = Color.black;
                GameUIRoot.Instance.HideCoreUI("ModularInventory");

                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerController playerController = GameManager.Instance.AllPlayers[i];
                    if (playerController)
                    {
                        playerController.CurrentInputState = PlayerInputState.NoInput;
                    }
                }
            }
            if (active == false)
            {
                GameManager.Instance.MainCameraController.SetManualControl(false, true);

                GameUIRoot.Instance.ShowCoreUI("ModularInventory");

                BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);

                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                {
                    if (GameManager.Instance.AllPlayers[j])
                    {
                        GameManager.Instance.AllPlayers[j].CurrentInputState = PlayerInputState.AllInput;
                    }
                }
            }
        }

        private void Nuke()
        {
            AkSoundEngine.PostEvent("Play_UI_menu_cancel_01", player.gameObject);
            AkSoundEngine.PostEvent("Play_UI_menu_unpause_01", player.gameObject);
            GameManager.Instance.PreventPausing = false;
            GameManager.Instance.Unpause();

            Minimap.Instance.TemporarilyPreventMinimap = false;
            GameManager.Instance.MainCameraController.SetManualControl(false, true);
            GameManager.Instance.StartCoroutine(DoFade(false));
            GameUIRoot.Instance.ShowCoreUI("ModularInventory");
            
            BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].CurrentInputState = PlayerInputState.AllInput;
                }
            }
        }


        public IEnumerator DoDelays(PlayerController p)
        {
            yield return null;
            CurrentMode = Mode.DEF;
            for (int i = 0; i < Core.ModuleContainers.Count; i++)
            {
                var defMod = Core.ModuleContainers[i].defaultModule;
                AddNewPages(defMod);
                AddNewPagesTiered(defMod);
            }
            yield return null;
            DoButtonRefresh(p);
            yield return null;
            DisplayModule(p);
        }

        public void DoUpdate(PlayerController p, int PageMove)
        {
            UpdatePageLabel();
            ListEntry += PageMove;
            UpLabel.label.Invalidate();

            switch (CurrentMode)
            {
                case Mode.DEF:
                    DisplayModule(p, true);
                    return;
                case Mode.TIERED_1:
                    DisplayModuleTiered(p, ModuleTier.Tier_1, true);
                    return;
                case Mode.TIERED_2:
                    DisplayModuleTiered(p, ModuleTier.Tier_2, true);
                    return;
                case Mode.TIERED_3:
                    DisplayModuleTiered(p, ModuleTier.Tier_3, true);
                    return;
                case Mode.TIERED_4:
                    DisplayModuleTiered(p, ModuleTier.Tier_Omega, true);
                    return;
                default:
                    DisplayModule(p, true);
                    return;

            }
        }

        public void DoButtonRefresh(PlayerController p)
        {
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            if (UpLabel == null)
            {
                UpLabel = Toolbox.GenerateText(p.transform, new Vector2(1f, 0.75f)  , 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP), cl, true, Scale);
                UpLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (IsNone == false && ListEntry > 0)
                    {
                        DoUpdate(p, -1);
                    }
                    else if (IsNone == false)
                    {
                        AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                    }
                };

                UpLabel.MouseHover = (label, boolean) =>
                {
                    if (ListEntry > 0 && IsNone == false) 
                    {
                        label.text = Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.UP_BRIGHT);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP);
                        label.color = new Color32(155, 155, 155, 155);
                        label.Invalidate();
                    }
                };
                UpLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }

            if (DownLabel == null)
            {
                DownLabel = Toolbox.GenerateText(p.transform, new Vector2(1f, 0), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN), cl, true, Scale);
                DownLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (IsNone == false && ReturnPagesCount() > ListEntry)
                    {
                        DoUpdate(p, 1);

                        /*
                        UpdatePageLabel();
                        ListEntry++;
                        DownLabel.label.Invalidate();
                        if (CurrentMode == Mode.DEF)
                        {
                            DisplayModule(p, true);
                        }
                        else if (CurrentMode == Mode.TIERED_1)
                        {
                            DisplayModuleTiered(p, ModuleTier.Tier_1, true);
                        }
                        else if (CurrentMode == Mode.TIERED_2)
                        {
                            DisplayModuleTiered(p, ModuleTier.Tier_2, true);
                        }
                        else if (CurrentMode == Mode.TIERED_3)
                        {
                            DisplayModuleTiered(p, ModuleTier.Tier_3, true);
                        }
                        else if (CurrentMode == Mode.TIERED_4)
                        {
                            DisplayModuleTiered(p, ModuleTier.Tier_Omega, true);
                        }
                        */
                    }
                    else if (IsNone == false)
                    {
                        AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                    }
                };
                DownLabel.MouseHover = (label, boolean) =>
                {
                    if (ReturnPagesCount() > ListEntry && IsNone == false)
                    {
                        label.text = Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.DOWN_BRIGHT);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN);
                        label.color = new Color32(155, 155, 155, 155);
                        label.Invalidate();
                    }
                };
                DownLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };

            }
            if (CloseLabel == null)
            {
                CloseLabel = Toolbox.GenerateText(p.transform, new Vector2(1f, 1.5f), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE), cl, true, Scale);
                CloseLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
                    Nuke();
                    ObliterateUI();
                    Destroy(this);
                    
                };
                CloseLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.CLOSE_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                CloseLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };

            }

            if (T1bLabel == null)
            {
                T1bLabel = Toolbox.GenerateText(p.transform, new Vector2(1.75f, 2.25f), 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_1), cl, true, Scale);
                T1bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    ListEntry = 0;
                    CurrentMode = Mode.TIERED_1;
                    DisplayModuleTiered(p, ModuleTier.Tier_1 ,true);
                };
                T1bLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T1B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_1);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T1bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T2bLabel == null)
            {
                T2bLabel = Toolbox.GenerateText(p.transform, new Vector2(2.5f, 2.25f), 0.5f,  DefaultModule.ReturnTierLabel(ModuleTier.Tier_2), cl, true, Scale);
                T2bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.TIERED_2;
                    ListEntry = 0;
                    DisplayModuleTiered(p, ModuleTier.Tier_2, true);
                };
                T2bLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T2B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_2);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T2bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T3bLabel == null)
            {

                T3bLabel = Toolbox.GenerateText(p.transform, new Vector2(3.25f, 2.25f), 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_3), cl, true, Scale);
                T3bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.TIERED_3;
                    ListEntry = 0;
                    DisplayModuleTiered(p, ModuleTier.Tier_3, true);
                };
                T3bLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T3B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_3);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T3bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T4bLabel == null && pages_T4.Count > 0)
            {
                T4bLabel = Toolbox.GenerateText(p.transform, new Vector2(4f, 2.25f), 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_Omega), cl, true, Scale);
                T4bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.TIERED_4;
                    ListEntry = 0;
                    DisplayModuleTiered(p, ModuleTier.Tier_Omega, true);
                };
                T4bLabel.MouseHover = (label, boolean) =>
                {
                    ListEntry = 0;
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T4B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_Omega);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T4bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (AnyLabel == null)
            {
                AnyLabel = Toolbox.GenerateText(p.transform, new Vector2(1, 2.25f), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.GOOGLY), cl, true, Scale);

                AnyLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.DEF;
                    ListEntry = 0;
                    DisplayModule(p, true);
                };
                AnyLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.GOOGLY_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.GOOGLY);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                AnyLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (PowerLabel == null)

            {
                PowerLabel = Toolbox.GenerateText(p.transform, new Vector2(1, 3f), 0.5f, "[ " + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + " : " + Core.ReturnPowerConsumption() + " / " + Core.ReturnTotalPower().ToString() + " ]", cl, true, Scale-1);
                PowerLabel.OnUpdate += (obj1) =>
                {
                    bool h = PowerLabel.IsMouseHovering();
                    string t = "[ " + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + " : " + Core.ReturnPowerConsumption() + " / " + (h == true ? StaticColorHexes.AddColorToLabelString((Core.ReturnTotalPower() + 1).ToString(), StaticColorHexes.Green_Hex) : Core.ReturnTotalPower().ToString()) + " ]";
                    if (h == true)
                    {
                        t += " [Upgrade:"+ StaticColorHexes.AddColorToLabelString(ReturnUpgradeCost().ToString(), CanAffordUpgrade() == true ? StaticColorHexes.Green_Hex : StaticColorHexes.Red_Color_Hex) + scrapLabel + "]";
                    }
                    obj1.color = h == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    obj1.text = t;
                    obj1.Invalidate();
                };
                PowerLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (CanAffordUpgrade() == true)
                    {
                        player.GetComponent<ConsumableStorage>().RemoveConsumableAmount("Scrap", ReturnUpgradeCost());
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(PowerCell.PowerCellID).gameObject, Vector3.zero, Quaternion.identity);
                        PickupObject component3 = gameObject.GetComponent<PickupObject>();
                        if (component3 != null)
                        {
                            component3.CanBeDropped = false;
                            component3.Pickup(player);
                        }
                    }
                };
                PowerLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
        }

        public void InstantUpdateLabelToText(dfLabel label, string newText)
        {
            label.text = newText;
            label.Invalidate();
        }

        public bool CanAffordUpgrade()
        {
            if (player.gameObject.GetComponent<ConsumableStorage>() == null) { return false; }
            if (player.gameObject.GetComponent<ConsumableStorage>().ReturnConsumableAmount("Scrap") >= ReturnUpgradeCost()) 
            {
                return true;
            }
            return false;
        }

        public int ReturnUpgradeCost()
        {
            return (Mathf.Max(2, Mathf.RoundToInt(Core.ReturnTotalPowerMasteryless()) + 3));
        }


        public int ReturnPagesCount()
        {
            switch (CurrentMode)
            {
                case Mode.DEF:
                    return pages_default.Count > 0 ? pages_default.Last().Page : 0;
                case Mode.TIERED_1:
                    return pages_T1.Count > 0 ? pages_T1.Last().Page : 0;
                case Mode.TIERED_2:
                    return pages_T2.Count > 0 ? pages_T2.Last().Page : 0;
                case Mode.TIERED_3:
                    return pages_T3.Count > 0 ? pages_T3.Last().Page : 0;
                case Mode.TIERED_4:
                    return pages_T4.Count > 0 ? pages_T4.Last().Page : 0;
                default:
                    return pages_default.Count > 0 ? pages_default.Last().Page : 0;
            }
        }
        private string scrapLabel = "[sprite \"" + "gear_" + "\"]";


        public ModulePrinterCore ReturnCore(PlayerController p)
        {
            for (int o = 0; o < p.passiveItems.Count; o++) 
            {
                if (p.passiveItems[o] is ModulePrinterCore core)
                { return core; }
            }
            return null;
        }

        private float MainOffset = 1.75f;
        public void DisplayModule(PlayerController p, bool ClearOut = false)
        {
            if (ClearOut == true)
            {
                for (int i = 0; i < garbageLabels.Count; i++)
                {
                    if (garbageLabels[i] != null) { Destroy(garbageLabels[i].gameObject); }
                }
                garbageLabels.Clear();
                if (extantLabel) { Destroy(extantLabel.gameObject); }
                if (PageLabel) { Destroy(PageLabel.gameObject); }
            }


            var ModuleContainers = Core.ModuleContainers;
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            string Text = "Modules Available:";
            garbageLabels.Add(Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 1.5f), 0.66f, Text, cl, true, Scale));

            int c = 0;
            if (ModuleContainers.Count == 0) 
            {
                var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "None.", cl, true, Scale);
                c++;
                garbageLabels.Add(Button);
            } 
            else
            {
                foreach (var page in pages_default)
                {
                    if (ListEntry == page.Page)
                    {
                        string T = page.module.LabelName + " (" + StaticColorHexes.AddColorToLabelString(page.module.ActiveStack().ToString() + " / " + page.module.TrueStack(), StaticColorHexes.Orange_Hex) + ") (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + Core.ReturnPowerConsumption(page.module) + ")";
                        var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 2, 0.75f - (0.75f * c)), 0.66f, T, cl, true, Scale);
                        Button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            if (extantLabel != null) { Destroy(extantLabel.gameObject); }
                            extantLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0f - (0.75f * c)), 0.66f, page.module.LabelDescription, cl, true, Scale / 2);
                        };
                        Button.MouseHover = (label, boolean) =>
                        {
                            label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.Invalidate();
                        };
                        Button.OnUpdate += (l) =>
                        {              
                            l.text = (Button.IsMouseHovering() == true ? StaticColorHexes.AddColorToLabelString(page.module.LabelName, StaticColorHexes.Yellow_Hex) : page.module.LabelName) + " (" + StaticColorHexes.AddColorToLabelString(page.module.ActiveStack().ToString() + " / " + page.module.TrueStack(), StaticColorHexes.Orange_Hex) + ") (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (Core.ReturnPowerConsumption(page.module)) + ")";
                            l.Invalidate();
                        };
                        Button.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };


                        var ButtonLeft = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "-", cl, true, Scale);
                        ButtonLeft.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(page.module.LabelName) > 0;
                            label.color = CanBeUsed == true ? boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) +( CanBeUsed == true && boolean == true ? StaticColorHexes.AddColorToLabelString("-", StaticColorHexes.Yellow_Hex) : "-");
                            label.Invalidate();
                        };
                        ButtonLeft.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        ButtonLeft.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(page.module.LabelName) > 0;
                            if (CanBeUsed == true && Core.ReturnPowerConsumption() > 0 && page.module.CanBeDisabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Fade_01", player.gameObject);
                                Core.DepowerModule(page.module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };

                        garbageLabels.Add(ButtonLeft);
                        var ButtonRight = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 1, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "+", cl, true, Scale);
                        ButtonRight.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(page.module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(page.module.LabelName) > Core.ReturnActiveStack(page.module.LabelName);
                            label.color = CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (CanBeUsed == true && boolean == true && CanBeUsed3 == true && CanBeUsed2 == true ? StaticColorHexes.AddColorToLabelString("+", StaticColorHexes.Yellow_Hex) : "+");

                            label.Invalidate();
                        };
                        ButtonRight.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(page.module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(page.module.LabelName) > Core.ReturnActiveStack(page.module.LabelName);

                            if (CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && page.module.CanBeEnabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ModulePowerUp", player.gameObject);
                                Core.PowerModule(page.module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };
                        ButtonRight.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };

                        garbageLabels.Add(ButtonRight);
                        garbageLabels.Add(Button);
                        c++;
                    }
                }
            }
            PageLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "Page:" + (ListEntry + 1).ToString() + " / " + (pages_default.Count > 0 ?  (pages_default.Last().Page + 1).ToString() : "1"), cl, true, Scale);
        }

        public void DisplayModuleTiered(PlayerController p, ModuleTier moduleTier ,bool ClearOut = false)
        {
            if (ClearOut == true)
            {
                for (int i = 0; i < garbageLabels.Count; i++)
                {
                    if (garbageLabels[i] != null) { Destroy(garbageLabels[i].gameObject); }
                }
                garbageLabels.Clear();
                if (extantLabel) { Destroy(extantLabel.gameObject); }
                if (PageLabel) { Destroy(PageLabel.gameObject); }
            }


            var ModuleContainers = ReturnPageListTier(moduleTier);

            Color32 cl = (moduleTier == ModuleTier.Tier_Omega) ? new Color32(200, 10, 10, 100) : p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);

            string Text = "Modules Available " + DefaultModule.ReturnTierLabel(moduleTier) + " :";
            garbageLabels.Add(Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 1.5f), 0.66f, Text, cl, true, Scale));


            int c = 0;
            if (ModuleContainers.Count == 0)
            {
                var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "None.", cl, true, Scale);
                c++;
                garbageLabels.Add(Button);
            }
            else
            {
                foreach (var page in ModuleContainers)
                {
                    if (ListEntry == page.Page)
                    {
                        string T = page.module.LabelName + " (" + StaticColorHexes.AddColorToLabelString(page.module.ActiveStack().ToString() + " / " + page.module.TrueStack(), StaticColorHexes.Orange_Hex) + ") (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + Core.ReturnPowerConsumption(page.module) + ")";
                        var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 2, 0.75f - (0.75f * c)), 0.66f, T, cl, true, Scale);
                        Button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            if (extantLabel != null) { Destroy(extantLabel.gameObject); }
                            extantLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0f - (0.75f * c)), 0.66f, page.module.LabelDescription, cl, true, Scale / 2);
                        };
                        Button.MouseHover = (label, boolean) =>
                        {
                            label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.Invalidate();
                        };
                        Button.OnUpdate += (l) =>
                        {
                            l.text = (Button.IsMouseHovering() == true ? StaticColorHexes.AddColorToLabelString(page.module.LabelName, StaticColorHexes.Yellow_Hex) : page.module.LabelName) + " (" + StaticColorHexes.AddColorToLabelString(page.module.ActiveStack().ToString() + " / " + page.module.TrueStack(), StaticColorHexes.Orange_Hex) + ") (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (Core.ReturnPowerConsumption(page.module)) + ")";
                            l.Invalidate();
                        };
                        Button.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        var ButtonLeft = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "-", cl, true, Scale);
                        ButtonLeft.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(page.module.LabelName) > 0;
                            label.color = CanBeUsed == true ? boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (CanBeUsed == true && boolean == true ? StaticColorHexes.AddColorToLabelString("-", StaticColorHexes.Yellow_Hex) : "-");
                            label.Invalidate();
                        };
                        ButtonLeft.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(page.module.LabelName) > 0;
                            if (CanBeUsed == true && Core.ReturnPowerConsumption() > 0 && page.module.CanBeDisabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Fade_01", player.gameObject);
                                Core.DepowerModule(page.module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };
                        ButtonLeft.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        garbageLabels.Add(ButtonLeft);
                        var ButtonRight = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 1, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "+", cl, true, Scale);
                        ButtonRight.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(page.module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(page.module.LabelName) > Core.ReturnActiveStack(page.module.LabelName);
                            label.color = CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (CanBeUsed == true && boolean == true && CanBeUsed3 == true && CanBeUsed2 == true ? StaticColorHexes.AddColorToLabelString("+", StaticColorHexes.Yellow_Hex) : "+");
                            label.Invalidate();
                        };
                        ButtonRight.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(page.module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(page.module.LabelName) > Core.ReturnActiveStack(page.module.LabelName);

                            if (CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && page.module.CanBeEnabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ModulePowerUp", player.gameObject);
                                Core.PowerModule(page.module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };
                        ButtonRight.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        garbageLabels.Add(ButtonRight);
                        garbageLabels.Add(Button);
                        c++;
                    }
                }
            }
            PageLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "Page:" + (ListEntry + 1).ToString() + " / " + (ModuleContainers.Count > 0 ? (ModuleContainers.Last().Page + 1).ToString() : "1"), cl, true, Scale);
        }


        public void UpdatePageLabel()
        {
            if (PageLabel)
            {
                PageLabel.label.text = "Page:" + (ListEntry + 1).ToString() + " / " + (pages_default.Count > 0 ? (pages_default.Last().Page + 1).ToString() : "1");
                PageLabel.label.Invalidate();
            }
        }

        public bool IsNone;


        public int ListEntry = 0;


        public void ObliterateUI()
        {
            if (extantLabel) { extantLabel.Inv(); }
            if (DownLabel) { DownLabel.Inv(); }
            if (UpLabel) { UpLabel.Inv(); }
            if (PageLabel) { PageLabel.Inv(); }
            if (CloseLabel) { CloseLabel.Inv(); }

            if (T1bLabel) { T1bLabel.Inv(); }
            if (T2bLabel) { T2bLabel.Inv(); }
            if (T3bLabel) { T3bLabel.Inv(); }
            if (T4bLabel) { T4bLabel.Inv(); }
            if (AnyLabel) { AnyLabel.Inv(); }

            if (PowerLabel) { PowerLabel.Inv(); }
            for (int i = 0; i < garbageLabels.Count; i++)
            {
                if (garbageLabels[i] != null) { garbageLabels[i].Inv(); }
            }
            garbageLabels.Clear();

        }


        public int AddNewPages(DefaultModule module)
        {
            int currentPage = pages_default.Count > 0 ? pages_default.Last().Page : 0;
            int LastEntry = pages_default.Count > 0 ? pages_default.Last().Entry : -1;
            if (LastEntry > 3)
            {
                currentPage += 1;
                LastEntry = -1;
            }
            QuickAndMessyPage quickAndMessy = new QuickAndMessyPage();
            quickAndMessy.Page = currentPage;
            quickAndMessy.Entry = LastEntry + 1;
            quickAndMessy.module = module;
            pages_default.Add(quickAndMessy);
            return LastEntry + 1;
        }

        public int AddNewPagesTiered(DefaultModule module)
        {
            var specList = ReturnPageListTier(module.Tier);
            int currentPage = specList.Count > 0 ? specList.Last().Page : 0;
            int LastEntry = specList.Count > 0 ? specList.Last().Entry : -1;
            if (LastEntry > 3)
            {
                currentPage += 1;
                LastEntry = -1;
            }
            QuickAndMessyPage quickAndMessy = new QuickAndMessyPage();
            quickAndMessy.Page = currentPage;
            quickAndMessy.Entry = LastEntry + 1;
            quickAndMessy.module = module;
            specList.Add(quickAndMessy);
            return LastEntry + 1;
        }


        public List<QuickAndMessyPage> ReturnPageListTier(ModuleTier moduleTier)
        {
            switch (moduleTier)
            {
                case ModuleTier.Tier_1:
                    return pages_T1;
                case ModuleTier.Tier_2:
                    return pages_T2;
                case ModuleTier.Tier_3:
                    return pages_T3;
                case ModuleTier.Tier_Omega:
                    return pages_T4;
                default:
                    return pages_T1;
            }
        }

        public Mode CurrentMode;

        public enum Mode
        {
            DEF,
            TIERED_1,
            TIERED_2,
            TIERED_3,
            TIERED_4,
        }



        public List<QuickAndMessyPage> pages_default = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T1 = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T2 = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T3 = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T4 = new List<QuickAndMessyPage>();

        public class QuickAndMessyPage
        {
            public DefaultModule module;
            public int Page;
            public int Entry;
        }


        private ModulePrinterCore Core;


        public ModifiedDefaultLabelManager UpLabel;
        public ModifiedDefaultLabelManager DownLabel;
        public ModifiedDefaultLabelManager CloseLabel;

        public ModifiedDefaultLabelManager PageLabel;
        public List<ModifiedDefaultLabelManager> garbageLabels = new List<ModifiedDefaultLabelManager>();
        public List<ModifiedDefaultLabelManager> secondaryGarbageLabels = new List<ModifiedDefaultLabelManager>();

        public ModifiedDefaultLabelManager extantLabel;

        public ModifiedDefaultLabelManager T1bLabel;
        public ModifiedDefaultLabelManager T2bLabel;
        public ModifiedDefaultLabelManager T3bLabel;
        public ModifiedDefaultLabelManager T4bLabel;
        public ModifiedDefaultLabelManager AnyLabel;
        public ModifiedDefaultLabelManager PowerLabel;

    }

    public class ModuleCrafingController : ScriptableObject
    {
        private int Scale = 9;
        private Vector2 AdditionalOffset = new Vector2(1f, 1.75f);

        public void DoQuickStart(PlayerController p)
        {
            AkSoundEngine.PostEvent("Play_UI_menu_pause_01", p.gameObject);
            player = p;
            Core = ReturnCore(p);
            if (Core == null) { return; }
            GameManager.Instance.PreventPausing = true;

            p.StartCoroutine(DoDelays(p));
            ToggleControl(true);
        }

        public IEnumerator DoFade(bool active)
        {
            GameManager.Instance.MainCameraController.SetManualControl(true, true);
            CameraController mainCameraController = GameManager.Instance.MainCameraController;
            mainCameraController.OverridePosition = active == true ? this.player.sprite.WorldCenter + new Vector2(7, -1.5f) : this.player.sprite.WorldCenter;

            float f = 0;
            while (f < 0.35f)
            {
                float q = f / 0.35f;
                f += GameManager.INVARIANT_DELTA_TIME;
                if (active == true)
                {
                    mainCameraController.SetZoomScaleImmediate(Mathf.Lerp(1, 1.8f, q));
                    Pixelator.Instance.fade = Mathf.Lerp(1f, 0.3f, q);

                }
                else
                {
                    Pixelator.Instance.fade = Mathf.Lerp(0.3f, 1f, q);
                    mainCameraController.SetZoomScaleImmediate(Mathf.Lerp(1.8f, 1, q));

                }
                yield return null;
            }
            if (active == false)
            {
                GameManager.Instance.MainCameraController.SetManualControl(false, true);
            }
            else
            {
                BraveTime.SetTimeScaleMultiplier(0, GameManager.Instance.gameObject);
            }
            yield break;
        }

        private void ToggleControl(bool active)
        {
            GameManager.Instance.StartCoroutine(DoFade(active));
            Minimap.Instance.TemporarilyPreventMinimap = active;
            if (active == true)
            {
                if (!GameManager.Instance.MainCameraController.ManualControl)
                {
                    GameManager.Instance.MainCameraController.OverridePosition = GameManager.Instance.MainCameraController.transform.position;
                    GameManager.Instance.MainCameraController.SetManualControl(true, true);
                }
                GameUIRoot.Instance.ForceClearReload(-1);
                GameUIRoot.Instance.notificationController.ForceHide();
                GameUIRoot.Instance.levelNameUI.BanishLevelNameText();
                Pixelator.Instance.FadeColor = Color.black;
                GameUIRoot.Instance.HideCoreUI("ModularCrafter");

                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerController playerController = GameManager.Instance.AllPlayers[i];
                    if (playerController)
                    {
                        playerController.CurrentInputState = PlayerInputState.NoInput;
                    }
                }
            }
            if (active == false)
            {
                GameManager.Instance.MainCameraController.SetManualControl(false, true);

                GameUIRoot.Instance.ShowCoreUI("ModularCrafter");

                BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);

                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                {
                    if (GameManager.Instance.AllPlayers[j])
                    {
                        GameManager.Instance.AllPlayers[j].CurrentInputState = PlayerInputState.AllInput;
                    }
                }
            }
        }

        public void Nuke()
        {
            GameManager.Instance.PreventPausing = false;
            GameManager.Instance.Unpause();

            Minimap.Instance.TemporarilyPreventMinimap = false;
            AkSoundEngine.PostEvent("Play_UI_menu_cancel_01", player.gameObject);
            AkSoundEngine.PostEvent("Play_UI_menu_unpause_01", player.gameObject);
            GameManager.Instance.MainCameraController.SetManualControl(false, true);
            GameManager.Instance.StartCoroutine(DoFade(false));
            GameUIRoot.Instance.ShowCoreUI("ModularCrafter");

            BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].CurrentInputState = PlayerInputState.AllInput;
                }
            }
            GameManager.Instance.StartCoroutine(CraftModules(player, Queue));
        }


        public IEnumerator CraftModules(PlayerController p, List<DefaultModule> modules)
        {
            AkSoundEngine.PostEvent("Play_OBJ_computer_boop_01", p.gameObject);
            ETGModConsole.Log(modules.Count);

            foreach (var entry in modules)
            {
                AkSoundEngine.PostEvent("Play_OBJ_bomb_fuse_01", p.gameObject);
                var debris = LootEngine.SpawnItem(entry.gameObject, p.sprite.WorldCenter, Vector2.zero, 0, true);
                debris.sprite.renderer.material.shader = StaticShaders.Displacer_Beast_Shader;
                debris.sprite.renderer.material.SetTexture("_MainTex", debris.sprite.renderer.material.mainTexture);
                SpriteOutlineManager.RemoveOutlineFromSprite(debris.sprite, false);
                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    if (debris == null) { yield break; }
                    float t = Toolbox.SinLerpTValueFull(elapsed);
                    entry.BraveLight.LightIntensity = Mathf.Lerp(0, 20, t);
                    entry.BraveLight.LightRadius = Mathf.Lerp(0, 3, t);
                    elapsed += BraveTime.DeltaTime;
                    debris.sprite.renderer.material.SetFloat("_BurnAmount", 1 - elapsed);
                    yield return null;
                }
                LootEngine.DoDefaultItemPoof(debris.sprite.WorldCenter);
                SpriteOutlineManager.AddOutlineToSprite(debris.sprite, Color.black);
            }
            yield return null;
        }

        public IEnumerator DoDelays(PlayerController p)
        {
            yield return null;
            yield return null;
            DoButtonRefreshSelect(p);
            //DoButtonRefresh(p);
            yield return null;
            //DisplayModule(p);
        }

        public void DoButtonRefreshSelect(PlayerController p)
        {
            Tier = SelectedTier.NONE;
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            if (T1_Select == null)
            {
                T1_Select = Toolbox.GenerateText(p.transform, new Vector2(2.5f, 0.25f) + AdditionalOffset, 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_1), cl, true, Scale + 6);
                T1_Select.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    Tier = SelectedTier.T1;
                    ObliterateSelectUI();
                    DoButtonRefreshCraft(p);
                };
                T1_Select.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T1B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_1);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T1_Select.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T2_Select == null)
            {
                T2_Select = Toolbox.GenerateText(p.transform, new Vector2(3.5f, 0.25f) + AdditionalOffset, 0.75f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_2), cl, true, Scale + 6);
                T2_Select.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    Tier = SelectedTier.T2;
                    ObliterateSelectUI();
                    DoButtonRefreshCraft(p);
                };
                T2_Select.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T2B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_2);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T2_Select.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T3_Select == null)
            {
                T3_Select = Toolbox.GenerateText(p.transform, new Vector2(4.5f, 0.25f) + AdditionalOffset, 0.75f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_3), cl, true, Scale + 6);
                T3_Select.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    Tier = SelectedTier.T3;
                    ObliterateSelectUI();
                    DoButtonRefreshCraft(p);
                };
                T3_Select.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T3B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_3);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T3_Select.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (CloseLabel == null)
            {
                CloseLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, 0.25f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE), cl, true, Scale + 6);
                CloseLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
                    ObliterateUI();
                    Destroy(this);
                    ToggleControl(false);
                    Nuke();
                };
                CloseLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.CLOSE_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                CloseLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (basicgarbageLabel == null)
            {
                basicgarbageLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, 1.125f) + AdditionalOffset, 0.5f, "Select Crafting Tier:", cl, true, 6);

            }
        }


        public Action OnCrafted;

        public int GetScrapCount(PlayerController p)
        {
            var con = p.GetComponent<ConsumableStorage>();
            if (con == null) { return 0; }
            return con.ReturnConsumableAmount("Scrap");
        }

        private string scrapLabel = "[sprite \"" + "gear_" + "\"]";

        public void DoButtonRefreshCraft(PlayerController p)
        {
            ListEntry = 0;
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages = ReturnLib();
            if (pageUpLabel) { Destroy(pageUpLabel.gameObject); }
            if (pageUpLabel == null)
            {
                pageUpLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, -1f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP), cl, true, Scale + 6);
                pageUpLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {

                    if (ListEntry > 0)
                    {
                        UpdatePageLabel(quickAndMessyPages);
                        ListEntry--;
                        pageUpLabel.label.Invalidate();
                        UpdateOptions();
                    }

                };
                pageUpLabel.MouseHover = (label, boolean) =>
                {
                    if (ListEntry > 0)
                    {
                        label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.UP_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.color = new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                };
                pageUpLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (pageReturnLabel) { Destroy(pageReturnLabel.gameObject); }
            if (pageReturnLabel == null)
            {
                pageReturnLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, -3.5f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.LEFT), cl, true, Scale + 6);
                pageReturnLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    ObliterateCraftUI();
                    DoButtonRefreshSelect(p);
                };
                pageReturnLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.LEFT_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.LEFT);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                pageReturnLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }

            if (pageDownLabel) { Destroy(pageDownLabel.gameObject); }
            if (pageDownLabel == null)
            {
                pageDownLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, -2.25f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN), cl, true, Scale + 6);
                pageDownLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (ReturnPagesCount(quickAndMessyPages) > ListEntry)
                    {
                        UpdatePageLabel(quickAndMessyPages);
                        ListEntry++;
                        pageDownLabel.label.Invalidate();
                        UpdateOptions();
                    }
                };
                pageDownLabel.MouseHover = (label, boolean) =>
                {
                    if (ReturnPagesCount(quickAndMessyPages) > ListEntry)
                    {
                        label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.DOWN_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.color = new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                };
                pageDownLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            UpdateOptions();
        }
        private PlayerController player;



        public void UpdateOptions()
        {
            if (extantLabel) { extantLabel.Inv(); }
            if (PageLabel) { Destroy(PageLabel.gameObject); }
            if (craftLabel) {craftLabel.Inv(); }

            Color32 cl = player.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);

            foreach (var entry in craftingLabels)
            {
                if (entry.Key != null) { Destroy(entry.Key.gameObject); }
            }

            List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages = ReturnLib();
            int c = 0;
            foreach (var page in quickAndMessyPages)
            {
                if (ListEntry == page.Page)
                {
                    string T = page.module.LabelName + " (" + scrapLabel + " " + ModuleCost(page.module).ToString() + ")";
                    bool encountered = GameStatsManager.m_instance.m_encounteredTrackables.ContainsKey(page.module.encounterTrackable.EncounterGuid);
                    if (encountered == false) { T = StaticColorHexes.AddColorToLabelString("[UNDISCOVERED]", StaticColorHexes.Light_Orange_Hex); }

                    var Button = Toolbox.GenerateText(player.transform, new Vector2(2.5f, 0.25f - (0.75f * c)) + AdditionalOffset, 0.66f, T, cl, true, Scale);
                    Button.StoredModuleInfo = page.module;

                    //Debug.Log(page.module.name + " : " + encountered);
                    if (encountered == true)
                    {
                        Button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            if (extantLabel != null) { Destroy(extantLabel.gameObject); }
                            extantLabel = Toolbox.GenerateText(player.transform, new Vector2(2.5f, -1.75f - (0.5f * c)) + AdditionalOffset, 0.66f, page.module.LabelDescription, cl, true, 4);
                            if (craftLabel) { Destroy(craftLabel.gameObject); }
                            craftLabel = Toolbox.GenerateText(player.transform, new Vector2(2.5f, 1f) + AdditionalOffset, 0.66f, StaticColorHexes.AddColorToLabelString("CRAFT", StaticColorHexes.Light_Green_Hex) + "( " + scrapLabel + " " + StaticColorHexes.AddColorToLabelString("-" + ModuleCost(page.module).ToString(), StaticColorHexes.Red_Color_Hex) + " )", cl, true, Scale);
                            craftLabel.MouseHover = (label, boolean) =>
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    label.text = StaticColorHexes.AddColorToLabelString("CRAFT", boolean == true ? StaticColorHexes.Light_Orange_Hex : StaticColorHexes.White_Hex) + "( " + scrapLabel + " " + StaticColorHexes.AddColorToLabelString("-" + ModuleCost(page.module).ToString(), StaticColorHexes.Red_Color_Hex) + " )";
                                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                                    label.Invalidate();
                                }
                            };
                            craftLabel.OnUpdate = (label) =>
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    label.text = StaticColorHexes.AddColorToLabelString("CRAFT", StaticColorHexes.White_Hex) + "( " + scrapLabel + " " + StaticColorHexes.AddColorToLabelString("-" + ModuleCost(page.module).ToString(), StaticColorHexes.Red_Color_Hex) + " )";
                                    label.Invalidate();
                                }
                                else
                                {
                                    label.text = StaticColorHexes.AddColorToLabelString("INSUFFICIENT SCRAP", StaticColorHexes.Red_Color_Hex);
                                    label.Invalidate();
                                }
                            };
                            craftLabel.label.Click += delegate (dfControl control1, dfMouseEventArgs mouseEvent2)
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    AkSoundEngine.PostEvent("Play_OBJ_metronome_jingle_01", player.gameObject);
                                    Toolbox.NotifyCustom("Crafted Module:", page.module.LabelName, page.module.sprite.spriteId, page.module.sprite.collection, UINotificationController.NotificationColor.PURPLE);
                                    player.GetComponent<ConsumableStorage>().RemoveConsumableAmount("Scrap", ModuleCost(page.module));
                                    Queue.Add(page.module);
                                    if (OnCrafted != null) { OnCrafted(); }
                                }
                            };
                            craftLabel.label.MouseEnter += (o1, o2) =>
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                                }
                            };
                            //craftLabel
                        };
                        Button.MouseHover = (label, boolean) =>
                        {
                            label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.Invalidate();
                        };
                        Button.OnUpdate = (label) =>
                        {
                            if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                            {//Button
                                label.text = StaticColorHexes.AddColorToLabelString(Button.StoredModuleInfo.LabelName, Button.IsMouseHovering() == true ? StaticColorHexes.Light_Orange_Hex : StaticColorHexes.White_Hex) + " (" + scrapLabel + " " + StaticColorHexes.AddColorToLabelString(ModuleCost(Button.StoredModuleInfo).ToString(), StaticColorHexes.Green_Hex) + ")";
                                label.Invalidate();
                            }
                            else
                            {
                                label.text = StaticColorHexes.AddColorToLabelString(Button.StoredModuleInfo.LabelName, Button.IsMouseHovering() == true ? StaticColorHexes.Light_Orange_Hex : StaticColorHexes.White_Hex) + " (" + scrapLabel + " " + StaticColorHexes.AddColorToLabelString(ModuleCost(Button.StoredModuleInfo).ToString(), StaticColorHexes.Red_Color_Hex) + ")";
                                label.Invalidate();
                            }
                        };
                        Button.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                    }
                    craftingLabels.Add(Button, page.module);
                    c++;
                }
            }
            PageLabel = Toolbox.GenerateText(player.transform, new Vector2(2.5f, 0.25f - (0.75f * c)) + AdditionalOffset, 0.66f, "Page:" + (ListEntry + 1).ToString() + " / " + (quickAndMessyPages.Count > 0 ? (quickAndMessyPages.Last().Page + 1).ToString() : "1"), cl, true, Scale);
            UpdatePageLabel(quickAndMessyPages);
        }


        public int ModuleCost(DefaultModule module)
        {
            if (module.OverrideScrapCost != null) { return module.OverrideScrapCost.Value; }
            switch (module.Tier)
            {
                case ModuleTier.Tier_1:
                    return 5;
                case ModuleTier.Tier_2:
                    return 10;
                case ModuleTier.Tier_3:
                    return 15;
                default:
                    return 10;
            }
        }

        private List<DefaultModule> Queue = new List<DefaultModule>();


        public void UpdatePageLabel(List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages)
        {
            if (PageLabel)
            {
                PageLabel.label.text = "Page:" + (ListEntry + 1).ToString() + " / " + (quickAndMessyPages.Count > 0 ? (quickAndMessyPages.Last().Page + 1).ToString() : "1");
                PageLabel.label.Invalidate();
            }
        }
        public int ReturnPagesCount(List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages)
        {
            return quickAndMessyPages.Count > 0 ? quickAndMessyPages.Last().Page : 0;
        }

        private List<GlobalModuleStorage.QuickAndMessyPage> ReturnLib()
        {
            switch (Tier)
            {
                case SelectedTier.T1:
                    return GlobalModuleStorage.pages_T1;
                case SelectedTier.T2:
                    return GlobalModuleStorage.pages_T2;
                case SelectedTier.T3:
                    return GlobalModuleStorage.pages_T3;

                default:
                    return GlobalModuleStorage.pages_T1;
            }
        }

        public ModulePrinterCore ReturnCore(PlayerController p)
        {
            for (int o = 0; o < p.passiveItems.Count; o++)
            {
                if (p.passiveItems[o] is ModulePrinterCore core)
                { return core; }
            }
            return null;
        }

        private void ObliterateCraftUI()
        {
            if (pageReturnLabel) { pageReturnLabel.Inv(); }
            if (PageLabel) { PageLabel.Inv(); }
            if (extantLabel) { extantLabel.Inv(); }
            if (pageUpLabel) { pageUpLabel.Inv(); }
            if (pageDownLabel) { pageDownLabel.Inv(); }
            if (PageLabel) { PageLabel.Inv(); }
            if (craftLabel) { craftLabel.Inv(); }


            foreach (var entry in craftingLabels)
            {
                if (entry.Key != null) { entry.Key.Inv(); }
            }
            craftingLabels.Clear();
        }
        private void ObliterateSelectUI()
        {
            if (T1_Select) { Destroy(T1_Select.gameObject); }
            if (T2_Select) { Destroy(T2_Select.gameObject); }
            if (T3_Select) { Destroy(T3_Select.gameObject); }
            if (basicgarbageLabel) { Destroy(basicgarbageLabel.gameObject); }

        }

        public void ObliterateUI()
        {
            if (T1_Select) { T1_Select.Inv(); }
            if (T2_Select) { T2_Select.Inv(); }
            if (T3_Select) { T3_Select.Inv(); }
            if (CloseLabel) { CloseLabel.Inv(); }
            if (extantLabel) { extantLabel.Inv(); }
            if (pageDownLabel) { pageDownLabel.Inv(); }
            if (pageUpLabel) { pageUpLabel.Inv(); }
            if (PageLabel) { PageLabel.Inv(); }
            if (pageReturnLabel) { pageReturnLabel.Inv(); }
            if (craftLabel) { craftLabel.Inv(); }
            if (basicgarbageLabel) { Destroy(basicgarbageLabel.gameObject); }


            foreach (var entry in craftingLabels)
            {
                if (entry.Key != null) { entry.Key.Inv(); }
            }
            craftingLabels.Clear();
        }

        private SelectedTier Tier = SelectedTier.NONE;
        public ModulePrinterCore Core;
        public int ListEntry = 0;

        //Labels
        public Dictionary<ModifiedDefaultLabelManager, DefaultModule> craftingLabels = new Dictionary<ModifiedDefaultLabelManager, DefaultModule>();


        public ModifiedDefaultLabelManager T1_Select;
        public ModifiedDefaultLabelManager T2_Select;
        public ModifiedDefaultLabelManager T3_Select;

        public ModifiedDefaultLabelManager CloseLabel;
        public ModifiedDefaultLabelManager PageLabel;
        public ModifiedDefaultLabelManager extantLabel;
        public ModifiedDefaultLabelManager craftLabel;

        public ModifiedDefaultLabelManager pageUpLabel;
        public ModifiedDefaultLabelManager pageDownLabel;
        public ModifiedDefaultLabelManager pageReturnLabel;

        public ModifiedDefaultLabelManager basicgarbageLabel;

        private enum SelectedTier
        {
            NONE, 
            T1,
            T2,
            T3,

        }
    }
}

