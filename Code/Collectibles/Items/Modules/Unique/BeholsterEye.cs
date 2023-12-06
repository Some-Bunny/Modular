using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class BeholsterEye : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BeholsterEye))
        {
            Name = "Beholsters Eye",
            Description = "Whizz Up",
            LongDescription = "On reloading your gun, spawns 1 (+1 per stack) beadie." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_Unique_Collection,
            ManualSpriteID = StaticCollections.Module_Unique_Collection.GetSpriteIdByName("beholstereye_u_module"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.Tier = ModuleTier.Unique;
            h.LabelName = "Beholsters Eye " + h.ReturnTierLabel();
            h.LabelDescription = "On reloading your gun,\nspawns 1 ("+StaticColorHexes.AddColorToLabelString("+1")+") beadie.";
            h.IsSpecialModule = true;   
            
            h.SetTag("modular_module");
            h.AddColorLight(new Color(1, 0.3f, 0));
            h.Offset_LabelDescription = new Vector2(0.25f, -0.125f);
            h.Offset_LabelName = new Vector2(0.25f, 2.25f);
            h.Label_Background_Color_Override = new Color32(255, 80, 0, 100);
            h.EnergyConsumption = 1;
            ID = h.PickupObjectId;
            BeadieProjectile = (PickupObjectDatabase.GetById(90) as Gun).DefaultModule.finalProjectile;
        }
        public static int ID;
        public static Projectile BeadieProjectile;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded += OGR;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded -= OGR;
        }

        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            for (int i = 0; i < this.ReturnStack(modulePrinterCore); i++)
            {
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(BeadieProjectile.gameObject, g.sprite.WorldCenter, Quaternion.Euler(0f, 0f, g.CurrentAngle), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    player.DoPostProcessProjectile(component);
                }
            }
        }
    }
}

