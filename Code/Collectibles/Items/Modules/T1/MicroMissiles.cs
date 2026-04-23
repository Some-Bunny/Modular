using Alexandria.ItemAPI;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModularMod
{
    public class MicroMissiles : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MicroMissiles))
        {
            Name = "Micro Missiles",
            Description = "Arsenal",
            LongDescription = "Increases clip size by 25% (+25% per stack). On reload, fire a missile (+1 missile per stack) for every 15 enemies killed on the current floor.",
            ManualSpriteCollection = StaticCollections.Module_T1_Collection,
            ManualSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("micromissiles_tier1_module"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.AltSpriteID = StaticCollections.Module_T1_Collection.GetSpriteIdByName("micromissiles_tier1_alt_module.png");
            h.Tier = ModuleTier.Tier_1;
            h.AdditionalWeightMultiplier = 0.7f;
            h.LabelName = "Micro Missiles " + h.ReturnTierLabel();
            h.LabelDescription = $"Increases clip size by 25% ({StaticColorHexes.AddColorToLabelString("+25%", StaticColorHexes.Light_Orange_Hex)}).\nOn reload, fire a missile ({StaticColorHexes.AddColorToLabelString("+1 missile", StaticColorHexes.Light_Orange_Hex)}) for every 15 enemies killed\non the current floor.";
            h.AddModuleTag(BaseModuleTags.CONDITIONAL);
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.cyan);
            h.Offset_LabelDescription = new Vector2(0.125f, -0.5f);
            h.Offset_LabelName = new Vector2(0.125f, 1.75f);
            ID = h.PickupObjectId;
        }


        public static int ID;
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            this.gunStatModifier = new ModuleGunStatModifier()
            {
                Name = "MiniMissile",
                ClipSize_Process = ProcessClipSize
            };
            modulePrinter.ProcessGunStatModifier(this.gunStatModifier);
            modulePrinter.OnGunReloaded += OGR;
            modulePrinter.OnKilledEnemy += OKE;
            modulePrinter.OnNewFloorStarted += ONF;
        }
        public override void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnGunReloaded -= OGR;
            modulePrinter.OnKilledEnemy -= OKE;
            modulePrinter.OnNewFloorStarted -= ONF;
            modulePrinter.RemoveGunStatModifier(this.gunStatModifier);
        }

        public void ONF(ModulePrinterCore modulePrinter, PlayerController playerController)
        {
            MissileKills = 0;
        }
        public void OKE(ModulePrinterCore modulePrinter, PlayerController playerController, AIActor aIActor)
        {
            Kills--;
            if (Kills <= 0)
            {
                Kills = 15;
                MissileKills++;
                var fx = playerController.PlayEffectOnActor(DeathTrigger.DeathCrossVFX, new Vector3(0.125f, 1.25f));
                fx.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("missileUp");
                AkSoundEngine.PostEvent("Play_wpn_chamberabbey_reload_01", playerController.gameObject);
            }
        }
        public int Kills = 15;
        public int MissileKills = 0;
        public int ProcessClipSize(int f, ModulePrinterCore modulePrinterCore, ModularGunController modularGunController, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            return f + ((modularGunController.Base_Clip_Size / 4) * modulePrinterCore.ReturnStack(this.LabelName));
        }

        public void OGR(ModulePrinterCore modulePrinterCore, PlayerController player, Gun g)
        {
            player.StartCoroutine(DoMissileBlast());
        }

        public IEnumerator DoMissileBlast()
        {
            int stacc = this.ReturnStack(Stored_Core);
            var g = Stored_Core.ModularGunController.gun;
            var player = Stored_Core.Owner;
            for (int i = 0; i < MissileKills * stacc; i++)
            {
                float acc = Stored_Core.ModularGunController.GetAccuracy(20);
                var p = SpawnManager.SpawnProjectile(Guns.Yari_Launcher.DefaultModule.projectiles[0].gameObject, g.barrelOffset.position, Quaternion.Euler(0, 0, g.CurrentAngle + acc)).GetComponent<Projectile>();
                if (p)
                {
                    AkSoundEngine.PostEvent("Play_BOSS_RatMech_Missile_01", player.gameObject);
                    p.Owner = player;
                    p.Shooter = player.specRigidbody;
                    p.baseData.damage = 7.5f;
                    player.DoPostProcessProjectile(p);
                }
                yield return new WaitForSeconds(0.05f);
            }
            yield break;
        }

    }
}

