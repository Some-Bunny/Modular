using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using System.IO;
using System.Reflection;

namespace ModularMod
{
    public class StaticTextures
    {
        public static Texture2D tiled_Modular_Background;
        public static Texture2D nebula_Background;
        public static Texture Displacer_NoiseTex;
        public static Texture Displacer_SpaceTex;

        public static void InitTextures()
        {
            tiled_Modular_Background = ResourceExtractor.GetTextureFromResource("ModularMod/Sprites/modular_hud_grid.png");
            nebula_Background = Module.ModularAssetBundle.LoadAsset<Texture2D>("nebula_reducednoise");//ResourceExtractor.GetTextureFromResource("ModularMod/Sprites/nebula_reducednoise.png");

            var Displacer_Beast_Mat = EnemyDatabase.GetOrLoadByGuid("45192ff6d6cb43ed8f1a874ab6bef316").sprite.renderer.material;
            Displacer_SpaceTex = Displacer_Beast_Mat.GetTexture("_SpaceTex");
            Displacer_NoiseTex = Displacer_Beast_Mat.GetTexture("_NoiseTex");
        }
        public static Texture2D GetTextureFromResource(string resourceName)
        {
            string file = resourceName;
            byte[] bytes = ExtractEmbeddedResource(file);
            if (bytes == null)
            {
                return null;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(texture, bytes);
            texture.filterMode = FilterMode.Point;

            string name = file.Substring(0, file.LastIndexOf('.'));
            if (name.LastIndexOf('.') >= 0)
            {
                name = name.Substring(name.LastIndexOf('.') + 1);
            }
            texture.name = name;

            return texture;
        }
        public static byte[] ExtractEmbeddedResource(String filePath)
        {
            filePath = filePath.Replace("/", ".");
            filePath = filePath.Replace("\\", ".");
            var baseAssembly = Assembly.GetCallingAssembly();
            using (Stream resFilestream = baseAssembly.GetManifestResourceStream(filePath))
            {
                if (resFilestream == null)
                {
                    return null;
                }
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }
    }
}
