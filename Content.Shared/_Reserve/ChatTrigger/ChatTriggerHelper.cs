namespace Content.Shared._Reserve.ChatTrigger;

public static class ChatTriggerHelper
{
    public static string[] ParseTriggerVariants(string rawTrigger)
    {
        if (string.IsNullOrWhiteSpace(rawTrigger))
            return Array.Empty<string>();

        var trimmed = rawTrigger.Trim();
        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
        {
            var inner = trimmed[1..^1];
            return inner.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        return new[] { trimmed };
    }

    public static bool MatchesSingleTrigger(string text, string trigger)
    {
        if (string.IsNullOrWhiteSpace(trigger) || string.IsNullOrWhiteSpace(text))
            return false;

        var idx = 0;
        while (idx <= text.Length - trigger.Length)
        {
            idx = text.IndexOf(trigger, idx, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                return false;

            var before = idx == 0 || !char.IsLetterOrDigit(text[idx - 1]);
            var after = idx + trigger.Length >= text.Length || !char.IsLetterOrDigit(text[idx + trigger.Length]);

            if (before && after)
                return true;

            idx++;
        }

        return false;
    }
}
