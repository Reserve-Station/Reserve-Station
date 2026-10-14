using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Robust.Shared.Log;
using Robust.Shared.Random;

namespace Content.Shared._Reserve.ChatTrigger;

public static class ChatTriggerHelper
{
    public const string RegexPrefix = "regex:";
    public const int MinNonRegexTriggerLength = 2;

    public static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(250);

    private static readonly ConcurrentDictionary<string, Regex?> RegexCache = new();
    private static readonly Regex MultipleSpacesRegex = new(@"[ \t]{2,}", RegexOptions.Compiled);
    private static readonly Regex PunctuationSpaceRegex = new(@"\s+([,.:;!?])", RegexOptions.Compiled);
    private static readonly Regex DuplicateCommasRegex = new(@",\s*,", RegexOptions.Compiled);

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
            var pattern = trimmed.Substring(RegexPrefix.Length).Trim();
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
                var pattern = variant.Substring(RegexPrefix.Length).Trim();
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
            else if (variant.Trim().Length < MinNonRegexTriggerLength)
            {
                error = $"Trigger variant must be at least {MinNonRegexTriggerLength} characters long";
                return false;
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
                var pattern = variant.Substring(RegexPrefix.Length).Trim();
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
            var inner = trimmed.Substring(1, trimmed.Length - 2);
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
            var pattern = trigger.Substring(RegexPrefix.Length).Trim();
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

    public static string MatchCase(string original, string replacement)
    {
        if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(replacement))
            return replacement;

        var hasLetters = false;
        var allUpper = true;
        var firstLetter = '\0';
        foreach (var c in original)
        {
            if (char.IsLetter(c))
            {
                if (!hasLetters)
                    firstLetter = c;

                hasLetters = true;
                if (!char.IsUpper(c))
                {
                    allUpper = false;
                    break;
                }
            }
        }

        if (hasLetters && allUpper)
            return replacement.ToUpperInvariant();

        if (hasLetters && char.IsUpper(firstLetter))
        {
            for (var i = 0; i < replacement.Length; i++)
            {
                if (char.IsLetter(replacement[i]))
                {
                    var upper = char.ToUpperInvariant(replacement[i]);
                    if (upper == replacement[i])
                        return replacement;

                    var chars = replacement.ToCharArray();
                    chars[i] = upper;
                    return new string(chars);
                }
            }
        }

        return replacement;
    }

    public static string ReplaceTriggerWord(string text, string trigger, string replacement)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(trigger))
            return text;

        StringBuilder? sb = null;
        var lastIdx = 0;
        var idx = 0;

        while (idx <= text.Length - trigger.Length)
        {
            idx = text.IndexOf(trigger, idx, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                break;

            var before = idx == 0 || !char.IsLetterOrDigit(text[idx - 1]);
            var after = idx + trigger.Length >= text.Length || !char.IsLetterOrDigit(text[idx + trigger.Length]);

            if (before && after)
            {
                sb ??= new StringBuilder(text.Length);
                sb.Append(text, lastIdx, idx - lastIdx);
                var matchedWord = text.Substring(idx, trigger.Length);
                sb.Append(MatchCase(matchedWord, replacement));
                idx += trigger.Length;
                lastIdx = idx;
            }
            else
            {
                idx++;
            }
        }

        if (sb == null)
            return text;

        if (lastIdx < text.Length)
            sb.Append(text, lastIdx, text.Length - lastIdx);

        return sb.ToString();
    }

    public static string ReplaceTriggerRegex(string text, Regex regex, string replacementPattern)
    {
        if (string.IsNullOrEmpty(text) || regex == null)
            return text;

        try
        {
            return regex.Replace(text, match =>
            {
                string evaluated;
                try
                {
                    evaluated = match.Result(replacementPattern);
                }
                catch
                {
                    evaluated = replacementPattern;
                }
                return MatchCase(match.Value, evaluated);
            });
        }
        catch (RegexMatchTimeoutException)
        {
            LogWarning("Regex trigger replacement timed out");
            return text;
        }
    }

    public static bool TryApplyReplacements(
        string text,
        IEnumerable<TriggerReplacementRule> rules,
        [NotNullWhen(true)] out string? fixedText,
        IRobustRandom? random = null)
    {
        fixedText = null;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var current = text;
        foreach (var rule in rules)
        {
            if (!rule.HasReplacement)
                continue;

            current = rule.Apply(current, random);
        }

        if (current != text)
        {
            fixedText = current;
            return true;
        }

        return false;
    }

    public static bool HasContent(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        foreach (var c in text)
        {
            if (char.IsLetterOrDigit(c))
                return true;
        }

        return false;
    }

    public static string SanitizeSpacesAndPunctuation(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Collapse multiple spaces or tabs into a single space
        var result = MultipleSpacesRegex.Replace(text, " ");

        // Remove space before punctuation: "слово , слово" -> "слово, слово", "слово !" -> "слово!"
        result = PunctuationSpaceRegex.Replace(result, "$1");

        // Remove duplicate commas: "слово, , слово" -> "слово, слово"
        result = DuplicateCommasRegex.Replace(result, ",");

        // Trim leading and trailing whitespace
        result = result.Trim();

        return result;
    }

    public static string RemoveTrigger(string text, string? trigger, string? matchedWord)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var result = text;

        if (!string.IsNullOrEmpty(matchedWord))
        {
            result = ReplaceTriggerWord(result, matchedWord, string.Empty);
        }

        if (!string.IsNullOrEmpty(trigger))
        {
            if (IsRegexTrigger(trigger))
            {
                var pattern = trigger.Substring(RegexPrefix.Length).Trim();
                var regex = GetOrCompileRegex(pattern);
                if (regex != null)
                {
                    try
                    {
                        result = regex.Replace(result, string.Empty);
                    }
                    catch (RegexMatchTimeoutException)
                    {
                        LogWarning("Regex trigger removal timed out");
                    }
                }
            }
            else
            {
                var variants = ParseTriggerVariants(trigger);
                foreach (var variant in variants)
                {
                    result = ReplaceTriggerWord(result, variant, string.Empty);
                }
            }
        }

        return SanitizeSpacesAndPunctuation(result);
    }
}
