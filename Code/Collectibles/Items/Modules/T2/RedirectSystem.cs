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
            LongDescription = "Increases Clip Size by 33% (+33% per stack). Decreases reload time by 20% (+20% hyperbolically) All shots will turn to face the same angle as the last fired shot." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_2),
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
            h.LabelDescription = "Increases Clip Size by 33% (" + StaticColorHexes.AddColorToLabelString("+33%", StaticColorHexes.Light_Orange_Hex) + ").\nDecreases reload time by 25% (" + StaticColorHexes.AddColorToLabelString("+25% hyperbolically", StaticColorHexes.Light_Orange_Hex) + ")\nAll shots will turn to face the same angle as the last fired shot.";

            h.AddModuleTag(BaseModuleTags.TRADE_OFF);

            h.SetTag("modular_module");
            h.AddColorLight(Color.green);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.25f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            h.OverrideScrapCost = 7;
            h.EnergyConsumption = 1;
            h.AddToGlobalStorage();

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
                Reload_Process = PFR,
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            var a = p.gameObject.GetOrAddComponent<RedirectComp>();
            a.StartUp(p, player.CurrentGun);
            p.baseData.speed *= 0.6f;
            p.UpdateSpeed();
        }

        public static List<RedirectComp> allActiveComps = new List<RedirectComp>();

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
            return f - (f - (f / (1 + (0.25f * stack))));
        }
    }

    public class RedirectComp : MonoBehaviour
    {
        public void StartUp(Projectile projectile, Gun gun) 
        {
            self = projectile;
            foreach (var entry in RedirectSystem.allActiveComps)
            {
                entry.Redirect(gun.CurrentAngle);
            }
            self.StartCoroutine(Delay());
        }
        public IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.15f);
            if (this)
            {
                RedirectSystem.allActiveComps.Add(this);
            }
            yield break;
        }

        public void Redirect(float angle)
        {
            if (self == null) { return; }
            /*
            if (ConfigManager.DoVisualEffect == true)
            {
                var a = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(401) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, this.transform.position, Quaternion.Euler(0, 0, angle - 180));
                a.transform.localScale *= 0.3f;
                Destroy(a, 2);
            }
            */
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

