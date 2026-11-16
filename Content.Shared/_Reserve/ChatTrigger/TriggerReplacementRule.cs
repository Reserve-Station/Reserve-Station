using System.Text.RegularExpressions;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared._Reserve.ChatTrigger;

public sealed class TriggerReplacementRule
{
    public bool IsRegex { get; }
    public Regex? CompiledRegex { get; }
    public string[]? ReplacementOptions { get; }
    public string? SingleReplacement { get; }
    public List<(string Trigger, string? Replacement, string[]? RandomReplacements)>? VariantRules { get; }

    public int MaxTriggerLength => VariantRules is { Count: > 0 } ? VariantRules[0].Trigger.Length : 0;

    public TriggerReplacementRule(string rawTrigger, string? rawReplacement)
    {
        if (string.IsNullOrWhiteSpace(rawReplacement))
            return;

        var repVariants = ChatTriggerHelper.ParseTriggerVariants(rawReplacement);
        if (repVariants.Length == 0)
            return;

        if (ChatTriggerHelper.IsRegexTrigger(rawTrigger))
        {
            IsRegex = true;
            var pattern = rawTrigger.Substring(ChatTriggerHelper.RegexPrefix.Length).Trim();
            CompiledRegex = ChatTriggerHelper.GetOrCompileRegex(pattern);
            if (repVariants.Length == 1)
                SingleReplacement = repVariants[0];
            else
                ReplacementOptions = repVariants;
            return;
        }

        var trigVariants = ChatTriggerHelper.ParseTriggerVariants(rawTrigger);
        VariantRules = new List<(string Trigger, string? Replacement, string[]? RandomReplacements)>(trigVariants.Length);

        if (trigVariants.Length == repVariants.Length)
        {
            for (var i = 0; i < trigVariants.Length; i++)
            {
                VariantRules.Add((trigVariants[i], repVariants[i], null));
            }
        }
        else if (repVariants.Length == 1)
        {
            for (var i = 0; i < trigVariants.Length; i++)
            {
                VariantRules.Add((trigVariants[i], repVariants[0], null));
            }
        }
        else
        {
            for (var i = 0; i < trigVariants.Length; i++)
            {
                VariantRules.Add((trigVariants[i], null, repVariants));
            }
        }

        VariantRules.Sort((a, b) => b.Trigger.Length.CompareTo(a.Trigger.Length));
    }

    public bool HasReplacement => IsRegex
        ? (CompiledRegex != null && (SingleReplacement != null || (ReplacementOptions != null && ReplacementOptions.Length > 0)))
        : (VariantRules != null && VariantRules.Count > 0);

    public string Apply(string text, IRobustRandom? random = null)
    {
        if (!HasReplacement || string.IsNullOrEmpty(text))
            return text;

        if (IsRegex)
        {
            if (CompiledRegex == null)
                return text;

            var rep = SingleReplacement;
            if (rep == null && ReplacementOptions != null && ReplacementOptions.Length > 0)
            {
                rep = ReplacementOptions[GetRandomIndex(ReplacementOptions.Length, random)];
            }

            if (rep == null)
                return text;

            return ChatTriggerHelper.ReplaceTriggerRegex(text, CompiledRegex, rep);
        }

        if (VariantRules == null || VariantRules.Count == 0)
            return text;

        foreach (var rule in VariantRules)
        {
            var rep = rule.Replacement;
            if (rep == null && rule.RandomReplacements != null && rule.RandomReplacements.Length > 0)
            {
                rep = rule.RandomReplacements[GetRandomIndex(rule.RandomReplacements.Length, random)];
            }

            if (rep != null)
            {
                text = ChatTriggerHelper.ReplaceTriggerWord(text, rule.Trigger, rep);
            }
        }

        return text;
    }

    private static int GetRandomIndex(int count, IRobustRandom? random)
    {
        if (random != null)
            return random.Next(count);

        try
        {
            var resolved = IoCManager.Resolve<IRobustRandom>();
            return resolved.Next(count);
        }
        catch
        {
            // IoC not initialized
        }

        return System.Random.Shared.Next(count);
    }
}
