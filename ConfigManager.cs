using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModularMod
{
    public static class ConfigManager
    {
        public static void CreateConfig(ConfigFile config)
        {
            DoVFX = config.Bind<bool>("Modular Remastered:", "Visual Effects", true, "(Default of true) Plays certain visual effects. Setting it to false disables more intrusive effects.");
            ImportantVFXIntensity = config.Bind<float>("Modular Remastered:", "Important VFX Intensity", 1, "(Default of 1) The intensity of certain visual effects. The lower the value, the less the intensity.");
            DistortionWaveIntensity = config.Bind<float>("Modular Remastered:", "Distortion Wave Intensity", 1, "(Default of 1) The intensity of distortion waves used by the mod. The lower the value, the less the intensity of the distortion waves.");
            AfterimageLifetimeMultiplier = config.Bind<float>("Modular Remastered:", "Afterimage Lifetime Multiplier", 1, "(Default of 1) The lifetime of afterimages used my the mod. 1 will make them last as long as usual, 0.1 will make them only last 10% of their intended lifetime.");

        }
        public static ConfigEntry<bool> DoVFX;
        public static ConfigEntry<float> ImportantVFXIntensity;
        public static ConfigEntry<float> DistortionWaveIntensity;
        public static ConfigEntry<float> AfterimageLifetimeMultiplier;

        public static bool DoVisualEffect
        {
            get 
            {
                return DoVFX.Value;
            }
        }
        public static float ImportantVFXMultiplier
        {
            get
            {
                return ImportantVFXIntensity.Value;
            }
        }
        public static float DistortionWaveMultiplier
        {
            get
            {
                return DistortionWaveIntensity.Value;
            }
        }
        public static float AfterimageLifetime
        {
            get
            {
                return AfterimageLifetimeMultiplier.Value;
            }
        }
    }
}
