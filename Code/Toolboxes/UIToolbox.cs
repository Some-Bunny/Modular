﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModularMod
{
    public static class UIToolbox
    {
        public static void AssignDefaultPresets(this dfPanel self, dfAtlas defaultAtlas, Vector2 size, dfAnchorStyle dfAnchorStyle)
        {
            self.anchorStyle = dfAnchorStyle;
            self.isEnabled = true;
            self.isVisible = true;
            self.isInteractive = true;
            self.tooltip = "";
            self.pivot = dfPivotPoint.TopLeft;
            self.zindex = -2;
            self.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.size = size;
            self.minSize = size;
            self.maxSize = size;
            self.clipChildren = false;
            self.inverseClipChildren = false;
            self.tabIndex = 2;
            self.canFocus = false;
            self.autoFocus = false;
            self.layout = new dfControl.AnchorLayout(dfAnchorStyle)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 0f,
                    left = 0f,
                    right = 0f,
                    top = 0f
                },
                owner = self
            };
            self.renderOrder = 30;
            self.isLocalized = false;
            self.hotZoneScale = Vector2.one;
            self.allowSignalEvents = true;
            self.PrecludeUpdateCycle = false;
            self.atlas = defaultAtlas;
            self.isControlClipped = false;

            self.backgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.padding = new RectOffset(0, 0, 0, 0);
            //self.cachedManager = StarterGunSelectUIController.mana;
        }


        public static void AssignDefaultPresets(this dfLabel self, dfAtlas defaultAtlas, dfFontBase defaultFont)
        {
            self.anchorStyle = (dfAnchorStyle.Top | dfAnchorStyle.CenterHorizontal);
            self.isEnabled = true;
            self.isVisible = true;
            self.isInteractive = true;
            self.pivot = dfPivotPoint.MiddleCenter;
            self.tooltip = "";
            self.pivot = dfPivotPoint.TopLeft;
            self.zindex = -2;
            self.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.size = new Vector2(486f, 42f);
            self.minSize = Vector2.zero;
            self.maxSize = Vector2.zero;
            self.clipChildren = false;
            self.inverseClipChildren = false;
            self.tabIndex = 2;
            self.canFocus = false;
            self.autoFocus = false;
            self.isControlClipped = false;

            self.layout = new dfControl.AnchorLayout(dfAnchorStyle.Top | dfAnchorStyle.CenterHorizontal)
            {
                margins = new dfAnchorMargins
                {
                    bottom = 0f,
                    left = 0f,
                    right = 0f,
                    top = 0f
                },
                owner = self
            };
            self.renderOrder = 0;
            self.IsLocalized = false;
            self.hotZoneScale = Vector2.one;
            self.allowSignalEvents = true;
            self.PrecludeUpdateCycle = false;
            self.atlas = defaultAtlas;
            self.font = defaultFont;
            self.backgroundSprite = "";
            self.backgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.autoSize = false;
            self.autoHeight = false;
            self.wordWrap = false;
            self.text = "";
            self.bottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.align = TextAlignment.Left;
            self.vertAlign = dfVerticalAlignment.Top;
            self.textScale = 3f;
            self.textScaleMode = dfTextScaleMode.None;
            self.charSpacing = 0;
            self.colorizeSymbols = false;
            self.processMarkup = true;
            self.outline = false;
            self.outlineWidth = 1;
            self.enableGradient = false;
            self.outlineColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.shadow = false;
            self.shadowColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.shadowOffset = new Vector2(1f, -1f);
            self.padding = new RectOffset(0, 0, 0, 0);
            self.tabSize = 48;
            self.MaintainJapaneseFont = false;
            self.MaintainKoreanFont = false;
            self.MaintainRussianFont = false;
            self.isControlClipped = false;

        }

        public static void AssignDefaultPresets(this dfButton self, dfAtlas defaultAtlas, dfFontBase defaultFont)
        {
            self.isEnabled = true;
            self.pivot = dfPivotPoint.MiddleCenter;
            self.renderOrder = 30;
            self.isLocalized = false;
            self.atlas = defaultAtlas;
            self.Atlas = defaultAtlas;

            self.font = defaultFont;
            self.state = dfButton.ButtonState.Default;
            self.group = null;
            self.text = "";
            self.hoverTextPixelOffset = -9;
            self.downTextPixelOffset = -12;
            self.textAlign = TextAlignment.Center;
            self.vertAlign = dfVerticalAlignment.Top;
            self.normalColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.textColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.hoverText = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.pressedText = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.focusText = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.disabledText = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.hoverColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.pressedColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.focusColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.textScale = 0.6f;
            self.wordWrap = false;
            self.padding = new RectOffset(0, 0, 0, 0);
            self.textShadow = false;
            self.shadowColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            self.shadowOffset = new Vector2(1f, -1f);
            self.autoSize = false;
            self.clickWhenSpacePressed = true;
            self.forceUpperCase = false;
            self.manualStateControl = false;
            self.isControlClipped = false;

        }


        public static dfButton CreateBlankDfButton(this GameObject obj, Vector2 size, dfAnchorStyle dfAnchor, dfAnchorMargins margins)
        {
            dfButton t = obj.AddComponent<dfButton>();
            t.anchorStyle = dfAnchor;
            t.isEnabled = true;
            t.isVisible = true;
            t.isInteractive = true;
            t.tooltip = "";
            t.pivot = dfPivotPoint.MiddleCenter;
            t.zindex = -2;
            t.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            t.disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            t.size = size;
            t.minSize = size;
            t.maxSize = size;
            t.clipChildren = false;
            t.inverseClipChildren = false;
            t.tabIndex = 2;
            t.canFocus = false;
            t.autoFocus = false;
            t.layout = new dfControl.AnchorLayout(dfAnchor)
            {
                margins = margins,
                owner = t
            };
            t.renderOrder = 20;
            t.isLocalized = false;
            t.font = null;
            t.hotZoneScale = Vector2.one;
            t.allowSignalEvents = true;
            t.PrecludeUpdateCycle = false;
            //t.cachedManager = StarterGunSelectUIController.mana;
            t.isControlClipped = false;

            return t;
        }

        public static dfLabel CreateBlankDfLabel(this GameObject obj, Vector2 size, dfAnchorStyle dfAnchor, dfAnchorMargins margins)
        {
            dfLabel t = obj.AddComponent<dfLabel>();
            t.anchorStyle = dfAnchor;
            t.isEnabled = true;
            t.isVisible = true;
            t.isInteractive = true;
            t.tooltip = "";
            t.pivot = dfPivotPoint.TopLeft;
            t.zindex = -2;
            t.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            t.disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            t.size = size;
            t.minSize = size;
            t.maxSize = size;
            t.clipChildren = false;
            t.inverseClipChildren = false;
            t.tabIndex = 2;
            t.canFocus = true;
            t.autoFocus = false;
            t.layout = new dfControl.AnchorLayout(dfAnchor)
            {
                margins = margins,
                owner = t
            };
            t.renderOrder = 50;
            t.isLocalized = false;
            t.hotZoneScale = Vector2.one;
            t.allowSignalEvents = true;
            t.PrecludeUpdateCycle = false;
            t.font = null;
            //t.cachedManager = StarterGunSelectUIController.mana;

            t.isControlClipped = false;

            return t;
        }
    }
}
