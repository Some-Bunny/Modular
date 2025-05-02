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
            LongDescription = "Hitting enemies deals 25% (+25% per stack) of the damage dealt to nearby enemies. Slain enemies detonate. (+Explosion Power per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
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
            h.AdditionalWeightMultiplier = 0.9f;
            h.LabelDescription = "Hitting enemies deals 25% (" + StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex) + ") of the damage dealt\nto enemies near the hurt enemy.\nSlain enemies detonate." + "("+StaticColorHexes.AddColorToLabelString("+Explosion Power", StaticColorHexes.Light_Orange_Hex) + ")";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            hitEffect = (PickupObjectDatabase.GetById(504) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects[0].effects[0].effect;
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;

            tremorHit = StaticExplosionDatas.CopyFields((PickupObjectDatabase.GetById(601) as Gun).DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>().explosionData);
            tremorHit.damageToPlayer = 0;
            tremorHit.damage = 15;
            tremorHit.force = 50;
        }


        public static ExplosionData tremorHit;


        public static GameObject hitEffect;
        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnDamagedEnemy += OnEnemyDamaged;
            modulePrinter.OnKilledEnemy += OKE;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnDamagedEnemy -= OnEnemyDamaged;
            modulePrinter.OnKilledEnemy -= OKE;
        }

        public void OKE(ModulePrinterCore modulePrinter, PlayerController playerController, AIActor aIActor)
        {
            tremorHit.damage = 15 * this.ReturnStack(modulePrinter);
            tremorHit.force = 40 * this.ReturnStack(modulePrinter);
            Exploder.Explode(aIActor.sprite.WorldCenter, tremorHit, aIActor.sprite.WorldCenter, null, true);
        }

        public void OnEnemyDamaged(ModulePrinterCore modulePrinterCore, PlayerController player, AIActor enemy, float damage)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            float trueDmg = Mathf.Max(damage * (0.25f * stack), 0.5f);
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

                        if (ai)
                        {
                            if (ai != noTarget)
                            {
                                bool flag = radius < 0f;
                                Vector2 vector = room.activeEnemies[i].CenterPosition - position;
                                if (!flag)
                                {
                                    flag = (vector.sqrMagnitude < num);
                                }
                                if (flag)
                                {
                                    ai.PlayEffectOnActor(hitEffect, new Vector3(0, 0));
                                    ai.healthHaver.ApplyDamage(noTarget == ai ? damage / 3 : damage, position, "AoE");
                                    if (ai.knockbackDoer != null && !ai.healthHaver.IsBoss)
                                    {
                                        ai.knockbackDoer.ApplyKnockback(position - ai.transform.PositionVector2(), 5);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

