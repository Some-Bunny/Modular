using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class BattersRevenge : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BattersRevenge))
        {
            Name = "Batters Revenge",
            Description = "The Best Weapon",
            LongDescription = "Reduces reload time by 20% (+20% per stack hyperbolically) When reloading, toss out 1 (+1) baseballs that can be shot to pitch them. Damage scales with the projectile that hit it. (+Higher Damage Scaling per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("battersrevenge_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("battersrevenge_tier1_module_alt.png");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.75f;
            h.LabelName = "Batters Revenge " + h.ReturnTierLabel();
            h.LabelDescription = "Reduces reload time by 20% ("+ StaticColorHexes.AddColorToLabelString("+20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").\nWhen reloading, toss out 1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") baseballs that can be shot\nto pitch them. Damage scales with the projectile that hit it.\n(" + StaticColorHexes.AddColorToLabelString("+Higher Damage Scaling", StaticColorHexes.Light_Orange_Hex) + ")";

            h.AddModuleTag(BaseModuleTags.BASIC);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);


            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            var proj_Base = (PickupObjectDatabase.GetById(541) as Gun).DefaultModule.chargeProjectiles[0].Projectile;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(88) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MakeFakePrefab(projectile.gameObject);
            DontDestroyOnLoad(projectile.gameObject);

            projectile.baseData.damage = 10f;
            projectile.shouldRotate = false;
            projectile.baseData.range = 1000f;
            projectile.baseData.speed = 8;
            projectile.baseData.force = 25;
            projectile.baseData.UsesCustomAccelerationCurve = true;
            projectile.baseData.AccelerationCurve = AnimationCurve.Linear(0.1f, 0.7f, 1, 0);
            projectile.collidesWithProjectiles = true;
            projectile.collidesOnlyWithPlayerProjectiles = true;


            //var KEBP = projectile.gameObject.AddComponent<KilledEnemiesBecomeProjectileModifier>();
            //KEBP.CompletelyBecomeProjectile = false;
            //KEBP.BaseProjectile = proj_Base.GetComponent<KilledEnemiesBecomeProjectileModifier>().BaseProjectile;
            projectile.gameObject.AddComponent<BallComponent>();
            projectile.AnimateProjectileBundle("ballin", StaticCollections.Projectile_Collection, StaticCollections.Projectile_Animation, "ballin",
            new List<IntVector2>() { new IntVector2(10, 10), new IntVector2(10, 10), new IntVector2(10, 10), new IntVector2(10, 10), new IntVector2(10, 10), new IntVector2(10, 10), new IntVector2(10, 10), new IntVector2(10, 10), new IntVector2(10, 10) },
            ProjectileToolbox.ConstructListOfSameValues(true, 9), ProjectileToolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 9), ProjectileToolbox.ConstructListOfSameValues(true, 9), ProjectileToolbox.ConstructListOfSameValues(false, 9),
            ProjectileToolbox.ConstructListOfSameValues<Vector3?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<IntVector2?>(null, 9), ProjectileToolbox.ConstructListOfSameValues<Projectile>(null, 9));
            projectile.objectImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.enemyImpactEventName = (PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0].enemyImpactEventName;

            projectile.hitEffects.tileMapHorizontal = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.tileMapVertical = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.enemy = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);
            projectile.hitEffects.deathAny = Toolbox.MakeObjectIntoVFX(proj_Base.hitEffects.enemy.effects.First().effects.First().effect);
            BounceProjModifier BounceProjMod1 = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            BounceProjMod1.bouncesTrackEnemies = false;
            BounceProjMod1.numberOfBounces = 1;
            ImprovedAfterImage yes = projectile.gameObject.AddComponent<ImprovedAfterImage>();
            yes.spawnShadows = true;
            yes.shadowLifetime = 0.5f;
            yes.shadowTimeDelay = 0.05f;
            yes.dashColor = new Color(0.8f, 0.8f, 0.8f, 1f);

            Ballin = projectile;

            ID = h.PickupObjectId;
        }

        public static Projectile Ballin;

        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded += OGR;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Batters_Revenge",
                Reload_Process = ProcessReloadTime
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);

        }

        public float ProcessReloadTime(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.2f * stack)));
        }

        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            int stacc = this.ReturnStack(modulePrinterCore);
            for (int i = 0; i < stacc; i++)
            {
                float acc = modulePrinterCore.ModularGunController.GetAccuracy(15);
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(Ballin.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, player.CurrentGun.CurrentAngle + UnityEngine.Random.Range(-acc, acc)), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.baseData.speed *= UnityEngine.Random.Range(0.85f, 1.5f);
                    component.UpdateSpeed();
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.GetComponent<BallComponent>().Stack = stacc;
                }
            }
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded -= OGR;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
        public class BallComponent : BraveBehaviour
        {
            public int Stack = 1;
            public void Start()
            {
                this.projectile.specRigidbody.OnPreRigidbodyCollision += PreToss;
                this.StartCoroutine(DoWait());
            }
            public IEnumerator DoWait()
            {
                var p = this.projectile;

                float e = 0;
                while (e < 0.75f)
                {
                    if (p == null)
                    {
                        yield break; }
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                p.specRigidbody.OnPreRigidbodyCollision -= PreToss;
                p.specRigidbody.OnPreRigidbodyCollision += HandleHitEnemyHitEnemy;

                yield break;
            }

            private void PreToss(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
            {
                Projectile p = otherRigidbody.projectile;
                if (p != null)
                {
                    PhysicsEngine.SkipCollision = true;
                    myRigidbody.RegisterTemporaryCollisionException(otherRigidbody, 0.25f);
                }
            }

            private void HandleHitEnemyHitEnemy(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
            {

                Projectile p = otherRigidbody.projectile;
                if (p != null && otherRigidbody.GetComponent<BallComponent>() == null)
                {
                    PhysicsEngine.SkipCollision = true;
                    myRigidbody.RegisterTemporaryCollisionException(otherRigidbody, 1);
                    if ((p.Owner as PlayerController) != null)
                    {
                        myRigidbody.projectile.hitEffects.HandleProjectileDeathVFX(myRigidbody.transform.position, 0, myRigidbody.transform, myRigidbody.transform.position, otherRigidbody.Velocity);
                        myRigidbody.projectile.specRigidbody.OnPreRigidbodyCollision -= HandleHitEnemyHitEnemy;
                        myRigidbody.projectile.collidesWithProjectiles = false;
                        myRigidbody.projectile.collidesOnlyWithPlayerProjectiles = false;
                        myRigidbody.Reinitialize();
                        myRigidbody.projectile.SendInDirection(otherRigidbody.Velocity, true);
                        myRigidbody.projectile.baseData.UsesCustomAccelerationCurve = false;
                        myRigidbody.projectile.baseData.speed = 60;
                        myRigidbody.projectile.UpdateSpeed();
                        myRigidbody.projectile.baseData.damage += otherRigidbody.projectile.baseData.damage;
                        myRigidbody.projectile.baseData.damage *= 1 + (0.25f * Stack);
                        myRigidbody.projectile.gameObject.AddComponent<KilledEnemiesBecomeProjectileModifier>();
                        BounceProjModifier BounceProjMod1 = myRigidbody.projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                        BounceProjMod1.bouncesTrackEnemies = false;
                        BounceProjMod1.numberOfBounces += 3;
                    }
                }
                else if (otherRigidbody.GetComponent<BallComponent>() != null)
                {
                    myRigidbody.RegisterTemporaryCollisionException(otherRigidbody, 1);
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }
    }
}

