using Alexandria.ItemAPI;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static BossFinalRogueLaunchShips1;
using static dfMaterialCache;
using static tk2dSpriteCollectionDefinition;


namespace ModularMod
{
    public class QuantumLeap : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(QuantumLeap))
        {
            Name = "Quantum Leap",
            Description = "Particles In The Universe",
            LongDescription = "Doubles reload time and clip size. Projectules fired are put into Stasis. Upon reloading an empty clip, enter a Cloak that removes all of your projectiles from Stasis, gaining slight homing, damage and bouncing. (+Bouncing and Damage per stack). Exiting your cloak puts all of your projectiles back into Stasis." + "\n\n" + "Tier:\n" + DefaultModule.ReturnTierLabel(DefaultModule.ModuleTier.Tier_3),
            ManualSpriteCollection = StaticCollections.Module_T3_Collection,
            ManualSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("quantumleap_t3_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T3_Collection.GetSpriteIdByName("quantumleap_t3_module_alt");
            h.Tier = ModuleTier.Tier_3;
            h.LabelName = "Quantum Leap " + h.ReturnTierLabel();
            h.LabelDescription = "Doubles reload time and clip size.\nProjectiles fired are put into "+StaticColorHexes.AddColorToLabelString("Stasis", StaticColorHexes.Blue_Color_Hex)+". Upon reloading an empty clip,\nenter a Cloak that removes all of your projectiles from "+ StaticColorHexes.AddColorToLabelString("Stasis", StaticColorHexes.Blue_Color_Hex) + ",\ngaining slight homing, damage and bouncing. ("+ StaticColorHexes.AddColorToLabelString("+Bouncing and Damage", StaticColorHexes.Light_Orange_Hex) + ")\nExiting your cloak puts all of your projectiles back into "+ StaticColorHexes.AddColorToLabelString("Stasis", StaticColorHexes.Blue_Color_Hex) + ".";

            h.AddModuleTag(BaseModuleTags.TRADE_OFF);
            h.AddModuleTag(BaseModuleTags.DEFENSIVE);
            h.AddModuleTag(BaseModuleTags.UNIQUE);
            h.AdditionalWeightMultiplier = 0.8f;

            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.yellow);
            h.Offset_LabelDescription = new Vector2(0.25f, -1.125f);
            h.Offset_LabelName = new Vector2(0.25f, 1.875f);
            h.OverrideScrapCost = 15;
            ID = h.PickupObjectId;
        }
        public static int ID;


        public override void OnFirstPickup(ModulePrinterCore printer, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                ClipSize_Process = ClipTime,
                Reload_Process = ReloadTime,
            };
            printer.ProcessGunStatModifier(this.gunStatModifier);
            printer.OnPostProcessProjectileOneFrameDelay += PPP;
            printer.OnGunReloaded += OGR;
        }

        public int ClipTime(int t, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return t * 2;
        }
        public float ReloadTime(float f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            return f * 2;
        }
        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player, bool IsCrit)
        {
            int stack = this.ReturnStack(modulePrinterCore);

            var a = p.gameObject.AddComponent<QuantumComponent>();
            a.self = p;
            a.Stack = stack;
        }

        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            modulePrinterCore.cloakDoer.ProcessCloak(new CloakDoer.CloakContext()
            {
                Length = g.reloadTime,
                OnForceCloakBroken = PP,
                OnCloakBroken = PP,
                OnEnteredCloak = PP_E
            });
        }
        public void PP_E(PlayerController ppe)
        {
            for (int i = allActiveComps.Count - 1; i > -1; i--)
            {
                var entry = allActiveComps[i];
                entry.Redirect();
            }
        }
        public void PP(PlayerController ppe)
        {
            for (int i = allActiveComps.Count - 1; i > -1; i--)
            {
                var entry = allActiveComps[i];
                entry.DoCloak();
            }
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
            modulePrinter.OnPostProcessProjectileOneFrameDelay -= PPP;
            modulePrinter.OnGunReloaded -= OGR;
            player.stats.RecalculateStats(player);

        }
        public static List<QuantumComponent> allActiveComps = new List<QuantumComponent>();

        public class QuantumComponent : MonoBehaviour
        {
            public int Stack;
            private void Start()
            {
                QuantumLeap.allActiveComps.Add(this);
                savedMaterial = self.sprite.renderer.material.shader;
                savedSpeed = self.baseData.speed;
                self.sprite.renderer.material.shader = StaticShaders.Hologram_Shader;
                self.baseData.speed = 0f;
                self.UpdateSpeed();
                isInStasis = true;
                self.collidesWithEnemies = false;
                self.UpdateCollisionMask();
                if (ConfigManager.DoVisualEffect == true)
                {
                    var vfx = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect, self.sprite.WorldCenter, Quaternion.identity);
                    Destroy(vfx, 3);
                }
            }
            private bool isInStasis = false;
            private Shader savedMaterial;
            private float savedSpeed;

            public void DoCloak()
            {
                if (isInStasis == true) { return; }
                if (self == null) { OnDestroy(); return; }
                isInStasis = true;
                self.sprite.renderer.material.shader = StaticShaders.Hologram_Shader;
                self.baseData.speed = 0f;
                self.UpdateSpeed();
                self.collidesWithEnemies = false;
                self.UpdateCollisionMask();
                if (ConfigManager.DoVisualEffect == true)
                {
                    var vfx = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect, self.sprite.WorldCenter, Quaternion.identity);
                    Destroy(vfx, 3);
                }
            }
            public void Redirect()
            {
                if (isInStasis == false) { return; }
                if (self == null) { return; }


                if (ConfigManager.DoVisualEffect == true)
                {
                    var vfx = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(13) as Gun).DefaultModule.projectiles[0].hitEffects.enemy.effects[0].effects[0].effect, self.sprite.WorldCenter, Quaternion.identity);
                    Destroy(vfx, 3);
                }

                self.collidesWithEnemies = true;
                self.UpdateCollisionMask();

                self.sprite.renderer.material.shader = savedMaterial;
                self.baseData.speed = savedSpeed;
                self.baseData.range += 10;
                self.UpdateSpeed();
                //var vec = Alexandria.Misc.ProjectileUtility.GetVectorToNearestEnemy(self);
                //self.SendInDirection(vec != Vector2.zero ? vec : Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), 1), true, true);
                var homing = self.gameObject.GetOrAddComponent<HomingModifier>();
                homing.AngularVelocity += 30 + (Stack*15);
                homing.HomingRadius += 4 + Stack;
                isInStasis = false;


                BounceProjModifier bounceProjModifier = self.gameObject.GetOrAddComponent<BounceProjModifier>();
                bounceProjModifier.numberOfBounces += Stack * 2;
                self.pierceMinorBreakables = true;

                PierceProjModifier pierce = self.gameObject.GetOrAddComponent<PierceProjModifier>();
                pierce.penetration += Stack;

                self.baseData.damage *= 1 + (0.3f * Stack);
            }



            private void OnDestroy()
            {
                if (QuantumLeap.allActiveComps.Contains(this))
                {
                    QuantumLeap.allActiveComps.Remove(this);
                }
            }
            public Projectile self;
        }
    }
}

