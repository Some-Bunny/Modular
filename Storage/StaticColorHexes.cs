using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModularMod
{
    public static class StaticColorHexes
    {
        public static string AddColorToLabelString(string text, string hexValue = "ff8888")
        {
            return "[color #" + hexValue + "]" + text + "[/color]";
        }

        public static string AddColorToLabelStringAlt(string text, string name = "red")
        {
            return "[color " + name + "] (" + text + ")[/color]";
        }
        public static string Red_Color_Hex = "ff0000";
        public static string Light_Blue_Color_Hex = "a39feb";
        public static string Blue_Color_Hex = "5b6ee1";

        public static string Orange_Hex = "df7126";
        public static string Lime_Green_Hex = "7dff00";

        public static string Dark_Red_Hex = "a70000";
        public static string Light_Orange_Hex = "f7a403";
        public static string White_Hex = "ffffff";
        public static string Light_Green_Hex = "d3ff42";
        public static string Green_Hex = "00ff23";
        public static string Yellow_Hex = "ffcd09";

        public static string Default_UI_Color_Hex = "9bebc7";

        public static string Purple_Hex = "9c59d1";
        public static string Black_Hex = "000000";
        public static string Pink_Hex = "f5a8b8";

    }
}
