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
    public class MercyRounds : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MercyRounds))
        {
            Name = "Mercy Rounds",
            Description = "Hits The Sick Harder",
            LongDescription = "Deal an additional 40% (+40% per stack) more damage to enemies for each buff or debuff they have. Enemies directly slain by you spread their debuffs to nearby enemies." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("mercybullets_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("mercybullets_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Mercy Rounds " + h.ReturnTierLabel();
            h.LabelDescription = "Deal an additional 40% (" + StaticColorHexes.AddColorToLabelString("+40%", StaticColorHexes.Light_Orange_Hex) + ")\nmore damage to enemies for each\nbuff or debuff they have.\nEnemies directly slain by you spread\ntheir debuffs to nearby enemies.";

            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AddModuleTag(BaseModuleTags.DAMAGE_OVER_TIME);

            h.AddToGlobalStorage();
            h.AdditionalWeightMultiplier = 0.8f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            ID = h.PickupObjectId;
        }

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.1f) { return; }
            p.specRigidbody.OnPreRigidbodyCollision += OPC;
            p.OnHitEnemy += OHE;
            p.OnWillKillEnemy += WillKill;
        }

        public static int ID;

        public int stack = 0;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            stack = this.ReturnStack(modulePrinter);
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.specRigidbody.OnPreRigidbodyCollision += OPC;
            p.OnHitEnemy += OHE;
            p.OnWillKillEnemy += WillKill;
        }

        public void WillKill(Projectile projectile, SpeculativeRigidbody body)
        {
            if (body.aiActor)
            {
                var enemy = body.aiActor;
                foreach (var effect in enemy.m_activeEffects)
                {
                    var enem = enemy.GetAbsoluteParentRoom().activeEnemies;
                    if (!DebuffStatics.BlacklistedEffects.Contains(effect.effectIdentifier))
                    {
                        foreach (var enemies in enem)
                        {
                            if (enemies != null && Vector2.Distance(enemies.transform.PositionVector2(), enemy.transform.PositionVector2()) < 3.5f)
                            {

                                enemies.ApplyEffect(effect);
                                if (ConfigManager.DoVisualEffect == true)
                                {
                                    var vfx = UnityEngine.Object.Instantiate(PatientZero.PoisonPoof, enemies.sprite.WorldCenter, Quaternion.identity);
                                    vfx.transform.localScale *= 0.6f;
                                    Destroy(vfx, 3);
                                }                            
                            }
                        }
                    }
                }
            }
        }


        public void OHE(Projectile projectile, SpeculativeRigidbody body, bool fatal)
        {
            if (body.aiActor != null)
            {
                if (body.aiActor.m_activeEffects.Count() > 0) 
                {
                    if (ConfigManager.DoVisualEffect == true)
                    {
                        var VFX = UnityEngine.Object.Instantiate(VFXStorage.MachoBraceBurstVFX, body.aiActor.sprite.WorldCenter - new Vector2(1.5f, 0), Quaternion.identity);
                        VFX.transform.localScale *= 0.6f;

                        Destroy(VFX, 2);
                    }
                }
            }
        }

        public void OPC(SpeculativeRigidbody mR, PixelCollider mP, SpeculativeRigidbody oR, PixelCollider oP)
        {
            if (oR.aiActor != null)
            {
                if (oR != null && oR.healthHaver != null && mR.projectile != null)
                {
                    float damage = mR.projectile.baseData.damage;
                    mR.projectile.baseData.damage *= 1 + ((0.4f * stack) * oR.aiActor.m_activeEffects.Count());
                    mR.projectile.StartCoroutine(FrameDelay(mR.projectile, damage));
                }
            }
        }

        public IEnumerator FrameDelay(Projectile p, float DmG)
        {
            yield return null;
            p.baseData.damage = DmG;
            yield break;
        }
    }
}

