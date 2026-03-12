// SPDX-FileCopyrightText: 2023 Hebi <spiritbreakz@gmail.com>
// SPDX-FileCopyrightText: 2023 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 moonheart08 <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Guidebook;

[Prototype]
public sealed partial class GuideEntryPrototype : GuideEntry, IPrototype
{
    public string ID => Id;
}

[Virtual]
public class GuideEntry
{
    /// <summary>
    ///     The file containing the contents of this guide.
    ///     Used as fallback when LocalizedText is not specified.
    /// </summary>
    [DataField(required: true)]
    public ResPath Text = ResPath.Empty;

    // Reserve localized guidebook begin
    /// <summary>
    ///     Localized paths for different cultures.
    ///     If specified, overrides Text based on current culture.
    ///     Format: culture code -> path
    ///     Example YAML:
    ///       localizedText:
    ///         en-US: "/path/en.xml"
    ///         ru-RU: "/path/ru.xml"
    /// </summary>
    [DataField("localizedText")]
    public Dictionary<string, ResPath> LocalizedText { get; set; } = [];
    // Reserve localized guidebook end

    /// <summary>
    ///     The unique id for this guide.
    /// </summary>
    [IdDataField]
    public string Id = default!;

    /// <summary>
    ///     The name of this guide. This gets localized.
    /// </summary>
    [DataField(required: true)] public string Name = default!;

    /// <summary>
    ///     The "children" of this guide for when guides are shown in a tree / table of contents.
    /// </summary>
    [DataField]
    public List<ProtoId<GuideEntryPrototype>> Children = new();

    /// <summary>
    ///     Enable filtering of items.
    /// </summary>
    [DataField] public bool FilterEnabled = default!;

    [DataField] public bool RuleEntry;

    /// <summary>
    ///     Priority for sorting top-level guides when shown in a tree / table of contents.
    ///     If the guide is the child of some other guide, the order simply determined by the order of children in <see cref="Children"/>.
    /// </summary>
    [DataField] public int Priority = 0;

    // Reserve localized guidebook begin

    /// <summary>
    ///     Gets the appropriate text path for the specified culture.
    /// </summary>
    public ResPath GetLocalizedTextPath(string? culture)
    {
        // If LocalizedText has entries, try to get culture-specific path
        if (LocalizedText.Count > 0)
        {
            if (culture != null && LocalizedText.TryGetValue(culture, out var culturePath))
                return culturePath;

            // Fallback to en-US if available
            if (LocalizedText.TryGetValue("en-US", out var enUsPath))
                return enUsPath;
        }

        // Use Text field as fallback
        return Text;
    }
    // Reserve localized guidebook end
}
