using Alexandria.ItemAPI;
using Dungeonator;
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
    public class CarpalNightmare : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CarpalNightmare))
        {
            Name = "Carpal Nightmare",
            Description = "Devourer",
            LongDescription = "Acts as 1 (+1 per stack) of every module you will own." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T4_Collection,
            ManualSpriteID = StaticCollections.Module_T4_Collection.GetSpriteIdByName("carpalnightmare"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };

        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            //h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("swarmer_t3_module_alt");
            h.Tier = ModuleTier.Tier_Omega;
            h.LabelName = "CARPAL NIGHTMARE " + h.ReturnTierLabel();
            h.LabelDescription = "PRESSING THE FIRE BUTTON TEMPORARILY BOOSTS FIRE RATE.\nPRESSING RELOAD WHILE RELOADING BOOSTS THE NEXT CLIP.\n("+StaticColorHexes.AddColorToLabelString("MORE STATS")+").";
            h.powerConsumptionData = new PowerConsumptionData()
            {
                FirstStack = 0,
                AdditionalStacks = 0,
                OverridePowerDescriptionLabel = "USES NO POWER.",
                OverridePowerManagement = null,
            };
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.red);
            h.AdditionalWeightMultiplier = 1f;
            h.Offset_LabelDescription = new Vector2(0.25f, -0.125f);
            h.Offset_LabelName = new Vector2(0.25f, 2f);
            h.Label_Background_Color_Override = new Color32(255, 10, 10, 100);
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            printer.OnPostProcessProjectile += PPP;
            printer.OnFrameUpdate += OFU;

            player.OnReloadPressed += ORP;
            printer.OnGunReloaded += OGR;

            player.OnTriedToInitiateAttack += Player_OnTriedToInitiateAttack;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnFrameUpdate -= OFU;

            player.OnReloadPressed -= ORP;
            modulePrinter.OnGunReloaded -= OGR;

            player.OnTriedToInitiateAttack -= Player_OnTriedToInitiateAttack;

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

        private void Player_OnTriedToInitiateAttack(PlayerController obj)
        {
            T += 0.1f * this.ReturnStack(Stored_Core);
        }
        private float T = 0;
        public void OFU(ModulePrinterCore modulePrinterCore, PlayerController player)
        {
            if (T > 0)
            {
                T -= BraveTime.DeltaTime * 1.2f;
            }
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / (1 + T);
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

