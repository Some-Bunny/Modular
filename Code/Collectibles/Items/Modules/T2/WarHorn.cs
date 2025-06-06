using Alexandria;
using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.Image;


namespace ModularMod
{
    public class WarHorn : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(WarHorn))
        {
            Name = "War Horn",
            Description = "Trumpet",
            LongDescription = "Gain a dramatic entrance every combat room. (+Extended Drama per stack).\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("warhorn_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("warhorn_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "War Horn " + h.ReturnTierLabel();
            h.LabelDescription = "Gain a dramatic entrance every combat room.\n(" + StaticColorHexes.AddColorToLabelString("+Extended Drama", StaticColorHexes.Light_Orange_Hex) + ").";
            h.AppearsInRainbowMode = false;
            h.AppearsFromBlessedModeRoll = false;

            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AdditionalWeightMultiplier = 0.7f;
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.IsUncraftable = true;
            h.EnergyConsumption = 2;
            h.AddToGlobalStorage();
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            var enemy = EnemyDatabase.GetOrLoadByGuid(EnemyGUIDs.HM_Absolution_GUID);
            foreach (Component item in enemy.GetComponentsInChildren(typeof(Component)))
            {
                if (item is BossFinalRogueLaserGun laser)
                {
                    if (laser.beamProjectile != null)
                    {
                        SuperBeamProjectile = laser.beamProjectile;
                    }
                }
            }

            ID = h.PickupObjectId;
        }
        public static int ID;
        public static Projectile SuperBeamProjectile;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat += ORE;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat -= ORE;
        }

        public void ORE(ModulePrinterCore modulePrinter, RoomHandler room, PlayerController player)
        {
            player.StartCoroutine(FireBeam(SuperBeamProjectile, player, 2f + (2f * this.ReturnStack(Stored_Core)), room));
            player.StartCoroutine(FireBeam(SuperBeamProjectile, player, 2f + (2f * this.ReturnStack(Stored_Core)), room, true));
        }

        public IEnumerator FireBeam(Projectile projectile, PlayerController player, float Duration, RoomHandler roomHandler, bool Cosmetic = false)
        {
            if (Cosmetic == false)
            {
                AkSoundEngine.PostEvent("Play_ModularHornofWar", player.gameObject);
            }

            yield return new WaitForSeconds(1);

            GameObject beamObject = UnityEngine.Object.Instantiate<GameObject>(projectile.gameObject);
            var m_laserBeam = beamObject.GetComponent<BasicBeamController>();
            m_laserBeam.Owner = player;
            m_laserBeam.HitsPlayers = false;
            m_laserBeam.HitsEnemies = true;
            m_laserBeam.collisionLength = 100;
            m_laserBeam.collisionWidth = 6;
            m_laserBeam.collisionRadius = 6;
            m_laserBeam.collisionType = Cosmetic ? BasicBeamController.BeamCollisionType.Rectangle : BasicBeamController.BeamCollisionType.Default;
            m_laserBeam.projectile.ImmuneToBlanks = true;




            m_laserBeam.OverrideHitChecks = delegate (SpeculativeRigidbody hitRigidbody, Vector2 dirVec)
           {
                HealthHaver healthHaver = (!hitRigidbody) ? null : hitRigidbody.healthHaver;
                if (hitRigidbody && hitRigidbody.projectile && hitRigidbody.GetComponent<BeholsterBounceRocket>())
                {
                    BounceProjModifier component = hitRigidbody.GetComponent<BounceProjModifier>();
                    if (component)
                    {
                        component.numberOfBounces = 0;
                    }
                    hitRigidbody.projectile.DieInAir(false, true, true, false);
                }
                if (healthHaver != null)
                {
                  
                    if (healthHaver.aiActor)
                    {
                       if (healthHaver.aiActor.parentRoom != null && healthHaver.aiActor.parentRoom == roomHandler)
                       {

                           Projectile currentProjectile = projectile;
                           healthHaver.ApplyDamage(200 * Time.deltaTime, dirVec, "Death", currentProjectile.damageTypes, DamageCategory.Normal, false, null, false);
                       }
                    }
                    else
                    {
                        Projectile currentProjectile = projectile;
                        healthHaver.ApplyDamage(200 * Time.deltaTime, dirVec, "Death", currentProjectile.damageTypes, DamageCategory.Normal, false, null, false);
                    }
                }
                if (hitRigidbody.majorBreakable)
                {
                    hitRigidbody.majorBreakable.ApplyDamage(100f * BraveTime.DeltaTime, dirVec, false, false, false);
                }
           };

            m_laserBeam.ContinueBeamArtToWall = true;
            m_laserBeam.projectile.baseData.damage = 100;
            m_laserBeam.projectile.collidesWithEnemies = true;
            float t = 0;

            while (m_laserBeam != null )
            {
                float clampedAngle = BraveMathCollege.ClampAngle360(player.CurrentGun.CurrentAngle);
                Vector2 dirVec = new Vector3(Mathf.Cos(clampedAngle * 0.0174532924f), Mathf.Sin(clampedAngle * 0.0174532924f)) * 10f;
                m_laserBeam.Direction = dirVec;
                m_laserBeam.Origin = player.CurrentGun.transform.position;
                t += Time.deltaTime;
                m_laserBeam.LateUpdatePosition(player.CurrentGun.transform.position);
                if (t > Duration)
                {
                    break;
                }
                yield return null;
            }
            
            if (m_laserBeam != null)
            {
               m_laserBeam.CeaseAttack();
            }
            if (m_laserBeam)
            {
               m_laserBeam.SelfUpdate = false;
                while (m_laserBeam)
                {
                    m_laserBeam.LateUpdatePosition(player.CurrentGun.transform.position);
                    m_laserBeam.Origin = player.CurrentGun.transform.position;
                    float clampedAngle = BraveMathCollege.ClampAngle360(player.CurrentGun.CurrentAngle);
                    Vector2 dirVec = new Vector3(Mathf.Cos(clampedAngle * 0.0174532924f), Mathf.Sin(clampedAngle * 0.0174532924f)) * 10f;
                    m_laserBeam.Direction = dirVec;
                    yield return null;
                }
            }
           m_laserBeam = null;
            
            yield break;
        }
    }
}


