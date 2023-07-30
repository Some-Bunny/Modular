
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using ModularMod.Code.Collectibles.Guns.Gravity_Pulsar;
using System.Collections.Generic;
using Alexandria.EnemyAPI;
using Alexandria.Misc;

namespace ModularMod
{
    public class GravityPulsarAlt : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Singularity Pulsar", "singularity_pulsar_alt");
            Game.Items.Rename("outdated_gun_mods:singularity_pulsar", "mdl:armcannon_10_alt");
            gun.gameObject.AddComponent<GravityPulsar>();
            gun.SetShortDescription("Mk.2");
            gun.SetLongDescription("Fires large rifts, followed up with energy attracted to said rifts. Compatible with Modular Upgrade Software.\n\nAn experimental tech graciously provided by a local laboratory, weaponized into an exotic weapon.");
            
            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "gravgunalt_idle_002", 11);
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "gravgunalt_idle";
            gun.shootAnimation = "gravgunalt_fire";
            gun.reloadAnimation = "gravgunalt_reload";
            gun.finalShootAnimation = "gravgunalt_altfire";
            gun.introAnimation = "gravgunalt_intro";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;



            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_WPN_bsg_shot_01" } });
            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.finalShootAnimation, new Dictionary<int, string> { { 0, "Play_WPN_looper_shot_01" } });
            
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(57) as Gun).gunSwitchGroup;


            gun.reloadTime = 3f;
            gun.DefaultModule.cooldownTime = .33f;
            gun.DefaultModule.numberOfShotsInClip = 26;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 7f;


            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.numberOfFinalProjectiles = 25;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;



            //================================================
            //================================================
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.AnimateProjectileBundle("fwoomp", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "fwoomp",
            new List<IntVector2>() { new IntVector2(48,48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48), new IntVector2(48, 48) },
            ProjectileToolbox.ConstructListOfSameValues(true, 22), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 22), ProjectileToolbox.ConstructListOfSameValues(true, 22), ProjectileToolbox.ConstructListOfSameValues(false, 22),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 22), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 22), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 22), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 22));

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.alwaysUseMidair = true;// Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect;

            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            PierceProjModifier spook = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook.penetration = 20;
            spook.penetratesBreakables = true;
            
            MaintainDamageOnPierce noDamageLoss = projectile.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
            noDamageLoss.damageMultOnPierce = 1f;
            
            BounceProjModifier BounceProjMod = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            BounceProjMod.bouncesTrackEnemies = false;
            BounceProjMod.numberOfBounces = 10;

            projectile.baseData.speed = 6;
            projectile.sprite.usesOverrideMaterial = true;
            projectile.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            projectile.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            projectile.sprite.renderer.material.SetFloat("_EmissivePower", 40);
            projectile.sprite.renderer.material.SetFloat("_EmissiveColorPower", 40);
            projectile.baseData.range *= 2.5f;
            projectile.pierceMinorBreakables = true;
            projectile.PenetratesInternalWalls = true;
            projectile.baseData.damage = 3f;
            projectile.shouldRotate = false;
            projectile.gameObject.AddComponent<GravityPulsarLargeProjectile>();
            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.1875f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(1.1f, 0.5f, 1.1f, 1f);


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = (PickupObjectDatabase.GetById(169) as Gun).DefaultModule.customAmmoType;

            gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.finalCustomAmmoType = "ArmCannonAlt";

            //================================================
            //================================================


            Projectile projectile_Small = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            projectile_Small.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile_Small.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile_Small);
            projectile_Small.SetProjectileCollisionRight("defaultarmcannonalt_projectile_burst_001", StaticCollections.Projectile_Collection, 11, 4, false, tk2dBaseSprite.Anchor.LowerCenter);
            projectile_Small.baseData.range = 250;
            projectile_Small.gameObject.AddComponent<GravityPulsarSmallProjectile>();
            gun.DefaultModule.finalProjectile = projectile_Small;
            PierceProjModifier spook1 = projectile_Small.gameObject.GetOrAddComponent<PierceProjModifier>();
            spook1.penetration = 6;
            spook1.penetratesBreakables = true;
            projectile_Small.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile_Small.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile_Small.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(89) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
            projectile_Small.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(89) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
            projectile_Small.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(89) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
            projectile_Small.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(89) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);

            BounceProjModifier BounceProjMod1 = projectile_Small.gameObject.GetOrAddComponent<BounceProjModifier>();
            BounceProjMod1.bouncesTrackEnemies = false;
            BounceProjMod1.numberOfBounces = 10;

            MaintainDamageOnPierce noDamageLoss1 = projectile_Small.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
            noDamageLoss1.damageMultOnPierce = 1f;

            projectile_Small.baseData.damage = 2.8f;
            ImprovedAfterImage yes1 = projectile_Small.gameObject.AddComponent<ImprovedAfterImage>();
            yes1.spawnShadows = true;
            yes1.shadowLifetime = 0.3f;
            yes1.shadowTimeDelay = 0.01f;
            yes1.dashColor = new Color(0f, 0.7f, 0f, 1f);


            gun.gunClass = GunClass.NONE;



            //Effects//
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects;
            gun.finalMuzzleFlashEffects = (PickupObjectDatabase.GetById(513) as Gun).muzzleFlashEffects;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(169) as Gun).gunSwitchGroup;

            ////


            gun.AddGlowShaderToGun(new Color32(0, 255, 54, 255), 3, 3);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.375f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.375f), "barrel_point").transform;

            //gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            //gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("ArmCannon", StaticCollections.Clip_Ammo_Atlas, "blaster_1", "blaster_2");

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
        }
        public void Start()
        {
            var thing = this.gun.GetComponent<ModularGunController>();
            if (thing)
            {
                thing.statMods.Add(new ModuleGunStatModifier()
                {
                    FinaleClipSize_Process = ProcessClipSize
                });
            }
        }

        public int ProcessClipSize(int currentFinales, int clipSize, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clipSize - 1;
        }
        public static int ID;
    }
}