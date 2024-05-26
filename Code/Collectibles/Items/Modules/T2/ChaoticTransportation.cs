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
    
    public class ChaoticTransportation : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(ChaoticTransportation))
        {
            Name = "Chaotic Transportation",
            Description = "Where Am I?",
            LongDescription = "Adds 1 (+1 Bounces). Increases Rate Of Fire by 20% (+20% per stack hyperbolically). Projectiles will randomly teleport to anywhere in the room 1 (+1) time." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("chaotictransportation_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);

            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("chaotictransportation_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Chaotic Transportation " + h.ReturnTierLabel();
            h.LabelDescription = "Adds 1 ("+StaticColorHexes.AddColorToLabelString("+1")+") Bounces.\nIncreases Rate Of Fire by 20% ("+StaticColorHexes.AddColorToLabelString("+20% hyperbolically")+").\nProjectiles can randomly teleport to anywhere in the room 1 ("+StaticColorHexes.AddColorToLabelString("+1")+") time.";

            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 7;
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();

            EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            if (UnityEngine.Random.value > 0.01f) { return; }
            var t = p.gameObject.GetOrAddComponent<Transport>();
            p.gameObject.GetOrAddComponent<BounceProjModifier>().numberOfBounces += 1;
            t.AmountOfTeleports = 1;
            p.baseData.speed *= 1.1f;
            p.UpdateSpeed();
        }

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Transportation",
                FireRate_Process = PFR,
                ChargeSpeed_Process = PFR,
            };            
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            var t = p.gameObject.GetOrAddComponent<Transport>();
            p.gameObject.GetOrAddComponent<BounceProjModifier>().numberOfBounces += this.ReturnStack(modulePrinterCore);
            t.AmountOfTeleports = this.ReturnStack(modulePrinterCore);
            p.baseData.speed *= 1.1f;
            p.UpdateSpeed();
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
        }

        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + ((modularGunController.Base_Clip_Size / 3) * modulePrinterCore.ReturnStack(this.LabelName));
        }

        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinter);
            return f - (f - (f / (1 + (0.20f * stack))));
        }
    }
    

    public class Transport : MonoBehaviour
    {
        private void Start() 
        { 
            self = this.GetComponent<Projectile>();
            newRNG = UnityEngine.Random.value;
        }

        public float E = 0;
        public float newRNG;
        public void Update()
        {
            if (AmountOfTeleports == 0) { return; }
            if (E < newRNG)
            {
                E += BraveTime.DeltaTime;
                return;
            }
            AmountOfTeleports--;
            E = 0;
            newRNG = UnityEngine.Random.value * 1.15f;
            VFXStorage.SpiratTeleportVFX.SpawnAtPosition(this.transform.position);

            var P = this.transform.position.GetAbsoluteRoom().GetRandomAvailableCellDumb().ToCenterVector3(0);
            VFXStorage.SpiratTeleportVFX.SpawnAtPosition(P);

            self.transform.position = P;
            self.specRigidbody.Reinitialize();
            self.Direction = self.GetVectorToNearestEnemy() - self.transform.PositionVector2();
            
        }

        public int AmountOfTeleports  = 1;

        public void Redirect(float angle)
        {
            if (self == null) { return; }
            self.SendInDirection(Toolbox.GetUnitOnCircle(angle, 1), true);
        }
        private void OnDestroy() { }
        private Projectile self;
    }
}

