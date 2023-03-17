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
    public class GlassGenerator : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(GlassGenerator))
        {
            Name = "Glass Generator",
            Description = "See The Use",
            LongDescription = "Grants 2 Glass Guon Stones. Picking up any Module gives 2 (+1 per stack) Glass Guon Stones.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
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
            h.LabelDescription = "Grants 2 Glass Guon Stones.\nPicking up any Module gives 2 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ")\nGlass Guon Stones.";
            h.AddToGlobalStorage();
            h.AdditionalWeightMultiplier = 0.8f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnAnyModuleObtained += OAMO;
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
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnAnyModuleObtained -= OAMO;
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

