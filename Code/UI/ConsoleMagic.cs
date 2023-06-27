using SGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace ModularMod
{
    public static class ConsoleMagic
    {
        public static string FormatWithSpaces(this string s)
        {
            return Regex.Replace(s, "([a-z])([A-Z])", "$1 $2");
        }
        public static string AddColorToLabelString(string text, string hexValue = "ff8888")
        {
            return "[color #" + hexValue + "]" + text + "[/color]";
        }


        public static void LogButCool(string text, Texture texture)
        {
            var container = new SGroup() { Size = new Vector2(20000, 32), AutoLayoutPadding = 0 };

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (c == ' ')
                {
                    container.Children.Add(new SRect(Color.clear) { Size = Vector2.one * 10 });
                }
                else
                {
                    var hue = Mathf.InverseLerp(0, text.Length, i);
                    var col = Color.HSVToRGB(hue, 1, 1);
                    var label = new SLabel(c.ToString()) { Foreground = col, With = { new ShakyWobbly() } };
                    container.Children.Add(label);
                }
            }
            container.AutoLayout = (SGroup g) => new Action<int, SElement>(g.AutoLayoutHorizontal);
            container.Children.Add(new SLabel()
            {
                Icon = texture,
                With = { new ShakyWobblyNoColor() }
            });
            container.Background = new Color(0, 0.1f, 0.1f, 0);
            ETGModConsole.Instance.GUI[0].Children.Add(container);
        }

        public static float Mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }
}

namespace SGUI
{
    public class ShakyWobbly : SModifier
    {
        float startTime;
        Color defaultColor = new Color(0.5f, 0.9f, 1f);
        float offset;
        float freq;
        float amp;

        public override void Init()
        {
            startTime = Time.realtimeSinceStartup;
            offset = UnityEngine.Random.Range(-0.25f, 0.25f);
            freq = UnityEngine.Random.Range(1, 2.1f);
            amp = UnityEngine.Random.Range(1, 2.1f);
        }

        public override void Update()
        {
            var time = (Time.realtimeSinceStartup - startTime);
            var y = Mathf.Sin(time * 3 * freq) * amp;


            Elem.Foreground = defaultColor;
            Elem.Position = Elem.Position.WithY(y);
        }
    }
    public class ShakyWobblyNoColor : SModifier
    {
        float startTime;
        float freq;
        float amp;

        public override void Init()
        {
            startTime = Time.realtimeSinceStartup;
            freq = 1;
            amp = 1.5f;
        }

        public override void Update()
        {
            var time = (Time.realtimeSinceStartup - startTime);
            var y = Mathf.Sin(time * 3 * freq) * amp;
            Elem.Position = Elem.Position.WithY(y);
        }
    }
}