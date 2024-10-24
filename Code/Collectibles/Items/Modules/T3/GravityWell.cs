using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Reflection;
using UnityEngine;


namespace ModularMod
{
    public class GravityWell : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(GravityWell))
        {
            Name = "Gravity Well",
            Description = "The Void",
            LongDescription = "Greatly reduces Rate Of Fire. Projectiles gain piercing and greatly reduced speed. Enemies are magnetically pulled towards your projectiles, and are hurt in their proximity.(+Stronger Gravity And Damage per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("gravitywell_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("gravitywell_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Gravity Well " + h.ReturnTierLabel();
            h.LabelDescription = "Greatly reduces Rate Of Fire.\nProjectiles gain piercing and greatly reduced speed.\nEnemies are pulled towards your projectiles\nand are hurt in their proximity.(" + StaticColorHexes.AddColorToLabelString("+Stronger Gravity And Damage", StaticColorHexes.Light_Orange_Hex) + ").";


            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);
            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AdditionalWeightMultiplier = 0.9f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);

            ID = h.PickupObjectId;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;

        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.005f) { return; }
            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += 10;
            p.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
            p.AdditionalScaleMultiplier *= 1.5f;

            p.baseData.speed *= 0.25f;
            p.UpdateSpeed();
            var well = p.gameObject.AddComponent<EnemyGravityWell>();
            well.self = p;
            well.gravitationalForceActors = 100;
            well.damageRadius = 2;
            well.Stack = 1;

            ImprovedAfterImage yes = p.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.4f;
            yes.shadowTimeDelay = 0.01f;
            yes.dashColor = new Color(1f, 0.1f, 0.5f, 1f);

            HomingModifier HomingMod = p.gameObject.GetOrAddComponent<HomingModifier>();
            HomingMod.AngularVelocity += 10;
            HomingMod.HomingRadius += 20;

            GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(536) as RelodestoneItem).ContinuousVFX, true);
            vfx.transform.parent = p.transform;
            vfx.transform.position = p.sprite.WorldCenter;
        }

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                ChargeSpeed_Process = ProcessFireRate,
                FireRate_Process = ProcessFireRate,
                ClipSize_Process = ProcessClip
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            printer.OnPostProcessProjectile += PPP;
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 3f;
        }
        public int ProcessClip(int f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return (int)(f / 2f);
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            PierceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<PierceProjModifier>();
            bounceProjModifier.penetration += 10;
            p.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
            p.AdditionalScaleMultiplier *= 1.5f;

            p.baseData.speed *= 0.25f;
            p.UpdateSpeed();
            var well = p.gameObject.AddComponent<EnemyGravityWell>();
            well.self = p;
            well.gravitationalForceActors = 100 * this.ReturnStack(modulePrinterCore);
            well.damageRadius = 2 + this.ReturnStack(modulePrinterCore);
            well.Stack = this.ReturnStack(modulePrinterCore);
            ImprovedAfterImage yes = p.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.75f;
            yes.shadowTimeDelay = 0.1f;
            yes.dashColor = new Color(1f, 0.1f, 0.5f, 1f);

            HomingModifier HomingMod = p.gameObject.GetOrAddComponent<HomingModifier>();
            HomingMod.AngularVelocity += 10;
            HomingMod.HomingRadius += 20;

            if (ConfigManager.DoVisualEffect == true)
            {
                GameObject vfx = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(536) as RelodestoneItem).ContinuousVFX, true);
                vfx.transform.parent = p.transform;
                vfx.transform.position = p.sprite.WorldCenter;
            }
        }


        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            player.stats.RecalculateStats(player);
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
    }

    public class EnemyGravityWell : MonoBehaviour
    {
        public Projectile self;
        public float damageRadius = 3f;
        public float radius = 25f;
        public float gravitationalForce = 10f;
        public float gravitationalForceActors = 50f;
        private float m_radiusSquared;
        public int Stack = 1;

        public void Start()
        {
            cachedgravitationalForceActors = gravitationalForceActors;
            this.m_radiusSquared = this.radius * this.radius;
        }
        private float cachedgravitationalForceActors;

        private float Elapsed = 0;
        public void Update()
        {
            Elapsed = Elapsed += BraveTime.DeltaTime;
            gravitationalForceActors = Mathf.Lerp(0, cachedgravitationalForceActors, Elapsed);
            for (int i = 0; i < PhysicsEngine.Instance.AllRigidbodies.Count; i++)
            {
                if (PhysicsEngine.Instance.AllRigidbodies[i].gameObject.activeSelf)
                {
                    if (PhysicsEngine.Instance.AllRigidbodies[i].enabled)
                    {
                        this.AdjustRigidbodyVelocity(PhysicsEngine.Instance.AllRigidbodies[i]);
                    }
                }
            }
            for (int j = 0; j < StaticReferenceManager.AllDebris.Count; j++)
            {
                this.AdjustDebrisVelocity(StaticReferenceManager.AllDebris[j]);
            }
        }


        private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
        {
            Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
            return new Vector4(vector.x, vector.y, 0f, 0f);
        }

        private float GetDistanceToRigidbody(SpeculativeRigidbody other)
        {
            return Vector2.Distance(other.UnitCenter, self.specRigidbody.UnitCenter);
        }

        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, float currentDistance, float g)
        {
            Vector2 zero = Vector2.zero;
            float num = Mathf.Clamp01(1f - currentDistance / this.radius);
            float d = g * num * num;
            Vector2 normalized = (self.specRigidbody.UnitCenter - unitCenter).normalized;
            return normalized * d;
        }

        private bool AdjustDebrisVelocity(DebrisObject debris)
        {
            if (debris.IsPickupObject)
            {
                return false;
            }
            if (debris.GetComponent<BlackHoleDoer>() != null)
            {
                return false;
            }
            Vector2 a = debris.sprite.WorldCenter - self.specRigidbody.UnitCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num >= this.m_radiusSquared)
            {
                return false;
            }
            float g = this.gravitationalForceActors;
            float num2 = Mathf.Sqrt(num);
            if (num2 < this.damageRadius)
            {
                UnityEngine.Object.Destroy(debris.gameObject);
                return true;
            }
            Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, num2, g);
            float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
            if (debris.HasBeenTriggered)
            {
                debris.ApplyVelocity(frameAccelerationForRigidbody * d);
            }
            else if (num2 < this.radius / 2f)
            {
                debris.Trigger(frameAccelerationForRigidbody * d, 0.5f, 1f);
            }
            return true;
        }

        // Token: 0x06007034 RID: 28724 RVA: 0x002C7BC8 File Offset: 0x002C5DC8
        private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other)
        {
            Vector2 a = other.UnitCenter - self.specRigidbody.UnitCenter;
            float num = Vector2.SqrMagnitude(a);
            if (num < this.m_radiusSquared)
            {
                float g = this.gravitationalForce;
                Vector2 velocity = other.Velocity;
                Projectile projectile = other.projectile;
                if (projectile)
                {
                    return false;
                }
                else
                {
                    if (!other.aiActor)
                    {
                        return false;
                    }
                    g = this.gravitationalForceActors;
                    if (!other.aiActor.enabled)
                    {
                        return false;
                    }
                    if (!other.aiActor.HasBeenEngaged)
                    {
                        return false;
                    }
                    if (BraveMathCollege.DistToRectangle(self.specRigidbody.UnitCenter, other.UnitBottomLeft, other.UnitDimensions) < this.damageRadius)
                    {
                        other.healthHaver.ApplyDamage(((self.baseData.damage * 0.2f) * BraveTime.DeltaTime)* Stack, a.normalized, string.Empty, CoreDamageTypes.None, DamageCategory.DamageOverTime, false, null, false);
                    }
                    if (other.healthHaver.IsBoss)
                    {
                        return false;
                    }
                }
                Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, Mathf.Sqrt(num), g);
                float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
                Vector2 b = frameAccelerationForRigidbody * d;
                Vector2 vector = velocity + b;
                if (BraveTime.DeltaTime > 0.02f)
                {
                    vector *= 0.02f / BraveTime.DeltaTime;
                }
                other.Velocity = vector;
                if (projectile != null)
                {
                    projectile.collidesWithPlayer = false;
                    if (projectile.IsBulletScript)
                    {
                        projectile.RemoveBulletScriptControl();
                    }
                    if (vector != Vector2.zero)
                    {
                        projectile.Direction = vector.normalized;
                        projectile.Speed = Mathf.Max(3f, vector.magnitude);
                        other.Velocity = projectile.Direction * projectile.Speed;
                        if (projectile.shouldRotate && (vector.x != 0f || vector.y != 0f))
                        {
                            float num2 = BraveMathCollege.Atan2Degrees(projectile.Direction);
                            if (!float.IsNaN(num2) && !float.IsInfinity(num2))
                            {
                                Quaternion rotation = Quaternion.Euler(0f, 0f, num2);
                                if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y))
                                {
                                    projectile.transform.rotation = rotation;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public void OuterLimitsProcessEnemy(AIActor a, float b)
        {
            if (a && a.IsNormalEnemy && a.healthHaver && !a.IsGone)
            {
                /*
                a.healthHaver.ApplyDamage(100f, Vector2.zero, "projectile", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                if (this.OuterLimitsDamageVFX != null)
                {
                    a.PlayEffectOnActor(this.OuterLimitsDamageVFX, Vector3.zero, false, true, false);
                }
                */
            }
        }
    }

}

