using Content.Client.Stylesheets.Palette;

namespace Content.Client.Stylesheets.Stylesheets;

public sealed partial class NanotrasenStylesheet
{
    public override ColorPalette PrimaryPalette => ReserveRedStylesheet.PrimaryPalette; // Reserve edit: ReserveRedStyle
    public override ColorPalette SecondaryPalette => ReserveRedStylesheet.SecondaryPalette; // Reserve edit: ReserveRedStyle
    public override ColorPalette PositivePalette => ReserveRedStylesheet.PositivePalette; // Reserve edit: ReserveRedStyle
    public override ColorPalette NegativePalette => ReserveRedStylesheet.NegativePalette; // Reserve edit: ReserveRedStyle
    public override ColorPalette HighlightPalette => ReserveRedStylesheet.HighlightPalette; // Reserve edit: ReserveRedStyle
}
