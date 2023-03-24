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


        public class UpgradeUISelectButtonController : MonoBehaviour
        {
            public void Start()
            {
                df_Button = this.gameObject.GetComponent<dfButton>();
                UpdateSprites();
            }

            public void UpdateSprites()
            {
                bool b = IsUnlocked();

                df_Button.backgroundSprite = b == false ? Player_Using_Alt_Skin == false ? Lock_Label : Lock_Label_Alt : Player_Using_Alt_Skin == false ? Label_Name : Label_Name_Alt ?? Label_Name;
                df_Button.disabledSprite = b == false ? Player_Using_Alt_Skin == false ? Lock_Label : Lock_Label_Alt : Player_Using_Alt_Skin == false ? Label_Name : Label_Name_Alt ?? Label_Name;

                df_Button.hoverSprite = b == false ? Player_Using_Alt_Skin == false ? Lock_Label : Lock_Label_Hover_Alt : Player_Using_Alt_Skin == false ? Label_Hover : Label_Hover_Alt ?? Label_Hover;
                df_Button.focusSprite = b == false ? Player_Using_Alt_Skin == false ? Lock_Label : Lock_Label_Hover_Alt : Player_Using_Alt_Skin == false ? Label_Hover : Label_Hover_Alt ?? Label_Hover;

                df_Button.pressedSprite = b == false ? Player_Using_Alt_Skin == false ? Lock_Label : Lock_Label_Press_Alt : Player_Using_Alt_Skin == false ? Label_Press : Label_Press_Alt ?? Label_Press;

                df_Button.Invalidate();
            }

            public bool IsUnlocked()
            {
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
                return c.ModularGunController.isAlt == false ? GunID : AltGunID != -1 ? AltGunID : GunID;
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

            public string Lock_Label = "Locked_Icon";
            public string Lock_Label_Press = "Locked_Icon_Press";
            public string Lock_Label_Hover = "Locked_Icon_Hover";

            public string Name_Label_Sprite_Name = "Modular_Name_Label_WIP";

            public bool Player_Using_Alt_Skin = false;

            public string Label_Name_Alt = null;//"UI_Button_Close";
            public string Label_Press_Alt = null;//"Default_Gun_Icon_Hightlighted";
            public string Label_Hover_Alt = null;//"Default_Gun_Icon_Pressed";

            public string Lock_Label_Alt = "Locked_Icon_Alt";
            public string Lock_Label_Press_Alt = "Locked_Icon_Press_Alt";
            public string Lock_Label_Hover_Alt = "Locked_Icon_Hover_Alt";

            public string Name_Label_Sprite_Name_Alt = "Modular_Name_Label_WIP_Alt";



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

            public int GunID = DefaultArmCannon.DefaultArmCannonID;
            public int AltGunID = DefaultArmCannonAlt.DefaultArmCannonAltID;

            public dfButton df_Button;

            public int Page = 0;
            public int Entry = 0;
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
            dfAtlas defaultAtlas = SaveTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>().Manager.DefaultAtlas;
            dfFontBase defaultFont = SaveTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>().Manager.DefaultFont;

            var Bundle = Module.ModularAssetBundle;
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked"), "Locked_Icon");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_highlighted"), "Locked_Icon_Hover");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_pressed"), "Locked_Icon_Press");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_alt"), "Locked_Icon_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_highlighted_alt"), "Locked_Icon_Hover_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_locked_pressed_alt"), "Locked_Icon_Press_Alt");



            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_template_bg"), "template_UI_background");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_template_bg_alt"), "template_UI_background_alt");

            StarterGunSelectUIController.UI_Frame = PrefabBuilder.BuildObject("Frame_Modular_UI");
            StarterGunSelectUIController.UI_Frame.layer = 24;

            var storage = UI_Frame.AddComponent<AltSkinStringStorage>();
            storage.df_label_string = "template_UI_background_alt";

            dfPanel dfPanel = StarterGunSelectUIController.UI_Frame.AddComponent<dfPanel>();
            dfPanel.anchorStyle = dfAnchorStyle.All;
            dfPanel.isEnabled = true;
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
            dfPanel.tabIndex = -1;
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
            dfPanel.isLocalized = false;
            dfPanel.hotZoneScale = Vector2.one;
            dfPanel.allowSignalEvents = true;
            dfPanel.PrecludeUpdateCycle = false;
            dfPanel.atlas = defaultAtlas;
            dfPanel.backgroundSprite = "template_UI_background";
            dfPanel.backgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            dfPanel.padding = new RectOffset(0, 0, 0, 0);
            


            StarterGunSelectUIController gunSelectUIController = StarterGunSelectUIController.UI_Frame.AddComponent<StarterGunSelectUIController>();


            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close"), "UI_Button_Close");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_highlighted"), "UI_Button_Close_Highlighted");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_pressed"), "UI_Button_Close_Pressed");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_alt"), "UI_Button_Close_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_highlighted_alt"), "UI_Button_Close_Highlighted_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_close_pressed_alt"), "UI_Button_Close_Pressed_Alt");

            GameObject closeButton_object = PrefabBuilder.BuildObject("CloseButton");
            closeButton_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
            closeButton_object.layer = 24;
            dfButton closeButton = closeButton_object.CreateBlankDfButton(new Vector2(160f, 160f), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
            {
                bottom = 1275,
                left = 530,
                right = 0f,
                top = 0f
            });
            closeButton.AssignDefaultPresets(defaultAtlas, defaultFont);
            closeButton.backgroundSprite = "UI_Button_Close";
            closeButton.hoverSprite = "UI_Button_Close_Highlighted";
            closeButton.disabledSprite = "UI_Button_Close_Pressed";
            closeButton.focusSprite = "UI_Button_Close";
            closeButton.pressedSprite = "UI_Button_Close_Pressed";

            var storage_closeButton = closeButton.gameObject.AddComponent<AltSkinStringStorage>();
            storage_closeButton.df_button_default_string = "UI_Button_Close_Alt";
            storage_closeButton.df_button_inactive_string = "UI_Button_Close_Alt";
            storage_closeButton.df_button_pressed_string = "UI_Button_Close_Pressed_Alt";
            storage_closeButton.df_button_highlighted_string = "UI_Button_Close_Highlighted_Alt";

            gunSelectUIController.Close_Button = closeButton;



            
            {
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun"), "Default_Gun_Icon");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_highlighted"), "Default_Gun_Icon_Hightlighted");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_pressed"), "Default_Gun_Icon_Pressed");

                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_alt"), "Default_Gun_Icon_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_highlighted_alt"), "Default_Gun_Icon_Hightlighted_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_default_gun_pressed_alt"), "Default_Gun_Icon_Pressed_Alt");

                GameObject Default_Gun_Button_object = PrefabBuilder.BuildObject("Default_Gun_Button");
                Default_Gun_Button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                Default_Gun_Button_object.layer = 24;
                dfButton Default_Gun_Button = Default_Gun_Button_object.CreateBlankDfButton(new Vector2(160, 160), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 1250,
                    left = 940f,
                    right = 0,
                    top = 0
                });
                Default_Gun_Button.AssignDefaultPresets(defaultAtlas, defaultFont);
                Default_Gun_Button.backgroundSprite = "Default_Gun_Icon";
                Default_Gun_Button.hoverSprite = "Default_Gun_Icon_Hightlighted";
                Default_Gun_Button.disabledSprite = "Default_Gun_Icon";
                Default_Gun_Button.focusSprite = "Default_Gun_Icon_Hightlighted";
                Default_Gun_Button.pressedSprite = "Default_Gun_Icon_Pressed";
                Default_Gun_Button.RelativePosition = new Vector3(0, 4);

                var upgrade_button_default_gun = Default_Gun_Button_object.AddComponent<UpgradeUISelectButtonController>();
                upgrade_button_default_gun.df_Button = Default_Gun_Button;
                upgrade_button_default_gun.FlagToCheck = CustomDungeonFlags.NOLLA;
                upgrade_button_default_gun.upgradeType = UpgradeUISelectButtonController.UpgradeType.Default_Gun;
                upgrade_button_default_gun.Upgrade_Description = "Simple, yet powerful weapon.\nFavoured by most Modular machines.";

                upgrade_button_default_gun.Label_Name = "Default_Gun_Icon";
                upgrade_button_default_gun.Label_Press = "Default_Gun_Icon_Pressed";
                upgrade_button_default_gun.Label_Hover = "Default_Gun_Icon_Hightlighted";

                upgrade_button_default_gun.Label_Name_Alt = "Default_Gun_Icon_Alt";
                upgrade_button_default_gun.Label_Press_Alt = "Default_Gun_Icon_Pressed_Alt";
                upgrade_button_default_gun.Label_Hover_Alt = "Default_Gun_Icon_Hightlighted_Alt";


                upgrade_button_default_gun.Name_Label_Sprite_Name = defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_defualtcannon"), "Default_Arm_Cannon_Text").name;
                upgrade_button_default_gun.Name_Label_Sprite_Name_Alt = defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_defualtcannon_alt"), "Default_Arm_Cannon_Text_Alt").name;

                gunSelectUIController.default_gun_button = upgrade_button_default_gun;

            }

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_doublehalf"), "DB_H_Default");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_doublehalf_highlighted"), "DB_H_Default_HightLighted");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_doublehalf_pressed"), "DB_H_Default_Pressed");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun"), "Peashooter_Gun_Icon");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_highlighted"), "Peashooter_Gun_Icon_Hightlighted");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_pressed"), "Peashooter_Gun_Icon_Pressed");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_alt"), "Peashooter_Gun_Icon_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_highlighted_alt"), "Peashooter_Gun_Icon_Hightlighted_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_peashooter_gun_pressed_alt"), "Peashooter_Icon_Pressed_Alt");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_peashooter"), "Modular_Name_Label_PeaShooter");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_peashooter_alt"), "Modular_Name_Label_PeaShooter_Alt");

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "PeaShooter_Button"
                   , "Peashooter_Gun_Icon" //asset name default
                   , "Peashooter_Gun_Icon_Hightlighted" //asset name highlighted
                   , "Peashooter_Gun_Icon_Pressed" //asset name pressed
                   , CustomDungeonFlags.NOLLA //Unlock flag, Set itt to NOLLA for no unlock condition
                   , ModularPeaShooter.GunID //Gun ID
                   , "Weaker, but allows for extra power." //Default description
                   , StaticColorHexes.AddColorToLabelString("Low", StaticColorHexes.Orange_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Yellow_Hex) //Clipsize Secription
                   , null //Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Fast", StaticColorHexes.Light_Green_Hex) //Shot Speed Secription
                   , "Grants the user 2 additional." + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER)  //Additional Notes, keep at one line
                   , "Modular_Name_Label_PeaShooter" //Label Name Asset Name
                   , ModularPeaShooterAlt.GunID //alt gun ID
                   , "Peashooter_Gun_Icon_Alt" //asset name default alt
                   , "Peashooter_Gun_Icon_Hightlighted_Alt" //asset name highlighted alt
                   , "Peashooter_Icon_Pressed_Alt" //asset name pressed alt
                   , "Modular_Name_Label_PeaShooter_Alt"  //Label Name Asset Name Alt
                   ); //sprite name for the Big Name Label

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun"), "scattercannon_Gun_Icon");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_highlighted"), "scattercannon_Gun_Icon_Hightlighted");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_pressed"), "scattercannon_Gun_Icon_Pressed");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_alt"), "scattercannon_Gun_Icon_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_highlighted_alt"), "scattercannon_Gun_Icon_Hightlighted_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_scattercannon_gun_pressed_alt"), "scattercannon_Icon_Pressed_Alt");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_scattercannon"), "Modular_Name_Label_scattercannon");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_scattercannon_alt"), "Modular_Name_Label_scattercannon_Alt");

            StarterGunSelectUIController.GenerateNewGunButton(defaultAtlas, defaultFont, gunSelectUIController,
                   "ScatterCannon_Button"
                   , "scattercannon_Gun_Icon" //asset name default
                   , "scattercannon_Gun_Icon_Hightlighted" //asset name highlighted
                   , "scattercannon_Gun_Icon_Pressed" //asset name pressed
                   , CustomDungeonFlags.NOLLA //Unlock flag, Set itt to NOLLA for no unlock condition
                   , ScatterBlast.ID //Gun ID
                   , "A basic scatter cannon.\nFires 6 pellets with one shot." //Default description
                   , StaticColorHexes.AddColorToLabelString("High", StaticColorHexes.Orange_Hex)  //Damage Secription
                   , StaticColorHexes.AddColorToLabelString("Slower Than Average", StaticColorHexes.Yellow_Hex) //Reload Secription
                   , StaticColorHexes.AddColorToLabelString("Lower Than Average", StaticColorHexes.Yellow_Hex) //Clipsize Secription
                   , StaticColorHexes.AddColorToLabelString("Low", StaticColorHexes.Orange_Hex) //Fire rate Secription
                   , StaticColorHexes.AddColorToLabelString("Average", StaticColorHexes.Default_UI_Color_Hex) //Shot Speed Secription
                   , "Has high spread, and lower range."  //Additional Notes, keep at one line
                   , "Modular_Name_Label_scattercannon" //Label Name Asset Name
                   , ScatterBlastAlt.ID //alt gun ID
                   , "scattercannon_Gun_Icon_Alt" //asset name default alt
                   , "scattercannon_Gun_Icon_Hightlighted_Alt" //asset name highlighted alt
                   , "scattercannon_Icon_Pressed_Alt" //asset name pressed alt
                   , "Modular_Name_Label_scattercannon_Alt"  //Label Name Asset Name Alt
                   ); //sprite name for the Big Name Label

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
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active"), "Mod_Accept");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_1"), "Mod_Accept_Inactive");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_press"), "Mod_Accept_Pressed");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_highlight"), "Mod_Accept_Highlight");

                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_alt"), "Mod_Accept_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_1_alt"), "Mod_Accept_Inactive_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_active_press_alt"), "Mod_Accept_Pressed_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("ui_button_accept_highlight_alt"), "Mod_Accept_Highlight_Alt");

                GameObject accept_Button_object = PrefabBuilder.BuildObject("AcceptButton");
                accept_Button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                accept_Button_object.layer = 24;
                dfButton acceptButton_df = accept_Button_object.CreateBlankDfButton(new Vector2(640f, 160f), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 550,
                    left = 1680,
                    right = 0f,
                    top = 0f
                });
                acceptButton_df.AssignDefaultPresets(defaultAtlas, defaultFont);
                acceptButton_df.backgroundSprite = "Mod_Accept";
                acceptButton_df.hoverSprite = "Mod_Accept_Highlight";
                acceptButton_df.disabledSprite = "Mod_Accept_Inactive";
                acceptButton_df.focusSprite = "Mod_Accept_Highlight";
                acceptButton_df.pressedSprite = "Mod_Accept_Pressed";

                var storage_acceptButton = acceptButton_df.gameObject.AddComponent<AltSkinStringStorage>();
                storage_acceptButton.df_button_default_string = "Mod_Accept_Alt";
                storage_acceptButton.df_button_inactive_string = "Mod_Accept_Inactive_Alt";
                storage_acceptButton.df_button_highlighted_string = "Mod_Accept_Highlight_Alt";
                storage_acceptButton.df_button_pressed_string = "Mod_Accept_Pressed_Alt";

                gunSelectUIController.Accept_Button = acceptButton_df;
            }

            {
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_active"), "Left_Button_Active");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_inactive"), "Left_Button_Inactive");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_highlighted"), "Left_Button_Highlight");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_pressed"), "Left_Button_Pressed");

                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_active_alt"), "Left_Button_Active_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_inactive_alt"), "Left_Button_Inactive_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_highlighted_alt"), "Left_Button_Highlight_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("left_button_pressed_alt"), "Left_Button_Pressed_Alt");


                GameObject left_button_object = PrefabBuilder.BuildObject("Left_Page_Button");
                left_button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                left_button_object.layer = 24;
                dfButton left_button = left_button_object.CreateBlankDfButton(new Vector2(80, 80f), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 500,
                    left = 748,
                    right = 0f,
                    top = 0f
                });
                left_button.AssignDefaultPresets(defaultAtlas, defaultFont);
                left_button.backgroundSprite = "Left_Button_Active";
                left_button.hoverSprite = "Left_Button_Highlight";
                left_button.disabledSprite = "Left_Button_Inactive";
                left_button.focusSprite = "Left_Button_Highlight";
                left_button.pressedSprite = "Left_Button_Pressed";

                var storage_left_button = left_button.gameObject.AddComponent<AltSkinStringStorage>();
                storage_left_button.df_button_default_string = "Left_Button_Active_Alt";
                storage_left_button.df_button_inactive_string = "Left_Button_Inactive_Alt";
                storage_left_button.df_button_highlighted_string = "Left_Button_Highlight_Alt";
                storage_left_button.df_button_pressed_string = "Left_Button_Pressed_Alt";

                gunSelectUIController.Left_Button = left_button;
            }

            {
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_active"), "Right_Button_Active");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_inactive"), "Right_Button_Inactive");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_highlighted"), "Right_Button_Highlight");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_pressed"), "Right_Button_Pressed");

                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_active_alt"), "Right_Button_Active_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_inactive_alt"), "Right_Button_Inactive_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_highlighted_alt"), "Right_Button_Highlight_Alt");
                defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("right_button_pressed_alt"), "Right_Button_Pressed_Alt");

                GameObject right_button_object = PrefabBuilder.BuildObject("Right_Page_Button");
                right_button_object.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
                right_button_object.layer = 24;
                dfButton right_button = right_button_object.CreateBlankDfButton(new Vector2(80, 80f), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
                {
                    bottom = 500,
                    left = 1180,
                    right = 0f,
                    top = 0f
                });
                right_button.AssignDefaultPresets(defaultAtlas, defaultFont);
                right_button.backgroundSprite = "Right_Button_Active";
                right_button.hoverSprite = "Right_Button_Highlight";
                right_button.disabledSprite = "Right_Button_Inactive";
                right_button.focusSprite = "Right_Button_Highlight";
                right_button.pressedSprite = "Right_Button_Pressed";

                var storage_right_button = right_button.gameObject.AddComponent<AltSkinStringStorage>();
                storage_right_button.df_button_default_string = "Right_Button_Active_Alt";
                storage_right_button.df_button_inactive_string = "Right_Button_Inactive_Alt";
                storage_right_button.df_button_highlighted_string = "Right_Button_Highlight_Alt";
                storage_right_button.df_button_pressed_string = "Right_Button_Pressed_Alt";

                gunSelectUIController.Right_Button = right_button;
            }

            dfLabel StatDescriptionLabel = PrefabBuilder.BuildObject("StatDescriptionLabel_Object").AddComponent<dfLabel>();
            StatDescriptionLabel.gameObject.transform.parent = StarterGunSelectUIController.UI_Frame.transform;
            StatDescriptionLabel.AssignDefaultPresets(defaultAtlas, defaultFont);
            StatDescriptionLabel.anchorStyle = (dfAnchorStyle.Bottom | dfAnchorStyle.Left);
            StatDescriptionLabel.color = new Color32(155, 235, 199, 255);
            StatDescriptionLabel.shadow = true;
            StatDescriptionLabel.shadowOffset = new Vector2(1, -0.5f);
            StatDescriptionLabel.ShadowColor = new Color32(1, 1, 1, 255);
            StatDescriptionLabel.textScale *= 0.95f;

            StatDescriptionLabel.layout = new dfControl.AnchorLayout(dfAnchorStyle.Bottom | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 20f,
                    left = 2670f,
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
            NameDescriptionLabel.AssignDefaultPresets(defaultAtlas, defaultFont);
            NameDescriptionLabel.anchorStyle = (dfAnchorStyle.Bottom | dfAnchorStyle.Left);
            NameDescriptionLabel.color = new Color32(155, 235, 199, 255);
            NameDescriptionLabel.textScale *= 1.15f;
            NameDescriptionLabel.shadow = true;
            NameDescriptionLabel.shadowOffset = new Vector2(1, -0.5f);
            NameDescriptionLabel.ShadowColor = new Color32(1, 1, 1, 255);

            NameDescriptionLabel.layout = new dfControl.AnchorLayout(dfAnchorStyle.Bottom | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 140f,
                    left = 2670f,
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
            name_display_Object.layer = 24;



            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_empty"), "Modular_Name_Label_None");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_locked"), "Modular_Name_Label_Locked");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_WIP"), "Modular_Name_Label_WIP");

            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_empty_alt"), "Modular_Name_Label_None_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_locked_alt"), "Modular_Name_Label_Locked_Alt");
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("name_label_WIP_alt"), "Modular_Name_Label_WIP_Alt");


            dfPanel name_display_Object_panel = name_display_Object.AddComponent<dfPanel>();
            name_display_Object_panel.AssignDefaultPresets(defaultAtlas, new Vector2(680.5f, 222.5f), dfAnchorStyle.Bottom | dfAnchorStyle.Left);
            //name_display_Object_panel.gameObject.transform.localScale /= 2;

            name_display_Object_panel.layout = new dfControl.AnchorLayout(dfAnchorStyle.Bottom | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 1195,
                    left = 1655,
                    right = 0f,
                    top = 0f
                },
                owner = name_display_Object_panel
            };
            name_display_Object_panel.backgroundSprite = "Modular_Name_Label_None";
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
            string Label_Name_Asset_Name = "Modular_Name_Label_WIP",
            int Alt_GunID = -1,
             string asset_name_default_alt = null,
             string asset_name_highlighted_alt = null,
             string asset_name_pressed_alt = null,
             string Label_Name_Asset_Name_Alt = "Modular_Name_Label_WIP_Alt"
            )
        {
            GameObject Default_Gun_Button_object = PrefabBuilder.BuildObject(Button_Name+"_Object");
            Default_Gun_Button_object.transform.parent = StarterGunSelectUIController.UI_Frame.GetComponent<dfPanel>().transform;
            Default_Gun_Button_object.layer = 24;

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



            int integer = AddNewEntry(self, upgrade_button_default_gun);

            dfButton Default_Gun_Button = Default_Gun_Button_object.CreateBlankDfButton(new Vector2(160, 160), dfAnchorStyle.Bottom | dfAnchorStyle.Left, new dfAnchorMargins
            {
                bottom = preDeterminedOffsets[integer].y,
                left = preDeterminedOffsets[integer].x,
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


            Default_Gun_Button.RelativePosition = new Vector3(4, 0);
        }


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
            panel_df.backgroundSprite = altSkin == false ? "template_UI_background" : "template_UI_background_alt";
            panel_df.Invalidate();

            var fuck_1 = Close_Button.GetComponent<AltSkinStringStorage>();
            Close_Button.backgroundSprite = altSkin == false ? "UI_Button_Close" : fuck_1.df_button_default_string != null ? fuck_1.df_button_default_string : "UI_Button_Close";//"UI_Button_Close";
            Close_Button.focusSprite = altSkin == false ? "UI_Button_Close" : fuck_1.df_button_default_string != null ? fuck_1.df_button_default_string : "UI_Button_Close";//"UI_Button_Close";
            Close_Button.hoverSprite = altSkin == false ? "UI_Button_Close_Highlighted" : fuck_1.df_button_highlighted_string != null ? fuck_1.df_button_highlighted_string : "UI_Button_Close_Highlighted";//"UI_Button_Close_Highlighted";
            Close_Button.disabledSprite = altSkin == false ? "UI_Button_Close_Pressed" : fuck_1.df_button_pressed_string != null ? fuck_1.df_button_pressed_string : "UI_Button_Close_Pressed";//"UI_Button_Close_Pressed";
            Close_Button.pressedSprite = altSkin == false ? "UI_Button_Close_Pressed" : fuck_1.df_button_pressed_string != null ? fuck_1.df_button_pressed_string : "UI_Button_Close_Pressed";// "UI_Button_Close_Pressed";
            Close_Button.Invalidate();


            var fuck_2 = Accept_Button.GetComponent<AltSkinStringStorage>();
            Accept_Button.backgroundSprite = altSkin == false ? "Mod_Accept" : fuck_2.df_button_default_string != null ? fuck_2.df_button_default_string : "Mod_Accept";
            Accept_Button.focusSprite = altSkin == false ? "Mod_Accept_Highlight" : fuck_2.df_button_default_string != null ? fuck_2.df_button_default_string : "Mod_Accept_Highlight";
            Accept_Button.hoverSprite = altSkin == false ? "Mod_Accept_Highlight" : fuck_2.df_button_highlighted_string != null ? fuck_2.df_button_highlighted_string : "Mod_Accept_Highlight";;
            Accept_Button.disabledSprite = altSkin == false ? "Mod_Accept_Inactive" : fuck_2.df_button_inactive_string != null ? fuck_2.df_button_inactive_string : "Mod_Accept_Inactive";
            Accept_Button.pressedSprite = altSkin == false ? "Mod_Accept_Pressed" : fuck_2.df_button_pressed_string != null ? fuck_2.df_button_pressed_string : "Mod_Accept_Pressed";
            Accept_Button.Invalidate();

            var fuck_3 = Left_Button.GetComponent<AltSkinStringStorage>();
            Left_Button.backgroundSprite = altSkin == false ? "Left_Button_Active" : fuck_3.df_button_default_string != null ? fuck_3.df_button_default_string : "Left_Button_Active";
            Left_Button.focusSprite = altSkin == false ? "Left_Button_Highlight" : fuck_3.df_button_default_string != null ? fuck_3.df_button_default_string : "Left_Button_Highlight";
            Left_Button.hoverSprite = altSkin == false ? "Left_Button_Highlight" : fuck_3.df_button_highlighted_string != null ? fuck_3.df_button_highlighted_string : "Left_Button_Highlight"; ;
            Left_Button.disabledSprite = altSkin == false ? "Left_Button_Inactive" : fuck_3.df_button_inactive_string != null ? fuck_3.df_button_inactive_string : "Left_Button_Inactive";
            Left_Button.pressedSprite = altSkin == false ? "Left_Button_Pressed" : fuck_3.df_button_pressed_string != null ? fuck_3.df_button_pressed_string : "Left_Button_Pressed";
            Left_Button.Invalidate();

            var fuck_4 = Right_Button.GetComponent<AltSkinStringStorage>();
            Right_Button.backgroundSprite = altSkin == false ? "Right_Button_Active" : fuck_4.df_button_default_string != null ? fuck_4.df_button_default_string : "Right_Button_Active";
            Right_Button.focusSprite = altSkin == false ? "Right_Button_Highlight" : fuck_4.df_button_default_string != null ? fuck_4.df_button_default_string : "Right_Button_Highlight";
            Right_Button.hoverSprite = altSkin == false ? "Right_Button_Highlight" : fuck_4.df_button_highlighted_string != null ? fuck_4.df_button_highlighted_string : "Right_Button_Highlight"; ;
            Right_Button.disabledSprite = altSkin == false ? "Right_Button_Inactive" : fuck_4.df_button_inactive_string != null ? fuck_4.df_button_inactive_string : "Right_Button_Inactive";
            Right_Button.pressedSprite = altSkin == false ? "Right_Button_Pressed" : fuck_4.df_button_pressed_string != null ? fuck_4.df_button_pressed_string : "Right_Button_Pressed";
            Right_Button.Invalidate();

            foreach (var List in ButtonControllers_Layers)
            {
                List.Player_Using_Alt_Skin = altSkin;
                List.UpdateSprites();
            }
            default_gun_button.Player_Using_Alt_Skin = altSkin;
            default_gun_button.UpdateSprites();
        }



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
                    OnUse(interactor, currentlySelectedButton.ReturnGun(p));
                    Inst.ToggleUI(false);
                }
            };

            this.Close_Button.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                Inst.ToggleUI(false);
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
            };
            Right_Button.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
            {
                Current_Page += 1;
                UpdatePageButtons();
                foreach (var v in ButtonControllers_Layers)
                {
                    Right_Button.StartCoroutine(this.ButtonFade(v ,0.3f));
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


        public void DoActivation()
        {
            DoAltSkinChecks();

            var bg = StarterGunSelectUIController.Inst.GetComponent<dfPanel>();
            bg.IsVisible = this.Is_Active;
            bg.IsInteractive = this.Is_Active;
            bg.IsEnabled = this.Is_Active;

            this.name_and_Description_Label.isEnabled = Is_Active;
            this.statDescrptionLabel.isEnabled = Is_Active;
            this.Name_Panel.isEnabled = Is_Active;

            this.Left_Button.isEnabled = Is_Active;
            this.Right_Button.isEnabled = Is_Active;


            default_gun_button.df_Button.isEnabled = Is_Active;
            default_gun_button.df_Button.ForceState(Is_Active == false ? dfButton.ButtonState.Disabled : dfButton.ButtonState.Default);

            ProcessButtonPages();

        }
        public void ProcessButtonPage(UpgradeUISelectButtonController self)
        {
            self.UpdateSprites();
            bool b = self.Page == Current_Page ? true : false;
            self.df_Button.isEnabled = b == true ? Is_Active : false;
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
                    
                    b == true ? "Modular_Name_Label_Locked_Alt" : "Modular_Name_Label_Locked" : 
                    currentlySelectedButton.ReturnNameLabel() :
                    b == true ? "Modular_Name_Label_None_Alt" : "Modular_Name_Label_None";

                Name_Panel.Enable();
                Name_Panel.Invalidate();
            }
        }

        public void UpdateNameDescLabel()
        {
            if (currentlySelectedButton == null) 
            { name_and_Description_Label.ModifyLocalizedText(""); return; }

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

    }
}
