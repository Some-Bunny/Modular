
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace ModularMod
{
    public class Apollo : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Apollo", "apollo_mdlr");
            Game.Items.Rename("outdated_gun_mods:apollo", "mdl:armcannon_8");
            var c = gun.gameObject.AddComponent<Apollo>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires multiple shots in a fixed spread.\n\nOne half of a pair, this weapon is used to keep clusters at bay and priority targets in check.");

            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "apollo_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "apollo_idle";
            gun.shootAnimation = "apollo_fire";
            gun.reloadAnimation = "apollo_reload";
            gun.introAnimation = "apollo_intro";
            gun.chargeAnimation = "apollo_charge";

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_BOSS_RatMech_Cannon_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_ModulePowerUp";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;
            comp.AdditionalPowerSupply = 0;

            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;


            for (int i = -3; i < 4; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(88) as Gun, true, true);
            }
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>();
            int q = -3;
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Charged;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 0.5f;
                projectileModule.angleFromAim = (30 * q);
                projectileModule.angleVariance = 2;
                q++;
                projectileModule.numberOfShotsInClip = 6;

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectile.baseData.damage = 10f;
                projectile.shouldRotate = true;
                projectile.baseData.range = 19f;
                projectile.baseData.speed = 40;
                projectile.baseData.force = 25;

                projectile.AnimateProjectileBundle("longshot_idle", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "longshot_idle",
                new List<IntVector2>() { new IntVector2(14, 5), new IntVector2(14, 5) },
                ProjectileToolbox.ConstructListOfSameValues(true, 2), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 2), ProjectileToolbox.ConstructListOfSameValues(true, 2), ProjectileToolbox.ConstructListOfSameValues(false, 2),
                ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 2), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 2), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 2), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 2));

                gun.DefaultModule.projectiles[0] = projectile;

                projectile.objectImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
                projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

                projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(59) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
                projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(59) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
                projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(59) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);
                projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(59) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects.First().effects.First().effect);

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                if (projectileModule != gun.DefaultModule)
                {
                    projectileModule.ammoCost = 0;
                }

                ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile,
                    ChargeTime = 0.35f
                };
                projectileModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>() { item2 };
                gun.DefaultModule.chargeProjectiles.Add(item2);
            }


            gun.reflectDuringReload = true;
            


            gun.gunSwitchGroup = "Railgun";

            gun.reloadTime = 4f;
            gun.SetBaseMaxAmmo(250);


            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("apollo_AAa", StaticCollections.Clip_Ammo_Atlas, "actualapollo_1", "actualapollo_2");

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(228) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.5f, 0.375f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.5f, 0.375f), "barrel_point").transform;

            ETGMod.Databases.Items.Add(gun, false, "ANY");
            GunID = gun.PickupObjectId;
            IteratedDesign.SpecialProcessGunSpecific += c.ProcessFireRateSpecial;
        }

        public void ProcessFireRateSpecial(ModulePrinterCore modulePrinterCore, Projectile p, int stack, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != GunID) { return; }
            p.baseData.damage *= 0.9f;
            p.baseData.damage += stack;
            p.baseData.speed *= 0.8f;
            var bounce = p.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounce.bouncesTrackEnemies = true;
            bounce.numberOfBounces += stack;
        }




        public void Start()
        {
            var thing = this.gun.GetComponent<ModularGunController>();
            if (thing)
            {
                thing.StartCoroutine(FUCK(thing));
            }
        }

        public IEnumerator FUCK(ModularGunController aaa)
        {
            aaa.Start();
            yield return null;
            aaa.statMods.Add(new ModuleGunStatModifier()
            {
                AngleFromAim_Process = ProcessClipSize
            });
            yield break;
        }

        public float ProcessClipSize(float clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            float q = clip * (1 - GetElapsed);
            return q;
        }

        public bool CheckModule()
        {
            if (gun.CurrentOwner as PlayerController)
            {
                if (GlobalModuleStorage.PlayerHasActiveModule((gun.CurrentOwner as PlayerController), IteratedDesign.ID))
                {
                    return true;
                }
            }
            return false;
        }


        public float GetElapsed
        {
            get
            {
                if (0 > elapsed) { return 0; }
                return elapsed;
            }
        }

        public override void Update()
        {
            base.Update();
            if (gun.CurrentOwner != null)
            {
                PlayerController player = gun.CurrentOwner as PlayerController;
                if (player != null)
                {
                    if (gun.IsCharging)
                    {
                        float charge = player.stats.GetStatValue(PlayerStats.StatType.ChargeAmountMultiplier);
                        if (this.elapsed <= 1)
                        {
                            this.elapsed += (BraveTime.DeltaTime * (charge / ReturnControl().GetChargeSpeed(1))) / (CheckModule() == true ? 1.125f : 2f);
                        }
                        if (VFXActive != true && GetElapsed > 0)
                        {
                            VFXActive = true;
                            for (int i = 0; i < 5; i++)
                            {
                                float num2 = 16f;
                                Vector2 zero = Vector2.zero;
                                if (BraveMathCollege.LineSegmentRectangleIntersection(this.gun.barrelOffset.transform.position, this.gun.barrelOffset.transform.position + BraveMathCollege.DegreesToVector(player.CurrentGun.CurrentAngle, 60f).ToVector3ZisY(-0.25f), new Vector2(-40, -40), new Vector2(40, 40), ref zero))
                                {
                                    num2 = (zero - new Vector2(this.gun.barrelOffset.transform.position.x, this.gun.barrelOffset.transform.position.y)).magnitude;
                                }
                                GameObject gameObject = SpawnManager.SpawnVFX(VFXStorage.LaserReticle, false);
                                tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
                                component2.transform.position = new Vector3(this.gun.barrelOffset.transform.position.x, this.gun.barrelOffset.transform.position.y, 99999);
                                component2.transform.localRotation = Quaternion.Euler(0f, 0f, player.CurrentGun.CurrentAngle + (45f * i));
                                component2.dimensions = new Vector2((num2) * 2f, 1f);
                                component2.UpdateZDepth();
                                component2.HeightOffGround = -2;
                                component2.renderer.enabled = true;
                                component2.transform.position.WithZ(component2.transform.position.z + 99999);


                                if (i == 0 | i == 2 | i == 4)
                                { component2.dimensions = new Vector2(32, 1f); }
                                else
                                { component2.dimensions = new Vector2(20, 1f); }
                                component2.usesOverrideMaterial = true;

                                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                                component2.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");

                                component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
                                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
                                Color laser = new Color(0f, 1, 1, 1f);
                                component2.sprite.renderer.material.SetColor("_OverrideColor", laser);
                                component2.sprite.renderer.material.SetColor("_EmissiveColor", laser);

                                this.Chargerreticles.Add(gameObject);
                            }
                        }
                        float Accuracy = player.stats.GetStatValue(PlayerStats.StatType.Accuracy) * 15;

                        if (Chargerreticles.Count > 0)
                        {

                            for (int i = -2; i < 3; i++)
                            {
                                GameObject obj = Chargerreticles[i + 2];
                                if (obj != null)
                                {
                                    tk2dTiledSprite component2 = obj.GetComponent<tk2dTiledSprite>();

                                    float a = ReturnControl().GetAccuracy(45);

                                    float AddaAngle = (GetElapsed * (a * i)) - (a * i);
                                    float ix = obj.transform.localRotation.eulerAngles.x + player.CurrentGun.barrelOffset.transform.localRotation.eulerAngles.x;
                                    float wai = obj.transform.localRotation.eulerAngles.y + player.CurrentGun.barrelOffset.transform.localRotation.eulerAngles.y;
                                    float zee = obj.transform.localRotation.z + player.CurrentGun.transform.eulerAngles.z;


                                    obj.transform.localRotation = Quaternion.Euler(ix, wai, zee + AddaAngle + 3600);
                                    obj.transform.position = this.gun.barrelOffset.transform.position;
                                    component2.transform.position.WithZ(component2.transform.position.z + 99999);
                                    component2.UpdateZDepth();
                                    component2.HeightOffGround = -2;
                                }
                            }
                        }
                        
                        foreach (ProjectileModule projectileModule in this.gun.Volley.projectiles)
                        {
                            Projectile proj = projectileModule.GetCurrentProjectile();
                            if (proj != null)
                            {
                                proj.baseData.damage = ((GetElapsed * 6) + 3.5f);
                            }
                        }
                        
                    }
                    else
                    {
                        elapsed = -0.35f;
                        VFXActive = false;
                        CleanupReticles();
                    }
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
            else
            {
                elapsed = -0.35f;
                CleanupReticles();
            }

        }
        private bool HasReloaded;
        private List<GameObject> Chargerreticles = new List<GameObject>();
        private bool VFXActive;
        private float elapsed;


        public ModularGunController ReturnControl()
        {
            return this.gun.GetComponent<ModularGunController>();
        }

        public void CleanupReticles()
        {
            for (int i = 0; i < this.Chargerreticles.Count; i++)
            {
                SpawnManager.Despawn(this.Chargerreticles[i]);
                Destroy(this.Chargerreticles[i]);
            }
            this.Chargerreticles.Clear();
        }


        public static int GunID;
    }
}