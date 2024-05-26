using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class BloatedCapacitor : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BloatedCapacitor))
        {
            Name = "Overcharged Capacitor",
            Description = "Too Much To Handle",
            LongDescription = "Clearing enough rooms breaks this module and grants a Power Cell. Reduces stats significantly, take double damage, and cannot be deactivated." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("bloatedcapacitor_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("bloatedcapacitor_tier1_module_alt");
            h.AdditionalWeightMultiplier = 0.2f;
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Overcharged Capacitor " + h.ReturnTierLabel();
            h.LabelDescription = "Clearing enough rooms breaks this module and grants a Power Cell.\n"+StaticColorHexes.AddColorToLabelString("Reduces stats significantly, take double damage,\nand cannot be deactivated", StaticColorHexes.Red_Color_Hex) +".";
            h.IsUncraftable = true;
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.red);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (RandomRoomsToGo == 0) { RandomRoomsToGo = UnityEngine.Random.Range(10, 20); }
        }

        public int RandomRoomsToGo = 0;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnRoomCleared += OnRoomCleared;
            modulePrinter.OnPostProcessProjectile += PPP;
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Glass Gen Church",
                FireRate_Process = PFR,
                ChargeSpeed_Process = PFR,
                Reload_Process = PFR,
            };
            modulePrinter.VoluntaryMovement_Modifier += ModifySpeed;
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.OnDamaged += OnDamaged;
        }

        public void OnDamaged(ModulePrinterCore modulePrinterCore, PlayerController playerController)
        {
            playerController.healthHaver.Armor--;
        }

        public Vector2 ModifySpeed(Vector2 currentVelocity, ModulePrinterCore core, PlayerController player)
        {
            return currentVelocity *= 0.90f;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnRoomCleared -= OnRoomCleared;
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.VoluntaryMovement_Modifier -= ModifySpeed;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.OnDamaged -= OnDamaged;

        }

        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            return f * 1.25f;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.speed *= 0.8f;
            p.baseData.damage *= 0.8f;

        }


        public override bool CanBeDisabled(ModulePrinterCore modulePrinter, ModularGunController modularGunController)
        {
            return false;
        }

        public void OnRoomCleared(ModulePrinterCore modulePrinter, PlayerController player, RoomHandler room)
        {
            RandomRoomsToGo--;
            if (RandomRoomsToGo == 0)
            {
                modulePrinter.RemoveModule(this, 1);
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(PowerCell.PowerCellID).gameObject, Vector3.zero, Quaternion.identity);
                PickupObject component3 = gameObject.GetComponent<PickupObject>();
                if (component3 != null)
                {
                    component3.CanBeDropped = false;
                    component3.Pickup(player);
                }
                RandomRoomsToGo = UnityEngine.Random.Range(10, 20);
                AkSoundEngine.PostEvent("Play_BOSS_FuseBomb_Death_01", player.gameObject);
                if (ConfigManager.DoVisualEffect == true)
                {
                    UnityEngine.Object.Instantiate(VFXStorage.TeleportDistortVFX, player.sprite.WorldCenter, Quaternion.identity);
                }
            }
        }
    } 
}

