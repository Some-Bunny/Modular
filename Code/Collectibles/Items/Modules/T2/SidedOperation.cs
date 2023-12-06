using Alexandria.ItemAPI;
using JuneLib.Items;
using ModularMod.Code.Components.Projectile_Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class SidedOperation : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SidedOperation))
        {
            Name = "Sided Operation",
            Description = "Divided",
            LongDescription = "Incraeses Crit Chance by 20%. Non-Critical Hits pull enemies towards the point of impact, hurting them. (+Pull Strength and Damage per stack). Critical Hits explode on hit for 33% (+33% per stack) of the projectiles damage.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("sided_t2_module_alt"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("sided_t2_module");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Sided Operation " + h.ReturnTierLabel();
            h.LabelDescription = "Increases "+StaticColorHexes.AddColorToLabelString("Crit Chance", StaticColorHexes.Light_Purple_Hex) +" by 15%.\nNon-Critical Hits pull enemies towards\nthe position of impact, hurting them. ("+StaticColorHexes.AddColorToLabelString("+Pull Strength and Damage") +").\n"+StaticColorHexes.AddColorToLabelString("Critical Hits", StaticColorHexes.Light_Purple_Hex) +" explode on hit for 25% ("+StaticColorHexes.AddColorToLabelString("+25%") +") of the projectiles damage.";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.CRIT);
            h.AddModuleTag(BaseModuleTags.CONDITIONAL);

            h.AdditionalWeightMultiplier = 0.4f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.AddToGlobalStorage();
            ID = h.PickupObjectId;
            data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            data.force = 500;
            data.doForce = true;
            data.damage = 10; 
        }
        public static int ID;
        public static ExplosionData data;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.CritContext = new CriticalHitComponent.CritContext()
            {
                CritChance = 0.15f
            };
            modulePrinter.CritContexts.Add(this.CritContext);
            modulePrinter.OnCritProjectileFailRoll += Fail;
            modulePrinter.OnCritProjectileRolled += Crit;
        }

        public void Fail(Projectile projectile, PlayerController player)
        {
            projectile.OnDestruction += Projectile_OnDestruction;
           
        }

        
        private void Projectile_OnDestruction(Projectile obj)
        {
            var room = obj.transform.position.GetAbsoluteRoom();
            if (room == null) { return; }
            if (room.activeEnemies == null) { return; }
            int stack = this.ReturnStack(Stored_Core);
            foreach (var enemy in room.activeEnemies)
            {
                if (enemy.knockbackDoer)
                {
                    enemy.healthHaver.ApplyDamage(1f * stack, Vector2.zero, "Force", CoreDamageTypes.Electric, DamageCategory.DamageOverTime);
                    enemy.knockbackDoer.ApplyKnockback(obj.transform.position - enemy.transform.position, 10 * stack);
                    Toolbox.DoMagicSparkles(7, 0.5f, obj.transform.position,  enemy.transform.position - obj.transform.position, 0.075f, 0.5f);
                    Toolbox.DoMagicSparkles(7, 0.5f, enemy.transform.position, obj.transform.position - enemy.transform.position, 0.075f, 0.5f);
                }
            }
        }
       
        public void Crit(Projectile projectile, PlayerController player)
        {
            var explode = projectile.gameObject.AddComponent<ExplosiveModifier>();
            explode.explosionData = StaticExplosionDatas.CopyFields(data);
            explode.explosionData.damage = (projectile.baseData.damage / 4) * this.ReturnStack(Stored_Core);
            projectile.baseData.speed *= 1.3f;
            projectile.UpdateSpeed();
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.CritContexts.Remove(this.CritContext);
            modulePrinter.OnCritProjectileFailRoll -= Fail;
            modulePrinter.OnCritProjectileDestroyed -= Crit;
        }
    }
}

