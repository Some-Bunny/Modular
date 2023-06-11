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
    public class OneShot : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(OneShot))
        {
            Name = "One Shot",
            Description = "One More",
            LongDescription = "Adds 1 (+1 per stack) Shots to your clip." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("oneshot_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("oneshot_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.66f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "One Shot " + h.ReturnTierLabel();
            h.LabelDescription = "Adds 1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") Shots to your clip.";
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
            /*
            foreach (PlayerController p in GameManager.Instance.AllPlayers)
            {
                orig(p, UnityEngine.Object.Instantiate(item.gameObject, Vector2.zero, p.transform).GetComponent<PassiveItem>());
            }
            Destroy(item);
            */

            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Post_Calculation_ClipSize_Process = ProcessClipSize
            };
            modularGunController.statMods.Add(this.gunStatModifier);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (modularGunController.statMods.Contains(this.gunStatModifier)) { modularGunController.statMods.Remove(this.gunStatModifier); }
        }
        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + modulePrinterCore.ReturnStack(this.LabelName);
        }
    } 
}

