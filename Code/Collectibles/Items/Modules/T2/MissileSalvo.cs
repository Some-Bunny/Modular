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
    public class MissileSalvo : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MissileSalvo))
        {
            Name = "Missile Salvo",
            Description = "Rocket Man",
            LongDescription = "20% (+20% per stack) chance to fire a missile when firing your gun.\nChances above 100% are rolled multiple times.",
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("missilesalvo_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("missilesalvo_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Missile Salvo " + h.ReturnTierLabel();
            h.LabelDescription = $"20% ({StaticColorHexes.AddColorToLabelString("+20%")}) chance to fire a missile when firing your gun.\nChances above 100% are rolled multiple times.";

            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.OnGunFired += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modularGunController.OnGunFired -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            var s = this.ReturnStack(modulePrinterCore);
            float chance = (0.2f * s);
            float _ = chance;
            for (int i = 0; i < _; i++)
            {
                if (UnityEngine.Random.value < chance)
                {
                    float acc = modulePrinterCore.ModularGunController.GetAccuracy(20);
                    var p = SpawnManager.SpawnProjectile(Guns.Yari_Launcher.DefaultModule.projectiles[0].gameObject, g.barrelOffset.position, Quaternion.Euler(0, 0, g.CurrentAngle + acc)).GetComponent<Projectile>();
                    if (p)
                    {
                        AkSoundEngine.PostEvent("Play_BOSS_RatMech_Missile_01", player.gameObject);
                        p.Owner = player;
                        p.Shooter = player.specRigidbody;
                        p.baseData.damage = 15;
                        player.DoPostProcessProjectile(p);
                    }
                }
                chance--;
            }
        }
    }
}

