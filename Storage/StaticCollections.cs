using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public static class StaticCollections
    {
        public static tk2dSpriteCollectionData Module_T1_Collection;
        public static tk2dSpriteCollectionData Module_T2_Collection;
        public static tk2dSpriteCollectionData Module_T3_Collection;
        public static tk2dSpriteCollectionData Module_T4_Collection;

        public static tk2dSpriteCollectionData Item_Collection;
        public static tk2dSpriteCollectionData UI_Collection;

        public static tk2dSpriteCollectionData Gun_Collection;
        public static tk2dSpriteCollectionData Projectile_Collection;
        public static tk2dSpriteCollectionData Beam_Collection;
        public static tk2dSpriteAnimation Beam_Animation;


        public static tk2dSpriteCollectionData VFX_Collection;


        public static tk2dSpriteCollectionData Modular_Character_Collection;
        public static tk2dSpriteCollectionData Modular_Character_Alt_Collection;


        public static void InitialiseCollections()
        {
            Module_T1_Collection = DoFastSetup(Module.ModularAssetBundle, "Tier_1_Module_Collection", "t1_module material.mat");
            if (Module_T1_Collection == null) { ETGModConsole.Log("Module_T1_Collection is NULL"); }

            Module_T2_Collection = DoFastSetup(Module.ModularAssetBundle, "Tier_2_Module_Collection", "t2_module material.mat");
            if (Module_T2_Collection == null) { ETGModConsole.Log("Module_T2_Collection is NULL"); }

            Module_T3_Collection = DoFastSetup(Module.ModularAssetBundle, "Module_T3_Collection", "t3_module material.mat");
            if (Module_T3_Collection == null) { ETGModConsole.Log("Module_T3_Collection is NULL"); }

            Module_T4_Collection = DoFastSetup(Module.ModularAssetBundle, "ModuleTier4Collection", "modulrt4 material.mat");
            if (Module_T4_Collection == null) { ETGModConsole.Log("Module_T4_Collection is NULL"); }

            Item_Collection = DoFastSetup(Module.ModularAssetBundle, "ModuleItemCollection", "moduleitem material.mat");
            if (Item_Collection == null) { ETGModConsole.Log("Item_Collection is NULL"); }

            Beam_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularBeamCollection", "beam material.mat");
            if (Beam_Collection == null) { ETGModConsole.Log("Beam_Collection is NULL"); }

            Beam_Animation = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularBeamAnimation").GetComponent<tk2dSpriteAnimation>();

            Modular_Character_Alt_Collection = DoFastSetup(Module.ModularAssetBundle, "Modular_Alt_Collection", "modular_alt material.mat");
            if (Modular_Character_Alt_Collection == null) { ETGModConsole.Log("Modular_Character_Alt_Collection is NULL"); }

            Modular_Character_Collection = DoFastSetup(Module.ModularAssetBundle, "Modular_Collection", "modular material.mat");
            if (Modular_Character_Collection == null) { ETGModConsole.Log("Modular_Character_Collection is NULL"); }

            VFX_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularVFXCollection", "modular_vfx material.mat");
            if (VFX_Collection == null) { ETGModConsole.Log("VFX_Collection is NULL"); }
        }

        public static tk2dSpriteCollectionData DoFastSetup(AssetBundle bundle, string CollectionName, string MaterialName)
        {
            tk2dSpriteCollectionData Colection = bundle.LoadAsset<GameObject>(CollectionName).GetComponent<tk2dSpriteCollectionData>();
            Material material = bundle.LoadAsset<Material>(MaterialName);
            FastAssetBundleSpriteSetup(Colection, material);
            return Colection;
        }
        public static void FastAssetBundleSpriteSetup(tk2dSpriteCollectionData bundleData, Material mat)
        {
            Texture texture = mat.GetTexture("_MainTex");
            texture.filterMode = FilterMode.Point;
            mat.SetTexture("_MainTex", texture);
            bundleData.material = mat;

            bundleData.materials = new Material[]
            {
                mat,
            };
            bundleData.materialInsts = new Material[]
            {
                mat,
            };
            foreach (var c in bundleData.spriteDefinitions)
            {
                c.material = bundleData.materials[0];
                c.materialInst = bundleData.materials[0];
                c.materialId = 0;
            }
        }
    }
}
