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
            LongDescription = "Adds 1 Pierce, Reduce Damage by 20% (-20% per stack hyperbolically) But each enemy pierced increases projectile damage by 1.75x (+0.5x per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
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
            h.LabelDescription = "Projectile speed reduced by 25% (" + StaticColorHexes.AddColorToLabelString("-25% hyperbolically", StaticColorHexes.Light_Orange_Hex) + "),\nProjectiles grain stronger homing the longer they are in the air (" + StaticColorHexes.AddColorToLabelString("+Faster Homing Ramping", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;
        }
        public static int ID;

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
            p.baseData.speed *= 1 - (1 - (1 / (1 + 0.25f * stack)));
            var ramp = p.gameObject.AddComponent<HomingRamp>();
            ramp.self = p;
            ramp.Angualt_Vel_Gain_PS = 60f * this.ReturnStack(modulePrinterCore);
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

