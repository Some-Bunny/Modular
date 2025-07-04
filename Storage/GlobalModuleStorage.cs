﻿using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModularMod;
using static ModularMod.ModulePrinterCore;
using static ModularMod.DefaultModule;
using UnityEngine;

namespace ModularMod
{
    public static class GlobalModuleStorage
    {
        public static void InitialiseLootTables()
        {
            D_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            {D_Tier_Table.AddItemToPool(entry,  entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { D_Tier_Table.AddItemToPool(entry, 0.5f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { D_Tier_Table.AddItemToPool(entry, 0.045f * entry.AdditionalWeightMultiplier); }

            C_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { C_Tier_Table.AddItemToPool(entry, 0.95f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { C_Tier_Table.AddItemToPool(entry, 0.65f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { C_Tier_Table.AddItemToPool(entry, 0.05f * entry.AdditionalWeightMultiplier); }

            B_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.85f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.75f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { B_Tier_Table.AddItemToPool(entry, 0.1f * entry.AdditionalWeightMultiplier); }

            A_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.35f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.7f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { A_Tier_Table.AddItemToPool(entry, 0.175f * entry.AdditionalWeightMultiplier); }

            S_Tier_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.35f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.75f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { S_Tier_Table.AddItemToPool(entry, 0.225f * entry.AdditionalWeightMultiplier); }

            Fallback_Table = LootUtility.CreateLootTable();
            foreach (var entry in all_Tier_1_Modules)
            { Fallback_Table.AddItemToPool(entry, 0.75f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_2_Modules)
            { Fallback_Table.AddItemToPool(entry, 0.5f * entry.AdditionalWeightMultiplier); }
            foreach (var entry in all_Tier_3_Modules)
            { Fallback_Table.AddItemToPool(entry, 0.2f * entry.AdditionalWeightMultiplier); }
        }
        public static GenericLootTable D_Tier_Table;
        public static GenericLootTable C_Tier_Table;
        public static GenericLootTable B_Tier_Table;
        public static GenericLootTable A_Tier_Table;
        public static GenericLootTable S_Tier_Table;

        public static GenericLootTable Fallback_Table;

        public static GenericLootTable SelectTable(PickupObject.ItemQuality itemQuality)
        {
            if (GameStatsManager.Instance.IsRainbowRun)
            {
                switch (itemQuality)
                {
                    case PickupObject.ItemQuality.D:
                        return B_Tier_Table;
                    case PickupObject.ItemQuality.C:
                        return A_Tier_Table;
                    case PickupObject.ItemQuality.B:
                        return S_Tier_Table;
                    case PickupObject.ItemQuality.A:
                        return S_Tier_Table;
                    case PickupObject.ItemQuality.S:
                        return S_Tier_Table;
                    default:
                        return Fallback_Table;
                }
            }
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
            module.LabelDescription += "\n" + (module.powerConsumptionData.OverridePowerDescriptionLabel != "FUCK" ? module.powerConsumptionData.OverridePowerDescriptionLabel : "Uses " + (module.powerConsumptionData.FirstStack != -420 ? module.powerConsumptionData.FirstStack : module.EnergyConsumption) + (module.powerConsumptionData.OverridePowerDescriptionLabel != "FUCK" ? "" : " (" + StaticColorHexes.AddColorToLabelString((module.powerConsumptionData.AdditionalStacks != -69 ? module.powerConsumptionData.AdditionalStacks : module.EnergyConsumption / 2).ToString(), StaticColorHexes.Light_Orange_Hex) + ")" + SpecialCharactersController.ReturnSpecialCharacter(SpecialCharactersController.SpecialCharacters.POWER))); // Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER)));
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
            return null;
        }
        public static DefaultModule ReturnRandomModule(bool Exclude_T4 = true, bool ExcludeSpecialModules = true)
        {
            var h = (Exclude_T4 == true) ? allModules.Where(self => self.Tier != ModuleTier.Tier_Omega) : allModules;
            var h2 = (ExcludeSpecialModules == true) ? h.Where(self => self.IsSpecialModule == false) : h;
            return BraveUtility.RandomElement<DefaultModule>(h2.Count() > 0 ? h2.ToList() : allModules);
        }
        public static DefaultModule ReturnRandomModule(DefaultModule.ModuleTier tier, bool ExcludeSpecialModules = true)
        {
            switch (tier)
            {
                case DefaultModule.ModuleTier.Tier_1:
                    var h1 = (ExcludeSpecialModules == true) ? all_Tier_1_Modules.Where(self => self.IsSpecialModule == false) : all_Tier_1_Modules;
                    return BraveUtility.RandomElement<DefaultModule>(h1.Count() > 0 ? h1.ToList() : all_Tier_1_Modules);
                case DefaultModule.ModuleTier.Tier_2:
                    var h2 = (ExcludeSpecialModules == true) ? all_Tier_2_Modules.Where(self => self.IsSpecialModule == false) : all_Tier_2_Modules;
                    return BraveUtility.RandomElement<DefaultModule>(h2.Count() > 0 ? h2.ToList() : all_Tier_2_Modules);
                case DefaultModule.ModuleTier.Tier_3:
                    var h3 = (ExcludeSpecialModules == true) ? all_Tier_3_Modules.Where(self => self.IsSpecialModule == false) : all_Tier_3_Modules;
                    return BraveUtility.RandomElement<DefaultModule>(h3.Count() > 0 ? h3.ToList() : all_Tier_3_Modules);
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

        public static bool PlayerHasThisModule(this PlayerController player, int ModuleID)
        {
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    foreach (var container in printerCore.ModuleContainers)
                    {
                        if (container.defaultModule.PickupObjectId == ModuleID) { return true; }
                    }
                }
            }
            return false;
        }
        public static bool PlayerHasThisModule(this PlayerController player, DefaultModule defaultModule)
        {
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    foreach (var container in printerCore.ModuleContainers)
                    {
                        if (container.defaultModule.DisplayName == defaultModule.DisplayName) { return true; }
                    }
                }
            }
            return false;
        }

        public static bool PlayerHasThisModule(int ModuleID)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                for (int c = 0; c < player.passiveItems.Count; c++)
                {
                    var entry = player.passiveItems[c];
                    if (entry is ModulePrinterCore printerCore)
                    {
                        foreach (var container in printerCore.ModuleContainers)
                        {
                            if (container.defaultModule.PickupObjectId == ModuleID) { return true; }
                        }
                    }
                }
            }
            return false;
        }
        public static bool PlayerHasThisModule(DefaultModule defaultModule)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                for (int c = 0; c < player.passiveItems.Count; c++)
                {
                    var entry = player.passiveItems[c];
                    if (entry is ModulePrinterCore printerCore)
                    {
                        foreach (var container in printerCore.ModuleContainers)
                        {
                            if (container.defaultModule.DisplayName == defaultModule.DisplayName) { return true; }
                        }
                    }
                }
            }
            return false;
        }


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


        public static bool AnyPlayerHasModuleWithTag(DefaultModule.BaseModuleTags tag, bool requiresActive = false)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                for (int c = 0; c < player.passiveItems.Count; c++)
                {
                    var entry = player.passiveItems[c];
                    if (entry is ModulePrinterCore printerCore)
                    {
                        foreach (var container in printerCore.ModuleContainers)
                        {
                            if (container.defaultModule.ModuleTags.Contains(tag.ToString()))
                            {
                                return requiresActive == true ? (container.ActiveCount > 0) ? true : false : true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool PlayerHasModuleWithTag(this PlayerController player, DefaultModule.BaseModuleTags tag, bool requiresActive = false)
        {
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    foreach (var container in printerCore.ModuleContainers)
                    {
                        if (container.defaultModule.ModuleTags.Contains(tag.ToString()))
                        {
                            return requiresActive == true ? (container.ActiveCount > 0) ? true : false : true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool AnyPlayerHasModuleWithTag(string tag, bool requiresActive = false)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                for (int c = 0; c < player.passiveItems.Count; c++)
                {
                    var entry = player.passiveItems[c];
                    if (entry is ModulePrinterCore printerCore)
                    {
                        foreach (var container in printerCore.ModuleContainers)
                        {
                            if (container.defaultModule.ModuleTags.Contains(tag))
                            {
                                return requiresActive == true ? (container.ActiveCount > 0) ? true : false : true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool PlayerHasModuleWithTag(this PlayerController player, string tag, bool requiresActive = false)
        {
            for (int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    foreach (var container in printerCore.ModuleContainers)
                    {
                        if (container.defaultModule.ModuleTags.Contains(tag))
                        {
                            return requiresActive == true ? (container.ActiveCount > 0) ? true : false : true;
                        }
                    }
                }
            }
            return false;
        }


        public static bool AnyPlayerHasActiveModule(int ModuleID)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
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
            }
            return false;
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
        public static int PlayerActiveModuleCount(this PlayerController player, int ModuleID)
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
                            return container.ActiveCount;
                        }
                    }
                }
            }
            return 0;
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


        public class ModulePoolType
        {
            public string Identifier = "";
            public GenericLootTable ModuleLootTable;
        }


        public static Func<DefaultModule, bool> AlterModuleIsSelected;
        public static Func<DefaultModule, float, float> AlterModuleWeight;

        public static GameObject ModularSelectByWeight(this GenericLootTable table, bool useSeedRandom = false, Func<DefaultModule, bool> CustomProcess = null, List<DefaultModule> excludedList = null)
        {
            int num = -1;

            if (excludedList == null) { excludedList = new List<DefaultModule>(); }
            List<WeightedGameObject> list = new List<WeightedGameObject>();
            float num2 = 0f;
            float MultWeight = 1f;
            float MultWeightInt = 1f;

            var genericCollection = new WeightedGameObjectCollection();
            genericCollection.elements = table.defaultItemDrops.elements;
            genericCollection.elements.Shuffle();

            for (int i = 0; i < genericCollection.elements.Count; i++)
            {
                WeightedGameObject weightedGameObject = genericCollection.elements[i];
                bool flag = true;
                

                if (weightedGameObject.gameObject != null)
                {
                    var module = weightedGameObject.gameObject.GetComponent<DefaultModule>();
                    if (module != null)
                    {
                        if (excludedList.Contains(module))
                        {
                            continue;
                        }

                        flag = module.IsAvailable();
                        MultWeight = module.ProcessedWeightMultiplier();
                        MultWeightInt = module.ProcessedWeightAdditional();

                        if (AlterModuleWeight != null)
                        {
                            MultWeight = AlterModuleWeight(module, MultWeight);
                        }



                        if (CustomProcess != null)
                        {
                            flag = CustomProcess(module);
                        }
                        if (AlterModuleIsSelected != null)
                        {
                            flag = AlterModuleIsSelected(module);
                        }

                    }
                }

                if (flag)
                {
                    list.Add(weightedGameObject);
                    num2 += (weightedGameObject.weight + MultWeightInt) * MultWeight;
                }
            }

            float num3 = (!useSeedRandom ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num2;
            float num4 = 0f;
            for (int k = 0; k < list.Count; k++)
            {
                var module = list[k].gameObject.GetComponent<DefaultModule>();
                num4 += (list[k].weight + ((module != null ? module.ProcessedWeightAdditional() : 0))) * (module != null ? module.ProcessedWeightMultiplier() : 1);
                if (num4 > num3)
                {
                    num = genericCollection.elements.IndexOf(list[k]);
                    return list[k].gameObject;
                }
            }

            num = genericCollection.elements.IndexOf(list[list.Count - 1]);
            return list[list.Count - 1].gameObject;
        }


    }
}
