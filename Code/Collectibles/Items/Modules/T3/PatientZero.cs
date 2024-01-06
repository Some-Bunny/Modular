using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class PatientZero : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PatientZero))
        {
            Name = "Patient Zero",
            Description = "Infected!",
            LongDescription = "Chance to poison enemies on hit, and to spawn poison pools on projectile destruction. (+Poison Chance And Pool Radius per stack). Hurting enemies can spread debuffs to other nearby enemies, breaking their resistances. Slain enemies cause an outbreak, greatly reducing resistances and cuasing panic. (+Virality and Severity per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("pandemic_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("pandemic_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Patient Zero " + h.ReturnTierLabel();
            h.LabelDescription = "Chance to poison enemies on hit, and to spawn\npoison pools on projectile destruction.\n(" + StaticColorHexes.AddColorToLabelString("+Poison Chance And Pool Radius", StaticColorHexes.Light_Orange_Hex) + ").\nHurting enemies can spread debuffs to other\nnearby enemies, breaking their resistances.\nSlain enemies cause an outbreak,\ngreatly reducing resistances and causing panic.\n(" + StaticColorHexes.AddColorToLabelString("+Virality and Severity", StaticColorHexes.Light_Orange_Hex) + ")";
            h.EnergyConsumption = 2;

            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);
            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AdditionalWeightMultiplier = 0.9f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;
            PoisonPoof = (PickupObjectDatabase.GetById(28) as Gun).DefaultModule.finalVolley.projectiles[0].projectiles[2].hitEffects.enemy.effects[0].effects[0].effect;
            fleeData = new FleePlayerData();
            fleeData.StartDistance = 100f;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;

        }
        public static int ID;
        public static GameObject PoisonPoof;
        private static FleePlayerData fleeData;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.075f) { return; }
            int stack = 1;
            p.AppliesPoison = true;
            p.PoisonApplyChance = 0.1f * stack;
            p.healthEffect = DebuffStatics.irradiatedLeadEffect;
            p.OnDestruction += (obj) =>
            {
                if (obj && UnityEngine.Random.value < (0.025f * stack))
                {
                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(JuneLib.Status.EasyGoopDefinitions.PoisonDef).TimedAddGoopCircle(obj.sprite.WorldBottomCenter, 2 + stack, 0.5f + (0.25f * stack), false);
                }
            };
        }

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            printer.OnPostProcessProjectile += PPP;
            printer.OnDamagedEnemy += OnDamagedEnemy;
            printer.OnKilledEnemy += OnKilledEnemy;

        }



        public void OnKilledEnemy(ModulePrinterCore core, PlayerController player, AIActor enemy)
        {
            
            int stack = this.ReturnStack(core);
            if (enemy)
            {
                var enem = enemy.GetAbsoluteParentRoom().activeEnemies;

                if (enemy.m_activeEffects != null || enemy.m_activeEffects.Count > 0)
                {
                    if (ConfigManager.DoVisualEffect == true)
                    {
                        var vfx = UnityEngine.Object.Instantiate(PoisonPoof, enemy.sprite.WorldCenter, Quaternion.identity);
                        vfx.transform.localScale = Vector3.one * 0.5f;
                        Destroy(vfx, 3);
                    }
                    AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Bite_01", enemy.gameObject);
                }
                foreach (var effect in enemy.m_activeEffects)
                {
                    if (!DebuffStatics.BlacklistedEffects.Contains(effect.effectIdentifier) )
                    {
                        foreach (var enemies in enem)
                        {
                            if (enemies.behaviorSpeculator != null)
                            {
                                FleePlayerData data = fleeData;
                                data.Player = player;
                                enemies.behaviorSpeculator.FleePlayerData = data;
                                GameManager.Instance.StartCoroutine(Panic(enemies));
                            }
                            if (enemies.healthHaver.damageTypeModifiers == null) { enemies.healthHaver.damageTypeModifiers = new List<DamageTypeModifier>(); }
                            enemies.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier()
                            {
                                damageType = CoreDamageTypes.Fire,
                                damageMultiplier = 1 + (stack / 4)
                            });
                            enemies.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier()
                            {
                                damageType = CoreDamageTypes.Poison,
                                damageMultiplier = 1 + (stack / 4)
                            }); ;
                            if (enemies != null && Vector2.Distance(enemies.transform.PositionVector2(), enemy.transform.PositionVector2()) < 2.5f + this.ReturnStack(core))
                            {
                                enemies.ApplyEffect(effect);
                            }
                            if (ConfigManager.DoVisualEffect == true)
                            {
                                var vfx = UnityEngine.Object.Instantiate(PoisonPoof, enemies.sprite.WorldCenter, Quaternion.identity);
                                vfx.transform.localScale = Vector3.one * 0.5f;
                                Destroy(vfx, 3);
                            }
                        }

                    }
                }
            }
        }

        public IEnumerator Panic(AIActor enemy)
        {
            float e = 0;
            while (e < 4)
            {
                if (enemy == null) { yield break; }
                e += BraveTime.DeltaTime;
                yield return null;
            }
            if (enemy != null) { enemy.behaviorSpeculator.FleePlayerData = null; }
            yield break;
        }

        private Dictionary<AIActor, int> Infections = new Dictionary<AIActor, int>();


        public void OnDamagedEnemy(ModulePrinterCore core, PlayerController player, AIActor enemy, float damage)
        {
            if (enemy)
            {
                int stack = this.ReturnStack(core);

                enemy.EffectResistances = new ActorEffectResistance[]
                {

                };
                var enem = enemy.GetAbsoluteParentRoom().activeEnemies;
                foreach (var effect in enemy.m_activeEffects)
                {
                    if (!DebuffStatics.BlacklistedEffects.Contains(effect.effectIdentifier))
                    {
                        foreach (var enemies in enem)
                        {
                            if (!Infections.ContainsKey(enemies))
                            {
                                Infections.Add(enemies, 0);
                            }

                            if (Infections[enemies] < 2)
                            {
                                if (UnityEngine.Random.value < (damage / 4))
                                {
                                    Infections[enemies]++;
                                    if (enemies != null && Vector2.Distance(enemies.transform.PositionVector2(), enemy.transform.PositionVector2()) < 2.5f + this.ReturnStack(core))
                                    {
                                        AkSoundEngine.PostEvent("Play_ENM_Tarnisher_Spit_01", enemy.gameObject);
                                        enemies.ApplyEffect(effect);

                                        if (ConfigManager.DoVisualEffect == true)
                                        {
                                            var vfx = UnityEngine.Object.Instantiate(PoisonPoof, enemies.sprite.WorldCenter, Quaternion.identity);
                                            vfx.transform.localScale = Vector3.one * 0.5f;
                                            Destroy(vfx, 3);
                                        }

                                        if (enemies.healthHaver.damageTypeModifiers == null) { enemies.healthHaver.damageTypeModifiers = new List<DamageTypeModifier>(); }
                                        enemies.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier()
                                        {
                                            damageType = CoreDamageTypes.Fire,
                                            damageMultiplier = 1 + (0.1f * stack)
                                        });
                                        enemies.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier()
                                        {
                                            damageType = CoreDamageTypes.Poison,
                                            damageMultiplier = 1 + (0.1f * stack)
                                        });
                                    }
                                }
                            }

                            
                        }
                    }                               
                }
            }
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.AppliesPoison = true;
            p.PoisonApplyChance = 0.1f * stack;
            p.healthEffect = DebuffStatics.irradiatedLeadEffect;
            p.OnDestruction += (obj) =>
            {
                if (obj && UnityEngine.Random.value < (0.025f * stack))
                {
                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(JuneLib.Status.EasyGoopDefinitions.PoisonDef).TimedAddGoopCircle(obj.sprite.WorldBottomCenter, stack, 0.5f + (0.25f * stack), false);
                }
            };
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnDamagedEnemy -= OnDamagedEnemy;
            modulePrinter.OnKilledEnemy -= OnKilledEnemy;
        }
    }
}

