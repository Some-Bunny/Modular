using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Alexandria.ItemAPI;
using Brave.BulletScript;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using MonoMod.RuntimeDetour;
using Planetside;
using UnityEngine;
using static Alexandria.Misc.CustomActions;

namespace ModularMod
{
    public class ModulePrinterCore : PassiveItem
    {
        public static void Init()
        {
            string name = "Modular Printer Core";
            GameObject gameObject = new GameObject(name);
            ModulePrinterCore item = gameObject.AddComponent<ModulePrinterCore>();
            ItemBuilder.AddSpriteToObject(name, "ModularMod/Sprites/Items/Item/modularprintercore.png", gameObject);
            string shortDesc = "Self Sustainment";
            string longDesc = "Allows for the scrapping of weapons into powerful, modular upgrades.\nDisplay enabled Modules by pressing Reload 3 times fast, Disable UI with one press.\n\nTechnology originally purposed for printing nails and other small equipment at construction sites, now used in machines of war.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "mdl");

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.IgnoredByRat = true;
            item.RespawnsIfPitfall = true;
            item.UsesCustomCost = true;
            item.CustomCost = 25;
            item.CanBeDropped = false;
            
            ModulePrinterCore.ModulePrinterCoreID = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;

            new Hook(typeof(SpawnGunslingGun).GetMethod("OnEnter", BindingFlags.Instance | BindingFlags.Public), typeof(ModulePrinterCore).GetMethod("GunslingKingGunCheck"));
        }

        //Hook to make Gunsling King work as intended with Modular
        public static void GunslingKingGunCheck(Action<SpawnGunslingGun> orig, SpawnGunslingGun self)
        {
            TalkDoerLite component = self.Owner.GetComponent<TalkDoerLite>();
            PlayerController player = (!component.TalkingPlayer) ? GameManager.Instance.PrimaryPlayer : component.TalkingPlayer;
            SelectGunslingGun selectGunslingGun = self.FindActionOfType<SelectGunslingGun>();
            CheckGunslingChallengeComplete checkGunslingChallengeComplete = self.FindActionOfType<CheckGunslingChallengeComplete>();
            if (selectGunslingGun != null)
            {
                GameObject selectedObject = selectGunslingGun.SelectedObject;
                currentGunslingKingGun = selectedObject.GetComponent<Gun>();
                Gun gun = LootEngine.TryGiveGunToPlayer(selectedObject, player, false);
                if (gun)
                {
                    gun.CanBeDropped = false;
                    gun.CanBeSold = false;
                    gun.IsMinusOneGun = true;
                    currentGunslingKingGun = gun;

                    if (checkGunslingChallengeComplete != null)
                    {
                        checkGunslingChallengeComplete.GunToUse = gun;
                        checkGunslingChallengeComplete.GunToUsePrefab = selectGunslingGun.SelectedObject.GetComponent<Gun>();
                    }
                }
            }
            self.Finish();
        }
        public static Gun currentGunslingKingGun;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnEnteredCombat += PlayerEnteredCombat;
            player.OnReceivedDamage += OnRecievedDamage;
            player.OnReloadedGun += OnReloadedGun;
            player.OnKilledEnemyContext += OnKilledEnemyContext;
            player.OnDealtDamageContext += OnDealtDamageContext;
            player.PostProcessProjectile += PostProcessProjectile;
            player.PostProcessBeamTick += PostProcessBeamTick;
            player.OnDodgedProjectile += OnDodgedProjectile;
            player.OnDodgedBeam += OnDodgedBeam;
            player.OnRollStarted += OnRollStarted;
            player.OnRoomClearEvent += OnRoomClearEvent;
            player.GunChanged += Player_GunChanged;
            player.OnNewFloorLoaded += NewFloorLoaded;
        }
        private bool b = false;
        public void NewFloorLoaded(PlayerController player)
        {
            b = !b;
            if (b == false) { return; }
            if (player && OnNewFloorStarted != null) { OnNewFloorStarted(this, player); }
        }

        public bool TemporaryDisableDrop = false;

        //Code to check for valid guns, and discards any guns considered invalid
        private void Player_GunChanged(Gun oldGun, Gun newGun, bool isNewGun)
        {
            if (TemporaryDisableDrop == true) { return; }
            if (newGun.GetComponent<ModularGunController>() != null) { return; }
            if (newGun.GetComponent<ModularGunController>() == ModularGunController) { return; }
            if (newGun.PickupObjectId == 251 || newGun.HasTag("modular_special_override"))
            {
                if (isNewGun == true)
                {
                    var t = Toolbox.GenerateText(base.Owner.transform, new Vector2(1.5f, 0.5f), 0.5f, "Gun Override Detected :\nWeapon Will Not Be Dropped.", new Color32(121, 234, 255, 100));
                    t.Invoke("Inv", 3.5f);
                }
                return;
            }
            if (currentGunslingKingGun != null)
            {
                this.StartCoroutine(FrameDelay(newGun));
            }
            else
            {
                base.Owner.ForceDropGun(newGun);
            }
        }
        private IEnumerator FrameDelay(Gun g)
        {
            yield return null;
            if (g == currentGunslingKingGun)
            {
                var t = Toolbox.GenerateText(base.Owner.transform, new Vector2(1.5f, 0.5f), 0.5f, "Gun Override Detected :\nWeapon Will Not Be Dropped.", new Color32(121, 234, 255, 100));
                t.Invoke("Inv", 3.5f);
            }
            else
            {
                base.Owner.ForceDropGun(g);
            }
            yield break;
        }
        public void OnRoomClearEvent(PlayerController p)
        {
            if (p == null) { return; }
            if (p.CurrentRoom == null) { return; }
            if (OnRoomCleared != null) { OnRoomCleared(this, p, p.CurrentRoom); }
        }
        public void OnRollStarted(PlayerController p, Vector2 vector)
        {
            if (RollStarted != null && p != null) { RollStarted(this, p, vector); }
        }
        public void OnDodgedBeam(BeamController b, PlayerController p)
        {
            if (DodgedBeam != null && p != null) { DodgedBeam(this, b, p); }
        }
        public void OnDodgedProjectile(Projectile p)
        {
            if (base.Owner == null) { return; }
            if (DodgedProjectile != null && p != null) { DodgedProjectile(this, p, base.Owner); }
        }

        public void UpdateModularGunController()
        {
            if (ModularGunController != null) { return; }
            ModularGunController = base.Owner.CurrentGun.GetComponent<ModularGunController>();
            if (ModularGunController) { ModularGunController.Start(); }
        }

        public override void Update()
        {
            base.Update();
            if (OnFrameUpdate != null && base.Owner)
            {
                OnFrameUpdate(this, base.Owner);
            }
            if (ModularGunController) 
            { ModularGunController.ProcessStats(); }
        }


        public void PostProcessBeamTick(BeamController b, SpeculativeRigidbody hitRigidbody ,float f)
        {
            if (base.Owner == null) { return; }
            if (OnPostProcessBeamTick != null && b != null && hitRigidbody != null) { OnPostProcessBeamTick(this, b,hitRigidbody ,f, base.Owner); }
        }
        public void PostProcessProjectile(Projectile p, float f)
        {
            if (base.Owner == null) { return; }
            if (OnPostProcessProjectile != null && p != null) { OnPostProcessProjectile(this, p, f, base.Owner); }
            p.specRigidbody.OnPreRigidbodyCollision += OPC;
        }
        public void OPC(SpeculativeRigidbody mR, PixelCollider mP, SpeculativeRigidbody oR, PixelCollider oP)
        {
            if (oR != null && oR.healthHaver != null && mR.projectile != null)
            {
                if (oR.healthHaver.aiActor)
                {
                    if (OnPreEnemyHit != null)
                    {
                        OnPreEnemyHit(this, this.Owner, oR.healthHaver.aiActor, mR.projectile);
                    }
                }
            }
        }

        public void OnDealtDamageContext(PlayerController p, float f, bool b, HealthHaver h)
        {
            if (OnDamagedEnemy != null && h.aiActor != null) { OnDamagedEnemy(this, p, h.aiActor, f); }
        }
        public void OnKilledEnemyContext(PlayerController p, HealthHaver h)
        {
            if (OnKilledEnemy != null && h.aiActor != null) { OnKilledEnemy(this, p, h.aiActor); }
        }
        public void OnReloadedGun(PlayerController p, Gun g)
        {
            if (OnGunReloaded != null) { OnGunReloaded(this, p, g); }
        }
        public void OnRecievedDamage(PlayerController p)
        {
            if (OnDamaged != null) { OnDamaged(this, p); }
        }
        public void PlayerEnteredCombat()
        {
            if (base.Owner == null) { return; }
            if (base.Owner.CurrentRoom == null) { return; }
            if (OnEnteredCombat == null) { return; }
            OnEnteredCombat(this, base.Owner.CurrentRoom, base.Owner);
        }
        //Many, MANY Actions 
        public Action<ModulePrinterCore, RoomHandler, PlayerController> OnEnteredCombat;
        public Action<ModulePrinterCore, PlayerController> OnDamaged;
        public Action<ModulePrinterCore, PlayerController, Gun> OnGunReloaded;
        public Action<ModulePrinterCore, PlayerController, AIActor> OnKilledEnemy;
        public Action<ModulePrinterCore, PlayerController, AIActor, float> OnDamagedEnemy;
        public Action<ModulePrinterCore, PlayerController> OnFrameUpdate;
        public Action<ModulePrinterCore, Projectile, float, PlayerController> OnPostProcessProjectile;
        public Action<ModulePrinterCore, BeamController, SpeculativeRigidbody,float ,PlayerController> OnPostProcessBeamTick;
        public Action<ModulePrinterCore, Projectile, PlayerController> DodgedProjectile;
        public Action<ModulePrinterCore, BeamController, PlayerController> DodgedBeam;
        public Action<ModulePrinterCore, PlayerController, Vector2> RollStarted;
        public Action<ModulePrinterCore, PlayerController, RoomHandler> OnRoomCleared;
        public Action<ModulePrinterCore, PlayerController> OnNewFloorStarted;
        public Action<ModulePrinterCore, PlayerController, DefaultModule> OnAnyModuleObtained;
        public Action<ModulePrinterCore, PlayerController, AIActor, Projectile> OnPreEnemyHit;

        public float StartingPower = 6;
        public Func<ModulePrinterCore, float> AdditionalPowerMods;

        public float ReturnTotalPower()
        {
            float c = StartingPower;
            if (AdditionalPowerMods != null)
            {
                c += AdditionalPowerMods(this);
            }
            foreach (PassiveItem item in this.Owner.passiveItems)
            {
                if (item is BasicStatPickup mastery)
                {
                    if (mastery.IsMasteryToken == true)
                    {
                        c++;                    
                    }
                }
                if (item.gameObject.GetComponent<AdditionalItemEnergyComponent>() != null)
                {
                    c += item.gameObject.GetComponent<AdditionalItemEnergyComponent>().AdditionalEnergy;
                }
            }
            if (ModularGunController != null) 
            {
                c += ModularGunController.AdditionalPowerSupply;
            }
            return c;
        }

        public float ReturnPowerConsumption()
        {
            float c = 0;
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    c += ReturnPowerConsumption(ModuleContainers[i].defaultModule);
                }
            }
            return c;
        }

        public float ReturnPowerConsumption(DefaultModule module)
        {
            float c = 0;
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    if (ModuleContainers[i].defaultModule.LabelName == module.LabelName)
                    {
                        if (ModuleContainers[i].defaultModule.powerConsumptionData != null)
                        {
                            if (ModuleContainers[i].defaultModule.powerConsumptionData.OverridePowerManagement != null)
                            {
                                c += ModuleContainers[i].defaultModule.powerConsumptionData.OverridePowerManagement(ModuleContainers[i].defaultModule);
                            }
                            else
                            {
                                c += ModuleContainers[i].defaultModule.powerConsumptionData.FirstStack + (ModuleContainers[i].defaultModule.powerConsumptionData.AdditionalStacks * (ModuleContainers[i].defaultModule.ActiveStack() - 1));
                            }

                        }
                        else
                        {
                            c += ModuleContainers[i].defaultModule.EnergyConsumption + ((ModuleContainers[i].defaultModule.EnergyConsumption * (ModuleContainers[i].defaultModule.ActiveStack() - 1)) / 2);
                        }
                    }           
                }
            }
            return c;
        }


        //UI Shit

        public void DepowerModule(DefaultModule self, int Amount_To_Remove = 1)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    if (ModuleContainers[i].LabelName == self.LabelName)
                    {
                        for (int A = 0; A < Amount_To_Remove; A++)
                        {
                            if (ModuleContainers[i].ActiveCount == 0) {return; }
                            ModuleContainers[i].ActiveCount--;
                            ModuleContainers[i].defaultModule.OnAnyRemoved(this, this.ModularGunController, base.Owner);
                            if (ModuleContainers[i].ActiveCount == 0) 
                            {
                                ModuleContainers[i].defaultModule.OnLastRemoved(this, this.ModularGunController, base.Owner);
                            }
                        }
                    }
                }
                else
                {
                    ModuleContainers.Remove(ModuleContainers[i]);
                }
            }
        }

        public void PowerModule(DefaultModule self, int Amount_To_Remove = 1)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    if (ModuleContainers[i].LabelName == self.LabelName)
                    {
                        for (int A = 0; A < Amount_To_Remove; A++)
                        {
                            if (ModuleContainers[i].ActiveCount == 0)
                            {
                                ModuleContainers[i].defaultModule.OnFirstPickup(this, this.ModularGunController, base.Owner);
                            }
                            ModuleContainers[i].ActiveCount++;
                            ModuleContainers[i].defaultModule.OnAnyPickup(this, this.ModularGunController, base.Owner, false);                 
                        }
                    }
                }
                else
                {
                    ModuleContainers.Remove(ModuleContainers[i]);
                }
            }
        }


        //General public variables that are useful
        public ModularGunController ModularGunController;
        public static int ModulePrinterCoreID;
        //Code related to module stacking and tracking
        public bool AddModule(DefaultModule self, PlayerController player)
        {
            if (OnAnyModuleObtained != null)
            {
                OnAnyModuleObtained(this, player, self);
            }
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i].LabelName == self.LabelName) 
                {
                    ModuleContainers[i].Count++;
                    //ModuleContainers[i].defaultModule.OnAnyPickup(this, this.ModularGunController, player, true);
                    return false;
                }
            }
            var modCont = new ModuleContainer()
            {
                LabelName = self.LabelName,
                tier = self.Tier,
                ID = self.PickupObjectId,
                Count = 1,
                defaultModule = self
            };
            modCont.defaultModule.Stored_Core = this;
            //modCont.defaultModule.OnAnyPickup(this, this.ModularGunController, player, true);
            ModuleContainers.Add(modCont);
            return true;
        }
        public int Scrap = 0;
        public int ReturnStack(string LabelName)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i].LabelName == LabelName)
                {
                    if (ModuleContainers[i].ActiveCount == 0) { return 0; }
                    return ModuleContainers[i].ActiveCount + ReturnFakeStack(LabelName);
                }
            }
            return 0;
        }
        public int ReturnTrueStack(string LabelName)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i].LabelName == LabelName)
                {
                    return ModuleContainers[i].Count;
                }
            }
            return 0;
        }
        public int ReturnActiveStack(string LabelName)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i].LabelName == LabelName)
                {
                    return ModuleContainers[i].ActiveCount;
                }
            }
            return 0;
        }
        public int ReturnActiveTotal()
        {
            int c = 0;
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                c += ModuleContainers[i].ActiveCount;
            }
            return 0;
        }

        public int ReturnFakeStack(string LabelName)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i].LabelName == LabelName)
                {
                    int count = 0;
                    for (int e = 0; e < ModuleContainers[i].FakeCount.Count; e++)
                    {
                        var T = ModuleContainers[i].FakeCount[e];
                        count += T.Second;
                    }
                    return count;
                }
            }
            return 0;
        }

        public void RemoveModule(DefaultModule self, int Amount_To_Remove = 1)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    if (ModuleContainers[i].LabelName == self.LabelName)
                    {
                        for (int A = 0; A < Amount_To_Remove; A++)
                        {
                            if (ModuleContainers[i].Count == 0) { ModuleContainers.Remove(ModuleContainers[i]); return; }
                            ModuleContainers[i].Count--;
                            ModuleContainers[i].defaultModule.OnAnyRemoved(this, this.ModularGunController, base.Owner);
                            if (ModuleContainers[i].Count == 0) { ModuleContainers[i].defaultModule.OnLastRemoved(this, this.ModularGunController, base.Owner); ModuleContainers.Remove(ModuleContainers[i]); }
                        }
                    }
                }
                else
                {
                    ModuleContainers.Remove(ModuleContainers[i]);
                }
            }
        }

        public void RemoveModule(int ID, int Amount_To_Remove = 1)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    if (ModuleContainers[i].LabelName == (PickupObjectDatabase.GetById(ID)as DefaultModule).LabelName)
                    {
                        for (int A = 0; A < Amount_To_Remove; A++)
                        {
                            if (ModuleContainers[i].Count == 0) { ModuleContainers.Remove(ModuleContainers[i]); return; }
                            ModuleContainers[i].Count--;
                            ModuleContainers[i].defaultModule.OnAnyRemoved(this, this.ModularGunController, base.Owner);
                            if (ModuleContainers[i].Count == 0) { ModuleContainers[i].defaultModule.OnLastRemoved(this, this.ModularGunController, base.Owner); ModuleContainers.Remove(ModuleContainers[i]); }
                        }
                    }
                }
                else
                {
                    ModuleContainers.Remove(ModuleContainers[i]);
                }
            }
        }


        public override void OnDestroy()
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                var cont = ModuleContainers[i];
                RemoveModule(cont.defaultModule, 999999);
                cont.defaultModule.OnCoreDestruction(this, this.ModularGunController);
            }
            base.OnDestroy();
        }



        public class ModuleContainer
        {
            public DefaultModule defaultModule;
            public string LabelName;
            public int ID;
            public DefaultModule.ModuleTier tier;
            public int Count;
            public List<Tuple<string, int>> FakeCount = new List<Tuple<string, int>>() { };
            public int ActiveCount = 0;
        }
        public class AdditionalItemEnergyComponent : MonoBehaviour
        {
            public int AdditionalEnergy = 1;
        }
        public List<ModuleContainer> ModuleContainers = new List<ModuleContainer>();
    }
}
