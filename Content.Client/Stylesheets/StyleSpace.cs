// SPDX-FileCopyrightText: 2020 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Swept <sweptwastaken@protonmail.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 E F R <602406+Efruit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 chairbender <kwhipke1@gmail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CSH <menelectro3@gmail.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 SX-7 <sn1.test.preria.2002@gmail.com>
// SPDX-FileCopyrightText: 2025 nazrin <tikufaev@outlook.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Robust.Client.UserInterface.StylesheetHelpers;

namespace Content.Client.Stylesheets
{
    public sealed class StyleSpace : StyleBase
    {
        public const string StyleClassGlitchText = "GlitchText"; // New: For glitch effect on text elements

        public static readonly Color PanelDark = Color.FromHex("#0A0A0F"); // Darkened for cyberpunk void

        // Enhanced Cyberpunk / Glitch color palette
        public static readonly Color NanoGold = Color.FromHex("#FFD700"); // Brighter neon gold
        public static readonly Color NeonCyan = Color.FromHex("#00FFFF"); // Core cyberpunk cyan
        public static readonly Color NeonMagenta = Color.FromHex("#FF00FF"); // Vibrant magenta
        public static readonly Color NeonGreenFore = Color.FromHex("#39FF14"); // Acid green
        public static readonly Color NeonPinkFore = Color.FromHex("#FF1493"); // Hot pink
        public static readonly Color ConcerningOrangeFore = Color.FromHex("#FF4500"); // Fiery orange
        public static readonly Color DangerousRedFore = Color.FromHex("#FF0000"); // Blood red
        public static readonly Color GlitchOverlay = Color.FromHex("#00FF41"); // Glitch green overlay
        public static readonly Color DisabledFore = Color.FromHex("#333333"); // Muted gray

        public static readonly Color ButtonColorDefault = Color.FromHex("#0D1117"); // Deeper black
        public static readonly Color ButtonColorHovered = NeonCyan;
        public static readonly Color ButtonColorPressed = NeonMagenta;
        public static readonly Color ButtonColorDisabled = Color.FromHex("#0A0A0F");

        public static readonly Color ButtonColorCautionDefault = DangerousRedFore;
        public static readonly Color ButtonColorCautionHovered = NeonPinkFore;
        public static readonly Color ButtonColorCautionPressed = NeonMagenta;
        public static readonly Color ButtonColorCautionDisabled = Color.FromHex("#220000");

        public static readonly Color ButtonColorGoodDefault = NeonGreenFore;
        public static readonly Color ButtonColorGoodHovered = Color.FromHex("#00FF88");
        public static readonly Color ButtonColorGoodDisabled = Color.FromHex("#002200");

        public static readonly Color SpaceRed = DangerousRedFore; // Updated to cyberpunk red

        public override Stylesheet Stylesheet { get; }

        public StyleSpace(IResourceCache resCache) : base(resCache)
        {
            var notoSans10 = resCache.GetFont
            (
                new[]
                {
                    "/Fonts/NotoSans/NotoSans-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
                },
                10
            );
            var notoSansBold16 = resCache.GetFont
            (
                new[]
                {
                    "/Fonts/NotoSans/NotoSans-Bold.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols-Regular.ttf",
                    "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
                },
                16
            );

            var progressBarBackground = new StyleBoxFlat
            {
                BackgroundColor = new Color(0.1f, 0.1f, 0.1f) // Deeper black
            };
            progressBarBackground.SetContentMarginOverride(StyleBox.Margin.Vertical, 14.5f);

            var progressBarForeground = new StyleBoxFlat
            {
                BackgroundColor = new Color(NeonGreenFore.R * 0.8f, NeonGreenFore.G * 0.8f, NeonGreenFore.B * 0.8f, NeonGreenFore.A) // Semi-transparent neon
            };
            progressBarForeground.SetContentMarginOverride(StyleBox.Margin.Vertical, 14.5f);

            var textureInvertedTriangle = resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");

            var tabContainerPanel = new StyleBoxTexture();
            tabContainerPanel.SetPatchMargin(StyleBox.Margin.All, 2);

            var tabContainerBoxActive = new StyleBoxFlat { BackgroundColor = new Color(20, 20, 30) }; // Darker cyberpunk tab
            tabContainerBoxActive.SetContentMarginOverride(StyleBox.Margin.Horizontal, 5);
            var tabContainerBoxInactive = new StyleBoxFlat { BackgroundColor = new Color(10, 10, 20) };
            tabContainerBoxInactive.SetContentMarginOverride(StyleBox.Margin.Horizontal, 5);

            Stylesheet = new Stylesheet(BaseRules.Concat(new StyleRule[]
            {
                Element<Label>().Class(StyleClassLabelHeading)
                    .Prop(Label.StylePropertyFont, notoSansBold16)
                    .Prop(Label.StylePropertyFontColor, NanoGold), // Neon gold for headings

                Element<Label>().Class(StyleClassLabelSubText)
                    .Prop(Label.StylePropertyFont, notoSans10)
                    .Prop(Label.StylePropertyFontColor, Color.DarkGray),

                // New: Glitch text style
                Element<Label>().Class(StyleClassGlitchText)
                    .Prop(Label.StylePropertyFontColor, NanoGold),

                Element<PanelContainer>().Class(ClassHighDivider)
                    .Prop(PanelContainer.StylePropertyPanel, new StyleBoxFlat
                    {
                        BackgroundColor = NanoGold, ContentMarginBottomOverride = 2, ContentMarginLeftOverride = 2
                    }),

                Element<PanelContainer>().Class(ClassLowDivider)
                    .Prop(PanelContainer.StylePropertyPanel, new StyleBoxFlat
                    {
                        BackgroundColor = new Color(NeonCyan.R * 0.5f, NeonCyan.G * 0.5f, NeonCyan.B * 0.5f, NeonCyan.A), // Semi-transparent neon divider
                        ContentMarginLeftOverride = 2,
                        ContentMarginBottomOverride = 2
                    }),

                // Shapes for the buttons.
                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Prop(ContainerButton.StylePropertyStyleBox, BaseButton),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Class(ButtonOpenRight)
                    .Prop(ContainerButton.StylePropertyStyleBox, BaseButtonOpenRight),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Class(ButtonOpenLeft)
                    .Prop(ContainerButton.StylePropertyStyleBox, BaseButtonOpenLeft),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Class(ButtonOpenBoth)
                    .Prop(ContainerButton.StylePropertyStyleBox, BaseButtonOpenBoth),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Class(ButtonSquare)
                    .Prop(ContainerButton.StylePropertyStyleBox, BaseButtonSquare),

                // Colors for the buttons with cyberpunk neon hovers.
                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Pseudo(ContainerButton.StylePseudoClassNormal)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorDefault),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Pseudo(ContainerButton.StylePseudoClassHover)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorHovered),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Pseudo(ContainerButton.StylePseudoClassPressed)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorPressed),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton)
                    .Pseudo(ContainerButton.StylePseudoClassDisabled)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorDisabled),

                // Colors for the caution buttons.
                Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                    .Pseudo(ContainerButton.StylePseudoClassNormal)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionDefault),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                    .Pseudo(ContainerButton.StylePseudoClassHover)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionHovered),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                    .Pseudo(ContainerButton.StylePseudoClassPressed)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionPressed),

                Element<ContainerButton>().Class(ContainerButton.StyleClassButton).Class(ButtonCaution)
                    .Pseudo(ContainerButton.StylePseudoClassDisabled)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorCautionDisabled),


                Element<Label>().Class(ContainerButton.StyleClassButton)
                    .Prop(Label.StylePropertyAlignMode, Label.AlignMode.Center),

                Element<PanelContainer>().Class(ClassAngleRect)
                    .Prop(PanelContainer.StylePropertyPanel, BaseAngleRect)
                    .Prop(Control.StylePropertyModulateSelf, Color.FromHex("#0A0A0F")),

                Child()
                    .Parent(Element<Button>().Class(ContainerButton.StylePseudoClassDisabled))
                    .Child(Element<Label>())
                    .Prop("font-color", Color.FromHex("#E5E5E581")),

                Element<ProgressBar>()
                    .Prop(ProgressBar.StylePropertyBackground, progressBarBackground)
                    .Prop(ProgressBar.StylePropertyForeground, progressBarForeground),

                // OptionButton
                Element<OptionButton>()
                    .Prop(ContainerButton.StylePropertyStyleBox, BaseButton),

                Element<OptionButton>().Pseudo(ContainerButton.StylePseudoClassNormal)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorDefault),

                Element<OptionButton>().Pseudo(ContainerButton.StylePseudoClassHover)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorHovered),

                Element<OptionButton>().Pseudo(ContainerButton.StylePseudoClassPressed)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorPressed),

                Element<OptionButton>().Pseudo(ContainerButton.StylePseudoClassDisabled)
                    .Prop(Control.StylePropertyModulateSelf, ButtonColorDisabled),

                Element<TextureRect>().Class(OptionButton.StyleClassOptionTriangle)
                    .Prop(TextureRect.StylePropertyTexture, textureInvertedTriangle),

                Element<Label>().Class(OptionButton.StyleClassOptionButton)
                    .Prop(Label.StylePropertyAlignMode, Label.AlignMode.Center),

                // TabContainer
                new StyleRule(new SelectorElement(typeof(TabContainer), null, null, null),
                    new[]
                    {
                        new StyleProperty(TabContainer.StylePropertyPanelStyleBox, tabContainerPanel),
                        new StyleProperty(TabContainer.StylePropertyTabStyleBox, tabContainerBoxActive),
                        new StyleProperty(TabContainer.StylePropertyTabStyleBoxInactive, tabContainerBoxInactive),
                    }),

            }).ToList());
        }
    }
}
