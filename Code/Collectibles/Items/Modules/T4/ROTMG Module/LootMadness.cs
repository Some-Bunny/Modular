using Alexandria.ItemAPI;
using Brave.BulletScript;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Alexandria.Misc;
using HarmonyLib;
using static ModularMod.LootMadness;
using System.ComponentModel;
using UnityEngine.SocialPlatforms;

namespace ModularMod
{
    public class LootMadness : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(LootMadness))
        {
            Name = "Loot Madness",
            Description = "Devourer",
            LongDescription = "Acts as 1 (+1 per stack) of every module you will own." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T4_Collection,
            ManualSpriteID = StaticCollections.Module_T4_Collection.GetSpriteIdByName("LOOT_MADNESS"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            //h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("swarmer_t3_module_alt");
            h.Tier = ModuleTier.Tier_Omega;
            h.LabelName = "LOOT MADNESS " + h.ReturnTierLabel();
            h.LabelDescription = "OBTAIN UNIQUE LOOT FROM A REALM CLOUDED BY MADNESS\n(" + StaticColorHexes.AddColorToLabelString("+HIGHER LOOT CHANCE", StaticColorHexes.Light_Orange_Hex) + ").\nLOOT CAN BE USED IN COMBAT.";
            h.SetTag("modular_module");
            h.AddColorLight(Color.red);
            h.powerConsumptionData = new PowerConsumptionData()
            {
                FirstStack = 0,
                AdditionalStacks = 0,
                OverridePowerDescriptionLabel = "USES NO POWER.",
                OverridePowerManagement = null
            };
            h.AddToGlobalStorage();
            h.AdditionalWeightMultiplier = 1f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.Label_Background_Color_Override = new Color32(255, 10, 10, 100);

            ID = h.PickupObjectId;

            GlobalTraitList.InitTraits();
            InitProjectiles();
            InitOrbitalPrefab();
        }
        public static void InitProjectiles()
        {
            var proj_Base_Sword = (PickupObjectDatabase.GetById(377) as Gun).DefaultModule.projectiles[0];
            Projectile swordProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
            swordProjectile.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(swordProjectile.gameObject);
            DontDestroyOnLoad(swordProjectile.gameObject);
            swordProjectile.baseData.damage = 18f;
            swordProjectile.shouldRotate = true;
            swordProjectile.baseData.range = 4.5f;
            swordProjectile.baseData.speed = 11;
            swordProjectile.baseData.force = 30;
            swordProjectile.baseData.UsesCustomAccelerationCurve = true;
            swordProjectile.baseData.AccelerationCurve = AnimationCurve.Linear(0.1f, 1f, 1, 0.6f);
            swordProjectile.SetProjectileCollisionRight("swordslash", StaticCollections.Projectile_Collection, 15, 33, false, tk2dBaseSprite.Anchor.LowerCenter);
            swordProjectile.objectImpactEventName = proj_Base_Sword.objectImpactEventName;
            swordProjectile.enemyImpactEventName = proj_Base_Sword.enemyImpactEventName;
            swordProjectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX(proj_Base_Sword.hitEffects.enemy.effects.First().effects.First().effect);
            swordProjectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX(proj_Base_Sword.hitEffects.enemy.effects.First().effects.First().effect);
            swordProjectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX(proj_Base_Sword.hitEffects.enemy.effects.First().effects.First().effect);
            swordProjectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX(proj_Base_Sword.hitEffects.enemy.effects.First().effects.First().effect);
            swordProjectile.AdditionalScaleMultiplier *= 0.66f;

            swordProjectile.sprite.usesOverrideMaterial = true;
            Material mat = swordProjectile.sprite.renderer.material;
            mat.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 50);
            mat.SetColor("_OverrideColor", new Color(1, 1, 1, 1));
            SwordProj = swordProjectile;

            var proj_Base_Arrow = (PickupObjectDatabase.GetById(12) as Gun).DefaultModule.projectiles[0];
            Projectile arrowProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
            arrowProjectile.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(arrowProjectile.gameObject);
            DontDestroyOnLoad(arrowProjectile.gameObject);
            arrowProjectile.baseData.damage = 10f;
            arrowProjectile.shouldRotate = true;
            arrowProjectile.baseData.range = 30f;
            arrowProjectile.baseData.speed = 25;
            arrowProjectile.baseData.force = 12;
            arrowProjectile.SetProjectileCollisionRight("arrow", StaticCollections.Projectile_Collection, 24, 9, false, tk2dBaseSprite.Anchor.LowerCenter);
            arrowProjectile.objectImpactEventName = proj_Base_Arrow.objectImpactEventName;
            arrowProjectile.enemyImpactEventName = proj_Base_Arrow.enemyImpactEventName;
            arrowProjectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX(proj_Base_Arrow.hitEffects.enemy.effects.First().effects.First().effect);
            arrowProjectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX(proj_Base_Arrow.hitEffects.enemy.effects.First().effects.First().effect);
            arrowProjectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX(proj_Base_Arrow.hitEffects.enemy.effects.First().effects.First().effect);
            arrowProjectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX(proj_Base_Arrow.hitEffects.enemy.effects.First().effects.First().effect);
            arrowProjectile.sprite.usesOverrideMaterial = true;
            arrowProjectile.AdditionalScaleMultiplier *= 0.66f;

            Material mat1 = arrowProjectile.sprite.renderer.material;
            mat1.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat1.SetFloat("_EmissivePower", 100);
            mat1.SetFloat("_EmissiveColorPower", 50);
            mat1.SetColor("_OverrideColor", new Color(1, 1, 1, 1));
            ArrowProj = arrowProjectile;

            var proj_Base_Staff = (PickupObjectDatabase.GetById(145) as Gun).DefaultModule.projectiles[0];
            Projectile staffProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
            staffProjectile.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(staffProjectile.gameObject);
            DontDestroyOnLoad(staffProjectile.gameObject);
            staffProjectile.baseData.damage = 16f;
            staffProjectile.shouldRotate = true;
            staffProjectile.baseData.range = 12f;
            staffProjectile.baseData.speed = 9;
            staffProjectile.baseData.force = 0;
            staffProjectile.SetProjectileCollisionRight("spell", StaticCollections.Projectile_Collection, 21, 9, false, tk2dBaseSprite.Anchor.LowerCenter);
            staffProjectile.objectImpactEventName = proj_Base_Staff.objectImpactEventName;
            staffProjectile.enemyImpactEventName = proj_Base_Staff.enemyImpactEventName;
            staffProjectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX(proj_Base_Staff.hitEffects.enemy.effects.First().effects.First().effect);
            staffProjectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX(proj_Base_Staff.hitEffects.enemy.effects.First().effects.First().effect);
            staffProjectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX(proj_Base_Staff.hitEffects.enemy.effects.First().effects.First().effect);
            staffProjectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX(proj_Base_Staff.hitEffects.enemy.effects.First().effects.First().effect);
            staffProjectile.sprite.usesOverrideMaterial = true;
            staffProjectile.AdditionalScaleMultiplier *= 0.66f;
            Material mat2 = staffProjectile.sprite.renderer.material;
            mat2.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat2.SetFloat("_EmissivePower", 100);
            mat2.SetFloat("_EmissiveColorPower", 50);
            mat2.SetColor("_OverrideColor", new Color(1, 1, 1, 1));
            StaffProj = staffProjectile;
        }

        public static void InitOrbitalPrefab()
        {
            GameObject Guon = new GameObject("Weapon_Orbital");
            FakePrefab.DontDestroyOnLoad(Guon);
            FakePrefab.MarkAsFakePrefab(Guon);
            var tk2d = Guon.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("blade_anim_001"));

            var speculativeRigidbody = Guon.CreateFastBody(CollisionLayer.EnemyBulletBlocker, new IntVector2(0, 0), new IntVector2(-1, -1));
            speculativeRigidbody.CollideWithTileMap = false;
            speculativeRigidbody.CollideWithOthers = false;

            
            var orbital = Guon.AddComponent<PlayerOrbital>();
            orbital.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
            orbital.shouldRotate = false;
            orbital.orbitRadius = 2;
            orbital.SetOrbitalTier(0);
            orbital.orbitDegreesPerSecond = 120;
            orbital.perfectOrbitalFactor = 180f;
           

            tk2d.usesOverrideMaterial = true;
            tk2d.sprite.usesOverrideMaterial = true;
            Material mat = tk2d.sprite.renderer.material;
            mat.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 50);
            mat.SetColor("_OverrideColor", new Color(1, 1, 1, 1));
            orbitalPrefab = Guon;
        }


        public static Projectile SwordProj, StaffProj, ArrowProj;
        public static GameObject orbitalPrefab;
        public static GameObject orbital_sprite_Prefab;

        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnKilledEnemy += OKEC;
            Alexandria.Misc.CustomActions.OnChestPreOpen += OCPO;
        }

        public void OCPO(Chest ch, PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_The_Bag", player.gameObject);
            LootEngine.SpawnItem(PickupObjectDatabase.GetById(White_Bag.ID).gameObject, player.sprite.WorldCenter, UnityEngine.Random.insideUnitCircle, 3, false);
        }

        public void OKEC(ModulePrinterCore printer, PlayerController player, AIActor enemy)
        {
            if (enemy == null) { return; }
            if (UnityEngine.Random.value < (1 - (1 - (this.ReturnStack(printer) / 22f))))
            {
                AkSoundEngine.PostEvent("Play_The_Bag", player.gameObject);
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(White_Bag.ID).gameObject, enemy.sprite.WorldCenter, UnityEngine.Random.insideUnitCircle, 3, false);
            }
        }


        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnKilledEnemy -= OKEC;
            Alexandria.Misc.CustomActions.OnChestPreOpen -= OCPO;
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {

        }
    }
}

