using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class BrilliantSun : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BrilliantSun))
        {
            Name = "Brilliant Sun",
            Description = "Duck Up",
            LongDescription = "Projectiles now inflict Sun Burn. Killing an enemy with Sun Burn causes it to explode. (+Explosion Power per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Unique),
            ManualSpriteCollection = StaticCollections.Module_Unique_Collection,
            ManualSpriteID = StaticCollections.Module_Unique_Collection.GetSpriteIdByName("brilliantsun_u_module"),
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.Tier = ModuleTier.Unique;
            h.LabelName = "Brilliant Sun " + h.ReturnTierLabel();
            h.LabelDescription = "Projectiles now inflict Sun Burn.\nKilling an enemy with Sun Burn causes it to explode.\n("+StaticColorHexes.AddColorToLabelString("+Explosion Power")+ ").";
            h.IsSpecialModule = true;      
            h.SetTag("modular_module");
            h.AddColorLight(new Color(1, 0.3f, 0));
            h.Offset_LabelDescription = new Vector2(0.25f, -0.125f);
            h.Offset_LabelName = new Vector2(0.25f, 2.25f);
            h.Label_Background_Color_Override = new Color32(255, 80, 0, 100);
            h.EnergyConsumption = 1;
            ID = h.PickupObjectId;
            SunFlame = (PickupObjectDatabase.GetById(748) as Gun).DefaultModule.chargeProjectiles[0].Projectile.fireEffect;
            Data = StaticExplosionDatas.CopyFields(StaticExplosionDatas.explosiveRoundsExplosion);
            Data.damage = 30;
            Data.effect = (PickupObjectDatabase.GetById(748) as Gun).DefaultModule.chargeProjectiles[0].Projectile.hitEffects.enemy.effects[0].effects[0].effect;
            Data.damageRadius = 2.5f;
        }
        public static int ID;
        public static GameActorEffect SunFlame;
        public static ExplosionData Data;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnKilledEnemy += OKE;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnKilledEnemy -= OKE;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.statusEffectsToApply = new List<GameActorEffect>()
            {
                SunFlame
            };   
        }

        public void OKE(ModulePrinterCore core, PlayerController player, AIActor enemy)
        {
            if (enemy.m_activeEffects.Contains(SunFlame))
            {
                var d = StaticExplosionDatas.CopyFields(Data);
                d.damage = 25 * this.ReturnStack(core);
                d.force = 50 * this.ReturnStack(core);
                d.damageRadius = 2 + this.ReturnStack(core);
                Exploder.Explode(enemy.sprite.WorldCenter, d, Vector2.zero);
            }
        }
    }
}

