// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Reserve.Discord.Logger;

/// <summary>
///     Кто совершил действие. Без <see cref="UserId"/> и <see cref="Name"/> это сервер/консоль сервера.
///     Если есть только <see cref="Name"/> (например, актор API), он выводится как есть.
/// </summary>
public readonly record struct DiscordLogActor(NetUserId? UserId, string? Name = null)
{
    public static DiscordLogActor Server => new(null);

    public static DiscordLogActor From(ICommonSession? session)
    {
        return session == null ? Server : new DiscordLogActor(session.UserId, session.Name);
    }
}
