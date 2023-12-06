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
    public class SolarFlare : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SolarFlare))
        {
            Name = "Solar Flare",
            Description = "Universe On Fire",
            LongDescription = "+25% Crit Chance. Critical Hits inflict Sun Burn onto enemies. (+Sun Burn Damage per stack). Enemies with Sun Burn explode on death. (+Explosion Damage per stack)\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("brilliantsun_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("brilliantsun_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Solar Flare " + h.ReturnTierLabel();
            h.LabelDescription = "+15% "+StaticColorHexes.AddColorToLabelString("Crit Chance", StaticColorHexes.Light_Purple_Hex) + ".\n"+StaticColorHexes.AddColorToLabelString("Critical Hits", StaticColorHexes.Light_Purple_Hex) +" inflict Sun Burn onto enemies. (" + StaticColorHexes.AddColorToLabelString("+Sun Burn Damage")+ ").\nEnemies with Sun Burn explode on death.\n(" + StaticColorHexes.AddColorToLabelString("Explosion Damage")+")";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.CRIT);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);

            h.AdditionalWeightMultiplier = 0.5f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.IsUncraftable = true;
            h.EnergyConsumption = 2;
            h.AddToGlobalStorage();
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            SunFlame = (PickupObjectDatabase.GetById(748) as Gun).DefaultModule.chargeProjectiles[0].Projectile.fireEffect;
            Data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            Data.damage = 30;
            Data.effect = (PickupObjectDatabase.GetById(748) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy.effects[0].effects[0].effect;
            Data.damageRadius = 2.5f;

            ID = h.PickupObjectId;
        }

        public static GameActorFireEffect SunFlame;
        public static ExplosionData Data;
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.CritContext = new CriticalHitComponent.CritContext()
            {
                CritChance = 0.15f
            };
            modulePrinter.CritContexts.Add(this.CritContext);
            modulePrinter.OnPostProcessProjectile += PPP;
            player.OnAnyEnemyReceivedDamage += OKE;

        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            if (IsCrit == true)
            {
                SunFlame.DamagePerSecondToEnemies = 6 * stack;
                p.statusEffectsToApply = new List<GameActorEffect>()
                {
                    SunFlame
                };
            }
        }   
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.CritContexts.Remove(this.CritContext);
            modulePrinter.OnPostProcessProjectile -= PPP;
            player.OnAnyEnemyReceivedDamage -= OKE;
        }
        public void OKE(float damage, bool fatal, HealthHaver healthHaver)
        {
            if (healthHaver.aiActor)
            {
                if (healthHaver.aiActor.m_activeEffects.Contains(SunFlame) && fatal == true)
                {
                    healthHaver.aiActor.m_activeEffects.Remove(SunFlame);
                    var d = StaticExplosionDatas.CopyFields(Data);
                    d.damage = 25 * this.ReturnStack(Stored_Core);
                    d.force = 50 * this.ReturnStack(Stored_Core);
                    d.damageRadius = 2 + this.ReturnStack(Stored_Core);
                    Exploder.Explode(healthHaver.aiActor.sprite.WorldCenter, d, Vector2.zero);
                }
            }
        }
    }
}

