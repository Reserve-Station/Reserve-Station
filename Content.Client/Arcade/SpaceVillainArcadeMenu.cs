// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 mirrorcult <notzombiedude@gmail.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Numerics;
using Content.Shared.Arcade;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Arcade
{
    public sealed class SpaceVillainArcadeMenu : DefaultWindow
    {
        // Reserve edit start: space-vilian-fix
        private const float MessageMaxWidth = 420f;
        private const float MinWindowWidth = 360f;
        private const float MinWindowHeight = 200f;
        private const float ContentsHorizontalMargin = 20f;
        private const float WindowHeaderHeight = 25f;
        private const float ContentsVerticalMargin = 20f;
        private const float HeightSafetyPadding = 16f;

        private readonly BoxContainer _contentRoot;
        private readonly BoxContainer _infoRow;
        private readonly RichTextLabel _enemyNameLabel;
        private readonly Label _playerInfoLabel;
        private readonly Label _enemyInfoLabel;
        private readonly RichTextLabel _playerActionLabel;
        private readonly RichTextLabel _enemyActionLabel;
        private readonly BoxContainer _playerActionSlot;
        private readonly BoxContainer _enemyActionSlot;
        private readonly Button[] _gameButtons = new Button[3];

        private float? _stableWidth;
        private string _gameTitle = string.Empty;
        private string _enemyName = string.Empty;

        public event Action<SharedSpaceVillainArcadeComponent.PlayerAction>? OnPlayerAction;

        public SpaceVillainArcadeMenu()
        {
            MinSize = new Vector2(MinWindowWidth, MinWindowHeight);
            Title = Loc.GetString("spacevillain-menu-title");

            _contentRoot = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
                SeparationOverride = 6,
                HorizontalExpand = true,
            };

            _infoRow = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Horizontal,
                SeparationOverride = 8,
                HorizontalExpand = true,
            };

            var playerColumn = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
                HorizontalExpand = true,
                HorizontalAlignment = HAlignment.Center,
            };
            playerColumn.AddChild(new Label
            {
                Text = Loc.GetString("spacevillain-menu-label-player"),
                Align = Label.AlignMode.Center,
                HorizontalExpand = true,
            });
            _playerInfoLabel = new Label
            {
                Align = Label.AlignMode.Center,
                HorizontalExpand = true,
            };
            playerColumn.AddChild(_playerInfoLabel);

            var enemyColumn = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
                HorizontalExpand = true,
                HorizontalAlignment = HAlignment.Center,
            };
            _enemyNameLabel = new RichTextLabel
            {
                HorizontalExpand = true,
                HorizontalAlignment = HAlignment.Center,
            };
            enemyColumn.AddChild(_enemyNameLabel);
            _enemyInfoLabel = new Label
            {
                Align = Label.AlignMode.Center,
                HorizontalExpand = true,
            };
            enemyColumn.AddChild(_enemyInfoLabel);

            _infoRow.AddChild(playerColumn);
            _infoRow.AddChild(new Label { Text = "|", Align = Label.AlignMode.Center, VAlign = Label.VAlignMode.Center });
            _infoRow.AddChild(enemyColumn);
            _contentRoot.AddChild(_infoRow);

            _playerActionSlot = CreateActionSlot(out _playerActionLabel);
            _contentRoot.AddChild(_playerActionSlot);

            _enemyActionSlot = CreateActionSlot(out _enemyActionLabel);
            _contentRoot.AddChild(_enemyActionSlot);

            var buttonRow = new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Horizontal,
                SeparationOverride = 8,
                HorizontalExpand = true,
            };

            _gameButtons[0] = new Button
            {
                Text = Loc.GetString("spacevillain-menu-button-attack"),
                HorizontalExpand = true,
            };
            _gameButtons[0].OnPressed +=
                _ => OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.Attack);
            buttonRow.AddChild(_gameButtons[0]);

            _gameButtons[1] = new Button
            {
                Text = Loc.GetString("spacevillain-menu-button-heal"),
                HorizontalExpand = true,
            };
            _gameButtons[1].OnPressed +=
                _ => OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.Heal);
            buttonRow.AddChild(_gameButtons[1]);

            _gameButtons[2] = new Button
            {
                Text = Loc.GetString("spacevillain-menu-button-recharge"),
                HorizontalExpand = true,
            };
            _gameButtons[2].OnPressed +=
                _ => OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.Recharge);
            buttonRow.AddChild(_gameButtons[2]);

            _contentRoot.AddChild(buttonRow);

            var newGame = new Button
            {
                Text = Loc.GetString("spacevillain-menu-button-new-game"),
                HorizontalExpand = true,
            };
            newGame.OnPressed += _ => OnPlayerAction?.Invoke(SharedSpaceVillainArcadeComponent.PlayerAction.NewGame);
            _contentRoot.AddChild(newGame);

            Contents.AddChild(_contentRoot);
            _gameTitle = Loc.GetString("spacevillain-menu-title");
            UserInterfaceManager.DeferAction(() =>
            {
                EnsureStableWidth(_gameTitle, string.Empty);
                ApplyWindowHeight(string.Empty, string.Empty);
            });
        }

        private static RichTextLabel CreateActionLabel()
        {
            return new RichTextLabel
            {
                MaxWidth = MessageMaxWidth,
                HorizontalExpand = true,
                HorizontalAlignment = HAlignment.Center,
            };
        }

        private static BoxContainer CreateActionSlot(out RichTextLabel label)
        {
            label = CreateActionLabel();
            return new BoxContainer
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
                HorizontalExpand = true,
                Children = { label },
            };
        }

        private float EnsureStableWidth(string gameTitle, string enemyName)
        {
            if (_stableWidth != null)
                return _stableWidth.Value;

            var sizingEnemyName = GetSizingEnemyName(enemyName);
            Title = gameTitle;
            var enemyNameForMeasure = string.IsNullOrEmpty(enemyName) ? sizingEnemyName : enemyName;

            _contentRoot.InvalidateMeasure();
            _contentRoot.Measure(Vector2Helpers.Infinity);

            var titleWidth = MeasureTextWidth(gameTitle) + 40;
            var maxStatsWidth = MeasureTextWidth(Loc.GetString("spacevillain-menu-label-stats", ("hp", 45), ("mp", 20)));
            var playerColumnWidth = MathF.Max(MeasureTextWidth(Loc.GetString("spacevillain-menu-label-player")), maxStatsWidth);
            var enemyColumnWidth = MathF.Max(MeasureEnemyNameWidth(enemyNameForMeasure), maxStatsWidth);
            var infoRowWidth = playerColumnWidth + MeasureTextWidth("|") + enemyColumnWidth + 8;
            var width = MathF.Max(
                MathF.Max(_contentRoot.DesiredSize.X, titleWidth),
                MathF.Max(MessageMaxWidth + ContentsHorizontalMargin, infoRowWidth));

            _stableWidth = width;
            MinSize = new Vector2(MinWindowWidth, MinWindowHeight);
            return width;
        }

        private void ApplyWindowHeight(string playerAction, string enemyAction)
        {
            var width = EnsureStableWidth(_gameTitle, _enemyName);
            Title = _gameTitle;
            var contentWidth = width - ContentsHorizontalMargin;
            var enemyColumnWidth = (contentWidth - MeasureTextWidth("|") - 8) / 2f;

            if (!string.IsNullOrEmpty(_enemyName))
            {
                _enemyNameLabel.MaxWidth = enemyColumnWidth;
                SetEnemyName(_enemyName);
            }

            _playerActionSlot.MinHeight = MeasureActionLabelHeight(playerAction, contentWidth);
            _enemyActionSlot.MinHeight = MeasureActionLabelHeight(enemyAction, contentWidth);

            _playerActionLabel.SetHeight = float.NaN;
            _enemyActionLabel.SetHeight = float.NaN;

            _contentRoot.InvalidateMeasure();
            _contentRoot.Measure(new Vector2(contentWidth, float.MaxValue));
            var contentHeight = _contentRoot.DesiredSize.Y;

            var targetHeight = MathF.Max(
                MinWindowHeight,
                WindowHeaderHeight + ContentsVerticalMargin + contentHeight + HeightSafetyPadding);

            SetSize = new Vector2(width, targetHeight);
        }

        private void SetEnemyName(string name)
        {
            _enemyNameLabel.SetMessage(name);
        }

        private static float MeasureActionLabelHeight(string message, float contentWidth)
        {
            if (string.IsNullOrWhiteSpace(message))
                return 0;

            var measureLabel = CreateActionLabel();
            measureLabel.SetMessage(message);
            measureLabel.Measure(new Vector2(contentWidth, float.MaxValue));
            return measureLabel.DesiredSize.Y + 4f;
        }

        private static string GetSizingEnemyName(string enemyName)
        {
            if (!string.IsNullOrEmpty(enemyName))
                return enemyName;

            var longestFirst = GetLongestLocString(
                "space-villain-game-enemy-first-the-automatic",
                "space-villain-game-enemy-first-farmer",
                "space-villain-game-enemy-first-lord",
                "space-villain-game-enemy-first-professor",
                "space-villain-game-enemy-first-the-cuban",
                "space-villain-game-enemy-first-the-evil",
                "space-villain-game-enemy-first-the-dread-king",
                "space-villain-game-enemy-first-the-space",
                "space-villain-game-enemy-first-the-great",
                "space-villain-game-enemy-first-duke",
                "space-villain-game-enemy-first-general");

            var longestLast = GetLongestLocString(
                "space-villain-game-enemy-last-melonoid",
                "space-villain-game-enemy-last-murdertron",
                "space-villain-game-enemy-last-sorcerer",
                "space-villain-game-enemy-last-ruin",
                "space-villain-game-enemy-last-jeff",
                "space-villain-game-enemy-last-ectoplasm",
                "space-villain-game-enemy-last-crushulon",
                "space-villain-game-enemy-last-uhangoid",
                "space-villain-game-enemy-last-vhakoid",
                "space-villain-game-enemy-last-peteoid",
                "space-villain-game-enemy-last-slime",
                "space-villain-game-enemy-last-griefer",
                "space-villain-game-enemy-last-erper",
                "space-villain-game-enemy-last-lizard-man",
                "space-villain-game-enemy-last-unicorn");

            return $"{longestFirst} {longestLast}";
        }

        private static string GetLongestLocString(params string[] keys)
        {
            var longest = string.Empty;
            var maxWidth = 0f;

            foreach (var key in keys)
            {
                var text = Loc.GetString(key);
                var width = MeasureTextWidth(text);
                if (width <= maxWidth)
                    continue;

                maxWidth = width;
                longest = text;
            }

            return longest;
        }

        private static float MeasureTextWidth(string text)
        {
            var label = new Label { Text = text };
            label.Measure(Vector2Helpers.Infinity);
            return label.DesiredSize.X;
        }

        private static float MeasureEnemyNameWidth(string name)
        {
            var measureLabel = new RichTextLabel
            {
                HorizontalExpand = true,
                HorizontalAlignment = HAlignment.Center,
            };
            measureLabel.SetMessage(name);
            measureLabel.Measure(Vector2Helpers.Infinity);
            return measureLabel.DesiredSize.X;
        }

        private void UpdateMetadata(SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage message)
        {
            _gameTitle = message.GameTitle;
            _enemyName = message.EnemyName;
            SetEnemyName(message.EnemyName);

            foreach (var gameButton in _gameButtons)
            {
                gameButton.Disabled = message.ButtonsDisabled;
            }
        }

        public void UpdateInfo(SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage message)
        {
            if (message is SharedSpaceVillainArcadeComponent.SpaceVillainArcadeMetaDataUpdateMessage metaMessage)
                UpdateMetadata(metaMessage);

            _playerInfoLabel.Text = Loc.GetString("spacevillain-menu-label-stats", ("hp", message.PlayerHP), ("mp", message.PlayerMP));
            _enemyInfoLabel.Text = Loc.GetString("spacevillain-menu-label-stats", ("hp", message.EnemyHP), ("mp", message.EnemyMP));
            _playerActionLabel.SetMessage(message.PlayerActionMessage);
            _enemyActionLabel.SetMessage(message.EnemyActionMessage);

            ApplyWindowHeight(message.PlayerActionMessage, message.EnemyActionMessage);
        }
        // Reserve edit end: space-vilian-fix
    }
}
