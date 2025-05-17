using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;


namespace ModularMod
{
    public class TheScavenger : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TheScavenger))
        {
            Name = "The Scavenger",
            Description = "Recycling",
            LongDescription = "Allows you to scrap Guns. Grants 5% (+5% per stack) and 2.5% movement speed per Scrap." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("scavenger_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("scavenger_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "The Scavenger " + h.ReturnTierLabel();
            h.LabelDescription = "Allows you to scrap Guns.\nGrants +5% damage ("+ StaticColorHexes.AddColorToLabelString("+5%", StaticColorHexes.Light_Orange_Hex) + ") and +2.5% movement speed per Scrap.";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.GENERATION);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AdditionalWeightMultiplier = 0.7f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.AdditionalWeightMultiplier *= 0.75f;

            ID = h.PickupObjectId;

        }
     
        public static int ID;




        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            Scrapper.OverrideCustomScrapCheck += ReturnScrapForGun;
            modulePrinter.VoluntaryMovement_Modifier += MovementMod;
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public int ReturnScrapForGun(PickupObject obj, int currentCost)
        {
            if (obj is Gun)
            {
                return Scrapper.ReturnAmountBasedOnTier(obj.quality);
            }
            return currentCost; //ALWAYS RETURN CURRENT COST IF YOUR CONDITION ISNT FULFILLED
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            float maths = 1 + (0.05f * GlobalConsumableStorage.GetConsumableOfName("Scrap") * this.ReturnStack(modulePrinterCore));
            float maths_ = 1 + (0.0125f * GlobalConsumableStorage.GetConsumableOfName("Scrap") * this.ReturnStack(modulePrinterCore));

            p.baseData.damage *= maths;
            p.baseData.force *= maths_;
            p.RuntimeUpdateScale(maths_);
        }

        public Vector2 MovementMod(Vector2 currentVel, ModulePrinterCore core, PlayerController p)
        {
            return currentVel *= 1 + (0.025f * GlobalConsumableStorage.GetConsumableOfName("Scrap"));
        }


        public int MSC(int amountOfScrap, ModulePrinterCore core, PlayerController player, Scrapper scrapper)
        {
            return amountOfScrap += 1;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            Scrapper.OverrideCustomScrapCheck -= ReturnScrapForGun;
            modulePrinter.VoluntaryMovement_Modifier -= MovementMod;
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

    }
}

