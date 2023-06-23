using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModularMod;
using static ModularMod.ModulePrinterCore;
using static ModularMod.DefaultModule;

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
            { D_Tier_Table.AddItemToPool(entry, 0.6f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { D_Tier_Table.AddItemToPool(entry, 0.075f * entry.AdditionalWeightMultiplier); }

            C_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { C_Tier_Table.AddItemToPool(entry, 1f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { C_Tier_Table.AddItemToPool(entry, 0.7f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { C_Tier_Table.AddItemToPool(entry, 0.075f * entry.AdditionalWeightMultiplier); }

            B_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.6f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.7f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.1f * entry.AdditionalWeightMultiplier); }

            A_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.3f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.65f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.14f * entry.AdditionalWeightMultiplier); }

            S_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.25f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.6f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.25f * entry.AdditionalWeightMultiplier); }

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

        public static GenericLootTable SelectTableLichsEyeBullets(PickupObject.ItemQuality itemQuality)
        {
            switch (itemQuality)
            {
                case PickupObject.ItemQuality.D:
                    return C_Tier_Table;
                case PickupObject.ItemQuality.C:
                    return B_Tier_Table;
                case PickupObject.ItemQuality.B:
                    return A_Tier_Table;
                case PickupObject.ItemQuality.A:
                    return S_Tier_Table;
                case PickupObject.ItemQuality.S:
                    return S_Tier_Table;
                default:
                    return Fallback_Table;
            }
        }


        private static void AssignPower(DefaultModule module)
        {
            switch (module.Tier)
            {
                case ModuleTier.Tier_1:
                    module.EnergyConsumption = 1;
                    return;
                case ModuleTier.Tier_2:
                    module.EnergyConsumption = 2;
                    return;
                case ModuleTier.Tier_3:
                    module.EnergyConsumption = 3;
                    return;
                case ModuleTier.Tier_Omega:
                    module.EnergyConsumption = 0;
                    return;

            }
        }
        public static void AddToGlobalStorage(this DefaultModule module)
        {
            if (module.EnergyConsumption == -1)
            {
                AssignPower(module);
            }
            module.LabelDescription += "\n" + (module.powerConsumptionData.OverridePowerDescriptionLabel != "FUCK" ? module.powerConsumptionData.OverridePowerDescriptionLabel : "Uses " + (module.powerConsumptionData.FirstStack != -420 ? module.powerConsumptionData.FirstStack : module.EnergyConsumption) + (module.powerConsumptionData.OverridePowerDescriptionLabel != "FUCK" ? "" : " (" +StaticColorHexes.AddColorToLabelString((module.powerConsumptionData.AdditionalStacks != -69 ? module.powerConsumptionData.AdditionalStacks : module.EnergyConsumption / 2).ToString(), StaticColorHexes.Light_Orange_Hex) + ")" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER)));
            allModules.Add(module);
            if (module.IsUncraftable == false)
            {
                AddNewPagesTiered(module);
            }
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

        public static DefaultModule ReturnModule(DefaultModule mod)
        {
            foreach(var entry in allModules)
            {
                if (mod.LabelName == entry.LabelName)
                { return entry; }
            }
            //allModules.Where(self => self.LabelName == mod.name).First();
            return null;
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

        public static bool PlayerHasActiveModule(this PlayerController player, int ModuleID)
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
                            return (container.ActiveCount > 0) ? true : false;
                        }
                    }
                }
            }
            return false;
        }

        public static ModulePrinterCore PlayerHasCore(this PlayerController player)
        {
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    return printerCore;
                }
            }
            return null;
        }

        public static Scrapper PlayerHasComputerCore(this PlayerController player)
        {
            for (int c = 0; c < player.activeItems.Count; c++)
            {
                var entry = player.activeItems[c];
                if (entry is Scrapper printerCore)
                {
                    return printerCore;
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


      

        private static int AddNewPagesTiered(DefaultModule module)
        {
            if (module.Tier == ModuleTier.Tier_Omega) { return -1; }
            var specList = ReturnPageListTier(module.Tier);
            
            int currentPage = specList.Count > 0 ? specList.Last().Page : 0;
            int LastEntry = specList.Count > 0 ? specList.Last().Entry : -1;
            if (LastEntry > 3)
            {
                currentPage += 1;
                LastEntry = -1;
            }
            QuickAndMessyPage quickAndMessy = new QuickAndMessyPage();
            quickAndMessy.Page = currentPage;
            quickAndMessy.Entry = LastEntry + 1;
            quickAndMessy.module = module;
            specList.Add(quickAndMessy);
            return LastEntry + 1;
        }


        private static List<QuickAndMessyPage> ReturnPageListTier(ModuleTier moduleTier)
        {
            switch (moduleTier)
            {
                case ModuleTier.Tier_1:
                    return pages_T1;
                case ModuleTier.Tier_2:
                    return pages_T2;
                case ModuleTier.Tier_3:
                    return pages_T3;
                case ModuleTier.Tier_Omega:
                    return null;
                default:
                    return pages_T1;
            }
        }

        public static List<QuickAndMessyPage> pages_T1 = new List<QuickAndMessyPage>();
        public static List<QuickAndMessyPage> pages_T2 = new List<QuickAndMessyPage>();
        public static List<QuickAndMessyPage> pages_T3 = new List<QuickAndMessyPage>();

        public class QuickAndMessyPage
        {
            public DefaultModule module;
            public int Page;
            public int Entry;
        }
    }
}
