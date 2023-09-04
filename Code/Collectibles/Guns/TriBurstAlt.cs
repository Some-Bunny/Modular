
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;

namespace ModularMod
{
    public class TriBurstAlt : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Quad-Burster", "bursteralt");
            Game.Items.Rename("outdated_gun_mods:quadburster", "mdl:armcannon_3_alt");
            gun.gameObject.AddComponent<TriBurstAlt>();
            gun.SetShortDescription("Mk.2");
            gun.SetLongDescription("Fires energy balls in a short burst. Compatible with Modular Upgrade Software.\n\nOne of the first to be designed purely for combat, instead of being a repurposed tool.");
            
            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "shotbursteralt_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "shotbursteralt_idle";
            gun.shootAnimation = "shotbursteralt_fire";
            gun.reloadAnimation = "shotbursteralt_reload";
            gun.introAnimation = "shotbursteralt_intro";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = true;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.burstShotCount = 4;
            gun.DefaultModule.burstCooldownTime = 0.05f;



            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(89) as Gun).gunSwitchGroup;


            gun.reloadTime = 3f;
            gun.DefaultModule.cooldownTime = .833f;
            gun.DefaultModule.numberOfShotsInClip = 16;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 5f;
           


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.SetProjectileCollisionRight("defaultarmcannonalt_projectile_burst_001", StaticCollections.Projectile_Collection, 11, 4, false, tk2dBaseSprite.Anchor.MiddleCenter);

            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects = (PickupObjectDatabase.GetById(89) as Gun).DefaultModule.projectiles[0].hitEffects;
            projectile.shouldRotate = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectile.sprite.renderer.material = mat;
            projectile.baseData.speed *= 1.33f;


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("burstingmakesmefeelgoodalt", StaticCollections.Clip_Ammo_Atlas, "bursteralt_1", "bursteralt_2");

            projectile.baseData.damage = 5.5f;
            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(0, 255, 54, 255), 3, 3);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(89) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "barrel_point").transform;

            ID = ETGMod.Databases.Items.Add(gun, false, "ANY");
        }
        public static int ID;
    }
}