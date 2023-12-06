using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class CarpalTunnel : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CarpalTunnel))
        {
            Name = "Carpal Tunnel",
            Description = "Ow.",
            LongDescription = "Pressing Reload while reloading increases the next clips damage by 5% (+5% per stack) per press." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_Unique_Collection,
            ManualSpriteID = StaticCollections.Module_Unique_Collection.GetSpriteIdByName("turbo_u_module"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.Tier = ModuleTier.Unique;
            h.LabelName = "Carpal Tunnel " + h.ReturnTierLabel();
            h.LabelDescription = "Pressing Reload while reloading increases the next clips\ndamage by 5% ("+StaticColorHexes.AddColorToLabelString("+5%")+") per press.";
            h.IsSpecialModule = true;   
            
            h.SetTag("modular_module");
            h.AddColorLight(new Color(1, 0.3f, 0));
            h.Offset_LabelDescription = new Vector2(0.25f, -0.125f);
            h.Offset_LabelName = new Vector2(0.25f, 2.25f);
            h.Label_Background_Color_Override = new Color32(255, 80, 0, 100);
            h.EnergyConsumption = 1;
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.OnReloadPressed += ORP;
            modulePrinter.OnGunReloaded += OGR;
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        private int m_counter = 0;
        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            this.m_counter = 0;
        }

        public void ORP(PlayerController player, Gun gun)
        {
            if (gun.IsReloading)
            {
                this.m_counter++;
                AkSoundEngine.PostEvent("Play_WPN_RechargeGun_Recharge_01", gun.gameObject);
            }
            else
            {
                this.m_counter = 0;
            }
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.OnReloadPressed -= ORP;
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + (((float)m_counter) / 20 * stack);
            p.baseData.speed *= 1 + (((float)m_counter) / 30 * stack);
            p.AdditionalScaleMultiplier *= 1 + (((float)m_counter) / 30 * stack);

            p.UpdateSpeed();
        }
    }
}

