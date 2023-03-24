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
    public class ImpactPower : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ImpactPower))
        {
            Name = "Impact Power",
            Description = "Know Where To Go",
            LongDescription = "Adds 1 Bounce, Reduce Damage by 20% (-20% per stack hyperbolically) But each bounce increases damage by 1.75x (+0.5x per stack)" + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("impactpower_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("impactpower_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Impact Power " + h.ReturnTierLabel();
            h.LabelDescription = "Adds 1 Bounce, Reduce Damage by 20% (" + StaticColorHexes.AddColorToLabelString("-20% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")\n" +
                StaticColorHexes.AddColorToLabelString("But", StaticColorHexes.Dark_Red_Hex) + " each bounce damage increases damage by 1.75x " +
                StaticColorHexes.AddColorToLabelString("+0.5x", StaticColorHexes.Light_Orange_Hex) + ".";
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
            modulePrinter.OnPostProcessProjectile += PPP;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile -= PPP;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            BounceProjModifier bounceProjModifier = p.gameObject.GetOrAddComponent<BounceProjModifier>();
            bounceProjModifier.numberOfBounces += stack;
            bounceProjModifier.damageMultiplierOnBounce *= (1 +(0.75f * stack));
            bounceProjModifier.OnBounceContext += OBC;
            p.baseData.damage *= 1 - (1 - (1 / (1 + 0.2f * stack))); //this formula fucking sucks lmao
        }
        public void OBC(BounceProjModifier self, SpeculativeRigidbody body)
        {
            var obj = UnityEngine.Object.Instantiate(VFXStorage.MachoBraceDustupVFX, self.projectile.transform);
            obj.transform.localPosition -= new Vector3(1.25f, 1.25f);
            Destroy(obj, 2);
            
        }
    }
}

