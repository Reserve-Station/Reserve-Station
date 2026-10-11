using Content.Client._Reserve.Brainrot;
using Content.Client._Reserve.ChatTrigger.UI;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared._Reserve.ChatTrigger;
using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client._Reserve.ChatTrigger;

public sealed class ChatTriggerSystem : EntitySystem
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly ReserveBrainrotClientSystem _reserveBrainrot = default!;

    private ChatUIController? _chatController;

    private GrammarnaziBotWindow? _grammarWindow;
    private BrainrotBotWindow? _brainrotWindow;

    private readonly List<CachedTrigger> _cachedGrammarTriggers = new();
    private readonly List<CachedTrigger> _cachedBrainrotTriggers = new();

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
        _cachedGrammarTriggers.Clear();
        foreach (var proto in _protoManager.EnumeratePrototypes<GrammarnaziBotTriggerPrototype>())
        {
            _cachedGrammarTriggers.Add(new CachedTrigger(proto.Trigger, proto.Description));
        }

        _cachedBrainrotTriggers.Clear();
        foreach (var proto in _protoManager.EnumeratePrototypes<BrainrotBotTriggerPrototype>())
        {
            _cachedBrainrotTriggers.Add(new CachedTrigger(proto.Trigger, proto.Description));
        }
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
                if (!ChatTriggerHelper.MatchesSingleTrigger(text, variant))
                    continue;

                args.Cancel = true;
                ShowGrammarWarning(box, channel, rawText, Loc.GetString(trigger.Description));
                return;
            }
        }

        foreach (var trigger in _cachedBrainrotTriggers)
        {
            foreach (var variant in trigger.Variants)
            {
                if (!ChatTriggerHelper.MatchesSingleTrigger(text, variant))
                    continue;

                args.Cancel = true;
                ShowBrainrotWarning(box, channel, rawText, variant, Loc.GetString(trigger.Description));
                return;
            }
        }

        foreach (var variants in _reserveBrainrot.ParsedVariants)
        {
            foreach (var variant in variants)
            {
                if (!ChatTriggerHelper.MatchesSingleTrigger(text, variant))
                    continue;

                args.Cancel = true;
                ShowBrainrotWarning(box, channel, rawText, variant, Loc.GetString("brainrot-trigger-desc-generic"));
                return;
            }
        }

        // Passed all checks without triggers
        CloseWarningWindows();
    }

    private void ShowGrammarWarning(ChatBox box, ChatSelectChannel channel, string originalRawText, string description)
    {
        if (_grammarWindow == null || _grammarWindow.Disposed)
            _grammarWindow = new GrammarnaziBotWindow();

        _grammarWindow.SetDescription(description);
        _grammarWindow.OnSendAnyway = () =>
        {
            if (box.Disposed)
                return;

            var currentText = box.ChatInput.Input.Text;
            var ignoreChecks = currentText == originalRawText;
            _chatController?.SendMessage(box, channel, ignoreBeforeMessage: ignoreChecks);
        };

        if (!_grammarWindow.IsOpen)
            _grammarWindow.OpenCentered();
        else
            _grammarWindow.MoveToFront();
    }

    private void ShowBrainrotWarning(ChatBox box, ChatSelectChannel channel, string originalRawText, string triggerWord, string description)
    {
        if (_brainrotWindow == null || _brainrotWindow.Disposed)
            _brainrotWindow = new BrainrotBotWindow();

        _brainrotWindow.SetTriggerWord(triggerWord);
        _brainrotWindow.SetDescription(description);
        _brainrotWindow.OnSendAnyway = () =>
        {
            if (box.Disposed)
                return;

            var currentText = box.ChatInput.Input.Text;
            var ignoreChecks = currentText == originalRawText;
            _chatController?.SendMessage(box, channel, ignoreBeforeMessage: ignoreChecks);
        };

        if (!_brainrotWindow.IsOpen)
            _brainrotWindow.OpenCentered();
        else
            _brainrotWindow.MoveToFront();
    }

    public void CloseWarningWindows()
    {
        if (_grammarWindow?.IsOpen == true)
            _grammarWindow.Close();
        if (_brainrotWindow?.IsOpen == true)
            _brainrotWindow.Close();
    }

    private readonly struct CachedTrigger
    {
        public readonly LocId Description;
        public readonly string[] Variants;

        public CachedTrigger(string rawTrigger, LocId description)
        {
            Description = description;
            Variants = ChatTriggerHelper.ParseTriggerVariants(rawTrigger);
        }
    }
}
