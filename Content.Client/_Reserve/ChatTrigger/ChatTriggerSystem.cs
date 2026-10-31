using Content.Client._Reserve.ChatTrigger.UI;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared._Reserve.ChatTrigger;
using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client._Reserve.ChatTrigger;

public sealed class ChatTriggerSystem : EntitySystem
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly ReserveBrainrotClientSystem _reserveBrainrot = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private ChatUIController? _chatController;

    private GrammarnaziBotWindow? _grammarWindow;
    private BrainrotBotWindow? _brainrotWindow;

    private readonly List<CachedTrigger> _cachedGrammarTriggers = new();
    private readonly List<CachedTrigger> _cachedBrainrotTriggers = new();
    private readonly List<TriggerReplacementRule> _cachedGrammarReplacementRules = new();
    private readonly List<TriggerReplacementRule> _cachedBrainrotReplacementRules = new();

    private static readonly ChatSelectChannel IcChannels =
        ChatSelectChannel.Local |
        ChatSelectChannel.Whisper |
        ChatSelectChannel.Radio |
        ChatSelectChannel.Emotes |
        ChatSelectChannel.CollectiveMind |
        ChatSelectChannel.Telepathic;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);

        RebuildTriggerCaches();

        _chatController = _uiManager.GetUIController<ChatUIController>();
        _chatController.BeforeMessageSent += OnBeforeMessageSent;
    }

    public override void Shutdown()
    {
        base.Shutdown();

        if (_chatController != null)
            _chatController.BeforeMessageSent -= OnBeforeMessageSent;

        CloseWarningWindows();
        _grammarWindow = null;
        _brainrotWindow = null;
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
    {
        if (ev.ByType.ContainsKey(typeof(GrammarnaziBotTriggerPrototype)) ||
            ev.ByType.ContainsKey(typeof(BrainrotBotTriggerPrototype)))
        {
            RebuildTriggerCaches();
        }
    }

    private void RebuildTriggerCaches()
    {
        ChatTriggerHelper.ClearCache();
        _cachedGrammarReplacementRules.Clear();
        _cachedBrainrotReplacementRules.Clear();

        _cachedGrammarTriggers.Clear();
        foreach (var proto in _protoManager.EnumeratePrototypes<GrammarnaziBotTriggerPrototype>())
        {
            var cached = new CachedTrigger(proto.Trigger, proto.Replacement, proto.Description);
            _cachedGrammarTriggers.Add(cached);
            if (cached.ReplacementRule.HasReplacement)
                _cachedGrammarReplacementRules.Add(cached.ReplacementRule);
            ChatTriggerHelper.Precompile(proto.Trigger);
        }
        _cachedGrammarTriggers.Sort((a, b) => b.ReplacementRule.HasReplacement.CompareTo(a.ReplacementRule.HasReplacement));
        _cachedGrammarReplacementRules.Sort((a, b) => b.MaxTriggerLength.CompareTo(a.MaxTriggerLength));

        _cachedBrainrotTriggers.Clear();
        foreach (var proto in _protoManager.EnumeratePrototypes<BrainrotBotTriggerPrototype>())
        {
            var cached = new CachedTrigger(proto.Trigger, proto.Replacement, proto.Description);
            _cachedBrainrotTriggers.Add(cached);
            if (cached.ReplacementRule.HasReplacement)
                _cachedBrainrotReplacementRules.Add(cached.ReplacementRule);
            ChatTriggerHelper.Precompile(proto.Trigger);
        }
        _cachedBrainrotTriggers.Sort((a, b) => b.ReplacementRule.HasReplacement.CompareTo(a.ReplacementRule.HasReplacement));
        _cachedBrainrotReplacementRules.Sort((a, b) => b.MaxTriggerLength.CompareTo(a.MaxTriggerLength));
    }

    private void OnBeforeMessageSent(object? sender, ChatBeforeMessageSentEventArgs args)
    {
        if ((args.Channel & IcChannels) == 0)
        {
            CloseWarningWindows();
            return;
        }

        var text = args.Text;
        var box = args.Box;
        var channel = args.Channel;
        var rawText = args.RawText;

        foreach (var trigger in _cachedGrammarTriggers)
        {
            foreach (var variant in trigger.Variants)
            {
                if (!ChatTriggerHelper.MatchesTrigger(text, variant, out var matchedWord))
                    continue;

                args.Cancel = true;
                ShowGrammarWarning(box, channel, rawText, variant, matchedWord, Loc.GetString(trigger.Description), trigger.ReplacementRule);
                return;
            }
        }

        foreach (var trigger in _cachedBrainrotTriggers)
        {
            foreach (var variant in trigger.Variants)
            {
                if (!ChatTriggerHelper.MatchesTrigger(text, variant, out var matchedWord))
                    continue;

                args.Cancel = true;
                ShowBrainrotWarning(box, channel, rawText, variant, matchedWord, Loc.GetString(trigger.Description), trigger.ReplacementRule);
                return;
            }
        }

        foreach (var variants in _reserveBrainrot.ParsedVariants)
        {
            foreach (var variant in variants)
            {
                if (!ChatTriggerHelper.MatchesTrigger(text, variant, out var matchedWord, isDynamic: true))
                    continue;

                args.Cancel = true;
                ShowBrainrotWarning(box, channel, rawText, variant, matchedWord, Loc.GetString("brainrot-trigger-desc-generic"), null, isDynamic: true);
                return;
            }
        }

        // Passed all checks without triggers
        CloseWarningWindows();
    }

    private void ShowGrammarWarning(ChatBox box, ChatSelectChannel channel, string originalRawText, string variant, string triggerWord, string description, TriggerReplacementRule? rule)
    {
        if (_grammarWindow == null || _grammarWindow.Disposed)
            _grammarWindow = new GrammarnaziBotWindow();

        var canFix = rule is { HasReplacement: true } &&
                     ChatTriggerHelper.TryApplyReplacements(originalRawText, _cachedGrammarReplacementRules, out _, _random);
        _grammarWindow.SetCanFix(canFix);
        _grammarWindow.SetDescription(description);
        _grammarWindow.OnFix = () => FixMessage(box, _cachedGrammarReplacementRules);
        _grammarWindow.OnSendWithoutTrigger = () => SendWithoutTrigger(box, channel, originalRawText, variant, triggerWord);
        _grammarWindow.OnCancel = () =>
        {
            if (!box.Disposed)
                box.Focus();
        };
        _grammarWindow.OnSendAnyway = () =>
        {
            if (box.Disposed)
                return;

            var currentText = box.ChatInput.Input.Text;
            var ignoreChecks = currentText == originalRawText;
            _chatController?.SendMessage(box, channel, ignoreBeforeMessage: ignoreChecks);
        };

        box.ChatInput.Input.ReleaseKeyboardFocus();

        if (!_grammarWindow.IsOpen)
            _grammarWindow.OpenCentered();
        else
            _grammarWindow.MoveToFront();

        _grammarWindow.GrabKeyboardFocus();
    }

    private void ShowBrainrotWarning(ChatBox box, ChatSelectChannel channel, string originalRawText, string variant, string triggerWord, string description, TriggerReplacementRule? rule, bool isDynamic = false)
    {
        if (_brainrotWindow == null || _brainrotWindow.Disposed)
            _brainrotWindow = new BrainrotBotWindow();

        var canFix = rule is { HasReplacement: true } &&
                     ChatTriggerHelper.TryApplyReplacements(originalRawText, _cachedBrainrotReplacementRules, out _, _random);
        _brainrotWindow.SetCanFix(canFix);
        _brainrotWindow.SetTriggerWord(triggerWord);
        _brainrotWindow.SetDescription(description);
        _brainrotWindow.OnFix = () => FixMessage(box, _cachedBrainrotReplacementRules);
        _brainrotWindow.OnSendWithoutTrigger = () => SendWithoutTrigger(box, channel, originalRawText, variant, triggerWord, isDynamic);
        _brainrotWindow.OnCancel = () =>
        {
            if (!box.Disposed)
                box.Focus();
        };
        _brainrotWindow.OnSendAnyway = () =>
        {
            if (box.Disposed)
                return;

            var currentText = box.ChatInput.Input.Text;
            var ignoreChecks = currentText == originalRawText;
            _chatController?.SendMessage(box, channel, ignoreBeforeMessage: ignoreChecks);
        };

        box.ChatInput.Input.ReleaseKeyboardFocus();

        if (!_brainrotWindow.IsOpen)
            _brainrotWindow.OpenCentered();
        else
            _brainrotWindow.MoveToFront();

        _brainrotWindow.GrabKeyboardFocus();
    }

    private void SendWithoutTrigger(ChatBox box, ChatSelectChannel channel, string originalRawText, string variant, string triggerWord, bool isDynamic = false)
    {
        if (box.Disposed)
            return;

        var currentText = box.ChatInput.Input.Text;
        var textToStrip = string.IsNullOrEmpty(currentText) ? originalRawText : currentText;
        var strippedText = ChatTriggerHelper.RemoveTrigger(textToStrip, variant, triggerWord, isDynamic);

        var (_, content, _, _) = _chatController != null
            ? _chatController.SplitInputContents(strippedText)
            : (ChatSelectChannel.None, strippedText, null, null);

        if (!ChatTriggerHelper.HasContent(content))
        {
            box.ChatInput.Input.Clear();
            box.ChatInput.Input.ReleaseKeyboardFocus();
            return;
        }

        box.ChatInput.Input.SetText(strippedText);
        _chatController?.SendMessage(box, channel, ignoreBeforeMessage: true);
    }

    private void FixMessage(ChatBox box, IEnumerable<TriggerReplacementRule> rules)
    {
        if (box.Disposed)
            return;

        var currentText = box.ChatInput.Input.Text;
        if (ChatTriggerHelper.TryApplyReplacements(currentText, rules, out var fixedText, _random))
        {
            box.ChatInput.Input.SetText(fixedText);
        }

        box.Focus();
    }

    public void CloseWarningWindows()
    {
        if (_grammarWindow?.IsOpen == true)
            _grammarWindow.Close();
        if (_brainrotWindow?.IsOpen == true)
            _brainrotWindow.Close();
    }

    private sealed class CachedTrigger
    {
        public readonly LocId Description;
        public readonly string[] Variants;
        public readonly TriggerReplacementRule ReplacementRule;

        public CachedTrigger(string rawTrigger, string? rawReplacement, LocId description)
        {
            Description = description;
            Variants = ChatTriggerHelper.ParseTriggerVariants(rawTrigger);
            ReplacementRule = new TriggerReplacementRule(rawTrigger, rawReplacement);
        }
    }
}
