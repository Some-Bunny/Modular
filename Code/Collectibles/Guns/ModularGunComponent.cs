﻿
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using HarmonyLib;
using UnityEngine;
using static Alexandria.DungeonAPI.SpecialComponents;
using static Alexandria.ItemAPI.ItemBuilder;
using Alexandria.Misc;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using static UnityEngine.UI.GridLayoutGroup;
using Brave.BulletScript;

namespace ModularMod 
{

    public class ModuleGunStatModifier
    {
        public string Name = "____";
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> FireRate_Process;
        public Func<int, ModulePrinterCore, ModularGunController, PlayerController, int> ClipSize_Process;
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> Reload_Process;
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> Accuracy_Process;
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> ChargeSpeed_Process;
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> AngleFromAim_Process;
        public Func<int, int, ModulePrinterCore, ModularGunController, PlayerController, int> FinaleClipSize_Process;

        public Func<int, int, ModulePrinterCore, ModularGunController, PlayerController, int> BurstAmount_Process;

        public Func<int, ModulePrinterCore, ModularGunController, PlayerController, int> Post_Calculation_ClipSize_Process;

    }

    [HarmonyPatch(typeof(Gun), nameof(Gun.Update))]
    public class GunPanic
    {
        static void Postfix(Gun __instance)
        {
            if (__instance.m_owner != null)
            {
                if (__instance.m_owner is PlayerController player && player.PlayerHasCore() != null && player.CurrentStoneGunTimer > 0 && GameManager.Instance.IsPaused == false)
                {
                    Vector3 vector = __instance.sprite.WorldBottomLeft.ToVector3ZisY(0);
                    Vector3 vector2 = __instance.sprite.WorldTopRight.ToVector3ZisY(0);
                    float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                    float num2 = 15f * num;
                    int num4 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime)); ;
                    Vector3 direction = Toolbox.GetUnitOnCircleVec3(BraveUtility.RandomAngle(), 1);
                    float magnitudeVariance = 0.7f;
                    float? startLifetime = new float?(UnityEngine.Random.Range(0.8f, 1.4f));
                    GlobalSparksDoer.DoRandomParticleBurst(num4, vector, vector2, direction, 0, magnitudeVariance, 0.333f, startLifetime, Color.gray, GlobalSparksDoer.SparksType.DARK_MAGICKS);
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameUIRoot), nameof(GameUIRoot.UpdateGunDataInternal))]
    public class NopeOutVisibility
    {
        public const string ERROR = "[color #ff0000][ERROR]: WEAPON JAM[/color]";
        static void Postfix(GameUIRoot __instance, PlayerController targetPlayer, GunInventory inventory, int inventoryShift, GameUIAmmoController targetAmmoController, int labelTarget)
        {
            bool isModularStoned = (targetPlayer.PlayerHasCore() != null && targetPlayer.CurrentStoneGunTimer > 0);
            if (isModularStoned == false) { return; }


            __instance.m_gunNameVisibilityTimers[labelTarget] -= __instance.m_deltaTime;
            if (__instance.m_gunNameVisibilityTimers[labelTarget] > 1f)
            {
                __instance.gunNameLabels[labelTarget].IsVisible = true;
                __instance.gunNameLabels[labelTarget].Opacity = 1f;
                __instance.gunNameLabels[labelTarget].Text = $"{NopeOutVisibility.ERROR}\n" + __instance.gunNameLabels[labelTarget].Text;

            }
            else if (__instance.m_gunNameVisibilityTimers[labelTarget] > 0f)
            {
                __instance.gunNameLabels[labelTarget].IsVisible = true;
                __instance.gunNameLabels[labelTarget].Opacity = __instance.m_gunNameVisibilityTimers[labelTarget];
                __instance.gunNameLabels[labelTarget].Text = $"{NopeOutVisibility.ERROR}\n" + $"{__instance.gunNameLabels[labelTarget].Text}";

            }
            else
            {
                __instance.gunNameLabels[labelTarget].IsVisible = true;
                __instance.gunNameLabels[labelTarget].Opacity = 1;
                __instance.gunNameLabels[labelTarget].Text = $"{NopeOutVisibility.ERROR}";
            }
        }
    }



    public class ModularGunController : MonoBehaviour
    {



        public class ModuleStatStorage
        {
            public float Angle_From_Aim;
            public float Accuracy;
            public float Rate_Of_Fire;
            public int Clip_Size;
            public int FinalProjectiles_Count;
            public int BurstShots;

        }


        public Projectile projectileToCopyForFlak;

        public Gun gun;
        public PlayerController Player;
        public ModulePrinterCore PrinterSelf;
        public bool isAlt;

        public float Base_Reload_Time;
        public int Base_Clip_Size;
        public int Base_Finale_Clip_Size;

        public float Base_Fire_Rate;
        public float Base_Accuracy;
        public float Base_AngleFromAim;

        public VFXPool Base_Muzzleflash;
        public int AdditionalPowerSupply = 0;
        public string DefaultSwitchGroup;
        public Dictionary<ProjectileModule, ModuleStatStorage> Default_Module_And_Stats = new Dictionary<ProjectileModule, ModuleStatStorage>();
        public Dictionary<ProjectileModule, ModuleStatStorage> Modified_Module_And_Stats = new Dictionary<ProjectileModule, ModuleStatStorage>();
        public Dictionary<ProjectileModule.ChargeProjectile, float> Default_ChargeProj_And_Cooldown = new Dictionary<ProjectileModule.ChargeProjectile, float>();
        private Dictionary<ProjectileModule.ChargeProjectile, float> Modified_ChargeProj_And_Cooldown = new Dictionary<ProjectileModule.ChargeProjectile, float>();
        private ProjectileVolleyData storedVolley;
        public List<ModuleGunStatModifier> statMods = new List<ModuleGunStatModifier>();

        private void Awake()
        {

        }



        public void OnReloadPressed(PlayerController p, Gun g, bool b) 
        {
            if (g.ClipCapacity > g.ClipShotsRemaining)
            {
                if (OnReload != null && PrinterSelf) 
                {
                    OnReload(PrinterSelf ,p, g);
                }
            }
        }

        public Projectile OnPreFireMod(Gun g, Projectile p, ProjectileModule pm)
        {
            if (OnPreFired != null && PrinterSelf) { OnPreFired(PrinterSelf, p, Player, g); }
            return p;
        }


        public Action<ModulePrinterCore, Projectile, PlayerController, Gun> OnPreFired;
        public Action<ModulePrinterCore, PlayerController, Gun> OnReload;
        public Action<ModulePrinterCore, PlayerController, Gun> GunUpdate;
        public Action<ModulePrinterCore, PlayerController, Gun> OnGunFired;

        public void Start()
        {
            this.gun = base.GetComponent<Gun>();
            Gun gun = this.gun;
            Base_Reload_Time = gun.reloadTime;
            Base_Clip_Size = gun.DefaultModule.GetModNumberOfShotsInClip(gun.CurrentOwner);
            Base_Finale_Clip_Size = gun.DefaultModule.numberOfFinalProjectiles;
            Base_Fire_Rate = gun.DefaultModule.cooldownTime;
            Base_Accuracy = gun.DefaultModule.angleVariance;

            Base_AngleFromAim = gun.DefaultModule.angleFromAim;
            Base_Muzzleflash = gun.muzzleFlashEffects;
            storedVolley = gun.Volley;

            
            foreach (var mod in gun.Volley.projectiles)
            {
                if (!Default_Module_And_Stats.ContainsKey(mod))
                {
                    Default_Module_And_Stats.Add(mod, new ModuleStatStorage()
                    {
                        Accuracy = mod.angleVariance,
                        Angle_From_Aim = mod.angleFromAim,
                        Rate_Of_Fire = mod.cooldownTime,
                        Clip_Size = mod.numberOfShotsInClip,
                        FinalProjectiles_Count = mod.numberOfFinalProjectiles,
                        BurstShots = mod.burstShotCount
                    });
                }
                foreach (var thing in mod.chargeProjectiles)
                {
                    if (!Default_ChargeProj_And_Cooldown.ContainsKey(thing))
                    {
                        Default_ChargeProj_And_Cooldown.Add(thing, thing.ChargeTime);
                    }
                }
            }
            storedCount = gun.alternateVolley != null ? gun.alternateVolley.projectiles != null ? gun.alternateVolley.projectiles.Count : 0 : 0;
            if (gun.CurrentOwner is PlayerController player)
            {
                Player = player;
                foreach (var entry in player.passiveItems)
                {
                    if (entry is ModulePrinterCore printerCore)
                    {
                        printerCore.ModularGunController = this;
                        PrinterSelf = printerCore;
                    }
                }
                gun.OnPreFireProjectileModifier += OnPreFireMod;
                gun.OnPostFired += OnPostFired;
                gun.OnReloadPressed += OnReloadPressed;
                DefaultSwitchGroup = gun.gunSwitchGroup;
            }
        }



        public void ResetSwitchGroup()
        {
            if (this.gun == null) { return; }
            this.gun.gunSwitchGroup = DefaultSwitchGroup;
        }

        public void OnPostFired(PlayerController player, Gun gun)
        {
            if (OnGunFired != null)
            {
                OnGunFired(PrinterSelf, player, gun);
            }
        }

        public void ChangeMuzzleFlash(VFXPool newMuzzle)
        {
            this.gun.muzzleFlashEffects = newMuzzle;
        }
        public void RevertMuzzleFlash()
        {
            this.gun.muzzleFlashEffects = Base_Muzzleflash;
        }


        public float GetAccuracy(ProjectileModule mod, float? overrideAngle = null)
        {
            float h = Default_Module_And_Stats[mod].Accuracy;
            foreach (var entry in statMods)
            {
                if (entry.Accuracy_Process != null) { h = entry.Accuracy_Process(overrideAngle != null ? overrideAngle.Value : h, PrinterSelf, this, Player); }
            }
            return h;
        }
        public float GetAccuracy(float overrideAngle)
        {
            foreach (var entry in statMods)
            {
                if (entry.Accuracy_Process != null) { overrideAngle = entry.Accuracy_Process(overrideAngle, PrinterSelf, this, Player); }
            }
            return overrideAngle;
        }
        public float GetRateOfFire(ProjectileModule mod, float? overrideRateOfFire = null)
        {
            float h = Default_Module_And_Stats[mod].Rate_Of_Fire;
            foreach (var entry in statMods)
            {
                if (entry.FireRate_Process != null) { h = entry.FireRate_Process(overrideRateOfFire != null ? overrideRateOfFire.Value : h, PrinterSelf, this, Player); }
            }
            return h;
        }
        public float GetRateOfFire(float overrideRateOfFire)
        {
            foreach (var entry in statMods)
            {
                if (entry.FireRate_Process != null) { overrideRateOfFire = entry.FireRate_Process(overrideRateOfFire, PrinterSelf, this, Player); }
            }
            return overrideRateOfFire;
        }
        public float GetReload(float overrideReload)
        {
            foreach (var entry in statMods)
            {
                if (entry.Reload_Process != null) { overrideReload = entry.Reload_Process(overrideReload, PrinterSelf, this, Player); }
            }
            return overrideReload;
        }
        public float GetChargeSpeed(float overrideChargeSpeed)
        {
            foreach (var entry in statMods)
            {
                if (entry.ChargeSpeed_Process != null) { overrideChargeSpeed = entry.ChargeSpeed_Process(overrideChargeSpeed, PrinterSelf, this, Player); }
            }

            return overrideChargeSpeed;
        }

        public int GetClipSize(int overrideClipSize)
        {
            foreach (var entry in statMods)
            {
                if (entry.ClipSize_Process != null) { overrideClipSize = entry.ClipSize_Process(overrideClipSize, PrinterSelf, this, Player); }
            }

            overrideClipSize = (Mathf.Max(overrideClipSize, 1));
            foreach (var entry in statMods)
            {
                if (entry.Post_Calculation_ClipSize_Process != null) { overrideClipSize = entry.Post_Calculation_ClipSize_Process(overrideClipSize, PrinterSelf, this, Player); }
            }

            return overrideClipSize;
        }

        public int GetModNumberOfShotsInClip(GameActor owner)
        {
            if (this.Base_Clip_Size == 1)
            {
                return this.Base_Clip_Size;
            }
            if (!(owner != null) || !(owner is PlayerController))
            {
                return this.Base_Clip_Size;
            }
            PlayerController playerController = owner as PlayerController;
            float statValue = playerController.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier);
            float statValue2 = playerController.stats.GetStatValue(PlayerStats.StatType.TarnisherClipCapacityMultiplier);
            int num = Mathf.FloorToInt((float)this.Base_Clip_Size * statValue * statValue2);
            if (num < 0)
            {
                return num;
            }
            return Mathf.Max(num, 1);
        }

        public void ProcessStats()
        {
            if (gun == null) { return; }
            if (PrinterSelf == null) { return; }
            if (Player == null) { return; }

            float r = Base_Reload_Time;
            foreach (var entry in statMods)
            {
                if (entry.Reload_Process != null) { r = entry.Reload_Process(r, PrinterSelf, this, Player); }
            }
            this.gun.reloadTime = r;

            /*
            int c = GetModNumberOfShotsInClip(Player);
            float f = Base_Fire_Rate;
            float q = Base_Accuracy;
            float gg = Base_AngleFromAim;
            int finales = Base_Finale_Clip_Size;
            //ETGModConsole.Log("Base RoF: " + f);
            //ETGModConsole.Log("Base Rel: " + r);
            //ETGModConsole.Log("Base Clip: " + c);
            //ETGModConsole.Log("======");
            //This code fucking sucks...
            foreach (var entry in statMods)
            {
                //ETGModConsole.Log("Premod RoF: " + f);
                //ETGModConsole.Log("Premod Rel: " + r);
                //ETGModConsole.Log("Premod Clip: " + c);

                if (entry.FireRate_Process != null)  { f = entry.FireRate_Process(f, PrinterSelf, this, Player);}
                if (entry.ClipSize_Process != null) {c = entry.ClipSize_Process(c, PrinterSelf, this, Player);}
                if (entry.Reload_Process != null)  {r = entry.Reload_Process(r, PrinterSelf, this, Player); }
                if (entry.Accuracy_Process != null) { q = entry.Accuracy_Process(q, PrinterSelf, this, Player); }

                if (entry.AngleFromAim_Process != null) { gg = entry.AngleFromAim_Process(gg, PrinterSelf, this, Player); }
            }
            */
            /*
            foreach (var entry in statMods)
            {
                if (entry.Post_Calculation_ClipSize_Process != null) { c = entry.Post_Calculation_ClipSize_Process(c, PrinterSelf, this, Player); }
            }

            foreach (var entry in statMods)
            {
                if (entry.FinaleClipSize_Process != null) { finales = entry.FinaleClipSize_Process(finales, c, PrinterSelf, this, Player); }
            }
            */
            //ETGModConsole.Log("Processed RoF: " + f);
            //ETGModConsole.Log("Processed Rel: " + r);
            //ETGModConsole.Log("Processed Clip: " + c);
            //ETGModConsole.Log("======");
            /*
            this.gun.DefaultModule.cooldownTime = f;
            this.gun.DefaultModule.angleVariance = q;
            this.gun.reloadTime = r;
            this.gun.DefaultModule.numberOfShotsInClip = c;
            this.gun.DefaultModule.numberOfFinalProjectiles = finales;

            this.gun.DefaultModule.angleFromAim = gg;
            */



            foreach (var cont in Default_Module_And_Stats)
            {
                float BaseFireRate = cont.Value.Rate_Of_Fire;
                float BaseAngle = cont.Value.Accuracy;
                float AngleFromAim = cont.Value.Angle_From_Aim;

                int ClipSize = cont.Value.Clip_Size;
                int finales = cont.Value.FinalProjectiles_Count;
                int burstShots = cont.Value.BurstShots;

                bool AmmoMaxIsClip = ClipSize == -1;

                foreach (var entry in statMods)
                {
                    if (entry.FireRate_Process != null) { BaseFireRate = entry.FireRate_Process(BaseFireRate, PrinterSelf, this, Player); }
                    if (entry.Accuracy_Process != null) { BaseAngle = entry.Accuracy_Process(BaseAngle, PrinterSelf, this, Player); }
                    if (entry.AngleFromAim_Process != null) 
                    {
                        AngleFromAim = entry.AngleFromAim_Process(AngleFromAim, PrinterSelf, this, Player);
                    }
                    if (AmmoMaxIsClip == false)
                    {
                        if (entry.ClipSize_Process != null) { ClipSize = entry.ClipSize_Process(ClipSize, PrinterSelf, this, Player); }
                    }
                }
                if (AmmoMaxIsClip == false)
                {
                    ClipSize = (Mathf.Max(ClipSize, 1));
                    foreach (var entry in statMods)
                    {
                        if (entry.Post_Calculation_ClipSize_Process != null) { ClipSize = entry.Post_Calculation_ClipSize_Process(ClipSize, PrinterSelf, this, Player); }
                    }
                }

                
                foreach (var entry in statMods)
                {
                    if (entry.FinaleClipSize_Process != null) { finales = entry.FinaleClipSize_Process(finales, ClipSize, PrinterSelf, this, Player); }
                    if (entry.BurstAmount_Process != null) { burstShots = entry.BurstAmount_Process(burstShots, ClipSize, PrinterSelf, this, Player); }
                }
                cont.Key.cooldownTime = BaseFireRate;
                cont.Key.angleVariance = BaseAngle;
                cont.Key.angleFromAim = AngleFromAim;
                cont.Key.numberOfShotsInClip = ClipSize;
                cont.Key.numberOfFinalProjectiles = finales;

            }

            foreach (var cont in Default_ChargeProj_And_Cooldown)
            {
                float BaseRate = cont.Value;
                foreach (var entry in statMods)
                {
                    if (entry.ChargeSpeed_Process != null) { BaseRate = entry.ChargeSpeed_Process(BaseRate, PrinterSelf, this, Player); }
                }
                cont.Key.ChargeTime = BaseRate;
            }

            foreach (var cont in Modified_Module_And_Stats)
            {
                float BaseFireRate = cont.Value.Rate_Of_Fire;
                float BaseAngle = cont.Value.Accuracy;
                float AngleFromAim = cont.Value.Angle_From_Aim;
                int ClipSize = cont.Value.Clip_Size;
                int finales = cont.Value.FinalProjectiles_Count;
                int burstShots = cont.Value.BurstShots;

                bool AmmoMaxIsClip = ClipSize == -1;

                foreach (var entry in statMods)
                {
                    if (entry.FireRate_Process != null) { BaseFireRate = entry.FireRate_Process(BaseFireRate, PrinterSelf, this, Player); }
                    if (entry.Accuracy_Process != null) { BaseAngle = entry.Accuracy_Process(BaseAngle, PrinterSelf, this, Player); }
                    if (entry.AngleFromAim_Process != null) { AngleFromAim = entry.AngleFromAim_Process(AngleFromAim, PrinterSelf, this, Player); }
                    if (AmmoMaxIsClip == false)
                    {
                        if (entry.ClipSize_Process != null) { ClipSize = entry.ClipSize_Process(ClipSize, PrinterSelf, this, Player); }
                    }
                }
                if (AmmoMaxIsClip == false)
                {
                    ClipSize = (Mathf.Max(ClipSize, 1));
                    foreach (var entry in statMods)
                    {
                        if (entry.Post_Calculation_ClipSize_Process != null) { ClipSize = entry.Post_Calculation_ClipSize_Process(ClipSize, PrinterSelf, this, Player); }
                    }
                }
                foreach (var entry in statMods)
                {
                    if (entry.FinaleClipSize_Process != null) { finales = entry.FinaleClipSize_Process(finales, ClipSize, PrinterSelf, this, Player); }
                    if (entry.BurstAmount_Process != null) { burstShots = entry.BurstAmount_Process(burstShots, ClipSize, PrinterSelf, this, Player); }
                }
                cont.Key.cooldownTime = BaseFireRate;
                cont.Key.angleVariance = BaseAngle;
                cont.Key.angleFromAim = AngleFromAim;
                cont.Key.numberOfShotsInClip = ClipSize;
                cont.Key.numberOfFinalProjectiles = finales;
            }

            foreach (var cont in Modified_ChargeProj_And_Cooldown)
            {
                float BaseRate = cont.Value;
                foreach (var entry in statMods)
                {
                    if (entry.ChargeSpeed_Process != null) { BaseRate = entry.ChargeSpeed_Process(BaseRate, PrinterSelf, this, Player); }
                }
                cont.Key.ChargeTime = BaseRate;
            }
        }


        private bool isStoned = false;

        private void Update()
        {
            if (Player && PrinterSelf && gun && GunUpdate != null)
            {
                GunUpdate(PrinterSelf, Player, gun);
            }
            
            if (Player)
            {
                if (Player.CurrentStoneGunTimer > 0)
                {
                    if (isStoned == false)
                    {
                        isStoned = true;
                        AkSoundEngine.PostEvent("Play_BOSS_mineflayer_trigger_01", Player.gameObject);
                    }
                }
                else if (isStoned == true)
                {

                }
            }
            
            if (gun.alternateVolley)
            {
                if (gun.alternateVolley.projectiles != null | gun.alternateVolley.projectiles.Count > 0)
                {
                    if (storedCount != gun.alternateVolley.projectiles.Count)
                    {
                        storedCount = gun.alternateVolley.projectiles.Count;
                        foreach (var cont in this.gun.alternateVolley.projectiles)
                        {
                            Modified_Module_And_Stats = new Dictionary<ProjectileModule, ModuleStatStorage>();

                            foreach (var entry in this.gun.alternateVolley.projectiles)
                            {
                                if (!Modified_Module_And_Stats.ContainsKey(entry))
                                {
                                    Modified_Module_And_Stats.Add(entry, new ModuleStatStorage()
                                    {
                                        Accuracy = entry.angleVariance,
                                        Angle_From_Aim = entry.angleFromAim,
                                        Rate_Of_Fire = entry.cooldownTime,
                                        Clip_Size = entry.numberOfShotsInClip,
                                        FinalProjectiles_Count = entry.numberOfFinalProjectiles,
                                        BurstShots = entry.burstShotCount

                                    });
                                    foreach (var chargeProj in entry.chargeProjectiles)
                                    {
                                        if (!Modified_ChargeProj_And_Cooldown.ContainsKey(chargeProj))
                                        {
                                            Modified_ChargeProj_And_Cooldown.Add(chargeProj, chargeProj.ChargeTime);
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            if (gun.Volley)
            {
                if (gun.Volley.projectiles != null | gun.Volley.projectiles.Count > 0)
                {
                    if (storedCountBase != gun.Volley.projectiles.Count)
                    {
                        storedCountBase = gun.Volley.projectiles.Count;
                        foreach (var cont in this.gun.Volley.projectiles)
                        {
                            Default_Module_And_Stats = new Dictionary<ProjectileModule, ModuleStatStorage>();
                            foreach (var entry in this.gun.Volley.projectiles)
                            {
                                if (!Default_Module_And_Stats.ContainsKey(entry))
                                {
                                    Default_Module_And_Stats.Add(entry, new ModuleStatStorage()
                                    {
                                        Accuracy = entry.angleVariance,
                                        Angle_From_Aim = entry.angleFromAim,
                                        Rate_Of_Fire = entry.cooldownTime,
                                        Clip_Size = entry.numberOfShotsInClip,
                                        FinalProjectiles_Count = entry.numberOfFinalProjectiles,
                                        BurstShots = entry.burstShotCount


                                    });
                                    foreach (var chargeProj in entry.chargeProjectiles)
                                    {
                                        if (!Default_ChargeProj_And_Cooldown.ContainsKey(chargeProj))
                                        {
                                            Default_ChargeProj_And_Cooldown.Add(chargeProj, chargeProj.ChargeTime);
                                        }
                                    }
                                }                      
                            }
                        }
                    }
                }
            }
            if (storedVolley != gun.Volley)
            {
                storedVolley = gun.Volley;
                Default_Module_And_Stats = new Dictionary<ProjectileModule, ModuleStatStorage>();
                foreach (var entry in this.gun.Volley.projectiles)
                {
                    if (!Default_Module_And_Stats.ContainsKey(entry))
                    {
                        Default_Module_And_Stats.Add(entry, new ModuleStatStorage()
                        {
                            Accuracy = entry.angleVariance,
                            Angle_From_Aim = entry.angleFromAim,
                            Rate_Of_Fire = entry.cooldownTime,
                            Clip_Size = entry.numberOfShotsInClip,
                            FinalProjectiles_Count = entry.numberOfFinalProjectiles,
                            BurstShots = entry.burstShotCount
                        });
                        foreach (var chargeProj in entry.chargeProjectiles)
                        {
                            if (!Default_ChargeProj_And_Cooldown.ContainsKey(chargeProj))
                            {
                                Default_ChargeProj_And_Cooldown.Add(chargeProj, chargeProj.ChargeTime);
                            }
                        }
                    }
                }
            }

            ProcessStats();
        }


        private int storedCount = 0;
        private int storedCountBase = 0;
    }
}