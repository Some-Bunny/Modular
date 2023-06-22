using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class TremorImpact : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TremorImpact))
        {
            Name = "Tremor Impact",
            Description = "Shockwave",
            LongDescription = "Hitting enemies deals 33% (+33% per stack) of the damage dealt to nearby enemies" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("tremor_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("tremor_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Tremor Impact " + h.ReturnTierLabel();
            h.LabelDescription = "Hitting enemies deals 33% (" + StaticColorHexes.AddColorToLabelString("+33%", StaticColorHexes.Light_Orange_Hex) + ") of the damage dealt\nto enemies near the hurt enemy.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }

        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnDamagedEnemy += OnEnemyDamaged;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnDamagedEnemy -= OnEnemyDamaged;
        }
        public void OnEnemyDamaged(ModulePrinterCore modulePrinterCore, PlayerController player, AIActor enemy, float damage)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            float trueDmg = Mathf.Max(damage * (0.33f * stack), 1);
            this.ApplyActionToNearbyEnemies(enemy.sprite.WorldCenter, 3.5f + (0.5f * stack), enemy.GetAbsoluteParentRoom(), trueDmg, enemy);
        }
        public void ApplyActionToNearbyEnemies(Vector2 position, float radius, RoomHandler room, float damage, AIActor noTarget)
        {
            float num = radius * radius;
            if (room.activeEnemies != null)
            {
                for (int i = 0; i < room.activeEnemies.Count; i++)
                {
                    if (room.activeEnemies[i])
                    {
                        AIActor ai = room.activeEnemies[i];

                        bool flag = radius < 0f;
                        Vector2 vector = room.activeEnemies[i].CenterPosition - position;
                        if (!flag)
                        {
                            flag = (vector.sqrMagnitude < num);
                        }
                        if (flag)
                        {
                            ai.PlayEffectOnActor((PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects[0].effects[0].effect, new Vector3(0 ,0));
                            ai.healthHaver.ApplyDamage(noTarget == ai ? damage / 3 : damage, position, "AoE");
                        }
                    }
                }
            }
        }
    }
}

