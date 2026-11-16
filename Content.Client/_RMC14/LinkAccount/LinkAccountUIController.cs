// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq; // Goob - ghost cosmetics
using Content.Client.Lobby;
using Content.Client.Lobby.UI;
using Content.Client.Message;
using Content.Goobstation.Common.CCVar;
using Content.Goobstation.Common.Patrons; // Goob - ooc icon selection
using Content.Goobstation.Shared.Patrons; // Goob - ooc icon selection
using Content.Goobstation.Shared.GhostCosmetics; // Goob - ghost cosmetics
using Content.Shared._RMC14.GhostColor; // Goob - ghost cosmetics
using Content.Shared._RMC14.LinkAccount;
using Content.Shared._White.CustomGhostSystem;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using Robust.Shared.Map; // Goob - ghost cosmetics
using Robust.Shared.Network;
using Robust.Shared.Prototypes; // Goob - ghost cosmetics
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Client.UserInterface.Controls; // Goob - ghost cosmetics
using static Robust.Client.UserInterface.Controls.BaseButton;
using static Robust.Client.UserInterface.Controls.LineEdit;
using static Robust.Client.UserInterface.Controls.TabContainer;

namespace Content.Client._RMC14.LinkAccount;

public sealed class LinkAccountUIController : UIController, IOnSystemChanged<LinkAccountSystem>
{
    [Dependency] private readonly IClipboardManager _clipboard = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!; // Goob - ghost cosmetics
    [Dependency] private readonly IEntityNetworkManager _entityNet = default!; // Goob - ghost cosmetics
    [Dependency] private readonly LinkAccountManager _linkAccount = default!;
    [Dependency] private readonly IClientPreferencesManager _preferencesManager = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!; // Goob - ghost cosmetics
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IUriOpener _uriOpener = default!;

    private LinkAccountWindow? _window;
    private PatronPerksWindow? _patronPerksWindow;
    private EntityUid? _cosmeticsPreviewGhost; // Goob - ghost cosmetics
    private string? _selectedOocIcon; // Goob - ooc icon selection
    private TimeSpan _disableUntil;

    private Guid _code;

    public override void Initialize()
    {
        _linkAccount.CodeReceived += OnCode;
        _linkAccount.Updated += OnUpdated;
    }

    private void OnCode(Guid code)
    {
        _code = code;

        if (_window == null)
            return;

        _window.CopyButton.Disabled = false;
    }

    private void OnUpdated()
    {
        if (UIManager.ActiveScreen is not LobbyGui gui)
            return;

        gui.CharacterPreview.PatronPerks.Visible = _linkAccount.CanViewPatronPerks();
    }

    private void OnLobbyMessageReceived(SharedRMCDisplayLobbyMessageEvent message)
    {
        if (UIManager.ActiveScreen is not LobbyGui gui)
            return;

        var user = FormattedMessage.EscapeText(message.User);
        var msg = FormattedMessage.EscapeText(message.Message);
        gui.LobbyMessageLabel.SetMarkupPermissive($"[font size=20]{Loc.GetString("rmc-ui-lobby-message-by")} [bold][color=#c23c2d]{user}[/color][/bold]\n{msg}[/font]");  // Reserve edit: Maecenas System
        gui.LobbyMessagePanel.Visible = true;  // Reserve edit: Maecenas System
    }

    public void ToggleWindow()
    {
        if (_window == null)
        {
            _window = new LinkAccountWindow();
            _window.OnClose += () => _window = null;
            _window.Label.SetMarkupPermissive($"{Loc.GetString("rmc-ui-link-discord-account-text")}");
            if (_linkAccount.Linked)
                _window.Label.SetMarkupPermissive($"{Loc.GetString("rmc-ui-link-discord-account-already-linked")}");  // Reserve edit: Maecenas System

            _window.CopyButton.Disabled = _linkAccount.Linked;  // Reserve edit: Maecenas System
            _window.CopyButton.Visible = !_linkAccount.Linked;  // Reserve edit: Maecenas System

            _window.CopyButton.OnPressed += _ =>
            {
                _clipboard.SetText(_code.ToString());
                _window.CopyButton.Text = Loc.GetString("rmc-ui-link-discord-account-copied");
                _window.CopyButton.Disabled = true;
                _disableUntil = _timing.RealTime.Add(TimeSpan.FromSeconds(3));
            };

            var messageLink = _config.GetCVar(GoobCVars.RMCDiscordAccountLinkingMessageLink);
            if (string.IsNullOrEmpty(messageLink))
            {
                _window.LinkButton.Visible = false;
                _window.CopyButton.RemoveStyleClass("OpenRight");
            }
            else
            {
                _window.LinkButton.Visible = true;
                _window.LinkButton.OnPressed += _ => _uriOpener.OpenUri(messageLink);
                _window.CopyButton.AddStyleClass("OpenRight");
            }

            _window.OpenCentered();

            if (_code == default)
                _window.CopyButton.Disabled = true;

            _net.ClientSendMessage(new LinkAccountRequestMsg());
            return;
        }

        _window.Close();
        _window = null;
    }

    public void TogglePatronPerksWindow()
    {
        if (_patronPerksWindow == null)
        {
            _patronPerksWindow = new PatronPerksWindow();
            _patronPerksWindow.OnClose += () =>
            {
                _patronPerksWindow = null;
                DeleteGhostCosmeticsPreview(); // Goob - ghost cosmetics
            };

            var tier = _linkAccount.Tier;

            // Reserve edit start: Maecenas System
            SetTabTitle(_patronPerksWindow.MainTab, Loc.GetString("rmc-ui-lobby-main"));
            SetTabVisible(_patronPerksWindow.MainTab, tier is not null);
            var tierName = string.Join(", ", _linkAccount.Tiers.Select(currentTier => currentTier.Tier));
            var tiersWithIconSelection = new[] {  // No easy way to check how many icons are in a tier, so we list them manually
                "Maecenas2",
                "Maecenas3",
                "Maecenas4",
                "Maecenas5",
                "MaecenasX",
                "Staff",
            };
            var maxTier = "MaecenasX";
            if (tier != null)
            {
                if (string.IsNullOrEmpty(tierName))
                    tierName = tier.Tier;

                var mainDescription = Loc.GetString("rmc-ui-lobby-main-description", ("tier", tierName));
                var lockedPerksDescription = "";

                if (tier.OocIcons && tiersWithIconSelection.Contains(tier.Id))
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ooc-icons");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ooc-icons");
                if (tier.ShowOnCredits)
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-credits");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-credits");
                if (tier.LobbyMessage)
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-lobby-message");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-lobby-message");
                if (tier.RoundEndShoutout)
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-shoutout");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-shoutout");
                if (tier.GhostColor)
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ghost-color");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ghost-color");
                if (tier.GhostCosmetics)
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ghost-cosmetics");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ghost-cosmetics");
                if (tier.GhostParticles)
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ghost-particles");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ghost-particles");
                if (tier.GhostCosmetics)
                    mainDescription = mainDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-sprite-export");
                else
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-sprite-export");

                if (!string.IsNullOrEmpty(lockedPerksDescription) && tiersWithIconSelection.Contains(tier.Id) && tier.Id != maxTier)
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ooc-icons-more");
                if (!string.IsNullOrEmpty(lockedPerksDescription) && tier.GhostCosmetics && tier.Id != maxTier)
                    lockedPerksDescription = lockedPerksDescription + "\n" + Loc.GetString("rmc-ui-lobby-main-ghost-cosmetics-more");

                if (!string.IsNullOrEmpty(lockedPerksDescription))
                    lockedPerksDescription = "\n" + Loc.GetString("rmc-ui-lobby-main-locked-perks") + lockedPerksDescription;
                _patronPerksWindow.MainTabLabel.SetMarkupPermissive(mainDescription + lockedPerksDescription);
            }
            // Reserve edit end: Maecenas System

            SetTabTitle(_patronPerksWindow.LobbyMessageTab, Loc.GetString("rmc-ui-lobby-message"));
            SetTabVisible(_patronPerksWindow.LobbyMessageTab, tier is { LobbyMessage: true });
            _patronPerksWindow.LobbyMessageSaveButton.OnPressed += OnLobbyMessageSave;

            if (_linkAccount.LobbyMessage?.Message is { } lobbyMessage)
                _patronPerksWindow.LobbyMessage.Text = lobbyMessage;

            SetTabTitle(_patronPerksWindow.ShoutoutTab, Loc.GetString("rmc-ui-shoutout"));
            SetTabVisible(_patronPerksWindow.ShoutoutTab, tier is { RoundEndShoutout: true });
            _patronPerksWindow.NTShoutoutSaveButton.OnPressed += OnNTShoutoutSave;

            if (_linkAccount.RoundEndShoutout?.NT is { } ntShoutout)
                _patronPerksWindow.NTShoutout.Text = ntShoutout;

            SetTabTitle(_patronPerksWindow.GhostColorTab, Loc.GetString("rmc-ui-ghost-color"));
            SetTabVisible(_patronPerksWindow.GhostColorTab, tier is { GhostColor: true } or { GhostCosmetics: true } or { GhostParticles: true });  // Reserve edit: Maecenas System
            _patronPerksWindow.GhostColorSliders.Color = _linkAccount.GhostColor ?? Color.White;
            _patronPerksWindow.GhostColorSliders.OnColorChanged += OnGhostColorChanged;
            _patronPerksWindow.GhostColorClearButton.OnPressed += OnGhostColorClear;
            _patronPerksWindow.GhostColorSaveButton.OnPressed += OnGhostColorSave;
            _patronPerksWindow.GhostColorLabel.Visible = tier is { GhostColor: true };  // Reserve edit: Maecenas System
            _patronPerksWindow.GhostColorSliders.Visible = tier is { GhostColor: true };  // Reserve edit: Maecenas System
            _patronPerksWindow.GhostColorClearButton.Visible = tier is { GhostColor: true };  // Reserve edit: Maecenas System
            _patronPerksWindow.GhostColorSaveButton.Visible = tier is { GhostColor: true };  // Reserve edit: Maecenas System

            // Goob start - ghost cosmetics
            // SetTabTitle(_patronPerksWindow.GhostCosmeticsTab, Loc.GetString("goob-ui-ghost-cosmetics"));  // Reserve edit: Maecenas System
            // SetTabVisible(_patronPerksWindow.GhostCosmeticsTab, tier is { GhostCosmetics: true } or { GhostParticles: true });  // Reserve edit: Maecenas System

            var cosmetics = _linkAccount.GhostCosmetics;
            _patronPerksWindow.GhostCosmeticsLabel.Visible = tier is { GhostCosmetics: true } or { GhostParticles: true };  // Reserve edit: Maecenas System
            _patronPerksWindow.GhostParticlesRow.Visible = tier is { GhostParticles: true };
            _patronPerksWindow.GhostHatRow.Visible = tier is { GhostCosmetics: true };
            _patronPerksWindow.GhostMaskRow.Visible = tier is { GhostCosmetics: true };
            _patronPerksWindow.GhostCosmeticsSaveButton.Visible = tier is { GhostCosmetics: true } or { GhostParticles: true };  // Reserve edit: Maecenas System
            PopulateGhostCosmetics(_patronPerksWindow.GhostParticlesButton, GhostCosmeticCategory.Particles, cosmetics?.Particles);
            PopulateGhostCosmetics(_patronPerksWindow.GhostHatButton, GhostCosmeticCategory.Hat, cosmetics?.Hat);
            PopulateGhostCosmetics(_patronPerksWindow.GhostMaskButton, GhostCosmeticCategory.Mask, cosmetics?.Mask);
            _patronPerksWindow.GhostCosmeticsSaveButton.OnPressed += OnGhostCosmeticsSave;

            /* Reserve edit: Maecenas System
            if (tier is { GhostCosmetics: true } or { GhostParticles: true })
            {
                _cosmeticsPreviewGhost = _entityManager.SpawnEntity("GhostCosmeticsPreviewDummy", MapCoordinates.Nullspace);
                if (_entityManager.TryGetComponent(_cosmeticsPreviewGhost.Value, out GhostColorComponent? previewColor))
                    previewColor.Color = _linkAccount.GhostColor;

                _patronPerksWindow.GhostCosmeticsPreview.SetEntity(_cosmeticsPreviewGhost);
                UpdateGhostCosmeticsPreview();
            }
            */
            _patronPerksWindow.GhostMaskButton.OnPressed += OnGhostCosmeticsChanged;  // Reserve edit: Maecenas System
            _patronPerksWindow.GhostHatButton.OnPressed += OnGhostCosmeticsChanged;  // Reserve edit: Maecenas System
            _patronPerksWindow.GhostParticlesButton.OnPressed += OnGhostCosmeticsChanged;  // Reserve edit: Maecenas System

            SetupGhostPreview();  // Reserve edit: Maecenas System
            // Goob end

            // Goob start - ooc icon selection
            SetTabTitle(_patronPerksWindow.OocIconTab, Loc.GetString("goob-ui-ooc-icon"));
            SetTabVisible(_patronPerksWindow.OocIconTab, tier is { OocIcons: true });
            if (tier is { OocIcons: true })
                PopulateOocIcons();  // Reserve edit: Maecenas System
            // Goob end

            UpdateExamples();

            for (var i = 0; i < _patronPerksWindow.Tabs.ChildCount; i++)
            {
                var child = _patronPerksWindow.Tabs.GetChild(i);
                if (!child.GetValue(TabVisibleProperty))
                    continue;

                _patronPerksWindow.Tabs.CurrentTab = i;
                break;
            }

            _patronPerksWindow.OpenCentered();
            return;
        }

        _patronPerksWindow.Close();
        _patronPerksWindow = null;
    }

    private void OnLobbyMessageSave(ButtonEventArgs args)
    {
        var text = _patronPerksWindow?.LobbyMessage.Text;
        if (text == null)
            return;

        if (text.Length > SharedRMCLobbyMessage.CharacterLimit)
        {
            text = text[..SharedRMCLobbyMessage.CharacterLimit];
            _patronPerksWindow?.LobbyMessage.SetText(text, false);
        }

        _net.ClientSendMessage(new RMCChangeLobbyMessageMsg { Text = text });
        _linkAccount.LobbyMessage = new SharedRMCLobbyMessage(text);  // Reserve edit: Maecenas System
    }

    private void OnNTShoutoutSave(ButtonEventArgs args)
    {
        var text = _patronPerksWindow?.NTShoutout.Text;
        if (text == null)
            return;

        if (text.Length > SharedRMCRoundEndShoutouts.CharacterLimit)
        {
            text = text[..SharedRMCRoundEndShoutouts.CharacterLimit];
            _patronPerksWindow?.NTShoutout.SetText(text, false);
        }

        _net.ClientSendMessage(new RMCChangeNTShoutoutMsg { Name = text });
        _linkAccount.RoundEndShoutout = new SharedRMCRoundEndShoutouts(text);  // Reserve edit: Maecenas System
        UpdateExamples();
    }

    private void OnGhostColorChanged(Color color)
    {
        if (_patronPerksWindow is not { IsOpen: true })
            return;

        _patronPerksWindow.GhostColorSaveButton.Disabled = false;
    }

    private void OnGhostColorClear(ButtonEventArgs args)
    {
        if (_patronPerksWindow is not { IsOpen: true })
            return;

        _patronPerksWindow.GhostColorSliders.Color = Color.White;
        _net.ClientSendMessage(new RMCClearGhostColorMsg());
        _linkAccount.GhostColor = null;  // Reserve edit: Maecenas System
        SetupGhostPreview();  // Reserve edit: Maecenas System
    }

    private void OnGhostColorSave(ButtonEventArgs args)
    {
        if (_patronPerksWindow is not { IsOpen: true })
            return;

        _net.ClientSendMessage(new RMCChangeGhostColorMsg { Color = _patronPerksWindow.GhostColorSliders.Color });
        _linkAccount.GhostColor = _patronPerksWindow.GhostColorSliders.Color;  // Reserve edit: Maecenas System
        SetupGhostPreview();  // Reserve edit: Maecenas System
    }

    // Goob start - ghost cosmetics
    private void PopulateGhostCosmetics(OptionButton button, GhostCosmeticCategory category, string? current)
    {
        button.Clear();
        button.AddItem(Loc.GetString("goob-ui-ghost-cosmetics-none"), 0);

        var cosmetics = _prototype.EnumeratePrototypes<GhostCosmeticPrototype>()
            .Where(cosmetic => cosmetic.Category == category)
            .OrderBy(cosmetic => Loc.GetString(cosmetic.Name))
            .ToList();

        for (var i = 0; i < cosmetics.Count; i++)
        {
            var cosmetic = cosmetics[i];
            button.AddItem(Loc.GetString(cosmetic.Name), i + 1);
            button.SetItemMetadata(button.ItemCount - 1, cosmetic.ID);

            if (cosmetic.ID == current)
                button.Select(button.ItemCount - 1);
        }

        button.OnItemSelected += args =>
        {
            button.SelectId(args.Id);
            UpdateGhostCosmeticsPreview();
        };
    }

    private void UpdateGhostCosmeticsPreview()
    {
        if (_patronPerksWindow == null || _cosmeticsPreviewGhost is not { } ghost)
            return;

        _entityManager.RemoveComponent<GhostCosmeticsComponent>(ghost);
        _entityManager.AddComponent(ghost, new GhostCosmeticsComponent
        {
            Hat = ToCosmeticProto(_patronPerksWindow.GhostHatButton.SelectedMetadata as string),
            Mask = ToCosmeticProto(_patronPerksWindow.GhostMaskButton.SelectedMetadata as string),
        });
    }

    private void DeleteGhostCosmeticsPreview()
    {
        _entityManager.DeleteEntity(_cosmeticsPreviewGhost);
        _cosmeticsPreviewGhost = null;
    }

    // Reserve edit start: Maecenas System
    private void SetupGhostPreview()
    {
        if (_cosmeticsPreviewGhost != null)
            DeleteGhostCosmeticsPreview();

        if (_patronPerksWindow != null && _linkAccount.Tier is { GhostCosmetics: true } or { GhostParticles: true } or { GhostColor: true })
        {
            var ghostProtoId = _preferencesManager.Preferences?.CustomGhost ?? new ProtoId<CustomGhostPrototype>("default");
            var ghostPrototype = _prototype.Index<CustomGhostPrototype>(ghostProtoId);
            _cosmeticsPreviewGhost = _entityManager.Spawn(ghostPrototype.GhostEntityPrototype, MapCoordinates.Nullspace);
            if (_entityManager.TryGetComponent(_cosmeticsPreviewGhost.Value, out GhostColorComponent? previewColor))
                previewColor.Color = _linkAccount.GhostColor;

            _patronPerksWindow.GhostCosmeticsPreview.SetEntity(_cosmeticsPreviewGhost);
            UpdateGhostCosmeticsPreview();
        }
    }

    private void OnGhostCosmeticsChanged(ButtonEventArgs args)
    {
        if (_patronPerksWindow is not { IsOpen: true })
            return;

        _patronPerksWindow.GhostCosmeticsSaveButton.Disabled = false;
    }
    // Reserve edit end: Maecenas System

    private void OnGhostCosmeticsSave(ButtonEventArgs args)
    {
        if (_patronPerksWindow is not { IsOpen: true })
            return;

        _entityNet.SendSystemNetworkMessage(new ChangeGhostCosmeticsEvent
        {
            Particles = ToCosmeticProto(_patronPerksWindow.GhostParticlesButton.SelectedMetadata as string),
            Hat = ToCosmeticProto(_patronPerksWindow.GhostHatButton.SelectedMetadata as string),
            Mask = ToCosmeticProto(_patronPerksWindow.GhostMaskButton.SelectedMetadata as string),
        });
        _linkAccount.GhostCosmetics = new SharedRMCGhostCosmetics(  // Reserve edit: Maecenas System
            ToCosmeticProto(_patronPerksWindow.GhostParticlesButton.SelectedMetadata as string),
            ToCosmeticProto(_patronPerksWindow.GhostHatButton.SelectedMetadata as string),
            ToCosmeticProto(_patronPerksWindow.GhostMaskButton.SelectedMetadata as string)
        );
    }

    private void PopulateOocIcons()  // Reserve edit: Maecenas System
    {
        if (_patronPerksWindow == null ||
            _linkAccount.Tier is not { } tier ||  // Reserve edit: Maecenas System
            !_prototype.TryIndex(tier.Id, out var tierProto))
            return;

        var window = _patronPerksWindow;
        window.OocIconList.RemoveAllChildren();
        window.OocIconPreview.DisplayRect.Stretch = TextureRect.StretchMode.Scale;
        _selectedOocIcon = _linkAccount.OocIcon;

        var group = new ButtonGroup();
        // Reserve edit start: Maecenas System
        var allowedIconTiers = new List<ProtoId<PatronTierPrototype>>();
        foreach (var currentTier in _linkAccount.Tiers)
        {
            if (!_prototype.TryIndex(currentTier.Id, out var currentTierProto))
                continue;

            foreach (var iconTier in currentTierProto.OocIconTiers)
            {
                if (!allowedIconTiers.Contains(iconTier))
                    allowedIconTiers.Add(iconTier);
            }
        }

        var icons = _prototype.EnumeratePrototypes<OocIconPrototype>()
            .Where(icon => allowedIconTiers.Contains(icon.Tier))
            .ToList();
        // Reserve edit end: Maecenas System

        foreach (var iconTier in allowedIconTiers)  // Reserve edit: Maecenas System
        {
            var tierIcons = icons
                .Where(icon => icon.Tier == iconTier)
                .OrderBy(icon => Loc.GetString(icon.Name))
                .ToList();

            if (tierIcons.Count == 0)
                continue;

            var heading = _prototype.TryIndex(iconTier, out var iconTierProto)
                ? Loc.GetString(iconTierProto.Name)
                : iconTier.Id;

            window.OocIconList.AddChild(new Label { Text = heading, StyleClasses = { "LabelHeading" }, Margin = new Thickness(0, 5, 0, 2) });
            var grid = new GridContainer { Columns = 12 };
            window.OocIconList.AddChild(grid);

            foreach (var icon in tierIcons)
            {
                var button = new Button
                {
                    ToggleMode = true,
                    Group = group,
                    SetSize = new System.Numerics.Vector2(40, 40),
                    ToolTip = Loc.GetString(icon.Name),
                    Pressed = icon.ID == _selectedOocIcon,
                };

                var iconRect = new AnimatedTextureRect
                {
                    SetSize = new System.Numerics.Vector2(24, 24),
                    HorizontalAlignment = Control.HAlignment.Center,
                    VerticalAlignment = Control.VAlignment.Center,
                };
                iconRect.DisplayRect.Stretch = TextureRect.StretchMode.Scale;
                iconRect.SetFromSpriteSpecifier(icon.Icon);
                button.AddChild(iconRect);

                var id = icon.ID;
                button.OnPressed += _ =>
                {
                    _selectedOocIcon = id;
                    UpdateOocIconPreview(tierProto);
                };

                grid.AddChild(button);
            }
        }

        window.OocIconDefaultButton.OnPressed += _ =>
        {
            _selectedOocIcon = null;
            foreach (var button in group.Buttons)
            {
                button.Pressed = false;
            }

            UpdateOocIconPreview(tierProto);
        };

        window.OocIconSaveButton.OnPressed += _ =>
        {
            _entityNet.SendSystemNetworkMessage(new ChangePatronOocIconEvent
            {
                Icon = _selectedOocIcon == null ? null : (ProtoId<OocIconPrototype>?) _selectedOocIcon,
            });
            window.OocIconSaveButton.Disabled = true;
        };

        UpdateOocIconPreview(tierProto, false);
    }

    private void UpdateOocIconPreview(PatronTierPrototype tier, bool changed = true)
    {
        if (_patronPerksWindow == null)
            return;

        var iconId = _selectedOocIcon ?? tier.Icon?.Id;
        if (iconId != null && _prototype.TryIndex<OocIconPrototype>(iconId, out var icon))
        {
            _patronPerksWindow.OocIconPreview.Visible = true;
            _patronPerksWindow.OocIconPreview.SetFromSpriteSpecifier(icon.Icon);
            _patronPerksWindow.OocIconSelectedName.Text = _selectedOocIcon == null
                ? Loc.GetString("goob-ui-ooc-icon-default-name", ("name", Loc.GetString(icon.Name)))
                : Loc.GetString(icon.Name);
        }
        else
        {
            _patronPerksWindow.OocIconPreview.Visible = false;
            _patronPerksWindow.OocIconSelectedName.Text = Loc.GetString("goob-ui-ooc-icon-none");
        }

        if (changed)
            _patronPerksWindow.OocIconSaveButton.Disabled = false;
    }

    private static ProtoId<GhostCosmeticPrototype>? ToCosmeticProto(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        return new ProtoId<GhostCosmeticPrototype>(id);
    }
    // Goob end

    private void UpdateExamples()
    {
        if (_patronPerksWindow == null)
            return;

        var nt = _patronPerksWindow.NTShoutout.Text.Trim();
        _patronPerksWindow.NTShoutoutExample.SetMarkupPermissive(string.IsNullOrWhiteSpace(nt)
            ? " "
            : $"{Loc.GetString("rmc-ui-shoutout-example")} {Loc.GetString("rmc-ui-shoutout-nt", ("name", nt))}");
    }

    public void OnSystemLoaded(LinkAccountSystem system)
    {
        system.LobbyMessageReceived += OnLobbyMessageReceived;
    }

    public void OnSystemUnloaded(LinkAccountSystem system)
    {
        system.LobbyMessageReceived -= OnLobbyMessageReceived;
    }

    public override void FrameUpdate(FrameEventArgs args)
    {
        if (_window == null)
            return;

        var time = _timing.RealTime;
        if (_disableUntil != default && time > _disableUntil)
        {
            _disableUntil = default;
            _window.CopyButton.Text = Loc.GetString("rmc-ui-link-discord-account-copy");
            _window.CopyButton.Disabled = false;
        }
    }
}
