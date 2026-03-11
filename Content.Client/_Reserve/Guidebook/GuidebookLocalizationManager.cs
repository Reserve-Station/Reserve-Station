// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Globalization;
using Content.Shared.Guidebook;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook;

/// <summary>
///     Manager for localizing guidebook prototypes.
///     Allows selecting different XML paths based on the current culture.
///     Uses a fallback mechanism: if a path for the current culture is not found,
///     the fallback culture (en-US) or default path is used.
/// </summary>
public sealed class GuidebookLocalizationManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private CultureInfo? _currentCulture;
    private CultureInfo? _fallbackCulture;

    /// <summary>
    ///     Initializes the manager, setting the current and fallback cultures.
    /// </summary>
    public void Initialize()
    {
        // Reserve - ru-RU as current
        _currentCulture = CultureInfo.GetCultureInfo("ru-RU");
        _fallbackCulture = CultureInfo.GetCultureInfo("en-US");
    }

    /// <summary>
    ///     Gets the GuideEntryPrototype for the specified ID.
    /// </summary>
    public GuideEntryPrototype? GetPrototype(string prototypeId)
    {
        if (_prototypeManager.TryIndex<GuideEntryPrototype>(prototypeId, out var prototype))
            return prototype;

        return null;
    }

    /// <summary>
    ///     Gets the XML file path for the specified prototype with localization applied.
    ///     First attempts to find the path for the current culture, then for the fallback culture.
    /// </summary>
    public ResPath? GetLocalizedPath(string prototypeId)
    {
        if (!_prototypeManager.TryIndex<GuideEntryPrototype>(prototypeId, out var prototype))
            return null;

        return prototype.GetLocalizedTextPath(_currentCulture?.Name);
    }

    /// <summary>
    ///     Sets the current culture.
    /// </summary>
    public void SetCulture(CultureInfo culture)
    {
        _currentCulture = culture;
    }

    /// <summary>
    ///     Sets the fallback culture.
    /// </summary>
    public void SetFallbackCulture(CultureInfo culture)
    {
        _fallbackCulture = culture;
    }

    /// <summary>
    ///     Gets all available cultures.
    /// </summary>
    public IEnumerable<string> GetAvailableCultures()
    {
        yield return _fallbackCulture?.Name ?? "en-US";

        if (_currentCulture != null && _currentCulture.Name != _fallbackCulture?.Name)
        {
            yield return _currentCulture.Name;
        }
    }
}
