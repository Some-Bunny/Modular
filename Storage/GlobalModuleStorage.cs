using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModularMod;
using static ModularMod.ModulePrinterCore;

namespace ModularMod
{
    public static class GlobalModuleStorage
    {
        public static void InitialiseLootTables()
        {
            D_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            {D_Tier_Table.AddItemToPool(entry, 0.9f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { D_Tier_Table.AddItemToPool(entry, 0.4f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { D_Tier_Table.AddItemToPool(entry, 0.05f * entry.AdditionalWeightMultiplier); }

            C_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { C_Tier_Table.AddItemToPool(entry, 1f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { C_Tier_Table.AddItemToPool(entry, 0.65f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { C_Tier_Table.AddItemToPool(entry, 0.05f * entry.AdditionalWeightMultiplier); }

            B_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.55f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.7f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.08f * entry.AdditionalWeightMultiplier); }

            A_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.4f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.65f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.14f * entry.AdditionalWeightMultiplier); }

            S_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.35f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.7f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.2f * entry.AdditionalWeightMultiplier); }

            Fallback_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { Fallback_Table.AddItemToPool(entry, 0.75f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { Fallback_Table.AddItemToPool(entry, 0.5f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { Fallback_Table.AddItemToPool(entry, 0.1f * entry.AdditionalWeightMultiplier); }
        }
        public static GenericLootTable D_Tier_Table;
        public static GenericLootTable C_Tier_Table;
        public static GenericLootTable B_Tier_Table;
        public static GenericLootTable A_Tier_Table;
        public static GenericLootTable S_Tier_Table;

        public static GenericLootTable Fallback_Table;

        public static GenericLootTable SelectTable(PickupObject.ItemQuality itemQuality)
        {
            switch (itemQuality)
            {
                case PickupObject.ItemQuality.D:
                    return D_Tier_Table;
                case PickupObject.ItemQuality.C:
                    return C_Tier_Table;
                case PickupObject.ItemQuality.B:
                    return B_Tier_Table;
                case PickupObject.ItemQuality.A:
                    return A_Tier_Table;
                case PickupObject.ItemQuality.S:
                    return S_Tier_Table;
                default: 
                    return Fallback_Table;
            }
        }


        public static void AddToGlobalStorage(this DefaultModule module)
        {
            allModules.Add(module);
            switch (module.Tier)
            {
                case DefaultModule.ModuleTier.Tier_1:
                    all_Tier_1_Modules.Add(module);
                    break;
                case DefaultModule.ModuleTier.Tier_2:
                    all_Tier_2_Modules.Add(module);
                    break;
                case DefaultModule.ModuleTier.Tier_3:
                    all_Tier_3_Modules.Add(module);
                    break;
                case DefaultModule.ModuleTier.Tier_Omega:
                    all_Tier_Omega_Modules.Add(module);
                    break;
            }              
        }
        public static DefaultModule ReturnRandomModule()
        {
            return BraveUtility.RandomElement<DefaultModule>(allModules);
        }
        public static DefaultModule ReturnRandomModule(DefaultModule.ModuleTier tier)
        {
            switch (tier)
            {
                case DefaultModule.ModuleTier.Tier_1:
                    return BraveUtility.RandomElement<DefaultModule>(all_Tier_1_Modules);
                case DefaultModule.ModuleTier.Tier_2:
                    return BraveUtility.RandomElement<DefaultModule>(all_Tier_2_Modules);
                case DefaultModule.ModuleTier.Tier_3:
                    return BraveUtility.RandomElement<DefaultModule>(all_Tier_3_Modules);
                case DefaultModule.ModuleTier.Tier_Omega:
                    return BraveUtility.RandomElement<DefaultModule>(all_Tier_Omega_Modules);
                default:
                    return BraveUtility.RandomElement<DefaultModule>(all_Tier_1_Modules);
            }
        }

        public static List<DefaultModule> allModules = new List<DefaultModule>();

        public static List<DefaultModule> all_Tier_1_Modules = new List<DefaultModule>();
        public static List<DefaultModule> all_Tier_2_Modules = new List<DefaultModule>();
        public static List<DefaultModule> all_Tier_3_Modules = new List<DefaultModule>();
        public static List<DefaultModule> all_Tier_Omega_Modules = new List<DefaultModule>();


        public static ModulePrinterCore.ModuleContainer PlayerHasModule(this PlayerController player, int ModuleID)
        {
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    foreach (var container in printerCore.ModuleContainers)
                    {
                        if (container.defaultModule.PickupObjectId == ModuleID) { return container; }
                    }
                }
            }
            return null;
        }
        public static ModulePrinterCore.ModuleContainer PhantomAddModule(this PlayerController player, int ModuleID, bool truePickup = true)
        {
            bool b = false;
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    foreach (var container in printerCore.ModuleContainers)
                    {
                        if (container.defaultModule.PickupObjectId == ModuleID) 
                        {
                            b = true;
                            container.Count++;
                            container.defaultModule.OnAnyPickup(printerCore, printerCore.ModularGunController, player, truePickup);
                            return container; 
                        }
                    }
                    if (b == false)
                    {
                        printerCore.AddModule(PickupObjectDatabase.GetById(ModuleID) as DefaultModule, player);
                    }
                }
            }
            return null;
        }
        public static void PhantomRemoveModule(this PlayerController player, int ModuleID, bool truePickup = true)
        {
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    foreach (var container in printerCore.ModuleContainers)
                    {
                        if (container.defaultModule.PickupObjectId == ModuleID)
                        {
                            if (container.Count > 0)
                            {
                                container.Count--;
                                container.defaultModule.OnAnyRemoved(printerCore, printerCore.ModularGunController, player);
                            }
                            if (container.Count == 0)
                            {
                                container.defaultModule.OnLastRemoved(printerCore, printerCore.ModularGunController, player);
                                printerCore.ModuleContainers.Remove(container);
                            }
                        }
                    }      
                }
            }
        }
    }
}
