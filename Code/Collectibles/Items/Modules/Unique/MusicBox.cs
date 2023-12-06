using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class MusicBox : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MusicBox))
        {
            Name = "Music Box",
            Description = "Banger Tunes",
            LongDescription = "On reloading your gun, spawns 1 music box that imitates your attacks. Further stacks simply increases fire rate by 20% (+20% hyperbolically per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_Unique_Collection,
            ManualSpriteID = StaticCollections.Module_Unique_Collection.GetSpriteIdByName("musicbox_u_module"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.Tier = ModuleTier.Unique;
            h.LabelName = "Music Box " + h.ReturnTierLabel();
            h.LabelDescription = "On reloading your gun,\nspawns 1 music box that imitates your attacks.\nFurther stacks simply increases fire rate by 20% ("+StaticColorHexes.AddColorToLabelString("+20% hyperbolically")+").";
            h.IsSpecialModule = true;   
            
            h.SetTag("modular_module");
            h.AddColorLight(new Color(1, 0.3f, 0));
            h.Offset_LabelDescription = new Vector2(0.25f, -0.125f);
            h.Offset_LabelName = new Vector2(0.25f, 2.25f);
            h.Label_Background_Color_Override = new Color32(255, 80, 0, 100);
            h.EnergyConsumption = 1;
            ID = h.PickupObjectId;
            MusicBoxObject = (PickupObjectDatabase.GetById(149) as Gun).ObjectToInstantiateOnReload;
        }
        public static int ID;
        public static GameObject MusicBoxObject;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.gun.ObjectToInstantiateOnReload = MusicBoxObject;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                ChargeSpeed_Process = ProcessFireRate,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            if (stack == 1) { return f; }
            return f - (f - (f / (1 + 0.2f * (stack - 1))));
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.gun.ObjectToInstantiateOnReload = null;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }
    }
}

