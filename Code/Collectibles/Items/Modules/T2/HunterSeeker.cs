using Alexandria.ItemAPI;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static HutongGames.PlayMaker.Actions.Teleport;


namespace ModularMod
{
    
    public class HunterSeeker : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HunterSeeker))
        {
            Name = "Hunter Seeker",
            Description = "Live For The Hunt",
            LongDescription = "Projectile speed reduced by 33%. Projectiles gain increased homing over time. (+Increased Homing Power Growth per stack).",
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
            h.LabelDescription = $"Reduces projectile speed by 33%.\nProjectiles gain increased homing over time. ({StaticColorHexes.AddColorToLabelString("+Increased Homing Power Growth")})";

            h.AddModuleTag(BaseModuleTags.BASIC);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AdditionalWeightMultiplier = 0.85f;
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            
            ID = h.PickupObjectId;
            
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.02f) { return; }

            int Stack = 1;
            p.baseData.speed *= 0.66f;
            p.UpdateSpeed();
            var homing = p.gameObject.GetOrAddComponent<HomingModifier>();
            homing.AngularVelocity = 20;
            homing.HomingRadius = 10 + (5 * Stack);

            var ramp = p.gameObject.GetOrAddComponent<HomingRamp>();
            ramp.self = p;
            ramp.mod = homing;
            ramp.Angualt_Vel_Gain_PS = 90 * Stack;
            ramp.Vision_Per_Second += Stack * 2;
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
            modulePrinter.OnPostProcessProjectile += PPP;
            /*
            modulePrinter.OnCritProjectileHitEnemy += OCPHE;
            modulePrinter.OnCritProjectileFailRoll += PP;

            this.CritContext = new CriticalHitComponent.CritContext()
            {
                CritChanceCalc = CritCalc,
            };
            modulePrinter.CritContexts.Add(this.CritContext);
            */
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            //modulePrinter.OnCritProjectileFailRoll -= PP;
            //modulePrinter.OnCritProjectileHitEnemy -= OCPHE;

            //modulePrinter.CritContexts.Remove(this.CritContext);
        }





        public static List<AIActor> lockedEnemies = new List<AIActor>();

        public void PP(Projectile p, PlayerController player)
        {


            /*
            if (lockedEnemies.Count > 0)
            {
                //homing.lockOnTarget = lockedEnemies[UnityEngine.Random.Range(0, lockedEnemies.Count)];
                homing.AngularVelocity = 150 + (150 * this.ReturnStack(Stored_Core));
                homing.HomingRadius = 60;
            }
            */
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int Stack = this.ReturnStack(Stored_Core);
            p.baseData.speed *= 0.66f;
            p.UpdateSpeed();
            var homing = p.gameObject.GetOrAddComponent<HomingModifier>();
            homing.AngularVelocity = 60;
            homing.HomingRadius = 5 + (5 * Stack);

            var ramp = p.gameObject.GetOrAddComponent<HomingRamp>();
            ramp.self = p;
            ramp.mod = homing;
            ramp.Angualt_Vel_Gain_PS = 90 * Stack;
            ramp.Vision_Per_Second += Stack * 2;
        }

        public class HomingRamp : MonoBehaviour
        {
            public Projectile self;
            public HomingModifier mod;
            public float Angualt_Vel_Gain_PS = 45;
            public float Vision_Per_Second = 2;


            public void Start()
            {

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

