using Alexandria.ItemAPI;
using JuneLib.Items;
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
            h.LabelDescription = "Projectile speed reduced by 33% (" + StaticColorHexes.AddColorToLabelString("-33% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").\nProjectiles grain stronger homing the longer they are in the air.\n(" + StaticColorHexes.AddColorToLabelString("+Faster Homing Ramping", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AdditionalWeightMultiplier = 0.85f;
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
        }
        public static int ID;


        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.033f) { return; }

            int stack = 1;
            p.baseData.speed *= 1 - (1 - (1 / (1 + 0.33f * stack)));
            var ramp = p.gameObject.AddComponent<HomingRamp>();
            ramp.self = p;
            ramp.Angualt_Vel_Gain_PS = 75f * this.ReturnStack(modulePrinterCore);
            ramp.Vision_Per_Second = 6f * this.ReturnStack(modulePrinterCore);
        }


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.speed *= 1 - (1 - (1 / (1 + 0.33f * stack)));
            var ramp = p.gameObject.AddComponent<HomingRamp>();
            ramp.self = p;
            ramp.Angualt_Vel_Gain_PS = 75f * this.ReturnStack(modulePrinterCore);
            ramp.Vision_Per_Second = 6f * this.ReturnStack(modulePrinterCore);

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

