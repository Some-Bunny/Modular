﻿using Alexandria.ItemAPI;
using Alexandria.Misc;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class HyperPropellant : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HyperPropellant))
        {
            Name = "Hyper Propellant",
            Description = "FWOOMP",
            LongDescription = "Greatly reduces fire rate, clip size and increases reload time. Projectiles ignite the air around them (+Larger Ignition Area per stack), travel at very high speeds, and hit with massive force. (+Speed, Force And Damage per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("hyperpropellant_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("hyperpropellant_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Hyper Propellant " + h.ReturnTierLabel();
            h.LabelDescription = "Greatly reduces fire rate, clip size and increases reload time.\nProjectiles ignite the air around them (" + StaticColorHexes.AddColorToLabelString("+Larger Ignition Area", StaticColorHexes.Light_Orange_Hex) + "),\ntravel at very high speeds, and hit with massive force.\n(" + StaticColorHexes.AddColorToLabelString("+Speed, Force And Damage", StaticColorHexes.Light_Orange_Hex) + ")";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.AdditionalWeightMultiplier = 0.9f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;

            var boomObj = UnityEngine.Object.Instantiate((EnemyDatabase.GetOrLoadByGuid("ffca09398635467da3b1f4a54bcfda80")).gameObject.GetComponent<BulletKingDeathController>().bigExplosionVfx);
            FakePrefab.MakeFakePrefab(boomObj);
            DontDestroyOnLoad(boomObj);
            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(boomObj.GetComponent<tk2dSpriteAnimator>(), boomObj.GetComponent<tk2dSpriteAnimator>().DefaultClip.name, new Dictionary<int, string>() { { 0, "Play_DragunGrenade" } });

            HyperPropellantExplosionData = new ExplosionData()
            {
                breakSecretWalls = true,
                comprehensiveDelay = 0,
                damage = 30,
                damageRadius = 3f,
                damageToPlayer = 0,
                debrisForce = 100,
                doDamage = true,
                doDestroyProjectiles = false,
                doExplosionRing = true,
                doForce = true,
                doScreenShake = true,
                doStickyFriction = false,
                effect = boomObj,
                explosionDelay = 0,
                force = 100,
                forcePreventSecretWallDamage = false,
                forceUseThisRadius = true,
                freezeEffect = null,
                freezeRadius = 0,
                IsChandelierExplosion = false,
                isFreezeExplosion = false,
                playDefaultSFX = false,
                preventPlayerForce = false,
                pushRadius = 1,
                secretWallsRadius = 1,
                ss = new ScreenShakeSettings()
                {
                    magnitude = 10,
                    simpleVibrationTime = Vibration.Time.Quick,
                    time = 0.2f,
                    vibrationType = ScreenShakeSettings.VibrationType.Auto,
                    simpleVibrationStrength = Vibration.Strength.Medium,
                    direction = Vector2.left,
                    falloff = 0.5f,
                    speed = 1,
                },
                ignoreList = new List<SpeculativeRigidbody>(),
                rotateEffectToNormal = false,
                useDefaultExplosion = false,
                usesComprehensiveDelay =  false,
                overrideRangeIndicatorEffect = null
            };
            GameObject VFX = new GameObject("HyperPropellantCircle");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var hyperProp = VFX.AddComponent<HyperPropellantAirIgnite>();
            AirBurn = VFX;

        }
        public static GameObject AirBurn;

        public static ExplosionData HyperPropellantExplosionData;
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.ChangeMuzzleFlash((PickupObjectDatabase.GetById(384) as Gun).muzzleFlashEffects);
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                ClipSize_Process = ProcessClipSize,
                FireRate_Process = ProcessFireRate,
                Reload_Process = ProcessReload,
            };
            modularGunController.statMods.Add(this.gunStatModifier);
            modularGunController.OnGunFired += OPF;
            printer.OnPostProcessProjectile += PPP;
            modularGunController.gun.gunSwitchGroup = (PickupObjectDatabase.GetById(19) as Gun).gunSwitchGroup;
        }

        public void OPF(ModulePrinterCore printer, PlayerController player, Gun gun)
        {
            

        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1.5f + stack;
            p.baseData.speed *= 2.5f + (0.5f* stack);
            p.baseData.force *= (10f * stack);
            p.AdditionalScaleMultiplier *= 2;
            p.pierceMinorBreakables = true;

            ExplosiveModifier explosiveModifier = p.gameObject.AddComponent<ExplosiveModifier>();
            explosiveModifier.explosionData = HyperPropellantExplosionData;
            explosiveModifier.doExplosion = true;
            explosiveModifier.IgnoreQueues = true;

            ImprovedAfterImage yes = p.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(0.9f, 0.6f, 0f, 1f);

            var uhfa = p.gameObject.AddComponent<HyperPropellantController>();
            uhfa.Radius = 1 + stack;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.RevertMuzzleFlash();
            if (modularGunController.statMods.Contains(this.gunStatModifier)) { modularGunController.statMods.Remove(this.gunStatModifier); }
            modularGunController.OnGunFired -= OPF;
            modulePrinter.OnPostProcessProjectile -= PPP;
            modularGunController.ResetSwitchGroup();
        }
        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return (int)(clip / 4);
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 4;
        }
        public float ProcessReload(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 2;
        }
    }

    public class HyperPropellantController : MonoBehaviour
    {
        public Projectile self;
        public Vector2 lastStoredPosition;
        public float Radius = 2;
        public void Start()
        {
            self = this.GetComponent<Projectile>();
            lastStoredPosition = self.sprite.WorldCenter;
        }

        public void Update()
        {
            float Dist = (int)Vector2.Distance(self.sprite.WorldCenter, lastStoredPosition);
            if (Dist < 1)
            {
                if (UnityEngine.Random.value < Dist)
                {
                    Vector3 vector3 = self.sprite.WorldCenter;
                    HyperPropellantAirIgnite ignite = UnityEngine.Object.Instantiate(HyperPropellant.AirBurn, vector3, Quaternion.identity).GetComponent<HyperPropellantAirIgnite>();
                    ignite.transform.position = self.sprite.WorldCenter;
                   // ignite.Enable(100);
                    ignite.radius = Radius;
                    ignite.StartCoroutine(ignite.ReduceToZero());
                }
            }
            else
            {
                for (int i = 0; i < (int)Dist; i++)
                {
                    float t = (float)i / (float)Dist;
                    Vector3 vector3 = Vector3.Lerp(self.sprite.WorldCenter, lastStoredPosition, t);
                    HyperPropellantAirIgnite ignite = UnityEngine.Object.Instantiate(HyperPropellant.AirBurn, vector3, Quaternion.identity).GetComponent<HyperPropellantAirIgnite>();
                    ignite.transform.position = self.sprite.WorldCenter;
                    //ignite.Enable(100);
                    ignite.radius = Radius;
                    ignite.StartCoroutine(ignite.ReduceToZero());
                }
            }
           
            lastStoredPosition = self.sprite.WorldCenter;
        }
    }


    public class HyperPropellantAirIgnite : MagicCircle
    {
        public float DamagePerSecond = 3;
        public HyperPropellantAirIgnite()
        {
            this.emitsParticles = false;
            this.colour = new Color(0, 0, 0, 0);
            this.preventMagicIndicator = false;
            this.radius = 2f;
            this.destroyOnDisable = true;
            this.autoEnableOnStart = true;
        }

        public IEnumerator ReduceToZero()
        {
            Enabled = true;
            float h = this.radius;
            float f = 0;
            float asdf = 0;
            while (f < 6)
            {
                float math = Mathf.Lerp(h, 0, f / 6);

                if (asdf > 0.25f)
                {
                    asdf = 0;
                    GlobalSparksDoer.DoSingleParticle(this.transform.PositionVector2() + Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0.1F, math)), Vector2.up, null, 3, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                }
                this.UpdateRadius(math);
                f += BraveTime.DeltaTime;
                asdf += BraveTime.DeltaTime;
                yield return null;
            }
            Enabled = false;
            
            this.Disable();
            yield break;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            var obj = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX, base.gameObject.transform.position, Quaternion.identity);
            Destroy(obj, 2);
        }
        private bool Enabled;
        public override void TickOnEnemy(AIActor enemy)
        {
            base.TickOnEnemy(enemy);
            if (Enabled == false) { return; }
            enemy.healthHaver.ApplyDamage(DamagePerSecond * BraveTime.DeltaTime, Vector2.zero, "Hellfire", CoreDamageTypes.Fire, DamageCategory.DamageOverTime);
            enemy.gameActor.ApplyEffect(DebuffStatics.hotLeadEffect);
        }
    }
}
