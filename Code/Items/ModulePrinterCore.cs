using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            player.OnReloadPressed += ReloadPressed;
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


        //UI Shit
        public void ReloadPressed(PlayerController p, Gun g)
        {
            if (g.ClipCapacity > g.ClipShotsRemaining) { return; }
            p.StartCoroutine(this.DoTick(p));
        }
        private IEnumerator DoTick(PlayerController user)
        {
            elapsed = 0;
            ReloadTapCount++;

            if (ReloadTapCount > 1 && extantLabel != null)
            {
                Running = false;
                ReloadTapCount = 0;
                DisplayModulesOfCertainTier(user, extantLabel.CurrentDisplayTier);
                yield break;
            }
            if (ReloadTapCount > 2)
            {
                Running = false;
                ReloadTapCount = 0;
                this.DisplayAllModules(user);
                AkSoundEngine.PostEvent("Play_BOSS_cyborg_eagle_01", user.gameObject);
                yield break;
            }
            if (Running == true) { yield break; }
            Running = true;

            while (elapsed < 0.5f)
            {
                if (Running == false)
                {yield break;}
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            if (extantLabel != null) { extantLabel.Inv(); }
            ReloadTapCount = 0;
            Running = false;
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", user.gameObject);
            yield break;
        }
        private bool Running = false;
        public void DisplayAllModules(PlayerController p)
        {
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);

            var ExtantLabelController = UnityEngine.Object.Instantiate(DefaultModule.LabelController).gameObject.GetComponent<ModifiedDefaultLabelManager>();
            ExtantLabelController.displayType = ModifiedDefaultLabelManager.DisplayType.All_Modules;

            string Text = "Modules Installed:";
            foreach (var entry in this.ModuleContainers) { Text += "\n" + entry.LabelName + " : " + entry.Count.ToString(); }
            Text += ModuleContainers.Count == 0 ? "\nNone." : "\nCycle through tiers by\npressing " + StaticColorHexes.AddColorToLabelString("Reload twice", StaticColorHexes.Light_Blue_Color_Hex) + ".";

            ExtantLabelController.label.Text = Text;
            ExtantLabelController.Trigger_CustomTime(p.transform, new Vector3(1.5f, 2), 0.5f);
            ExtantLabelController.label.backgroundColor = cl;

            GameUIRoot.Instance.m_manager.AddControl(ExtantLabelController.panel);
            dfLabel componentInChildren = ExtantLabelController.gameObject.GetComponentInChildren<dfLabel>();
            componentInChildren.ColorizeSymbols = false;
            componentInChildren.ProcessMarkup = true;

            extantLabel = ExtantLabelController;
        }
        public void DisplayModulesOfCertainTier(PlayerController p, int CycleTier)
        {
            if (extantLabel) { extantLabel.Inv(); }

            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);

            var ExtantLabelController = Toolbox.GenerateText(p.transform, new Vector2(1.5f, 2), 0.5f, "", cl, false);
            ExtantLabelController.CurrentDisplayTier = CycleTier;
            var currentCycle = ExtantLabelController.CurrentDisplayTier;
            ExtantLabelController.displayType = ModifiedDefaultLabelManager.DisplayType.Specific_Module_Tiers;

            bool HasContainers = false;
            int c = 0;
            while (HasContainers == false)
            {
                c++;
                currentCycle = ExtantLabelController.CycleTier();
                if (c > 7)
                {

                    ExtantLabelController.label.Text = StaticColorHexes.AddColorToLabelString("No Modules available.", StaticColorHexes.Red_Color_Hex);
                    ExtantLabelController.Trigger_CustomTime(p.transform, new Vector3(1.5f, 2), 0.5f);
                    extantLabel = ExtantLabelController;
                    return;
                }

                foreach (var entry in this.ModuleContainers)
                {
                    if (entry.tier == (DefaultModule.ModuleTier)currentCycle) 
                    {
                        HasContainers = true;
                        break;
                    }
                }           
            }
                     
            string Text = "Modules Installed: " + DefaultModule.ReturnTierLabel((DefaultModule.ModuleTier)currentCycle);
            foreach (var entry in this.ModuleContainers) { if (entry.tier == (DefaultModule.ModuleTier)currentCycle) Text += "\n" + entry.LabelName + " : " + entry.Count.ToString(); }
            Text += ModuleContainers.Count == 0 ? "\nNone." : "\nCycle through tiers by\npressing "+ StaticColorHexes.AddColorToLabelString("Reload twice", StaticColorHexes.Light_Blue_Color_Hex) + ".";
            ExtantLabelController.label.Text = Text;
            ExtantLabelController.Trigger_CustomTime(p.transform, new Vector3(0, 2), 0.5f);
            extantLabel = ExtantLabelController;
            

        }
        public ModifiedDefaultLabelManager extantLabel;
        private int ReloadTapCount = 0;
        private float elapsed = 0;


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
                    ModuleContainers[i].defaultModule.OnAnyPickup(this, this.ModularGunController, player, true);
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
            modCont.defaultModule.OnAnyPickup(this, this.ModularGunController, player, true);
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
                    return ModuleContainers[i].Count + ReturnFakeStack(LabelName);
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
        }
        public List<ModuleContainer> ModuleContainers = new List<ModuleContainer>();
    }
    public class TestModuleProper4 : DefaultModule
    {
        public static ItemTemplate template = new ItemTemplate(typeof(TestModuleProper4))
        {
            Name = "TrueTestModule4",
            Description = "TrueTest",
            LongDescription = "TestTest.",
            SpriteResource = "ModularMod/Sprites/Icons/tier_label_4",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = PostInit
        };
        public static void PostInit(PickupObject v)
        {
            var h = (v as DefaultModule);
            h.Tier = ModuleTier.Tier_Omega;
            h.LabelName = "Damage Module" + h.ReturnTierLabel();
            h.LabelDescription = "Increases Damage by\n20% (+20% per stack)";
            h.AddToGlobalStorage();
            h.SetTag("modular_module");
            h.AddColorLight(Color.red);
            h.Label_Background_Color = new Color32(240, 10, 10, 100);
            //EncounterDatabase.GetEntry(h.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
        }
        public override void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {
            modulePrinter.OnPostProcessProjectile += PPP;
        }

        public void PPP(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {
            int stack = this.ReturnStack(modulePrinterCore);
            p.baseData.damage *= 1 + (0.2f * stack);
        }
    }
}
