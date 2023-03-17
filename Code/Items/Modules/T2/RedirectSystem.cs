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
    public class RedirectSystem : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(RedirectSystem))
        {
            Name = "Redirect System",
            Description = "Exchange Rate",
            LongDescription = "Increases Clip Size by 50% (+50% per stack). Decreases reload time by 15% (+15% hyperbolically) All shots will turn to face the same angle as the last fired shot." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
            ManualSpriteCollection = StaticCollections.Module_T2_Collection,
            ManualSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("redirectsystem_t2_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T2_Collection.GetSpriteIdByName("redirectsystem_t2_module_alt");
            h.Tier = ModuleTier.Tier_2;
            h.LabelName = "Redirect System " + h.ReturnTierLabel();
            h.LabelDescription = "Increases Clip Size by 50% (" + StaticColorHexes.AddColorToLabelString("+50%", StaticColorHexes.Light_Orange_Hex) + ").\nDecreases reload time by 15% (" + StaticColorHexes.AddColorToLabelString("+15% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")\nAll shots will turn to face the same angle as the last fired shot.";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 9;
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "Redirect",
                ClipSize_Process = ProcessClipSize,
                Reload_Process = ProcessReloadTime,
            };
            modularGunController.statMods.Add(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            p.gameObject.GetOrAddComponent<RedirectComp>();
            p.baseData.speed *= 0.66f;
            p.UpdateSpeed();
        }

        public static List<RedirectComp> allActiveComps = new List<RedirectComp>();

        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            if (modularGunController.statMods.Contains(this.gunStatModifier)) 
            {
                modularGunController.statMods.Remove(this.gunStatModifier);
            }
            modulePrinter.OnPostProcessProjectile -= PPP;
        }

        public override void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool truePickup)
        {
        }

        public float ProcessReloadTime(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f / (1 + this.ReturnStack(modulePrinterCore));
        }

        public int ProcessClipSize(int clip, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return clip + ((modularGunController.Base_Clip_Size / 2) * modulePrinterCore.ReturnStack(this.LabelName));
        }

        public float PFR(float f, ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinter);
            return f - (f - (f / (1 + (0.15f * stack))));
        }
    }

    public class RedirectComp : MonoBehaviour
    {
        private void Start() { self = this.GetComponent<Projectile>(); 
            foreach (var entry in RedirectSystem.allActiveComps)
            {
                entry.Redirect(self.m_currentDirection.ToAngle());
            }
            RedirectSystem.allActiveComps.Add(this); }
        public void Redirect(float angle)
        {
            var a = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(401) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, self.sprite.WorldCenter, Quaternion.Euler(0, 0, angle - 180));
            Destroy(a, 2);
            self.SendInDirection(Toolbox.GetUnitOnCircle(angle, 1), true);
        }
        private void OnDestroy()
        {
            
            if (RedirectSystem.allActiveComps.Contains(this))
            {
                RedirectSystem.allActiveComps.Remove(this);
            }
            
        }
        private Projectile self;
    }

}

