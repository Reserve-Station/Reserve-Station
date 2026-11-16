// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;
using Content.Server.Administration;
using Content.Server.Chat.Managers;
using Content.Server.Database;
using Content.Shared.Chat;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Goobstation.Server.ServerCurrency.Commands;

/// <summary>
/// Shared plumbing for the currency commands: target lookup (including offline players),
/// chat feedback to the author/receiver and uniform, traceable error reporting.
/// </summary>
internal static class CurrencyCommandHelper
{
    public readonly record struct Target(NetUserId UserId, string Name);

    /// <summary>
    /// Runs a command body, converting any unexpected exception into a traceable error message for the author.
    /// </summary>
    public static async void Run(IConsoleShell shell, string operation, Func<Task> body)
    {
        try
        {
            await body();
        }
        catch (Exception e)
        {
            FailUnexpected(shell, operation, e);
        }
    }

    /// <summary>
    /// Finds a player by name or user id, whether they are online or not.
    /// The player must have a database record, otherwise there is no balance to modify.
    /// </summary>
    public static async Task<Target?> ResolveTarget(IConsoleShell shell, string operation, string nameOrId)
    {
        var locator = IoCManager.Resolve<IPlayerLocator>();
        var located = await locator.LookupIdByNameOrIdAsync(nameOrId);
        if (located == null)
        {
            Fail(shell, operation, Loc.GetString("server-currency-command-error-1"));
            return null;
        }

        var db = IoCManager.Resolve<IServerDbManager>();
        if (await db.GetPlayerRecordByUserId(located.UserId) == null)
        {
            Fail(shell, operation, Loc.GetString("server-currency-error-never-joined", ("player", located.Username)));
            return null;
        }

        return new Target(located.UserId, located.Username);
    }

    /// <summary>
    /// Reports a failure to the author in both the console and the chat.
    /// </summary>
    public static void Fail(IConsoleShell shell, string operation, string reason, Exception? exception = null)
    {
        var id = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var message = Loc.GetString("server-currency-error-format",
            ("operation", Loc.GetString(operation)),
            ("reason", reason),
            ("id", id),
            ("time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));

        var sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("server_currency");
        var author = shell.Player?.Name ?? "server";
        if (exception != null)
            sawmill.Error($"[{id}] '{operation}' by {author} failed: {reason}\n{exception}");
        else
            sawmill.Warning($"[{id}] '{operation}' by {author} failed: {reason}");

        shell.WriteError(message);
        SendChat(shell.Player, message, error: true);
    }

    public static void FailUnexpected(IConsoleShell shell, string operation, Exception exception)
    {
        Fail(shell, operation,
            Loc.GetString("server-currency-error-unexpected",
                ("type", exception.GetType().Name),
                ("message", exception.Message)),
            exception);
    }

    /// <summary>
    /// Sends a message to the author both in console and chat.
    /// </summary>
    public static void Succeed(IConsoleShell shell, string message)
    {
        shell.WriteLine(message);
        SendChat(shell.Player, message);
    }

    /// <summary>
    /// Sends a chat message to a player if they are currently online.
    /// </summary>
    public static void NotifyIfOnline(NetUserId userId, string message)
    {
        if (IoCManager.Resolve<IPlayerManager>().TryGetSessionById(userId, out var session))
            SendChat(session, message);
    }

    private static void SendChat(ICommonSession? session, string message, bool error = false)
    {
        if (session == null)
            return;

        var wrapped = error
            ? $"[color=red]{FormattedMessage.EscapeText(message)}[/color]"
            : FormattedMessage.EscapeText(message);

        IoCManager.Resolve<IChatManager>()
            .ChatMessageToOne(ChatChannel.Local, message, wrapped, EntityUid.Invalid, false, session.Channel);
    }
}
