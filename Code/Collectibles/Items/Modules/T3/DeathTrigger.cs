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
using UnityEngine.UI;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;
using Gungeon;
using static ModularMod.DebuffStuff;
using UnityEngine.SocialPlatforms;

namespace ModularMod
{
    public class DeathTrigger : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(DeathTrigger))
        {
            Name = "Death Trigger",
            Description = "Death Cheats",
            LongDescription = "Enemies have a chance to activate 'On Kill Enemy' effects when hit. (+Increased Chance per stack). \nRecharges after 5 seconds.\nSlain enemies fire 4 damaging lines of energy in a + formation. (+Increased Damage per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("deathtrigger_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("deathtrigger_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Death Trigger " + h.ReturnTierLabel();
            h.LabelDescription = $"{StaticColorHexes.AddColorToLabelString("Enemies have a chance to activate 'On Kill Enemy' effects when hit", StaticColorHexes.Green_Hex)}. ({StaticColorHexes.AddColorToLabelString("+Increased Chance", StaticColorHexes.Light_Orange_Hex)})\nRecharges after 5 seconds.\nSlain enemies fire 4 damaging lines of energy in a + formation. ({StaticColorHexes.AddColorToLabelString("+Increased Damage", StaticColorHexes.Light_Orange_Hex)})";

            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AdditionalWeightMultiplier = 0.75f;
            h.EnergyConsumption = 3;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;

            deathtriggerMarl = new GameActorDecorationEffect();
            deathtriggerMarl.AffectsEnemies = true;
            deathtriggerMarl.PlaysVFXOnActor = false;
            deathtriggerMarl.effectIdentifier = "mdl_candeathtrigger";
            deathtriggerMarl.PlaysVFXOnActor = false;
            deathtriggerMarl.stackMode = GameActorEffect.EffectStackingMode.Ignore;
            deathtriggerMarl.duration = 7.5f;
            deathtriggerMarl.TintColor = new Color(0.6f, 0.94f, 1, 1);
            deathtriggerMarl.AppliesTint = false;


            GameObject VFX = new GameObject("DeathCrossVFX");
            FakePrefab.DontDestroyOnLoad(VFX);
            FakePrefab.MarkAsFakePrefab(VFX);
            var tk2d = VFX.AddComponent<tk2dSprite>();
            tk2d.usesOverrideMaterial = true;
            tk2d.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutEmissive");
            tk2d.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
            tk2d.renderer.material.SetFloat("_EmissivePower", 10);
            tk2d.renderer.material.SetFloat("_EmissiveColorPower", 30);

            tk2d.Collection = StaticCollections.VFX_Collection;
            tk2d.SetSprite(StaticCollections.VFX_Collection.GetSpriteIdByName("marksmanhit_005"));
            var tk2dAnim = VFX.AddComponent<tk2dSpriteAnimator>();

            tk2dAnim.Library = StaticCollections.Generic_VFX_Animation;
            DeathCrossVFX = VFX;

        }
        private static GameObject DeathCrossVFX;
        public static GameActorDecorationEffect deathtriggerMarl;

        public static int ID;
        private float math;
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            math = (0.0125f * this.ReturnStack(modulePrinter));
            base.OnAnyPickup(modulePrinter, modularGunController, player, IsTruePickup);
        }

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            math = (0.0125f * this.ReturnStack(printer));
            printer.OnDamagedEnemy += ODE;
            printer.OnKilledEnemy += OKE;

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnDamagedEnemy -= ODE;
            modulePrinter.OnKilledEnemy -= OKE;
        }

        public void ODE(ModulePrinterCore printer, PlayerController player, AIActor enemy, float Damage)
        {
            if (enemy.healthHaver && hasEffect(enemy) == false)
            {
                if (UnityEngine.Random.value < (Damage * math))
                {
                    enemy.ApplyEffect(deathtriggerMarl);
                    var fx = enemy.PlayEffectOnActor(DeathCrossVFX, new Vector3(0, 1));
                    fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("deathCross");
                    var p = enemy.sprite.WorldCenter;
                    AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Blast_01", enemy.gameObject);

                    for (int i = 0; i < 8; i++)
                    {
                        GlobalSparksDoer.DoRandomParticleBurst(1, p, p, new Vector2(1, 1) * i, 12, 0.5f, 0.1f, 3, new Color(3, 0, 2.1f), GlobalSparksDoer.SparksType.DARK_MAGICKS);
                        GlobalSparksDoer.DoRandomParticleBurst(1, p, p, new Vector2(-1, 1) * i, 12, 0.5f, 0.1f, 3, new Color(3, 0, 2.1f), GlobalSparksDoer.SparksType.DARK_MAGICKS);
                        GlobalSparksDoer.DoRandomParticleBurst(1, p, p, new Vector2(1, -1) * i, 12, 0.5f, 0.1f, 3, new Color(3, 0, 2.1f), GlobalSparksDoer.SparksType.DARK_MAGICKS);
                        GlobalSparksDoer.DoRandomParticleBurst(1, p, p, new Vector2(-1, -1) * i, 12, 0.5f, 0.1f, 3, new Color(3, 0, 2.1f), GlobalSparksDoer.SparksType.DARK_MAGICKS);

                    }
                    /*
                    Delegate dodge = enemy.healthHaver.GetEventDelegate("OnPreDeath");
                    if (dodge != null)
                    {
                        dodge.DynamicInvoke(new object[] { Vector2.zero });
                    }
                    */

                    Delegate OKE = player.GetEventDelegate("OnKilledEnemy");
                    if (OKE != null)
                    {
                        OKE.DynamicInvoke(new object[] { player });
                    }

                    Delegate OKEC = player.GetEventDelegate("OnKilledEnemyContext");
                    if (OKEC != null)
                    {
                        OKEC.DynamicInvoke(new object[] { player, enemy.healthHaver });
                    }
                }
            }         
        }

        private bool hasEffect(GameActor gameActor)
        {
            for (int i = 0; i < gameActor.m_activeEffects.Count; i++)
            {
                if (gameActor.m_activeEffects[i] == deathtriggerMarl)
                {
                    return true;
                }
            }
            return false;
        }

        public void OKE(ModulePrinterCore printer, PlayerController player, AIActor enemy)
        {
            AkSoundEngine.PostEvent("Play_WPN_Vorpal_Shot_Critical_01", enemy.gameObject);
            for (int i = 0; i < 4; i++)
            {
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(153) as Gun).DefaultModule.projectiles[0].gameObject, enemy.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (90 * i)), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.baseData.damage = 2.5f + (2.5f * this.ReturnStack(printer));
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    player.DoPostProcessProjectile(component);              
                    var b = component.gameObject.GetOrAddComponent<PierceProjModifier>();
                    b.penetration += 5;
                }
            }
        }
    }
}

