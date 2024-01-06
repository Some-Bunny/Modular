using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;



namespace ModularMod
{
    /*
    public class PinpointStrike : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PinpointStrike))
        {
            Name = "Pin-Point Strike",
            Description = "On The Dot",
            LongDescription = "Enables Guaranteed Critical Hits, but reduces the amount of damage given by other crit-based modules. Landing 3 consecutive hits slashes at nearly projectiles, reflecting them. (+Slash Effectiveness per stack) Slash recharges after 5 seconds. Increases the probability of crit-based modules appearing.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("perfectstrike_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("perfectstrike_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Pin-Point Strike " + h.ReturnTierLabel();
            h.LabelDescription = StaticColorHexes.AddColorToLabelString("Enables Guaranteed Critical Hits", StaticColorHexes.Light_Purple_Hex) + ", but reduces\nthe amount of damage given by other crit-based modules.\nLanding 3 consecutive hits slashes at nearly projectiles, reflecting them.\n("+StaticColorHexes.AddColorToLabelString("+Slash Effectiveness", StaticColorHexes.Light_Orange_Hex)+")\nSlash recharges after 5 seconds.\n"+ StaticColorHexes.AddColorToLabelString("Increases the probability of crit-based modules appearing.", StaticColorHexes.Pink_Hex);

            h.AddModuleTag(BaseModuleTags.CRIT);
            h.AdditionalWeightMultiplier = 0.45f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 15;

            SlashVFX = (PickupObjectDatabase.GetById(822) as ComplexProjectileModifier).LinearChainExplosionData.effect;

            ID = h.PickupObjectId;
        }
        public static GameObject SlashVFX;

        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            this.CritContext = new CriticalHitComponent.CritContext()
            {
                isGuaranteedCrit = true,
                CritDamageCalc = CritDamageCalc,
            };
            printer.CritContexts.Add(this.CritContext);
            GlobalModuleStorage.AlterModuleWeight += ModuleWeight;
            printer.OnCritProjectileHitEnemy += CritProjectileHit;
            printer.OnModularProjectileDestroyed += ProjectileDestroy;
        }

        public void ProjectileDestroy(ModulePrinterCore core, Projectile p, bool hitEnemy)
        {
            if (hitEnemy == false)
            {
                Hits = 0;
            }
        }

        public bool onCooldown = false;

        public void CritProjectileHit(Projectile projectile, PlayerController player, AIActor enemyHit)
        {
            if (onCooldown == true) { return; }
            Hits++;
            if (Hits == 3)
            {
                onCooldown = true;
                var pos = player.transform.position + Toolbox.GetUnitOnCircleVec3(player.CurrentGun.CurrentAngle, 2);

                var VFX = UnityEngine.Object.Instantiate(SlashVFX, pos, Quaternion.Euler(0,0, player.CurrentGun.CurrentAngle));
                Destroy(VFX, 3);
                Hits = 0;
                player.StartCoroutine(C());

                int a = this.ReturnStack(Stored_Core);
                Exploder.DoRadialPush(player.sprite.WorldCenter, 100 * a, 3);
                Exploder.DoRadialKnockback(player.sprite.WorldCenter, 100 * a, 3);
                Exploder.DoRadialMinorBreakableBreak(player.sprite.WorldCenter, 3);
                AkSoundEngine.PostEvent("Play_OBJ_katana_slash_01", player.gameObject);

                foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
                {
                    if (proj != null)
                    {
                        AIActor enemy = proj.Owner as AIActor;
                        if (proj.GetComponent<BasicBeamController>() == null)
                        {
                            if (Vector2.Distance(proj.sprite ? proj.sprite.WorldCenter : proj.transform.PositionVector2(), player.sprite.WorldCenter) < 3 * a && proj.Owner != null && proj.Owner == enemy)
                            {
                                FistReflectBullet(proj, player.gameActor, proj.baseData.speed *= 1.75f, (proj.sprite.WorldCenter - player.transform.PositionVector2()).ToAngle(), 1f, proj.IsBlackBullet ? ((5 * a) + proj.baseData.speed) * 2 : (5 * a) + proj.baseData.speed, 0f);
                            }
                        }
                    }
                }
            }
        }

        public static void FistReflectBullet(Projectile p, GameActor newOwner, float minReflectedBulletSpeed, float ReflectAngle, float scaleModifier = 1f, float damageModifier = 10f, float spread = 0f)
        {
            p.RemoveBulletScriptControl();
            Vector2 Point1 = Toolbox.GetUnitOnCircle(ReflectAngle, 1);
            p.Direction = Point1;

            if (spread != 0f)
            {
                p.Direction = p.Direction.Rotate(UnityEngine.Random.Range(-spread, spread));
            }
            if (p.Owner && p.Owner.specRigidbody)
            {
                p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
            }
            p.Owner = newOwner;
            p.SetNewShooter(newOwner.specRigidbody);
            p.allowSelfShooting = false;
            if (newOwner is AIActor)
            {
                p.collidesWithPlayer = true;
                p.collidesWithEnemies = false;
            }
            else
            {
                p.collidesWithPlayer = false;
                p.collidesWithEnemies = true;
            }
            if (scaleModifier != 1f)
            {
                SpawnManager.PoolManager.Remove(p.transform);
                p.RuntimeUpdateScale(scaleModifier);
            }
            if (p.Speed < minReflectedBulletSpeed)
            {
                p.Speed = minReflectedBulletSpeed;
            }
            if (p.baseData.damage < ProjectileData.FixedFallbackDamageToEnemies)
            {
                p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
            }
            p.baseData.damage = damageModifier;
            p.UpdateCollisionMask();
            p.ResetDistance();
            p.Reflected();
        }


        public IEnumerator C()
        {
            yield return new WaitForSeconds(5);
            onCooldown = false;
            yield break;
        }
        private int Hits = 0;

        public float ModuleWeight(DefaultModule module, float f)
        {
            if (module.ContainsTag(BaseModuleTags.CRIT)) { return f *= 1.5f; }
            return f;
        }

        public float CritDamageCalc(float baseChance)
        {
            baseChance *= 0.4f;
            baseChance = Mathf.Max(baseChance, 1.1f);
            return baseChance;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.CritContexts.Remove(this.CritContext);
            GlobalModuleStorage.AlterModuleWeight -= ModuleWeight;
            modulePrinter.OnCritProjectileHitEnemy -= CritProjectileHit;
            modulePrinter.OnModularProjectileDestroyed -= ProjectileDestroy;
        }
    }
    */
}

