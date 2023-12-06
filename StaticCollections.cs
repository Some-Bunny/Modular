using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static tk2dSpriteCollectionData Module_Unique_Collection;

        public static tk2dSpriteCollectionData Item_Collection;
        public static tk2dSpriteCollectionData UI_Collection;

        public static tk2dSpriteCollectionData Gun_Collection;
        public static tk2dSpriteAnimation Gun_Animation;


        public static tk2dSpriteCollectionData Projectile_Collection;
        public static tk2dSpriteAnimation Projectile_Animation;

        public static tk2dSpriteCollectionData Beam_Collection;
        public static tk2dSpriteAnimation Beam_Animation;


        public static tk2dSpriteCollectionData VFX_Collection;


        public static tk2dSpriteCollectionData Crate_Collection;
        public static tk2dSpriteAnimation Crate_Animation;

        public static tk2dSpriteCollectionData Past_Decorative_Object_Collection;

        public static tk2dSpriteCollectionData Enemy_Collection;
        public static tk2dSpriteCollectionData Boss_Collection;


        public static tk2dSpriteCollectionData Modular_Character_Collection;
        public static tk2dSpriteCollectionData Modular_Character_Alt_Collection;

        public static dfAtlas Clip_Ammo_Atlas;

        public static dfFontBase ModularFont;
        public static dfAtlas ModularUIAtlas;

        public static void InitialiseCollections()
        {
            Module_T1_Collection = DoFastSetup(Module.ModularAssetBundle, "Tier_1_Module_Collection", "t1_module material.mat");


            ModularFont = Module.ModularAssetBundle.LoadAsset<GameObject>("ShootPeopUltra-16").GetComponent<dfFont>();
            //Mechanical Demo (Dynamic)
            //ShootPeopUltra (Dynamic)
            Clip_Ammo_Atlas = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularGunClip_Atlas").GetComponent<dfAtlas>();

            GameObject obj = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularUIAtlas");
            UnityEngine.Object.DontDestroyOnLoad(obj);
            ModularUIAtlas = obj.GetComponent<dfAtlas>();

            if (Module_T1_Collection == null) { ETGModConsole.Log("Module_T1_Collection is NULL"); }

            Module_T2_Collection = DoFastSetup(Module.ModularAssetBundle, "Tier_2_Module_Collection", "t2_module material.mat");
            if (Module_T2_Collection == null) { ETGModConsole.Log("Module_T2_Collection is NULL"); }

            Module_T3_Collection = DoFastSetup(Module.ModularAssetBundle, "Module_T3_Collection", "t3_module material.mat");
            if (Module_T3_Collection == null) { ETGModConsole.Log("Module_T3_Collection is NULL"); }

            Module_T4_Collection = DoFastSetup(Module.ModularAssetBundle, "ModuleTier4Collection", "modulrt4 material.mat");
            if (Module_T4_Collection == null) { ETGModConsole.Log("Module_T4_Collection is NULL"); }

            Module_Unique_Collection = DoFastSetup(Module.ModularAssetBundle, "UniqueModuleCollection", "unique module material.mat");
            if (Module_Unique_Collection == null) { ETGModConsole.Log("Module_Unique_Collection is NULL"); }

            

            Item_Collection = DoFastSetup(Module.ModularAssetBundle, "ModuleItemCollection", "moduleitem material.mat");
            if (Item_Collection == null) { ETGModConsole.Log("Item_Collection is NULL"); }

            Beam_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularBeamCollection", "beam material.mat");
            if (Beam_Collection == null) { ETGModConsole.Log("Beam_Collection is NULL"); }
            Beam_Animation = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularBeamAnimation").GetComponent<tk2dSpriteAnimation>();

            Gun_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularGunCollection", "modulargun material.mat");
            if (Gun_Collection == null) { ETGModConsole.Log("Gun_Collection is NULL"); }
            Gun_Animation = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularGunAnimation").GetComponent<tk2dSpriteAnimation>();

            GunJsonEmbedder.EmbedJsonDataFromAssembly(Assembly.GetExecutingAssembly(), Gun_Collection, "ModularMod/Pain/GunJsons");


            Projectile_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularProjectileCollection", "modularprojectile material.mat");
            if (Projectile_Collection == null) { ETGModConsole.Log("Projectile_Collection is NULL"); }
            Projectile_Animation = Module.ModularAssetBundle.LoadAsset<GameObject>("ModularProjectileAnimation").GetComponent<tk2dSpriteAnimation>();

            //Crate and Decoratives
            Crate_Collection = DoFastSetup(Module.ModularAssetBundle, "CrateCollection", "crate material.mat");
            if (Crate_Collection == null) { ETGModConsole.Log("Crate_Collection is NULL"); }
            Crate_Animation = Module.ModularAssetBundle.LoadAsset<GameObject>("CrateAnimation").GetComponent<tk2dSpriteAnimation>();

            Past_Decorative_Object_Collection = DoFastSetup(Module.ModularAssetBundle, "PastDecorCollection", "decor material.mat");
            if (Past_Decorative_Object_Collection == null) { ETGModConsole.Log("Past_Decorative_Object_Collection is NULL"); }
            
            //Enemies
            Enemy_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularEnemyCollection", "mdlrenemy material.mat");
            if (Enemy_Collection == null) { ETGModConsole.Log("Enemy_Collection is NULL"); }

            Boss_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularBossCollection", "modularboss material.mat");
            if (Boss_Collection == null) { ETGModConsole.Log("Boss_Collection is NULL"); }

            //Character
            Modular_Character_Alt_Collection = DoFastSetup(Module.ModularAssetBundle, "Modular_Alt_Collection", "modular_alt material.mat");
            if (Modular_Character_Alt_Collection == null) { ETGModConsole.Log("Modular_Character_Alt_Collection is NULL"); }

            Modular_Character_Collection = DoFastSetup(Module.ModularAssetBundle, "Modular_Collection", "modular material.mat");
            if (Modular_Character_Collection == null) { ETGModConsole.Log("Modular_Character_Collection is NULL"); }
            //

            //VFX
            VFX_Collection = DoFastSetup(Module.ModularAssetBundle, "ModularVFXCollection", "modular_vfx material.mat");
            if (VFX_Collection == null) { ETGModConsole.Log("VFX_Collection is NULL"); }


            //Generic_VFX_Animation
            Generic_VFX_Animation = Module.ModularAssetBundle.LoadAsset<GameObject>("GenericVFXAnimation").GetComponent<tk2dSpriteAnimation>();
        }
        public static tk2dSpriteAnimation Generic_VFX_Animation;

        public static tk2dSpriteCollectionData DoFastSetup(AssetBundle bundle, string CollectionName, string MaterialName)
        {
            tk2dSpriteCollectionData Colection = bundle.LoadAsset<GameObject>(CollectionName).GetComponent<tk2dSpriteCollectionData>();
            Material material = bundle.LoadAsset<Material>(MaterialName);
            Texture texture = material.GetTexture("_MainTex");
            texture.filterMode = FilterMode.Point;
            material.SetTexture("_MainTex", texture);
            Colection.material = material;

            Colection.materials = new Material[]
            {
                material,
            };
            Colection.materialInsts = new Material[]
            {
                material,
            };
            foreach (var c in Colection.spriteDefinitions)
            {
                c.material = Colection.materials[0];
                c.materialInst = Colection.materials[0];
                c.materialId = 0;
            }
            return Colection;
        }

    }
}
