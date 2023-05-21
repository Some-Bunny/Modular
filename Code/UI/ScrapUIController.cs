using Alexandria.PrefabAPI;
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
            dfPanel.backgroundSprite = "gear_";
            dfPanel.backgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            dfPanel.padding = new RectOffset(0, 0, 0, 0);
            dfPanel.transform.localPosition += new Vector3(0, -0.0625f);
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
            Scrap_UI.transform.localPosition += new Vector3(1, 0f);

            new Hook(typeof(GameUIRoot).GetMethod("UpdatePlayerConsumables", BindingFlags.Instance | BindingFlags.Public), typeof(ScrapUIController).GetMethod("UpdatePlayerConsumablesHook", BindingFlags.Static | BindingFlags.Public));
            new Hook(typeof(GameUIRoot).GetMethod("HideCoreUI", BindingFlags.Instance | BindingFlags.Public), typeof(ScrapUIController).GetMethod("HideCoreUIHook", BindingFlags.Static | BindingFlags.Public));
            new Hook(typeof(GameUIRoot).GetMethod("UpdateScale", BindingFlags.Instance | BindingFlags.Public), typeof(ScrapUIController).GetMethod("UpdateScaleHook", BindingFlags.Static | BindingFlags.Public));

            Alexandria.Misc.CustomActions.OnNewPlayercontrollerSpawned += ONPCS;
        }

        public static void ONPCS(PlayerController player)
        {
            var c = player.gameObject.AddComponent<ConsumableStorage>();
            c.AddNewConsumable("Scrap");
        }

        private static Transform transformInstance;

        public static void UpdateScaleHook(Action<GameUIRoot> orig, GameUIRoot self)
        {
            orig(self);
            var scrapLabel = FindScrapUI(self);
            scrapLabel.transform.position = self.p_playerCoinLabel.transform.position + (new Vector3(0.04f + (0.0125f * self.p_playerCoinLabel.Text.Length), -0.005f));
            var thing = ScrapCounterVisible();
            scrapLabel.gameObject.GetComponentInChildren<dfLabel>().text = thing.Second.ToString();
            scrapLabel.gameObject.GetComponentInChildren<dfLabel>().Invalidate();
            scrapLabel.gameObject.GetComponentInChildren<dfLabel>().isVisible = thing.First;
            scrapLabel.gameObject.GetComponent<dfPanel>().isVisible = thing.First;

        }

        public static dfPanel FindScrapUI(GameUIRoot self)
        {
            var UI = transformInstance;
            if (UI == null)
            {
                UI = self.Manager.AddPrefab(ScrapUI.gameObject).transform;
                UI.name = "DO_NOT_FUCKING_CLONE";
                var dfc = UI.gameObject.GetComponent<dfControl>();
                dfc.RelativePosition = self.p_playerCoinLabel.transform.position + (new Vector3(0.04f + (0.0125f * self.p_playerCoinLabel.Text.Length), -0.005f));
                self.AddControlToMotionGroups(dfc, Dungeonator.DungeonData.Direction.WEST);
                transformInstance = UI;
            }
            return UI.gameObject.GetComponent<dfPanel>();
        }

        public static void UpdatePlayerConsumablesHook(Action<GameUIRoot, PlayerConsumables> orig, GameUIRoot self, PlayerConsumables playerConsumables)
        {
            orig(self, playerConsumables);
            var scrapLabel = FindScrapUI(self);
            scrapLabel.transform.position = self.p_playerCoinLabel.transform.position + (new Vector3(0.04f + (0.0125f * self.p_playerCoinLabel.Text.Length), -0.005f));
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
                var c = player.GetComponent<ConsumableStorage>();
                if (c != null)
                {
                    count = c.ReturnConsumableAmount("Scrap");
                }
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
