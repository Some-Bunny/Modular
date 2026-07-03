
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;

namespace ModularMod
{
    public class ChargeBlasterAlt : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Charge Blaster", "energychargeralt");
            Game.Items.Rename("outdated_gun_mods:charge_blaster", "mdl:armcannon_4_alt");
            var c = gun.gameObject.AddComponent<ChargeBlasterAlt>();
            gun.SetShortDescription("Mk.2");
            gun.SetLongDescription("Fires weak energy pellets, can be charged up for a strong attack. Compatible with Modular Upgrade Software.\n\nDraws a lot of power to fire.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "energychargeralt_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "energychargeralt_idle";
            gun.shootAnimation = "energychargeralt_fire";
            gun.reloadAnimation = "energychargeralt_reload";
            gun.introAnimation = "energychargeralt_intro";
            gun.chargeAnimation = "energychargeralt_charge";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(57) as Gun, true, false);


            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = true;
            comp.AdditionalPowerSupply = 0;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(41) as Gun).gunSwitchGroup;


            gun.reloadTime = 2.1f;
            gun.DefaultModule.cooldownTime = .2f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 3f;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;



            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.SetProjectileCollisionRight("defaultarmcannonalt_projectile_001", StaticCollections.Projectile_Collection, 4, 4, false, tk2dBaseSprite.Anchor.LowerCenter);
            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            
            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(207) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(207) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(207) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(207) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectile.sprite.renderer.material = mat;
            projectile.baseData.speed = 30f;
            projectile.baseData.damage = 3f;
            projectile.shouldRotate = false;
            projectile.baseData.force *= 10;


            ///===========================================================================
            ///===========================================================================
            ///===========================================================================

            Projectile projectileMed = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectileMed.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectileMed.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectileMed);
            gun.DefaultModule.projectiles[0] = projectileMed;
            projectileMed.SetProjectileCollisionRight("defaultarmcannonalt_projectile_burst_001", StaticCollections.Projectile_Collection, 11, 4, false, tk2dBaseSprite.Anchor.LowerCenter);
            projectileMed.objectImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectileMed.enemyImpactEventName = (PickupObjectDatabase.GetById(334) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            projectileMed.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectileMed.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectileMed.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectileMed.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            Material mat_1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat_1.mainTexture = projectileMed.sprite.renderer.material.mainTexture;
            mat_1.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat_1.SetFloat("_EmissiveColorPower", 100);
            mat_1.SetFloat("_EmissivePower", 100);
            projectileMed.sprite.renderer.material = mat_1;
            projectileMed.baseData.speed = 40f;
            projectileMed.baseData.damage = 16f;
            projectileMed.shouldRotate = true;
            projectileMed.baseData.force *= 4;

            ImprovedAfterImage aaaa = projectileMed.gameObject.AddComponent<ImprovedAfterImage>();
            aaaa.spawnShadows = true;
            aaaa.shadowLifetime = 0.25f;
            aaaa.shadowTimeDelay = 0.1f;
            aaaa.dashColor = new Color(0f, 1f, 0.1f, 1f);

            PierceProjModifier bounceProjModifier = projectileMed.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration = 1;

            ///===========================================================================
            ///===========================================================================
            ///===========================================================================

            Projectile LargeBullet = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            LargeBullet.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(LargeBullet.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(LargeBullet);

            LargeBullet.AnimateProjectileBundle("modblasteralt_dile", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "modblasteralt_dile",
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
            LargeBullet.baseData.damage = 27f;
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
            yes.shadowTimeDelay = 0.04f;
            yes.dashColor = new Color(0f, 1f, 0.1f, 1f);

            Projectile MegaBullet = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            MegaBullet.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(MegaBullet.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(MegaBullet);

            MegaBullet.AnimateProjectileBundle("giantenergyalt", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "giantenergyalt",
            new List<IntVector2>() { new IntVector2(51, 19), new IntVector2(51, 19), new IntVector2(51, 19), new IntVector2(51, 19), new IntVector2(51, 19), new IntVector2(51, 19), },
            ProjectileToolbox.ConstructListOfSameValues(true, 6), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 6), ProjectileToolbox.ConstructListOfSameValues(true, 6), ProjectileToolbox.ConstructListOfSameValues(false, 6),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 6), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 6), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 6), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 6));

            MegaBullet.objectImpactEventName = (PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            MegaBullet.enemyImpactEventName = (PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;
            MegaBullet.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            MegaBullet.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            MegaBullet.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            MegaBullet.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(180) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            Material matTuah = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            matTuah.mainTexture = projectile.sprite.renderer.material.mainTexture;
            matTuah.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            matTuah.SetFloat("_EmissiveColorPower", 100);
            matTuah.SetFloat("_EmissivePower", 100);
            MegaBullet.sprite.renderer.material = matTuah;
            MegaBullet.baseData.speed = 65;
            MegaBullet.baseData.damage = 45f;
            MegaBullet.shouldRotate = true;
            MegaBullet.pierceMinorBreakables = true;

            bounceProjModifier = MegaBullet.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration = 3;
            bounceProjModifier.penetratesBreakables = true;

            var tro_1 = MegaBullet.gameObject.AddChild("trail object");
            tro_1.transform.position = MegaBullet.sprite.WorldTopLeft + new Vector2(0.625f, -0.375f);// + new Vector2(.25f, 0.3125f);
            tro_1.transform.localPosition = MegaBullet.sprite.WorldTopLeft + new Vector2(0.625f, -0.375f);
            TrailRenderer tr_1 = tro_1.AddComponent<TrailRenderer>();
            tr_1.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr_1.receiveShadows = false;
            var mat_Trail = new Material(Shader.Find("Sprites/Default"));
            tr_1.material = mat_Trail;
            tr_1.minVertexDistance = 0.01f;
            tr_1.numCapVertices = 640;

            //======
            UnityEngine.Color color = new UnityEngine.Color(0, 2, 0, 2);
            mat_1.SetColor("_Color", color);
            tr_1.startColor = color;
            tr_1.endColor = new Color(0f, 1, 0, 0.5f);
            //======
            tr_1.time = 0.3125f;
            //======
            tr_1.startWidth = 0.5f;
            tr_1.endWidth = 0f;
            tr_1.autodestruct = false;

            var rend = MegaBullet.gameObject.AddComponent<ProjectileTrailRendererController>();
            rend.trailRenderer = tr_1;
            rend.desiredLength = 6;

            var shrapnelbolb = MegaBullet.gameObject.AddComponent<SpawnProjModifier>();
            shrapnelbolb.fireRandomlyInAngle = true;
            shrapnelbolb.collisionSpawnStyle = SpawnProjModifier.CollisionSpawnStyle.RADIAL;
            shrapnelbolb.PostprocessSpawnedProjectiles = true;
            shrapnelbolb.numberToSpawnOnCollison = 8;
            shrapnelbolb.spawnProjectilesOnCollision = true;
            shrapnelbolb.spawnCollisionProjectilesOnBounce = true;
            shrapnelbolb.spawnOnObjectCollisions = true;
            shrapnelbolb.UsesMultipleCollisionSpawnProjectiles = true;


            shrapnelbolb.collisionSpawnProjectiles = new Projectile[]
            {
                projectile,
                projectileMed
            };

            explosiveModifier = MegaBullet.gameObject.GetOrAddComponent<ExplosiveModifier>();
            explosiveModifier.explosionData = new ExplosionData()
            {
                breakSecretWalls = false,
                comprehensiveDelay = 0,
                damage = 20,
                damageRadius = 3f,
                damageToPlayer = 0,
                debrisForce = 100,
                doDamage = true,
                doDestroyProjectiles = false,
                doExplosionRing = false,
                doForce = true,
                doScreenShake = false,
                doStickyFriction = false,
                effect = (PickupObjectDatabase.GetById(545) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect,
                explosionDelay = 0,
                force = 30,
                forcePreventSecretWallDamage = false,
                forceUseThisRadius = true,
                freezeEffect = null,
                freezeRadius = 0,
                IsChandelierExplosion = false,
                isFreezeExplosion = false,
                playDefaultSFX = true,
                preventPlayerForce = false,
                pushRadius = 3,
                secretWallsRadius = 1,
            };
            explosiveModifier.doExplosion = true;
            explosiveModifier.IgnoreQueues = true;



            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 1,
            };

            ProjectileModule.ChargeProjectile emdium = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectileMed,
                ChargeTime = 0.8f,
                AmmoCost = 2,

            };
            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = LargeBullet,
                ChargeTime = 1.6f,
                AmmoCost = 2,
            };

            Mega = MegaBullet;

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item2,
                emdium,
                item3,
            };

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("CHARGER_AAALT", StaticCollections.Clip_Ammo_Atlas, "art_1", "art_2");

            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(0, 255, 54, 255), 3, 3);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(151) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.25f, 0.125f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.25f, 0.125f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            GunID = gun.PickupObjectId;

            IteratedDesign.SpecialProcessFirstPickup += (modCore, i, Play) =>
            {
                (modCore as ModulePrinterCore).ModularGunController.gun.Volley.projectiles[0].chargeProjectiles.Add(new ProjectileModule.ChargeProjectile()
                {
                    Projectile = Mega,
                    ChargeTime = 2.4f,
                    AmmoCost = 2,
                    OverrideMuzzleFlashVfxPool = (PickupObjectDatabase.GetById(387) as Gun).muzzleFlashEffects,
                    AdditionalWwiseEvent = "Play_SentryRailgun",
                    UsedProperties = ProjectileModule.ChargeProjectileProperties.muzzleFlash | ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent

                });
            };
            IteratedDesign.SpecialProcessUnequip += (modCore, i, Play) =>
            {
                (modCore as ModulePrinterCore).ModularGunController.gun.Volley.projectiles[0].chargeProjectiles.RemoveAt(3);
            };
            IteratedDesign.OverrideAdditionalDescription += (text, gunID) =>
            {
                if (gunID == GunID)
                {
                    return text + $"\n{StaticColorHexes.AddColorToLabelString("Bonus Effect:", StaticColorHexes.Green_Hex)} Adds an extra powerful 4th charge tier.";
                }
                return text;
            };

        }
        private static Projectile Mega;
        public static int GunID;
    }
}