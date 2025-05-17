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
    public class RegenerativePlating : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RegenerativePlating))
        {
            Name = "Regenerative Plating",
            Description = "Time Heals All",
            LongDescription = "Entering a new floor restores 1 (+1 per stack) Armor. SScrapping pickups / items has a chance to restore 1 Armor. (+Increased Armor Chance per stack) " + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("selfcare_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("selfcare_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.7f;
            h.LabelName = "Regenerative Plating " + h.ReturnTierLabel();
            h.LabelDescription = "Entering a new floor restores 1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") Armor.\nScrapping pickups / items has a chance to restore 1 Armor.\n("+ StaticColorHexes.AddColorToLabelString("+Increased Armor Chance", StaticColorHexes.Light_Orange_Hex) + ")";

            h.AddModuleTag(BaseModuleTags.GENERATION);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.OverrideScrapCost = 6;

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnNewFloorStarted += ONFS;
            modulePrinter.OnScrapped += OnScrap;

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnNewFloorStarted -= ONFS;
            modulePrinter.OnScrapped -= OnScrap;
        }
        public void ONFS(ModulePrinterCore modulePrinter, PlayerController player)
        {
            player.PlayEffectOnActor(VFXStorage.HealingSparklesVFX, new Vector3(0, 0));
            AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", player.gameObject);
            player.healthHaver.Armor += this.ReturnStack(modulePrinter);
        }
        public void OnScrap(int ScrapCount, ModulePrinterCore core, PlayerController player, Scrapper scrapper)
        {
            if (UnityEngine.Random.value < (1 - (1 / (1 + 0.15f * this.ReturnStack(core)))))
            {
                player.PlayEffectOnActor(VFXStorage.HealingSparklesVFX, new Vector3(0, 0));
                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", player.gameObject);
                player.healthHaver.Armor += 1;
            }
        }
    }
}

