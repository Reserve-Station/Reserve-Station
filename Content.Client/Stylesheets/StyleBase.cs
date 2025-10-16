// SPDX-FileCopyrightText: 2020 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Swept <sweptwastaken@protonmail.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 E F R <602406+Efruit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 chairbender <kwhipke1@gmail.com>
// SPDX-FileCopyrightText: 2022 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ike709 <ike709@github.com>
// SPDX-FileCopyrightText: 2024 ike709 <ike709@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CSH <menelectro3@gmail.com>
// SPDX-FileCopyrightText: 2025 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 SX-7 <sn1.test.preria.2002@gmail.com>
// SPDX-FileCopyrightText: 2025 bedroomvampire <leannetoni@proton.me>
// SPDX-FileCopyrightText: 2025 nazrin <tikufaev@outlook.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Stylesheets
{
    public abstract class StyleBase
    {
        public const string ClassHighDivider = "HighDivider";
        public const string ClassLowDivider = "LowDivider";
        public const string StyleClassLabelHeading = "LabelHeading";
        public const string StyleClassLabelSubText = "LabelSubText";
        public const string StyleClassItalic = "Italic";
        public const string StyleClassGlitchText = "GlitchText"; // New: For glitch effect on text elements

        public const string ClassAngleRect = "AngleRect";

        public const string ButtonOpenRight = "OpenRight";
        public const string ButtonOpenLeft = "OpenLeft";
        public const string ButtonOpenBoth = "OpenBoth";
        public const string ButtonSquare = "ButtonSquare";

        public const string ButtonCaution = "Caution";

        public const int DefaultGrabberSize = 10;

        public abstract Stylesheet Stylesheet { get; }

        protected StyleRule[] BaseRules { get; }

        protected StyleBoxTexture BaseButton { get; }
        protected StyleBoxTexture BaseButtonOpenRight { get; }
        protected StyleBoxTexture BaseButtonOpenLeft { get; }
        protected StyleBoxTexture BaseButtonOpenBoth { get; }
        protected StyleBoxTexture BaseButtonSquare { get; }

        protected StyleBoxTexture BaseAngleRect { get; }
        protected StyleBoxTexture AngleBorderRect { get; }

        // Enhanced Cyberpunk / Glitch color palette (common)
        public static readonly Color PanelDark = Color.FromHex("#0A0A0F"); // Darkened for cyberpunk void
        public static readonly Color NanoGold = Color.FromHex("#FFD700"); // Brighter neon gold
        public static readonly Color NeonCyan = Color.FromHex("#00FFFF"); // Core cyberpunk cyan
        public static readonly Color NeonMagenta = Color.FromHex("#FF00FF"); // Vibrant magenta
        public static readonly Color NeonGreenFore = Color.FromHex("#39FF14"); // Acid green
        public static readonly Color NeonPinkFore = Color.FromHex("#FF1493"); // Hot pink
        public static readonly Color ConcerningOrangeFore = Color.FromHex("#FF4500"); // Fiery orange
        public static readonly Color DangerousRedFore = Color.FromHex("#FF0000"); // Blood red
        public static readonly Color GlitchOverlay = Color.FromHex("#00FF41"); // Glitch green overlay
        public static readonly Color DisabledFore = Color.FromHex("#333333"); // Muted gray

        // Goobstation - ZH text support
        protected StyleBase(IResourceCache resCache)
        {
            var notoSans12 = resCache.GetFont
            (
                new[]
                {
                    "/Fonts/NotoSans/NotoSans-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSC-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf",
                },
                12
            );
            var notoSans12Italic = resCache.GetFont
            (
                new[]
                {
                    "/Fonts/NotoSans/NotoSans-Italic.ttf",
                    "/Fonts/NotoSans/NotoSansSC-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf",
                },
                12
            );
            var textureCloseButton = resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png");

            // Button styles.
            var buttonTex = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
            BaseButton = new StyleBoxTexture
            {
                Texture = buttonTex,
            };
            BaseButton.SetPatchMargin(StyleBox.Margin.All, 10);
            BaseButton.SetPadding(StyleBox.Margin.All, 1);
            BaseButton.SetContentMarginOverride(StyleBox.Margin.Vertical, 2);
            BaseButton.SetContentMarginOverride(StyleBox.Margin.Horizontal, 14);

            BaseButtonOpenRight = new StyleBoxTexture(BaseButton)
            {
                Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new Vector2(0, 0), new Vector2(14, 24))),
            };
            BaseButtonOpenRight.SetPatchMargin(StyleBox.Margin.Right, 0);
            BaseButtonOpenRight.SetContentMarginOverride(StyleBox.Margin.Right, 8);
            BaseButtonOpenRight.SetPadding(StyleBox.Margin.Right, 2);

            BaseButtonOpenLeft = new StyleBoxTexture(BaseButton)
            {
                Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new Vector2(10, 0), new Vector2(14, 24))),
            };
            BaseButtonOpenLeft.SetPatchMargin(StyleBox.Margin.Left, 0);
            BaseButtonOpenLeft.SetContentMarginOverride(StyleBox.Margin.Left, 8);
            BaseButtonOpenLeft.SetPadding(StyleBox.Margin.Left, 1);

            BaseButtonOpenBoth = new StyleBoxTexture(BaseButton)
            {
                Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new Vector2(10, 0), new Vector2(3, 24))),
            };
            BaseButtonOpenBoth.SetPatchMargin(StyleBox.Margin.Horizontal, 0);
            BaseButtonOpenBoth.SetContentMarginOverride(StyleBox.Margin.Horizontal, 8);
            BaseButtonOpenBoth.SetPadding(StyleBox.Margin.Right, 2);
            BaseButtonOpenBoth.SetPadding(StyleBox.Margin.Left, 1);

            BaseButtonSquare = new StyleBoxTexture(BaseButton)
            {
                Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new Vector2(10, 0), new Vector2(3, 24))),
            };
            BaseButtonSquare.SetPatchMargin(StyleBox.Margin.Horizontal, 0);
            BaseButtonSquare.SetContentMarginOverride(StyleBox.Margin.Horizontal, 8);
            BaseButtonSquare.SetPadding(StyleBox.Margin.Right, 2);
            BaseButtonSquare.SetPadding(StyleBox.Margin.Left, 1);

            BaseAngleRect = new StyleBoxTexture
            {
                Texture = buttonTex,
            };
            BaseAngleRect.SetPatchMargin(StyleBox.Margin.All, 10);

            AngleBorderRect = new StyleBoxTexture
            {
                Texture = resCache.GetTexture("/Textures/Interface/Nano/geometric_panel_border.svg.96dpi.png"),
            };
            AngleBorderRect.SetPatchMargin(StyleBox.Margin.All, 10);

            var vScrollBarGrabberNormal = new StyleBoxFlat
            {
                BackgroundColor = new Color(NeonCyan.R * 0.35f, NeonCyan.G * 0.35f, NeonCyan.B * 0.35f, 0.35f), ContentMarginLeftOverride = DefaultGrabberSize,
                ContentMarginTopOverride = DefaultGrabberSize
            };
            var vScrollBarGrabberHover = new StyleBoxFlat
            {
                BackgroundColor = new Color(NeonCyan.R * 0.5f, NeonCyan.G * 0.5f, NeonCyan.B * 0.5f, 0.35f), ContentMarginLeftOverride = DefaultGrabberSize,
                ContentMarginTopOverride = DefaultGrabberSize
            };
            var vScrollBarGrabberGrabbed = new StyleBoxFlat
            {
                BackgroundColor = new Color(NeonCyan.R * 0.6f, NeonCyan.G * 0.6f, NeonCyan.B * 0.6f, 0.35f), ContentMarginLeftOverride = DefaultGrabberSize,
                ContentMarginTopOverride = DefaultGrabberSize
            };

            var hScrollBarGrabberNormal = new StyleBoxFlat
            {
                BackgroundColor = new Color(NeonCyan.R * 0.35f, NeonCyan.G * 0.35f, NeonCyan.B * 0.35f, 0.35f), ContentMarginTopOverride = DefaultGrabberSize
            };
            var hScrollBarGrabberHover = new StyleBoxFlat
            {
                BackgroundColor = new Color(NeonCyan.R * 0.5f, NeonCyan.G * 0.5f, NeonCyan.B * 0.5f, 0.35f), ContentMarginTopOverride = DefaultGrabberSize
            };
            var hScrollBarGrabberGrabbed = new StyleBoxFlat
            {
                BackgroundColor = new Color(NeonCyan.R * 0.6f, NeonCyan.G * 0.6f, NeonCyan.B * 0.6f, 0.35f), ContentMarginTopOverride = DefaultGrabberSize
            };


            BaseRules = new[]
            {
                // Default font.
                new StyleRule(
                    new SelectorElement(null, null, null, null),
                    new[]
                    {
                        new StyleProperty("font", notoSans12),
                    }),

                // Default font.
                new StyleRule(
                    new SelectorElement(null, new[] {StyleClassItalic}, null, null),
                    new[]
                    {
                        new StyleProperty("font", notoSans12Italic),
                    }),

                // New: Glitch text style - neon color for glitch effect
                new StyleRule(
                    new SelectorElement(null, new[] {StyleClassGlitchText}, null, null),
                    new[]
                    {
                        new StyleProperty("font-color", NanoGold),
                    }),

                // Window close button base texture.
                new StyleRule(
                    new SelectorElement(typeof(TextureButton), new[] {DefaultWindow.StyleClassWindowCloseButton}, null,
                        null),
                    new[]
                    {
                        new StyleProperty(TextureButton.StylePropertyTexture, textureCloseButton),
                        new StyleProperty(Control.StylePropertyModulateSelf, new Color(NeonCyan.R * 0.7f, NeonCyan.G * 0.7f, NeonCyan.B * 0.7f, NeonCyan.A)),
                    }),
                // Window close button hover.
                new StyleRule(
                    new SelectorElement(typeof(TextureButton), new[] {DefaultWindow.StyleClassWindowCloseButton}, null,
                        new[] {TextureButton.StylePseudoClassHover}),
                    new[]
                    {
                        new StyleProperty(Control.StylePropertyModulateSelf, GlitchOverlay),
                    }),
                // Window close button pressed.
                new StyleRule(
                    new SelectorElement(typeof(TextureButton), new[] {DefaultWindow.StyleClassWindowCloseButton}, null,
                        new[] {TextureButton.StylePseudoClassPressed}),
                    new[]
                    {
                        new StyleProperty(Control.StylePropertyModulateSelf, NeonMagenta),
                    }),

                // Scroll bars with cyberpunk neon
                new StyleRule(new SelectorElement(typeof(VScrollBar), null, null, null),
                    new[]
                    {
                        new StyleProperty(ScrollBar.StylePropertyGrabber,
                            vScrollBarGrabberNormal),
                    }),

                new StyleRule(
                    new SelectorElement(typeof(VScrollBar), null, null, new[] {ScrollBar.StylePseudoClassHover}),
                    new[]
                    {
                        new StyleProperty(ScrollBar.StylePropertyGrabber,
                            vScrollBarGrabberHover),
                    }),

                new StyleRule(
                    new SelectorElement(typeof(VScrollBar), null, null, new[] {ScrollBar.StylePseudoClassGrabbed}),
                    new[]
                    {
                        new StyleProperty(ScrollBar.StylePropertyGrabber,
                            vScrollBarGrabberGrabbed),
                    }),

                new StyleRule(new SelectorElement(typeof(HScrollBar), null, null, null),
                    new[]
                    {
                        new StyleProperty(ScrollBar.StylePropertyGrabber,
                            hScrollBarGrabberNormal),
                    }),

                new StyleRule(
                    new SelectorElement(typeof(HScrollBar), null, null, new[] {ScrollBar.StylePseudoClassHover}),
                    new[]
                    {
                        new StyleProperty(ScrollBar.StylePropertyGrabber,
                            hScrollBarGrabberHover),
                    }),

                new StyleRule(
                    new SelectorElement(typeof(HScrollBar), null, null, new[] {ScrollBar.StylePseudoClassGrabbed}),
                    new[]
                    {
                        new StyleProperty(ScrollBar.StylePropertyGrabber,
                            hScrollBarGrabberGrabbed),
                    }),
            };
        }
    }
}
