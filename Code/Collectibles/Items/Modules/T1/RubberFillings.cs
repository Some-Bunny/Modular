using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class RubberFillings : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RubberFillings))
        {
            Name = "Rubber Fillings",
            Description = "Ricochet Up",
            LongDescription = "Adds 2 Bounce to player projectiles (+2 per stack), but divides knockback force by (2 + stack) and reduces accuracy by 15% (+15% hyperbolically per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("rubbercase_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static string SFX;

        public static void PostInit(PickupObject v)
        {
            SFX = (PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].objectImpactEventName;
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("rubbercase_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.9f;
            h.LabelName = "Rubber Fillings" + h.ReturnTierLabel();
            h.LabelDescription = "Adds 2 Bounces (" + StaticColorHexes.AddColorToLabelString("+2", StaticColorHexes.Light_Orange_Hex) + ")\nBut divides knockback force by 2 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ")\nand reduces accuracy by 15% " + StaticColorHexes.AddColorToLabelString("+15% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "RubberFillings",
                Accuracy_Process = ProcessFireRate
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }

        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.15f * stack)));
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            BounceProjModifier bounceProjModifier =  p.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier.numberOfBounces += stack * 2;
            p.baseData.force /= stack + 1;
            p.objectImpactEventName = SFX;        
        }
    }
}

