
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;

namespace ModularMod
{
    public class BigNukeAlt : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Kinetic Payload", "bignukegunalt");
            Game.Items.Rename("outdated_gun_mods:kinetic_payload", "mdl:armcannon_12_alt");
            gun.gameObject.AddComponent<BigNukeAlt>();
            gun.SetShortDescription("Mk.2");
            gun.SetLongDescription("Fires kinetic warheads. Compatible with Modular Upgrade Software.\n\nComplete overkill, in the palm of your hand.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "bigbombalt_idle_004", 11);
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "bignukealt_idle";
            gun.shootAnimation = "bignukealt_fire";
            gun.reloadAnimation = "bignukealt_reload";
            gun.introAnimation = "bignukealt_intro";
            gun.emptyAnimation = "bignukealt_clipempty";

            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = true;
            comp.AdditionalPowerSupply = -2;


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(39) as Gun).gunSwitchGroup;


            gun.reloadTime = 8f;
            gun.DefaultModule.cooldownTime = 5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 4f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.SetProjectileCollisionRight("bombbombalt_001", StaticCollections.Projectile_Collection, 17, 7, false, tk2dBaseSprite.Anchor.MiddleCenter);

            projectile.AnimateProjectileBundle("bombombalt", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "bombombalt",
            new List<IntVector2>() { new IntVector2(17, 7), new IntVector2(17, 7), new IntVector2(17, 7), new IntVector2(17, 7), new IntVector2(17, 7)},
            ProjectileToolbox.ConstructListOfSameValues(true, 5), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 5), ProjectileToolbox.ConstructListOfSameValues(true, 5), ProjectileToolbox.ConstructListOfSameValues(false, 5),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 5));

            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(387) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(387) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(390) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(390) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(390) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(390) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy.effects.First().effects.First().effect);

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.4f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0f, 1f, 0f, 1f);

            var homing = projectile.gameObject.GetOrAddComponent<HomingModifier>();
            homing.AngularVelocity = 60;
            homing.HomingRadius = 20;


            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectile.sprite.renderer.material = mat;
            projectile.baseData.speed *= 0.35f;
            projectile.baseData.damage = 60f;
            projectile.shouldRotate = true;

            projectile.gameObject.AddComponent<KineticBomb>();
            var explosive = projectile.gameObject.AddComponent<ExplosiveModifier>();
            explosive.explosionData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            explosive.explosionData.damage = 45;
            explosive.explosionData.damageToPlayer = 0;
            explosive.explosionData.damageRadius = 7;
            explosive.explosionData.forceUseThisRadius = true;

            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0f, 0.3f, 1, 1.5f);
            projectile.baseData.CustomAccelerationCurveDuration = 2.5f;
            gun.gunClass = GunClass.NONE;


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("BigNukeAlt_Modular", StaticCollections.Clip_Ammo_Atlas, "bignukealt_1", "bignukealt_2");

            gun.AddGlowShaderToGun(new Color32(0, 255, 54, 255), 3, 3);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(151) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.125f, 0.125f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.125f, 0.125f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            BigNukeAlt.GunID = gun.PickupObjectId;
        }
        public static int GunID;
    }
}