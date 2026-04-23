using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ETGMod;


namespace ModularMod
{
    public class ConcussionGuillotine : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ConcussionGuillotine))
        {
            Name = "Concussion Guillotine",
            Description = "Recoil Up",
            LongDescription = "Deal 3x (+1.5x per stack) damage to stunned enemies, and launch them with massive force when slain.",
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("concussionguillotine_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("concussionguillotine_tier1_alt_module");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Concussion Guillotine " + h.ReturnTierLabel();
            h.AdditionalWeightMultiplier = 0.65f;
            h.LabelDescription = $"Deal 3x ({StaticColorHexes.AddColorToLabelString("+1.5x")}) damage to stunned enemies,\nand launch them with massive force when slain.";

            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.OverrideScrapCost = 6;

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.01f) { return; }
            p.specRigidbody.OnPreRigidbodyCollision += (_1, _2, _3, _4) =>
            {
                if (_3.aiActor != null && _3.aiActor.healthHaver != null && projectile != null)
                {
                    if (_3.aiActor.behaviorSpeculator.IsStunned)
                    {
                        float damage = projectile.baseData.damage;
                        p.baseData.damage *= 5;
                        p.StartCoroutine(FrameDelay(projectile, damage));
                    }
                }
            };
            p.AppliedStunDuration = 1;
            p.StunApplyChance = 0.01f;
            p.AppliesStun = true;
            p.OnWillKillEnemy = (p1, spec) =>
            {
                if (spec.aiActor != null && spec.aiActor.behaviorSpeculator != null)
                {
                    if (spec.aiActor.behaviorSpeculator.IsStunned)
                    {
                        spec.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));

                        SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate onPreRigidbodyCollisionDelegate = (_1, _2, _3, _4) => { };
                        onPreRigidbodyCollisionDelegate = (_1, _2, _3, _4) =>
                        {
                            if (_3 && _3.aiActor && _1 && _1.healthHaver)
                            {
                                AIActor aiActor = _3.aiActor;
                                _1.OnPreRigidbodyCollision -= onPreRigidbodyCollisionDelegate;
                                if (aiActor.IsNormalEnemy && aiActor.healthHaver)
                                {
                                    aiActor.healthHaver.ApplyDamage(_1.healthHaver.GetMaxHealth() * 1.25f, _1.Velocity, "Pinball", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                                }
                            }
                        };

                        spec.OnPreRigidbodyCollision += onPreRigidbodyCollisionDelegate;
                        spec.knockbackDoer.ApplyKnockback(p1.LastVelocity, p1.baseData.force * 5);
                    }
                }
            };
        }

        public static VFXPool Impact = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy;
        public static string ImpactSFX = (PickupObjectDatabase.GetById(539) as Gun).DefaultModule.chargeProjectiles[0].Projectile.enemyImpactEventName;

        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPreEnemyHit += OPC;
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPreEnemyHit -= OPC;
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public float Mult;
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            int stack = this.ReturnStack(modulePrinter);
            Mult = 1.5f + (stack * 1.5f);
        }






        public void OPC(ModulePrinterCore modulePrinterCore, PlayerController playerController, AIActor aIActor, Projectile projectile)
        {
            if (aIActor != null && aIActor.healthHaver != null && projectile != null)
            {
                if (aIActor.behaviorSpeculator.IsStunned)
                {
                    float damage = projectile.baseData.damage;
                    projectile.baseData.damage *= Mult;
                    projectile.StartCoroutine(FrameDelay(projectile, damage));
                }
            }
        }

        public IEnumerator FrameDelay(Projectile p, float DmG)
        {
            VFXPool Ef = p.hitEffects.enemy;
            VFXPool deathEf = p.hitEffects.deathEnemy;
            p.hitEffects.enemy = Impact;
            p.hitEffects.deathEnemy = Impact;
            yield return null;
            p.baseData.damage = DmG;
            p.hitEffects.enemy = Ef;
            p.hitEffects.deathEnemy = deathEf;
        }



        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.AppliedStunDuration = 1;
            p.StunApplyChance = 0.01f;
            p.AppliesStun = true;
            p.OnWillKillEnemy = (p1, spec) => 
            {
                if (spec.aiActor != null && spec.aiActor.behaviorSpeculator != null)
                {
                    if (spec.aiActor.behaviorSpeculator.IsStunned)
                    {
                        spec.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
                        spec.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(spec.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy));
                        spec.knockbackDoer.ApplyKnockback(p1.LastVelocity, p1.baseData.force * 5);
                    }
                }
            };

        }

        private void HandleHitEnemyHitEnemy(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody && otherRigidbody.aiActor && myRigidbody && myRigidbody.healthHaver)
            {
                AIActor aiActor = otherRigidbody.aiActor;
                myRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(myRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy));
                if (aiActor.IsNormalEnemy && aiActor.healthHaver)
                {
                    aiActor.healthHaver.ApplyDamage(myRigidbody.healthHaver.GetMaxHealth() * (Mult * 0.25f), myRigidbody.Velocity, "Pinball", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
            }
        }

        public static Projectile BaseProjectile = (PickupObjectDatabase.GetById(541) as Gun).DefaultModule.chargeProjectiles[0].Projectile.GetComponent<KilledEnemiesBecomeProjectileModifier>().BaseProjectile;
    }
}

