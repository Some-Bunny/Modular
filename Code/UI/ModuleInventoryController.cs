using SaveAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ModularMod.DefaultModule;
using static ModularMod.ModulePrinterCore;
using UnityEngine;

namespace ModularMod
{
    public class ModuleInventoryController : ScriptableObject
    {
        private PlayerController player;
        private int Scale = 9;
        private float ScalePos = 9;

        public float ScaleMult()
        {
            Vector2 val = Toolbox.CalculateScale_X_Y_Based_On_Resolution();
            if (val.x < 1 && val.y < 1) { return 1; }
            return 1 - (val.x - 1);
        }

        public void DoQuickStart(PlayerController p)
        {
            AkSoundEngine.PostEvent("Play_UI_menu_pause_01", p.gameObject);
            player = p;
            Core = ReturnCore(p);
            CurrentMode = Mode.DEF;
            if (Core == null) { return; }
            IsNone = Core.ModuleContainers.Count == 0;
            ToggleControl(true);
            ScalePos = ScaleMult();
            CursorPatch.DisplayCursorOnController = true;
            p.StartCoroutine(DoDelays(p));
            UIHooks.OnPaused += Le_Bomb;
        }
        public IEnumerator DoFade(bool active)
        {
            GameManager.Instance.MainCameraController.SetManualControl(true, true);
            CameraController mainCameraController = GameManager.Instance.MainCameraController;
            mainCameraController.OverridePosition = active == true ? this.player.sprite.WorldCenter + new Vector2(7, -1.5f) : this.player.sprite.WorldCenter;

            float f = 0;
            while (f < 0.35f)
            {
                float q = f / 0.35f;
                f += GameManager.INVARIANT_DELTA_TIME;
                if (active == true)
                {
                    mainCameraController.SetZoomScaleImmediate(Mathf.Lerp(1, 1.8f, q));
                    Pixelator.Instance.fade = Mathf.Lerp(1f, 0.3f, q);

                }
                else
                {
                    Pixelator.Instance.fade = Mathf.Lerp(0.3f, 1f, q);
                    mainCameraController.SetZoomScaleImmediate(Mathf.Lerp(1.8f, 1, q));

                }
                yield return null;
            }
            if (active == false)
            {
                GameManager.Instance.MainCameraController.SetManualControl(false, true);
            }
            else
            {
                BraveTime.SetTimeScaleMultiplier(0, GameManager.Instance.gameObject);
            }
            yield break;
        }


        public void Le_Bomb()
        {
            UIHooks.OnPaused -= Le_Bomb;
            BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
            Nuke();
            ObliterateUI();
            Destroy(this);
            CursorPatch.DisplayCursorOnController = false;
        }
        private void ToggleControl(bool active)
        {
            GameManager.Instance.StartCoroutine(DoFade(active));
            Minimap.Instance.TemporarilyPreventMinimap = active;
            if (active == true)
            {
                if (!GameManager.Instance.MainCameraController.ManualControl)
                {
                    GameManager.Instance.MainCameraController.OverridePosition = GameManager.Instance.MainCameraController.transform.position;
                    GameManager.Instance.MainCameraController.SetManualControl(true, true);
                }
                GameUIRoot.Instance.ForceClearReload(-1);
                GameUIRoot.Instance.notificationController.ForceHide();
                GameUIRoot.Instance.levelNameUI.BanishLevelNameText();
                Pixelator.Instance.FadeColor = Color.black;
                GameUIRoot.Instance.HideCoreUI("ModularInventory");

                for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
                {
                    PlayerController playerController = GameManager.Instance.AllPlayers[i];
                    if (playerController)
                    {
                        playerController.CurrentInputState = PlayerInputState.NoInput;
                    }
                }
            }
            if (active == false)
            {
                GameManager.Instance.MainCameraController.SetManualControl(false, true);

                GameUIRoot.Instance.ShowCoreUI("ModularInventory");

                BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);

                for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
                {
                    if (GameManager.Instance.AllPlayers[j])
                    {
                        GameManager.Instance.AllPlayers[j].CurrentInputState = PlayerInputState.AllInput;
                    }
                }
            }
        }

        private void Nuke()
        {
            AkSoundEngine.PostEvent("Play_UI_menu_cancel_01", player.gameObject);
            AkSoundEngine.PostEvent("Play_UI_menu_unpause_01", player.gameObject);
            GameManager.Instance.Unpause();

            Minimap.Instance.TemporarilyPreventMinimap = false;
            GameManager.Instance.MainCameraController.SetManualControl(false, true);
            GameManager.Instance.StartCoroutine(DoFade(false));
            GameUIRoot.Instance.ShowCoreUI("ModularInventory");

            BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].CurrentInputState = PlayerInputState.AllInput;
                }
            }
        }


        public IEnumerator DoDelays(PlayerController p)
        {
            yield return null;
            CurrentMode = Mode.DEF;
            for (int i = 0; i < Core.ModuleContainers.Count; i++)
            {
                var defMod = Core.ModuleContainers[i];
                AddNewPages(defMod);
                AddNewPagesTiered(defMod);
            }
            yield return null;
            DoButtonRefresh(p);
            yield return null;
            DisplayModule(p);
        }

        public void DoUpdate(PlayerController p, int PageMove)
        {
            UpdatePageLabel();
            ListEntry += PageMove;
            UpLabel.label.Invalidate();

            switch (CurrentMode)
            {
                case Mode.DEF:
                    DisplayModule(p, true);
                    return;
                case Mode.TIERED_1:
                    DisplayModuleTiered(p, ModuleTier.Tier_1, true);
                    return;
                case Mode.TIERED_2:
                    DisplayModuleTiered(p, ModuleTier.Tier_2, true);
                    return;
                case Mode.TIERED_3:
                    DisplayModuleTiered(p, ModuleTier.Tier_3, true);
                    return;
                case Mode.TIERED_4:
                    DisplayModuleTiered(p, ModuleTier.Tier_Omega, true);
                    return;
                default:
                    DisplayModule(p, true);
                    return;

            }
        }

        public void DoButtonRefresh(PlayerController p)
        {
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            if (UpLabel == null)
            {
                UpLabel = Toolbox.GenerateText(p.transform, new Vector2(1f, 0.75f), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP), cl, true, Scale);
                UpLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (IsNone == false && ListEntry > 0)
                    {
                        DoUpdate(p, -1);
                    }
                    else if (IsNone == false)
                    {
                        AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                    }
                };

                UpLabel.MouseHover = (label, boolean) =>
                {
                    if (ListEntry > 0 && IsNone == false)
                    {
                        label.text = Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.UP_BRIGHT);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP);
                        label.color = new Color32(155, 155, 155, 155);
                        label.Invalidate();
                    }
                };
                UpLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }

            if (DownLabel == null)
            {
                DownLabel = Toolbox.GenerateText(p.transform, new Vector2(1f, 0), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN), cl, true, Scale);
                DownLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (IsNone == false && ReturnPagesCount() > ListEntry)
                    {
                        DoUpdate(p, 1);


                    }
                    else if (IsNone == false)
                    {
                        AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                    }
                };
                DownLabel.MouseHover = (label, boolean) =>
                {
                    if (ReturnPagesCount() > ListEntry && IsNone == false)
                    {
                        label.text = Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.DOWN_BRIGHT);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN);
                        label.color = new Color32(155, 155, 155, 155);
                        label.Invalidate();
                    }
                };
                DownLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };

            }
            if (CloseLabel == null)
            {
                CloseLabel = Toolbox.GenerateText(p.transform, new Vector2(1f, 1.5f), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE), cl, true, Scale);
                CloseLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    UIHooks.OnPaused -= Le_Bomb;
                    BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
                    Nuke();
                    ObliterateUI();
                    Destroy(this);
                    CursorPatch.DisplayCursorOnController = false;
                };
                CloseLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.CLOSE_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                CloseLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };

            }

            if (T1bLabel == null)
            {
                T1bLabel = Toolbox.GenerateText(p.transform, new Vector2(1.75f, 2.25f), 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_1), cl, true, Scale);
                T1bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    ListEntry = 0;
                    CurrentMode = Mode.TIERED_1;
                    DisplayModuleTiered(p, ModuleTier.Tier_1, true);
                };
                T1bLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T1B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_1);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T1bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T2bLabel == null)
            {
                T2bLabel = Toolbox.GenerateText(p.transform, new Vector2(2.5f, 2.25f), 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_2), cl, true, Scale);
                T2bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.TIERED_2;
                    ListEntry = 0;
                    DisplayModuleTiered(p, ModuleTier.Tier_2, true);
                };
                T2bLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T2B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_2);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T2bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T3bLabel == null)
            {

                T3bLabel = Toolbox.GenerateText(p.transform, new Vector2(3.25f, 2.25f), 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_3), cl, true, Scale);
                T3bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.TIERED_3;
                    ListEntry = 0;
                    DisplayModuleTiered(p, ModuleTier.Tier_3, true);
                };
                T3bLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T3B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_3);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T3bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T4bLabel == null && pages_T4.Count > 0)
            {
                T4bLabel = Toolbox.GenerateText(p.transform, new Vector2(4f, 2.25f), 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_Omega), cl, true, Scale);
                T4bLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.TIERED_4;
                    ListEntry = 0;
                    DisplayModuleTiered(p, ModuleTier.Tier_Omega, true);
                };
                T4bLabel.MouseHover = (label, boolean) =>
                {
                    ListEntry = 0;
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T4B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_Omega);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T4bLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }

            if (InfoLabel == null)
            {
                InfoLabel = Toolbox.GenerateText(p.transform, new Vector2(pages_T4.Count > 0 ? 4.75f : 4f, 2.25f), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.INFO), cl, true, Scale);
                InfoLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    var ModuleContainers = CurrentMode != Mode.DEF ? ReturnPageListTier(selectedTier) : pages_default;
                    int c = ModuleContainers.Count() == 0 ? 1 : 0;

                    if (extantLabel) { Destroy(extantLabel.gameObject); }
                    foreach (var page in ModuleContainers)
                    {
                        if (ListEntry == page.Page)
                        {
                            c++;
                        }
                    }
                    extantLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0 - (0.75f * c)), 0.66f, GetInfoPage() + "\nPage: " + (InfoPage + 1).ToString() + "/" + Advice.Count.ToString(), cl, true, Scale / 2); ;
                };
                InfoLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.INFO_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.INFO);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                InfoLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }

            if (AnyLabel == null)
            {
                AnyLabel = Toolbox.GenerateText(p.transform, new Vector2(1, 2.25f), 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.GOOGLY), cl, true, Scale);

                AnyLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    CurrentMode = Mode.DEF;
                    ListEntry = 0;
                    DisplayModule(p, true);
                };
                AnyLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.GOOGLY_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.GOOGLY);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                AnyLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (PowerLabel == null)

            {
                PowerLabel = Toolbox.GenerateText(p.transform, new Vector2(1, 3f), 0.5f, "[ " + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + " : " + Core.ReturnPowerConsumption() + " / " + Core.ReturnTotalPower().ToString() + " ]" + " [" + StaticColorHexes.AddColorToLabelString("+", StaticColorHexes.Light_Orange_Hex) + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "]", cl, true, Scale - 1);
                PowerLabel.OnUpdate += (obj1) =>
                {
                    bool h = PowerLabel.IsMouseHovering();
                    string t = "[ " + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + " : " + Core.ReturnPowerConsumption() + " / " + (h == true ? StaticColorHexes.AddColorToLabelString((Core.ReturnTotalPower() + 1).ToString(), StaticColorHexes.Green_Hex) : Core.ReturnTotalPower().ToString()) + " ]";
                    t += h == true ? " [Upgrade:" + StaticColorHexes.AddColorToLabelString(ReturnUpgradeCost().ToString(), CanAffordUpgrade() == true ? StaticColorHexes.Green_Hex : StaticColorHexes.Red_Color_Hex) + scrapLabel + "]" : " [" + StaticColorHexes.AddColorToLabelString("+", StaticColorHexes.Light_Orange_Hex) + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "]";
                    obj1.color = h == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    obj1.text = t;
                    obj1.Invalidate();
                };
                PowerLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (CanAffordUpgrade() == true)
                    {
                        GlobalConsumableStorage.RemoveConsumableAmount("Scrap", ReturnUpgradeCost());
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(PowerCell.PowerCellID).gameObject, Vector3.zero, Quaternion.identity);
                        PickupObject component3 = gameObject.GetComponent<PickupObject>();
                        if (component3 != null)
                        {
                            component3.CanBeDropped = false;
                            component3.Pickup(player);
                        }
                    }
                };
                PowerLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
        }

        public void InstantUpdateLabelToText(dfLabel label, string newText)
        {
            label.text = newText;
            label.Invalidate();
        }

        public bool CanAffordUpgrade()
        {
            //if (player.gameObject.GetComponent<ConsumableStorage>() == null) { return false; }
            if (GlobalConsumableStorage.ReturnConsumableAmount("Scrap") >= ReturnUpgradeCost())
            {
                return true;
            }
            return false;
        }
        public int ReturnUpgradeCost()
        {
            return (Mathf.Max(2, Mathf.RoundToInt(Core.ReturnTotalPowerMasteryless()) + 3));
        }
        public int ReturnPagesCount()
        {
            switch (CurrentMode)
            {
                case Mode.DEF:
                    return pages_default.Count > 0 ? pages_default.Last().Page : 0;
                case Mode.TIERED_1:
                    return pages_T1.Count > 0 ? pages_T1.Last().Page : 0;
                case Mode.TIERED_2:
                    return pages_T2.Count > 0 ? pages_T2.Last().Page : 0;
                case Mode.TIERED_3:
                    return pages_T3.Count > 0 ? pages_T3.Last().Page : 0;
                case Mode.TIERED_4:
                    return pages_T4.Count > 0 ? pages_T4.Last().Page : 0;
                default:
                    return pages_default.Count > 0 ? pages_default.Last().Page : 0;
            }
        }


        public ModulePrinterCore ReturnCore(PlayerController p)
        {
            for (int o = 0; o < p.passiveItems.Count; o++)
            {
                if (p.passiveItems[o] is ModulePrinterCore core)
                { return core; }
            }
            return null;
        }

        public void DisplayModule(PlayerController p, bool ClearOut = false)
        {
            if (ClearOut == true)
            {
                for (int i = 0; i < garbageLabels.Count; i++)
                {
                    if (garbageLabels[i] != null) { Destroy(garbageLabels[i].gameObject); }
                }
                garbageLabels.Clear();
                if (extantLabel) { Destroy(extantLabel.gameObject); }
                if (PageLabel) { Destroy(PageLabel.gameObject); }
            }


            var ModuleContainers = Core.ModuleContainers;
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            string Text = "Modules Available:";
            garbageLabels.Add(Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 1.5f), 0.66f, Text, cl, true, Scale));

            int c = 0;
            if (ModuleContainers.Count == 0)
            {
                var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "None.", cl, true, Scale);
                c++;
                garbageLabels.Add(Button);
            }
            else
            {
                foreach (var page in pages_default)
                {
                    if (ListEntry == page.Page)
                    {
                        var module = page.moduleContainer.defaultModule;

                        string Temp = Core.ReturnTemporaryStack(module.LabelName) > 0 ? " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOCK) + " " + StaticColorHexes.AddColorToLabelString(Core.ReturnTemporaryStack(module.LabelName).ToString(), StaticColorHexes.Orange_Hex) + ") " : " ";
                        string PowerLabels = page.moduleContainer.isPurelyFake ? Temp : "(" + StaticColorHexes.AddColorToLabelString(module.Stack(false).ToString() + " / " + module.TrueStack(), StaticColorHexes.Orange_Hex) + ")" + Temp + " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + Core.ReturnPowerConsumption(module) + ")";
                        string T = module.LabelName + PowerLabels;
                        string TYellow = StaticColorHexes.AddColorToLabelString(module.LabelName, StaticColorHexes.Yellow_Hex) + PowerLabels;

                        var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 2, 0.75f - (0.75f * c)), 0.66f, T, cl, true, Scale);
                        Button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            if (extantLabel != null) { Destroy(extantLabel.gameObject); }
                            extantLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0f - (0.75f * c)), 0.66f, module.LabelDescription, cl, true, Scale / 2);
                        };
                        Button.MouseHover = (label, boolean) =>
                        {
                            label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.Invalidate();
                        };
                        Button.OnUpdate += (l) =>
                        {
                            Temp = Core.ReturnTemporaryStack(module.LabelName) > 0 ? " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOCK) + " " + StaticColorHexes.AddColorToLabelString(Core.ReturnTemporaryStack(module.LabelName).ToString(), StaticColorHexes.Orange_Hex) + ") " : " ";
                            PowerLabels = page.moduleContainer.isPurelyFake ? Temp : "(" + StaticColorHexes.AddColorToLabelString(module.Stack(false).ToString() + " / " + module.TrueStack(), StaticColorHexes.Orange_Hex) + ")" + Temp + " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + Core.ReturnPowerConsumption(module) + ")";
                            T = module.LabelName + PowerLabels;
                            TYellow = StaticColorHexes.AddColorToLabelString(module.LabelName, StaticColorHexes.Yellow_Hex) + PowerLabels;
                            l.text = (Button.IsMouseHovering() == true ? TYellow : T);
                            l.Invalidate();
                        };
                        Button.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };


                        var ButtonLeft = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "-", cl, true, Scale);
                        ButtonLeft.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(module.LabelName) > 0;
                            label.color = CanBeUsed == true ? boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (CanBeUsed == true && boolean == true ? StaticColorHexes.AddColorToLabelString("-", StaticColorHexes.Yellow_Hex) : "-");
                            label.Invalidate();
                        };
                        ButtonLeft.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        ButtonLeft.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(module.LabelName) > 0;
                            if (CanBeUsed == true && Core.ReturnPowerConsumption() > 0 && module.CanBeDisabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Fade_01", player.gameObject);
                                Core.DepowerModule(module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };

                        garbageLabels.Add(ButtonLeft);
                        var ButtonRight = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 1, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "+", cl, true, Scale);
                        ButtonRight.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(module.LabelName) > Core.ReturnActiveStack(module.LabelName);
                            label.color = CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (CanBeUsed == true && boolean == true && CanBeUsed3 == true && CanBeUsed2 == true ? StaticColorHexes.AddColorToLabelString("+", StaticColorHexes.Yellow_Hex) : "+");

                            label.Invalidate();
                        };
                        ButtonRight.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(module.LabelName) > Core.ReturnActiveStack(module.LabelName);

                            if (CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && module.CanBeEnabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ModulePowerUp", player.gameObject);
                                Core.PowerModule(module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };
                        ButtonRight.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };

                        garbageLabels.Add(ButtonRight);
                        garbageLabels.Add(Button);
                        c++;
                    }
                }
            }
            PageLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "Page:" + (ListEntry + 1).ToString() + " / " + (pages_default.Count > 0 ? (pages_default.Last().Page + 1).ToString() : "1"), cl, true, Scale);
        }

        public void DisplayModuleTiered(PlayerController p, ModuleTier moduleTier, bool ClearOut = false)
        {
            if (ClearOut == true)
            {
                for (int i = 0; i < garbageLabels.Count; i++)
                {
                    if (garbageLabels[i] != null) { Destroy(garbageLabels[i].gameObject); }
                }
                garbageLabels.Clear();
                if (extantLabel) { Destroy(extantLabel.gameObject); }
                if (PageLabel) { Destroy(PageLabel.gameObject); }
            }

            selectedTier = moduleTier;
            var ModuleContainers = ReturnPageListTier(moduleTier);

            Color32 cl = (moduleTier == ModuleTier.Tier_Omega) ? new Color32(200, 10, 10, 100) : p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);

            string Text = "Modules Available " + DefaultModule.ReturnTierLabel(moduleTier) + " :";
            garbageLabels.Add(Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 1.5f), 0.66f, Text, cl, true, Scale));


            int c = 0;
            if (ModuleContainers.Count == 0)
            {
                var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "None.", cl, true, Scale);
                c++;
                garbageLabels.Add(Button);
            }
            else
            {
                foreach (var page in ModuleContainers)
                {
                    if (ListEntry == page.Page)
                    {
                        var module = page.moduleContainer.defaultModule;

                        string Temp = Core.ReturnTemporaryStack(module.LabelName) > 0 ? " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOCK) + " " + StaticColorHexes.AddColorToLabelString(Core.ReturnTemporaryStack(module.LabelName).ToString(), StaticColorHexes.Orange_Hex) + ") " : " ";
                        string PowerLabels = page.moduleContainer.isPurelyFake ? Temp : "(" + StaticColorHexes.AddColorToLabelString(module.Stack(false).ToString() + " / " + module.TrueStack(), StaticColorHexes.Orange_Hex) + ")" + Temp + " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + Core.ReturnPowerConsumption(module) + ")";
                        string T = module.LabelName + PowerLabels;
                        string TYellow = StaticColorHexes.AddColorToLabelString(module.LabelName, StaticColorHexes.Yellow_Hex) + PowerLabels;

                        var Button = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 2, 0.75f - (0.75f * c)), 0.66f, T, cl, true, Scale);
                        Button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            if (extantLabel != null) { Destroy(extantLabel.gameObject); }
                            extantLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0f - (0.75f * c)), 0.66f, module.LabelDescription, cl, true, Scale / 2);
                        };
                        Button.MouseHover = (label, boolean) =>
                        {
                            label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.Invalidate();
                        };
                        Button.OnUpdate += (l) =>
                        {
                            Temp = Core.ReturnTemporaryStack(module.LabelName) > 0 ? " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOCK) + " " + StaticColorHexes.AddColorToLabelString(Core.ReturnTemporaryStack(module.LabelName).ToString(), StaticColorHexes.Orange_Hex) + ") " : " ";
                            PowerLabels = page.moduleContainer.isPurelyFake ? Temp : "(" + StaticColorHexes.AddColorToLabelString(module.Stack(false).ToString() + " / " + module.TrueStack(), StaticColorHexes.Orange_Hex) + ")" + Temp + " (" + Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + Core.ReturnPowerConsumption(module) + ")";
                            T = module.LabelName + PowerLabels;
                            TYellow = StaticColorHexes.AddColorToLabelString(module.LabelName, StaticColorHexes.Yellow_Hex) + PowerLabels;
                            l.text = (Button.IsMouseHovering() == true ? TYellow : T); l.Invalidate();
                        };
                        Button.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        var ButtonLeft = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "-", cl, true, Scale);
                        ButtonLeft.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(module.LabelName) > 0;
                            label.color = CanBeUsed == true ? boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (CanBeUsed == true && boolean == true ? StaticColorHexes.AddColorToLabelString("-", StaticColorHexes.Yellow_Hex) : "-");
                            label.Invalidate();
                        };
                        ButtonLeft.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnActiveStack(module.LabelName) > 0;
                            if (CanBeUsed == true && Core.ReturnPowerConsumption() > 0 && module.CanBeDisabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Fade_01", player.gameObject);
                                Core.DepowerModule(module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };
                        ButtonLeft.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        garbageLabels.Add(ButtonLeft);
                        var ButtonRight = Toolbox.GenerateText(p.transform, new Vector2(MainOffset + 1, 0.75f - (0.75f * c)), 0.66f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + "+", cl, true, Scale);
                        ButtonRight.MouseHover = (label, boolean) =>
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(module.LabelName) > Core.ReturnActiveStack(module.LabelName);
                            label.color = CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.text = Scrapper.ReturnButtonString(Scrapper.ButtonUI.POWER) + (CanBeUsed == true && boolean == true && CanBeUsed3 == true && CanBeUsed2 == true ? StaticColorHexes.AddColorToLabelString("+", StaticColorHexes.Yellow_Hex) : "+");
                            label.Invalidate();
                        };
                        ButtonRight.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            bool CanBeUsed = Core.ReturnPowerConsumption() <= Core.ReturnTotalPower();
                            bool CanBeUsed2 = Core.ReturnPowerConsumptionOfNextStack(module) <= Core.ReturnTotalPower();
                            bool CanBeUsed3 = Core.ReturnTrueStack(module.LabelName) > Core.ReturnActiveStack(module.LabelName);

                            if (CanBeUsed == true && CanBeUsed2 == true && CanBeUsed3 == true && module.CanBeEnabled(Core, Core.ModularGunController) == true)
                            {
                                AkSoundEngine.PostEvent("Play_ModulePowerUp", player.gameObject);
                                Core.PowerModule(module);
                            }
                            else
                            {
                                AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", player.gameObject);
                            }
                        };
                        ButtonRight.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                        garbageLabels.Add(ButtonRight);
                        garbageLabels.Add(Button);
                        c++;
                    }
                }
            }
            PageLabel = Toolbox.GenerateText(p.transform, new Vector2(MainOffset, 0.75f - (0.75f * c)), 0.66f, "Page:" + (ListEntry + 1).ToString() + " / " + (ModuleContainers.Count > 0 ? (ModuleContainers.Last().Page + 1).ToString() : "1"), cl, true, Scale);
        }


        public void UpdatePageLabel()
        {
            if (PageLabel)
            {
                PageLabel.label.text = "Page:" + (ListEntry + 1).ToString() + " / " + (pages_default.Count > 0 ? (pages_default.Last().Page + 1).ToString() : "1");
                PageLabel.label.Invalidate();
            }
        }


        public void ObliterateUI()
        {
            if (extantLabel) { extantLabel.Inv(); }
            if (DownLabel) { DownLabel.Inv(); }
            if (UpLabel) { UpLabel.Inv(); }
            if (PageLabel) { PageLabel.Inv(); }
            if (CloseLabel) { CloseLabel.Inv(); }

            if (T1bLabel) { T1bLabel.Inv(); }
            if (T2bLabel) { T2bLabel.Inv(); }
            if (T3bLabel) { T3bLabel.Inv(); }
            if (T4bLabel) { T4bLabel.Inv(); }
            if (AnyLabel) { AnyLabel.Inv(); }
            if (InfoLabel) { InfoLabel.Inv(); }

            if (PowerLabel) { PowerLabel.Inv(); }
            for (int i = 0; i < garbageLabels.Count; i++)
            {
                if (garbageLabels[i] != null) { garbageLabels[i].Inv(); }
            }
            garbageLabels.Clear();

        }

        public int AddNewPages(ModuleContainer Container)
        {
            int currentPage = pages_default.Count > 0 ? pages_default.Last().Page : 0;
            int LastEntry = pages_default.Count > 0 ? pages_default.Last().Entry : -1;
            if (LastEntry > 3)
            {
                currentPage += 1;
                LastEntry = -1;
            }
            QuickAndMessyPage quickAndMessy = new QuickAndMessyPage();
            quickAndMessy.Page = currentPage;
            quickAndMessy.Entry = LastEntry + 1;
            quickAndMessy.moduleContainer = Container;
            pages_default.Add(quickAndMessy);
            return LastEntry + 1;
        }

        public int AddNewPagesTiered(ModuleContainer Container)
        {
            var specList = ReturnPageListTier(Container.tier);
            int currentPage = specList.Count > 0 ? specList.Last().Page : 0;
            int LastEntry = specList.Count > 0 ? specList.Last().Entry : -1;
            if (LastEntry > 3)
            {
                currentPage += 1;
                LastEntry = -1;
            }
            QuickAndMessyPage quickAndMessy = new QuickAndMessyPage();
            quickAndMessy.Page = currentPage;
            quickAndMessy.Entry = LastEntry + 1;
            quickAndMessy.moduleContainer = Container;
            specList.Add(quickAndMessy);
            return LastEntry + 1;
        }


        public List<QuickAndMessyPage> ReturnPageListTier(ModuleTier moduleTier)
        {
            switch (moduleTier)
            {
                case ModuleTier.Tier_1:
                    return pages_T1;
                case ModuleTier.Tier_2:
                    return pages_T2;
                case ModuleTier.Tier_3:
                    return pages_T3;
                case ModuleTier.Tier_Omega:
                    return pages_T4;
                default:
                    return pages_T1;
            }
        }



        public bool IsNone;
        public int ListEntry = 0;
        public Mode CurrentMode;
        private int InfoPage = -1;
        public ModuleTier selectedTier;
        private string scrapLabel = "[sprite \"" + "gear_" + "\"]";
        private float MainOffset = 1.75f;

        public enum Mode
        {
            DEF,
            TIERED_1,
            TIERED_2,
            TIERED_3,
            TIERED_4,
        }

        private string GetInfoPage()
        {
            InfoPage++;
            if (InfoPage == Advice.Count())
            {
                InfoPage = 0;
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.CHECKED_ALL_ADVICE, true);
            }
            return UnityEngine.Random.value < 0.05f && AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.CHECKED_ALL_ADVICE) == true ? BraveUtility.RandomElement<string>(SpecialAdvice) : Advice[InfoPage];
        }


        public List<QuickAndMessyPage> pages_default = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T1 = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T2 = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T3 = new List<QuickAndMessyPage>();
        public List<QuickAndMessyPage> pages_T4 = new List<QuickAndMessyPage>();

        public class QuickAndMessyPage
        {
            public ModuleContainer moduleContainer;
            public int Page;
            public int Entry;
        }


        private ModulePrinterCore Core;

        public ModifiedDefaultLabelManager UpLabel;
        public ModifiedDefaultLabelManager DownLabel;
        public ModifiedDefaultLabelManager CloseLabel;
        public ModifiedDefaultLabelManager PageLabel;
        public List<ModifiedDefaultLabelManager> garbageLabels = new List<ModifiedDefaultLabelManager>();
        public List<ModifiedDefaultLabelManager> secondaryGarbageLabels = new List<ModifiedDefaultLabelManager>();
        public ModifiedDefaultLabelManager extantLabel;
        public ModifiedDefaultLabelManager T1bLabel;
        public ModifiedDefaultLabelManager T2bLabel;
        public ModifiedDefaultLabelManager T3bLabel;
        public ModifiedDefaultLabelManager T4bLabel;
        public ModifiedDefaultLabelManager AnyLabel;
        public ModifiedDefaultLabelManager PowerLabel;
        public ModifiedDefaultLabelManager InfoLabel;

        public static List<string> Advice = new List<string>()
        {
            "This is your Module Inventory, from where you will "+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+" and look at your collected modules.\nWhen in your inventory, you can click on a module to see its description and " + StaticColorHexes.AddColorToLabelString("Power usage", StaticColorHexes.Light_Blue_Color_Hex) + ".",
            "Modules need to be "+StaticColorHexes.AddColorToLabelString("powered", StaticColorHexes.Light_Blue_Color_Hex)+" to gain their benefits, but you only have a limited reserve of "+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+".\nYou can get multiple of the same module to get "+ StaticColorHexes.AddColorToLabelString("increased benefits", StaticColorHexes.Yellow_Hex)+", but also "+StaticColorHexes.AddColorToLabelString("increased Power Consumption", StaticColorHexes.Light_Blue_Color_Hex)+".\nModules stacked past the first one will only use half as much " +StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+".",
            "To upgrade your "+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+" reserve, you will need to collect Scrap by scrapping items or finding it naturally.\nYou can upgrade your Power by clicking on the label that shows your power, as long as you have enough Scrap.\n"+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+" can also be gotten from various items, like health items.",
            "Your starting active items mode can be switched by pressing "+StaticColorHexes.AddColorToLabelString("Reload", StaticColorHexes.Yellow_Hex)+" on a full clip.\nThe altername mode lets you "+StaticColorHexes.AddColorToLabelString("turn items and pickups into useful Scrap", StaticColorHexes.Yellow_Hex)+".",
            "Sometimes a gun spawns in a "+StaticColorHexes.AddColorToLabelString("slightly inconvenient position", StaticColorHexes.Yellow_Hex)+", and you can't access a shown module.\nIf you interact with the gun in this state,\nit till tether to you and move towards you, "+StaticColorHexes.AddColorToLabelString("dragging any modules", StaticColorHexes.Yellow_Hex)+" along as well.",
            "In a modules description, "+StaticColorHexes.AddColorToLabelString("Orange", StaticColorHexes.Orange_Hex)+" text represents what you get after "+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+" a module past the first stack.\nThis also includes how much "+StaticColorHexes.AddColorToLabelString("Power", StaticColorHexes.Light_Blue_Color_Hex)+" it will use as well.",
            "Some items may not be as "+StaticColorHexes.AddColorToLabelString("useless as they initially seem.", StaticColorHexes.Yellow_Hex)+".\nIndicated with a synergy, these items usually have a special benefit to the Modular.",
            "Remember to read the description of any Modules you find carefully.\nSome modules may be "+StaticColorHexes.AddColorToLabelString("more beneficial to take in very specific circumstances", StaticColorHexes.Yellow_Hex)+".",
            "Remember to "+StaticColorHexes.AddColorToLabelString("take the Bullet That Can Kill The Past", StaticColorHexes.Yellow_Hex)+", if you seek to return to your Past.",
            StaticColorHexes.AddColorToLabelString("Have fun!   :)", StaticColorHexes.Yellow_Hex),
        };
        public static List<string> SpecialAdvice = new List<string>()
        {
            "Modular cannot do a handstand, as they "+ StaticColorHexes.AddColorToLabelString("only have one hand.", StaticColorHexes.Yellow_Hex)+".",
            "There are very rare secret modules that you can find with "+ StaticColorHexes.AddColorToLabelString("very", StaticColorHexes.Yellow_Hex)+" unique effects.",
             StaticColorHexes.AddColorToLabelString("[][][][][][][][][][][][][][][]", StaticColorHexes.Yellow_Hex) +"\n"+StaticColorHexes.AddColorToLabelString("[][][][][][][][][][][][][][][]", StaticColorHexes.White_Hex) +"\n"+StaticColorHexes.AddColorToLabelString("[][][][][][][][][][][][][][][]", StaticColorHexes.Purple_Hex) +"\n"+StaticColorHexes.AddColorToLabelString("[][][][][][][][][][][][][][][]", StaticColorHexes.Black_Hex) + "\n      :)",
             StaticColorHexes.AddColorToLabelString("T", StaticColorHexes.Light_Blue_Color_Hex) +  StaticColorHexes.AddColorToLabelString("r", StaticColorHexes.Pink_Hex)+"a"+ StaticColorHexes.AddColorToLabelString("n", StaticColorHexes.Pink_Hex)+ StaticColorHexes.AddColorToLabelString("s", StaticColorHexes.Light_Blue_Color_Hex)+" Rights!",
             StaticColorHexes.AddColorToLabelString("Be kind to people. <3", StaticColorHexes.Yellow_Hex),
             StaticColorHexes.AddColorToLabelString("You've got this!", StaticColorHexes.Yellow_Hex),
             StaticColorHexes.AddColorToLabelString("Remember, there is always someone that cares about you.\nReach out if you are in need.", StaticColorHexes.Yellow_Hex),
             StaticColorHexes.AddColorToLabelString("Remember to go outside every once in a while.\nSometimes, life isn't as bleak as it may seem, and you may just need a different perspective.", StaticColorHexes.Yellow_Hex),
             StaticColorHexes.AddColorToLabelString("Take breaks from gaming every once in a while.\nAs fun as it can be, you can still burn out.", StaticColorHexes.Yellow_Hex),
             StaticColorHexes.AddColorToLabelString("If you are feeling down, do talk to someone about it.\nBottling up emotions ends up hurting you more in the long run.", StaticColorHexes.Yellow_Hex),
        };
    }
}
