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
    public class HellfireLauncher : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(HellfireLauncher))
        {
            Name = "Hellfire Launcher",
            Description = "Love The Smell",
            LongDescription = "Grants Fire Immunity. Reloading creates a line of fire in the direction you are aiming. The trail will initially hurt enemies directly when created. (+Range, Damage and Radius per stack) Scales with amount of shots left in the clip." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("hellfire_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("hellfire_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Hellfire Launcher " + h.ReturnTierLabel();
            h.LabelDescription = "Grants Fire Immunity.\nReloading creates a line of fire in the direction you are aiming.\nThe trail will initially hurt enemies directly when created.\n(" + StaticColorHexes.AddColorToLabelString("+Range, Damage and Radius", StaticColorHexes.Light_Orange_Hex) + ").\nScales with amount of shots left in the clip.";
            h.AdditionalWeightMultiplier = 0.9f;

            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);

            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public DamageTypeModifier FireImmunity = new DamageTypeModifier()
        {
            damageMultiplier = 0,
            damageType = CoreDamageTypes.Fire
        };

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Add(FireImmunity);
            modulePrinter.OnGunReloaded += OGR;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.healthHaver.damageTypeModifiers.Remove(FireImmunity);
            modulePrinter.OnGunReloaded -= OGR;
        }
        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            float a = ((float)g.ClipShotsRemaining / (float)g.ClipCapacity);
            a = 1 - a; 
            int stack = this.ReturnStack(modulePrinterCore);


            Vector2 startLocation = player.sprite.WorldCenter +Toolbox.GetUnitOnCircle(g.CurrentAngle, (1f + (0.25f * stack)) + 1f);

            Vector2 endLocation = startLocation + Toolbox.GetUnitOnCircle(g.CurrentAngle, (5f + (stack * 3.5f)) * a);

            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Alexandria.Misc.GoopUtility.FireDef).TimedAddGoopLine(startLocation, endLocation, 1f + (0.33f * stack), 0.35f);
            player.StartCoroutine(BurningWitness(player.CurrentRoom, startLocation, endLocation, 1f + (0.25f * stack), 0.35f));
        }
        private IEnumerator BurningWitness(RoomHandler room, Vector2 p1, Vector2 p2, float radius, float duration)
        {
            float elapsed = 0f;
            Vector2 lastEnd = p1;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                Vector2 currentEnd = Vector2.Lerp(p1, p2, elapsed / duration);
                float curDist = Vector2.Distance(currentEnd, lastEnd);
                int steps = Mathf.CeilToInt(curDist / radius);
                for (int i = 0; i < steps; i++)
                {
                    Vector2 center = lastEnd + (currentEnd - lastEnd) * (((float)i + 1f) / (float)steps);
                    if (room != null)
                    {
                        room.ApplyActionToNearbyEnemies(center, radius + 0.75f, Burn);
                    }
                    GlobalSparksDoer.DoSingleParticle(center + Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(0, radius)), Vector2.zero, null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                }
                lastEnd = currentEnd;
                yield return null;
            }
            yield break;
        }
        public void Burn(AIActor enemy, float f)
        {
            if (enemy) 
            {
                enemy.healthHaver.ApplyDamage(1.5f * this.ReturnStack(Stored_Core), Vector2.zero ,"HeatVent");
                enemy.ApplyEffect(DebuffStatics.hotLeadEffect);
            }
        }
    }
}

