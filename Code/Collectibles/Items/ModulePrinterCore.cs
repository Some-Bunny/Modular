﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Alexandria.CharacterAPI;
using Alexandria.ItemAPI;
using Brave.BulletScript;
using Dungeonator;
using HutongGames.PlayMaker.Actions;
using JuneLib.Items;
using MonoMod.RuntimeDetour;
using Planetside;
using UnityEngine;
using static Alexandria.Misc.CustomActions;
using static UnityEngine.UI.CanvasScaler;

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
            string longDesc = "The Heart of the Modular. Controls all hardware and software upgrades.\n\nTechnology originally purposed for keeping track of various non-lethal equipment, now used in machines of war.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "mdl");

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.IgnoredByRat = true;
            item.RespawnsIfPitfall = true;
            item.UsesCustomCost = true;
            item.CustomCost = 25;
            item.CanBeDropped = false;
            
            ModulePrinterCore.ModulePrinterCoreID = item.PickupObjectId;
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;



            
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Additional Sight", "lichs_eye_bullets") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Power Allocation", "unity") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Chaos, Chaos!", "chance_bullets") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Wish I could have one of those!", "magazine_rack") { });

            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Power Up", "heart_holster") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Power Up", "heart_lunchbox") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Power Up", "heart_locket") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Power Up", "heart_bottle") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Power Up", "heart_purse") { });

            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Poison Power", "monster_blood") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Power Internal", "yellow_chamber") { });
            BasicCustomSynergyNotifier.synergizing_Items.Add(new BasicCustomSynergyNotifier.ModularSynergy("Money = Power", "ring_of_miserly_protection") { });

            //CustomSynergies.Add("Additional Sight", new List<string> {"mdl:modular_printer_core" }, new List<string> {"lichs_eye_bullets" }, true);
            //CustomSynergies.Add("Power Allocation", new List<string> { "mdl:modular_printer_core" }, new List<string> { "unity" }, true);
            //CustomSynergies.Add("Chaos, Chaos!", new List<string> { "mdl:modular_printer_core" }, new List<string> { "chance_bullets" }, true);
            //CustomSynergies.Add("Wish I could have one of those!", new List<string> { "mdl:modular_printer_core" }, new List<string> { "magazine_rack" }, true);

            //CustomSynergies.Add("Poison Power", new List<string> { "mdl:modular_printer_core" }, new List<string> { "monster_blood" }, true);
            //CustomSynergies.Add("Power Internal", new List<string> { "mdl:modular_printer_core" }, new List<string> { "yellow_chamber" }, true);
            //CustomSynergies.Add("Money = Power", new List<string> { "mdl:modular_printer_core" }, new List<string> { "ring_of_miserly_protection" }, true);

            //CustomSynergies.Add("Quick Draw", new List<string> { "mdl:modular_printer_core" }, new List<string> { "heart_holster" }, true);



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
            player.OnAboutToFall += OnAboutToFall;
            player.OnTableFlipped += OnTableFlipped;
            player.OnTableFlipCompleted += OnTableFlipCompletely;

            cloakDoer = ScriptableObject.CreateInstance<CloakDoer>();
            cloakDoer.DoStartUp(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnEnteredCombat -= PlayerEnteredCombat;
            player.OnReceivedDamage -= OnRecievedDamage;
            player.OnReloadedGun -= OnReloadedGun;
            player.OnKilledEnemyContext -= OnKilledEnemyContext;
            player.OnDealtDamageContext -= OnDealtDamageContext;
            player.PostProcessProjectile -= PostProcessProjectile;
            player.PostProcessBeamTick -= PostProcessBeamTick;
            player.OnDodgedProjectile -= OnDodgedProjectile;
            player.OnDodgedBeam -= OnDodgedBeam;
            player.OnRollStarted -= OnRollStarted;
            player.OnRoomClearEvent -= OnRoomClearEvent;
            player.GunChanged -= Player_GunChanged;
            player.OnNewFloorLoaded -= NewFloorLoaded;
            player.OnAboutToFall -= OnAboutToFall;
            player.OnTableFlipped -= OnTableFlipped;
            player.OnTableFlipCompleted -= OnTableFlipCompletely;
            return base.Drop(player);
        }

        public void OnTableFlipCompletely(FlippableCover table)
        {
            if (TableFlipCompleted != null) { TableFlipCompleted(this, this.Owner, table); }
        }
        public void OnTableFlipped(FlippableCover table)
        {
            if (TableFlipped != null) { TableFlipped(this, this.Owner, table); }
        }

        public bool OnAboutToFall(bool b)
        {
            if (OnAboutToFallContext != null) { b = OnAboutToFallContext(this, this.Owner); }
            return !b;
        }

        private bool b = false;
        public void NewFloorLoaded(PlayerController player)
        {
            b = !b;
            if (b == false) { return; }
            if (player && OnNewFloorStarted != null) { OnNewFloorStarted(this, player); }
        }

        public bool TemporaryDisableDrop = false;


        public static Color ColorToUse(PlayerController p)
        {
            return p.IsUsingAlternateCostume ? green_Color : cyan_Color;
        }

        private static Color32 cyan_Color = new Color32(121, 234, 255, 100);
        private static Color32 green_Color = new Color32(0, 255, 54, 100);

        //Code to check for valid guns, and discards any guns considered invalid
        private void Player_GunChanged(Gun oldGun, Gun newGun, bool isNewGun)
        {
            if (TemporaryDisableDrop == true) { return; }
            if (newGun.GetComponent<ModularGunController>() != null) { UpdateModularGunController(); return; }
            if (newGun.GetComponent<ModularGunController>() == ModularGunController) { UpdateModularGunController();  return; }
            if (newGun.PickupObjectId == 251 || newGun.HasTag("modular_special_override"))
            {
                if (isNewGun == true)
                {
                    var t = Toolbox.GenerateText(base.Owner.transform, new Vector2(1.5f, 0.5f), 0.5f, "Gun Override Detected :\nWeapon Will Not Be Dropped.", ColorToUse(Owner));
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
                var t = Toolbox.GenerateText(base.Owner.transform, new Vector2(1.5f, 0.5f), 0.5f, "Gun Override Detected :\nWeapon Will Not Be Dropped.", ColorToUse(Owner));
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
            if (this == null) { return; }
            if (base.Owner == null) { return; }
            if (base.Owner.CurrentGun == null) { return; }
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

            LastPower_Tick = ReturnPowerConsumption();
            LastTotal_Tick = ReturnTotalPower();

            if (LastPower_Tick > LastTotal_Tick)
            {
                if (OnPowerUsageHigherThanCapacity != null) { OnPowerUsageHigherThanCapacity(this, Owner); }
                DepoweredModuelNames = new Dictionary<string, int>();
                while (LastPower_Tick > LastTotal_Tick) 
                {
                    List<ModuleContainer> H = new List<ModuleContainer>();
                    H.AddRange(ModuleContainers.Where(self => self.ActiveCount > 0));
                    H.Shuffle();

                    for (int i = 0; i < H.Count; i++)
                    {
                        var defModule = H[i].defaultModule;
                        DepowerModule(defModule);
                        if (!DepoweredModuelNames.ContainsKey(defModule.LabelName)) { DepoweredModuelNames.Add(defModule.LabelName, 1); } else { DepoweredModuelNames[defModule.LabelName]++; }
                        LastPower_Tick = ReturnPowerConsumption();

                        if (LastPower_Tick == LastTotal_Tick || LastPower_Tick < LastTotal_Tick)
                        {
                            string t = "";
                            foreach (var entry in DepoweredModuelNames)
                            {
                                t += "\n" + entry.Key + " : " + StaticColorHexes.AddColorToLabelString(entry.Value.ToString(), StaticColorHexes.Orange_Hex);
                            }
                            AkSoundEngine.PostEvent("Play_ENM_hammer_target_01", Owner.gameObject);
                            var label = Toolbox.GenerateText(Owner.transform, new Vector2(2, 2), 0.5f, StaticColorHexes.AddColorToLabelString("! WARNING !", StaticColorHexes.Red_Color_Hex) + "\nPOWER USAGE EXCEEDED LIMIT.\nDepowered:" + t, ColorToUse(Owner), true, 4);
                            label.Trigger_CustomTime(Owner.transform, new Vector2(2, 2), 0.5f, 4);
                            return;
                        }
                    }
                }
            }
        }
        private float LastPower_Tick, LastTotal_Tick;
        private Dictionary<string, int> DepoweredModuelNames = new Dictionary<string, int>();

        public Action<ModulePrinterCore, PlayerController> OnPowerUsageHigherThanCapacity;

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
            if (OnProjectileStickAction != null)
            {
                var modifier = p.gameObject.AddComponent<StickyProjectileModifier>();
                modifier.OnStick = OnProjectileStickAction;
                
                if (OnStickyDestroyAction != null)
                {
                    modifier.OnStickyDestroyed = OnStickyDestroyAction;
                }
                if (OnPreProjectileStickAction != null)
                {
                    modifier.OnPreStick = OnPreProjectileStickAction;
                }
                modifier.stickyContexts = this.stickyContexts;
            }
            if (this.Owner.HasPickupID(521))
            {
                if (UnityEngine.Random.value < 0.3f) 
                {
                    var bounce = p.gameObject.GetOrAddComponent<BounceProjModifier>();
                    bounce.ExplodeOnEnemyBounce = UnityEngine.Random.value < 0.1f ? true : false;
                    bounce.damageMultiplierOnBounce += UnityEngine.Random.Range(0.75f, 1.25f);
                    bounce.chanceToDieOnBounce += UnityEngine.Random.Range(0.00f, 0.2f);
                    bounce.bouncesTrackEnemies = UnityEngine.Random.value < 0.2f ? true : false;
                    bounce.bounceTrackRadius += UnityEngine.Random.Range(1f, 25f);
                }
                if (UnityEngine.Random.value < 0.3f)
                {
                    var pierce = p.gameObject.GetOrAddComponent<PierceProjModifier>();
                    pierce.penetration += UnityEngine.Random.Range(1, 5);
                    if (UnityEngine.Random.value < 0.3f)
                    {
                        var pierceMain = p.gameObject.GetOrAddComponent<MaintainDamageOnPierce>();
                        pierceMain.damageMultOnPierce *= UnityEngine.Random.Range(1.01f, 1.5f);
                    }
                }
                if (UnityEngine.Random.value < 0.15f)
                {
                    var homing = p.gameObject.GetOrAddComponent<HomingModifier>();
                    homing.AngularVelocity += UnityEngine.Random.Range(60f, 1080f);
                    homing.HomingRadius += UnityEngine.Random.Range(1f, 25f);
                }

                p.AppliesPoison = true;
                p.PoisonApplyChance = UnityEngine.Random.Range(0.01f, 1f);
                p.healthEffect = DebuffStatics.irradiatedLeadEffect;

                p.AppliesFire = true;
                p.FireApplyChance = UnityEngine.Random.Range(0.1f, 1f);
                p.fireEffect = UnityEngine.Random.value < 0.2 ? DebuffStatics.greenFireEffect : DebuffStatics.hotLeadEffect;

                p.AppliesFreeze = true;
                p.FreezeApplyChance = UnityEngine.Random.Range(0.1f, 1f);
                p.freezeEffect = DebuffStatics.frostBulletsEffect;

                p.AppliesCheese = true;
                p.CheeseApplyChance = UnityEngine.Random.Range(0.02f, 0.5f);
                p.cheeseEffect = DebuffStatics.cheeseeffect;

                p.AppliesCharm = true;
                p.CharmApplyChance = UnityEngine.Random.Range(0.02f, 0.5f);
                p.charmEffect = DebuffStatics.charmingRoundsEffect;

                p.CanTransmogrify = true;
                p.ChanceToTransmogrify = 0.005f;

                p.AdjustPlayerProjectileTint(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)), 10);
                if (ModifyForChanceBullets != null) { ModifyForChanceBullets(this, p, f, Owner); }
            }
        }

        public static Action<ModulePrinterCore, Projectile, float, PlayerController> ModifyForChanceBullets;

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
        public Action<ModulePrinterCore, PlayerController, FlippableCover> TableFlipped;
        public Action<ModulePrinterCore, PlayerController, FlippableCover> TableFlipCompleted;
        public Func<ModulePrinterCore, PlayerController, bool> OnAboutToFallContext;

        public float StartingPower = 5;
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


        public float ReturnTotalPowerMasteryless()
        {
            float c = StartingPower;
            if (AdditionalPowerMods != null)
            {
                c += AdditionalPowerMods(this);
            }
            foreach (PassiveItem item in this.Owner.passiveItems)
            {
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

        public float ReturnRemainingPower()
        {
            float c = 0;
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    c += ReturnPowerConsumption(ModuleContainers[i].defaultModule);
                }
            }
            return ReturnTotalPower() - c;
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
                                c += ModuleContainers[i].defaultModule.powerConsumptionData.OverridePowerManagement(ModuleContainers[i].defaultModule, ReturnActiveStack(ModuleContainers[i].defaultModule.LabelName));
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
        public float ReturnPowerConsumptionOfNextStack(DefaultModule module, int stacksToIncrement = 1)
        {
            float c = 0;
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    var defModule = ModuleContainers[i].defaultModule;
                    bool asfas = ModuleContainers[i].defaultModule.LabelName == module.LabelName;
                    if (asfas == true)
                    {
                        if (ModuleContainers[i].defaultModule.powerConsumptionData != null)
                        {
                            if (defModule.powerConsumptionData.OverridePowerManagement != null)
                            {
                                c += defModule.powerConsumptionData.OverridePowerManagement(defModule, ReturnActiveStack(defModule.LabelName) == 0 ? 1 : ReturnActiveStack(defModule.LabelName) + stacksToIncrement);
                            }
                            else
                            {
                                c += ReturnActiveStack(defModule.LabelName) == 0 ? defModule.powerConsumptionData.FirstStack : defModule.powerConsumptionData.FirstStack + (defModule.powerConsumptionData.AdditionalStacks * ((defModule.ActiveStack() - 1) + stacksToIncrement));
                            }
                        }
                        else
                        {
                            c += ReturnActiveStack(defModule.LabelName) == 0 ? defModule.EnergyConsumption : defModule.EnergyConsumption + ((defModule.EnergyConsumption * ((defModule.ActiveStack() - 1) + stacksToIncrement)));
                        }
                    }
                    else
                    {
                        c += ReturnPowerConsumption(ModuleContainers[i].defaultModule);
                    }
                }
            }
            return c;

        }
        public void DepowerModule(DefaultModule self, int Amount_To_Remove = 1)
        {
            for (int i = 0; i < ModuleContainers.Count; i++)
            {
                if (ModuleContainers[i] != null)
                {
                    if (ModuleContainers[i].LabelName == self.LabelName)
                    {
                        if (OnAnyModuleUnpowered != null) { OnAnyModuleUnpowered(this, self, Amount_To_Remove); }
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
                        if (OnAnyModulePowered != null) { OnAnyModulePowered(this, self, Amount_To_Remove); }
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

        public Action<ModulePrinterCore, DefaultModule, int> OnAnyModulePowered;
        public Action<ModulePrinterCore, DefaultModule, int> OnAnyModuleUnpowered;


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
            return c;
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
                            if (ModuleContainers[i].Count == 1) { ModuleContainers.Remove(ModuleContainers[i]); return; }
                            ModuleContainers[i].Count--;
                            ModuleContainers[i].defaultModule.OnAnyRemoved(this, this.ModularGunController, base.Owner);
                            if (ModuleContainers[i].Count == 1) { ModuleContainers[i].defaultModule.OnLastRemoved(this, this.ModularGunController, base.Owner); ModuleContainers.Remove(ModuleContainers[i]); }
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
            for (int i = ModuleContainers.Count - 1; i > -1; i--)
            {
                var cont = ModuleContainers[i];
                RemoveModule(cont.defaultModule, 999999);
                cont.defaultModule.OnCoreDestruction(this, this.ModularGunController);
            }
            base.OnDestroy();
        }
        public Action<GameObject, PlayerController> OnPreProjectileStickAction;
        public Action<GameObject, StickyProjectileModifier, tk2dBaseSprite, PlayerController> OnProjectileStickAction;
        public Action<GameObject, StickyProjectileModifier, PlayerController> OnStickyDestroyAction;
        public List<StickyProjectileModifier.StickyContext> stickyContexts = new List<StickyProjectileModifier.StickyContext>();


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
        public CloakDoer cloakDoer;
        public List<ModuleContainer> ModuleContainers = new List<ModuleContainer>();
    }

    public class CloakDoer : ScriptableObject
    {
        private PlayerController player;
        public void DoStartUp(PlayerController p)
        {
            player = p;
        }
        public List<CloakContext> cloakContexts = new List<CloakContext>();
        public void ProcessCloak(CloakContext context)
        {
            cloakContexts.Add(context);
            Cloak_Time += context.Length;
            if (cloakContexts.Count == 1)
            {
                StartCloak();
            }
            foreach (var cloakCs in cloakContexts)
            {
                if (cloakCs.OnEnteredCloak != null)
                {
                    cloakCs.OnEnteredCloak(player);
                    if (cloakCs.Retrigger_Enter == false) { cloakCs.OnEnteredCloak = null; }
                }
            }
        }
        public void StartCloak()
        {
            player.StartCoroutine(HandleStealth(player));
        }


        private float Cloak_Time = 0;

        private IEnumerator HandleStealth(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_cloak", user.gameObject);
            Hooks.Stencility_Enabled = false;
            float elapsed = 0f;
            user.sprite.usesOverrideMaterial = true;
            user.sprite.renderer.material.shader = StaticShaders.TransparencyShader;
            user.sprite.renderer.material.SetFloat("_Fade", 1f);
            for (int i = 0; i < user.healthHaver.bodySprites.Count; i++)
            {
                user.healthHaver.bodySprites[i].usesOverrideMaterial = true;
                user.healthHaver.bodySprites[i].renderer.material.shader = StaticShaders.TransparencyShader;
                user.healthHaver.bodySprites[i].renderer.material.SetFloat("_Fade", 1f);
            }
            if (user.primaryHand && user.primaryHand.sprite)
            {
                user.primaryHand.sprite.usesOverrideMaterial = true;
                user.primaryHand.sprite.renderer.material.shader = StaticShaders.TransparencyShader;
                user.primaryHand.sprite.renderer.material.SetFloat("_Fade", 1f);
            }
            if (user.secondaryHand && user.secondaryHand.sprite)
            {
                user.secondaryHand.sprite.usesOverrideMaterial = true;
                user.secondaryHand.sprite.renderer.material.shader = StaticShaders.TransparencyShader;
                user.secondaryHand.sprite.renderer.material.SetFloat("_Fade", 1f);
            }
            currentState = Cloak_State.Active;

            //EnteredCloak = null;
            user.SetIsStealthed(true, "cloak");
            user.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            user.OnDidUnstealthyAction += this.BreakStealth;
            while (elapsed < Cloak_Time)
            {
                if (currentState != Cloak_State.Active) { yield break; }
                AlterShader(user, Mathf.Max(0.2f, Mathf.Lerp(1, 0, elapsed)));
                elapsed += BraveTime.DeltaTime;
                if (!user.IsStealthed)
                {
                    break;
                }
                yield return null;
            }
            user.OnDidUnstealthyAction -= this.BreakStealth;
            user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            user.SetIsStealthed(false, "cloak");
            user.StartCoroutine(BreakStealth(false));
            yield break;
        }

        private void AlterShader(PlayerController user, float f)
        {
            if (user.sprite.renderer.material.shader != StaticShaders.TransparencyShader) { return; }
            user.sprite.renderer.material.SetFloat("_Fade", f);
            for (int i = 0; i < user.healthHaver.bodySprites.Count; i++)
            {
                user.healthHaver.bodySprites[i].renderer.material.SetFloat("_Fade", f);
            }
            if (user.primaryHand && user.primaryHand.sprite)
            {
                user.primaryHand.sprite.renderer.material.SetFloat("_Fade", f);
            }
            if (user.secondaryHand && user.secondaryHand.sprite)
            {
                user.secondaryHand.sprite.renderer.material.SetFloat("_Fade", f);
            }
        }


        private IEnumerator BreakStealth(bool isForced)
        {
            currentState = Cloak_State.Disabling;

            foreach (var cloakCs in cloakContexts)
            {
                if (cloakCs.OnCloakBroken != null)
                {
                    cloakCs.OnCloakBroken(player);
                    if (cloakCs.Retrigger_Cloak_Break == false) { cloakCs.OnCloakBroken = null; }
                }
                if (isForced == true)
                {
                    if (cloakCs.OnForceCloakBroken != null)
                    {
                        cloakCs.OnForceCloakBroken(player);
                        if (cloakCs.Retrigger_Force_Cloak_Break == false) { cloakCs.OnForceCloakBroken = null; }
                    }
                }
            }

            //ForceCloakBroken = null;
            cloakContexts.Clear();
            Cloak_Time = 0;
            float elapsed = 0f;
            while (elapsed < 1.5f)
            {
                elapsed += BraveTime.DeltaTime;
                if (currentState == Cloak_State.Active) { yield break; }
                AlterShader(player, Mathf.Lerp(0.2f, 1, elapsed / 1.5f));
                yield return null;
            }
            player.sprite.renderer.material = player.IsUsingAlternateCostume == true ? player.gameObject.GetComponent<CustomCharacter>().data.altGlowMaterial : player.gameObject.GetComponent<CustomCharacter>().data.glowMaterial;
            Hooks.Stencility_Enabled = true;
            currentState = Cloak_State.Inactive;
            yield break;
        }

        public Cloak_State currentState = Cloak_State.Inactive;
        public enum Cloak_State
        {
            Inactive,
            Active,
            Disabling
        }

        private void BreakStealth(PlayerController p)
        {
            player.StartCoroutine(BreakStealth(true));
            player.OnDidUnstealthyAction -= this.BreakStealth;
            player.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            player.SetIsStealthed(false, "cloak");
        }

        public class CloakContext
        {
            public string Reason = "idk";
            public Action<PlayerController> OnEnteredCloak;
            public bool Retrigger_Enter = false;

            public Action<PlayerController> OnCloakBroken;
            public bool Retrigger_Cloak_Break = false;

            public Action<PlayerController> OnForceCloakBroken;
            public bool Retrigger_Force_Cloak_Break = false;

            public float Length = 1;
        }
    }
}