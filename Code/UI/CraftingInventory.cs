using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ModularMod.DefaultModule;
using UnityEngine;

namespace ModularMod
{
    public class ModuleCrafingController : ScriptableObject
    {
        private int Scale = 9;
        private Vector2 AdditionalOffset = new Vector2(1f, 1.75f);

        public void DoQuickStart(PlayerController p)
        {

            AkSoundEngine.PostEvent("Play_UI_menu_pause_01", p.gameObject);
            player = p;
            Core = ReturnCore(p);
            if (Core == null) { return; }
            GameManager.Instance.PreventPausing = true;
            CursorPatch.DisplayCursorOnController = true;

            p.StartCoroutine(DoDelays(p));
            ToggleControl(true);
            UIHooks.OnPaused += Le_Bomb;

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
                GameUIRoot.Instance.HideCoreUI("ModularCrafter");

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

                GameUIRoot.Instance.ShowCoreUI("ModularCrafter");

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

        public void Nuke()
        {
            UIHooks.OnPaused -= Le_Bomb;
            GameManager.Instance.PreventPausing = false;
            GameManager.Instance.Unpause();

            Minimap.Instance.TemporarilyPreventMinimap = false;
            AkSoundEngine.PostEvent("Play_UI_menu_cancel_01", player.gameObject);
            AkSoundEngine.PostEvent("Play_UI_menu_unpause_01", player.gameObject);
            GameManager.Instance.MainCameraController.SetManualControl(false, true);
            GameManager.Instance.StartCoroutine(DoFade(false));
            GameUIRoot.Instance.ShowCoreUI("ModularCrafter");

            BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                if (GameManager.Instance.AllPlayers[j])
                {
                    GameManager.Instance.AllPlayers[j].CurrentInputState = PlayerInputState.AllInput;
                }
            }
            CursorPatch.DisplayCursorOnController = false;
            if (Queue.Count == 0) { return; }
            GameManager.Instance.StartCoroutine(CraftModules(player, Queue));
        }

        public IEnumerator CraftModules(PlayerController p, List<DefaultModule> modules)
        {
            AkSoundEngine.PostEvent("Play_OBJ_computer_boop_01", p.gameObject);
            //ETGModConsole.Log(modules.Count);

            foreach (var entry in modules)
            {
                AkSoundEngine.PostEvent("Play_OBJ_bomb_fuse_01", p.gameObject);
                var debris = LootEngine.SpawnItem(entry.gameObject, p.sprite.WorldCenter, Vector2.zero, 0, true);
                debris.sprite.renderer.material.shader = StaticShaders.Displacer_Beast_Shader;
                debris.sprite.renderer.material.SetTexture("_MainTex", debris.sprite.renderer.material.mainTexture);
                SpriteOutlineManager.RemoveOutlineFromSprite(debris.sprite, false);
                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    if (debris == null) { yield break; }
                    float t = Toolbox.SinLerpTValueFull(elapsed);
                    entry.BraveLight.LightIntensity = Mathf.Lerp(0, 20, t);
                    entry.BraveLight.LightRadius = Mathf.Lerp(0, 3, t);
                    elapsed += BraveTime.DeltaTime;
                    debris.sprite.renderer.material.SetFloat("_BurnAmount", 1 - elapsed);
                    yield return null;
                }
                LootEngine.DoDefaultItemPoof(debris.sprite.WorldCenter);
                SpriteOutlineManager.AddOutlineToSprite(debris.sprite, Color.black);
            }
            yield return null;
        }

        public IEnumerator DoDelays(PlayerController p)
        {
            yield return null;
            yield return null;
            DoButtonRefreshSelect(p);
            //DoButtonRefresh(p);
            yield return null;
            //DisplayModule(p);
        }

        public void DoButtonRefreshSelect(PlayerController p)
        {
            Tier = SelectedTier.NONE;
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            if (T1_Select == null)
            {
                T1_Select = Toolbox.GenerateText(p.transform, new Vector2(2.5f, 0.25f) + AdditionalOffset, 0.5f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_1), cl, true, Scale + 6);
                T1_Select.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    Tier = SelectedTier.T1;
                    ObliterateSelectUI();
                    DoButtonRefreshCraft(p);
                };
                T1_Select.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T1B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_1);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T1_Select.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T2_Select == null)
            {
                T2_Select = Toolbox.GenerateText(p.transform, new Vector2(3.5f, 0.25f) + AdditionalOffset, 0.75f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_2), cl, true, Scale + 6);
                T2_Select.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    Tier = SelectedTier.T2;
                    ObliterateSelectUI();
                    DoButtonRefreshCraft(p);
                };
                T2_Select.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T2B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_2);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T2_Select.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (T3_Select == null)
            {
                T3_Select = Toolbox.GenerateText(p.transform, new Vector2(4.5f, 0.25f) + AdditionalOffset, 0.75f, DefaultModule.ReturnTierLabel(ModuleTier.Tier_3), cl, true, Scale + 6);
                T3_Select.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    Tier = SelectedTier.T3;
                    ObliterateSelectUI();
                    DoButtonRefreshCraft(p);
                };
                T3_Select.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.T3B) : DefaultModule.ReturnTierLabel(ModuleTier.Tier_3);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                T3_Select.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (CloseLabel == null)
            {
                CloseLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, 0.25f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.CLOSE), cl, true, Scale + 6);
                CloseLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    UIHooks.OnPaused -= Le_Bomb;
                    BraveTime.ClearMultiplier(GameManager.Instance.gameObject);
                    ObliterateUI();
                    Destroy(this);
                    ToggleControl(false);
                    Nuke();
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
            if (basicgarbageLabel == null)
            {
                basicgarbageLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, 1.125f) + AdditionalOffset, 0.5f, "Select Crafting Tier:", cl, true, 6);

            }
        }


        public Action OnCrafted;

        public int GetScrapCount(PlayerController p)
        {
            return GlobalConsumableStorage.ReturnConsumableAmount("Scrap");
        }

        private string scrapLabel = "[sprite \"" + "gear_" + "\"]";

        public void DoButtonRefreshCraft(PlayerController p)
        {
            ListEntry = 0;
            Color32 cl = p.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);
            List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages = ReturnLib();
            if (pageUpLabel) { Destroy(pageUpLabel.gameObject); }
            if (pageUpLabel == null)
            {
                pageUpLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, -1f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP), cl, true, Scale + 6);
                pageUpLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {

                    if (ListEntry > 0)
                    {
                        UpdatePageLabel(quickAndMessyPages);
                        ListEntry--;
                        pageUpLabel.label.Invalidate();
                        UpdateOptions();
                    }

                };
                pageUpLabel.MouseHover = (label, boolean) =>
                {
                    if (ListEntry > 0)
                    {
                        label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.UP_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.UP);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.color = new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                };
                pageUpLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            if (pageReturnLabel) { Destroy(pageReturnLabel.gameObject); }
            if (pageReturnLabel == null)
            {
                pageReturnLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, -3.5f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.LEFT), cl, true, Scale + 6);
                pageReturnLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    ObliterateCraftUI();
                    DoButtonRefreshSelect(p);
                };
                pageReturnLabel.MouseHover = (label, boolean) =>
                {
                    label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.LEFT_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.LEFT);
                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                    label.Invalidate();
                };
                pageReturnLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }

            if (pageDownLabel) { Destroy(pageDownLabel.gameObject); }
            if (pageDownLabel == null)
            {
                pageDownLabel = Toolbox.GenerateText(p.transform, new Vector2(1.5f, -2.25f) + AdditionalOffset, 0.5f, Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN), cl, true, Scale + 6);
                pageDownLabel.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                {
                    if (ReturnPagesCount(quickAndMessyPages) > ListEntry)
                    {
                        UpdatePageLabel(quickAndMessyPages);
                        ListEntry++;
                        pageDownLabel.label.Invalidate();
                        UpdateOptions();
                    }
                };
                pageDownLabel.MouseHover = (label, boolean) =>
                {
                    if (ReturnPagesCount(quickAndMessyPages) > ListEntry)
                    {
                        label.text = boolean == true ? Scrapper.ReturnButtonStringBright(Scrapper.ButtonUIBright.DOWN_BRIGHT) : Scrapper.ReturnButtonString(Scrapper.ButtonUI.DOWN);
                        label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                    else
                    {
                        label.color = new Color32(200, 200, 200, 200);
                        label.Invalidate();
                    }
                };
                pageDownLabel.label.MouseEnter += (o1, o2) =>
                {
                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                };
            }
            UpdateOptions();
        }
        private PlayerController player;



        public void UpdateOptions()
        {
            if (extantLabel) { extantLabel.Inv(); }
            if (PageLabel) { Destroy(PageLabel.gameObject); }
            if (craftLabel) { craftLabel.Inv(); }

            Color32 cl = player.IsUsingAlternateCostume == true ? new Color32(0, 255, 54, 100) : new Color32(121, 234, 255, 100);

            foreach (var entry in craftingLabels)
            {
                if (entry.Key != null) { Destroy(entry.Key.gameObject); }
            }

            List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages = ReturnLib();
            int c = 0;
            foreach (var page in quickAndMessyPages)
            {
                if (ListEntry == page.Page)
                {
                    string T = page.module.LabelName + " (" + scrapLabel + " " + ModuleCost(page.module).ToString() + ")";
                    bool encountered = GameStatsManager.m_instance.m_encounteredTrackables.ContainsKey(page.module.encounterTrackable.EncounterGuid);
                    if (encountered == false) { T = StaticColorHexes.AddColorToLabelString("[UNDISCOVERED]", StaticColorHexes.Light_Orange_Hex); }

                    var Button = Toolbox.GenerateText(player.transform, new Vector2(2.5f, 0.25f - (0.75f * c)) + AdditionalOffset, 0.66f, T, cl, true, Scale);
                    Button.StoredModuleInfo = page.module;

                    //Debug.Log(page.module.name + " : " + encountered);
                    if (encountered == true)
                    {
                        Button.label.Click += delegate (dfControl control, dfMouseEventArgs mouseEvent)
                        {
                            if (extantLabel != null) { Destroy(extantLabel.gameObject); }
                            extantLabel = Toolbox.GenerateText(player.transform, new Vector2(2.5f, -1.75f - (0.5f * c)) + AdditionalOffset, 0.66f, page.module.LabelDescription, cl, true, 4);
                            if (craftLabel) { Destroy(craftLabel.gameObject); }
                            craftLabel = Toolbox.GenerateText(player.transform, new Vector2(2.5f, 1f) + AdditionalOffset, 0.66f, StaticColorHexes.AddColorToLabelString("CRAFT", StaticColorHexes.Light_Green_Hex) + "( " + scrapLabel + " " + StaticColorHexes.AddColorToLabelString("-" + ModuleCost(page.module).ToString(), StaticColorHexes.Red_Color_Hex) + " )", cl, true, Scale);
                            craftLabel.MouseHover = (label, boolean) =>
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    label.text = StaticColorHexes.AddColorToLabelString("CRAFT", boolean == true ? StaticColorHexes.Light_Orange_Hex : StaticColorHexes.White_Hex) + "( " + scrapLabel + " " + StaticColorHexes.AddColorToLabelString("-" + ModuleCost(page.module).ToString(), StaticColorHexes.Red_Color_Hex) + " )";
                                    label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                                    label.Invalidate();
                                }
                            };
                            craftLabel.OnUpdate = (label) =>
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    label.text = StaticColorHexes.AddColorToLabelString("CRAFT", StaticColorHexes.White_Hex) + "( " + scrapLabel + " " + StaticColorHexes.AddColorToLabelString("-" + ModuleCost(page.module).ToString(), StaticColorHexes.Red_Color_Hex) + " )";
                                    label.Invalidate();
                                }
                                else
                                {
                                    label.text = StaticColorHexes.AddColorToLabelString("INSUFFICIENT SCRAP", StaticColorHexes.Red_Color_Hex);
                                    label.Invalidate();
                                }
                            };
                            craftLabel.label.Click += delegate (dfControl control1, dfMouseEventArgs mouseEvent2)
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    AkSoundEngine.PostEvent("Play_OBJ_metronome_jingle_01", player.gameObject);
                                    Toolbox.NotifyCustom("Crafted Module:", page.module.LabelName, page.module.sprite.spriteId, page.module.sprite.collection, UINotificationController.NotificationColor.PURPLE);
                                    GlobalConsumableStorage.RemoveConsumableAmount("Scrap", ModuleCost(page.module));
                                    Queue.Add(page.module);
                                    if (OnCrafted != null) { OnCrafted(); }
                                }
                            };
                            craftLabel.label.MouseEnter += (o1, o2) =>
                            {
                                if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                                {
                                    AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                                }
                            };
                            //craftLabel
                        };
                        Button.MouseHover = (label, boolean) =>
                        {
                            label.color = boolean == true ? new Color32(255, 255, 255, 255) : new Color32(200, 200, 200, 200);
                            label.Invalidate();
                        };
                        Button.OnUpdate = (label) =>
                        {
                            if (ModuleCost(Button.StoredModuleInfo) <= GetScrapCount(player))
                            {//Button
                                label.text = StaticColorHexes.AddColorToLabelString(Button.StoredModuleInfo.LabelName, Button.IsMouseHovering() == true ? StaticColorHexes.Light_Orange_Hex : StaticColorHexes.White_Hex) + " (" + scrapLabel + " " + StaticColorHexes.AddColorToLabelString(ModuleCost(Button.StoredModuleInfo).ToString(), StaticColorHexes.Green_Hex) + ")";
                                label.Invalidate();
                            }
                            else
                            {
                                label.text = StaticColorHexes.AddColorToLabelString(Button.StoredModuleInfo.LabelName, Button.IsMouseHovering() == true ? StaticColorHexes.Light_Orange_Hex : StaticColorHexes.White_Hex) + " (" + scrapLabel + " " + StaticColorHexes.AddColorToLabelString(ModuleCost(Button.StoredModuleInfo).ToString(), StaticColorHexes.Red_Color_Hex) + ")";
                                label.Invalidate();
                            }
                        };
                        Button.label.MouseEnter += (o1, o2) =>
                        {
                            AkSoundEngine.PostEvent("Play_UI_menu_select_01", player.gameObject);
                        };
                    }
                    craftingLabels.Add(Button, page.module);
                    c++;
                }
            }
            PageLabel = Toolbox.GenerateText(player.transform, new Vector2(2.5f, 0.25f - (0.75f * c)) + AdditionalOffset, 0.66f, "Page:" + (ListEntry + 1).ToString() + " / " + (quickAndMessyPages.Count > 0 ? (quickAndMessyPages.Last().Page + 1).ToString() : "1"), cl, true, Scale);
            UpdatePageLabel(quickAndMessyPages);
        }


        public int ModuleCost(DefaultModule module)
        {
            if (module.OverrideScrapCost != null) { return module.OverrideScrapCost.Value; }
            switch (module.Tier)
            {
                case ModuleTier.Tier_1:
                    return 5;
                case ModuleTier.Tier_2:
                    return 10;
                case ModuleTier.Tier_3:
                    return 15;
                default:
                    return 10;
            }
        }

        public List<DefaultModule> Queue = new List<DefaultModule>();


        public void UpdatePageLabel(List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages)
        {
            if (PageLabel)
            {
                PageLabel.label.text = "Page:" + (ListEntry + 1).ToString() + " / " + (quickAndMessyPages.Count > 0 ? (quickAndMessyPages.Last().Page + 1).ToString() : "1");
                PageLabel.label.Invalidate();
            }
        }
        public int ReturnPagesCount(List<GlobalModuleStorage.QuickAndMessyPage> quickAndMessyPages)
        {
            return quickAndMessyPages.Count > 0 ? quickAndMessyPages.Last().Page : 0;
        }

        private List<GlobalModuleStorage.QuickAndMessyPage> ReturnLib()
        {
            switch (Tier)
            {
                case SelectedTier.T1:
                    return GlobalModuleStorage.pages_T1;
                case SelectedTier.T2:
                    return GlobalModuleStorage.pages_T2;
                case SelectedTier.T3:
                    return GlobalModuleStorage.pages_T3;

                default:
                    return GlobalModuleStorage.pages_T1;
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

        private void ObliterateCraftUI()
        {
            if (PageLabel) { PageLabel.Inv(); }
            if (extantLabel) { extantLabel.Inv(); }
            if (pageUpLabel) { pageUpLabel.Inv(); }
            if (pageDownLabel) { pageDownLabel.Inv(); }
            if (PageLabel) { PageLabel.Inv(); }
            if (craftLabel) { craftLabel.Inv(); }
            if (pageReturnLabel) { pageReturnLabel.Inv(); }


            foreach (var entry in craftingLabels)
            {
                if (entry.Key != null) { entry.Key.Inv(); }
            }
            craftingLabels.Clear();
        }
        private void ObliterateSelectUI()
        {
            if (T1_Select) { Destroy(T1_Select.gameObject); }
            if (T2_Select) { Destroy(T2_Select.gameObject); }
            if (T3_Select) { Destroy(T3_Select.gameObject); }
            if (basicgarbageLabel) { Destroy(basicgarbageLabel.gameObject); }

        }

        public void ObliterateUI()
        {
            if (T1_Select) { T1_Select.Inv(); }
            if (T2_Select) { T2_Select.Inv(); }
            if (T3_Select) { T3_Select.Inv(); }
            if (CloseLabel) { CloseLabel.Inv(); }
            if (extantLabel) { extantLabel.Inv(); }
            if (pageDownLabel) { pageDownLabel.Inv(); }
            if (pageUpLabel) { pageUpLabel.Inv(); }
            if (PageLabel) { PageLabel.Inv(); }
            if (pageReturnLabel) { pageReturnLabel.Inv(); }
            if (craftLabel) { craftLabel.Inv(); }
            if (basicgarbageLabel) { Destroy(basicgarbageLabel.gameObject); }


            foreach (var entry in craftingLabels)
            {
                if (entry.Key != null) { entry.Key.Inv(); }
            }
            craftingLabels.Clear();
        }

        private SelectedTier Tier = SelectedTier.NONE;
        public ModulePrinterCore Core;
        public int ListEntry = 0;

        //Labels
        public Dictionary<ModifiedDefaultLabelManager, DefaultModule> craftingLabels = new Dictionary<ModifiedDefaultLabelManager, DefaultModule>();


        public ModifiedDefaultLabelManager T1_Select;
        public ModifiedDefaultLabelManager T2_Select;
        public ModifiedDefaultLabelManager T3_Select;

        public ModifiedDefaultLabelManager CloseLabel;
        public ModifiedDefaultLabelManager PageLabel;
        public ModifiedDefaultLabelManager extantLabel;
        public ModifiedDefaultLabelManager craftLabel;

        public ModifiedDefaultLabelManager pageUpLabel;
        public ModifiedDefaultLabelManager pageDownLabel;
        public ModifiedDefaultLabelManager pageReturnLabel;

        public ModifiedDefaultLabelManager basicgarbageLabel;

        private enum SelectedTier
        {
            NONE,
            T1,
            T2,
            T3,

        }
    }

}
