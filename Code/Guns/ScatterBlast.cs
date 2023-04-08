
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;

namespace ModularMod
{
    public class ScatterBlast : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Scatter Blast", "scattercannon");
            Game.Items.Rename("outdated_gun_mods:scatter_blast", "mdl:armcannon_2");
            gun.gameObject.AddComponent<ScatterBlast>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires an energy spread. Compatible with Modular Upgrade Software.\n\nA very unusual modification of a paint gun. *Very* unusual.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "scattercannon_idle_001", 11);
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "scattercannon_idle";
            gun.shootAnimation = "scattercannon_fire";
            gun.reloadAnimation = "scattercannon_reload";
            gun.introAnimation = "scattercannon_intro";

            for (int i = 0; i < 6; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, true);
            }

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(123) as Gun).gunSwitchGroup;


            gun.reloadTime = 2.75f;
            gun.SetBaseMaxAmmo(250);


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;

            int q = 0;
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                q++;
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 0.6f;
                projectileModule.angleVariance = 21;
                projectileModule.numberOfShotsInClip = 8;


                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectileModule.projectiles[0] = projectile;
                if (q == 1 | q == 2)
                {
                    projectile.SetProjectileCollisionRight("defaultarmcannon_projectile_medium_001", StaticCollections.Projectile_Collection, 6, 6, false, tk2dBaseSprite.Anchor.LowerCenter);
                    projectile.baseData.range = 13;
                    projectile.baseData.damage = 6f;
                }
                else
                {
                    projectile.SetProjectileCollisionRight("defaultarmcannon_projectile_001", StaticCollections.Projectile_Collection, 4, 4, false, tk2dBaseSprite.Anchor.LowerCenter);
                    projectile.baseData.range = 8;
                    projectile.baseData.damage = 3f;
                }
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

                projectile.shouldRotate = false;

                if (projectileModule != gun.DefaultModule)
                {
                    projectileModule.ammoCost = 0;
                }
            }

            gun.Volley.UsesShotgunStyleVelocityRandomizer = true;
            //gun.Volley.DecreaseFinalSpeedPercentMin = 0.66f;
            //un.Volley.IncreaseFinalSpeedPercentMax = 1.5f;


            gun.gunClass = GunClass.SHOTGUN;

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("SHOTGUNSAASASA", StaticCollections.Clip_Ammo_Atlas, "shotgunpellet_1", "shotgunpellet_2");



            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(223) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.375f, 0.1875f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.375f, 0.1875f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
        }
        public static int ID;
    }
}