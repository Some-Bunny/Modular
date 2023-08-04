using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
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
    public class CloakPlating : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(CloakPlating))
        {
            Name = "Cloak Plating",
            Description = "+60%",
            LongDescription = "Entering combat cloaks the player for 5 (+2.5 per stack) seconds. Uncloaking forcefully grants a 4x (+1 per stack) damage multiplier that degrades fast. Grants a 30% movement speed buff while cloaked." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("cloakup_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("cloakup_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Cloak Plating " + h.ReturnTierLabel();
            h.LabelDescription = "Entering combat cloaks the player for 6 ("+ StaticColorHexes.AddColorToLabelString("+3", StaticColorHexes.Light_Orange_Hex) + ") seconds.\nForcefully uncloaking grants a\n" +
                "4x (" + StaticColorHexes.AddColorToLabelString("+2", StaticColorHexes.Light_Orange_Hex) + ") damage multiplier that degrades fast.\nGrants a 30% movement speed buff while cloaked.";
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();

            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;

        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat += OEC;
            modulePrinter.OnPostProcessProjectile += PPP;
            modulePrinter.OnFrameUpdate += OFU;
            modulePrinter.VoluntaryMovement_Modifier += MovementMod;
        }

        public void OFU(ModulePrinterCore modulePrinter, PlayerController player)
        {
            if (Mult > 1)
            {
                Mult -= BraveTime.DeltaTime;
            }
        }

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnEnteredCombat -= OEC;
            modulePrinter.OnPostProcessProjectile -= PPP;
            modulePrinter.OnFrameUpdate -= OFU;

            modulePrinter.VoluntaryMovement_Modifier -= MovementMod;

        }

        public Vector2 MovementMod(Vector2 currentVel, ModulePrinterCore core, PlayerController p)
        {
            return currentVel *= 1 + (0.3f * (core.cloakDoer.currentState == CloakDoer.Cloak_State.Active ? 1 : 0));
        }

        public float DamageMax = 5;

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
            int stack = this.ReturnStack(modulePrinter);
            DamageMax = 2 + (stack * 2);
        }

        public void OEC(ModulePrinterCore modulePrinterCore, RoomHandler room, PlayerController p)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            modulePrinterCore.cloakDoer.ProcessCloak(new CloakDoer.CloakContext()
            {
                Length = 3f + (stack * 3f),
                OnForceCloakBroken = PP,
                //Retrigger_Force_Cloak_Break = false
            });
        }

        public void PP(PlayerController ppe)
        {
            AkSoundEngine.PostEvent("Play_BOSS_cyborg_storm_01", ppe.gameObject);
            ppe.PlayEffectOnActor(VFXStorage.MachoBraceDustupVFX, new Vector3(-1f, -1f));
            Mult = DamageMax;
        }

        private float Mult = 1;
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.baseData.damage *= Mult;
            p.AdditionalScaleMultiplier *= Mathf.Min(Mult, 2.5f);
        }
    }
}

