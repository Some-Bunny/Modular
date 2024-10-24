﻿using Alexandria.PrefabAPI;
using SaveAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.NPCAPI;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace ModularMod
{
    public static class ScrapUIController
    {
        public static GameObject ScrapUI;
        public static void Init()
        {
            dfAtlas defaultAtlas = SaveTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>().Manager.DefaultAtlas;
            dfFontBase defaultFont = SaveTools.LoadAssetFromAnywhere<GameObject>("UI Root").GetComponent<GameUIRoot>().Manager.DefaultFont;
            var Bundle = Module.ModularAssetBundle;
            defaultAtlas.AddNewItemToAtlas(Bundle.LoadAsset<Texture2D>("gear"), "gear_");

            var gearUI = PrefabBuilder.BuildObject("Scrap_UI_Panel");
            gearUI.layer = 24;

            dfPanel dfPanel = gearUI.AddComponent<dfPanel>();
            gearUI.name = "Scrap_UI_Panel";
            dfPanel.anchorStyle = dfAnchorStyle.Top | dfAnchorStyle.Left;
            dfPanel.isEnabled = true;
            dfPanel.isVisible = true;
            dfPanel.isInteractive = true;
            dfPanel.tooltip = "";
            dfPanel.pivot = dfPivotPoint.TopLeft;
            dfPanel.zindex = -1;
            dfPanel.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            dfPanel.disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            dfPanel.size = new Vector2(36, 36);
            dfPanel.minSize = Vector2.zero;
            dfPanel.maxSize = new Vector2(36, 36);
            dfPanel.clipChildren = false;
            dfPanel.inverseClipChildren = false;
            dfPanel.tabIndex = -1;
            dfPanel.canFocus = false;
            dfPanel.autoFocus = false;
            dfPanel.layout = new dfControl.AnchorLayout(dfAnchorStyle.Top | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 00f,
                    left = 0f,
                    right = 0f,
                    top = 0f
                },
                owner = dfPanel
            };
            dfPanel.renderOrder = 30;
            dfPanel.isLocalized = false;
            dfPanel.hotZoneScale = Vector2.one;
            dfPanel.allowSignalEvents = true;
            dfPanel.PrecludeUpdateCycle = false;
            dfPanel.backgroundSprite = null;//"gear_";
            dfPanel.backgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            dfPanel.padding = new RectOffset(0, 0, 0, 0);
            dfPanel.transform.localPosition += new Vector3(0, -0.0125f);
            
            //var dfC = dfPanel.gameObject.GetOrAddComponent<dfControl>();
            //dfC.name = "Scrap_UI_Panel";
            ScrapUI = dfPanel.gameObject;

            dfLabel Scrap_UI = PrefabBuilder.BuildObject("UI_Label_Scrap_").AddComponent<dfLabel>();
            Scrap_UI.AssignDefaultPresets(defaultAtlas, defaultFont);
            Scrap_UI.color = new Color32(255, 255, 255, 255);
            Scrap_UI.text = "11a";
            Scrap_UI.anchorStyle = dfAnchorStyle.Top | dfAnchorStyle.Left;
            Scrap_UI.layout = new dfControl.AnchorLayout(dfAnchorStyle.Top | dfAnchorStyle.Left)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 0f,
                    left = 0f,
                    right = 0f,
                    top = 0f
                },
                owner = Scrap_UI
            };

            Scrap_UI.transform.parent = gearUI.transform;
            //Scrap_UI.transform.localPosition += new Vector3(1.1875f, 0f);

            new Hook(typeof(GameUIRoot).GetMethod("UpdatePlayerConsumables", BindingFlags.Instance | BindingFlags.Public), typeof(ScrapUIController).GetMethod("UpdatePlayerConsumablesHook", BindingFlags.Static | BindingFlags.Public));
            new Hook(typeof(GameUIRoot).GetMethod("HideCoreUI", BindingFlags.Instance | BindingFlags.Public), typeof(ScrapUIController).GetMethod("HideCoreUIHook", BindingFlags.Static | BindingFlags.Public));
            new Hook(typeof(GameUIRoot).GetMethod("UpdateScale", BindingFlags.Instance | BindingFlags.Public), typeof(ScrapUIController).GetMethod("UpdateScaleHook", BindingFlags.Static | BindingFlags.Public));

            GlobalConsumableStorage.AddNewConsumable("Scrap");
            Alexandria.Misc.CustomActions.OnRunStart += ONPCS;

        }

        public static void ONPCS(PlayerController player1, PlayerController player2, GameManager.GameMode mode)
        {
            //var c = player.gameObject.AddComponent<ConsumableStorage>();
            GlobalConsumableStorage.SetConsumableAmount("Scrap", 0);
        }

        private static Transform transformInstance;

        public static void UpdateScaleHook(Action<GameUIRoot> orig, GameUIRoot self)
        {
            orig(self);
            var scrapLabel = FindScrapUI(self);
            int m = GameManager.Options.SmallUIEnabled ? 1 : 2;
            scrapLabel.transform.position = self.p_playerCoinLabel.transform.position + (new Vector3((0.025f + (0.025f * self.p_playerCoinLabel.Text.Length) * m) * Toolbox.CalculateScale_X_Y_Based_On_Resolution().x, -0.0025f));
            var thing = ScrapCounterVisible();
            var lab = scrapLabel.gameObject.GetComponentInChildren<dfLabel>();
            lab.text = "[sprite \"" + "gear_" + "\"]" +" "+ thing.Second.ToString();
            lab.Invalidate();
            lab.isVisible = thing.First;
            lab.gameObject.transform.position = scrapLabel.gameObject.transform.position;
            lab.gameObject.transform.localScale = self.p_playerCoinLabel.transform.localScale;
            scrapLabel.gameObject.GetComponentInChildren<dfLabel>().gameObject.transform.position = scrapLabel.gameObject.transform.position + new Vector3((0.00625f * m), 0);
        }

        public static void UpdatePlayerConsumablesHook(Action<GameUIRoot, PlayerConsumables> orig, GameUIRoot self, PlayerConsumables playerConsumables)
        {
            orig(self, playerConsumables);
            var scrapLabel = FindScrapUI(self);
            int m = GameManager.Options.SmallUIEnabled ? 1 : 2;
            scrapLabel.transform.position = self.p_playerCoinLabel.transform.position + (new Vector3((0.025f + (0.025f * self.p_playerCoinLabel.Text.Length) * m )* Toolbox.CalculateScale_X_Y_Based_On_Resolution().x, -0.0025f));
            scrapLabel.gameObject.GetComponentInChildren<dfLabel>().gameObject.transform.position = scrapLabel.gameObject.transform.position + new Vector3((0.00625f * m), 0);

        }
        public static dfPanel FindScrapUI(GameUIRoot self)
        {
            var UI = transformInstance;
            if (UI == null)
            {
                UI = self.Manager.AddPrefab(ScrapUI.gameObject).transform;
                UI.name = "DO_NOT_FUCKING_CLONE";
                var dfc = UI.gameObject.GetComponent<dfControl>();
                dfc.RelativePosition = self.p_playerCoinLabel.transform.position + (new Vector3(0.025f + (0.025f * self.p_playerCoinLabel.Text.Length), -0.0025f));
                self.AddControlToMotionGroups(dfc, Dungeonator.DungeonData.Direction.WEST);
                transformInstance = UI;
            }
            return UI.gameObject.GetComponent<dfPanel>();
        }


        public static void HideCoreUIHook(Action<GameUIRoot, string> orig, GameUIRoot self, string reason)
        {
            orig(self, reason);
            var scrapLabel = FindScrapUI(self);
            scrapLabel.isVisible = ScrapCounterVisible().First;
        }

        public static Tuple<bool, int> ScrapCounterVisible()
        {
            int count = 0;
            bool active = false;
            foreach (PlayerController player in GameManager.Instance.AllPlayers)
            {
                count = GlobalConsumableStorage.ReturnConsumableAmount("Scrap");

                for (int i = 0; i < player.passiveItems.Count; i++)
                {
                    if (player.passiveItems[i] is ModulePrinterCore) { active = true; }
                }
            }
            if (GameManager.Instance.IsFoyer == true) { active = false; }
            if (GameManager.Instance.IsPaused == true) { active = false;  }
            return new Tuple<bool, int>(active, count);
        }
    }
}
