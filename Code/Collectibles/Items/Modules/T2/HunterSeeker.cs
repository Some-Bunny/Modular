using Alexandria.ItemAPI;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class HunterSeeker : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HunterSeeker))
        {
            Name = "Hunter Seeker",
            Description = "Live For The Hunt",
            LongDescription = "Projectile speed reduced by 33% (-33% hyperbolically per stack). Projectiles grain stronger homing the longer they are in the air. (+Faster Homing Ramping per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("hunterseeker_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("hunterseeker_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Hunter Seeker " + h.ReturnTierLabel();
            h.LabelDescription = "+15% ("+StaticColorHexes.AddColorToLabelString("+5%")+") "+StaticColorHexes.AddColorToLabelString("Crit Chance", StaticColorHexes.Light_Purple_Hex)
                +".\nProjectile speed reduced by 33%.\n"+StaticColorHexes.AddColorToLabelString("Critical Hits", StaticColorHexes.Light_Purple_Hex) 
                +" mark enemies, and makes non-crit projectiles\nhome towards marked enemies. ("+StaticColorHexes.AddColorToLabelString("Homing Power")+").";

            h.AddModuleTag(BaseModuleTags.BASIC);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AdditionalWeightMultiplier = 0.85f;
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            
            ID = h.PickupObjectId;
            
            //ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }
        public static int ID;
        public static MarkedEffect effect = new MarkedEffect()
        {
            effectIdentifier = "dasfafafa",
            AffectsEnemies = true,
            resistanceType = EffectResistanceType.None,
            duration = 7f,
            AppliesTint = true,
            AppliesDeathTint = true,
            OverheadVFX = MarkedEffect.MarkedVFX,
            PlaysVFXOnActor = true,
            AffectsPlayers = false,
            stackMode = GameActorEffect.EffectStackingMode.Refresh,
        };


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnCritProjectileHitEnemy += OCPHE;
            modulePrinter.OnCritProjectileFailRoll += PP;
            //modulePrinter.OnPostProcessProjectile += PPP;

            this.CritContext = new CriticalHitComponent.CritContext()
            {
                CritChanceCalc = CritCalc,
            };
            modulePrinter.CritContexts.Add(this.CritContext);
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            //modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnCritProjectileFailRoll -= PP;
            modulePrinter.OnCritProjectileHitEnemy -= OCPHE;

            modulePrinter.CritContexts.Remove(this.CritContext);
        }

        public float CritCalc(float baseChance)
        {
            return baseChance += 0.1f + (0.05f * this.ReturnStack(Stored_Core));
        }

        public void OCPHE(Projectile projectile, PlayerController player, AIActor enemy)
        {
            effect.M = 0.1f * this.ReturnStack(Stored_Core);
            enemy.ApplyEffect(effect);
        }

        public static List<AIActor> lockedEnemies = new List<AIActor>();

        public void PP(Projectile p, PlayerController player)
        {
            p.baseData.speed *= 0.66f;
            p.UpdateSpeed();
            if (lockedEnemies.Count > 0)
            {
                var homing = p.gameObject.AddComponent<LockOnHomingModifier>();
                homing.lockOnTarget = lockedEnemies[UnityEngine.Random.Range(0, lockedEnemies.Count)];
                homing.AngularVelocity = 150 + (150 * this.ReturnStack(Stored_Core));
                homing.HomingRadius = 60;
            }
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
        }

        public class HomingRamp : MonoBehaviour
        {
            public Projectile self;
            public HomingModifier mod;
            public float Angualt_Vel_Gain_PS = 60;
            public float Vision_Per_Second = 6;


            public void Start()
            {
                if (self != null)
                {
                    mod = self.gameObject.GetOrAddComponent<HomingModifier>();
                }
            }
            public void Update()
            {
                if (mod != null)
                {
                    mod.AngularVelocity += (Angualt_Vel_Gain_PS * BraveTime.DeltaTime);
                    mod.HomingRadius += (Vision_Per_Second * BraveTime.DeltaTime);

                }
            }
        }
    }
}

