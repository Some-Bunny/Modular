using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class SpecialCharactersController
    {
       
        public enum SpecialCharacters
        {
            DAMAGE,
            ACCURACY,
            RANGE,
            BULLET_SIZE,
            CLIP_CAPACITY,
            FIRE_RATE,
            RELOAD,
            ENCASED_ROUND,
            HEART,
            GRAVE,
            HOLY_CHAMBER,
            RADAR,
            WIFI,
            SHOTSPEED,
            FIRE,
            BOMB,
            ELECTRICITY,
            SNAIL,
            RICOCHET,
            SNOWFLAKE,
            PIERCE,
            TWO_BULLETS,
            DEATH,
            LIGHT_BULB,
            SCATTERSHOT,
            MAGNET,
            CLAM,
            RAGE,
            WEAK,
            ARROW,

            UP, DOWN, LEFT, RIGHT, CLOSE, GOOGLY, POWER, CLOCK, INFO, T1, T2, T3, T4, T_UNIQUE,
            
        }

        public static void Init()
        {
            InitSpecialCharacterContainer(SpecialCharacters.DAMAGE, "lync_icon_001");//, "lync_icon_red_001", "lync_icon_yellow_001", "lync_icon_green_001", "lync_icon_blue_001");
            InitSpecialCharacterContainer(SpecialCharacters.ACCURACY, "lync_icon_002");//, "lync_icon_red_002", "lync_icon_yellow_002", "lync_icon_green_002", "lync_icon_blue_002");
            InitSpecialCharacterContainer(SpecialCharacters.RANGE, "lync_icon_003");//, "lync_icon_red_003", "lync_icon_yellow_003", "lync_icon_green_003", "lync_icon_blue_003");
            InitSpecialCharacterContainer(SpecialCharacters.BULLET_SIZE, "lync_icon_004");//, "lync_icon_red_004", "lync_icon_yellow_004", "lync_icon_green_004", "lync_icon_blue_004");
            InitSpecialCharacterContainer(SpecialCharacters.CLIP_CAPACITY, "lync_icon_005");//, "lync_icon_red_005", "lync_icon_yellow_005", "lync_icon_green_005", "lync_icon_blue_005");
            InitSpecialCharacterContainer(SpecialCharacters.FIRE_RATE, "lync_icon_006");//, "lync_icon_red_006", "lync_icon_yellow_006", "lync_icon_green_006", "lync_icon_blue_006");
            InitSpecialCharacterContainer(SpecialCharacters.RELOAD, "lync_icon_007");//, null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.SHOTSPEED, "lync_icon_014");
            InitSpecialCharacterContainer(SpecialCharacters.HEART, "lync_icon_009");
            InitSpecialCharacterContainer(SpecialCharacters.ARROW, "lync_icon_030");

            InitSpecialCharacterContainer(SpecialCharacters.UP, "ButtonUp", "ButtonUpBright");
            InitSpecialCharacterContainer(SpecialCharacters.DOWN, "ButtonDown", "ButtonDownBright");
            InitSpecialCharacterContainer(SpecialCharacters.LEFT, "ButtonLeft", "ButtonLeftBright");
            InitSpecialCharacterContainer(SpecialCharacters.RIGHT, "ButtonRight", "ButtonRightBright");

            InitSpecialCharacterContainer(SpecialCharacters.CLOSE, "Cancel", "CanceBrightl");

            InitSpecialCharacterContainer(SpecialCharacters.POWER, "Power");

            InitSpecialCharacterContainer(SpecialCharacters.GOOGLY, "GooglyMoogly", "GooglyMooglyBright");

            InitSpecialCharacterContainer(SpecialCharacters.CLOCK, "Clock");

            InitSpecialCharacterContainer(SpecialCharacters.T1, "tier_label_1_d", "tier_label_1");
            InitSpecialCharacterContainer(SpecialCharacters.T2, "tier_label_2_d", "tier_label_2");
            InitSpecialCharacterContainer(SpecialCharacters.T3, "tier_label_3_d", "tier_label_3");
            InitSpecialCharacterContainer(SpecialCharacters.T4, "tier_label_4_d", "tier_label_4");
            InitSpecialCharacterContainer(SpecialCharacters.T_UNIQUE, "tier_label_unique_d", "tier_label_unique");

            InitSpecialCharacterContainer(SpecialCharacters.INFO, "Info", "Info_Hover");


            /*
            InitSpecialCharacterContainer(SpecialCharacters.ENCASED_ROUND, "lync_icon_008", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.GRAVE, "lync_icon_010", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.HOLY_CHAMBER, "lync_icon_011", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.RADAR, "lync_icon_012", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.WIFI, "lync_icon_013", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.FIRE, "lync_icon_015", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.BOMB, "lync_icon_016", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.ELECTRICITY, "lync_icon_017", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.SNAIL, "lync_icon_018", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.RICOCHET, "lync_icon_019", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.SNOWFLAKE, "lync_icon_020", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.PIERCE, "lync_icon_021", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.TWO_BULLETS, "lync_icon_022", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.DEATH, "lync_icon_023", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.LIGHT_BULB, "lync_icon_024", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.SCATTERSHOT, "lync_icon_025", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.MAGNET, "lync_icon_026", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.CLAM, "lync_icon_027", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.RAGE, "lync_icon_028", null, null, null, null);
            InitSpecialCharacterContainer(SpecialCharacters.WEAK, "lync_icon_029", null, null, null, null);
            */

        }



        public static void InitSpecialCharacterContainer(SpecialCharacters Character, string defaultCharacter, string brightVariant = null)
        {
            var container = new SpecialCharacterContainer();
            container.defaultCharacter = Character;
            container.DefaultVariant = defaultCharacter;//AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>(defaultCharacter), defaultCharacter + "_MDLR");
            if (brightVariant != null)
            {
                container.BrightVariant = brightVariant;//AtlasEditors.AddUITextImage(Module.ModularAssetBundle.LoadAsset<Texture2D>(brightVariant), brightVariant + "_MDLR");
            }
            SpecialCharacterList.Add(container);
        }


        public static string ReturnSpecialCharacter(SpecialCharacters specialCharacter, bool IsBright = false)
        {
            var List = SpecialCharacterList.Where(self => self.defaultCharacter == specialCharacter);
            if (List.Count() > 0) 
            {
                return List.First().GetColor(SpecialCharacterContainer.Color.NONE, IsBright);
            }
            Debug.Log("Failed To Get Character: |" + specialCharacter);
            return null;
        }

        public static List<SpecialCharacterContainer> SpecialCharacterList = new List<SpecialCharacterContainer>();

        public class SpecialCharacterContainer
        {
            public SpecialCharacters defaultCharacter;
            public string GetColor(Color c, bool IsBright)
            {
                if (IsBright == true)
                {
                    return BrightVariant != null ? "[sprite " + BrightVariant + "]" : "[sprite " + DefaultVariant + "]";

                }
                switch (c)
                {
                    case Color.NONE:
                        return "[sprite " + DefaultVariant + "]";
                    case Color.RED:
                        return RedVariant != null ? "[sprite " + RedVariant + "]" : "[sprite " + DefaultVariant + "]";
                    case Color.YELLOW:
                        return YellowVariant != null ? "[sprite " + YellowVariant + "]" : "[sprite " + DefaultVariant + "]";
                    case Color.GREEN:
                        return GreenVariant != null ? "[sprite " + GreenVariant + "]" : "[sprite " + DefaultVariant + "]";
                    case Color.BLUE:
                        return BlueVariant != null ? "[sprite " + BlueVariant + "]" : "[sprite " + DefaultVariant + "]";
                    default:
                        return "[sprite " + DefaultVariant + "]";
                }
            }
            public string DefaultVariant;
            public string RedVariant;
            public string YellowVariant;
            public string GreenVariant;
            public string BlueVariant;
            public string BrightVariant;

            public enum Color
            {
                NONE,
                GREEN,
                YELLOW,
                RED,
                BLUE
            }
        }
    }
}
