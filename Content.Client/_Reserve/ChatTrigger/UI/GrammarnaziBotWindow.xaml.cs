using System.Numerics;

namespace Content.Client._Reserve.ChatTrigger.UI;

public sealed class GrammarnaziBotWindow : ChatTriggerBaseWindow
{
    public GrammarnaziBotWindow()
    {
        Title = Loc.GetString("chat-trigger-grammarnazi-title");
        SetMessage(Loc.GetString("chat-trigger-grammarnazi-message"));
        MinSize = new Vector2(440, 240);
        SetSize = new Vector2(440, 240);
    }
}
