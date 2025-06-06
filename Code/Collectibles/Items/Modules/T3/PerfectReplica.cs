using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class PerfectReplica : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(PerfectReplica))
        {
            Name = "Perfect Replica",
            Description = "A Bit Broken",
            LongDescription = "Acts as 1 (+1 per stack) of every module currently active." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("perfectclone_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("perfectclone_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Perfect Replica" + h.ReturnTierLabel();
            h.LabelDescription = "Acts as 1 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") of every module currently active.";

            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AddModuleTag(BaseModuleTags.GENERATION);
            h.AdditionalWeightMultiplier = 0.625f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);
            h.OverrideScrapCost = 25;
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            DoSort(printer, player);
            printer.OnAnyModuleObtained += OAMO;
            printer.OnAnyModulePowered += OAMP;
            printer.OnAnyModuleUnpowered += OAMP;

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnAnyModuleObtained -= OAMO;
            modulePrinter.OnAnyModulePowered -= OAMP;
            modulePrinter.OnAnyModuleUnpowered -= OAMP;
            for (int i = modulePrinter.ModuleContainers.Count - 1; i > -1; i--)
            {
                var moduleContainer = modulePrinter.ModuleContainers[i];
                if (moduleContainer.LabelName != this.LabelName)
                {
                    for (int w = 0; w < modulePrinter.ModuleContainers[i].FakeCount.Count; w++)
                    {
                        var fake = modulePrinter.ModuleContainers[i].FakeCount[w];
                        if (fake.First == "PerfectReplication")
                        {
                            VFXStorage.DoFancyDestroyOfModules(modulePrinter.ModuleContainers[i].FakeCount[w].Second, player, moduleContainer.defaultModule);
                            modulePrinter.ModuleContainers[i].FakeCount.RemoveAt(w);
                        }
                    }
                }
            }
        }

        public void OAMP(ModulePrinterCore modulePrinterCore,DefaultModule defaultModule, int a)
        {
            DoSort(modulePrinterCore, modulePrinterCore.Owner);
        }

        public void OAMO(ModulePrinterCore modulePrinterCore, PlayerController player, DefaultModule defaultModule)
        {
            DoSort(modulePrinterCore, player);
        }


        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            DoSort(modulePrinter, player);
        }

        public void DoSort(ModulePrinterCore modulePrinter, PlayerController player)
        {
            for (int i = modulePrinter.ModuleContainers.Count - 1; i > -1; i--)
            {
                bool has = false;
                var moduleContainer = modulePrinter.ModuleContainers[i];
                if (moduleContainer.LabelName != this.LabelName && moduleContainer.ActiveCount > 0)
                {
                    for (int w = 0; w < modulePrinter.ModuleContainers[i].FakeCount.Count; w++)
                    {
                        var fake = modulePrinter.ModuleContainers[i].FakeCount[w];
                        if (fake.First == "PerfectReplication")
                        {
                            has = true;

                            if (fake.Second > this.ReturnStack(modulePrinter))
                            {
                                VFXStorage.DoFancyDestroyOfModules(fake.Second - this.ReturnStack(modulePrinter), player, moduleContainer.defaultModule);
                            }
                            else
                            {
                                VFXStorage.DoFancyFlashOfModules(this.ReturnStack(modulePrinter) - fake.Second, player, moduleContainer.defaultModule);
                            }
                            fake.Second = this.ReturnStack(modulePrinter);
                        }
                    }
                    if (has == false)
                    {
                        moduleContainer.FakeCount.Add(new Tuple<string, int>("PerfectReplication", this.ReturnStack(modulePrinter)));
                        moduleContainer.defaultModule.OnAnyPickup(modulePrinter, modulePrinter.ModularGunController, player, false);
                        VFXStorage.DoFancyFlashOfModules(this.ReturnStack(modulePrinter), player, moduleContainer.defaultModule);
                    }
                    
                }
                else if (moduleContainer.LabelName != this.LabelName && moduleContainer.ActiveCount == 0)
                {
                    for (int w = 0; w < modulePrinter.ModuleContainers[i].FakeCount.Count; w++)
                    {
                        var fake = modulePrinter.ModuleContainers[i].FakeCount[w];
                        if (fake.First == "PerfectReplication")
                        {
                            VFXStorage.DoFancyDestroyOfModules(modulePrinter.ModuleContainers[i].FakeCount[w].Second, player, moduleContainer.defaultModule);
                            modulePrinter.ModuleContainers[i].FakeCount.RemoveAt(w);
                        }
                    }
                }
            }
        }
    }
}

