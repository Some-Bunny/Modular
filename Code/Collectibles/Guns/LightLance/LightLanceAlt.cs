
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using ModularMod.Code.Toolboxes;
using static UnityEngine.UI.Image;

namespace ModularMod
{
    public class LightLanceAlt : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Light Lance", "lightlancealt");
            Game.Items.Rename("outdated_gun_mods:light_lance", "mdl:armcannon_9_alt");
            var c = gun.gameObject.AddComponent<LightLance>();
            gun.SetShortDescription("Mk.2");
            gun.SetLongDescription("Slashes enemies, can be charged to do a deflecting attack with an energy projectile.\n\nA close-combat weapon. In the hands of a machine with a fast enough camera, it can fulfill the dream of every person with a samurai sword.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "coollancealt_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "lightlancealt_idle";
            gun.shootAnimation = "lightlancealt_fire";
            gun.reloadAnimation = "lightlancealt_reload";
            gun.introAnimation = "lightlancealt_intro";
            gun.chargeAnimation = "lightlancealt_charge";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = true;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(57) as Gun).gunSwitchGroup;


            gun.reloadTime = 3f;
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 0f;


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;


            Projectile dudProjectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            dudProjectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(dudProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(dudProjectile);
            gun.DefaultModule.projectiles[0] = dudProjectile;
            dudProjectile.baseData.damage = 0f;
            dudProjectile.baseData.speed = 1;
            dudProjectile.sprite.renderer.enabled = false;
            dudProjectile.baseData.range = 1;
            dudProjectile.hitEffects.suppressMidairDeathVfx = true;
            ProjectileSlashingBehaviour projectileSlashingBehaviour = dudProjectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            projectileSlashingBehaviour.DestroyBaseAfterFirstSlash = true;
            projectileSlashingBehaviour.SlashDamageUsesBaseProjectileDamage = true;
            projectileSlashingBehaviour.slashParameters = ScriptableObject.CreateInstance<SlashData>();
            projectileSlashingBehaviour.slashParameters.hitVFX = (PickupObjectDatabase.GetById(345) as Gun).DefaultModule.projectiles[0].hitEffects.enemy;
            projectileSlashingBehaviour.slashParameters.projInteractMode = SlashDoer.ProjInteractMode.IGNORE;
            projectileSlashingBehaviour.slashParameters.playerKnockbackForce = 20;
            //slash.SlashDamageUsesBaseProjectileDamage = false;
            projectileSlashingBehaviour.slashParameters.enemyKnockbackForce = 30;
            projectileSlashingBehaviour.slashParameters.doVFX = true;
            projectileSlashingBehaviour.slashParameters.doHitVFX = true;
            projectileSlashingBehaviour.slashParameters.slashRange = 3.75f;
            projectileSlashingBehaviour.slashParameters.slashDegrees = 150;
            projectileSlashingBehaviour.slashParameters.soundEvent = "Play_WPN_beam_slash_01";
            projectileSlashingBehaviour.SlashDamageUsesBaseProjectileDamage = false;
            projectileSlashingBehaviour.initialDelay = 0.1f;
            //projectileSlashingBehaviour.slashParameters.CustomReflectProjectile += OnDoSlash;
            projectileSlashingBehaviour.slashParameters.damage = 25f;
            projectileSlashingBehaviour.slashParameters.damagesBreakables = true;


            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = dudProjectile,
                ChargeTime = 0f,
                AmmoCost = 1,
            };


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

            projectile.AnimateProjectileBundle("swordslashalt", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "swordslashalt",
            new List<IntVector2>() { new IntVector2(20, 64), new IntVector2(20, 64), new IntVector2(20, 64), new IntVector2(20, 64), new IntVector2(20, 64), new IntVector2(20, 64), new IntVector2(20, 64), new IntVector2(20, 64), new IntVector2(20, 64), },
            ProjectileToolbox.ConstructListOfSameValues(true, 9), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 9), ProjectileToolbox.ConstructListOfSameValues(true, 9), ProjectileToolbox.ConstructListOfSameValues(false, 9),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 9));

            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            projectile.hitEffects.alwaysUseMidair = true;
            projectile.hitEffects.overrideMidairDeathVFX = (PickupObjectDatabase.GetById(545) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect;
            projectile.AdditionalScaleMultiplier *= 0.75f;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 50);
            mat.SetFloat("_EmissivePower", 50);
            projectile.sprite.renderer.material = mat;
            projectile.gameObject.AddComponent<LightLance.SpeedUp>();
            projectile.baseData.damage = 16f;
            projectile.baseData.speed = 1;
            projectile.shouldRotate = true;
            projectile.baseData.range = 9;
            projectile.PenetratesInternalWalls = true;

            CustomProjectileSlashingBehaviour projectileSlashingBehaviour2 = projectile.gameObject.AddComponent<CustomProjectileSlashingBehaviour>();
            projectileSlashingBehaviour2.DestroyBaseAfterFirstSlash = true;
            projectileSlashingBehaviour2.SlashDamageUsesBaseProjectileDamage = true;
            projectileSlashingBehaviour2.DestroysOnlyComponentAfterFirstSlash = true;
            projectileSlashingBehaviour2.SlashDamageUsesBaseProjectileDamage = false;
            projectileSlashingBehaviour2.initialDelay = 0.05f;

            var slash = ScriptableObject.CreateInstance<SpecialLaserSlash>();
            slash.projInteractMode = CustomSlashDoer.ProjInteractMode.REFLECTANDPOSTPROCESS;
            slash.playerKnockbackForce = 20;
            slash.enemyKnockbackForce = 70;
            slash.doVFX = false;
            slash.doHitVFX = true;
            slash.slashRange = 4.25f;
            slash.slashDegrees = 120;
            slash.soundEvent = "Play_WPN_beam_slash_01";
            slash.damage = 45;
            slash.damagesBreakables = true;
            slash.hitVFX = (PickupObjectDatabase.GetById(345) as Gun).DefaultModule.projectiles[0].hitEffects.enemy;
            customSlash = slash;

            projectileSlashingBehaviour2.slashParameters = customSlash;

            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.1f;
            yes.dashColor = new Color(0f, 0.9f, 0f, 0.666f);
            gun.gunClass = GunClass.NONE;

            PierceProjModifier bounceProjModifier = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += 3;

            projectile.pierceMinorBreakables = true;

            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                OverrideShootAnimation = "lightlancealt_altfire",
                ChargeTime = 1,
                AmmoCost = 2,
                UsedProperties = ProjectileModule.ChargeProjectileProperties.shootAnim | ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent,
                AdditionalWwiseEvent = "Play_BOSS_agunim_orb_01"
            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item2,
                item3,
            };


            tk2dSpriteAnimationClip fireClip2 = gun.spriteAnimator.GetClipByName("lightlancealt_fire");
            float[] offsetsX2 = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, -0.75f, -0.75f, -1f, -0.5f, -0.25f, 0f, 0f, 0f, };
            float[] offsetsY2 = new float[] { 0f, 0.125f, 0.25f, 0.375f, -0.75f, -0.75f, -0.75f, -0.6875f, -0.6875f, -0.5625f, -0.5625f, -0.4375f, -0.375f, -0.25f, -0.125f };
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

            tk2dSpriteAnimationClip chargeClip = gun.spriteAnimator.GetClipByName("lightlancealt_charge");
            offsetsX2 = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            offsetsY2 = new float[] { 0f, 0f, 0.125f, 0.25f, 0.25f, 0.375f, 0.5f };
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


            tk2dSpriteAnimationClip reloadClip = gun.spriteAnimator.GetClipByName("lightlancealt_reload");
            offsetsX2 = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            offsetsY2 = new float[] { 0f, -0.25f, -0.5f, -0.5f, -0.5f, 0, };
            for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < reloadClip.frames.Length; i++)
            {
                int id = reloadClip.frames[i].spriteId;
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
                reloadClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
            }

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(125) as Gun).gunSwitchGroup;
            gun.AddGlowShaderToGun(new Color32(0, 255, 54, 255), 3, 3);
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.carryPixelOffset = new IntVector2(4, 2);

            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };

            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.125f, -0f), "barrel_point").transform;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("lancebeam_alt", StaticCollections.Clip_Ammo_Atlas, "lancebeamalt_1", "lancebeamalt_2");

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
            IteratedDesign.SpecialProcessGunSpecificFireRate += c.ProcessFireRateSpecial;
        }

        public float ProcessFireRateSpecial(float f, int stack, ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != ID) { return f; }
            return f / (1 + (stack / 4));
        }



        public static CustomSlashData customSlash;
        public static int ID;

        public class SpecialLaserSlash : CustomSlashData
        {
            public override CustomSlashData ReturnClone()
            {

                SpecialLaserSlash newData = ScriptableObject.CreateInstance<SpecialLaserSlash>();
                newData.doVFX = this.doVFX;
                newData.VFX = this.VFX;
                newData.doHitVFX = this.doHitVFX;
                newData.hitVFX = this.hitVFX;
                newData.projInteractMode = this.projInteractMode;
                newData.playerKnockbackForce = this.playerKnockbackForce;
                newData.enemyKnockbackForce = this.enemyKnockbackForce;
                newData.statusEffects = this.statusEffects;
                newData.jammedDamageMult = this.jammedDamageMult;
                newData.bossDamageMult = this.bossDamageMult;
                newData.doOnSlash = this.doOnSlash;
                newData.doPostProcessSlash = this.doPostProcessSlash;
                newData.slashRange = this.slashRange;
                newData.slashDegrees = this.slashDegrees;
                newData.damage = this.damage;
                newData.damagesBreakables = this.damagesBreakables;
                newData.soundEvent = this.soundEvent;
                newData.OnHitTarget = this.OnHitTarget;
                newData.OnHitBullet = this.OnHitBullet;
                newData.OnHitMinorBreakable = this.OnHitMinorBreakable;
                newData.OnHitMajorBreakable = this.OnHitMajorBreakable;
                return newData;
            }

            public override void OnProjectileReflect(Projectile p, bool retargetReflectedBullet, GameActor newOwner, float minReflectedBulletSpeed, bool doPostProcessing = false, float scaleModifier = 1, float baseDamage = 10, float spread = 0, string sfx = null)
            {
                AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", p.gameObject);

                p.RemoveBulletScriptControl();

                if ((bool)p.Owner && (bool)p.Owner.specRigidbody)
                {
                    p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
                }

                p.Owner = newOwner;
                p.SetNewShooter(newOwner.specRigidbody);
                p.allowSelfShooting = false;
                if (newOwner is AIActor)
                {
                    p.collidesWithPlayer = true;
                    p.collidesWithEnemies = false;
                }
                else if (newOwner is PlayerController)
                {
                    p.collidesWithPlayer = false;
                    p.collidesWithEnemies = true;
                }
                SpawnManager.PoolManager.Remove(p.transform);
                float previousSpeed = p.baseData.speed;

                p.baseData.damage = 1.25f + (p.baseData.speed * 0.3f);

                p.baseData.speed *= 2;
                p.UpdateSpeed();
                if (newOwner is PlayerController)
                {
                    PlayerController playerController = newOwner as PlayerController;
                    if (playerController != null)
                    {


                        var controller = playerController.CurrentGun.GetComponent<ModularGunController>();
                        if (controller != null)
                        {
                            float speedMath = Mathf.Max(0, 90 - (previousSpeed * 3f));
                            p.Direction = Toolbox.GetUnitOnCircle(playerController.CurrentGun.CurrentAngle + UnityEngine.Random.Range(controller.GetAccuracy(speedMath), controller.GetAccuracy(speedMath * -1)), 1);
                            var VFX = UnityEngine.Object.Instantiate(controller.isAlt == true ? ConvexLens.greenImpact.effects[0].effects[0].effect : LineUp.PierceImpact, p.sprite.WorldCenter - new Vector2(1.5f, 0), Quaternion.identity);
                            Destroy(VFX, 2);
                        }
                        p.baseData.damage *= playerController.stats.GetStatValue(PlayerStats.StatType.Damage);
                        p.baseData.speed *= playerController.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                        p.UpdateSpeed();
                        p.baseData.force *= playerController.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                        p.baseData.range *= playerController.stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
                        p.BossDamageMultiplier *= playerController.stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
                        p.RuntimeUpdateScale(playerController.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale));
                        playerController.DoPostProcessProjectile(p);
                    }
                }

                if (newOwner is AIActor)
                {
                    p.baseData.damage = 0.5f;
                    p.baseData.SetAll((newOwner as AIActor).bulletBank.GetBullet().ProjectileData);
                    p.specRigidbody.CollideWithTileMap = false;
                    p.ResetDistance();
                    p.collidesWithEnemies = (newOwner as AIActor).CanTargetEnemies;
                    p.collidesWithPlayer = true;
                    p.UpdateCollisionMask();
                    p.sprite.color = new Color(1f, 0.1f, 0.1f);
                    p.MakeLookLikeEnemyBullet();
                    p.RemovePlayerOnlyModifiers();
                    if ((newOwner as AIActor).IsBlackPhantom)
                    {
                        p.baseData.damage = 1f;
                        p.BecomeBlackBullet();
                    }
                }

                p.UpdateCollisionMask();
                p.Reflected();
                p.SendInDirection(p.Direction, resetDistance: true);
            }
        }
    }
}