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
    public class MirroredSoftware : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MirroredSoftware))
        {
            Name = "Mirrored Software",
            Description = "I'm You, Two",
            LongDescription = "Acts as 2 (+2 per stack) copies of a random active module. Switches at every combat encounter.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
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
            h.LabelDescription = "Acts as 2 (" + StaticColorHexes.AddColorToLabelString("+2", StaticColorHexes.Light_Orange_Hex) + ") copies of\na random active module.\nSwitches at every combat encounter.";

            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();
            ID = h.PickupObjectId;

        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat += OEC;
            modulePrinter.OnRoomCleared += ORC;
        }


        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat -= OEC;
            modulePrinter.OnRoomCleared -= ORC;
        }


        

        public void OEC(ModulePrinterCore printer, Dungeonator.RoomHandler roomHandler, PlayerController player)
        {
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

                        c.FakeCount.Add(new Tuple<string, int>("Mirror", this.ReturnStack(printer) * 2));
                        c.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);


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
        public void ORC(ModulePrinterCore printer, PlayerController player, Dungeonator.RoomHandler roomHandler)
        {
            foreach (var help in printer.ModuleContainers)
            {
                for (int i = 0; i < help.FakeCount.Count; i++)
                {
                    var faker = help.FakeCount[i];
                    if (faker.First == "Mirror")
                    {
                        help.FakeCount.Remove(help.FakeCount[i]);
                        help.defaultModule.OnAnyPickup(printer, printer.ModularGunController, player, false);
                        VFXStorage.DoFancyDestroyOfModules(this.ReturnStack(printer) * 2, printer.Owner, help.defaultModule);
                    }
                }
            }
        }
    }
}

