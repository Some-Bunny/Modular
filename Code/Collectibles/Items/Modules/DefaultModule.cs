using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModularMod
{
    public class DefaultModule : PickupObject, IPlayerInteractable
    {
        public static void DoQuickSetup()
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DefaultLabelPanel", ".prefab"));
            FakePrefab.MakeFakePrefab(gameObject);
            DontDestroyOnLoad(gameObject);

            var dlm = gameObject.GetComponent<DefaultLabelController>();

            var mdlm = gameObject.AddComponent<ModifiedDefaultLabelManager>();
            mdlm.label = dlm.label;
            mdlm.m_cachedCache = dlm.m_cachedCache;
            mdlm.m_manager = dlm.m_manager;
            mdlm.offset = dlm.offset;
            mdlm.panel = dlm.panel;
            mdlm.targetObject = dlm.targetObject;
            Destroy(dlm);

            var label = mdlm.label;
            label.backgroundColor = new Color32(121, 234, 255, 100);
            label.colorizeSymbols = false;
            //label.outlineWidth = 2;

            //label.outlineColor = new Color32(20, 20, 50, 255);
            //label.outline = true;
            label.textScale = 5;
            label.text = "AAAAAAAAAAAAAAAAAAAAAAAAA||||||||||||||||||||||||||||||||||";
            label.Invalidate();
            //label.textScaleMode = dfTextScaleMode.None;

            label.shadow = true;
            label.shadowColor = new Color32(20, 20, 50, 255);
            label.shadowOffset = new Vector2(0, -0.75f);
            //label.padding = new RectOffset(1, 1, 1, 1);
            //label.Height = 1000;

            label.anchorStyle = dfAnchorStyle.Top | dfAnchorStyle.Left;

            label.autoSize = true;
            label.autoHeight = false;
            label.Height = 160;
            label.wordWrap = false;

            var data = new dfRenderData()
            {
                Glitchy = false,
                Shader = ShaderCache.Acquire("Brave/Internal/HologramShader"),
                
            };
            label.renderData = data;
            //mdlm.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HologramShader");




            //label.backgroundSprite = AtlasEditors.AddUITextImage("ModularMod/Sprites/modular_hud_grid.png", "modular_hud_grid");

            LabelController = mdlm;

            Tier_1_Label = AtlasEditors.AddUITextImage("ModularMod/Sprites/Icons/tier_label_1.png", "tier_1");
            Tier_2_Label = AtlasEditors.AddUITextImage("ModularMod/Sprites/Icons/tier_label_2.png", "tier_2");
            Tier_3_Label = AtlasEditors.AddUITextImage("ModularMod/Sprites/Icons/tier_label_3.png", "tier_3");
            Tier_Omega_Label = AtlasEditors.AddUITextImage("ModularMod/Sprites/Icons/tier_label_4.png", "tier_omega");

            GameObject roomIcon = new GameObject("VFX");
            FakePrefab.DontDestroyOnLoad(roomIcon);
            FakePrefab.MarkAsFakePrefab(roomIcon);
            var tk2d_2 = roomIcon.AddComponent<tk2dSprite>();
            tk2d_2.Collection = StaticCollections.Item_Collection;
            tk2d_2.SetSprite(StaticCollections.Item_Collection.GetSpriteIdByName("t1_a_roomicon"));
            minimapIcon = roomIcon;
        }

        public static GameObject minimapIcon;
        private GameObject extant_minimapIcon;
        private RoomHandler m_minimapIconRoom;

        public static string Tier_1_Label;
        public static string Tier_2_Label;
        public static string Tier_3_Label;
        public static string Tier_Omega_Label;

        public bool IsSpecialModule = false;
        private string ReturnRoomIconSpriteName(bool alt)
        {
            switch (this.Tier)
            {
                case ModuleTier.Tier_1:
                    return alt ? "t1_b_roomicon" : "t1_a_roomicon";
                case ModuleTier.Tier_2:
                    return alt ? "t2_b_roomicon" : "t2_a_roomicon";
                case ModuleTier.Tier_3:
                    return alt ? "t3_b_roomicon" : "t3_a_roomicon";
                case ModuleTier.Tier_Omega:
                    return "t4_roomicon";
                default: return "t1_a_roomicon";
            }
        }

        public string ReturnTierLabel()
        {
            switch (this.Tier)
            {
                case ModuleTier.Tier_1:
                    return "[sprite \"" + Tier_1_Label + "\"]";
                case ModuleTier.Tier_2:
                    return "[sprite \"" + Tier_2_Label + "\"]"; ;
                case ModuleTier.Tier_3:
                    return "[sprite \"" + Tier_3_Label + "\"]"; ;
                case ModuleTier.Tier_Omega:
                    return "[sprite \"" + Tier_Omega_Label + "\"]";
                default: return "[sprite \"" + Tier_1_Label + "\"]";
            }
        }

        public static string ReturnTierLabel(ModuleTier moduleTier)
        {
            switch (moduleTier)
            {
                case ModuleTier.Tier_1:
                    return "[sprite \"" + Tier_1_Label + "\"]";
                case ModuleTier.Tier_2:
                    return "[sprite \"" + Tier_2_Label + "\"]";
                case ModuleTier.Tier_3:
                    return "[sprite \"" + Tier_3_Label + "\"]";
                case ModuleTier.Tier_Omega:
                    return "[sprite \"" + Tier_Omega_Label + "\"]";
                default: return "[sprite \"" + Tier_1_Label + "\"]"; ;
            }
        }


        public override void Pickup(PlayerController player)
        {
            for(int c = 0; c < player.passiveItems.Count; c++)
            {
                var entry = player.passiveItems[c];
                if (entry is ModulePrinterCore printerCore)
                {
                    printerCore.UpdateModularGunController();
                    bool isFirst = printerCore.AddModule(this, player);
                    if (isFirst == true)
                    {
                        GameStatsManager.Instance.HandleEncounteredObject(this.encounterTrackable);
                        OnFirstEverObtainedNonActivation(printerCore, printerCore.ModularGunController, player);
                    }
                    OnAnyEverObtainedNonActivation(printerCore, printerCore.ModularGunController, player);
                }
            }
            this.GetRidOfMinimapIcon();
            AkSoundEngine.PostEvent("Play_ClickIntoPlace", player.gameObject);
            Toolbox.NotifyCustom("Added Module:", this.LabelName, this.sprite.spriteId, this.sprite.collection, UINotificationController.NotificationColor.PURPLE);
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public int? OverrideScrapCost;
        public bool IsUncraftable = false;

        public virtual void OnFirstEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {

        }
        public virtual void OnAnyEverObtainedNonActivation(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {

        }

        public virtual void OnFirstPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {

        }
        public virtual void OnAnyPickup(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player, bool IsTruePickup)
        {

        }
        public virtual void OnAnyRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {

        }
        public virtual void OnLastRemoved(ModulePrinterCore modulePrinter, ModularGunController modularGunController, PlayerController player)
        {

        }
        public virtual void OnCoreDestruction(ModulePrinterCore modulePrinter, ModularGunController modularGunController)
        {

        }
        public virtual bool CanBeEnabled(ModulePrinterCore modulePrinter, ModularGunController modularGunController)
        {
            return true;
        }
        public virtual bool CanBeDisabled(ModulePrinterCore modulePrinter, ModularGunController modularGunController)
        {
            return true;
        }
        public virtual void ChanceBulletsModify(ModulePrinterCore modulePrinterCore, Projectile p, float f, PlayerController player)
        {

        }
        public int Stack(bool usesTemporaryStack = true)
        {
            return this.ReturnStack(Stored_Core, usesTemporaryStack);
        }
        public int TrueStack()
        {
            return Stored_Core.ReturnTrueStack(this.LabelName);
        }
        public int ActiveStack()
        {
            return Stored_Core.ReturnActiveStack(this.LabelName);
        }


        public ModulePrinterCore Stored_Core;

        public int ReturnStack(ModulePrinterCore modulePrinterCore, bool useTemporaryStack = true)
        {
            return modulePrinterCore.ReturnStack(this.LabelName, useTemporaryStack);
        }


        public void ChangeShader(Shader s = null)
        {
            this.sprite.renderer.material.shader = s;
        }
        public bool ModularIsAltSkin()
        {
            foreach (PlayerController p in GameManager.Instance.AllPlayers)
            {
                if (p.HasPassiveItem(ModulePrinterCore.ModulePrinterCoreID) && p.IsUsingAlternateCostume == true) 
                {
                    return true; 
                }
            }
            return false;
        }
        protected void Start()
        {
            if (minimapIcon != null)
            {
                this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
                this.extant_minimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, minimapIcon, false);
                extant_minimapIcon.GetComponent<tk2dBaseSprite>().SetSprite(StaticCollections.Item_Collection, ReturnRoomIconSpriteName(ModularIsAltSkin()));
            }
            if (AltSpriteID != -69 && ModularIsAltSkin() == true) 
            {
                this.sprite.SetSprite(AltSpriteID);
            }
            this.sprite.usesOverrideMaterial = true;
            try
            {
                var rH = this.transform.position.GetAbsoluteRoom();
                if (rH != null) 
                { 
                    rH.RegisterInteractable(this);
                }
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(er.Message, false);
            }
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 2f;
        }
        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void UpdateOnDrop(PlayerController p, bool UpdateUI = false)
        {
            if (OnPostDrop != null)
            {
                UpdateUI = OnPostDrop(this);
            }
            if (UpdateUI == true)
            {
                if (ExtantLabelController != null)
                { UnityEngine.Object.Destroy(ExtantLabelController.gameObject); }

                if (ExtantNameLabelController != null)
                { UnityEngine.Object.Destroy(ExtantNameLabelController.gameObject); }
            }
            OnEnteredRange(p);
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            Color colorToSelect = Label_Background_Color_Override != Color.clear ? Label_Background_Color_Override : (interactor.IsUsingAlternateCostume == true ? Label_Background_Color_Alt : Label_Background_Color);
            if (ExtantLabelController == null && CanDisplayText == true)
            {
                ExtantLabelController = Toolbox.GenerateText(this.transform, Offset_LabelDescription, 0.5f, GetLabelNameDescrption(), colorToSelect, true, 5, false);
            }
            if (ExtantNameLabelController == null && CanDisplayText == true)
            {
                ExtantNameLabelController = Toolbox.GenerateText(this.transform, Offset_LabelName, 0.5f, GetLabelNameProper(), colorToSelect, true, 5, false);
            }
            if (EnteredRange != null)
            {
                EnteredRange(this);
            }
            if (OverrideEnteredRangeOutline != null)
            {
                OverrideEnteredRangeOutline(this);
            }
            else
            {
                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }            
            base.sprite.UpdateZDepth();
        }
        public Action<DefaultModule> OverrideEnteredRangeOutline;
        public Action<DefaultModule> OverrideExitedRangeOutline;

        private bool CanDisplayText = true;
        public void OverrideCanDisplayText(bool value, bool DestroysActiveText = true)
        {
            CanDisplayText = value;
            if (DestroysActiveText == true)
            {
                if (ExtantLabelController != null)
                { ExtantLabelController.Inv(); }
                if (ExtantNameLabelController != null)
                { ExtantNameLabelController.Inv(); }
            }
        }

        public string GetLabelNameProper()
        {
            if (OverrideLabelName == null | OverrideLabelName == string.Empty) { return LabelName; }
            return OverrideLabelName;
        }

        public string GetLabelNameDescrption()
        {
            if (OverrideLabelDescription == null | OverrideLabelDescription == string.Empty) { return LabelDescription; }
            return OverrideLabelDescription;

        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            
            if (ExtantLabelController != null) 
            { ExtantLabelController.Inv(); }
            if (ExtantNameLabelController != null)
            { ExtantNameLabelController.Inv(); }

            if (OverrideExitedRangeOutline != null)
            {
                OverrideExitedRangeOutline(this);
            }
            else
            {
                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            base.sprite.UpdateZDepth();
            if (ExitedRange != null)
            {
                ExitedRange(this);
            }
            
        }

        public Action<DefaultModule> ExitedRange;
        public Action<DefaultModule> EnteredRange;

        private void Update()
        {

        }

        public void Interact(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (interactor.PlayerHasCore() == null) { return; }
            if (RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                RoomHandler.unassignedInteractableObjects.Remove(this);
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);

            if (PreInteractLogic != null)
            {
                if (PreInteractLogic(this, interactor) == false)
                {
                    return;
                }
            }
            this.Pickup(interactor);
            if (OnInteractedWith != null)
            {
                OnInteractedWith(this);
            }
        }

        public Action<DefaultModule> OnInteractedWith;
        public Func<DefaultModule, PlayerController, bool> PreInteractLogic;
        public Func<DefaultModule, bool> OnPostDrop;

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private ModifiedDefaultLabelManager ExtantLabelController;
        private ModifiedDefaultLabelManager ExtantNameLabelController;


        public ModuleTier Tier = ModuleTier.Tier_1;
        public enum ModuleTier
        {
            Tier_1 = 0,
            Tier_2 = 1,
            Tier_3 = 2,
            Tier_Omega = 3
        };

        public string LabelName = "Default Module";
        public string LabelDescription = "This is a test label. \\n\\n La la la look MONEY! \"[sprite \\\"ui_coin\\\"]\"";

        public string OverrideLabelName = null;
        public string OverrideLabelDescription = null;
        public AdditionalBraveLight BraveLight = null;

        public Vector2 Offset_LabelName = new Vector2(0, 2);
        public Vector2 Offset_LabelDescription = new Vector2(0, -2);

        public Color Label_Background_Color = new Color32(121, 234, 255, 100);
        public Color Label_Background_Color_Alt = new Color32(0, 255, 54, 100);
        public Color Label_Background_Color_Override = Color.clear;


        public int AltSpriteID = -69;
        public float AdditionalWeightMultiplier = 1;

        public float EnergyConsumption = -1;

        public StickyProjectileModifier.StickyContext stickyContext = new StickyProjectileModifier.StickyContext();


        public PowerConsumptionData powerConsumptionData = new PowerConsumptionData() 
        {
            OverridePowerManagement = ReturnBasePowerConsumption
        };

        public static float ReturnBasePowerConsumption(DefaultModule module, int stack)
        {
            if (stack == 0) { return 0; }
            return stack == 1 ? module.EnergyConsumption : module.EnergyConsumption + ((module.EnergyConsumption *(stack-1))/2);
        }

        public ModuleGunStatModifier gunStatModifier;


        public static ModifiedDefaultLabelManager LabelController;


        private void GetRidOfMinimapIcon()
        {
            if (this.extant_minimapIcon != null)
            {
                Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.extant_minimapIcon);
                this.extant_minimapIcon = null;
            }
        }
        public override void OnDestroy()
        {
            this.GetRidOfMinimapIcon();

            if (ExtantLabelController != null)
            { UnityEngine.Object.Destroy(ExtantLabelController.gameObject); }

            if (ExtantNameLabelController != null)
            { UnityEngine.Object.Destroy(ExtantNameLabelController.gameObject); }
            base.OnDestroy();
        }

        public class PowerConsumptionData
        {

            public string OverridePowerDescriptionLabel = "FUCK";
            public float FirstStack = -420;
            public float AdditionalStacks = -69f;
            public Func<DefaultModule, int, float> OverridePowerManagement;
        }
    }

    public class ModifiedDefaultLabelManager : DefaultLabelController
    {
        public void Trigger_C(float d)
        {
            base.StartCoroutine(this.Expand_CR_Custom(d));
        }
        public float scaleMultiplier = 1;
        public void Trigger_CustomTime(Transform aTarget, Vector3 anOffset, float duration, float AutoDestroyTimer = -1)
        {
            this.offset = anOffset;
            this.targetObject = aTarget;
            this.Trigger_C(duration);
            if (AutoDestroyTimer > 0)
            {
                this.Invoke("Inv", AutoDestroyTimer);
            }
        }

        public void Trigger_CustomDestroyTime(float duration)
        {
            base.StartCoroutine(this.Unexpand_CR_Custom(duration, true));
        }

        public void Inv()
        {
            base.StartCoroutine(this.Unexpand_CR_Custom(0.35f, true));
        }


        private IEnumerator Expand_CR_Custom(float duration)
        {
            float elapsed = 0f;
            float targetWidth = this.label.Width + 1f;
            float targetHeight = this.label.Height + 1f;
            panel.padding.bottom = (int)(targetHeight * -2);
            this.panel.Width = targetWidth;
            while (elapsed < duration)
            {
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                //BraveTime.DeltaTime;            
                this.panel.Width = Mathf.Lerp(1f, targetWidth * scaleMultiplier, Toolbox.SinLerpTValue(elapsed / duration));
                yield return null;
            }
            yield break;
        }

        public void Update()
        {
            if (this == null) { return; }
            if (label == null) { return; }

            if (label)
            {
                if (MouseHover != null)
                {
                    MouseHover(label, label.isMouseHovering);
                }
                if (OnUpdate != null)
                {
                    OnUpdate(label);
                }
            }
        }

        public bool IsMouseHovering()
        {
            if (label == null) { return false; }
            return label.isMouseHovering;
        }

        public Action<dfLabel, bool> MouseHover;
        public Action<dfLabel> OnUpdate;
        public DefaultModule StoredModuleInfo;


        private IEnumerator Unexpand_CR_Custom(float duration, bool willDestroy = false)
        {
            float elapsed = 0f;
            float targetWidth = this.label.Width + 1f;
            float targetHeight = this.label.Height + 1f;
            panel.padding.bottom = (int)(targetHeight * -2);
            this.panel.Width = targetWidth;
            while (elapsed < duration)
            {
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                this.panel.Width = Mathf.Lerp(targetWidth, 0, elapsed / duration);
                yield return null;
            }
            if (willDestroy == true)
            {
                Destroy(this.gameObject);
            }
            yield break;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public DisplayType displayType = DisplayType.Item_Label;

        public enum DisplayType
        {
            Item_Label,
            All_Modules,
            Specific_Module_Tiers
        }

        public int CurrentDisplayTier = -1;

        public int CycleTier()
        {
            CurrentDisplayTier++;
            if (CurrentDisplayTier == 4) { CurrentDisplayTier = 0; }
            return CurrentDisplayTier;
        }
    }
}
