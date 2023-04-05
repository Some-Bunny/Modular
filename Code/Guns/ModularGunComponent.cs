using System;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using UnityEngine;
using static Alexandria.ItemAPI.ItemBuilder;

namespace ModularMod 
{

    public class ModuleGunStatModifier
    {
        public string Name = "____";
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> FireRate_Process;
        public Func<int, ModulePrinterCore, ModularGunController, PlayerController, int> ClipSize_Process;
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> Reload_Process;
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> Accuracy_Process;

    }

    public class ModularGunController : MonoBehaviour 
    {
        public Gun gun;
        public PlayerController Player;
        public ModulePrinterCore PrinterSelf;
        public bool isAlt;

        public float Base_Reload_Time;
        public int Base_Clip_Size;
        public float Base_Fire_Rate;
        public float Base_Accuracy;
        public VFXPool Base_Muzzleflash;

        public int AdditionalPowerSupply = 0;

        public string DefaultSwitchGroup;
        public Dictionary<ProjectileModule, Tuple<float, float>> Default_Module_And_Stats = new Dictionary<ProjectileModule, Tuple<float, float>>();

        public Dictionary<ProjectileModule, Tuple<float, float>> Modified_Module_And_Stats = new Dictionary<ProjectileModule, Tuple<float, float>>();

        public Dictionary<ProjectileModule.ChargeProjectile, float> Default_ChargeProj_And_Cooldown = new Dictionary<ProjectileModule.ChargeProjectile, float>();
        private Dictionary<ProjectileModule.ChargeProjectile, float> Modified_ChargeProj_And_Cooldown = new Dictionary<ProjectileModule.ChargeProjectile, float>();


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
            
            Base_Fire_Rate = gun.DefaultModule.cooldownTime;
            
            Base_Accuracy = gun.DefaultModule.angleVariance;
            
            Base_Muzzleflash = gun.muzzleFlashEffects;
            foreach (var mod in gun.Volley.projectiles)
            {
                Default_Module_And_Stats.Add(mod,new Tuple<float, float>(mod.cooldownTime, mod.angleVariance));
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
            int c = GetModNumberOfShotsInClip(Player);
            float f = Base_Fire_Rate;
            float q = Base_Accuracy;
            //ETGModConsole.Log("Base RoF: " + f);
            //ETGModConsole.Log("Base Rel: " + r);
            //ETGModConsole.Log("Base Clip: " + c);
            //ETGModConsole.Log("======");

            foreach (var entry in statMods)
            {
                //ETGModConsole.Log("Premod RoF: " + f);
                //ETGModConsole.Log("Premod Rel: " + r);
                //ETGModConsole.Log("Premod Clip: " + c);

                if (entry.FireRate_Process != null)  { f = entry.FireRate_Process(f, PrinterSelf, this, Player);}
                if (entry.ClipSize_Process != null) {c = entry.ClipSize_Process(c, PrinterSelf, this, Player);}
                if (entry.Reload_Process != null)  {r = entry.Reload_Process(r, PrinterSelf, this, Player); }
                if (entry.Accuracy_Process != null) { q = entry.Accuracy_Process(q, PrinterSelf, this, Player); }

                //ETGModConsole.Log("Postmod RoF: " + f);
                //ETGModConsole.Log("Postmod Rel: " + r);
                //ETGModConsole.Log("Postmod Clip: " + c);
                //ETGModConsole.Log("\n\n");

            }
            //ETGModConsole.Log("Processed RoF: " + f);
            //ETGModConsole.Log("Processed Rel: " + r);
            //ETGModConsole.Log("Processed Clip: " + c);
            //ETGModConsole.Log("======");
            this.gun.DefaultModule.cooldownTime = f;
            this.gun.DefaultModule.angleVariance = q;
            this.gun.reloadTime = r;
            this.gun.DefaultModule.numberOfShotsInClip = c;

            foreach (var cont in Default_Module_And_Stats)
            {
                if (cont.Key != this.gun.DefaultModule)
                {
                    float BaseFireRate = cont.Value.First;
                    float BaseAngle = cont.Value.Second;

                    foreach (var entry in statMods)
                    {
                        if (entry.FireRate_Process != null) { BaseFireRate = entry.FireRate_Process(BaseFireRate, PrinterSelf, this, Player); }
                        if (entry.Accuracy_Process != null) { BaseAngle = entry.Accuracy_Process(BaseAngle, PrinterSelf, this, Player); }
                    }
                    cont.Key.cooldownTime = BaseFireRate;
                    cont.Key.angleVariance = BaseAngle;
                }
            }

            foreach (var cont in Default_ChargeProj_And_Cooldown)
            {
                float BaseRate = cont.Value;
                foreach (var entry in statMods)
                {
                    if (entry.FireRate_Process != null) { BaseRate = entry.FireRate_Process(BaseRate, PrinterSelf, this, Player); }
                }
                cont.Key.ChargeTime = BaseRate;
            }

            foreach (var cont in Modified_Module_And_Stats)
            {
                float BaseFireRate = cont.Value.First;
                float BaseAngle = cont.Value.Second;

                foreach (var entry in statMods)
                {
                    if (entry.FireRate_Process != null) { BaseFireRate = entry.FireRate_Process(BaseFireRate, PrinterSelf, this, Player); }
                    if (entry.Accuracy_Process != null) { BaseAngle = entry.Accuracy_Process(BaseAngle, PrinterSelf, this, Player); }
                }
                cont.Key.angleVariance = BaseAngle;
                cont.Key.cooldownTime = BaseFireRate;
            }

            foreach (var cont in Modified_ChargeProj_And_Cooldown)
            {
                float BaseRate = cont.Value;
                foreach (var entry in statMods)
                {
                    if (entry.FireRate_Process != null) { BaseRate = entry.FireRate_Process(BaseRate, PrinterSelf, this, Player); }
                }
                cont.Key.ChargeTime = BaseRate;
            }
        }


        private void Update()
        {
            if (Player && PrinterSelf && gun && GunUpdate != null)
            {
                GunUpdate(PrinterSelf, Player, gun);
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
                            Modified_Module_And_Stats = new Dictionary<ProjectileModule, Tuple<float, float>>();

                            foreach (var entry in this.gun.alternateVolley.projectiles)
                            {
                                Modified_Module_And_Stats.Add(entry, new Tuple<float, float>(entry.cooldownTime, entry.angleVariance));
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
            if (gun.Volley)
            {
                if (gun.Volley.projectiles != null | gun.Volley.projectiles.Count > 0)
                {
                    if (storedCountBase != gun.Volley.projectiles.Count)
                    {
                        storedCountBase = gun.Volley.projectiles.Count;
                        foreach (var cont in this.gun.Volley.projectiles)
                        {
                            Default_Module_And_Stats = new Dictionary<ProjectileModule, Tuple<float, float>>();
                            foreach (var entry in this.gun.Volley.projectiles)
                            {
                                Default_Module_And_Stats.Add(entry, new Tuple<float, float>(entry.cooldownTime, entry.angleVariance));

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
        private int storedCount = 0;
        private int storedCountBase = 0;

    }
}