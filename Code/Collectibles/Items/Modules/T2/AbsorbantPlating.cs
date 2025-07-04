﻿using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using JuneLib.Items;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class AbsorbantPlating : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(AbsorbantPlating))
        {
            Name = "Absorbant Plating",
            Description = "Spongy",
            LongDescription = "Fire and Poison takes 3 (+1 per stack) times longer to damage you.\nGain a 75% (+75% per stack) fire rate and accuracy boost when standing on any goop. Being poisoned or on fire makes projectiles inflict poison or fire." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("absorbantplate_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("absorbantplate_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Absorbant Plating" + h.ReturnTierLabel();
            h.LabelDescription = "Fire and Poison take 3 (" + StaticColorHexes.AddColorToLabelString("+1", StaticColorHexes.Light_Orange_Hex) + ") times longer do damage you.\nGain a 75% (" + StaticColorHexes.AddColorToLabelString("+75% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ") fire rate and accuracy boost\nwhen standing on any goop.\nBeing poisoned or on fire makes projectiles inflict poison or fire.";

            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.OverrideScrapCost = 6;
            h.EnergyConsumption = 1;

            ID = h.PickupObjectId;
            new Hook(typeof(PlayerController).GetMethod("IncreaseFire", BindingFlags.Instance | BindingFlags.Public), typeof(AbsorbantPlating).GetMethod("IncreaseFireHook"));
            new Hook(typeof(PlayerController).GetMethod("IncreasePoison", BindingFlags.Instance | BindingFlags.Public), typeof(AbsorbantPlating).GetMethod("IncreasePoisonHook"));

        }
        public static void IncreaseFireHook(Action<PlayerController, float> orig, PlayerController self, float amount)
        {
            if (GlobalModuleStorage.PlayerHasModule(self, ID) != null)
            {
                var container = GlobalModuleStorage.PlayerHasModule(self, ID);
                amount /= container.defaultModule.ReturnStack(container.defaultModule.Stored_Core) + 2;
            }
            orig(self, amount);
        }
        public static void IncreasePoisonHook(Action<PlayerController, float> orig, PlayerController self, float amount)
        {
            if (GlobalModuleStorage.PlayerHasModule(self, ID) != null)
            {
                var container = GlobalModuleStorage.PlayerHasModule(self, ID);
                amount /= container.defaultModule.ReturnStack(container.defaultModule.Stored_Core) + 2;
            }
            orig(self, amount);
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Gooper",
                FireRate_Process = ProcessFireRate,
                ChargeSpeed_Process = ProcessFireRate,
                Accuracy_Process = ProcessFireRate,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public float ProcessFireRate(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            if (player.CurrentGoop == null) { return f; }
            int stack = this.ReturnStack(modulePrinterCore);
            return f - (f - (f / (1 + 0.75f * stack)));
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);

            modulePrinter.OnPostProcessProjectile -= PPP;
        }


        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            if (p.statusEffectsToApply == null){p.statusEffectsToApply = new List<GameActorEffect>(); }
            if (player.IsOnFire == true)
            {
                p.statusEffectsToApply.Add(DebuffStatics.hotLeadEffect);
                p.AdjustPlayerProjectileTint(Color.red, 2);
            }
            if (player.CurrentPoisonMeterValue > 0)
            {
                p.statusEffectsToApply.Add(DebuffStatics.irradiatedLeadEffect);
                p.AdjustPlayerProjectileTint(Color.green, 2);
            }
        }
        
    }
}

