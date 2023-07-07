using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class AggressiveReload : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(AggressiveReload))
        {
            Name = "Aggressive Reload",
            Description = "Air Force",
            LongDescription = "Reloading harms, pushes and stuns enemies near you. Effectiveness scales on how empty the clip is. (Damage and force is increased per stack). Can deflect enemy bullets, with good timing." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("aggressivereload_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("aggressivereload_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Aggressive Reload " + h.ReturnTierLabel();
            h.LabelDescription = "Reloading harms, pushes and stuns enemies near you.\nEffectiveness scales on how empty the clip is.\n(" + StaticColorHexes.AddColorToLabelString("+Damage and Force", StaticColorHexes.Light_Orange_Hex) + ").\nCan deflect enemy bullets, with good timing.";
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
            modulePrinter.OnGunReloaded += OGR;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded -= OGR;
        }
        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            int stack = this.ReturnStack(modulePrinter);
            Mult = 1 + (0.5f * stack);
        }

        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            GameObject silencerVFX = (GameObject)ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            GameObject blankObj = GameObject.Instantiate(silencerVFX.gameObject, player.sprite.WorldCenter, Quaternion.identity);
            blankObj.transform.localScale = Vector3.one;

            Destroy(blankObj, 2f);
            AkSoundEngine.PostEvent("Play_ENM_cannonball_blast_01", player.gameObject);

            float a =  1 - g.PercentageOfClipLeft();
            int stack = this.ReturnStack(modulePrinterCore);
            a *= 1 + (stack * 0.25f);
            Exploder.DoRadialPush(player.sprite.WorldCenter, 80 * a, 5);
            Exploder.DoRadialKnockback(player.sprite.WorldCenter, 80 * a, 5);
            Exploder.DoRadialMinorBreakableBreak(player.sprite.WorldCenter, 5);
            ApplyActionToNearbyEnemies(player.sprite.WorldCenter, 5, player.CurrentRoom, a);

            foreach (Projectile proj in StaticReferenceManager.AllProjectiles)
            {
                if (proj != null)
                {
                    AIActor enemy = proj.Owner as AIActor;
                    if (proj.GetComponent<BasicBeamController>() == null)
                    {
                        if (Vector2.Distance(proj.sprite ? proj.sprite.WorldCenter : proj.transform.PositionVector2(), player.sprite.WorldCenter) < 1.375f * a && proj.Owner != null && proj.Owner == enemy)
                        {
                            FistReflectBullet(proj, player.gameActor, proj.baseData.speed *= 2f, (proj.sprite.WorldCenter - player.transform.PositionVector2()).ToAngle(), 1f, proj.IsBlackBullet ? (5 + proj.baseData.speed)*2 : 5 + proj.baseData.speed, 0f);
                        }
                    }
                }
            }
        }

        public static void FistReflectBullet(Projectile p, GameActor newOwner, float minReflectedBulletSpeed, float ReflectAngle, float scaleModifier = 1f, float damageModifier = 10f, float spread = 0f)
        {
            p.RemoveBulletScriptControl();
            Vector2 Point1 = Toolbox.GetUnitOnCircle(ReflectAngle, 1);
            p.Direction = Point1;

            if (spread != 0f)
            {
                p.Direction = p.Direction.Rotate(UnityEngine.Random.Range(-spread, spread));
            }
            if (p.Owner && p.Owner.specRigidbody)
            {
                p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
            }
            p.Owner = newOwner;
            p.SetNewShooter(newOwner.specRigidbody);
            p.allowSelfShooting = false;
            if (newOwner is AIActor)
            {
                p.collidesWithPlayer = true;
                p.collidesWithEnemies = false;
            }
            else
            {
                p.collidesWithPlayer = false;
                p.collidesWithEnemies = true;
            }
            if (scaleModifier != 1f)
            {
                SpawnManager.PoolManager.Remove(p.transform);
                p.RuntimeUpdateScale(scaleModifier);
            }
            if (p.Speed < minReflectedBulletSpeed)
            {
                p.Speed = minReflectedBulletSpeed;
            }
            if (p.baseData.damage < ProjectileData.FixedFallbackDamageToEnemies)
            {
                p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
            }
            p.baseData.damage = damageModifier;
            p.UpdateCollisionMask();
            p.ResetDistance();
            p.Reflected();
        }


        public float Mult;

        public void ApplyActionToNearbyEnemies(Vector2 position, float radius, RoomHandler room, float m)
        {
            List<AIActor> a = new List<AIActor>();
            float num = radius * radius;
            if (room.activeEnemies != null)
            {
                for (int i = 0; i < room.activeEnemies.Count; i++)
                {
                    if (room.activeEnemies[i])
                    {
                        AIActor aI = room.activeEnemies[i];
                        bool flag = radius < 0f;
                        Vector2 vector = room.activeEnemies[i].CenterPosition - position;
                        if (!flag)
                        {
                            flag = (vector.sqrMagnitude < num);
                        }
                        if (flag)
                        {
                            if (aI.behaviorSpeculator)
                            {
                                if (aI.behaviorSpeculator.ImmuneToStun == false) { aI.behaviorSpeculator.Stun((1.5f + (Mult / 2)) * m); }
                            }
                            aI.healthHaver.ApplyDamage((25f * Mult)*m, aI.transform.PositionVector2(), "Vent", CoreDamageTypes.Fire);
                        }
                    }
                }
            }
        }
    }
}

