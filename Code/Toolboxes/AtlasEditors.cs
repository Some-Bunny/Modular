using Alexandria.NPCAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public static class AtlasEditors
    {
        public static string AddUITextImage(string ammoTypeSpritePath, string name)
        {
            Toolbox.LoadAssetFromAnywhere<GameObject>("Ammonomicon Atlas").GetComponent<dfAtlas>().AddNewItemToAtlas(GetTextureFromResource(ammoTypeSpritePath), name);
            return GameUIRoot.Instance.ConversationBar.portraitSprite.Atlas.AddNewItemToAtlas(GetTextureFromResource(ammoTypeSpritePath), name).name;
        }

        public static Texture2D GetTextureFromResource(string resourceName)
        {
            byte[] array = ExtractEmbeddedResource(resourceName);
            bool flag = array == null;
            Texture2D result;
            if (flag)
            {
                //Tools.PrintError<string>("No bytes found in " + resourceName, "FF0000");
                result = null;
            }
            else
            {
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                texture2D.LoadImage(array);
                texture2D.filterMode = FilterMode.Point;
                string text = resourceName.Substring(0, resourceName.LastIndexOf('.'));
                bool flag2 = text.LastIndexOf('.') >= 0;
                if (flag2)
                {
                    text = text.Substring(text.LastIndexOf('.') + 1);
                }
                texture2D.name = text;
                result = texture2D;
            }
            return result;
        }
        public static byte[] ExtractEmbeddedResource(string filePath)
        {
            filePath = filePath.Replace("/", ".");
            filePath = filePath.Replace("\\", ".");
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            byte[] result;
            using (Stream manifestResourceStream = callingAssembly.GetManifestResourceStream(filePath))
            {
                bool flag = manifestResourceStream == null;
                if (flag)
                {
                    result = null;
                }
                else
                {
                    byte[] array = new byte[manifestResourceStream.Length];
                    manifestResourceStream.Read(array, 0, array.Length);
                    result = array;
                }
            }
            return result;
        }

    }
}
