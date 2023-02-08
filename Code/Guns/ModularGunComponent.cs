using System;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using UnityEngine;
namespace ModularMod 
{

    public class ModuleGunStatModifier
    {
        public string Name = "____";
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> FireRate_Process;
        public Func<int, ModulePrinterCore, ModularGunController, PlayerController, int> ClipSize_Process;
        public Func<float, ModulePrinterCore, ModularGunController, PlayerController, float> Reload_Process;
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

        public void Start()
        {
            this.gun = base.GetComponent<Gun>();
            Gun gun = this.gun;
            Base_Reload_Time = gun.reloadTime;
            Base_Clip_Size = gun.DefaultModule.GetModNumberOfShotsInClip(gun.CurrentOwner);
            Base_Fire_Rate = gun.DefaultModule.cooldownTime;
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
                gun.OnReloadPressed += OnReloadPressed;
            }
        }

        public void ProcessStats()
        {
            if (gun == null) { return; }
            if (PrinterSelf == null) { return; }
            if (Player == null) { return; }

            float r = Base_Reload_Time;
            int c = Base_Clip_Size;
            float f = Base_Fire_Rate;
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

                //ETGModConsole.Log("Postmod RoF: " + f);
                //ETGModConsole.Log("Postmod Rel: " + r);
                //ETGModConsole.Log("Postmod Clip: " + c);
                //ETGModConsole.Log("\n\n");

            }
            //ETGModConsole.Log("Processed RoF: " + f);
            //ETGModConsole.Log("Processed Rel: " + r);
            //ETGModConsole.Log("Processed Clip: " + c);
            //ETGModConsole.Log("======");

            this.gun.reloadTime = r;
            this.gun.DefaultModule.numberOfShotsInClip = c;
            this.gun.DefaultModule.cooldownTime = f;
        }


        private void Update()
        {
            if (Player && PrinterSelf && gun && GunUpdate != null)
            {
                GunUpdate(PrinterSelf, Player, gun);
            }
        }
    }
}