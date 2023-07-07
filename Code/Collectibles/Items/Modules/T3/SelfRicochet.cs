using Alexandria.ItemAPI;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class SelfRicochet : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SelfRicochet))
        {
            Name = "Ultra Ricochet",
            Description = "Stylish",
            LongDescription = "Doubles layer projectile speed. Player Projectiles bounce off walls, enemies and each other, with force (+Extra Force per stack). Increases rate of fire (+25% hyperbolically per stack) and increases spread. Bounces increase damage by 10% (+10% per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("ultraricochet_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("ultraricochet_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Ultra Ricochet " + h.ReturnTierLabel();
            h.LabelDescription = "Doubles layer projectile speed.\nPlayer Projectiles bounce off walls, enemies and each other\nwith force (" + StaticColorHexes.AddColorToLabelString("+Extra Force", StaticColorHexes.Light_Orange_Hex) + "). Increases rate of fire (" + StaticColorHexes.AddColorToLabelString("+25% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")\n and increases spread.\nBounces increase damage by 10% (" + StaticColorHexes.AddColorToLabelString("+10%", StaticColorHexes.Light_Orange_Hex) + ").";
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.AdditionalWeightMultiplier = 0.8f;
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 12;
            h.EnergyConsumption = 2;
            h.AddToGlobalStorage();
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;

            ricoChetData = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            ricoChetData.damageToPlayer = 0;
            ricoChetData.damage = 4;
            
            ID = h.PickupObjectId;
        }
        public static int ID;
        public static ExplosionData ricoChetData;


        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.03f) { return; }
            p.gameObject.AddComponent<RicoShot>();
            BounceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier.numberOfBounces += (20 * this.ReturnStack(modulePrinterCore));
            bounceProjModifier.bouncesTrackEnemies = true;
            bounceProjModifier.bounceTrackRadius = 5;
            bounceProjModifier.TrackEnemyChance = 1f;
            bounceProjModifier.damageMultiplierOnBounce = 1 + (0.1f);
            bounceProjModifier.OnBounce += () =>
            {
                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, p.sprite.WorldCenter, Quaternion.identity);
                blankObj.transform.localScale = Vector3.one * 0.35f;
                Destroy(blankObj, 2f);
                Exploder.DoRadialPush(p.sprite.WorldCenter, 25, 3);
                Exploder.DoRadialKnockback(p.sprite.WorldCenter, 25, 3);
                Exploder.DoRadialMinorBreakableBreak(p.sprite.WorldCenter, 3);
                Exploder.Explode(p.projectile.sprite.WorldCenter, ricoChetData, p.projectile.sprite.WorldCenter, null, true);
            };
        }

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Accuracy_Process = ProcessAccuracy,
                FireRate_Process = ProcessRoF
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            player.stats.RecalculateStats(player);
            printer.OnPostProcessProjectile += PPP;
        }
        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 2.5f;
        }
        public float ProcessRoF(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f - (f - (f / (1 + 0.25f * this.ReturnStack(modulePrinterCore))));
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.gameObject.AddComponent<RicoShot>();
            p.baseData.speed *= 2;
            p.UpdateSpeed();
            BounceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier.numberOfBounces += (20 * this.ReturnStack(modulePrinterCore));
            bounceProjModifier.bouncesTrackEnemies = true;
            bounceProjModifier.bounceTrackRadius = 5;
            bounceProjModifier.TrackEnemyChance = 1f;
            bounceProjModifier.damageMultiplierOnBounce = 1 + (0.1f * this.ReturnStack(modulePrinterCore));
            bounceProjModifier.OnBounce += () =>
            {
                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
                GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, p.sprite.WorldCenter, Quaternion.identity);
                blankObj.transform.localScale = Vector3.one * 0.35f;
                Destroy(blankObj, 2f);
                Exploder.DoRadialPush(p.sprite.WorldCenter, 25 * this.ReturnStack(modulePrinterCore), 3);
                Exploder.DoRadialKnockback(p.sprite.WorldCenter, 25 * this.ReturnStack(modulePrinterCore), 3);
                Exploder.DoRadialMinorBreakableBreak(p.sprite.WorldCenter, 3);
                Exploder.Explode(p.projectile.sprite.WorldCenter, ricoChetData, p.projectile.sprite.WorldCenter, null, true);
            };
        }


        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            player.stats.RecalculateStats(player);
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            player.stats.RecalculateStats(player);
        }
    }

    internal class RicoShot : MonoBehaviour
    {
        private void Awake()
        {
            this.projectile = base.GetComponent<Projectile>();
            this.projectile.specRigidbody.OnPreRigidbodyCollision += PissAndShit;
            this.projectile.OnHitEnemy += HandleProjectileHitEnemy;

        }

        private void Update()
        {
            this.projectile.collidesOnlyWithPlayerProjectiles = true;
            this.projectile.collidesWithProjectiles = true;
            this.projectile.UpdateCollisionMask();
        }

        private void PissAndShit(SpeculativeRigidbody myBody, PixelCollider myCollider, SpeculativeRigidbody otherBody, PixelCollider otherCollider)
        {
            if (otherBody && otherBody.projectile && myBody.projectile)
            {
                PhysicsEngine.SkipCollision = true;
                myBody.RegisterTemporaryCollisionException(otherBody, 0.1f);
                otherBody.projectile.hitEffects.HandleEnemyImpact(otherBody.projectile.sprite.WorldCenter, otherBody.transform.localEulerAngles.z, otherBody.transform, Vector2.zero, myBody.projectile.LastVelocity, true);
                myBody.projectile.SendInDirection(UnityEngine.Random.insideUnitCircle, false, true);
                otherBody.projectile.SendInDirection(UnityEngine.Random.insideUnitCircle, false, true);
                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");

                GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, myBody.projectile.sprite.WorldCenter, Quaternion.identity);
                blankObj.transform.localScale = Vector3.one * 0.35f;
                Destroy(blankObj, 2f);
                Exploder.DoRadialPush(myBody.projectile.sprite.WorldCenter, 25 , 3);
                Exploder.DoRadialKnockback(myBody.projectile.sprite.WorldCenter, 25 , 3);
                Exploder.DoRadialMinorBreakableBreak(myBody.projectile.sprite.WorldCenter, 3);
                Exploder.Explode(myBody.projectile.sprite.WorldCenter, SelfRicochet.ricoChetData, myBody.projectile.sprite.WorldCenter, null, true);
            }
        }

        private void HandleProjectileHitEnemy(Projectile obj, SpeculativeRigidbody enemy, bool killed)
        {
            PierceProjModifier orAddComponent = obj.gameObject.GetOrAddComponent<PierceProjModifier>();
            orAddComponent.penetratesBreakables = true;
            orAddComponent.penetration++;

            Vector2 dirVec = UnityEngine.Random.insideUnitCircle;
            float num = this.ChanceToSeekEnemyOnBounce;
            Gun possibleSourceGun = obj.PossibleSourceGun;
            if (this.NormalizeAcrossFireRate && possibleSourceGun)
            {
                float num2 = 1f / possibleSourceGun.DefaultModule.cooldownTime;
                if (possibleSourceGun.Volley != null && possibleSourceGun.Volley.UsesShotgunStyleVelocityRandomizer)
                {
                    num2 *= (float)Mathf.Max(1, possibleSourceGun.Volley.projectiles.Count);
                }
                num = Mathf.Clamp01(this.ActivationsPerSecond / num2);
                num = Mathf.Max(this.MinActivationChance, num);
            }
            if (UnityEngine.Random.value < num && enemy.aiActor)
            {
                Func<AIActor, bool> isValid = (AIActor a) => a && a.HasBeenEngaged && a.healthHaver && a.healthHaver.IsVulnerable;
                AIActor closestToPosition = BraveUtility.GetClosestToPosition<AIActor>(enemy.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All), enemy.UnitCenter, isValid, new AIActor[]
                {
                        enemy.aiActor
                });
                if (closestToPosition)
                {
                    dirVec = closestToPosition.CenterPosition - obj.transform.position.XY();
                }
            }
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");

            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, obj.projectile.sprite.WorldCenter, Quaternion.identity);
            blankObj.transform.localScale = Vector3.one * 0.5f;
            Destroy(blankObj, 2f);
            Exploder.DoRadialPush(obj.projectile.sprite.WorldCenter, 25, 3);
            Exploder.DoRadialKnockback(obj.projectile.sprite.WorldCenter, 25, 3);
            Exploder.DoRadialMinorBreakableBreak(obj.projectile.sprite.WorldCenter, 3);
            obj.SendInDirection(dirVec, false, true);
        }

        public float ChanceToSeekEnemyOnBounce = 1;

        public bool NormalizeAcrossFireRate= true;

        public float ActivationsPerSecond = 1000;

        public float MinActivationChance = 1;
        private Projectile projectile;
    }
}

