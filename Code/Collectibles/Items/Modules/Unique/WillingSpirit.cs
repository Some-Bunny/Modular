using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;


namespace ModularMod
{
    public class WillingSpirit : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(WillingSpirit))
        {
            Name = "Willing Spirit",
            Description = "SpoooOOoOky",
            LongDescription = "Entering combat spawns 1 (+1) ghosts that fight for you." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_Unique_Collection,
            ManualSpriteID = StaticCollections.Module_Unique_Collection.GetSpriteIdByName("willingspirit_u_module"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.Tier = ModuleTier.Unique;
            h.LabelName = "Willing Spirit " + h.ReturnTierLabel();
            h.LabelDescription = "Entering combat spawns 1 ("+StaticColorHexes.AddColorToLabelString("+1")+") ghosts\nthat fight for you.";
            h.IsSpecialModule = true;   
            
            h.SetTag("modular_module");
            h.AddColorLight(new Color(1, 0.3f, 0));
            h.Offset_LabelDescription = new Vector2(0.25f, -0.125f);
            h.Offset_LabelName = new Vector2(0.25f, 2.25f);
            h.Label_Background_Color_Override = new Color32(255, 80, 0, 100);
            h.EnergyConsumption = 1;
            ID = h.PickupObjectId;
            GhostProjectile = (PickupObjectDatabase.GetById(198) as Gun).DefaultModule.projectiles[0];
        }
        public static int ID;
        public static Projectile GhostProjectile;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat += OnRoomEntered;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat -= OnRoomEntered;
        }
        public void OnRoomEntered(ModulePrinterCore core, RoomHandler room, PlayerController playerController)
        {
            playerController.StartCoroutine(Spawn(playerController));
        }
        public IEnumerator Spawn(PlayerController playerController)
        {
            float e = 0;
            while (e < 1.25f)
            {
                e += BraveTime.DeltaTime;
                yield return null;
            }
            for (int i = 0; i < this.ReturnStack(Stored_Core); i++)
            {
                GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(GhostProjectile.gameObject, playerController.sprite.WorldCenter, Quaternion.Euler(0f, 0f, playerController.CurrentGun.CurrentAngle), true);
                Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
                if (component != null)
                {
                    component.Owner = playerController;
                    component.Shooter = playerController.specRigidbody;
                    component.baseData.range *= 20;
                    BounceProjModifier bounceProjModifier = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                    bounceProjModifier.numberOfBounces += 10;
                    playerController.DoPostProcessProjectile(component);
                }
            }
            yield break;
        }
    }
}

