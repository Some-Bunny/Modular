using Alexandria.ItemAPI;
using Alexandria.Misc;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class GlassGenerator : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(GlassGenerator))
        {
            Name = "Glass Generator",
            Description = "See The Use",
            LongDescription = "Grants 2 Glass Guon Stones. While active, picking up any Module gives 2 (+1 per stack) Glass Guon Stones. Very slightly increases rate of fire per Glass Guon Stone.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("glassgen_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("glassgen_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Glass Generator " + h.ReturnTierLabel();
            h.LabelDescription = "Grants 2 Glass Guon Stones.\nWhile active, picking up any Module\ngives 2 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") Glass Guon Stones.\nSlightly increases rate of fire per Glass Guon Stone.";
            h.EnergyConsumption = 1;
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.GENERATION);
            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AdditionalWeightMultiplier = 0.8f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(565).gameObject, Vector3.zero, Quaternion.identity);
                PickupObject component3 = gameObject.GetComponent<PickupObject>();
                if (component3 != null)
                {
                    component3.CanBeDropped = false;
                    component3.Pickup(player);
                }
            }
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnAnyModuleObtained += OAMO;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Glass Gen Church",
                FireRate_Process = PFR,
                ChargeSpeed_Process = PFR
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnAnyModuleObtained -= OAMO;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }

        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            return f - (f - (f / (1 + 0.05f * Multiplier(player, modulePrinter))));
        }

        public int Multiplier(PlayerController player, ModulePrinterCore modulePrinter)
        {
            int dM = 0;
            foreach (PassiveItem item in player.passiveItems)
            {
                if (item.PickupObjectId == 565)
                {
                    dM++;
                }
            }
            return dM;
        }

        public void OAMO(ModulePrinterCore modulePrinterCore, PlayerController player, DefaultModule defaultModule)
        {
            int amountToGive = this.ReturnStack(modulePrinterCore) + 1;
            for (int i = 0; i < amountToGive; i++)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(565).gameObject, Vector3.zero, Quaternion.identity);
                PickupObject component3 = gameObject.GetComponent<PickupObject>();
                if (component3 != null)
                {
                    component3.CanBeDropped = false;
                    component3.Pickup(player);
                }
            }
        }
    }
}

