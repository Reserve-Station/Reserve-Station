using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Robust.Shared.Log;

namespace Content.Shared._Reserve.ChatTrigger;

public static class ChatTriggerHelper
{
    public const string RegexPrefix = "regex:";

    public static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(250);

    private static readonly ConcurrentDictionary<string, Regex?> RegexCache = new();

    public static bool IsRegexTrigger([NotNullWhen(true)] string? trigger)
    {
        return trigger != null && trigger.StartsWith(RegexPrefix, StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeTrigger(string? trigger)
    {
        if (string.IsNullOrWhiteSpace(trigger))
            return string.Empty;

        var trimmed = trigger.Trim();
        if (IsRegexTrigger(trimmed))
        {
            var pattern = trimmed[RegexPrefix.Length..].Trim();
            if (string.IsNullOrEmpty(pattern))
                return string.Empty;

            return RegexPrefix + pattern;
        }

        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
        {
            var variants = ParseTriggerVariants(trimmed);
            if (variants.Length == 0)
                return string.Empty;

            var normalizedVariants = new List<string>(variants.Length);
            foreach (var variant in variants)
            {
                var norm = NormalizeTrigger(variant);
                if (!string.IsNullOrEmpty(norm))
                    normalizedVariants.Add(norm);
            }

            if (normalizedVariants.Count == 0)
                return string.Empty;

            return $"[{string.Join("; ", normalizedVariants)}]";
        }

        return trimmed.ToLowerInvariant();
    }

    public static bool IsValidTrigger(string? trigger, [NotNullWhen(false)] out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(trigger))
        {
            error = "Trigger cannot be empty";
            return false;
        }

        var normalized = NormalizeTrigger(trigger);
        if (string.IsNullOrEmpty(normalized))
        {
            error = "Trigger cannot be empty";
            return false;
        }

        if (normalized.Length > 512)
        {
            error = "Trigger exceeds maximum length of 512 characters";
            return false;
        }

        var variants = ParseTriggerVariants(normalized);
        if (variants.Length == 0)
        {
            error = "Trigger has no valid variants";
            return false;
        }

        foreach (var variant in variants)
        {
            if (string.IsNullOrWhiteSpace(variant))
            {
                error = "Trigger variant cannot be empty";
                return false;
            }

            if (IsRegexTrigger(variant))
            {
                var pattern = variant[RegexPrefix.Length..].Trim();
                if (string.IsNullOrEmpty(pattern))
                {
                    error = "Regex pattern cannot be empty";
                    return false;
                }

                if (!TryCompileRegex(pattern, out var compiled, out error))
                    return false;

                if (compiled.IsMatch(string.Empty))
                {
                    error = "Regex trigger pattern cannot match an empty string";
                    return false;
                }
            }
        }

        return true;
    }

    public static bool TryCompileRegex(string pattern, [NotNullWhen(true)] out Regex? regex, [NotNullWhen(false)] out string? error)
    {
        error = null;
        try
        {
            regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, RegexTimeout);
            return true;
        }
        catch (ArgumentException e)
        {
            regex = null;
            error = e.Message;
            return false;
        }
    }

    private static void LogWarning(string message)
    {
        try
        {
            Logger.GetSawmill("chat_trigger").Warning(message);
        }
        catch
        {
            // IoC may not be initialized on this thread or in unit tests
        }
    }

    public static Regex? GetOrCompileRegex(string pattern)
    {
        return RegexCache.GetOrAdd(pattern, p =>
        {
            if (TryCompileRegex(p, out var compiled, out var error))
                return compiled;

            LogWarning($"Invalid regex trigger pattern '{p}': {error}");
            return null;
        });
    }

    public static void ClearCache()
    {
        RegexCache.Clear();
    }

    public static void Precompile(string? triggerOrVariant)
    {
        if (string.IsNullOrWhiteSpace(triggerOrVariant))
            return;

        var variants = ParseTriggerVariants(triggerOrVariant);
        foreach (var variant in variants)
        {
            if (IsRegexTrigger(variant))
            {
                var pattern = variant[RegexPrefix.Length..].Trim();
                if (!string.IsNullOrEmpty(pattern))
                    GetOrCompileRegex(pattern);
            }
        }
    }

    public static string[] ParseTriggerVariants(string? rawTrigger)
    {
        if (string.IsNullOrWhiteSpace(rawTrigger))
            return Array.Empty<string>();

        var trimmed = rawTrigger.Trim();
        if (IsRegexTrigger(trimmed))
            return new[] { trimmed };

        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
        {
            var inner = trimmed[1..^1];
            return inner.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        return new[] { trimmed };
    }

    public static bool MatchesTrigger(string? text, string? trigger, [NotNullWhen(true)] out string? matchedWord)
    {
        matchedWord = null;
        if (string.IsNullOrWhiteSpace(trigger) || string.IsNullOrWhiteSpace(text))
            return false;

        if (IsRegexTrigger(trigger))
        {
            var pattern = trigger[RegexPrefix.Length..].Trim();
            if (string.IsNullOrEmpty(pattern))
                return false;

            var regex = GetOrCompileRegex(pattern);
            if (regex == null)
                return false;

            try
            {
                var match = regex.Match(text);
                while (match.Success)
                {
                    string? candidate = null;
                    if (match.Groups["word"].Success && !string.IsNullOrWhiteSpace(match.Groups["word"].Value))
                    {
                        candidate = match.Groups["word"].Value.Trim();
                    }
                    else if (match.Groups["trigger"].Success && !string.IsNullOrWhiteSpace(match.Groups["trigger"].Value))
                    {
                        candidate = match.Groups["trigger"].Value.Trim();
                    }
                    else if (match.Length > 0 && !string.IsNullOrWhiteSpace(match.Value))
                    {
                        candidate = match.Value.Trim();
                    }

                    if (!string.IsNullOrEmpty(candidate))
                    {
                        matchedWord = candidate;
                        return true;
                    }

                    match = match.NextMatch();
                }

                return false;
            }
            catch (RegexMatchTimeoutException)
            {
                LogWarning($"Regex trigger match timed out for pattern '{pattern}'");
                return false;
            }
        }

        var idx = 0;
        while (idx <= text.Length - trigger.Length)
        {
            idx = text.IndexOf(trigger, idx, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                return false;

            var before = idx == 0 || !char.IsLetterOrDigit(text[idx - 1]);
            var after = idx + trigger.Length >= text.Length || !char.IsLetterOrDigit(text[idx + trigger.Length]);

            if (before && after)
            {
                matchedWord = text.Substring(idx, trigger.Length);
                return true;
            }

            idx++;
        }

        return false;
    }

    public static bool MatchesSingleTrigger(string? text, string? trigger)
    {
        return MatchesTrigger(text, trigger, out _);
    }
}
