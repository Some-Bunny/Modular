
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using ModularMod.Code.Collectibles.Guns.Update_3;
using System.Collections.Generic;

namespace ModularMod
{
    public class Fortifier : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Fortifier", "turretplacer");
            Game.Items.Rename("outdated_gun_mods:fortifier", "mdl:armcannon_15");
            var c = gun.gameObject.AddComponent<Fortifier>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires wall-mounted turrets. Compatible with Modular Upgrade Software.\n\nThe perfect defense? An even better offense.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "turretplace_reload_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "turretplacer_idle";
            gun.shootAnimation = "turretplacer_fire";
            gun.reloadAnimation = "turretplacer_reload";
            gun.introAnimation = "turretplacer_intro";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(156) as Gun).gunSwitchGroup;


            gun.reloadTime = 6.5f;
            gun.DefaultModule.cooldownTime = 5;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 0f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

            projectile.AnimateProjectileBundle("turretidle", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "turretidle",
            new List<IntVector2>() { new IntVector2(8, 8), new IntVector2(8, 8), new IntVector2(8, 8), new IntVector2(8, 8), new IntVector2(8, 8), new IntVector2(8, 8), },
            ProjectileToolbox.ConstructListOfSameValues(true, 6), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 6), ProjectileToolbox.ConstructListOfSameValues(true, 6), ProjectileToolbox.ConstructListOfSameValues(false, 6),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 6), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 6), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 6), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 6));


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(121, 234, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 10);
            mat.SetFloat("_EmissivePower", 10);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.2f);
            mat.SetTexture("_MainTex", projectile.sprite.renderer.material.GetTexture("_MainTex"));
            projectile.sprite.renderer.material = mat;

            projectile.baseData.speed = 45;
            projectile.baseData.damage = 15f;
            projectile.shouldRotate = false;
            projectile.baseData.range = 100000;


            Projectile turretProjectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            turretProjectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(turretProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(turretProjectile);

            turretProjectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            turretProjectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            turretProjectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            turretProjectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            turretProjectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            turretProjectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            turretProjectile.baseData.damage = 10;
            turretProjectile.baseData.speed = 35;
            turretProjectile.baseData.range = 10000;
            turretProjectile.shouldRotate = true;

            ImprovedAfterImage yes1 = turretProjectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes1.spawnShadows = true;
            yes1.shadowLifetime = 0.75f;
            yes1.shadowTimeDelay = 0.05f;
            yes1.dashColor = new Color(0, 0.5f, 0.5f, 1f);

            Material mat2 = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            mat2.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat2.SetFloat("_EmissivePower", 20);
            mat2.SetFloat("_EmissiveColorPower", 20);
            turretProjectile.sprite.renderer.material = mat2;

            turretProjectile.AnimateProjectileBundle("longshot_idle", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "longshot_idle",
            new List<IntVector2>() { new IntVector2(14, 5), new IntVector2(14, 5) },
            ProjectileToolbox.ConstructListOfSameValues(true, 2), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 2), ProjectileToolbox.ConstructListOfSameValues(true, 2), ProjectileToolbox.ConstructListOfSameValues(false, 2),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 2), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 2), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 2), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 2));

            ExplosiveModifier explosiveModifier = projectile.gameObject.GetOrAddComponent<ExplosiveModifier>();
            explosiveModifier.explosionData = new ExplosionData()
            {
                breakSecretWalls = false,
                comprehensiveDelay = 0,
                damage = 12,
                damageRadius = 2.5f,
                damageToPlayer = 0,
                debrisForce = 40,
                doDamage = true,
                doDestroyProjectiles = false,
                doExplosionRing = false,
                doForce = true,
                doScreenShake = false,
                doStickyFriction = false,
                effect = StaticExplosionDatas.genericLargeExplosion.effect,
                explosionDelay = 0,
                force = 4,
                forcePreventSecretWallDamage = false,
                forceUseThisRadius = true,
                freezeEffect = null,
                freezeRadius = 0,
                IsChandelierExplosion = false,
                isFreezeExplosion = false,
                playDefaultSFX = true,
                preventPlayerForce = false,
                pushRadius = 1,
                secretWallsRadius = 1,
            };
            explosiveModifier.doExplosion = true;
            explosiveModifier.IgnoreQueues = true;


            PierceProjModifier bounceProjModifier = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += 1000;
            projectile.pierceMinorBreakables = true;
            var turretComp = projectile.gameObject.GetOrAddComponent<TurretComponent>();
            turretComp.materialToCopy = mat;
            turretComp.muzzleFlashPrefab = (PickupObjectDatabase.GetById(153) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
            TurretComponent.projectileToFire = turretProjectile;



            projectile.gameObject.AddComponent<StickyProjectileModifier>();

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0.85f, 0.85f, 0.85f, 1f);

            gun.gunClass = GunClass.NONE;


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("TurretBall", StaticCollections.Clip_Ammo_Atlas, "turret_1", "turret_2");

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(1, -1);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(370) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.5f, 0.5f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.5f, 0.5f), "barrel_point").transform;

            gun.IsMinusOneGun = true;

            

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            Fortifier.GunID = gun.PickupObjectId;
            IteratedDesign.SpecialProcessGunSpecificReload += c.ProcessFireRateSpecial;
        }

        public float ProcessFireRateSpecial(float f, int stack, ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != GunID) { return f; }
            return f / (1 + (stack / 4));
        }



        public void Start()
        {
            gun.IsMinusOneGun = true;
        }

        public static int GunID;
    }
}