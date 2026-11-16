using Robust.Client.Graphics;
using Robust.Client.UserInterface;

namespace Content.Client.Chat.UI;

public static class RunechatStyle
{
    public const string FontPath = "/Fonts/_Reserve/Grand9KPixelUnicode.otf";  // Reserve edit: Cyrillic Runechat support
    public const string WhisperFontPath = "/Fonts/_Reserve/Grand9KPixelUnicode.otf";  // Reserve edit: Cyrillic Runechat support
    public const int MinFontSize = 6;
    public const int MaxFontSize = 24;
    private const float DesignPixelsPerEm = 8f;

    public static Font GetFont(string path, int size)
    {
        return IoCManager.Resolve<RunechatFontCache>().GetFont(path, size);
    }

    public static Stylesheet GetStylesheet(string path, int size)
    {
        return IoCManager.Resolve<RunechatFontCache>().GetStylesheet(path, size);
    }

    public static int ScaleSize(int baseSize, int fontSize)
    {
        return Math.Max(1, (int) MathF.Round(baseSize * fontSize / 12f));
    }

    public static int GetWhisperSize(int fontSize)  // Reserve edit: Cyrillic Runechat support
    {
        return ScaleSize(10, fontSize);
    }

    public static int GetLoudSize(int fontSize)
    {
        return ScaleSize(18, fontSize);
    }

    public static float GetOutline(int pointSize, float scale)
    {
        return pointSize * 96f / 72f / DesignPixelsPerEm * scale;
    }
}
