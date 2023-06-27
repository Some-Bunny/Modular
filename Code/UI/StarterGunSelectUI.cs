using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Alexandria.PrefabAPI;
using HarmonyLib;
using SaveAPI;
using UnityEngine;
using System.Security.Cryptography;
using static UnityEngine.ParticleSystem;
using UnityEngine.Networking;
using static ModularMod.StarterGunSelectUIController.UpgradeUISelectButtonController;
using System.Collections;

namespace ModularMod
{



    internal class StarterGunSelectUIController : MonoBehaviour
    {
        public static StarterGunSelectUIController Inst;
        public static dfAtlas def;
        //public static dfGUIManager mana;


        public class UpgradeUISelectButtonController : MonoBehaviour
        {
            public void Start()
            {
                df_Button = this.gameObject.GetComponent<dfButton>();
                UpdateSprites();
                df_Button.MouseEnter += (o1, o2) =>
                {
                    if (df_Button.isActiveAndEnabled)
                    {
                        AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
                    }
                };
                df_Button.Click += (o1, o2) =>
                {
                    if (df_Button.isActiveAndEnabled)
                    {
                        AkSoundEngine.PostEvent("Play_FS_slipper_stone_01", this.gameObject);
                    }
                };

            }

            public void UpdateSprites()
            {
                bool b = IsUnlocked();

                df_Button.backgroundSprite = b == false ? ReturnAltSelection(Lock_Label, Lock_Label_Alt) : ReturnAltSelection(Label_Name, Label_Name_Alt);
                df_Button.disabledSprite = b == false ? ReturnAltSelection(Lock_Label, Lock_Label_Alt) : ReturnAltSelection(Label_Name, Label_Name_Alt);
                df_Button.hoverSprite = b == false ? ReturnAltSelection(Lock_Label_Hover, Lock_Label_Hover_Alt) : ReturnAltSelection(Label_Hover, Label_Hover_Alt);
                df_Button.focusSprite = b == false ? ReturnAltSelection(Lock_Label_Hover, Lock_Label_Hover_Alt) : ReturnAltSelection(Label_Hover, Label_Hover_Alt);
                df_Button.pressedSprite = b == false ? ReturnAltSelection(Lock_Label_Press, Lock_Label_Press_Alt) : ReturnAltSelection(Label_Press, Label_Press_Alt);
                df_Button.Invalidate();
                df_Button.renderOrder = 3000;
            }

            public string ReturnAltSelection(string def, string alt)
            {
                if (Player_Using_Alt_Skin == true && alt != null) { return alt; }else { return def; }
            }


            public bool IsUnlocked()
            {
                if (OverrideUnlock != null) { return OverrideUnlock(); }

                if (FlagToCheck != CustomDungeonFlags.NOLLA)
                {
                    return SaveAPIManager.GetFlag(FlagToCheck);
                }
                return true;
            }

            public int ReturnGun(ModulePrinterCore c)
            {
                if (c == null) { return GunID; }
                if (c.ModularGunController == null) { return GunID; }
                return c.ModularGunController.isAlt == true ? AltGunID != -1 ? AltGunID : GunID : GunID;
            }

            public string ReturnNameLabel()
            {
                return Player_Using_Alt_Skin == false ? Name_Label_Sprite_Name : Name_Label_Sprite_Name_Alt ?? Name_Label_Sprite_Name;
            }


            public CustomDungeonFlags FlagToCheck = CustomDungeonFlags.NONE;

            public string Label_Name = null;//"UI_Button_Close";
            public string Label_Press = null;//"Default_Gun_Icon_Hightlighted";
            public string Label_Hover = null;//"Default_Gun_Icon_Pressed";


            public string Upgrade_Description = "Blah Blah Blah";

            public string Unlock_Name = "[LOCKED]";
            public string Unlock_Description = "Blah Blah Blah";

            public string Lock_Label = "ui_button_locked";
            public string Lock_Label_Press = "ui_button_locked_pressed";
            public string Lock_Label_Hover = "ui_button_locked_highlighted";

            public string Name_Label_Sprite_Name = "name_label_WIP";

            public bool Player_Using_Alt_Skin = false;

            public string Label_Name_Alt = null;//"UI_Button_Close";
            public string Label_Press_Alt = null;//"Default_Gun_Icon_Hightlighted";
            public string Label_Hover_Alt = null;//"Default_Gun_Icon_Pressed";

            public string Lock_Label_Alt = "ui_button_locked_alt";
            public string Lock_Label_Press_Alt = "ui_button_locked_pressed_alt";
            public string Lock_Label_Hover_Alt = "ui_button_locked_highlighted_alt";

            public string Name_Label_Sprite_Name_Alt = "name_label_WIP_alt";



            public string Damage_Description = StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            public string FireRate_Description = StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            public string Reload_Description = StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            public string Shotspeed_Description = StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            public string Clipsize_Description = StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            public string Additional_Notes = "";

            public UpgradeType upgradeType = UpgradeType.Minor_Upgrade;
            public enum UpgradeType
            {
                Minor_Upgrade,
                Major_Upgrade,
                Different_Gun,
                Default_Gun
            };

            public int GunID = DefaultArmCannon.ID;
            public int AltGunID = -1;//DefaultArmCannonAlt.ID;

            public dfButton df_Button;

            public int Page = 0;
            public int Entry = 0;

            public Func<bool> OverrideUnlock;

        }

        public class AltSkinStringStorage : MonoBehaviour
        {
            public string df_label_string = null;

            public string df_button_default_string = null;
            public string df_button_pressed_string = null;
            public string df_button_highlighted_string = null;
            public string df_button_inactive_string = null;
        }


        


        public static void Init()
        {



            dfFontBase defaultFont = SaveTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>().Manager.DefaultFont;

            var Bundle = Module.ModularAssetBundle;
            GameObject obj = Bundle.LoadAsset<GameObject>("ModularUIAtlas");
            DontDestroyOnLoad(obj);



            //var defMan = UnityEngine.Object.Instantiate(SaveTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>().Manager);
            //defMan.name = "FUCK YOU!!!";
            //DontDestroyOnLoad(defMan);
            //FakePrefab.MakeFakePrefab(defMan.gameObject);


            var GameUIDefautlAtlas =  SaveTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>().Manager.DefaultAtlas;

            dfAtlas defaultAtlas = obj.GetComponent<dfAtlas>();//Bundle.LoadAsset<dfAtlas>("ModularUIAtlas");//

            //Material mat = Bundle.LoadAsset<Material>("ModularUIAtlas.mat");
            Texture tex = Bundle.LoadAsset<Texture>("ModularUIAtlas");
            //mat.SetTexture("_MainTex", tex);
            for (int i = 0; i < defaultAtlas.items.Count; i++)
            {
                dfAtlas.ItemInfo itemInfo = defaultAtlas.items[i];
                //itemInfo.textureGUID = itemInfo.name;
                defaultAtlas.map.Add(itemInfo.name, itemInfo);
            }
            defaultAtlas.Texture.Apply();

            //defaultAtlas.material = mat;
            //defaultAtlas.Material = mat;
            defaultAtlas.generator = dfAtlas.TextureAtlasGenerator.Internal;


            defaultAtlas.enabled = true;
            def = defaultAtlas;

            //defMan.DefaultAtlas = defaultAtlas;
            //mana = defMan;

            //dfGUIManager.activeInstances.Add(mana);

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked"), "Locked_Icon");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_highlighted"), "Locked_Icon_Hover");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_pressed"), "Locked_Icon_Press");

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_alt"), "Locked_Icon_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_highlighted_alt"), "Locked_Icon_Hover_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_pressed_alt"), "Locked_Icon_Press_Alt");



            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_template_bg"), "template_UI_background");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_template_bg_alt"), "template_UI_background_alt");

            StarterGunSelectUIController.UI_Frame = PrefabBuilder.BuildObject("Frame_Modular_UI");
            StarterGunSelectUIController.UI_Frame.layer = 24;

            var storage = UI_Frame.AddComponent<AltSkinStringStorage>();
            storage.df_label_string = "ui_template_bg_alt";




            dfPanel dfPanel = StarterGunSelectUIController.UI_Frame.AddComponent<dfPanel>();
            dfPanel.anchorStyle = dfAnchorStyle.All;
            dfPanel.isEnabled = false;
            dfPanel.isVisible = true;
            dfPanel.isInteractive = true;
            dfPanel.tooltip = "";
            dfPanel.pivot = dfPivotPoint.TopLeft;
            dfPanel.zindex = -1;
            dfPanel.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            dfPanel.disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            dfPanel.size = new Vector2(640, 360);
            dfPanel.minSize = Vector2.zero;
            dfPanel.maxSize = Vector2.zero;
            dfPanel.clipChildren = false;
            dfPanel.inverseClipChildren = false;
            dfPanel.tabIndex = 1;
            dfPanel.canFocus = false;
            dfPanel.autoFocus = false;
            dfPanel.layout = new dfControl.AnchorLayout(dfAnchorStyle.All)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 0f,
                    left = 0f,
                    right = 0f,
                    top = 0f
                },
                owner = dfPanel
            };
            dfPanel.renderOrder = 30;
            dfPanel.isLocalized = true;
            dfPanel.hotZoneScale = Vector2.one;
            dfPanel.allowSignalEvents = true;
            dfPanel.PrecludeUpdateCycle = false;
            dfPanel.Atlas = defaultAtlas;
            dfPanel.atlas = defaultAtlas;
            
            dfPanel.backgroundSprite = "ui_template_bg";
            dfPanel.backgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            //dfPanel.padding = new RectOffset(0, 0, 0, 0);
            dfPanel.gameObject.SetActive(true);


            float mult = GameManager.Options.SmallUIEnabled == true ? 1 : 2;

            StarterGunSelectUIController gunSelectUIController = StarterGunSelectUIController.UI_Frame.AddComponent<StarterGunSelectUIController>();


            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close"), "UI_Button_Close");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_highlighted"), "UI_Button_Close_Highlighted");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_pressed"), "UI_Button_Close_Pressed");

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_alt"), "UI_Button_Close_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_highlighted_alt"), "UI_Button_Close_Highlighted_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_pressed_alt"), "UI_Button_Close_Pressed_Alt");

            GameObject closeButton_object = PrefabBuilder.BuildObject("CloseButton");
            closeButton_object.layer = 24;
            closeButton_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
            dfButton closeButton = closeButton_object.CreateBlankDfButton(new Vector2(160f / mult, 160f/ mult), dfAnchorStyle.Bottom | dfAnchorStyle.Left , new dfAnchorMargins
            {
                bottom = 1275 / mult,
                left = 530 / mult,
                right = 0f,
                top = 0f
            });
            closeButton.AssignDefaultPresets(defaultAtlas, defaultFont);
            closeButton.backgroundSprite = "ui_button_close";
            closeButton.hoverSprite = "ui_button_close_highlighted";
            closeButton.disabledSprite = "ui_button_close_pressed";
            closeButton.focusSprite = "UI_Button_Close";
            closeButton.pressedSprite = "UI_Button_Close_Pressed";

            var storage_closeButton = closeButton.gameObject.AddComponent<AltSkinStringStorage>();
            storage_closeButton.df_button_default_string = "ui_button_close_alt";
            storage_closeButton.df_button_inactive_string = "ui_button_close_alt";
            storage_closeButton.df_button_pressed_string = "ui_button_close_pressed_alt";
            storage_closeButton.df_button_highlighted_string = "ui_button_close_highlighted_alt";

            gunSelectUIController.Close_Button = closeButton;
            closeButton.gameObject.SetActive(false);




            {
                //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun"), "Default_Gun_Icon");
                //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_highlighted"), "Default_Gun_Icon_Hightlighted");
                //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_pressed"), "Default_Gun_Icon_Pressed");

                //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_alt"), "Default_Gun_Icon_Alt");
                //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_highlighted_alt"), "Default_Gun_Icon_Hightlighted_Alt");
                //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_pressed_alt"), "Default_Gun_Icon_Pressed_Alt");

                GameObject Default_Gun_Button_object = PrefabBuilder.BuildObject("Default_Gun_Button");
                //Default_Gun_Button_object.layer = 24;
                Default_Gun_Button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                dfButton Default_Gun_Button = Default_Gun_Button_object.CreateBlankDfButton(new Vector2(160 / mult, 160 / mult), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 1250 / mult,
                    left = 936 / mult,
                    right = 0,
                    top = 0
                });
                Default_Gun_Button.AssignDefaultPresets(defaultAtlas, defaultFont);
                Default_Gun_Button.backgroundSprite = "ui_button_default_gun";
                Default_Gun_Button.hoverSprite = "ui_button_default_gun_highlighted";
                Default_Gun_Button.disabledSprite = "ui_button_default_gun";
                Default_Gun_Button.focusSprite = "ui_button_default_gun_highlighted";
                Default_Gun_Button.pressedSprite = "ui_button_default_gun_pressed";
                //Default_Gun_Button.RelativePosition = new Vector3(4, 0);
                Default_Gun_Button.atlas = defaultAtlas;
                Default_Gun_Button.gameObject.SetActive(false);

                var upgrade_button_default_gun = Default_Gun_Button_object.AddComponent<UpgradeUISelectButtonController>();
                upgrade_button_default_gun.df_Button = Default_Gun_Button;
                upgrade_button_default_gun.FlagToCheck = CustomDungeonFlags.NOLLA;
                upgrade_button_default_gun.upgradeType = UpgradeUISelectButtonController.UpgradeType.Default_Gun;
                upgrade_button_default_gun.Upgrade_Description = "Simple, yet powerful weapon.\nFavoured by the Modular.";

                upgrade_button_default_gun.Label_Name = "ui_button_default_gun";
                upgrade_button_default_gun.Label_Press = "ui_button_default_gun_pressed";
                upgrade_button_default_gun.Label_Hover = "ui_button_default_gun_highlighted";
                
                upgrade_button_default_gun.Label_Name_Alt = "ui_button_default_gun_alt";
                upgrade_button_default_gun.Label_Press_Alt = "ui_button_default_gun_pressed_alt";
                upgrade_button_default_gun.Label_Hover_Alt = "ui_button_default_gun_highlighted_alt";

                upgrade_button_default_gun.Name_Label_Sprite_Name = "name_label_defualtcannon";
                upgrade_button_default_gun.Name_Label_Sprite_Name_Alt = "name_label_defualtcannon_alt";


                //upgrade_button_default_gun.Name_Label_Sprite_Name = defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_defualtcannon"), "Default_Arm_Cannon_Text").name;
                //upgrade_button_default_gun.Name_Label_Sprite_Name_Alt = defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_defualtcannon_alt"), "Default_Arm_Cannon_Text_Alt").name;

                gunSelectUIController.default_gun_button = upgrade_button_default_gun;

            }

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_doublehalf"), "DB_H_Default");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_doublehalf_highlighted"), "DB_H_Default_HightLighted");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_doublehalf_pressed"), "DB_H_Default_Pressed");

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun"), "Peashooter_Gun_Icon");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_highlighted"), "Peashooter_Gun_Icon_Hightlighted");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_pressed"), "Peashooter_Gun_Icon_Pressed");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_alt"), "Peashooter_Gun_Icon_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_highlighted_alt"), "Peashooter_Gun_Icon_Hightlighted_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_pressed_alt"), "Peashooter_Icon_Pressed_Alt");

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_peashooter"), "Modular_Name_Label_PeaShooter");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_peashooter_alt"), "Modular_Name_Label_PeaShooter_Alt");

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "PeaShooter_Button"
                   , "ui_button_peashooter_gun" //asset name default
                   , "ui_button_peashooter_gun_highlighted" //asset name highlighted
                   , "ui_button_peashooter_gun_pressed" //asset name pressed
                   , CustomDungeonFlags.BEAT_FLOOR_3 //Unlock flag, Set itt to NOLLA for no unlock condition
                   , ModularPeaShooter.GunID //Gun ID
                   , "Weaker, but allows for extra power." //Default description
                   , StaticColorHexes.AddColorToLabelString("Low", StaticColorHexes.Orange_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Yellow_Hex) //Clipsize Secription
                   , null //Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Shot Speed Secription
                   , "Grants the user 1 additional." + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER)  //Additional Notes, keep at one line
                   , "name_label_peashooter" //Label Name Asset Name
                   , ModularPeaShooterAlt.GunID //alt gun ID
                   , "ui_button_peashooter_gun_alt" //asset name default alt
                   , "ui_button_peashooter_gun_highlighted_alt" //asset name highlighted alt
                   , "ui_button_peashooter_gun_pressed_alt" //asset name pressed alt
                   , "name_label_peashooter_alt"  //Label Name Asset Name Alt
                   , "As Modular, reach and beat\nthe Black Powder Mines."); //Unlock Description

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun"), "scattercannon_Gun_Icon");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_highlighted"), "scattercannon_Gun_Icon_Hightlighted");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_pressed"), "scattercannon_Gun_Icon_Pressed");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_alt"), "scattercannon_Gun_Icon_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_highlighted_alt"), "scattercannon_Gun_Icon_Hightlighted_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_pressed_alt"), "scattercannon_Icon_Pressed_Alt");

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_scattercannon"), "Modular_Name_Label_scattercannon");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_scattercannon_alt"), "Modular_Name_Label_scattercannon_Alt");

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "ScatterCannon_Button"
                   , "ui_button_scattercannon_gun" //asset name default
                   , "ui_button_scattercannon_gun_highlighted" //asset name highlighted
                   , "ui_button_scattercannon_gun_pressed" //asset name pressed
                   , CustomDungeonFlags.BEAT_FLOOR_3 //Unlock flag, Set itt to NOLLA for no unlock condition
                   , ScatterBlast.ID //Gun ID
                   , "A basic scatter cannon.\nFires 6 pellets with one shot." //Default description
                   , StaticColorHexes.AddColorToLabelString("High", StaticColorHexes.Orange_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Slower Than Average", StaticColorHexes.Yellow_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Yellow_Hex) //Clipsize Secription
                   , StaticColorHexes.AddColorToLabelString("Low", StaticColorHexes.Orange_Hex) //Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex) //Shot Speed Secription
                   , "Has high spread, and lower range."  //Additional Notes, keep at one line
                   , "name_label_scattercannon" //Label Name Asset Name
                   , ScatterBlastAlt.ID //alt gun ID
                   , "ui_button_scattercannon_gun_alt" //asset name default alt
                   , "ui_button_scattercannon_gun_highlighted_alt" //asset name highlighted alt
                   , "ui_button_scattercannon_gun_pressed_alt" //asset name pressed alt
                   , "name_label_scattercannon_alt"  //Label Name Asset Name Alt
                   , "As Modular, reach and beat\nthe Black Powder Mines."); //Unlock Description


          


            //=============================================================================================================================================
            //=============================================================================================================================================
            //Energy Charger ================================================================================================================================
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_quadburster_gun"), "quadburster_Gun_Icon");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_quadburster_gun_highlighted"), "quadburster_Gun_Icon_Hightlighted");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_quadburster_gun_pressed"), "quadburster_Gun_Icon_Pressed");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_quadburster_gun_alt"), "quadburster_Gun_Icon_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_quadburster_gun_highlighted_alt"), "quadburster_Gun_Icon_Hightlighted_Alt");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_quadburster_gun_pressed_alt"), "quadburster_Icon_Pressed_Alt");

            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_QuadBurster"), "Modular_Name_Label_quadburster");
            //defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_QuadBurster_alt"), "Modular_Name_Label_quadburster_Alt");

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "QuadBurster_Button"
                   , "ui_button_quadburster_gun" //asset name default
                   , "ui_button_quadburster_gun_highlighted" //asset name highlighted
                   , "ui_button_quadburster_gun_pressed" //asset name pressed
                   , CustomDungeonFlags.BEAT_FLOOR_3 //Unlock flag, Set itt to NOLLA for no unlock condition
                   , TriBurst.ID //Gun ID
                   , "A 4-shot burst gun." //Default description
                   , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Yellow_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Slow", StaticColorHexes.Orange_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Higher Than Average", StaticColorHexes.Light_Green_Hex) //Clipsize Secription
                   , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Green_Hex) //Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Faster Than Average", StaticColorHexes.Light_Green_Hex) //Shot Speed Secription
                   , "Very fast bursts, but high cooldown."  //Additional Notes, keep at one line
                   , "name_label_QuadBurster" //Label Name Asset Name
                   , TriBurstAlt.ID //alt gun ID
                   , "ui_button_quadburster_gun_alt" //asset name default alt
                   , "ui_button_quadburster_gun_highlighted_alt" //asset name highlighted alt
                   , "ui_button_quadburster_gun_pressed_alt" //asset name pressed alt
                   , "name_label_QuadBurster_alt"  //Label Name Asset Name Alt
                   , "As Modular, reach and beat\nthe Black Powder Mines."); //Unlock Description
             //=============================================================================================================================================
             //=============================================================================================================================================
             //=============================================================================================================================================


            //=============================================================================================================================================
            //=============================================================================================================================================
            //Quad Burster ================================================================================================================================
            /*
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_charger_gun"), "charger_Gun_Icon");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_charger_gun_highlighted"), "charger_Gun_Icon_Hightlighted");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_charger_gun_pressed"), "charger_Gun_Icon_Pressed");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_charger_gun_alt"), "charger_Gun_Icon_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_charger_gun_highlighted_alt"), "charger_Gun_Icon_Hightlighted_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_charger_gun_pressed_alt"), "charger_Icon_Pressed_Alt");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_energycharger"), "Modular_Name_Label_energycharger");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_charger_alt"), "Modular_Name_Label_energycharger_alt");
            */
            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "EnergyBlaster_Button"
                   , "ui_button_charger_gun" //asset name default
                   , "ui_button_charger_gun_highlighted" //asset name highlighted
                   , "ui_button_charger_gun_pressed" //asset name pressed
                   , CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR //Unlock flag, Set itt to NOLLA for no unlock condition
                   , ChargeBlaster.GunID //Gun ID
                   , "A charge-shot gun. Charge up\nto fire a fast, high damage shot." //Default description
                   , StaticColorHexes.AddColorToLabelString("High", StaticColorHexes.Green_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Very Slow", StaticColorHexes.Red_Color_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Low", StaticColorHexes.Orange_Hex) //Clipsize Secription
                   , StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Light_Green_Hex) //Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Green_Hex) //Shot Speed Secription
                   , "Make your shots count."  //Additional Notes, keep at one line
                   , "name_label_energycharger" //Label Name Asset Name
                   , ChargeBlasterAlt.GunID //alt gun ID
                   , "ui_button_charger_gun_alt" //asset name default alt
                   , "ui_button_charger_gun_highlighted_alt" //asset name highlighted alt
                   , "ui_button_charger_gun_pressed_alt" //asset name pressed alt
                   , "name_label_charger_alt"  //Label Name Asset Name Alt
                   , "As Modular, reach and beat\nthe Forge."); //Unlock Description
             //=============================================================================================================================================
             //=============================================================================================================================================
             //=============================================================================================================================================

            //=============================================================================================================================================
            //=============================================================================================================================================
            //Precision Rifle ================================================================================================================================
            /*
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_precisionrifle_gun"), "precisionrifle_Gun_Icon");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_precisionrifle_gun_highlighted"), "precisionrifle_Gun_Icon_Hightlighted");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_precisionrifle_gun_pressed"), "precisionrifle_Gun_Icon_Pressed");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_precisionrifle_gun_alt"), "precisionrifle_Gun_Icon_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_precisionrifle_gun_highlighted_alt"), "precisionrifle_Gun_Icon_Hightlighted_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_precisionrifle_gun_pressed_alt"), "precisionrifle_Icon_Pressed_Alt");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_PrecisionRifle"), "Modular_Name_Label_PrecisionRifle");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_charger_alt"), "Modular_Name_Label_PrecisionRifle_alt");
            */
            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "PrecisionRifle_Button"
                   , "ui_button_precisionrifle_gun" //asset name default
                   , "ui_button_precisionrifle_gun_highlighted" //asset name highlighted
                   , "ui_button_precisionrifle_gun_pressed" //asset name pressed
                   , CustomDungeonFlags.BEAT_DRAGUN_AS_MODULAR //Unlock flag, Set itt to NOLLA for no unlock condition
                   , PresicionRifle.ID //Gun ID
                   , "A slow fire, accurate rifle.\nCan "+ StaticColorHexes.AddColorToLabelString("pierce", StaticColorHexes.Orange_Hex) + " enemies." //Default description
                   , StaticColorHexes.AddColorToLabelString("Very High", StaticColorHexes.Green_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Very Slow", StaticColorHexes.Red_Color_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Very Low", StaticColorHexes.Red_Color_Hex) //Clipsize Secription
                   , StaticColorHexes.AddColorToLabelString("Very Slow", StaticColorHexes.Red_Color_Hex) //Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Very Fast", StaticColorHexes.Green_Hex) //Shot Speed Secription
                   , "Line up shots to kill groups."  //Additional Notes, keep at one line
                   , "name_label_PrecisionRifle" //Label Name Asset Name
                   , PresicionRifleAlt.ID //alt gun ID
                   , "ui_button_precisionrifle_gun_alt" //asset name default alt
                   , "ui_button_precisionrifle_gun_highlighted_alt" //asset name highlighted alt
                   , "ui_button_precisionrifle_gun_pressed_alt" //asset name pressed alt
                   , "name_label_PrecisionRifle_alt"  //Label Name Asset Name Alt
                   , "As Modular, reach and beat\nthe Forge."); //Unlock Description
                                                                //=============================================================================================================================================
                                                                //=============================================================================================================================================
                                                                //=============================================================================================================================================


            //=============================================================================================================================================
            //=============================================================================================================================================
            //Suppressor ================================================================================================================================
            /*
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_suppressor_gun"), "suppressor_Gun_Icon");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_suppressor_gun_highlighted"), "suppressor_Gun_Icon_Hightlighted");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_suppressor_gun_pressed"), "suppressor_Gun_Icon_Pressed");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_suppressor_gun_alt"), "suppressor_Gun_Icon_Alt");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_suppressor_gun_highlighted_alt"), "suppressor_Gun_Icon_Hightlighted_Alt");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_suppressor_gun_pressed_alt"), "suppressor_Icon_Pressed_Alt");


            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_minigun"), "Modular_Name_Label_Suppressor");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_suppressor_alt"), "Modular_Name_Label_Suppressor_alt");
            */
            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "Suppressor_Button"
                   , "ui_button_suppressor_gun" //asset name default
                   , "ui_button_suppressor_gun_highlighted" //asset name highlighted
                   , "ui_button_suppressor_gun_pressed" //asset name pressed
                   , CustomDungeonFlags.BEAT_LICH_AS_MODULAR //Unlock flag, Set itt to NOLLA for no unlock condition
                   , Suppressor.GunID //Gun ID
                   , "Starts off slow, but\nfire rate increases fast." //Default description
                   , StaticColorHexes.AddColorToLabelString("Very Low", StaticColorHexes.Red_Color_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Slow", StaticColorHexes.Light_Orange_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Very High", StaticColorHexes.Green_Hex) //Clipsize Secription
                   , StaticColorHexes.AddColorToLabelString("Very Slow", StaticColorHexes.Red_Color_Hex) + StaticColorHexes.AddColorToLabelString(", increases fast.", StaticColorHexes.Green_Hex)//Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Shot Speed Secription
                   , "Hold the trigger for as long as possible."  //Additional Notes, keep at one line
                   , "name_label_minigun" //Label Name Asset Name
                   , SuppressorAlt.GunID //alt gun ID
                   , "ui_button_suppressor_gun_alt" //asset name default alt
                   , "ui_button_suppressor_gun_highlighted_alt" //asset name highlighted alt
                   , "ui_button_suppressor_gun_pressed_alt" //asset name pressed alt
                   , "name_label_suppressor_alt"  //Label Name Asset Name Alt
                   , "As Modular, reach and beat\n Bullet Hell."); //Unlock Description

            //=============================================================================================================================================
            //=============================================================================================================================================
            //=============================================================================================================================================

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                  "Hammer_Button"
                  , "ui_button_hammer_gun" //asset name default
                  , "ui_button_hammer_gun_highlighted" //asset name highlighted
                  , "ui_button_hammer_gun_pressed" //asset name pressed
                  , CustomDungeonFlags.BEAT_LICH_AS_MODULAR //Unlock flag, Set itt to NOLLA for no unlock condition
                  , TheHammer.ID //Gun ID
                  , "Has an active reload that\ninstantly reloads the clip." //Default description
                  , StaticColorHexes.AddColorToLabelString("High", StaticColorHexes.Red_Color_Hex)  //Damage Secription
                  , StaticColorHexes.AddColorToLabelString("Very Slow", StaticColorHexes.Red_Color_Hex) + StaticColorHexes.AddColorToLabelString(" unless perfect.", StaticColorHexes.Green_Hex)//Reload Secription
                  , StaticColorHexes.AddColorToLabelString("Very Low", StaticColorHexes.Red_Color_Hex) //Clipsize Secription
                  , StaticColorHexes.AddColorToLabelString("Below Average", StaticColorHexes.Light_Orange_Hex)//Fire rate Secription
                  , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Shot Speed Secription
                  , "Get into rhythm."  //Additional Notes, keep at one line
                  , "name_label_hammer" //Label Name Asset Name
                  , TheHammerAlt.ID //alt gun ID
                  , "ui_button_hammer_gun_alt" //asset name default alt
                  , "ui_button_hammer_gun_highlighted_alt" //asset name highlighted alt
                  , "ui_button_hammer_gun_pressed_alt" //asset name pressed alt
                  , "name_label_hammer_alt"  //Label Name Asset Name Alt
                  , "As Modular, reach and beat\nBullet Hell."); //Unlock Description

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                 "ApolloAndArtemisButton"
                 , "ui_button_apollo_gun" //asset name default
                 , "ui_button_apollo_gun_highlighted" //asset name highlighted
                 , "ui_button_apollo_gun_pressed" //asset name pressed
                 , CustomDungeonFlags.BEAT_OLD_KING_AS_MODULAR //Unlock flag, Set itt to NOLLA for no unlock condition
                 , Apollo.GunID //Gun ID
                 , "Charge up to change accuracy." //Default description
                 , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Light_Orange_Hex)  //Damage Secription
                 , StaticColorHexes.AddColorToLabelString("Slow", StaticColorHexes.Orange_Hex)//Reload Secription
                 , StaticColorHexes.AddColorToLabelString("Low", StaticColorHexes.Light_Orange_Hex) //Clipsize Secription
                 , StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Lime_Green_Hex)//Fire rate Secription
                 , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Shot Speed Secription
                 , "Charge up only as much as you need to."  //Additional Notes, keep at one line
                 , "name_label_apollo" //Label Name Asset Name
                 , Artemis.GunID //alt gun ID
                 , "ui_button_artemis_gun_alt" //asset name default alt
                 , "ui_button_artemis_gun_highlighted_alt" //asset name highlighted alt
                 , "ui_button_artemis_gun_pressed_alt" //asset name pressed alt
                 , "name_label_artemis"  //Label Name Asset Name Alt
                 , "As Modular, beat the\nOld King."); //Unlock Description

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                "GravityPulsarGun"
                , "ui_button_grav_gun" //asset name default
                , "ui_button_grav_gun_highlighted" //asset name highlighted
                , "ui_button_grav_gun_pressed" //asset name pressed
                , CustomDungeonFlags.BEAT_RAT_AS_MODULAR //Unlock flag, Set itt to NOLLA for no unlock condition
                , GravityPulsar.ID //Gun ID
                , "Creates Rifts that attracts\nits own projectiles." //Default description
                , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Light_Orange_Hex)  //Damage Secription
                , StaticColorHexes.AddColorToLabelString("Slow", StaticColorHexes.Orange_Hex)//Reload Secription
                , StaticColorHexes.AddColorToLabelString("High", StaticColorHexes.Green_Hex) //Clipsize Secription
                , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Light_Orange_Hex)//Fire rate Secription
                , StaticColorHexes.AddColorToLabelString("Very Fast", StaticColorHexes.Green_Hex) //Shot Speed Secription
                , "Group Up enemies for best results."  //Additional Notes, keep at one line
                , "name_label_gravgun" //Label Name Asset Name
                , GravityPulsarAlt.ID //alt gun ID
                , "ui_button_grav_gun_alt" //asset name default alt
                , "ui_button_grav_gun_highlighted_alt" //asset name highlighted alt
                , "ui_button_grav_gun_pressed_alt" //asset name pressed alt
                , "name_label_gravgun_alt"  //Label Name Asset Name Alt
                , "As Modular, beat the\nResourceful Rat."); //Unlock Description

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                "LightLanceButton"
                , "ui_button_lance_gun" //asset name default
                , "ui_button_lance_gun_highlighted" //asset name highlighted
                , "ui_button_lance_gun_pressed" //asset name pressed
                , CustomDungeonFlags.PAST //Unlock flag, Set itt to NOLLA for no unlock condition
                , LightLance.ID //Gun ID
                , "Melee weapon.\nCharge up to parry projectiles." //Default description
                , StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Light_Green_Hex)  //Damage Secription
                , StaticColorHexes.AddColorToLabelString("Slower Than Average", StaticColorHexes.Light_Orange_Hex)//Reload Secription
                , StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Light_Green_Hex) //Clipsize Secription
                , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Green_Hex)//Fire rate Secription
                , StaticColorHexes.AddColorToLabelString("N/A | Very Slow", StaticColorHexes.Red_Color_Hex) //Shot Speed Secription
                , "Parrying Faster Bullets deals more damage."  //Additional Notes, keep at one line
                , "name_label_lightlance" //Label Name Asset Name
                , LightLanceAlt.ID //alt gun ID
                , "ui_button_lance_gun_alt" //asset name default alt
                , "ui_button_lance_gun_highlighted_alt" //asset name highlighted alt
                , "ui_button_lance_gun_pressed_alt" //asset name pressed alt
                , "name_label_lightlance_alt"  //Label Name Asset Name Alt
                , "As Modular, beat your\nPast."); //Unlock Description

            /*
            for (int i = 0; i < 13; i++)
            {
                GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                    "Double_Half_Button" + i.ToString()
                    , "DB_H_Default"
                    , "DB_H_Default_HightLighted"
                    , "DB_H_Default_Pressed"
                    , CustomDungeonFlags.NOLLA //Unlock flag, Set itt to NOLLA for no unlock condition
                    , 56 //Gun ID
                    , "Is Literally Just\nThe 38 Special" //Default description
                    , null  //Damage Secription
                    , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Reload Secription
                    , StaticColorHexes.AddColorToLabelString("Low", StaticColorHexes.Orange_Hex) //Clipsize Secription
                    , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Fire rate Secription
                    , null //Shot Speed Secription
                    , "Gun does not have infinite ammo."  //Additional Notes, keep at one line
                    , "Modular_Name_Label_WIP"); //sprite name for the Big Name Label
                
            }
            */


            {
                /*
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active"), "Mod_Accept");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_1"), "Mod_Accept_Inactive");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_press"), "Mod_Accept_Pressed");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_highlight"), "Mod_Accept_Highlight");

                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_alt"), "Mod_Accept_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_1_alt"), "Mod_Accept_Inactive_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_press_alt"), "Mod_Accept_Pressed_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_highlight_alt"), "Mod_Accept_Highlight_Alt");
                */
                GameObject accept_Button_object = PrefabBuilder.BuildObject("AcceptButton");
                //accept_Button_object.layer = 24;
                accept_Button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                dfButton acceptButton_df = accept_Button_object.CreateBlankDfButton(new Vector2(640f / mult, 160f / mult), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 550 / mult,
                    left = 1680 / mult,
                    right = 0f,
                    top = 0f
                });
                acceptButton_df.AssignDefaultPresets(defaultAtlas, defaultFont);
                acceptButton_df.backgroundSprite = "ui_button_accept_active";
                acceptButton_df.hoverSprite = "ui_button_accept_highlight";
                acceptButton_df.disabledSprite = "ui_button_accept_active_1";
                acceptButton_df.focusSprite = "ui_button_accept_highlight";
                acceptButton_df.pressedSprite = "ui_button_accept_active_press";
                acceptButton_df.atlas = defaultAtlas;

                var storage_acceptButton = acceptButton_df.gameObject.AddComponent<AltSkinStringStorage>();
                storage_acceptButton.df_button_default_string = "ui_button_accept_active_alt";
                storage_acceptButton.df_button_inactive_string = "ui_button_accept_active_1_alt";
                storage_acceptButton.df_button_highlighted_string = "ui_button_accept_highlight_alt";
                storage_acceptButton.df_button_pressed_string = "ui_button_accept_active_press_alt";

                gunSelectUIController.Accept_Button = acceptButton_df;
                acceptButton_df.gameObject.SetActive(false);

            }

            {
                /*
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_active"), "Left_Button_Active");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_inactive"), "Left_Button_Inactive");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_highlighted"), "Left_Button_Highlight");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_pressed"), "Left_Button_Pressed");

                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_active_alt"), "Left_Button_Active_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_inactive_alt"), "Left_Button_Inactive_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_highlighted_alt"), "Left_Button_Highlight_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_pressed_alt"), "Left_Button_Pressed_Alt");
                */

                GameObject left_button_object = PrefabBuilder.BuildObject("Left_Page_Button");
                //left_button_object.layer = 24;
                left_button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                dfButton left_button = left_button_object.CreateBlankDfButton(new Vector2(80 / mult, 80f / mult), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 500 / mult,
                    left = 748 / mult,
                    right = 0f,
                    top = 0f
                });
                left_button.AssignDefaultPresets(defaultAtlas, defaultFont);
                left_button.backgroundSprite = "left_button_active";
                left_button.hoverSprite = "left_button_highlighted";
                left_button.disabledSprite = "left_button_inactive";
                left_button.focusSprite = "left_button_highlighted";
                left_button.pressedSprite = "left_button_pressed";
                left_button.atlas = defaultAtlas;
                left_button.Atlas = defaultAtlas;
                left_button.gameObject.SetActive(false);

                var storage_left_button = left_button.gameObject.AddComponent<AltSkinStringStorage>();
                storage_left_button.df_button_default_string = "left_button_active_alt";
                storage_left_button.df_button_inactive_string = "left_button_inactive_alt";
                storage_left_button.df_button_highlighted_string = "left_button_highlighted_alt";
                storage_left_button.df_button_pressed_string = "left_button_pressed_alt";

                gunSelectUIController.Left_Button = left_button;
            }

            {
                /*
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_active"), "Right_Button_Active");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_inactive"), "Right_Button_Inactive");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_highlighted"), "Right_Button_Highlight");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_pressed"), "Right_Button_Pressed");

                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_active_alt"), "Right_Button_Active_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_inactive_alt"), "Right_Button_Inactive_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_highlighted_alt"), "Right_Button_Highlight_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_pressed_alt"), "Right_Button_Pressed_Alt");
                */
                GameObject right_button_object = PrefabBuilder.BuildObject("Right_Page_Button");
                //right_button_object.layer = 24;
                right_button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                dfButton right_button = right_button_object.CreateBlankDfButton(new Vector2(80 / mult, 80f / mult), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 500 / mult,
                    left = 1180 / mult,
                    right = 0f,
                    top = 0f
                });
                right_button.AssignDefaultPresets(defaultAtlas, defaultFont);
                right_button.backgroundSprite = "right_button_active";
                right_button.hoverSprite = "right_button_highlighted";
                right_button.disabledSprite = "right_button_inactive";
                right_button.focusSprite = "right_button_highlighted";
                right_button.pressedSprite = "right_button_pressed";
                right_button.atlas = defaultAtlas;
                right_button.gameObject.SetActive(false);

                var storage_right_button = right_button.gameObject.AddComponent<AltSkinStringStorage>();
                storage_right_button.df_button_default_string = "right_button_active_alt";
                storage_right_button.df_button_inactive_string = "right_button_inactive_alt";
                storage_right_button.df_button_highlighted_string = "right_button_highlighted_alt";
                storage_right_button.df_button_pressed_string = "right_button_pressed_alt";

                gunSelectUIController.Right_Button = right_button;
            }

            dfLabel StatDescriptionLabel = PrefabBuilder.BuildObject("StatDescriptionLabel_Object").AddComponent<dfLabel>();
            StatDescriptionLabel.gameObject.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
            StatDescriptionLabel.AssignDefaultPresets(GameUIDefautlAtlas, defaultFont);
            StatDescriptionLabel.anchorStyle = (dfAnchorStyle.Bottom | dfAnchorStyle.Left);
            StatDescriptionLabel.color = new Color32(155, 235, 199, 255);
            StatDescriptionLabel.shadow = true;
            StatDescriptionLabel.shadowOffset = new Vector2(1, -0.5f);
            StatDescriptionLabel.ShadowColor = new Color32(1, 1, 1, 255);
            StatDescriptionLabel.textScale *= (0.95f / mult);
            StatDescriptionLabel.gameObject.SetActive(false);

            StatDescriptionLabel.layout = new dfControl.AnchorLayout(dfAnchorStyle.Bottom | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = ((1020f / mult) - 1000 ) - (mult == 2 ? 20 : 0),
                    left = (1670f / mult) + 1000,
                    right = 0,
                    top = 0
                },
                owner = StatDescriptionLabel
            };
            StatDescriptionLabel.padding = new RectOffset()
            {
                bottom = -1000,
                top = -1000,
                left = -1000,
                right = -1000,

            };


            gunSelectUIController.statDescrptionLabel = StatDescriptionLabel;

            dfLabel NameDescriptionLabel = PrefabBuilder.BuildObject("NameDescriptionLabel_Object").AddComponent<dfLabel>();
            NameDescriptionLabel.gameObject.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
            NameDescriptionLabel.AssignDefaultPresets(GameUIDefautlAtlas, defaultFont);
            NameDescriptionLabel.anchorStyle = (dfAnchorStyle.Bottom | dfAnchorStyle.Left);
            NameDescriptionLabel.color = new Color32(155, 235, 199, 255);
            NameDescriptionLabel.textScale *= (1.15f / mult);
            NameDescriptionLabel.shadow = true;
            NameDescriptionLabel.shadowOffset = new Vector2(1, -0.5f);
            NameDescriptionLabel.ShadowColor = new Color32(1, 1, 1, 255);
            NameDescriptionLabel.gameObject.SetActive(false);

            NameDescriptionLabel.layout = new dfControl.AnchorLayout(dfAnchorStyle.Bottom | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = ((1140f / mult)-1000)-(mult == 2 ? 20 : 0),
                    left = (1670f / mult)+1000,
                    right = 0,
                    top = 0
                },
                owner = NameDescriptionLabel
            };
            NameDescriptionLabel.padding = new RectOffset()
            {
                bottom = -1000,
                top = -1000,
                left = -1000,
                right = -1000,

            };
            gunSelectUIController.name_and_Description_Label = NameDescriptionLabel;


            var name_display_Object = PrefabBuilder.BuildObject("Name_display_Panel");
            //name_display_Object.layer = 24;



            /*
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_empty"), "Modular_Name_Label_None");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_locked"), "Modular_Name_Label_Locked");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_WIP"), "Modular_Name_Label_WIP");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_empty_alt"), "Modular_Name_Label_None_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_locked_alt"), "Modular_Name_Label_Locked_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_WIP_alt"), "Modular_Name_Label_WIP_Alt");
            */

            dfPanel name_display_Object_panel = name_display_Object.AddComponent<dfPanel>();
            name_display_Object_panel.AssignDefaultPresets(defaultAtlas, new Vector2(680.5f / mult, 222.5f / mult), dfAnchorStyle.Bottom | dfAnchorStyle.Left);
            //name_display_Object_panel.gameObject.transform.localScale /= 2;

            name_display_Object_panel.layout = new dfControl.AnchorLayout(dfAnchorStyle.Bottom | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 1195 / mult,
                    left = 1655 / mult,
                    right = 0f,
                    top = 0f
                },
                owner = name_display_Object_panel
            };
            name_display_Object_panel.backgroundSprite = "name_label_empty";
            name_display_Object_panel.atlas = defaultAtlas;
            name_display_Object_panel.Atlas = defaultAtlas;
            name_display_Object_panel.gameObject.SetActive(false);

            name_display_Object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
            gunSelectUIController.Name_Panel = name_display_Object_panel;
        }

        public static void GenerateNewGunButton(dfAtlas dfAtlas, dfFontBase defaultFont, StarterGunSelectUIController self, string Button_Name, string asset_name_default, string asset_name_highlighted, string asset_name_pressed, CustomDungeonFlags unlock, int GunId,
            string Basic_Description = "",
            string Damage_Description = null,
            string Reload_Description = null,
            string Clipsize_Description = null,
            string FireRate_Description = null,
            string ShotSpeed_Description = null,
            string AdditionalNotes = "",
            string Label_Name_Asset_Name = "name_label_WIP",
            int Alt_GunID = -1,
             string asset_name_default_alt = null,
             string asset_name_highlighted_alt = null,
             string asset_name_pressed_alt = null,
             string Label_Name_Asset_Name_Alt = "name_label_WIP_alt", string UnlockDescription = "Blah Blah Blah", Func<bool> OverrideUnlock = null
            )
        {
            float mult = GameManager.Options.SmallUIEnabled == true ? 1 : 2;

            GameObject Default_Gun_Button_object = PrefabBuilder.BuildObject(Button_Name+"_Object");
            //Default_Gun_Button_object.layer = 24;
            Default_Gun_Button_object.gameObject.transform.parent = StarterGunSelectUIController.UI_Frame.transform;

            var upgrade_button_default_gun = Default_Gun_Button_object.AddComponent<UpgradeUISelectButtonController>();

            upgrade_button_default_gun.Label_Name = asset_name_default;
            upgrade_button_default_gun.Label_Press = asset_name_pressed;
            upgrade_button_default_gun.Label_Hover = asset_name_highlighted;

            upgrade_button_default_gun.Name_Label_Sprite_Name = Label_Name_Asset_Name;


            upgrade_button_default_gun.Label_Name_Alt = asset_name_default_alt ?? asset_name_default;
            upgrade_button_default_gun.Label_Press_Alt = asset_name_pressed_alt ?? asset_name_pressed;
            upgrade_button_default_gun.Label_Hover_Alt = asset_name_highlighted_alt ?? asset_name_highlighted;

            upgrade_button_default_gun.Name_Label_Sprite_Name_Alt = Label_Name_Asset_Name_Alt ?? Label_Name_Asset_Name;


            upgrade_button_default_gun.FlagToCheck = unlock;
            upgrade_button_default_gun.upgradeType = UpgradeUISelectButtonController.UpgradeType.Different_Gun;
            upgrade_button_default_gun.GunID = GunId;
            upgrade_button_default_gun.AltGunID = Alt_GunID;

            upgrade_button_default_gun.Damage_Description = Damage_Description ?? StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            upgrade_button_default_gun.Reload_Description = Reload_Description ?? StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            upgrade_button_default_gun.Clipsize_Description = Clipsize_Description ?? StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            upgrade_button_default_gun.FireRate_Description = FireRate_Description ?? StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex);
            upgrade_button_default_gun.Shotspeed_Description = ShotSpeed_Description ?? StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex); ;

            upgrade_button_default_gun.Additional_Notes = AdditionalNotes;
            upgrade_button_default_gun.Upgrade_Description = Basic_Description;

            upgrade_button_default_gun.Unlock_Description = UnlockDescription;

            upgrade_button_default_gun.OverrideUnlock = OverrideUnlock;

            int integer = AddNewEntry(self, upgrade_button_default_gun);

            dfButton Default_Gun_Button = Default_Gun_Button_object.CreateBlankDfButton(new Vector2(160 / mult, 160 / mult), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
            {
                bottom = preDeterminedOffsets[integer].y / mult,
                left = preDeterminedOffsets[integer].x / mult,
                right = 0,
                top = 0
            });
            upgrade_button_default_gun.df_Button = Default_Gun_Button;
            Default_Gun_Button.AssignDefaultPresets(dfAtlas, defaultFont);

            Default_Gun_Button.backgroundSprite = asset_name_default;
            Default_Gun_Button.disabledSprite = asset_name_default;
            Default_Gun_Button.hoverSprite = asset_name_highlighted;
            Default_Gun_Button.focusSprite = asset_name_highlighted;
            Default_Gun_Button.pressedSprite = asset_name_pressed;
            

            //Default_Gun_Button.RelativePosition = new Vector3(4, 0);
        }

        //RelativePosition

        [HarmonyPatch(typeof(GameUIRoot), "Start")]
        [HarmonyPostfix]

        private void Update()
        {
        }

        public static StarterGunSelectUIController GenerateUI()
        {
            
            if (GameUIRoot.Instance.Manager.transform.Find("Frame_Modular_UI(Clone)") == null)
            {
                StarterGunSelectUIController.Inst = GameUIRoot.Instance.Manager.AddPrefab(StarterGunSelectUIController.UI_Frame).gameObject.GetComponent<StarterGunSelectUIController>();
                return StarterGunSelectUIController.Inst;

            }
            else
            {
                StarterGunSelectUIController.Inst = GameUIRoot.Instance.Manager.transform.Find("Frame_Modular_UI(Clone)").gameObject.GetComponent<StarterGunSelectUIController>();
                return StarterGunSelectUIController.Inst;
            }
        }

        public void DoAltSkinChecks()
        {
            if (interactor == null) { return; }
            bool altSkin = interactor.IsUsingAlternateCostume;

            var panel_df = Inst.GetComponent<dfPanel>();
            panel_df.backgroundSprite = altSkin == false ? "ui_template_bg" : "ui_template_bg_alt";
            panel_df.Invalidate();


            var fuck_1 = Close_Button.GetComponent<AltSkinStringStorage>();
            Close_Button.backgroundSprite = altSkin == false ? "ui_button_close" : fuck_1.df_button_default_string != null ? fuck_1.df_button_default_string : "ui_button_close";//"UI_Button_Close";
            Close_Button.focusSprite = altSkin == false ? "ui_button_close" : fuck_1.df_button_default_string != null ? fuck_1.df_button_default_string : "ui_button_close";//"UI_Button_Close";
            Close_Button.hoverSprite = altSkin == false ? "ui_button_close_highlighted" : fuck_1.df_button_highlighted_string != null ? fuck_1.df_button_highlighted_string : "ui_button_close_highlighted";//"UI_Button_Close_Highlighted";
            Close_Button.disabledSprite = altSkin == false ? "ui_button_close_pressed" : fuck_1.df_button_pressed_string != null ? fuck_1.df_button_pressed_string : "ui_button_close_pressed";//"UI_Button_Close_Pressed";
            Close_Button.pressedSprite = altSkin == false ? "ui_button_close_pressed" : fuck_1.df_button_pressed_string != null ? fuck_1.df_button_pressed_string : "ui_button_close_pressed";// "UI_Button_Close_Pressed";
            Close_Button.Invalidate();


            var fuck_2 = Accept_Button.GetComponent<AltSkinStringStorage>();
            Accept_Button.backgroundSprite = altSkin == false ? "ui_button_accept_active" : fuck_2.df_button_default_string != null ? fuck_2.df_button_default_string : "ui_button_accept_active";
            Accept_Button.focusSprite = altSkin == false ? "ui_button_accept_highlight" : fuck_2.df_button_default_string != null ? fuck_2.df_button_default_string : "ui_button_accept_highlight";
            Accept_Button.hoverSprite = altSkin == false ? "ui_button_accept_highlight" : fuck_2.df_button_highlighted_string != null ? fuck_2.df_button_highlighted_string : "ui_button_accept_highlight"; ;
            Accept_Button.disabledSprite = altSkin == false ? "ui_button_accept_active_1" : fuck_2.df_button_inactive_string != null ? fuck_2.df_button_inactive_string : "ui_button_accept_active_1";
            Accept_Button.pressedSprite = altSkin == false ? "ui_button_accept_active_press" : fuck_2.df_button_pressed_string != null ? fuck_2.df_button_pressed_string : "ui_button_accept_active_press";
            Accept_Button.Invalidate();

            var fuck_3 = Left_Button.GetComponent<AltSkinStringStorage>();
            Left_Button.backgroundSprite = altSkin == false ? "left_button_active" : fuck_3.df_button_default_string != null ? fuck_3.df_button_default_string : "left_button_active";
            Left_Button.focusSprite = altSkin == false ? "left_button_highlighted" : fuck_3.df_button_default_string != null ? fuck_3.df_button_default_string : "left_button_highlighted";
            Left_Button.hoverSprite = altSkin == false ? "left_button_highlighted" : fuck_3.df_button_highlighted_string != null ? fuck_3.df_button_highlighted_string : "left_button_highlighted"; ;
            Left_Button.disabledSprite = altSkin == false ? "left_button_inactive" : fuck_3.df_button_inactive_string != null ? fuck_3.df_button_inactive_string : "left_button_inactive";
            Left_Button.pressedSprite = altSkin == false ? "left_button_pressed" : fuck_3.df_button_pressed_string != null ? fuck_3.df_button_pressed_string : "left_button_pressed";
            Left_Button.Invalidate();

            var fuck_4 = Right_Button.GetComponent<AltSkinStringStorage>();
            Right_Button.backgroundSprite = altSkin == false ? "right_button_active" : fuck_4.df_button_default_string != null ? fuck_4.df_button_default_string : "right_button_active";
            Right_Button.focusSprite = altSkin == false ? "right_button_highlighted" : fuck_4.df_button_default_string != null ? fuck_4.df_button_default_string : "right_button_highlighted";
            Right_Button.hoverSprite = altSkin == false ? "right_button_highlighted" : fuck_4.df_button_highlighted_string != null ? fuck_4.df_button_highlighted_string : "right_button_highlighted"; ;
            Right_Button.disabledSprite = altSkin == false ? "right_button_inactive" : fuck_4.df_button_inactive_string != null ? fuck_4.df_button_inactive_string : "right_button_inactive";
            Right_Button.pressedSprite = altSkin == false ? "right_button_pressed" : fuck_4.df_button_pressed_string != null ? fuck_4.df_button_pressed_string : "right_button_pressed";
            Right_Button.Invalidate();

            foreach (var List in ButtonControllers_Layers)
            {
                List.Player_Using_Alt_Skin = altSkin;
                List.UpdateSprites();
            }
            default_gun_button.Player_Using_Alt_Skin = altSkin;
            default_gun_button.UpdateSprites();
        }


        public Action OnUsed;
        public Action OnClosed;

        public void OnUse(PlayerController player, int GunID)
        {
            Gun carriedGun = player.CurrentGun;
            ModulePrinterCore var = null;
            if (player.passiveItems != null && player.passiveItems.Count > 0) 
            {
                foreach (var passives in player.passiveItems)
                {
                    if (passives is ModulePrinterCore printerCore)
                    {
                        var = printerCore;
                        printerCore.ModularGunController = null;
                        printerCore.TemporaryDisableDrop = true;
                    }
                }
            }
            Gun newGun = LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(GunID).gameObject, player, false);
            var co = newGun.gameObject.GetOrAddComponent<ModularGunController>();
            if (var != null)
            {
                var.ModularGunController = co;
                var.TemporaryDisableDrop = false;
            }
            player.inventory.DestroyGun(carriedGun);
            if (OnUsed != null)
            {
                OnUsed();
            }
        }


        public bool CanBeSelected()
        {
            if (currentlySelectedButton == null) { Accept_Button.Disable(); return false; }
            if (currentlySelectedButton.IsUnlocked() == false) { Accept_Button.Disable(); return false; }
            if (interactor != null)
            {
                if (interactor.CurrentGun.PickupObjectId == currentlySelectedButton.GunID) 
                { Accept_Button.Disable(); return false; }
            }
            Accept_Button.Enable();
            Accept_Button.isEnabled = true;
            return true;
        }



        private void Start()
        {
            if (interactor == null) 
            {
                foreach (PlayerController p in GameManager.Instance.AllPlayers)
                {
                    if (p.HasPassiveItem(ModulePrinterCore.ModulePrinterCoreID)) { interactor = p; }
                }
            }
            UpdatePanels();
            CursorPatch.DisplayCursorOnController = true;

            this.Accept_Button.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                if (CanBeSelected() == true)
                {
                    UpdatePanels();
                    ModulePrinterCore p = null;
                    foreach (var entry in interactor.passiveItems)
                    {
                        if (entry is ModulePrinterCore printerCore)
                        {
                            p = printerCore;
                        }
                    }
                    AkSoundEngine.PostEvent("Play_FS_slipper_stone_01", this.gameObject);
                    OnUse(interactor, currentlySelectedButton.ReturnGun(p));
                    Inst.ToggleUI(false);
                }
            };
            Accept_Button.MouseEnter += (o1, o2) =>
            {
                if (Close_Button.isActiveAndEnabled)
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
                }
            };

            this.Close_Button.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                Inst.ToggleUI(false);
                if (OnClosed != null)
                {
                    OnClosed();
                    CursorPatch.DisplayCursorOnController = false;
                }
            };
            Close_Button.MouseEnter += (o1, o2) =>
            {
                if (Close_Button.isActiveAndEnabled)
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
                }
            };
            Close_Button.Click += (o1, o2) =>
            {
                if (Close_Button.isActiveAndEnabled)
                {
                    AkSoundEngine.PostEvent("Play_FS_slipper_stone_01", this.gameObject);
                }
            };


            default_gun_button.df_Button.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                name_and_Description_Label.Localize();
                statDescrptionLabel.Localize();

                currentlySelectedButton = default_gun_button;
                UpdateStatDescLabel();
                UpdateNameDescLabel();

                UpdatePanels();
            };



            foreach (var entry in ButtonControllers_Layers)
            {
                entry.UpdateSprites();
                entry.df_Button.Click += ClickyDoo;
            }

            Left_Button.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                Current_Page -= 1;
                UpdatePageButtons();
                foreach (var v in ButtonControllers_Layers)
                {
                    Left_Button.StartCoroutine(this.ButtonFade(v, 0.3f));
                }
                AkSoundEngine.PostEvent("Play_FS_slipper_stone_01", this.gameObject);
            };
            Left_Button.MouseEnter += (o1, o2) =>
            {
                if (Close_Button.isActiveAndEnabled)
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
                }
            };
            Right_Button.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                Current_Page += 1;
                UpdatePageButtons();
                foreach (var v in ButtonControllers_Layers)
                {
                    Right_Button.StartCoroutine(this.ButtonFade(v ,0.3f));
                }
                AkSoundEngine.PostEvent("Play_FS_slipper_stone_01", this.gameObject);

            };
            Right_Button.MouseEnter += (o1, o2) =>
            {
                if (Close_Button.isActiveAndEnabled)
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
                }
            };
        }

     



        public void ClickyDoo(dfControl control, dfMouseEventArgs mouseEvent)
        {
            name_and_Description_Label.Localize();
            statDescrptionLabel.Localize();

            currentlySelectedButton = control.GetComponent<UpgradeUISelectButtonController>();
            UpdateStatDescLabel();
            UpdateNameDescLabel();

            UpdatePanels();
        }

        public IEnumerator DoFade(float from, float to, float duration, bool Deactivate, bool ForceInstant = false)
        {
            if (Deactivate == true)
            {
                name_and_Description_Label.ModifyLocalizedText("");
                statDescrptionLabel.ModifyLocalizedText("");
                DoActivation();
            }

            if (ForceInstant == false)
            {
                var c = Inst.GetComponent<dfPanel>();
                float e = 0;
                while (e < duration)
                {
                    if (c == null) { yield break; }
                    c.Opacity = Mathf.Lerp(from, to, e / 0.25f);
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
            }
           

            if (Deactivate == false) 
            {
                DoActivation();
            }
            yield break;
        }

        public IEnumerator ButtonFade(UpgradeUISelectButtonController self, float duration, bool ForceInstant = false)
        {
            UpdatePageButtons();

            if (ForceInstant == false)
            {
                float e = 0;
                while (e < self.Entry * 0.025f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                bool v = false;
                while (e < duration)
                {
                    if (e > (duration / 2) && v == false)
                    {
                        v = true;
                        //ProcessButtonPages();
                        ProcessButtonPage(self);
                    }
                    self.df_Button.transform.localScale = Vector3.one * (1 - Toolbox.SinLerpTValueFull(e / duration));
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
            }
            self.df_Button.transform.localScale = Vector3.one;
            yield break;
        }


        private static void UIScaleFormla(dfPanel panel)
        {
            panel.transform.localScale = GameUIUtility.GetCurrentTK2D_DFScale(panel.cachedManager) * Vector3.one;

        }

        private static void UIScaleFormla(dfButton panel)
        {
            panel.transform.localScale = Vector3.one * (GameUIUtility.GetCurrentTK2D_DFScale(panel.cachedManager) * 2);

            /*
            int FUckYouGame = panel.GetManager().UIScale < 0.5f ? 1 : 2;
            Vector2 sizeInPixels = panel.Atlas[panel.backgroundSprite].sizeInPixels;
            panel.Size = (sizeInPixels * Pixelator.Instance.CurrentTileScale) / FUckYouGame;
            panel.Invalidate();
            */
        }


        public void DoActivation()
        {
            DoAltSkinChecks();

            var bg = StarterGunSelectUIController.Inst.GetComponent<dfPanel>();
            bg.IsVisible = this.Is_Active;
            bg.IsInteractive = this.Is_Active;
            bg.IsEnabled = this.Is_Active;

            this.name_and_Description_Label.isEnabled = Is_Active;
            name_and_Description_Label.gameObject.SetActive(Is_Active);

            this.statDescrptionLabel.isEnabled = Is_Active;
            statDescrptionLabel.gameObject.SetActive(Is_Active);

            this.Accept_Button.isEnabled = Is_Active;
            Accept_Button.gameObject.SetActive(Is_Active);

            this.Name_Panel.isEnabled = Is_Active;
            Name_Panel.gameObject.SetActive(Is_Active);

            this.Left_Button.isEnabled = Is_Active;
            Left_Button.gameObject.SetActive(Is_Active);


            this.Right_Button.isEnabled = Is_Active;
            Right_Button.gameObject.SetActive(Is_Active);


            default_gun_button.df_Button.isEnabled = Is_Active;
            default_gun_button.df_Button.ForceState(Is_Active == false ? dfButton.ButtonState.Disabled : dfButton.ButtonState.Default);
            default_gun_button.gameObject.SetActive(Is_Active);


            this.Close_Button.isEnabled = Is_Active;
            Close_Button.gameObject.SetActive(Is_Active);


            ProcessButtonPages();
            UpdateNameDescLabel();

            if (Is_Active == true)
            {
                Name_Panel.PerformLayout();
                Left_Button.PerformLayout();
                Right_Button.PerformLayout();
                statDescrptionLabel.PerformLayout();
                name_and_Description_Label.PerformLayout();

                default_gun_button.df_Button.PerformLayout();
                Close_Button.PerformLayout();
                Accept_Button.PerformLayout();
                Name_Panel.PerformLayout();

            }

        }
        public void ProcessButtonPage(UpgradeUISelectButtonController self)
        {
            self.UpdateSprites();
            bool b = self.Page == Current_Page ? true : false;
            self.df_Button.isEnabled = b == true ? true : false;
            self.df_Button.gameObject.SetActive(b == true ? Is_Active : false);

        }

        public void ProcessButtonPages()
        {
            foreach (var List in ButtonControllers_Layers)
            {
                List.Player_Using_Alt_Skin = interactor == null ? false : interactor.IsUsingAlternateCostume;
                List.UpdateSprites();
                bool b = List.Page == Current_Page ? true : false;
                List.df_Button.isEnabled = b == true ? Is_Active : false;
                List.df_Button.gameObject.SetActive(b == true ? Is_Active : false);
                List.df_Button.PerformLayout();
            }
            UpdatePageButtons();
        }

        public void UpdatePageButtons()
        {
            Left_Button.Enable();
            Right_Button.Enable();

            if (ButtonControllers_Layers.First().Page == Current_Page)
            { Left_Button.Disable();}

            if (ButtonControllers_Layers.Last().Page == Current_Page)
            {Right_Button.Disable();}
        }
     


        public void UpdatePanels()
        {
            CanBeSelected();
            default_gun_button.UpdateSprites();

            if (Name_Panel)
            {
                bool b = interactor == null ? false : interactor.IsUsingAlternateCostume;
                Name_Panel.backgroundSprite = currentlySelectedButton != null ?
                    
                    currentlySelectedButton.IsUnlocked() == false ? 
                    
                    b == true ? "name_label_locked_alt" : "name_label_locked" : 
                    currentlySelectedButton.ReturnNameLabel() :
                    b == true ? "name_label_empty_alt" : "name_label_empty";

                Name_Panel.Enable();
                Name_Panel.Invalidate();
            }
        }

        public void UpdateNameDescLabel()
        {
            if (currentlySelectedButton == null) 
            { name_and_Description_Label.ModifyLocalizedText(StaticColorHexes.AddColorToLabelString("Tip Of The Day:\n\n", StaticColorHexes.Yellow_Hex) + TipsOfTheDay[UnityEngine.Random.Range(0, TipsOfTheDay.Count)]); return; }

            bool isLocked = currentlySelectedButton.IsUnlocked();
            string newtext = isLocked == true ? currentlySelectedButton.Upgrade_Description : currentlySelectedButton.Unlock_Description;
            name_and_Description_Label.Localize();
            statDescrptionLabel.Localize();
            name_and_Description_Label.ModifyLocalizedText(newtext);

        }

        public void UpdateStatDescLabel()
        {
            if (currentlySelectedButton == null)
            { statDescrptionLabel.ModifyLocalizedText(""); return; }

            if (currentlySelectedButton.IsUnlocked() == false)
            {statDescrptionLabel.ModifyLocalizedText(""); return; }

            if (currentlySelectedButton.upgradeType == UpgradeUISelectButtonController.UpgradeType.Minor_Upgrade | currentlySelectedButton.upgradeType == UpgradeUISelectButtonController.UpgradeType.Major_Upgrade) 
            { statDescrptionLabel.ModifyLocalizedText(""); return; }
            
            string newtext = 
                "Damage : " + currentlySelectedButton.Damage_Description + "\n" +
                "Rate Of Fire : " + currentlySelectedButton.FireRate_Description + "\n" +
                "Reload Time : " + currentlySelectedButton.Reload_Description + "\n" +
                "Clip Size : " + currentlySelectedButton.Clipsize_Description + "\n" +
                "Shot Speed : " + currentlySelectedButton.Shotspeed_Description + "\n" +
                currentlySelectedButton.Additional_Notes;
            name_and_Description_Label.Localize();
            statDescrptionLabel.Localize();
            statDescrptionLabel.ModifyLocalizedText(newtext);
        }



        private static dfAtlas storedAtlas = GameUIRoot.Instance.Manager.DefaultAtlas;

        public void ToggleUI(bool? Force_Set_Value = null, PlayerController player = null, bool Instant = false)
        {
            currentlySelectedButton = null;
            Current_Page = 0;
            if (player != null)
            {
                interactor = player;
            }
            this.Is_Active = Force_Set_Value != null ? Force_Set_Value.Value : !this.Is_Active;
            this.StartCoroutine(DoFade(Is_Active == true? 0 : 1, Is_Active == true ? 1 : 0, 0.3f, Is_Active, Instant));
            UpdatePanels();


            GameUIRoot.Instance.ToggleLowerPanels(!this.Is_Active, false, "Modular_UI");
            GameUIRoot.Instance.ToggleAllDefaultLabels(!this.Is_Active, "Modular_UI");
            if (this.Is_Active)
            {
                if (!GameManager.Instance.MainCameraController.ManualControl)
                {
                    GameManager.Instance.MainCameraController.OverridePosition = GameManager.Instance.MainCameraController.transform.position;
                    GameManager.Instance.MainCameraController.SetManualControl(true, false);
                    this.Camera_Lock = true;
                }
                else
                {
                    this.Camera_Lock = false;
                }
                GameUIRoot.Instance.HideCoreUI("Modular_UI");
                GameUIRoot.Instance.notificationController.ForceHide();
                GameUIRoot.Instance.levelNameUI.BanishLevelNameText();

                GameUIRoot.Instance.ForceClearReload(-1);
            }
            else
            {
                if (GameManager.Instance.MainCameraController && this.Camera_Lock)
                {
                    GameManager.Instance.MainCameraController.SetManualControl(false, true);
                }
                GameUIRoot.Instance.ShowCoreUI("Modular_UI");
                BraveInput.FlushAll();
            }

            Minimap.Instance.TemporarilyPreventMinimap = Is_Active;

            
            if (Is_Active)
            {
                AkSoundEngine.PostEvent("Play_UI_map_open_01", base.gameObject);
            }
            Shader.SetGlobalFloat("_FullMapActive", (float)((!this.Is_Active) ? 0 : 1));
            if (Is_Active)
            {
                Pixelator.Instance.FadeColor = Color.black;
                Pixelator.Instance.fade = 0.3f;
                GameUIRoot.Instance.HideCoreUI(string.Empty);
                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerController playerController = GameManager.Instance.AllPlayers[i];
                    if (playerController)
                    {
                        playerController.CurrentInputState = PlayerInputState.NoInput;
                    }
                }
            }
            else
            {
                Pixelator.Instance.FadeColor = Color.black;
                Pixelator.Instance.fade = 1f;
                GameUIRoot.Instance.ShowCoreUI(string.Empty);
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



        private static Vector2[] preDeterminedOffsets = new Vector2[]
        {
            new Vector2(556, 855),
            new Vector2(748, 855),
            new Vector2(940, 855), //Centre Top
            new Vector2(1132, 855),
            new Vector2(1324, 855),
            new Vector2(556, 645),
            new Vector2(748, 645),
            new Vector2(940, 645), //Centre Bottom
            new Vector2(1132,645),
            new Vector2(1324, 645),
        };
        public int Current_Page = 0;
        public UpgradeUISelectButtonController currentlySelectedButton = null;



        public static int AddNewEntry(StarterGunSelectUIController self, UpgradeUISelectButtonController button)
        {            
            int currentPage = self.ButtonControllers_Layers.Count > 0 ? self.ButtonControllers_Layers.Last().Page : 0;
            int LastEntry = self.ButtonControllers_Layers.Count > 0 ? self.ButtonControllers_Layers.Last().Entry : -1;
            if (LastEntry > 8)
            {
                //ETGModConsole.Log("New Page!");
                currentPage += 1;
                LastEntry = -1;
            }
            button.Page = currentPage;
            button.Entry = LastEntry + 1;
            //ETGModConsole.Log("New Entry!");

            self.ButtonControllers_Layers.Add(button);
            return LastEntry + 1;
        }

        //Does not need AltSkinStringStorage
        public List<UpgradeUISelectButtonController> ButtonControllers_Layers = new List<UpgradeUISelectButtonController>();
        public dfPanel Name_Panel;
        public static GameObject UI_Frame;
        public UpgradeUISelectButtonController default_gun_button;
        public PlayerController interactor;
        public dfLabel statDescrptionLabel;
        public dfLabel name_and_Description_Label;


        //Does need AltSkinStringStorage
        public dfButton Close_Button;
        public dfButton Accept_Button;

        public dfButton Left_Button;
        public dfButton Right_Button;

        private bool Camera_Lock;
        private bool Is_Active = false;

        public static List<string> TipsOfTheDay = new List<string>
        {
            "Higher tier weapons have a\ngreater chance to give higher\ntier modules.",
            "Upgrade your power by\nhovering over the " + StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex) +" label\nin your inventory.\nRemember, it's not free.",
            "Scrap is more useful than\nit may seem!\nRemember to scrap useless\nitems to gain potential benefit\nlater.",
            "Some Modules are more niche\nthan others.\nRemember to think about\nwhich ones you're getting.",
            StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex) +" is a very valuable resource,\nand getting requires lots of Scrap\nto upgrade your reserves.",
            "Nearly all health upgrade items\ngrant extra "+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+", but makes it\nharder to upgrade your Power\nin the future.",
            "Master Rounds are very valuable!\nThey grant extra "+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+" and do NOT\nincrease the cost of\nupgrading your " + StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+"!",
            "Remember that you can switch your\nactive items Mode by pressing\nReload on a full clip.",
            "Certain alternate weapons may do\nbetter with Modules that may\nlooknot very useful at first glance.",
            "Modules need to be powered\nbefore they give any benefit.\nRemember to power them in your\nInventory.",
            "Look out after your " + StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+"!\nHigher tier modules use more" +StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+"\nmake sure to manage it well!",
            "Certain Modules may give some\nbenefit without even powering them,\nbut only to a limited extent.",
            "Time still flows when you\nare in your inventory,\ncheck if the coast is clear\nbeforehand.",
            StaticColorHexes.AddColorToLabelString("FATAL ERROR GETTING TIP\n\nFAIL AT: 17", StaticColorHexes.Red_Color_Hex),
            "Powering duplicate Modules\nuses less power than usual.\nKeep that in mind when choosing\na new module.",
            "Modules of specific tiers use\nspecific amounts of power.\nHowever, some nicher modules\nuse less power\nthan usual.",
            "Variety is the spice of life,\ntry to diversify your builds!",
            "Modular cannot do handstands,\nas they only have one hand and\ncant balance well on it.",
            "You can depower Modules if\nyou want to change up your build.",
            "You can see what a Module does\nby clicking on it in your\ninventory.\nAternatively you can check the\nAmmonomicon."
        };

    }
}
