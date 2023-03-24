
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;

namespace ModularMod
{
    public class ScatterBlastAlt : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Scatter Blast", "scattercannonalt");
            Game.Items.Rename("outdated_gun_mods:scatter_blast", "mdl:armcannon_2_alt");
            gun.gameObject.AddComponent<ScatterBlastAlt>();
            gun.SetShortDescription("Mk.2");
            gun.SetLongDescription("Fires an energy spread. Compatible with Modular Upgrade Software.\n\nA very unusual modification of a paint gun. *Very* unusual.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "scattercannonalt_idle_001", 11);
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "scattercannonalt_idle";
            gun.shootAnimation = "scattercannonalt_fire";
            gun.reloadAnimation = "scattercannonalt_reload";
            gun.introAnimation = "scattercannonalt_intro";

            for (int i = 0; i < 6; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, true);
            }

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = true;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(57) as Gun).gunSwitchGroup;


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
                    projectile.SetProjectileCollisionRight("defaultarmcannonalt_projectile_medium_001", StaticCollections.Projectile_Collection, 6, 6, false, tk2dBaseSprite.Anchor.LowerCenter);
                    projectile.baseData.range = 10;
                    projectile.baseData.damage = 6f;
                }
                else
                {
                    projectile.SetProjectileCollisionRight("defaultarmcannonalt_projectile_001", StaticCollections.Projectile_Collection, 4, 4, false, tk2dBaseSprite.Anchor.LowerCenter);
                    projectile.baseData.range = 6;
                    projectile.baseData.damage = 3f;
                }
                projectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
                projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

                int gunIDProj = 207;
                projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(gunIDProj) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(gunIDProj) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(gunIDProj) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
                projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(gunIDProj) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

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

            gun.AddGlowShaderToGun(new Color32(0, 255, 54, 255), 3, 3);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(6, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(207) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.375f, 0.1875f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.375f, 0.1875f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
        }
        public static int ID;
    }
}