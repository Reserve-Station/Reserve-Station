// SPDX-License-Identifier: AGPL-3.0-or-later

// based on https://github.com/space-wizards/space-station-14/pull/34600
using Content.Shared.Atmos;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Localizations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.Botany.PlantAnalyzer;

public sealed class PlantAnalyzerLocalizationHelper
{
    public static string GasesToLocalizedStrings(List<Gas> gases, IPrototypeManager protMan)
    {
        if (gases.Count == 0)
            return "";

        List<string> gasesLoc = [];
        foreach (var gas in gases)
        {
            var gasId = ((int)gas).ToString();
            if (protMan.TryIndex<GasPrototype>(gasId, out var prototype))
                gasesLoc.Add(Loc.GetString(prototype.Name));
        }

        return ContentLocalizationManager.FormatList(gasesLoc);
    }

    public static string ChemicalsToLocalizedStrings(List<string> ids, IPrototypeManager protMan)
    {
        if (ids.Count == 0)
            return "";

        List<string> locStrings = [];
        foreach (var id in ids)
            locStrings.Add(protMan.TryIndex<ReagentPrototype>(id, out var prototype) ? prototype.LocalizedName : id);

        return ContentLocalizationManager.FormatList(locStrings);
    }

    public static (string Singular, string Plural) ProduceToLocalizedStrings(List<string> ids, IPrototypeManager protMan)
    {
        if (ids.Count == 0)
            return ("", "");

        List<string> singularStrings = [];
        List<string> pluralStrings = [];
        foreach (var id in ids)
        {
            var singular = protMan.TryIndex<EntityPrototype>(id, out var prototype) ? prototype.Name : id;
            var plural = Loc.GetString("plant-analyzer-produce-plural", ("thing", singular));

            singularStrings.Add(singular);
            pluralStrings.Add(plural);
        }

        return (
            ContentLocalizationManager.FormatListToOr(singularStrings),
            ContentLocalizationManager.FormatListToOr(pluralStrings)
        );
    }
}

