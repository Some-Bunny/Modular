using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;


namespace ModularMod
{
    public class SupplyDrop : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SupplyDrop))
        {
            Name = "Supply Drop",
            Description = "Free Loot!",
            LongDescription = "Grants 2 Keys, 4 Scrap and 20 Casings on pickup. Air drops 1 (+1 per stack) random pickup every floor. Passively reduces the chance of better modules appearing." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("supplydrop_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("supplydrop_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.5f;
            h.LabelName = "Supply Drop " + h.ReturnTierLabel();
            h.LabelDescription = "Grants 2 Keys, 4 Scrap and 20 Casings on pickup.\nAir drops 1 (" + StaticColorHexes.AddColorToLabelString("+1") + ") random pickup every floor.\n" +StaticColorHexes.AddColorToLabelString("Passively reduces the chance of better modules appearing", StaticColorHexes.Pink_Hex) + ".";
            h.EnergyConsumption = 1f;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.GENERATION);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.OverrideScrapCost = 6;
            h.IsUncraftable = true;
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);


            ID = h.PickupObjectId;
            GlobalModuleStorage.AlterModuleWeight += ModuleWeight;
        }
        public static int ID;
        public static float ModuleWeight(DefaultModule module, float f)
        {
            bool t = module.Tier == ModuleTier.Tier_2 || module.Tier == ModuleTier.Tier_3;
            if (GlobalModuleStorage.PlayerHasThisModule(ID) == true && t == true)
            {
                return f *= 0.8f;
            }
            return f;
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnNewFloorStarted += ONFS;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnNewFloorStarted -= ONFS;
        }
        public void ONFS(ModulePrinterCore modulePrinter, PlayerController player)
        {
            player.StartCoroutine(W(modulePrinter, player, player.sprite.WorldCenter));
        }

        public IEnumerator W(ModulePrinterCore modulePrinter, PlayerController player, Vector2 position)
        {
            yield return new WaitForSeconds(1);
            position = player.sprite.WorldCenter;
            float e = 0;
            for (int i = 0; i < this.ReturnStack(modulePrinter); i++)
            {
                while (e < 0.5f)
                {
                    e += BraveTime.DeltaTime;
                    yield return null;
                }
                e = 0;
                SpawnObject(player, (position + new Vector2(0, -7))+ Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0.5f, 1.75f)));
            }
            B = true;
            yield break;
        }

        public bool B = false;


        public void SpawnObject(PlayerController playerController, Vector2 position)
        {
            switch (UnityEngine.Random.Range(0, 10))
            {
                case 0:
                    SpawnCrate(Scrap.Scrap_ID, playerController, position);
                    return;
                case 1:
                    SpawnCrate(Scrap.Scrap_ID, playerController, position);
                    return;
                case 2:
                    SpawnCrate(Scrap.Scrap_ID, playerController, position);
                    return;
                case 3:
                    SpawnCrate(Scrap.Scrap_ID, playerController, position);
                    return;
                case 4:
                    SpawnCrate(67, playerController, position);
                    return;
                case 5:
                    SpawnCrate(67, playerController, position);
                    return;
                case 6:
                    SpawnCrate(224, playerController, position);
                    return;
                case 7:
                    SpawnCrate(224, playerController, position);
                    return;
                case 8:
                    SpawnCrate(224, playerController, position);
                    return;
                case 9:
                    if (UnityEngine.Random.value < 0.1)
                    {
                        SpawnCrate(CraftingCore.CraftingCoreID, playerController, position);
                    }
                    else
                    {
                        SpawnCrate(Scrap.Scrap_ID, playerController, position);
                    }
                    return;
            }
        }

        private static void SpawnCrate(int item, PlayerController p, Vector3 position)
        {
            GameObject gameObject = (GameObject)BraveResources.Load("EmergencyCrate", ".prefab");
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
            EmergencyCrateController component = gameObject2.GetComponent<EmergencyCrateController>();
            SimplerCrateBehaviour simpleCrate = SimplerCrateBehaviour.TurnIntoSimplerCrate(component);
            simpleCrate.LootID = item;

            simpleCrate.Trigger(new Vector3(-5f, -5f, -5f), position + new Vector3(15f, 15f, 15f), p.CurrentRoom);
        }

        public override void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", player.gameObject);
            GlobalConsumableStorage.AddConsumableAmount("Scrap", 5);
            player.carriedConsumables.KeyBullets += 2;

            AkSoundEngine.PostEvent("Play_OBJ_coin_large_01", player.gameObject);
            player.carriedConsumables.Currency += 20;
        }
    }
}

