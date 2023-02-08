using Alexandria.ItemAPI;
using System;
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
            StarterGunSelectUIController.GenerateUI().ToggleUI(null, user);

            //GlobalMessageRadio.BroadcastMessage("eye_shot_1");
            //GameManager.Instance.LoadCustomFlowForDebug("NPCParadise", "Base_Castle", "tt_castle");
        }
    }
}
