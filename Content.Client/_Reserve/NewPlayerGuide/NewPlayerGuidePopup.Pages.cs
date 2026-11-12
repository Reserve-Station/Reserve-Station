using Content.Client.Stylesheets;
using Content.Client.UserInterface.Screens;
using Content.Goobstation.Common.CCVar;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Reserve.NewPlayerGuide;

public sealed partial class NewPlayerGuidePopup
{
    private void BuildPages()
    {
        AddPage(BuildWelcomePage());
        AddPage(BuildSpeechPage());
        AddPage(BuildHudPage());
        AddPage(BuildMoodPage());
    }

    private Control BuildWelcomePage()
    {
        return BuildPage(
            "new-player-guide-page-welcome-title",
            "new-player-guide-page-welcome-text");
    }

    private Control BuildSpeechPage()
    {
        var runechat = _cfg.GetCVar(GoobCVars.ChatRunechat);

        var runechatOption = BuildChoiceOption(
            "new-player-guide-speech-runechat",
            "new-player-guide-speech-runechat-desc",
            () => ApplySpeechStyle(true));
        var bubblesOption = BuildChoiceOption(
            "new-player-guide-speech-bubbles",
            "new-player-guide-speech-bubbles-desc",
            () => ApplySpeechStyle(false));

        _slidersCard = BuildCard(BuildSpeechSliders());
        _bubblesCard = BuildCard(BuildBubblesOptions());
        _slidersCard.Visible = runechat;
        _bubblesCard.Visible = !runechat;

        var preview = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            HorizontalExpand = true,
            SeparationOverride = 8,
        };

        preview.AddChild(new Label
        {
            Text = Loc.GetString("new-player-guide-speech-preview-title"),
            StyleClasses = { StyleClass.LabelSubText },
        });
        preview.AddChild(_speechPreview);

        var content = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            HorizontalExpand = true,
            SeparationOverride = 8,
        };

        content.AddChild(BuildChoiceRow(runechatOption, bubblesOption, runechat));
        content.AddChild(_slidersCard);
        content.AddChild(_bubblesCard);
        content.AddChild(BuildCard(preview));

        _speechPreview.RefreshFromConfig();

        return BuildPage(
            "new-player-guide-page-speech-title",
            "new-player-guide-page-speech-text",
            content);
    }

    private void ApplySpeechStyle(bool runechat)
    {
        SetSetting(GoobCVars.ChatRunechat, runechat);

        if (_slidersCard != null)
            _slidersCard.Visible = runechat;
        if (_bubblesCard != null)
            _bubblesCard.Visible = !runechat;

        _speechPreview.RefreshFromConfig();
    }

    private Control BuildHudPage()
    {
        var layout = _cfg.GetCVar(CCVars.UILayout);
        var separated = string.Equals(layout, nameof(ScreenType.Separated), StringComparison.OrdinalIgnoreCase);

        var defaultOption = BuildChoiceOption(
            "ui-options-hud-layout-default",
            "new-player-guide-hud-standard-desc",
            () => ApplyHudLayout(ScreenType.Default));
        var separatedOption = BuildChoiceOption(
            "ui-options-hud-layout-separated",
            "new-player-guide-hud-separated-desc",
            () => ApplyHudLayout(ScreenType.Separated));

        return BuildPage(
            "new-player-guide-page-hud-title",
            "new-player-guide-page-hud-text",
            BuildChoiceRow(defaultOption, separatedOption, !separated));
    }

    private void ApplyHudLayout(ScreenType layout)
    {
        SetSetting(CCVars.UILayout, layout.ToString());
    }

    private Control BuildMoodPage()
    {
        var moodEffects = _cfg.GetCVar(CCVars.MoodVisualEffects);

        var enabledOption = BuildChoiceOption(
            "new-player-guide-mood-enabled",
            "new-player-guide-mood-enabled-desc",
            () => ApplyMoodEffects(true));
        var disabledOption = BuildChoiceOption(
            "new-player-guide-mood-disabled",
            "new-player-guide-mood-disabled-desc",
            () => ApplyMoodEffects(false));

        var content = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            HorizontalExpand = true,
            SeparationOverride = 8,
        };

        content.AddChild(BuildChoiceRow(enabledOption, disabledOption, moodEffects));
        content.AddChild(BuildMoodInfoCard());

        return BuildPage(
            "new-player-guide-page-mood-title",
            "new-player-guide-page-mood-text",
            content);
    }

    private void ApplyMoodEffects(bool enabled)
    {
        SetSetting(CCVars.MoodVisualEffects, enabled);
    }
}
