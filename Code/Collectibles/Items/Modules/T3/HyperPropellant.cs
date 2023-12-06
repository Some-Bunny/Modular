using Alexandria.ItemAPI;
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

            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
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
                damage = 20,
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
                ss = new ScreenShakeSettings() { },
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
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            AirBurn = VFX;

        }
        public static GameObject AirBurn;

        public static ExplosionData HyperPropellantExplosionData;
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.015f) { return; }
            int stack = 1;
            p.baseData.damage *= 0.625f + stack;
            p.baseData.speed *= 2.5f + (0.5f * stack);
            p.baseData.force *= (7.5f * stack);
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
            uhfa.Radius = 1.25f + stack;
            uhfa.DPS = 3 + stack;
        }

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.ChangeMuzzleFlash((PickupObjectDatabase.GetById(384) as Gun).muzzleFlashEffects);
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                ClipSize_Process = ProcessClipSize,
                FireRate_Process = ProcessFireRate,
                Reload_Process = ProcessReload,
                ChargeSpeed_Process = ProcessFireRate,
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            printer.OnPostProcessProjectile += PPP;
            modularGunController.gun.gunSwitchGroup = (PickupObjectDatabase.GetById(19) as Gun).gunSwitchGroup;
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 0.625f + stack;
            p.baseData.speed *= 2.5f + (0.5f * stack);
            p.baseData.force *= (7.5f * stack);
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
            uhfa.Radius = 1.25f + stack;
            uhfa.DPS = 3 + stack;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.RevertMuzzleFlash();
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectile -= PPP;
            modularGunController.ResetSwitchGroup();
        }
        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return (int)(clip / 5);
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 4f;
        }
        public float ProcessReload(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 1.75f;
        }
    }

    public class HyperPropellantController : MonoBehaviour
    {
        public Projectile self;
        public Vector2 lastStoredPosition;
        public float Radius = 2.25f;
        public float DPS = 4;

        public void Start()
        {
            self = this.GetComponent<Projectile>();
            lastStoredPosition = self.sprite.WorldCenter;
        }
        private float DistTick = 0;

        public void Update()
        {
            int n = 0;
            DistTick += (int)Vector2.Distance(self.sprite.WorldCenter, lastStoredPosition) / 2;
            for (int i = 0; i < DistTick; i++)
            {
                n++;
                float t = (float)i / DistTick;
                Vector3 vector3 = Vector3.Lerp(self.sprite.WorldCenter, lastStoredPosition, t);
                HyperPropellantAirIgnite ignite = UnityEngine.Object.Instantiate(HyperPropellant.AirBurn, vector3, Quaternion.identity).GetComponent<HyperPropellantAirIgnite>();
                ignite.transform.position = vector3;
                ignite.DamagePerSecond = DPS;
                //ignite.Enable(100);
                ignite.radius = Radius;
                ignite.StartCoroutine(ignite.ReduceToZero());
                lastStoredPosition = self.sprite.WorldCenter;
            }
            DistTick -= n;
        }
    }


    public class HyperPropellantAirIgnite : MagicCircle
    {
        public float DamagePerSecond = 4;
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

                if (asdf > 0.3f)
                {
                    asdf = 0;
                    if (UnityEngine.Random.value < ConfigManager.ImportantVFXMultiplier)
                    {
                        GlobalSparksDoer.DoSingleParticle(this.transform.PositionVector2() + Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0.1f, math)), Vector2.up, null, 3, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
                    }
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
            if (ConfigManager.DoVisualEffect == true)
            {
                var obj = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(328) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.overrideMidairDeathVFX, base.gameObject.transform.position, Quaternion.identity);
                obj.transform.localScale *= 0.6f;
                Destroy(obj, 2);
            }
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

