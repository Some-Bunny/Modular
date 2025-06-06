
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections;

namespace ModularMod
{
    public class DefaultArmCannon : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Modular Arm Cannon", "defaultarmcannon");
            Game.Items.Rename("outdated_gun_mods:modular_arm_cannon", "mdl:armcannon_0");
            var c = gun.gameObject.AddComponent<DefaultArmCannon>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires simple energy balls. Compatible with Modular Upgrade Software.\n\nGiven the right circumstances, this piece of equipment would have been able to assist in many different ways with construction. An under-the-table deal weaponized it.");
            
            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "defaultarmcannon_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "defaultarmcannon_idle";
            gun.shootAnimation = "defaultarmcannon_fire";
            gun.reloadAnimation = "defaultarmcannon_reload";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(57) as Gun).gunSwitchGroup;


            gun.reloadTime = 2.25f;
            gun.DefaultModule.cooldownTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 7.5f;


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.SetProjectileCollisionRight("defaultarmcannon_projectile_medium_001", StaticCollections.Projectile_Collection, 6, 6, false, tk2dBaseSprite.Anchor.MiddleCenter);

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

            projectile.baseData.damage = 7f;
            projectile.shouldRotate = false;
            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(223) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "barrel_point").transform;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("ArmCannon", StaticCollections.Clip_Ammo_Atlas, "blaster_1", "blaster_2");

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;

            IteratedDesign.SpecialProcessGunSpecificFireRate += c.ProcessFireRateSpecial;
            IteratedDesign.SpecialProcessGunSpecific += c.ProcessShot;
        }


        public static VFXPool Impact = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy;
        public static string ImpactSFX = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.enemyImpactEventName;

        public void ProcessShot(ModulePrinterCore modulePrinterCore, Projectile p, int stack, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != ID) { return; }
            p.AppliesStun = true;
            p.StunApplyChance = 0.02f;
            p.AppliedStunDuration = 2.25f;



            p.TreatedAsNonProjectileForChallenge = true;
            p.specRigidbody.OnPreRigidbodyCollision += OPC;
        }

        public void OPC(SpeculativeRigidbody mR, PixelCollider mP, SpeculativeRigidbody oR, PixelCollider oP)
        {
            if (oR.aiActor != null && oR.healthHaver != null && mR.projectile != null)
            {
                if (oR.aiActor.behaviorSpeculator && oR.aiActor.behaviorSpeculator.IsStunned)
                {
                    float damage = mR.projectile.baseData.damage;
                    float force = mR.projectile.baseData.force;
                    mR.projectile.baseData.damage *= 1.5f + ((float)IteratedDesign.PlayerHasIteratedDesign(Toolbox.GetModular()) * 0.5f);
                    mR.projectile.baseData.force *= 1.5f + ((float)IteratedDesign.PlayerHasIteratedDesign(Toolbox.GetModular()) * 0.5f);
                    mR.projectile.StartCoroutine(FrameDelay(mR.projectile, damage, force));
                }
            }
        }

        public IEnumerator FrameDelay(Projectile p, float DmG, float forec)
        {
            VFXPool Ef = p.hitEffects.enemy;
            VFXPool Ef_ = p.hitEffects.deathEnemy;

            string hit = p.enemyImpactEventName;
            
            p.hitEffects.enemy = Impact;
            p.hitEffects.deathEnemy = Impact;

            p.enemyImpactEventName = ImpactSFX;

            yield return null;
            if (p)
            {
                p.baseData.damage = DmG;
                p.hitEffects.enemy = Ef;
                p.hitEffects.deathEnemy = Ef_;
                p.enemyImpactEventName = hit;
                p.baseData.damage = DmG;
                p.baseData.force = forec;
            }
        }

        public float ProcessFireRateSpecial(float f, int stack, ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != ID) { return f; }
            return f / (1 + (stack / 5));
        }


        public static int ID;
    }
}