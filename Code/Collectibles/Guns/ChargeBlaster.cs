
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;

namespace ModularMod
{
    public class ChargeBlaster : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Charge Blaster", "energycharger");
            Game.Items.Rename("outdated_gun_mods:charge_blaster", "mdl:armcannon_4");
            gun.gameObject.AddComponent<ChargeBlaster>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires weak energy pellets, can be charged up for a strong attack. Compatible with Modular Upgrade Software.\n\nDraws a lot of power to fire.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "energycharger_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "energycharger_idle";
            gun.shootAnimation = "energycharger_fire";
            gun.reloadAnimation = "energycharger_reload";
            gun.introAnimation = "energycharger_intro";
            gun.chargeAnimation = "energycharger_charge";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(57) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;
            comp.AdditionalPowerSupply = 0;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(41) as Gun).gunSwitchGroup;


            gun.reloadTime = 4f;
            gun.DefaultModule.cooldownTime = .2f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 3f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.SetProjectileCollisionRight("defaultarmcannon_projectile_001", StaticCollections.Projectile_Collection, 4, 4, false, tk2dBaseSprite.Anchor.LowerCenter);
            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectile.sprite.renderer.material = mat;
            projectile.baseData.speed = 15f;
            projectile.baseData.damage = 3f;
            projectile.shouldRotate = false;
            projectile.baseData.force *= 5;

            Projectile LargeBullet = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            LargeBullet.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(LargeBullet.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(LargeBullet);

            LargeBullet.AnimateProjectileBundle("modblaster_dile", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "modblaster_dile",
            new List<IntVector2>() { new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10) },
            ProjectileToolbox.ConstructListOfSameValues(true, 9), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 9), ProjectileToolbox.ConstructListOfSameValues(true, 9), ProjectileToolbox.ConstructListOfSameValues(false, 9),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 9));


            LargeBullet.objectImpactEventName = (PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            LargeBullet.enemyImpactEventName = (PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            LargeBullet.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            LargeBullet.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            LargeBullet.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            LargeBullet.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat1.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat1.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat1.SetFloat("_EmissiveColorPower", 100);
            mat1.SetFloat("_EmissivePower", 100);
            LargeBullet.sprite.renderer.material = mat;
            LargeBullet.baseData.speed = 50;
            LargeBullet.baseData.damage = 20f;
            LargeBullet.shouldRotate = true;
            LargeBullet.AdditionalScaleMultiplier *= 1.33f;
            LargeBullet.pierceMinorBreakables = true;
            ExplosiveModifier explosiveModifier = LargeBullet.gameObject.GetOrAddComponent<ExplosiveModifier>();
            explosiveModifier.explosionData = new ExplosionData()
            {
                breakSecretWalls = false,
                comprehensiveDelay = 0,
                damage = 7,
                damageRadius = 2.5f,
                damageToPlayer = 0,
                debrisForce = 40,
                doDamage = true,
                doDestroyProjectiles = false,
                doExplosionRing = false,
                doForce = true,
                doScreenShake = false,
                doStickyFriction = false,
                effect = (PickupObjectDatabase.GetById(545) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect,
                explosionDelay = 0,
                force = 10,
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

            ImprovedAfterImage yes = LargeBullet.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.4f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0f, 1f, 1f, 1f);

            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 1,
            };
            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = LargeBullet,
                ChargeTime = 2f,
                AmmoCost = 2,
                
            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {

                item2,
                item3,
            };


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("CHARGER_AA", StaticCollections.Clip_Ammo_Atlas, "apoll_1", "apoll_2");

            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(223) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.25f, 0.125f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.25f, 0.125f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            GunID = gun.PickupObjectId;
        }
        public static int GunID;
    }
}