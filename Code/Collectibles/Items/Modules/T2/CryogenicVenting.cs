using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class CryogenicVenting : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CryogenicVenting))
        {
            Name = "Cryogenic Venting",
            Description = "Heart Of Cold",
            LongDescription = "Passively freeze enemies near you. (+Freeze Power and Radius per stack).\nFrozen enemies shatter into cold when killed. (+Cold Strength per stack)\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("cryogenicvents_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("cryogenicvents_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Cryogenic Venting " + h.ReturnTierLabel();
            h.LabelDescription = $"Passively freeze enemies near you. ({StaticColorHexes.AddColorToLabelString("+Freeze Power and Radius", StaticColorHexes.Light_Orange_Hex)}).\nFrozen enemies shatter into bursts of cold when killed. ({StaticColorHexes.AddColorToLabelString("+Cold Strength", StaticColorHexes.Light_Orange_Hex)})";
            h.EnergyConsumption = 2;
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);

            h.AdditionalWeightMultiplier = 0.7f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -0.875f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;

            cryoBurst = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            cryoBurst.damageToPlayer = 0;
            cryoBurst.damage = 10;

            var superFreeze = new GameActorFreezeEffect();
            superFreeze.AffectsEnemies = DebuffStatics.frostBulletsEffect.AffectsEnemies;
            superFreeze.AffectsPlayers = DebuffStatics.frostBulletsEffect.AffectsPlayers;
            superFreeze.FreezeAmount = DebuffStatics.frostBulletsEffect.FreezeAmount;
            superFreeze.FreezeCrystals = DebuffStatics.frostBulletsEffect.FreezeCrystals;
            superFreeze.AppliesDeathTint = DebuffStatics.frostBulletsEffect.AppliesDeathTint;
            superFreeze.AppliesOutlineTint = DebuffStatics.frostBulletsEffect.AppliesOutlineTint;
            superFreeze.AppliesTint = DebuffStatics.frostBulletsEffect.AppliesTint;
            superFreeze.PlaysVFXOnActor = DebuffStatics.frostBulletsEffect.PlaysVFXOnActor;
            superFreeze.debrisAngleVariance = DebuffStatics.frostBulletsEffect.debrisAngleVariance;
            superFreeze.FreezeAmount = 100;
            superFreeze.crystalNum = DebuffStatics.frostBulletsEffect.crystalNum * 3;
            superFreeze.crystalVariation = DebuffStatics.frostBulletsEffect.crystalVariation * 3;
            superFreeze.DeathTintColor = DebuffStatics.frostBulletsEffect.DeathTintColor;
            superFreeze.debrisMaxForce = DebuffStatics.frostBulletsEffect.debrisMaxForce;
            superFreeze.debrisMinForce = DebuffStatics.frostBulletsEffect.debrisMinForce;
            superFreeze.duration = DebuffStatics.frostBulletsEffect.duration;
            superFreeze.effectIdentifier = DebuffStatics.frostBulletsEffect.effectIdentifier + "Super";
            superFreeze.resistanceType = DebuffStatics.frostBulletsEffect.resistanceType;
            superFreeze.UnfreezeDamagePercent = DebuffStatics.frostBulletsEffect.UnfreezeDamagePercent;
            superFreeze.vfxExplosion = DebuffStatics.frostBulletsEffect.vfxExplosion;
            superFreeze.stackMode = DebuffStatics.frostBulletsEffect.stackMode;
            superFreeze.maxStackedDuration = DebuffStatics.frostBulletsEffect.maxStackedDuration;
            superFreeze.TintColor = DebuffStatics.frostBulletsEffect.TintColor;

            cryoBurst.freezeEffect = superFreeze;
            cryoBurst.isFreezeExplosion = true;
            cryoBurst.freezeRadius = 4;
            cryoBurst.playDefaultSFX = false;

        }
        public static ExplosionData cryoBurst;
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate += OFU;
            modulePrinter.OnKilledEnemy += OKE;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnFrameUpdate -= OFU;
            modulePrinter.OnKilledEnemy -= OKE;

        }
        public void OKE(ModulePrinterCore printer, PlayerController player, AIActor enemy)
        {
            if (enemy.IsFrozen)
            {
                GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");

                GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, enemy.sprite.WorldCenter, Quaternion.identity);
                blankObj.transform.localScale = Vector3.one;
                Destroy(blankObj, 2f);
                Exploder.DoRadialPush(enemy.sprite.WorldCenter, 25, 3);
                Exploder.DoRadialKnockback(enemy.sprite.WorldCenter, 25, 3);
                Exploder.DoRadialMinorBreakableBreak(enemy.sprite.WorldCenter, 3);

                cryoBurst.freezeRadius = 5 + (this.ReturnStack(printer));
                cryoBurst.damageRadius = 7 + (this.ReturnStack(printer));
                cryoBurst.damage = 10 + (5 * this.ReturnStack(printer));

                Exploder.Explode(enemy.sprite.WorldCenter, cryoBurst, enemy.sprite.WorldCenter, null, true);


                float f = BraveUtility.RandomAngle();
                int amuount = 3 + (3 *(this.ReturnStack(printer)));
                for (int i = 0; i < amuount; i++)
                {
                    GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(223) as Gun).DefaultModule.projectiles[0].gameObject, enemy.sprite.WorldCenter, Quaternion.Euler(0f, 0f, ((360 / amuount) * i) + f), true);
                    Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                    if (component != null)
                    {
                        component.baseData.damage = 5;
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        component.baseData.speed = 30;
                        player.DoPostProcessProjectile(component);
                        var b = player.gameObject.GetOrAddComponent<BounceProjModifier>();
                        b.numberOfBounces += 3;
                        b.damageMultiplierOnBounce = 1.25f;
                        var b_ = player.gameObject.GetOrAddComponent<PierceProjModifier>();
                        b_.penetration += 1;
                    }
                }
            }
        }

        public void OFU(ModulePrinterCore modulePrinter, PlayerController player)
        {
            if(player.CurrentRoom == null) { return;}
            List<AIActor> enemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
            float range = ReturnRange(modulePrinter) + 0.25f;      
            if (UnityEngine.Random.value < 0.075)
            {
                var p = player.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(range, -range), UnityEngine.Random.Range(range, -range));
                GlobalSparksDoer.DoRandomParticleBurst(1, p, p, Vector3.down, 0f, 0.5f, 0.1f, 4f, Color.cyan, GlobalSparksDoer.SparksType.DARK_MAGICKS);
            }
            if (enemies == null || enemies.Count == 0) { return; }
            foreach (var enemy in enemies)
            {
                if (enemy && Vector2.Distance(enemy.sprite.WorldCenter, player.sprite.WorldCenter) < range)
                {
                    enemy.ApplyEffect(DebuffStatics.frostBulletsEffect, (1.25f + (1.25f * this.ReturnStack(modulePrinter))) * Time.deltaTime);
                }
            }
        }

        public float ReturnRange(ModulePrinterCore core)
        {
            return 2.5f + (this.ReturnStack(core));
        }
    }
}

