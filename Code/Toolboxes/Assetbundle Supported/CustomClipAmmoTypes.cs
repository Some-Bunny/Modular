using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;
using Alexandria.ItemAPI;

namespace ModularMod
{
    [HarmonyPatch]
    public static class CustomClipAmmoTypeToolbox
    {
        /*
        [HarmonyPatch(typeof(GameUIAmmoController), "UpdateAmmoUIForModule")]
        [HarmonyPostfix]
        public static void ActivateClipSprites(dfTiledSprite currentAmmoFGSprite, dfTiledSprite currentAmmoBGSprite)
        {
            if (currentAmmoFGSprite != null && !currentAmmoFGSprite.gameObject.activeSelf)
            {
                currentAmmoFGSprite.gameObject.SetActive(true);
            }
            if (currentAmmoBGSprite != null && !currentAmmoBGSprite.gameObject.activeSelf)
            {
                currentAmmoBGSprite.gameObject.SetActive(true);
            }
        }
        */
        /*
         gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
         gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Rebar Shells", "NevernamedsItems/Resources/CustomGunAmmoTypes/rebargun_clipfull", "NevernamedsItems/Resources/CustomGunAmmoTypes/rebargun_clipempty");
         */
        public static void Init()
        {
            ETGModMainBehaviour.Instance.gameObject.AddComponent<AddMissingAmmoTypes>();
        }
        public static List<GameUIAmmoType> addedAmmoTypes = new List<GameUIAmmoType>();
        public static string AddCustomAmmoType(string name, dfAtlas atlas, string name_full, string name_empty, GameUIAmmoType.AmmoType ammoType = GameUIAmmoType.AmmoType.CUSTOM)
        {
            GameObject fgSpriteObject = new GameObject("sprite fg");
            fgSpriteObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(fgSpriteObject);
            UnityEngine.Object.DontDestroyOnLoad(fgSpriteObject);
            GameObject bgSpriteObject = new GameObject("sprite bg");
            bgSpriteObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bgSpriteObject);
            UnityEngine.Object.DontDestroyOnLoad(bgSpriteObject);

            dfTiledSprite fgSprite = fgSpriteObject.SetupDfSpriteFromTexture<dfTiledSprite>(atlas, name_full);
            dfTiledSprite bgSprite = bgSpriteObject.SetupDfSpriteFromTexture<dfTiledSprite>(atlas, name_empty);
            GameUIAmmoType uiammotype = new GameUIAmmoType
            {
                ammoBarBG = bgSprite,
                ammoBarFG = fgSprite,
                ammoType = ammoType,
                customAmmoType = name,
            };
            CustomClipAmmoTypeToolbox.addedAmmoTypes.Add(uiammotype);
            foreach (GameUIAmmoController uiammocontroller in GameUIRoot.Instance.ammoControllers)
            {
                Add(ref uiammocontroller.ammoTypes, uiammotype);
            }
            return name;
        }

        public static T SetupDfSpriteFromTexture<T>(this GameObject obj, dfAtlas atlas, string texture_Name) where T : dfSprite
        {
            T sprite = obj.GetOrAddComponent<T>();
            sprite.Atlas = atlas;
            sprite.SpriteName = texture_Name;
            return sprite;
        }
        public static void Add<T>(ref T[] array, T toAdd)
        {
            List<T> list = array.ToList();
            list.Add(toAdd);
            array = list.ToArray<T>();
        }
    }
    public class AddMissingAmmoTypes : MonoBehaviour
    {
        public void Update()
        {
            if (GameUIRoot.HasInstance)
            {
                foreach (GameUIAmmoController uiammocontroller in GameUIRoot.Instance.ammoControllers)
                {
                    foreach (GameUIAmmoType uiammotype in CustomClipAmmoTypeToolbox.addedAmmoTypes)
                    {
                        if (!uiammocontroller.ammoTypes.Contains(uiammotype))
                        {
                            CustomClipAmmoTypeToolbox.Add(ref uiammocontroller.ammoTypes, uiammotype);
                        }
                    }
                }
            }
        }
    }
}
