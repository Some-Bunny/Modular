using Alexandria.ItemAPI;
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
using static ETGMod;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class JackHammer : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(JackHammer))
        {
            Name = "Jack Hammer",
            Description = "Hitstop",
            LongDescription = "Disables Contact Damage. Deal contact damage that scales with your velocity. Holding Reload for 0.5 seconds charges up a superpowered dodgeroll with increased damage and creates projectiles on impact, but temporarily disables your weapon. Super roll recharges after 7.5 seconds. (+Contact Damage and Projectiles Created per stack)" ,
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("jackhammer_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("jackhammer_t3_alt_module");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Jack Hammer " + h.ReturnTierLabel();
            h.LabelDescription = $"{StaticColorHexes.AddColorToLabelString("Disables Contact Damage", StaticColorHexes.Light_Green_Hex)}.\nDeal contact damage that scales with your velocity. Holding reload for 0.5 seconds\ncharges up a superpowered dodgeroll\nwith increased damage and creates projectiles on impact,\nbut temporarily disables your weapon.\nSuper roll recharges after 7.5 seconds.\n({StaticColorHexes.AddColorToLabelString("+Contact Damage And Projectiles Created")})";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AdditionalWeightMultiplier = 0.7f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);
            ID = h.PickupObjectId;
        }
        public static int ID;
        public float ModifySpeed(Vector2 currentVelocity, ModulePrinterCore core, PlayerController player)
        {
            return 0.15f;
        }

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.VoluntaryMovement_Modifier += ModifySpeed;
            player.specRigidbody.OnRigidbodyCollision += HandleRigidbodyCollision;
            player.m_additionalReceivesTouchDamage = false;
            printer.OnFrameUpdate += OFU;
            player.OnPreDodgeRoll += RollStarted;

        }
        private StatModifier statModifier;
        public void RollStarted(PlayerController playerController)
        {
            if (isReady)
            {
                statModifier = new StatModifier()
                {
                    amount = 3,
                    ignoredForSaveData = true,
                    modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE,
                    statToBoost = PlayerStats.StatType.DodgeRollDistanceMultiplier,
                };
                AkSoundEngine.PostEvent("Play_BOSS_RatMech_Cannon_01", playerController.gameObject);
                if (Stored_Core.Owner.ownerlessStatModifiers == null)
                {
                    Stored_Core.Owner.ownerlessStatModifiers = new List<StatModifier>() { };
                }
                Stored_Core.Owner.ownerlessStatModifiers.Add(statModifier);
                Stored_Core.Owner.stats.RecalculateStats(Stored_Core.Owner);

                Stored_Core.Owner.StartCoroutine(DoRoll());
            }
        }
        public IEnumerator DoRoll()
        {
            Stored_Core.Owner.sprite.usesOverrideMaterial = true;
            Stored_Core.Owner.SetOverrideShader(ShaderCache.Acquire("Brave/ItemSpecific/MetalSkinShader"));
            HoldTime = 0;
            Cooldown = 7.5f;
            ForceNotCooldown = true;
            cooldownOff = true;
            yield return null;
            Stored_Core.Owner.healthHaver.IsVulnerable = false;
            var __ = StaticExplosionDatas.CopyFields(StaticExplosionDatas.genericLargeExplosion);
            __.ignoreList = new List<SpeculativeRigidbody> { this.Stored_Core.Owner.specRigidbody };
            __.damage = 5;
            Exploder.Explode(this.Stored_Core.Owner.sprite.WorldCenter, __, Vector2.zero);

            while (Stored_Core.Owner.IsDodgeRolling)
            {
                Stored_Core.Owner.healthHaver.IsVulnerable = false;
                GlobalSparksDoer.DoRandomParticleBurst(1, Stored_Core.Owner.sprite.WorldBottomLeft, Stored_Core.Owner.sprite.WorldTopRight,
                Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 0.1f),
                2.5f, 0.05f, null, 0.5f, Stored_Core.Owner.IsUsingAlternateCostume ? Color.green : Color.cyan * 3, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                yield return null;
            }
            isReady = false;
            ForceNotCooldown = false;
            Stored_Core.Owner.ownerlessStatModifiers.Remove(statModifier);
            Stored_Core.Owner.stats.RecalculateStats(Stored_Core.Owner);
            float f = 0;
            while (f < 0.5f)
            {
                Stored_Core.Owner.CurrentStoneGunTimer = 2.5f;
                Stored_Core.Owner.healthHaver.IsVulnerable = false;
                f += BraveTime.DeltaTime;
                yield return null;
            }
            Stored_Core.Owner.healthHaver.IsVulnerable = true;
            Stored_Core.Owner.ClearOverrideShader();
            AkSoundEngine.PostEvent("Play_OBJ_metalskin_end_01", Stored_Core.Owner.gameObject);
        }


        private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            int stack = this.ReturnStack(Stored_Core);


            if (rigidbodyCollision.OtherRigidbody)
            {
                bool isMoving = Stored_Core.Owner.Velocity.magnitude > 0;
                if (isMoving)
                {
                    var p = (PickupObjectDatabase.GetById(Stored_Core.Owner.IsUsingAlternateCostume ? DefaultArmCannonAlt.ID : DefaultArmCannon.ID) as Gun).DefaultModule.projectiles[0];
                    var d = rigidbodyCollision.OtherRigidbody.UnitCenter - Stored_Core.Owner.specRigidbody.UnitCenter;
                    float damage = Stored_Core.Owner.Velocity.magnitude * (2f + stack) * (ForceNotCooldown  ? 2.25f : 1);

                    Vector2 v = BraveMathCollege.ClosestPointOnRectangle(Stored_Core.Owner.specRigidbody.GetUnitCenter(ColliderType.HitBox), rigidbodyCollision.OtherRigidbody.UnitBottomLeft, rigidbodyCollision.OtherRigidbody.UnitDimensions);
                    SpawnManager.SpawnVFX((GameObject)BraveResources.Load("Global VFX/VFX_DodgeRollHit", ".prefab"), v, Quaternion.identity, true);
                    if (rigidbodyCollision.OtherRigidbody.aiActor)
                    {
                        AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", Stored_Core.Owner.gameObject);
                        if (ForceNotCooldown)
                        {
                            float t = BraveUtility.RandomAngle();
                            float div = 360f / (3 + stack);
                            for (int i = 0; i < 3 + stack; i++)
                            {
                                var PP = SpawnManager.SpawnProjectile(p.gameObject, v, Quaternion.Euler(0, 0, t + (div * i))).GetComponent<Projectile>();
                                if (PP)
                                {
                                    PP.baseData.damage = 7;
                                    PP.Owner = Stored_Core.Owner;
                                    PP.Shooter = Stored_Core.Owner.specRigidbody;
                                    PP.baseData.speed = 25;
                                    PP.UpdateSpeed();
                                    var _ =  PP.gameObject.AddComponent<PierceProjModifier>();
                                    _.penetratesBreakables = true;
                                    _.penetration = 3;
                                    Stored_Core.Owner.DoPostProcessProjectile(PP);
                                }
                            }
                        }

                        rigidbodyCollision.OtherRigidbody.aiActor.healthHaver.ApplyDamage(damage, Stored_Core.Owner.Velocity, "BONK");
                        if (rigidbodyCollision.OtherRigidbody.aiActor.knockbackDoer)
                        {
                            rigidbodyCollision.OtherRigidbody.aiActor.knockbackDoer.ApplyKnockback(d, 100);
                        }
                       
                    }
                    if (rigidbodyCollision.OtherRigidbody.majorBreakable)
                    {

                        if (ForceNotCooldown)
                        {
                            float t = BraveUtility.RandomAngle();
                            float div = 360f / (3 + stack);
                            for (int i = 0; i < 3 + stack; i++)
                            {
                                var PP = SpawnManager.SpawnProjectile(p.gameObject, v, Quaternion.Euler(0, 0, t + (div * i))).GetComponent<Projectile>();
                                if (PP)
                                {
                                    PP.baseData.damage = 7;
                                    PP.Owner = Stored_Core.Owner;
                                    PP.Shooter = Stored_Core.Owner.specRigidbody;
                                    PP.baseData.speed = 25;
                                    PP.UpdateSpeed();
                                    var _ = PP.gameObject.AddComponent<PierceProjModifier>();
                                    _.penetratesBreakables = true;
                                    _.penetration = 3;
                                    Stored_Core.Owner.DoPostProcessProjectile(PP);
                                }
                            }
                        }
                        rigidbodyCollision.OtherRigidbody.majorBreakable.ApplyDamage(damage, Stored_Core.Owner.Velocity, false);
                    }
                }
            }
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.VoluntaryMovement_Modifier -= ModifySpeed;
            player.specRigidbody.OnRigidbodyCollision -= HandleRigidbodyCollision;
            player.m_additionalReceivesTouchDamage = true;
            modulePrinter.OnFrameUpdate -= OFU;
            player.OnPreDodgeRoll -= RollStarted;
        }



        public void OFU(ModulePrinterCore modulePrinter, PlayerController player)
        {
            if (Cooldown > 0)
            {
                if (!ForceNotCooldown)
                {
                    Cooldown -= BraveTime.DeltaTime;
                }
                return;
            }
            if (cooldownOff == true)
            {
                cooldownOff = false;
                AkSoundEngine.PostEvent("Play_ITM_Folding_Table_Use_01", player.gameObject);
                AkSoundEngine.PostEvent("Play_ITM_Folding_Table_Use_01", player.gameObject);
                for (int i = 0; i < 32; i++)
                {
                    GlobalSparksDoer.DoRandomParticleBurst(1, player.sprite.WorldCenter + new Vector2(-0.5f, 0.5f), player.sprite.WorldCenter + new Vector2(0.5f, 0.5f),
                        BraveUtility.RandomVector2(new Vector2(-1, -1), new Vector2(1, 1)),
                        0f,
                        0.5f,
                        null,
                        UnityEngine.Random.Range(0.8f, 1.4f),
                        Stored_Core.Owner.IsUsingAlternateCostume ? Color.green : Color.cyan,
                        GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                }
            }

            if (player.m_activeActions != null)
            {
                if (HoldTime < 0.5f)
                {
                    bool wasPressed = player.m_activeActions.ReloadAction.State;
                    if (wasPressed)
                    {
                        HoldTime += BraveTime.DeltaTime;
                        GlobalSparksDoer.DoRandomParticleBurst(1, player.sprite.WorldCenter, player.sprite.WorldCenter,
                            Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), (5f * HoldTime) + 3),
                            1f, 0.05f, 0.05f, 0.5f, Stored_Core.Owner.IsUsingAlternateCostume ? Color.green : Color.cyan * 3, GlobalSparksDoer.SparksType.FLOATY_CHAFF);

                    }
                    else
                    {
                        isReady = false;
                        HoldTime = 0;

                        if (t > 1.25f)
                        {
                            t -= 1.25f;
                            for (int i = 0; i < 24; i++)
                            {
                                GlobalSparksDoer.DoRandomParticleBurst(1, Stored_Core.Owner.sprite.WorldCenter, Stored_Core.Owner.sprite.WorldCenter,
                                    Toolbox.GetUnitOnCircle(15 * i, 1), 1f, 0.05f, 0.125f, 0.75f, Stored_Core.Owner.IsUsingAlternateCostume ? Color.green : Color.cyan, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                            }
                        }

                    }
                }
                else
                {
                    if (!isReady)
                    {
                        isReady = true;
                        Stored_Core.ModularGunController.gun.DoChargeCompletePoof();
                    }

                    t += BraveTime.DeltaTime;
                    if (t > 0.125f)
                    {
                        t -= 0.125f;
                        for (int i = 0; i < 24; i++)
                        {
                            GlobalSparksDoer.DoRandomParticleBurst(1, Stored_Core.Owner.sprite.WorldCenter, Stored_Core.Owner.sprite.WorldCenter,
                                Toolbox.GetUnitOnCircle(15 * i, 4), 1f, 0.05f, 0.125f, 0.25f, Color.yellow, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        }
                    }
                }
            }
        }
        float t = 0;
        private bool cooldownOff = false;
        private bool isReady = false;
        private bool ForceNotCooldown = false;
        private float Cooldown = 0;
        private float HoldTime = 0;

    }
}

