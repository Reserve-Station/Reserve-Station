using System.Numerics;

namespace Content.Client._Reserve.ChatTrigger.UI;

public sealed class BrainrotBotWindow : ChatTriggerBaseWindow
{
    public BrainrotBotWindow()
    {
        Title = Loc.GetString("chat-trigger-brainrot-title");
        SetMessage(Loc.GetString("chat-trigger-brainrot-message"));
        MinSize = new Vector2(440, 260);
        SetSize = new Vector2(440, 460);
    }
}
