
using UnityEngine;
using Gungeon;
using Alexandria.ItemAPI;
using System;
using ModularMod;
using System.Linq;
using System.Collections.Generic;
using Alexandria.Misc;

namespace ModularMod
{



    public class TheHammer : MultiActiveReloadController
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("The Hammer", "thehammer");
            Game.Items.Rename("outdated_gun_mods:the_hammer", "mdl:armcannon_7");
            var c =  gun.gameObject.AddComponent<TheHammer>();
            gun.SetShortDescription("Mk.1");
            gun.SetLongDescription("Fires high power energy. Hitting the active reload timing instantly reloads the clip. Compatible with Modular Upgrade Software.\n\nBuilt off of a mechanism that would lightly hammer in nails for hanging up things, but taken to the logical extreme.");
            
            GunInt.SetupSprite(gun, StaticCollections.Gun_Collection, "hammershot_idle_001");
            gun.spriteAnimator.Library = StaticCollections.Gun_Animation;
            gun.sprite.SortingOrder = 1;
            gun.idleAnimation = "hammershot_idle";
            gun.shootAnimation = "hammershot_fire";
            gun.reloadAnimation = "hammershot_reload";
            gun.introAnimation = "hammershot_intro";
            gun.PersistsOnDeath = true;
            gun.PreventStartingOwnerFromDropping = true;

            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(56) as Gun, true, false);

            var comp = gun.gameObject.AddComponent<ModularGunController>();
            comp.isAlt = false;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(383) as Gun).gunSwitchGroup;


            gun.reloadTime = 4f;
            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(250);
            gun.DefaultModule.angleVariance = 0f;


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.AnimateProjectileBundle("hammershot_idle", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "hammershot_idle",
            new List<IntVector2>() { new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), new IntVector2(16, 10), },
            ProjectileToolbox.ConstructListOfSameValues(true, 5), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 5), ProjectileToolbox.ConstructListOfSameValues(true, 5), ProjectileToolbox.ConstructListOfSameValues(false, 5),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 5), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 5));


            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX((PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect);

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = projectile.sprite.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 100);
            mat.SetFloat("_EmissivePower", 100);
            projectile.sprite.renderer.material = mat;

            projectile.baseData.damage = 10f;
            projectile.baseData.speed = 50f;
            projectile.pierceMinorBreakables = true;

            projectile.shouldRotate = true;

            var tro = projectile.gameObject.AddChild("trail object");
            tro.transform.position = projectile.sprite.WorldCenter;// + new Vector2(.25f, 0.3125f);
            tro.transform.localPosition = projectile.sprite.WorldCenter;// + new Vector2(.25f, 0.3125f);

            TrailRenderer tr = tro.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.receiveShadows = false;
            var asd = new Material(Shader.Find("Sprites/Default"));
            tr.material = asd;
            tr.minVertexDistance = 0.01f;
            tr.numCapVertices = 20;

            //======
            UnityEngine.Color color = new UnityEngine.Color(0, 500, 500, 500);
            mat.SetColor("_Color", color * 0.7f);
            tr.startColor = color;
            tr.endColor = color * 0.7f;
            //======
            tr.time = 0.3f;
            //======
            tr.startWidth = 0.75f;
            tr.endWidth = 0f;
            tr.autodestruct = false;

            var rend = projectile.gameObject.AddComponent<ProjectileTrailRendererController>();
            rend.trailRenderer = tr;
            rend.desiredLength = 4;

            projectile.gameObject.AddComponent<DetachTrailFromParent>();

            gun.gunClass = GunClass.NONE;

            gun.AddGlowShaderToGun(new Color32(121, 234, 255, 255), 10, 10);

            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.carryPixelOffset = new IntVector2(4, 2);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(390) as Gun).muzzleFlashEffects;
            gun.muzzleOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "muzzle_point").transform;
            gun.barrelOffset = Toolbox.GenerateTransformPoint(gun.gameObject, new Vector2(0.3125f, 0.25f), "barrel_point").transform;

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("ThehammerOfTheFunny", StaticCollections.Clip_Ammo_Atlas, "hammer_1", "hammer_2");
            gun.activeReloadData = new ActiveReloadData()
            {
                reloadSpeedMultiplier = 1.05f,
                damageMultiply = 1.025f,
                ActiveReloadIncrementsTier = true,
                ActiveReloadStacks = true,
                MaxTier = 50,
                
            };

            gun.m_canAttemptActiveReload = true;
            gun.LocalActiveReload = true;
            c.activeReloadEnabled = true;
            c.canAttemptActiveReload = true;
            /*
            behavior.activeReloadEnabled = true;
            behavior.canAttemptActiveReload = true;
            behavior.reloads = new List<MultiActiveReloadData>
            {
                new MultiActiveReloadData(0, 50, 60, 48, 0, true, false, new ActiveReloadData
                {
                    damageMultiply = 1f,

                }, true, "SwitchClip"),
            };
            */
            ETGMod.Databases.Items.Add(gun, false, "ANY");
            ID = gun.PickupObjectId;


            GameObject VFX = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            VFX.SetActive(false);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("hammerstrike_006"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = Module.ModularAssetBundle.LoadAsset<GameObject>("HammerStrikeAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.GetClipIdByName("hammerstrike");
            tk2dAnim.playAutomatically = true;

            var k = tk2dAnim.gameObject.AddComponent<SpriteAnimatorKiller>();
            k.animator = tk2dAnim;
            k.fadeTime = 0.6f;

            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 10);

            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(tk2dAnim, "hammerstrike", new Dictionary<int, string>()
            {
                {5, "Play_OBJ_spears_clank_01"},
            });
            StrikeVFX = VFX;
            gun.activeReloadSuccessEffects = new VFXPool() { type = VFXPoolType.Single, effects = new VFXComplex[] { new VFXComplex() { effects = new VFXObject[] { new VFXObject() { effect = StrikeVFX } } } } };


            HammerData = new ExplosionData()
            {
                breakSecretWalls = true,
                comprehensiveDelay = 0,
                damage = 20,
                damageRadius = 3f,
                damageToPlayer = 0,
                debrisForce = 100,
                doDamage = true,
                doDestroyProjectiles = true,
                doExplosionRing = true,
                doForce = true,
                doScreenShake = true,
                doStickyFriction = false,
                effect = (PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX,
                explosionDelay = 0,
                force = 50,
                forcePreventSecretWallDamage = false,
                forceUseThisRadius = true,
                freezeEffect = null,
                freezeRadius = 0,
                IsChandelierExplosion = false,
                isFreezeExplosion = false,
                playDefaultSFX = false,
                preventPlayerForce = true,
                pushRadius = 1,
                secretWallsRadius = 1,
                ss = new ScreenShakeSettings() { },

                ignoreList = new List<SpeculativeRigidbody>(),
                rotateEffectToNormal = false,
                useDefaultExplosion = false,
                usesComprehensiveDelay = false,
                overrideRangeIndicatorEffect = null
            };

            ExplosiveModifier explosiveModifier = projectile.gameObject.AddComponent<ExplosiveModifier>();
            explosiveModifier.explosionData = HammerData;
            explosiveModifier.doExplosion = true;
            explosiveModifier.IgnoreQueues = true;

            IteratedDesign.SpecialProcessGunSpecific += c.Process;
            IteratedDesign.SpecialProcessGunSpecificReload += c.ProcessReloadSpecial;

        }
        public float ProcessReloadSpecial(float f, int stack, ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != ID) { return f; }
            return f / (1 + (stack / 5));
        }
        public void Process(ModulePrinterCore modulePrinterCore, Projectile p, int stack, PlayerController player)
        {
            if (modulePrinterCore.ModularGunController.gun.PickupObjectId != ID) { return; }
            p.baseData.damage += 5 * stack;
            p.StunApplyChance = 0.2f;
            p.AppliesStun = true;
        }



        public static ExplosionData HammerData;

        public static GameObject StrikeVFX;

        public override void OnReloadEndedSafe(PlayerController player, Gun gun)
        {
            base.OnReloadEndedSafe(player, gun);
        }

        public override void OnActiveReloadSuccess(MultiActiveReload reload)
        {
            var fx = base.gun.CurrentOwner.PlayEffectOnActor(StrikeVFX, new Vector3(0, 1.25f));
            fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("hammerstrike");
            base.OnActiveReloadSuccess(reload);
        }

        public override void OnActiveReloadFailure(MultiActiveReload reload)
        {
            base.OnActiveReloadFailure(reload);
            AkSoundEngine.PostEvent("Play_DragunGrenade", gun.gameObject);
            if (gun.CurrentOwner as PlayerController)
            {
                if (GlobalModuleStorage.PlayerHasActiveModule((gun.CurrentOwner as PlayerController), IteratedDesign.ID))
                {
                    var newData = StaticExplosionDatas.CopyFields(HammerData);
                    newData.damage = 70;
                    newData.damageRadius = 4;
                    Exploder.Explode(gun.sprite.WorldCenter, newData, gun.sprite.WorldCenter);

                }
            }
            else
            {
                Exploder.Explode(gun.sprite.WorldCenter, HammerData, gun.sprite.WorldCenter);
            }
        }


        public static int ID;
    }
}