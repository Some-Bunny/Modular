
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using ModularMod.Code.Collectibles.Guns.Update_3;
using Alexandria.Misc;

namespace ModularMod
{
    public class Flamethrower : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Flame Ejector", "flamethrower");
            Game.Items.Rename("outdated_gun_mods:flame_ejector", "mdl:armcannon_13");
            var c = gun.gameObject.AddComponent<Flamethrower>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires super heated energy. Compatible with Modular Upgrade Software.\n\nA flamethrower. This is 100% a flamethrower, no-one can convince anyone else otherwise.");
            
            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "flamer_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "flamer_idle";
            gun.shootAnimation = "flamer_fire";
            gun.reloadAnimation = "flamer_reload";
            gun.introAnimation = "flamer_intro";
            gun.emptyAnimation = "flamer_empty";


            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.shootAnimation, new Dictionary<int, string> { { 0, "Play_OBJ_bomb_fuse_01" } });
            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(gun.GetComponent<tk2dSpriteAnimator>(), gun.reloadAnimation, new Dictionary<int, string> { { 2, "Play_WPN_brickgun_reload_01" } });


            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(336) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(125) as Gun).gunSwitchGroup;

            gun.reloadTime = 4.5f;
            gun.DefaultModule.cooldownTime = 0.04f;
            gun.DefaultModule.numberOfShotsInClip = 125;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 7.2f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.AnimateProjectileBundle("flamingfire", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "flamingfire",
            new List<IntVector2>() { new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), new IntVector2(30, 30), },
            ProjectileToolbox.ConstructListOfSameValues(true, 14), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 14), ProjectileToolbox.ConstructListOfSameValues(true, 14), ProjectileToolbox.ConstructListOfSameValues(false, 14),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 14), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 14), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 14), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 14));

            projectile.objectImpactEventName = null;//(PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };//Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };//Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };// Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX);
            projectile.hitEffects.deathAny = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0] };//Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX);
            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.EaseInOut(0, 1.1f, 0.75f, 0.35f);

            Material mat = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive"));
            mat.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            mat.SetFloat("_EmissivePower", 100);
            mat.SetFloat("_EmissiveColorPower", 100);
            projectile.sprite.renderer.material = mat;
            projectile.baseData.speed = 18.5f;
            projectile.baseData.damage = 0.45f;
            projectile.baseData.force = 2.2f;

            projectile.AppliesFire = true;
            projectile.FireApplyChance = 0.35f;
            projectile.fireEffect = DebuffStatics.hotLeadEffect;


            projectile.gameObject.AddComponent<MaintainDamageOnPierce>();
            PierceProjModifier pierceProjModifier = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            pierceProjModifier.penetration = 10000;

            BounceProjModifier bounceProjModifier = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier.numberOfBounces = 100;

            projectile.gameObject.AddComponent<FlamethrowerFire>();

            projectile.shouldRotate = true;
            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4,0);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(329) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.75f, 0.5625f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.75f, 0.5625f), "barrel_point").transform;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("flamethrowerMDLR", StaticCollections.Clip_Ammo_Atlas, "flamer_1", "flamer_2");

            gun.gameObject.transform.Find("Clip").transform.position = new Vector3(1.125f, 0.5f);
            gun.clipObject = Toolbox.GenerateDebrisObject("canister_clip", StaticCollections.Gun_Collection, true, 1, 3, 60, 20, null, 2, "Play_ITM_Crisis_Stone_Impact_02", null, 1).gameObject;
            gun.reloadClipLaunchFrame = 7;
            gun.clipsToLaunchOnReload = 1;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;
            IteratedDesign.SpecialProcessGunSpecific += c.Process;

        }

        public void Process(ModulePrinterCore modulePrinterCore, Projectile p, int stack, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != ID) { return; }
            p.baseData.damage += 0.25f * stack;
            p.baseData.speed *= 1 + (0.5f * stack);
            p.FireApplyChance = 0.1f * stack;
            p.baseData.force += stack;
            p.fireEffect = DebuffStatics.greenFireEffect;
            p.AdjustPlayerProjectileTint(new Color(0, 1, 0, 1), 10);
        }
        public static int ID;
    }
}