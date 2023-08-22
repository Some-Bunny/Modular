
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;
using Alexandria.Assetbundle;

namespace ModularMod
{
    public class FlakCannon : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Shrapnel Launcher", "flakcannon");
            Game.Items.Rename("outdated_gun_mods:shrapnel_launcher", "mdl:armcannon_11");
            gun.gameObject.AddComponent<FlakCannon>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires chunks of completely waste material. Compatible with Modular Upgrade Software.\n\nA somewhat effective way to get rid of waste material that simply cannot be repurposed.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "flakcannon_idle_001", 11);
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "flakcannon_idle";
            gun.shootAnimation = "flakcannon_fire";
            gun.reloadAnimation = "flakcannon_reload";
            gun.introAnimation = "flakcannon_intro";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(19) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(19) as Gun).gunSwitchGroup;


            gun.reloadTime = 3.5f;
            gun.DefaultModule.cooldownTime = 1.25f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 7f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.shouldRotate = false;

            projectile.SetProjectileCollisionRight("flak_largeprojectile_001", StaticCollections.Projectile_Collection, 12, 12, false, tk2dBaseSprite.Anchor.MiddleCenter);
            UnityEngine.Object.Destroy(projectile.GetComponent<BounceProjModifier>());

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0.35f, 0.35f, 0.35f, 1f);

            if (projectile is GrenadeProjectile grenadeProjectile)
            {
                grenadeProjectile.startingHeight = 2;
            }


            var hhh = projectile.GetComponent<ExplosiveModifier>();
            var p = projectile.gameObject.AddComponent<ExplosiveModifier>().CopyFrom<ExplosiveModifier>(hhh);
            Destroy(hhh);
            p.explosionData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            p.explosionData.damage = 2;
            p.explosionData.damageToPlayer = 0;

            projectile.AnimateProjectileBundle("flakcannon", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "flakcannon",
            new List<IntVector2>() { new IntVector2(9, 9), new IntVector2(9, 9), new IntVector2(9, 9), new IntVector2(9, 9), new IntVector2(9, 9) },
            ProjectileToolbox.ConstructListOfSameValues(true, 5), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 5), ProjectileToolbox.ConstructListOfSameValues(true, 5), ProjectileToolbox.ConstructListOfSameValues(false, 5),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 5));


            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(37) as Gun).DefaultModule.chargeProjectiles[0].Projectile.objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(37) as Gun).DefaultModule.chargeProjectiles[0].Projectile.enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(37) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.tileMapVertical.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(37) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.tileMapVertical.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(37) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.tileMapVertical.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(37) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.tileMapVertical.effects.First().effects.First().effect);


            projectile.baseData.speed *= 1f;
            projectile.baseData.damage = 15f;
            projectile.shouldRotate = false;

            Material mat = new Material(StaticShaders.Default_Shader);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            projectile.sprite.renderer.material = mat;

            var shrapnelbolb = projectile.gameObject.AddComponent<SpawnProjModifier>();
            shrapnelbolb.fireRandomlyInAngle = true;
            shrapnelbolb.collisionSpawnStyle = SpawnProjModifier.CollisionSpawnStyle.FLAK_BURST;
            shrapnelbolb.PostprocessSpawnedProjectiles = true;
            shrapnelbolb.numberToSpawnOnCollison = 7;


            shrapnelbolb.spawnProjectilesOnCollision = true;
            shrapnelbolb.spawnCollisionProjectilesOnBounce = true;
            shrapnelbolb.spawnOnObjectCollisions = true;
            shrapnelbolb.UsesMultipleCollisionSpawnProjectiles = true;


            shrapnelbolb.collisionSpawnProjectiles = new Projectile[]
            {
                ReturnShrapnel("flak_projectile_002", new IntVector2(2,2 ), 5, 11),
                ReturnShrapnel("flak_projectile_003", new IntVector2(3,3 ), 6, 14),
                ReturnShrapnel("flak_projectile_004", new IntVector2(4,4 ), 6, 5),
                ReturnShrapnel("flak_projectile_005", new IntVector2(6,6 ), 7, 7),
            };

            gun.gunClass = GunClass.NONE;


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("FlakCannon_MDLR", StaticCollections.Clip_Ammo_Atlas, "scrapcube_1", "scrapcube_2");
            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);
            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(19) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.125f, 0.125f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.125f, 0.125f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            FlakCannon.GunID = gun.PickupObjectId;
        }

        public static Projectile ReturnShrapnel(string spriteName, IntVector2 size, float damage, float speed)
        {
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.SetProjectileCollisionRight(spriteName, StaticCollections.Projectile_Collection, size.x * 6, size.y* 6, false, tk2dBaseSprite.Anchor.MiddleCenter);

            Material mat = new Material(StaticShaders.Default_Shader);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            projectile.sprite.renderer.material = mat;

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0.35f, 0.35f, 0.35f, 1f);

            PierceProjModifier bounceProjModifier = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration = 1;

            projectile.baseData.damage = damage;
            projectile.baseData.range = (4 + (speed * 1.5f/ damage)) * .75f;
            projectile.baseData.speed = speed * 3f;
            return projectile;
        }

        public static int GunID;
    }
}