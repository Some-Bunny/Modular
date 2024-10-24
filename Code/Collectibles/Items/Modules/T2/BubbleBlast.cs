using Alexandria.ItemAPI;
using Alexandria.Misc;
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
    public class BubbleBlast : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(BubbleBlast))
        {
            Name = "Bubble Blast",
            Description = "Bubbles!",
            LongDescription = "Slightly decreases Accuracy\nand increases fire rate by 20% (+20% per stack hyperbolically). Projectiles will slow to a crawl after a moment. Reloading speeds up all projectiles and increases their damage by 33% (+33% per stack)." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("bubbleblast_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("bubbleblast_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Bubble Blast " + h.ReturnTierLabel();
            h.LabelDescription = "Slightly decreases Accuracy, and increases fire rate by 20% (" + StaticColorHexes.AddColorToLabelString("+20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ").\nProjectiles will slow to a crawl after a moment.\nReloading speeds up all projectiles\nand increases their damage by 20% (" + StaticColorHexes.AddColorToLabelString("+20%", StaticColorHexes.Light_Orange_Hex) + ").";

            h.AddModuleTag(BaseModuleTags.BASIC);
            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "CouterProduction",
                Accuracy_Process = ProcessAccuracy,
                FireRate_Process = ProcessFireRate,
                ChargeSpeed_Process = ProcessFireRate,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnGunReloaded += OGR;
        }

        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            for (int i = activeBubbles.Count-1; i >= 0; i-- )
            {
                var thing = activeBubbles[i];
                if (thing != null)
                {
                    thing.DamageBoost = 1 + (0.2f * this.ReturnStack(modulePrinterCore));
                    thing.DoBoost();
                }
                activeBubbles.RemoveAt(i);
            }
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            p.baseData.range += 5;
            p.StartCoroutine(SpeedChange(p));   
        }


        public IEnumerator SpeedChange(Projectile p)
        {
            float s = p.baseData.speed;
            float e = 0;
            while (e < 0.66f)
            {
                if (p == null) { yield break; }
                float t = e / 0.66f;
                p.baseData.speed = Mathf.Lerp(s, s * 0.0666f, t);
                p.UpdateSpeed();
                e += (BraveTime.DeltaTime*1.5f);
                yield return null;
            }
            var bubble = p.gameObject.AddComponent<BoostProjectileComponent>();
            bubble.self = p;
            activeBubbles.Add(bubble);
            yield break;
        }


        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);

            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnGunReloaded -= OGR;
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
        }

        public float ProcessAccuracy(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 1.20f;
        }
        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.20f * stack)));
        }
        public List<BoostProjectileComponent> activeBubbles = new List<BoostProjectileComponent>();

        public class BoostProjectileComponent : MonoBehaviour
        {
            public void DoBoost()
            {
                if (self == null) { return; }
                self.baseData.speed *= 20;
                self.UpdateSpeed();
                self.baseData.damage *= DamageBoost;
            }
            public Projectile self;
            public float DamageBoost = 1;
        }
    }
}

