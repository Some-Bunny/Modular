using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class CleaningProtocol : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CleaningProtocol))
        {
            Name = "Cleaning Protocol",
            Description = "BRUSH",
            LongDescription = "Deal 60% (+30% per stack) more damage to enemies above 90% HP.\nAll enemies take an additional 20% (+20% per stack) extra damage from various effects.\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("cleaner_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("cleaner_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Cleaning Protocol " + h.ReturnTierLabel();
            h.LabelDescription = "Deal 60% (" + StaticColorHexes.AddColorToLabelString("+30%", StaticColorHexes.Light_Orange_Hex) + ") more damage to enemies above 90% HP.\nAll enemies take an additional 20% (" + StaticColorHexes.AddColorToLabelString("+20%", StaticColorHexes.Light_Orange_Hex) + ")\nextra damage from various effects.";

            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.05f) { return; }
            p.specRigidbody.OnPreRigidbodyCollision = (o, t, th, fr) =>
            {
                if (th != null && th.healthHaver != null)
                {
                    float maxHealth = th.healthHaver.GetMaxHealth();
                    float num = maxHealth * 0.9f;
                    float currentHealth = th.healthHaver.GetCurrentHealth();
                    if (currentHealth > num)
                    {
                        float damage = o.projectile.baseData.damage;
                        o.projectile.baseData.damage *= 1.6f;
                        GameManager.Instance.StartCoroutine(this.ChangeProjectileDamage(o.projectile, damage));
                        AkSoundEngine.PostEvent("Play_OBJ_cauldron_splash_01", o.gameObject);
                        th.aiActor.PlayEffectOnActor((PickupObjectDatabase.GetById(404) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, new Vector2(0, 0));
                    }
                }
            };
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat += EC;
            modulePrinter.OnPostProcessProjectile += PPP;
            ETGMod.AIActor.OnPreStart += OPEH;
        }

        public void EC(ModulePrinterCore core, Dungeonator.RoomHandler r, PlayerController p)
        {
            var l = r.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.RoomClear);
            if (l != null)
            {
                foreach (var enemy in l)
                {
                    if (enemy.CanTargetPlayers && !enemyC.Contains(enemy))
                    {
                        enemyC.Add(enemy);
                        enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Fire, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                        enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Electric, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                        enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Ice, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                        enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Magic, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                        enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Poison, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                        enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Void, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                        enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Water, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                    }
                }
            }
        }

        public void OPEH(AIActor enemy)
        {
            if (enemy.CanTargetPlayers && !enemyC.Contains(enemy))
            {
                enemyC.Add(enemy);
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Fire, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core))});
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Electric, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Ice, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Magic, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Poison, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Void, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
                enemy.healthHaver.damageTypeModifiers.Add(new DamageTypeModifier() { damageType = CoreDamageTypes.Water, damageMultiplier = 1 + (0.2f + this.ReturnStack(Stored_Core)) });
            }
        }

        private List<AIActor> enemyC = new List<AIActor>();

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat -= EC;
            modulePrinter.OnPostProcessProjectile -= PPP;
            ETGMod.AIActor.OnPreStart -= OPEH;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {                
            int stack = this.ReturnStack(modulePrinterCore);
            p.specRigidbody.OnPreRigidbodyCollision = (o,t,th,fr) =>
            {
                if (th != null && th.aiActor && th.healthHaver != null)
                {
                    float maxHealth = th.healthHaver.GetMaxHealth();
                    float num = maxHealth * 0.9f;
                    float currentHealth = th.healthHaver.GetCurrentHealth();
                    if (currentHealth > num)
                    {
                        float damage = o.projectile.baseData.damage;

                        o.projectile.baseData.damage *= 1f + (0.3f +  (0.3f *this.ReturnStack(modulePrinterCore)));
                        GameManager.Instance.StartCoroutine(this.ChangeProjectileDamage(o.projectile, damage));
                        AkSoundEngine.PostEvent("Play_OBJ_cauldron_splash_01", o.gameObject);
                        th.aiActor.PlayEffectOnActor((PickupObjectDatabase.GetById(404) as Gun).DefaultModule.projectiles[0].hitEffects.tileMapHorizontal.effects.First().effects.First().effect, new Vector2(0, 0));
                    }
                }
            };
        }

        private IEnumerator ChangeProjectileDamage(Projectile bullet, float oldDamage)
        {
            yield return new WaitForSeconds(0.1f);
            if (bullet != null)
            {
                bullet.baseData.damage = oldDamage;
            }
            yield break;
        }
    }
}

