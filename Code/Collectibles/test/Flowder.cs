using Alexandria.ItemAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public class Flowder : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Flowder";
            string resourceName = "ModularMod/Sprites/Items/Item/modularprintercore.png";
            GameObject obj = new GameObject(itemName);
            Flowder testActive = obj.AddComponent<Flowder>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Flows Loads";
            string longDesc = "Loads a specifie debug flow";
            testActive.SetupItem(shortDesc, longDesc, "mdl");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Timed, 1f);
            testActive.consumable = false;
            ItemBuilder.AddPassiveStatModifier(testActive, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            testActive.quality = PickupObject.ItemQuality.EXCLUDED;

        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }
        public override void DoEffect(PlayerController user)
        {
            if (this.LastOwner.PlayerHasCore() != null)
            {
                var core = this.LastOwner.PlayerHasCore().GiveTemporaryModule(GlobalModuleStorage.ReturnRandomModule(), "Randomweisser", 3);
                GameManager.Instance.StartCoroutine(Delay(core.defaultModule, "Randomweisser"));
            }
            /*
            GameObject spawnedBulletOBJ = SpawnManager.SpawnProjectile(SteelPanopticon.MegaFuckingLaser.gameObject, user.transform.position, Quaternion.Euler(0f, 0f, Vector2.down.ToAngle()), true);
            Projectile component = spawnedBulletOBJ.GetComponent<Projectile>();
            if (component != null)
            {
                component.Owner = user;
                component.Shooter = user.specRigidbody;
                //component.IgnoreTileCollisionsFor(120);
                component.baseData.speed = 150;
                component.Update();
            }
            */
            //StarterGunSelectUIController.GenerateUI().ToggleUI(null, user);

            //GlobalMessageRadio.BroadcastMessage("eye_shot_1");
            //GameManager.Instance.LoadCustomFlowForDebug("NPCParadise", "Base_Castle", "tt_castle");
        }
        public IEnumerator Delay(DefaultModule mod, string context)
        {

            yield return new WaitForSeconds(12);
            if (this.LastOwner.PlayerHasCore() != null)
            {
                this.LastOwner.PlayerHasCore().RemoveTemporaryModule(mod, context);
            }
            yield break;
        }
    }
}
