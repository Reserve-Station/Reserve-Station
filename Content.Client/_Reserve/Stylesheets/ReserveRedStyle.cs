using Content.Client.Stylesheets.Palette;

namespace Content.Client.Stylesheets;

public sealed class ReserveRed : Shared.Stylesheets.ReserveRed
{
    // Palettes

    public static readonly ColorPalette RedPalette = ColorPalette.FromHexBase(Red.ToHex(), element: GrayRed);
    public static readonly ColorPalette DarkRedPalette = ColorPalette.FromHexBase(DarkRed.ToHex(), chromaShift: 0.02f);
}

public sealed partial class ReserveRedStylesheet
{
    public static ColorPalette PrimaryPalette => Palettes.Slate;
    public static ColorPalette SecondaryPalette => Palettes.Neutral;
    public static ColorPalette PositivePalette => ReserveRed.RedPalette;
    public static ColorPalette NegativePalette => ReserveRed.DarkRedPalette;
    public static ColorPalette HighlightPalette => ReserveRed.RedPalette;
}
