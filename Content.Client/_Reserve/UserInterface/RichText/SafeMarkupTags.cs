// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Reflection;

namespace Content.Client._Reserve.UserInterface.RichText;

/// <summary>
/// Markup tag allowlists for rendering text that may contain player-controlled content.
/// </summary>
/// <remarks>
/// <see cref="CommandLinkTag"/> runs a console command as whoever clicks it, with that player's permissions.
/// If player text ever reaches a label that allows it (e.g. escaping was missed somewhere on the server),
/// a player can make an admin run any admin command with a single click.
/// Only use the full tag set (<c>tagsAllowed: null</c>) for text that is fully server-generated.
/// </remarks>
public static class SafeMarkupTags
{
    private static readonly Type[] EngineTags =
    [
        typeof(BoldItalicTag),
        typeof(BoldTag),
        typeof(BulletTag),
        typeof(ColorTag),
        typeof(FontTag),
        typeof(HeadingTag),
        typeof(ItalicTag),
    ];

    private static Type[]? _withoutCommandLinks;

    /// <summary>
    /// Every registered markup tag (engine and content) except <see cref="CommandLinkTag"/>.
    /// </summary>
    public static Type[] WithoutCommandLinks
    {
        get
        {
            if (_withoutCommandLinks != null)
                return _withoutCommandLinks;

            var reflection = IoCManager.Resolve<IReflectionManager>();
            _withoutCommandLinks = reflection.GetAllChildren<IMarkupTagHandler>()
                .Concat(EngineTags)
                .Where(t => t != typeof(CommandLinkTag))
                .Distinct()
                .ToArray();

            return _withoutCommandLinks;
        }
    }
}
