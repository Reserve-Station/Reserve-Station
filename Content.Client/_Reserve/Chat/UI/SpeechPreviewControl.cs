using Content.Client.Chat.UI;
using Content.Client.Popups;
using Content.Client.Stylesheets;
using Content.Goobstation.Common.CCVar;
using Content.Shared.CCVar;
using Content.Shared.Popups;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Utility;

namespace Content.Client._Reserve.Chat.UI;

public sealed class SpeechPreviewControl : BoxContainer
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private bool _runechat;
    private int _fontSize;
    private float _outline;
    private bool _fancyBubbles;
    private bool _nameBackground;

    public SpeechPreviewControl()
    {
        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;
        SeparationOverride = 6;

        IoCManager.InjectDependencies(this);
    }

    /// <summary>
    ///     Rebuilds the preview from the current values of the speech related CVars.
    /// </summary>
    public void RefreshFromConfig()
    {
        SetState(
            _cfg.GetCVar(GoobCVars.ChatRunechat),
            _cfg.GetCVar(GoobCVars.ChatRunechatFontSize),
            _cfg.GetCVar(GoobCVars.ChatRunechatOutlineScale),
            _cfg.GetCVar(CCVars.ChatEnableFancyBubbles),
            _cfg.GetCVar(CCVars.ChatFancyNameBackground));
    }

    /// <summary>
    ///     Rebuilds the preview from explicit values, for callers that preview unsaved settings.
    /// </summary>
    public void SetState(bool runechat, int fontSize, float outline, bool fancyBubbles, bool nameBackground)
    {
        _runechat = runechat;
        _fontSize = fontSize;
        _outline = outline;
        _fancyBubbles = fancyBubbles;
        _nameBackground = nameBackground;

        Rebuild();
    }

    private void Rebuild()
    {
        RemoveAllChildren();

        var speakerName = Loc.GetString("ui-options-runechat-preview-speaker-name");

        if (_runechat)
        {
            var nameColor = RunechatSpeechBubble.GetNameColor(speakerName);
            AddRunechatRow("normal", nameColor, whisper: false, loud: false);
            AddRunechatRow("loud", nameColor, whisper: false, loud: true);
            AddRunechatRow("whisper", nameColor, whisper: true, loud: false);
            AddRunechatRow("emote", nameColor, whisper: false, loud: false);
            AddRunechatRow("looc", RunechatSpeechBubble.LoocColor, whisper: false, loud: false);
        }
        else
        {
            var normalText = FormattedMessage.EscapeText(Loc.GetString("ui-options-runechat-preview-normal-text"));
            var loudText = $"[bold]{FormattedMessage.EscapeText(Loc.GetString("ui-options-runechat-preview-loud-text"))}[/bold]";
            var whisperText = $"[italic]{FormattedMessage.EscapeText(Loc.GetString("ui-options-runechat-preview-whisper-text"))}[/italic]";
            var emoteAction = Loc.GetString("ui-options-runechat-preview-emote-text").Trim().Trim('*').Trim();
            var emoteText = $"[italic]{FormattedMessage.EscapeText(speakerName)} {FormattedMessage.EscapeText(emoteAction)}[/italic]";
            var loocText = Loc.GetString("chat-manager-entity-looc-wrap-message",
                ("entityName", FormattedMessage.EscapeText(speakerName)),
                ("message", FormattedMessage.EscapeText(Loc.GetString("ui-options-runechat-preview-looc-text"))));

            AddSpeechBubbleRow("normal", speakerName, normalText, "sayBox");
            AddSpeechBubbleRow("loud", speakerName, loudText, "sayBox");
            AddSpeechBubbleRow("whisper", speakerName, whisperText, "whisperBox");
            AddPlainBubbleRow("emote", emoteText, "emoteBox");
            AddPlainBubbleRow("looc", loocText, "emoteBox", RunechatSpeechBubble.LoocColor);
        }

        AddPopupRow("small", PopupType.Small);
        AddPopupRow("medium", PopupType.MediumCaution);
        AddPopupRow("large", PopupType.LargeCaution);
    }

    private void AddRunechatRow(string key, Color color, bool whisper, bool loud)
    {
        var text = Loc.GetString($"ui-options-runechat-preview-{key}-text");
        var label = RunechatSpeechBubble.CreateLabel(text, color, whisper, loud, _fontSize, _outline);
        AddPreviewRow($"ui-options-runechat-preview-{key}", label);
    }

    private void AddSpeechBubbleRow(string key, string speaker, string text, string styleClass)
    {
        var bubble = _fancyBubbles
            ? CreateFancyBubble(speaker, text, styleClass)
            : CreatePlainBubble(text, styleClass);
        AddPreviewRow($"ui-options-runechat-preview-{key}", bubble);
    }

    private void AddPlainBubbleRow(string key, string text, string styleClass, Color? textColor = null)
    {
        var bubble = CreatePlainBubble(text, styleClass, textColor);
        AddPreviewRow($"ui-options-runechat-preview-{key}", bubble);
    }

    private void AddPopupRow(string key, PopupType type)
    {
        var (font, color, thickness) = PopupUIController.GetPopupStyle(type, _runechat, _fontSize, _outline);
        var label = new Label
        {
            Text = Loc.GetString($"ui-options-runechat-preview-popup-{key}-text"),
            FontOverride = font,
            FontColorOverride = color,
            OutlineThicknessOverride = thickness,
            OutlineColorOverride = Color.Black,
            Margin = new Thickness(MathF.Ceiling(thickness)),
        };

        AddPreviewRow($"ui-options-runechat-preview-popup-{key}", label);
    }

    private void AddPreviewRow(string labelKey, Control sample)
    {
        sample.VerticalAlignment = VAlignment.Center;
        sample.HorizontalAlignment = HAlignment.Center;

        var row = new BoxContainer
        {
            Orientation = LayoutOrientation.Horizontal,
            HorizontalExpand = true,
            MinHeight = 32,
        };

        row.AddChild(new Label
        {
            Text = Loc.GetString(labelKey),
            MinWidth = 160,
            VerticalAlignment = VAlignment.Center,
            StyleClasses = { StyleClass.LabelSubText },
        });

        var sampleBox = new BoxContainer
        {
            Orientation = LayoutOrientation.Horizontal,
            HorizontalExpand = true,
            Align = AlignMode.Center,
        };
        sampleBox.AddChild(sample);

        row.AddChild(sampleBox);
        AddChild(row);
    }

    private Control CreatePlainBubble(string text, string styleClass, Color? textColor = null)
    {
        var bgOpacity = _cfg.GetCVar(CCVars.SpeechBubbleBackgroundOpacity);

        var label = new RichTextLabel
        {
            MaxWidth = SpeechBubble.SpeechMaxWidth,
            HorizontalAlignment = HAlignment.Center,
        };
        var msg = new FormattedMessage();
        if (textColor.HasValue)
            msg.PushColor(textColor.Value);
        msg.AddMarkupPermissive(text);
        if (textColor.HasValue)
            msg.Pop();
        label.SetMessage(msg, SpeechBubble.AllowedTags);

        return new PanelContainer
        {
            StyleClasses = { "speechBox", styleClass },
            Children = { label },
            ModulateSelfOverride = Color.White.WithAlpha(bgOpacity),
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Center,
        };
    }

    private Control CreateFancyBubble(string speaker, string text, string styleClass, Color? textColor = null)
    {
        var bgOpacity = _cfg.GetCVar(CCVars.SpeechBubbleBackgroundOpacity);
        var textOpacity = _cfg.GetCVar(CCVars.SpeechBubbleTextOpacity);
        var speakerOpacity = _cfg.GetCVar(CCVars.SpeechBubbleSpeakerOpacity);

        var bubbleHeader = new RichTextLabel
        {
            ModulateSelfOverride = Color.White.WithAlpha(speakerOpacity),
            Margin = new Thickness(1, 1, 1, 1),
            HorizontalAlignment = HAlignment.Center,
        };
        var headerMsg = new FormattedMessage();
        headerMsg.AddMarkupPermissive($"[bold]{FormattedMessage.EscapeText(speaker)}[/bold]");
        bubbleHeader.SetMessage(headerMsg, SpeechBubble.AllowedTags);

        var bubbleContent = new RichTextLabel
        {
            ModulateSelfOverride = Color.White.WithAlpha(textOpacity),
            MaxWidth = SpeechBubble.SpeechMaxWidth,
            Margin = new Thickness(2, 6, 2, 2),
            StyleClasses = { "bubbleContent" },
            HorizontalAlignment = HAlignment.Center,
        };
        var contentMsg = new FormattedMessage();
        if (textColor.HasValue)
            contentMsg.PushColor(textColor.Value);
        contentMsg.AddMarkupPermissive(text);
        if (textColor.HasValue)
            contentMsg.Pop();
        bubbleContent.SetMessage(contentMsg, SpeechBubble.AllowedTags);

        var mainPanel = new PanelContainer
        {
            StyleClasses = { "speechBox", styleClass },
            Children = { bubbleContent },
            ModulateSelfOverride = Color.White.WithAlpha(bgOpacity),
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Bottom,
            Margin = new Thickness(4, 14, 4, 2),
        };

        var headerPanel = new PanelContainer
        {
            StyleClasses = { "speechBox", styleClass },
            Children = { bubbleHeader },
            ModulateSelfOverride = Color.White.WithAlpha(_nameBackground ? bgOpacity : 0f),
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Top,
        };

        return new PanelContainer
        {
            Children = { mainPanel, headerPanel },
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Center,
        };
    }
}
