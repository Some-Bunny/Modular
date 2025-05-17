using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class MachineInstinct : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MachineInstinct))
        {
            Name = "Machine Instinct",
            Description = "Perfect Machine",
            LongDescription = "Being in proximity of projectiles and enemies increases Risk, which increases both Rate Of Fire and Damage. (+Higher Risk Capacity per stack). Building up enough Risk allows you to negate one instance of damage, providing you with 2.5 second of invulnerability. Ability recharges after 15 (-20% hyperbolically per stack) seconds." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("machineinstinct_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("machineinstinct_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Machine Instinct " + h.ReturnTierLabel();
            h.LabelDescription = "Being in proximity of projectiles and enemies increases "+ StaticColorHexes.AddColorToLabelString("Risk", StaticColorHexes.Lime_Green_Hex) + ",\nwhich increases both Rate Of Fire and Damage. (" + StaticColorHexes.AddColorToLabelString("+Higher Risk Capacity", StaticColorHexes.Light_Orange_Hex) + ").\nBuilding up enough " + StaticColorHexes.AddColorToLabelString("Risk", StaticColorHexes.Lime_Green_Hex) + " allows you to negate\none instance of damage, providing you with 2.5 seconds of invulnerability.\nAbility recharges after 15 (" + StaticColorHexes.AddColorToLabelString("-20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ") seconds.";
            h.EnergyConsumption = 2;

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.RETALIATION);
            h.OverrideScrapCost = 10;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.AdditionalWeightMultiplier = 0.8f;
            h.Offset_LabelDescription = new Vector2(0.125f, -0.375f);
            h.Offset_LabelName = new Vector2(0.125f, 1.9375f);

            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                FireRate_Process = ProcessFireRate,
                ChargeSpeed_Process = ProcessFireRate,
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            printer.OnPostProcessProjectile += PPP;
            printer.OnFrameUpdate += OFU;
            player.healthHaver.ModifyDamage += ModifyIncomingDamage;

            player.m_additionalReceivesTouchDamage = false;

        }
        private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if (DodgeCooldown == false && BuildupCount >= 2) 
            {
                args.InitialDamage = 0;
                args.ModifiedDamage = 0;
                source.IsVulnerable = false;
                DodgeCooldown = true;
                BuildupCount = 0;

                LightningController c = new LightningController();
                c.MajorNodesCount = UnityEngine.Random.Range(4, 6);
                c.MinorNodesMin = UnityEngine.Random.Range(1, 3);
                c.OnPostStrike += (obj) =>
                {
                    AkSoundEngine.PostEvent("Play_OBJ_lightning_flash_01", source.gameObject);
                    Exploder.Explode(obj, StaticExplosionDatas.genericLargeExplosion, obj);
                };
                c.LightningPreDelay = 0;
                c.GenerateLightning(source.sprite.WorldCenter + new Vector2(UnityEngine.Random.Range(-7, 7), 16), source.sprite.WorldCenter);
                GameManager.Instance.StartCoroutine(IFrame(source));
            }
        }

        public IEnumerator IFrame(HealthHaver h)
        {
            h.m_player.sprite.usesOverrideMaterial = true;
            h.m_player.SetOverrideShader(ShaderCache.Acquire("Brave/ItemSpecific/MetalSkinShader"));

            float f = 0;
            while (f < 2.5f)
            {
                h.IsVulnerable = false;
                f += BraveTime.DeltaTime;
                yield return null;
            }
            h.m_player.ClearOverrideShader();
            AkSoundEngine.PostEvent("Play_OBJ_metalskin_end_01", h.gameObject);

            h.IsVulnerable = true;
            f = 0;
            while (f < 15 - (15 - (15 / (1 + 0.20f * this.ReturnStack(Stored_Core)))))
            {
                f += BraveTime.DeltaTime;
                yield return null;
            }
            DodgeCooldown = false;
            yield break;
        }

        public void OFU(ModulePrinterCore core, PlayerController player)
        {
            if (DodgeCooldown == true) {
                if (currentRisk > 1)
                {
                    currentRisk -= (0.125f * BraveTime.DeltaTime);
                }
                return;  
            }
            ProcessNearbyProjectiles(player);
            ProcessNearbyEnemies(player);
            if (UnityEngine.Random.value < ((currentRisk - 1) * 0.5f))
            {
                DoFlameParticles(player.sprite);
            }

            if (CanDecrement(player) == false)
            {
                return; 
            }
            if (projectilesNearMe.Count == 0 && enemiesNearMe.Count == 0) 
            {
                if (currentRisk > 1)
                {
                    currentRisk -= (0.4f * BraveTime.DeltaTime);
                }
                return;
            }
            else if (currentRisk < TrueRiskCap)
            {
                currentRisk += (enemiesNearMe.Count * (IncrementPerSecondEnemy*BraveTime.DeltaTime)) + (projectilesNearMe.Count * (IncrementPerSecondProjectile * BraveTime.DeltaTime));
            }
            if (currentRisk > RiskCap)
            {
                if (BuildupCount < 2)
                {
                    BuildupCount += BraveTime.DeltaTime;
                }
            }
            else
            {
                if (BuildupCount >= 2)
                {
                    BuildupCount -= (BraveTime.DeltaTime * 0.75f);
                }
            }

        }

        public float IncrementPerSecondEnemy = 0.1f;
        public float IncrementPerSecondProjectile  = 0.05f;
        public float BuildupCount = 0f;
        public bool DodgeCooldown = false;


        public void DoFlameParticles(tk2dBaseSprite targetSprite, float zOffset = 0f)
        {
            if (GameManager.Instance.IsPaused == true) { return; }



            if (UnityEngine.Random.value < Mathf.Max(0.1f, Mathf.Min(1, ConfigManager.ImportantVFXMultiplier)))
            if (targetSprite)
            {
                Vector3 vector = targetSprite.WorldBottomLeft.ToVector3ZisY(zOffset);
                Vector3 vector2 = targetSprite.WorldTopRight.ToVector3ZisY(zOffset);
                float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                float num2 = 25f * num;
                int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
                int num4 = num3;
                Vector3 minPosition = vector;
                Vector3 maxPosition = vector2;
                Vector3 direction = Vector3.up / 2f;
                float angleVariance = 120f;
                float magnitudeVariance = 0.2f;
                float? startLifetime = new float?(UnityEngine.Random.Range(0.8f, 1.25f));
                GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, BuildupCount >= 2 ? GlobalSparksDoer.SparksType.STRAIGHT_UP_GREEN_FIRE : GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
            }
        }

        public bool CanDecrement(PlayerController player)
        {
            if (GameManager.Instance.Dungeon == null) { return false; }
            if (GameManager.Instance.IsLoadingLevel == true) { return false; }
            if (GameManager.Instance.PreventPausing == true) { return false; }
            if (GameManager.Instance.Dungeon.IsEndTimes == true) { return false; }
            if (player.IsDodgeRolling == true) { return false; }
            return true;
        }


        public void ProcessNearbyProjectiles(PlayerController player)
        {
            for (int i = 0; i < projectilesNearMe.Count; i++)
            {
                if (projectilesNearMe.Count > i)
                {
                    if (projectilesNearMe[i] == null) { projectilesNearMe.RemoveAt(i); }
                    else if (Vector2.Distance(projectilesNearMe[i].transform.PositionVector2(), player.sprite.WorldCenter) > 2.25f) { projectilesNearMe.Remove(projectilesNearMe[i]); }
                }
            }

            if (StaticReferenceManager.m_allProjectiles == null) { return; }
            for (int i = 0; i < StaticReferenceManager.m_allProjectiles.Count; i++)
            {
                Projectile projectile = StaticReferenceManager.m_allProjectiles[i];
                if (projectile)
                {
                    if (projectile.Owner != (projectile.Owner as PlayerController))
                    {
                        if (Vector2.Distance(projectile.transform.PositionVector2(), player.sprite.WorldCenter) < 2.25f)
                        {
                            if (!projectilesNearMe.Contains(projectile))
                            {
                                projectilesNearMe.Add(projectile);
                            }
                        }
                        else if (projectilesNearMe.Contains(projectile))
                        {
                            projectilesNearMe.Remove(projectile);
                        }
                    }
                }
            }
          
        }
        public void ProcessNearbyEnemies(PlayerController player)
        {

            for (int i = 0; i < enemiesNearMe.Count; i++)
            {
                if (enemiesNearMe.Count > i)
                {
                    if (enemiesNearMe[i] == null) { enemiesNearMe.RemoveAt(i); }
                    else if (Vector2.Distance(enemiesNearMe[i].transform.PositionVector2(), player.sprite.WorldCenter) > 6.25f) { enemiesNearMe.Remove(enemiesNearMe[i]); }
                }
            }

            if (StaticReferenceManager.AllEnemies == null) { return; }
            for (int i = 0; i < StaticReferenceManager.AllEnemies.Count; i++)
            {
                AIActor aIActor = StaticReferenceManager.AllEnemies[i];
                if (aIActor)
                {
                    if (aIActor.CanTargetPlayers == true)
                    {
                        if (Vector2.Distance(aIActor.transform.PositionVector2(), player.sprite.WorldCenter) < 6.25f)
                        {
                            if (!enemiesNearMe.Contains(aIActor))
                            {
                                enemiesNearMe.Add(aIActor);
                            }
                        }
                        else if (enemiesNearMe.Contains(aIActor))
                        {
                            enemiesNearMe.Remove(aIActor);
                        }
                    }
                }
            }
        }

        private List<Projectile> projectilesNearMe = new List<Projectile>();
        private List<AIActor> enemiesNearMe = new List<AIActor>();

        public float currentRisk = 1;
        private float RiskCap = 1.5f;
        private float TrueRiskCap = 2.5f;


        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            float q = Mathf.Min(currentRisk, RiskCap);
            if (q > RiskCap) { q = RiskCap; }
            return f / q;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.damage *= 1 + (Mathf.Min(currentRisk, RiskCap) / 3);
        }
     

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {
            TrueRiskCap = 2f + (0.5f * this.ReturnStack(modulePrinter));
            RiskCap = 1.375f + (0.125f * this.ReturnStack(modulePrinter));

        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            player.m_additionalReceivesTouchDamage = true;
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnFrameUpdate -= OFU;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            player.healthHaver.ModifyDamage -= ModifyIncomingDamage;

        }
    }
}

