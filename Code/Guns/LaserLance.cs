
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;

namespace ModularMod
{
    public class LightLance : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Light Lance", "lightlance");
            Game.Items.Rename("outdated_gun_mods:light_lance", "mdl:armcannon_9");
            gun.gameObject.AddComponent<LightLance>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires simple energy balls. Compatible with Modular Upgrade Software.\n\nGiven the right circumstances, this piece of equipment would have been able to assist in many different ways with construction. An under-the-table deal weaponized it.");
            
            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "coollance_idle_001", 11);
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "lightlance_idle";
            gun.shootAnimation = "lightlance_fire";
            gun.reloadAnimation = "lightlance_reload";
            gun.introAnimation = "lightlance_intro";

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(57) as Gun).gunSwitchGroup;


            gun.reloadTime = 3f;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 100;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 0f;


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL; 


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.SetProjectileCollisionRight("defaultarmcannon_projectile_tiny_001", StaticCollections.Projectile_Collection, 2, 2, false, tk2dBaseSprite.Anchor.LowerCenter);

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
            projectile.gameObject.AddComponent<SpeedUp>();
            projectile.baseData.damage = 1f;
            projectile.baseData.speed = 0;
            projectile.shouldRotate = false;
            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.4f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0f, 1f, 1f, 1f);
            gun.gunClass = GunClass.NONE;

            var slash = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slash.DestroyBaseAfterFirstSlash = false;
            slash.slashParameters = new SlashData();
            slash.slashParameters.soundEvent = null;
            slash.slashParameters.projInteractMode = SlashDoer.ProjInteractMode.REFLECTANDPOSTPROCESS;
            slash.slashParameters.playerKnockbackForce = 10;
            slash.SlashDamageUsesBaseProjectileDamage = true;
            slash.slashParameters.enemyKnockbackForce = 20;
            slash.slashParameters.doVFX = true;
            slash.slashParameters.doHitVFX = true;
            slash.slashParameters.slashRange = 3f;



            tk2dSpriteAnimationClip fireClip2 = gun.spriteAnimator.GetClipByName("lightlance_fire");
            float[] offsetsX2 = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, //HERE
                0f, -0.75f, -1f, -0.5f, 0f, 0f, 0f, };
            float[] offsetsY2 = new float[] { 0f, 0.125f, 0.25f, 0.3125f, 0.3125f, 0.375f, 0.375f, -1.75f, -1.625f, -1.5f, -1.25f, -1.125f, -0.75f, -0.25f};
            for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
            {
                int id = fireClip2.frames[i].spriteId;
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
            }


            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(223) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "barrel_point").transform;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("lancebeam_", StaticCollections.Clip_Ammo_Atlas, "lancebeam_1", "lancebeam_2");


            

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
        }
        public static int ID;


        public class SpeedUp : MonoBehaviour
        {
            private Projectile self;
            private float elapsed = -1;

            public void Start()
            {
                self = this.GetComponent<Projectile>();
            }

            public void Update()
            {
                if (self && elapsed > 0 && elapsed < 2)
                {
                    self.UpdateSpeed();
                    self.baseData.speed += 15 * BraveTime.DeltaTime;
                }   
                elapsed += BraveTime.DeltaTime;
            }
        }
    }
}