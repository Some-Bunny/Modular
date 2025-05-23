﻿using Alexandria.ItemAPI;
using JuneLib.Items;
using MonoMod.RuntimeDetour.Platforms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class MirroredSoftware : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MirroredSoftware))
        {
            Name = "Mirrored Software",
            Description = "I'm You, Two",
            LongDescription = "Acts as 2 (+2 per stack) copies of a random active module. Switches on every floor.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("mirrored_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("mirrored_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Mirrored Software " + h.ReturnTierLabel();
            h.LabelDescription = "Acts as 2 (" + StaticColorHexes.AddColorToLabelString("+2", StaticColorHexes.Light_Orange_Hex) + ") copies of\na random active module.\nSwitches on every floor.";

            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AddModuleTag(BaseModuleTags.GENERATION);

            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();
            ID = h.PickupObjectId;

        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            DoSelect(modulePrinter, player);
            modulePrinter.OnNewFloorStarted += ONFS;
            modulePrinter.OnAnyModulePowered += OAMP;
            modulePrinter.OnAnyModuleUnpowered += OAMP;
        }

        public void OAMP(ModulePrinterCore modulePrinter, DefaultModule player, int i)
        {
            DoSelect(modulePrinter, modulePrinter.Owner);
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            if (containerSelected != null)
            {
                var c = containerSelected.FakeCount.Where(self => self.First == "Mirror");

                if (c.Count() > 0)
                {
                    int Count = c.First().Second;
                    c.First().Second = this.ReturnStack(modulePrinter) * 2;
                    containerSelected.defaultModule.OnAnyPickup(modulePrinter, modulePrinter.ModularGunController, player, false);

                    if ((Count < c.First().Second))
                    {
                        AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Active_01", player.gameObject);
                        VFXStorage.DoFancyFlashOfModules((this.ReturnStack(modulePrinter) * 2) - Count, modulePrinter.Owner, containerSelected.defaultModule);
                    }
                    else if ((Count > c.First().Second))
                    {
                        VFXStorage.DoFancyDestroyOfModules(Count - (this.ReturnStack(modulePrinter) * 2), modulePrinter.Owner, containerSelected.defaultModule);
                    }
                }
            }
        }






        public void ONFS(ModulePrinterCore modulePrinter, PlayerController player)
        {
            ORC(modulePrinter, player);
            containerSelected = null;
            cachedContainerSelected = null;
            DoSelect(modulePrinter, player);
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            ORC(modulePrinter, player);
            containerSelected = null;
            modulePrinter.OnNewFloorStarted -= ONFS;
            modulePrinter.OnAnyModulePowered -= OAMP;
            modulePrinter.OnAnyModuleUnpowered -= OAMP;
        }

        public ModulePrinterCore.ModuleContainer containerSelected = null;
        private ModulePrinterCore.ModuleContainer cachedContainerSelected = null;



        public void DoSelect(ModulePrinterCore printer, PlayerController player)
        {
            if (containerSelected != null)
            {
                OnAnyPickup(printer, null, player, false);
                return;
            }

                /*
                if (containerSelected != null) 
                {
                    var c = containerSelected.FakeCount.Where(self => self.First == "Mirror");
                    if (c.Count() > 0)
                    {
                        //DoVFXLogic(printer, player);
                        int Count = c.First().Second;
                        c.First().Second = this.ReturnStack(printer) * 2;
                        containerSelected.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);
                        Debug.Log($"Diff: {Count}| {this.ReturnStack(printer) * 2}");

                        /*
                        if ((Count < c.First().Second))
                        {
                            AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Active_01", player.gameObject);
                            VFXStorage.DoFancyFlashOfModules((this.ReturnStack(printer) * 2) - Count, printer.Owner, containerSelected.defaultModule);
                        }
                        else if ((Count > c.First().Second))
                        {
                            VFXStorage.DoFancyDestroyOfModules(Count - (this.ReturnStack(printer) * 2), printer.Owner, containerSelected.defaultModule);
                        }   

                    }
                    return; 
                }
                */
            if (cachedContainerSelected != null)
            {
                containerSelected = cachedContainerSelected;
                var t = new Tuple<string, int>("Mirror", this.ReturnStack(printer) * 2);
                containerSelected.FakeCount.Add(t);
                containerSelected.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);

                //AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Active_01", player.gameObject);
                //VFXStorage.DoFancyFlashOfModules(this.ReturnStack(printer) * 2, printer.Owner, containerSelected.defaultModule);
                return;
            }

            int rolls = 5;
            bool found = false;
            while (found == false && rolls > 0)
            {

                var list = printer.ModuleContainers.Where(self => self.ActiveCount > 0 && self.LabelName != this.LabelName); //BraveUtility.RandomElement<ModulePrinterCore.ModuleContainer>(printer.ModuleContainers)
                if (list.Count() >0)
                {
                    var c = BraveUtility.RandomElement<ModulePrinterCore.ModuleContainer>(list.ToList());
                    if (c.LabelName != this.LabelName)
                    {
                        found = !found;

                        containerSelected = c;
                        cachedContainerSelected = c;

                        c.FakeCount.Add(new Tuple<string, int>("Mirror", this.ReturnStack(printer) * 2));
                        c.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);

                        //Debug.Log($"InitAct: {this.ReturnStack(printer) * 2}");

                        AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Active_01", player.gameObject);
                        VFXStorage.DoFancyFlashOfModules(this.ReturnStack(printer) * 2, printer.Owner, c.defaultModule);
                    }
                    else
                    {
                        rolls--;
                    }
                }
                else
                {
                    rolls--;
                }
            }
        }


        public void ORC(ModulePrinterCore printer, PlayerController player)
        {
            if (containerSelected == null) { return; }

            for (int i = 0; i < containerSelected.FakeCount.Count; i++)
            {

                var faker = containerSelected.FakeCount[i];
                if (faker.First == "Mirror")
                {
                    int t = containerSelected.FakeCount[i].Second;
                    VFXStorage.DoFancyDestroyOfModules(t, printer.Owner, containerSelected.defaultModule);
                    
                    containerSelected.FakeCount.Remove(containerSelected.FakeCount[i]);
                    containerSelected.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);
                    break;
                }
            }
        }
    }
}

