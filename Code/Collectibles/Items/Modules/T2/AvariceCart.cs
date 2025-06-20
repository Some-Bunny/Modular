﻿using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class AvariceCart : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(AvariceCart))
        {
            Name = "Avarice Cart",
            Description = "MONEY",
            LongDescription = "Grants 50 Casings, 1 Key and 2 Blanks on pickup. Enemies have a 6% chance of dropping a casing when killed (+6% per stack hyperbolically).\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("avarice_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("avarice_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Avarice Cart " + h.ReturnTierLabel();
            h.LabelDescription = "Grants 50 Casings, 1 Key and 2 Blanks on pickup.\nEnemies have a 6% chance of\ndropping an additional casing when killed (" + StaticColorHexes.AddColorToLabelString("+6% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.GENERATION);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AdditionalWeightMultiplier = 0.6f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.IsUncraftable = true;
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnKilledEnemy += OKE;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnKilledEnemy -= OKE;
        }

        public void OKE(ModulePrinterCore modulePrinter, PlayerController player, AIActor aIActor)
        {
            if (UnityEngine.Random.value < 1 - (1 / (1 + 0.06f * this.ReturnStack(modulePrinter))))
            {
                LootEngine.SpawnCurrency(aIActor.sprite.WorldBottomCenter, 1);
            }
        }

        public override void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Pickup"));
            tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
            component.PlaceAtPositionByAnchor(player.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            component.UpdateZDepth();
            Destroy(gameObject, 2);

            AkSoundEngine.PostEvent("Play_OBJ_coin_large_01", player.gameObject);
            player.carriedConsumables.Currency += 50;//IsTruePickup == true ? 50: 10;
            player.carriedConsumables.KeyBullets += 1;//IsTruePickup == true ? 1 : 0;
            player.Blanks += 2;
        }
    }
}

