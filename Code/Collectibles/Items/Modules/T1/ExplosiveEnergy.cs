using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class ExplosiveEnergy : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ExplosiveEnergy))
        {
            Name = "Explosive Energy",
            Description = "Hit Harder, Differently",
            LongDescription = "Increases knockback force by 2x (+1 per stack), boss damage by 25% (+25% per stack), and grants a small chance to stun enemies." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_1),
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("glitteringsparks_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("glitteringsparks_tier1_module_alt");
            h.Tier = ModuleTier.Tier_1;
            h.LabelName = "Explosive Energy " + h.ReturnTierLabel();
            h.LabelDescription = "Projectiles explode up to 3 ("+StaticColorHexes.AddColorToLabelString("+1")+") times after\ntravelling a certain distance. Projectiles self-destruct after finishing.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.25f, -1f);
            h.Offset_LabelName = new Vector2(0.25f, 1.75f);

            h.AddModuleTag(BaseModuleTags.BASIC);

            ModulePrinterCore.ModifyForChanceBullets += h.ChanceBulletsModify;
            specialEffect = (PickupObjectDatabase.GetById(593) as Gun).DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>().explosionData.effect;
            ID = h.PickupObjectId;

            ExplosionData = new ExplosionData()
            {
                breakSecretWalls = false,
                comprehensiveDelay = 0,
                damage = 4,
                damageRadius = 3f,
                damageToPlayer = 0,
                debrisForce = 50,
                doDamage = true,
                doDestroyProjectiles = false,
                doExplosionRing = false,
                doForce = false,
                doScreenShake = false,
                doStickyFriction = false,
                effect = (PickupObjectDatabase.GetById(593) as Gun).DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>().explosionData.effect,
                explosionDelay = 0,
                force = 20,
                forcePreventSecretWallDamage = false,
                forceUseThisRadius = true,
                freezeEffect = null,
                freezeRadius = 0,
                IsChandelierExplosion = false,
                isFreezeExplosion = false,
                playDefaultSFX = false,
                preventPlayerForce = false,
                pushRadius = 5,
                secretWallsRadius = 1,

                ignoreList = new List<SpeculativeRigidbody>(),
                overrideRangeIndicatorEffect = null,

            };
        }
        public static ExplosionData ExplosionData;
        public static int ID;
        public static GameObject specialEffect;
        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.02f) { return; }
            int stack = 1;
            TravelledDistanceComponent travelledDistanceComponent = p.gameObject.AddComponent<TravelledDistanceComponent>();
            travelledDistanceComponent.DistanceToTravel = 3 + stack;
            travelledDistanceComponent.TriggerAmount = 3;
            travelledDistanceComponent.OnTravelledDistance += (proj, h1, h4) =>
            {
                Exploder.Explode(h1, ExplosionData, Vector2.zero, null, true);
                if (h4 == 3 + stack)
                {
                    p.DieInAir();
                }
            };
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            TravelledDistanceComponent travelledDistanceComponent = p.gameObject.AddComponent<TravelledDistanceComponent>();
            travelledDistanceComponent.DistanceToTravel = (stack*3) + 3;
            travelledDistanceComponent.TriggerAmount = 3;
            travelledDistanceComponent.OnTravelledDistance += (proj, h1, h4) =>
            {
                Exploder.Explode(h1, ExplosionData, Vector2.zero, null, true);
                if (h4 == 2 + stack)
                {
                    p.DieInAir();
                    
                }
            };
        }
    }
}

