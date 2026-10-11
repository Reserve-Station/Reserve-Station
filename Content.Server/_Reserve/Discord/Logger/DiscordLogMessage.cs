// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Discord;

namespace Content.Server._Reserve.Discord.Logger;

/// <summary>
///     Лог-сообщение: заголовок, строки и подвал. Одинаково превращается в embed или в обычный текст.
/// </summary>
public sealed class DiscordLogMessage(string title, int color, string footer)
{
    public const int EmbedLimit = 4000;

    private readonly List<string> _lines = new();

    public string Title { get; } = title;
    public int Color { get; } = color;
    public string Footer { get; } = footer;
    public string Body => string.Join('\n', _lines);

    /// <summary>
    ///     Добавляет локализованную строку.
    /// </summary>
    public void Add(string locId, params (string, object)[] args)
    {
        _lines.Add(Loc.GetString(locId, args));
    }

    public string ToPlainText() => $"### {Title}\n{Body}\n-# {Footer}";

    public WebhookEmbed ToWebhookEmbed()
    {
        var body = Body;

        return new WebhookEmbed
        {
            Title = Title,
            Description = body.Length <= EmbedLimit ? body : body[..(EmbedLimit - 1)] + "…",
            Color = Color,
            Footer = new WebhookEmbedFooter { Text = Footer },
        };
    }
}
