﻿
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;
using ModularMod.Code.Collectibles.Guns.Update_3;
using Alexandria.Misc;

namespace ModularMod
{
    public class ShieldGenerator : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Barrier Builder", "barrierbuilder");
            Game.Items.Rename("outdated_gun_mods:barrier_builder", "mdl:armcannon_14");
            var c = gun.gameObject.AddComponent<ShieldGenerator>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Creates energy cubes that projectiles stick to. Compatible with Modular Upgrade Software.\n\nUsing hardlight technology as a weapon? What next?");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "shieldgen_idle_004");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "shieldgen_idle";
            gun.shootAnimation = "shieldgen_fire";
            gun.reloadAnimation = "shieldgen_reload";
            gun.introAnimation = "shieldgen_intro";
            gun.chargeAnimation = "shieldgen_charge";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(57) as Gun, true, false);

            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.chargeAnimation, new Dictionary<int, string> { 
                { 5, "Play_BOSS_cannon_stop_01" },
                { 6, "Play_BOSS_cyborg_charge_01" },
                { 15, "Play_WPN_thor_charge_01" },
                { 19, "Play_OBJ_mine_beep_01" },
            });
            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> {
                { 0, "Play_ITM_Macho_Brace_Trigger_01" },
            });
            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> {
                { 0, "Play_BOSS_omegaBeam_fade_01" },
                { 6, "Play_BOSS_hatch_open_01" },
                { 13, "Play_BOSS_lasthuman_torch_01" },

            });
            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(21) as Gun).gunSwitchGroup;


            gun.reloadTime = 4f;
            gun.DefaultModule.cooldownTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 6f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            projectile.gameObject.name = "EnergyCube_Small";
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.AnimateProjectileBundle("energycube_small", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "energycube_small",        
            new List<IntVector2>() { new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15), new IntVector2(10, 15) },
            ProjectileToolbox.ConstructListOfSameValues(true, 12), 
            ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 12),
            ProjectileToolbox.ConstructListOfSameValues(true, 12),
            ProjectileToolbox.ConstructListOfSameValues(false, 12),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 12),
            ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 12),
            ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 12),
            ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 12));

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectile.sprite.renderer.material = mat;
            projectile.baseData.speed = 7f;
            projectile.baseData.damage = 3f;
            projectile.shouldRotate = false;
            projectile.baseData.force = 3;
            projectile.collidesWithProjectiles = true;
            projectile.projectileHitHealth = 5;
            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.033f);
            projectile.baseData.range *= 1.2f;

            projectile.gameObject.AddComponent<BeamCollisionEvent>();

            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);


            var snare = projectile.gameObject.AddComponent<ShieldEater>();
            projectile.gameObject.AddComponent<MaintainDamageOnPierce>();


            PierceProjModifier pierceProjModifier = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierceProjModifier.penetration = 100;

            BounceProjModifier bounceProjModifier = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier.numberOfBounces = 3;

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.25f;
            yes.dashColor = new Color(0f, 0.7f, 0.7f, 1f);

            Projectile projectileBig = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectileBig.gameObject.SetActive(false);
            projectileBig.gameObject.name = "EnergyCube_Small";
            FakePrefab.MarkAsFakePrefab(projectileBig.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectileBig);

            projectileBig.AnimateProjectileBundle("largecube", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "largecube",
            new List<IntVector2>() { new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), new IntVector2(25, 30), },
            ProjectileToolbox.ConstructListOfSameValues(true, 18),
            ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 18),
            ProjectileToolbox.ConstructListOfSameValues(true, 18),
            ProjectileToolbox.ConstructListOfSameValues(false, 18),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 18),
            ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 18),
            ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 18),
            ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 18));

            Material mat2 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectileBig.sprite.renderer.material = mat2;
            projectileBig.baseData.speed = 10f;
            projectileBig.baseData.damage = 6f;
            projectileBig.shouldRotate = false;
            projectileBig.baseData.force = 5;
            projectileBig.collidesWithProjectiles = true;
            //projectileBig.collidesOnlyWithPlayerProjectiles
            projectileBig.projectileHitHealth = 15;
            projectileBig.baseData.UsesCustomAccelerationCurve = true;
            projectileBig.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.05f);
            projectileBig.baseData.range *= 3f;

            projectileBig.objectImpactEventName = (PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectileBig.enemyImpactEventName = (PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectileBig.gameObject.AddComponent<BeamCollisionEvent>();


            projectileBig.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectileBig.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectileBig.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectileBig.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

            projectileBig.gameObject.AddComponent<MaintainDamageOnPierce>();

            var b = projectileBig.gameObject.AddComponent<ShieldBlock>();
            b.Range = 1.5f;

            PierceProjModifier pierceProjModifier1 = projectileBig.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierceProjModifier1.penetration = 100;
            projectileBig.gameObject.AddComponent<MaintainDamageOnPierce>();

            BounceProjModifier bounceProjModifier1 = projectileBig.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier1.numberOfBounces = 5;

            ImprovedAfterImage yes1 = projectileBig.gameObject.AddComponent<ImprovedAfterImage>();
            yes1.spawnShadows = true;
            yes1.shadowLifetime = 0.5f;
            yes1.shadowTimeDelay = 0.25f;
            yes1.dashColor = new Color(0f, 0.7f, 0.7f, 1f);



            Projectile projTinyCube = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(57) as Gun).DefaultModule.projectiles[0]);
            projTinyCube.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projTinyCube.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projTinyCube);
            projTinyCube.SetProjectileCollisionRight("defaultarmcannon_projectile_001", StaticCollections.Projectile_Collection, 4, 4, false, tk2dBaseSprite.Anchor.LowerCenter);
            projTinyCube.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projTinyCube.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            projTinyCube.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projTinyCube.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projTinyCube.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projTinyCube.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projTinyCube.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projTinyCube.sprite.renderer.material = mat;
            projTinyCube.baseData.speed = 15f;
            projTinyCube.baseData.damage = 3.25f;
            projTinyCube.baseData.range = 125f;

            projTinyCube.shouldRotate = false;
            projTinyCube.baseData.force *= 0.25f;

            projTinyCube.gameObject.AddComponent<BeamCollisionEvent>();
            projTinyCube.gameObject.AddComponent<MaintainDamageOnPierce>();
            projTinyCube.collidesWithProjectiles = true;
            var s_e = projTinyCube.gameObject.AddComponent<ShieldEater>();
            s_e.Damage = 1;
            projTinyCube.baseData.UsesCustomAccelerationCurve = true;
            projTinyCube.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.1f);

            ProjectileModule.ChargeProjectile item1 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projTinyCube,
                ChargeTime = 0,
                AmmoCost = 1,
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0.55f,
                AmmoCost = 1,
            };
            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectileBig,
                ChargeTime = 1.85f,
                AmmoCost = 1,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item1,
                item2,
                item3,
            };
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("EnergyCube", StaticCollections.Clip_Ammo_Atlas, "cube_1", "cube_2");

            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 4);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(156) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.5f, 0.9375f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.5f, 0.9375f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            GunID = gun.PickupObjectId;
            IteratedDesign.SpecialProcessGunSpecificFireRate += c.ProcessFireRateSpecial;
        }

        public float ProcessFireRateSpecial(float f, int stack, ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != GunID) { return f; }
            return f / (1 + (stack / 5));
        }

        public static int GunID;
    }
}