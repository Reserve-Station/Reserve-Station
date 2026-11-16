// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Common.ServerCurrency;
using Content.Server._Reserve.Discord.AdminGuard; // Reserve edit: Reserve Red - AdminGuard
using Content.Server._Reserve.Discord.Logger; // Reserve edit: Reserve Red - AdminGuard
using Content.Server.Administration;
using Content.Server.Chat.Managers;
using Content.Shared.Administration;
using Content.Shared.Chat;
using System.Threading.Tasks; // Reserve edit: Coin transfer ovehaul
using Robust.Server.Player;
using Robust.Shared.Console;
using Content.Shared._Reserve.LenaApi; // Reserve-LenaApi
using Robust.Shared.Configuration; // Reserve-LenaApi

namespace Content.Goobstation.Server.ServerCurrency.Commands
{
    [AnyCommand]
    public sealed partial class BalanceServerCurrencyCommand : IConsoleCommand
    {
        [Dependency] private ICommonCurrencyManager _currencyMan = default!;
        [Dependency] private IChatManager _chatManager = default!;
        [Dependency] private IConfigurationManager _configuration = default!; // Reserve-LenaApi
        public string Command => Loc.GetString("server-currency-balance-command");
        public string Description => Loc.GetString("server-currency-balance-command-description");
        public string Help => Loc.GetString("server-currency-balance-command-help");

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            // Reserve-LenaApi-start
            var apiIntegrationEnabled = _configuration.GetCVar(LenaApiCVars.ApiIntegration);
            if (apiIntegrationEnabled)
            {
                shell.WriteLine(Loc.GetString("reserve-command-disabled-due-integration-enabled"));
                return;
            }
            // Reserve-LenaApi-end

            if (args.Length != 0)
            {
                shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
                return;
            }

            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            var balance = Loc.GetString("server-currency-balance-command-return",
                ("balance", _currencyMan.Stringify(_currencyMan.GetBalance(shell.Player.UserId))));

            _chatManager.ChatMessageToOne(ChatChannel.Local, balance, balance, EntityUid.Invalid, false, shell.Player.Channel);
            shell.WriteLine(balance);
        }
    }

    [AnyCommand]
    public sealed partial class GiftServerCurrencyCommand : IConsoleCommand
    {
        private const string Operation = "server-currency-operation-gift"; // Reserve edit: Coin transfer ovehaul

        [Dependency] private ICommonCurrencyManager _currencyMan = default!;
        [Dependency] private IChatManager _chatManager = default!;
        [Dependency] private IConfigurationManager _configuration = default!; // Reserve-LenaApi
        [Dependency] private IAdminGuardManager _adminGuard = default!; // Reserve edit: Reserve Red - AdminGuard

        public string Command => Loc.GetString("server-currency-gift-command");
        public string Description => Loc.GetString("server-currency-gift-command-description");
        public string Help => Loc.GetString("server-currency-gift-command-help");
        // Reserve edit start: Coin transfer ovehaul

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            CurrencyCommandHelper.Run(shell, Operation, () => ExecuteAsync(shell, args));
        }

        private async Task ExecuteAsync(IConsoleShell shell, string[] args)
        {
            // Reserve-LenaApi-start
            var apiIntegrationEnabled = _configuration.GetCVar(LenaApiCVars.ApiIntegration);
            if (apiIntegrationEnabled)
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("server-currency-error-integration-enabled"));
                return;
            }
            // Reserve-LenaApi-end

            if (args.Length != 2)
            {
                CurrencyCommandHelper.Fail(shell, Operation, $"{Loc.GetString("shell-wrong-arguments-number")} {Help}");
                return;
            }

            if (shell.Player is not { } player)
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (!int.TryParse(args[1], out var amount))
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("server-currency-command-error-2"));
                return;
            }

            var target = await CurrencyCommandHelper.ResolveTarget(shell, Operation, args[0]);
            if (target is not { } resolved)
                return;

            if (resolved.UserId == player.UserId)
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("server-currency-gift-command-error-1"));
                return;
            }

            amount = Math.Abs(amount);

            if (amount == 0)
                amount = 1; // Trolled

            if (!_currencyMan.CanAfford(player.UserId, amount, out var balance))
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("server-currency-gift-command-error-2", ("balance", balance)));
                return;
            }

            _currencyMan.TransferCurrency(player.UserId, resolved.UserId, amount);
            _adminGuard.CurrencyTransferred(player.UserId, resolved.UserId, _currencyMan.Stringify(amount)); // Reserve edit: Reserve Red - AdminGuard

            var giver = Loc.GetString("server-currency-gift-command-giver", ("player", resolved.Name), ("amount", _currencyMan.Stringify(amount)));
            var reciever = Loc.GetString("server-currency-gift-command-reciever", ("player", player.Name), ("amount", _currencyMan.Stringify(amount)));

            CurrencyCommandHelper.NotifyIfOnline(resolved.UserId, reciever);
            CurrencyCommandHelper.Succeed(shell, giver);
        }
        // Reserve edit end: Coin transfer ovehaul

        public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            return args.Length switch
            {
                1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(), Loc.GetString("server-currency-command-completion-1")),
                2 => CompletionResult.FromHint(Loc.GetString("server-currency-command-completion-2")),
                _ => CompletionResult.Empty
            };
        }
    }

    [AdminCommand(AdminFlags.Admin)] // Reserve edit: Coin transfer ovehaul
    public sealed partial class AddServerCurrencyCommand : IConsoleCommand
    {
        private const string Operation = "server-currency-operation-add"; // Reserve edit: Coin transfer ovehaul

        [Dependency] private ICommonCurrencyManager _currencyMan = default!;
        [Dependency] private IAdminGuardManager _adminGuard = default!; // Reserve edit: Reserve Red - AdminGuard

        public string Command => Loc.GetString("server-currency-add-command");
        public string Description => Loc.GetString("server-currency-add-command-description");
        public string Help => Loc.GetString("server-currency-add-command-help");
        // Reserve edit start: Coin transfer ovehaul

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            CurrencyCommandHelper.Run(shell, Operation, () => ExecuteAsync(shell, args));
        }

        private async Task ExecuteAsync(IConsoleShell shell, string[] args)
        {
            if (args.Length != 2)
            {
                CurrencyCommandHelper.Fail(shell, Operation, $"{Loc.GetString("shell-wrong-arguments-number")} {Help}");
                return;
            }

            if (!int.TryParse(args[1], out var currency))
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("server-currency-command-error-2"));
                return;
            }

            var target = await CurrencyCommandHelper.ResolveTarget(shell, Operation, args[0]);
            if (target is not { } resolved)
                return;

            var newBalance = _currencyMan.AddCurrency(resolved.UserId, currency);
            var newCurrency = _currencyMan.Stringify(newBalance);
            _adminGuard.AdminCurrencyChanged(resolved.UserId, AdminCurrencyAction.Add, _currencyMan.Stringify(currency), newCurrency, DiscordLogActor.From(shell.Player)); // Reserve edit: Reserve Red - AdminGuard

            CurrencyCommandHelper.NotifyIfOnline(resolved.UserId,
                Loc.GetString("server-currency-admin-add-target", ("amount", _currencyMan.Stringify(currency)), ("balance", newCurrency)));
            CurrencyCommandHelper.Succeed(shell,
                Loc.GetString("server-currency-admin-add-author", ("player", resolved.Name), ("amount", _currencyMan.Stringify(currency)), ("balance", newCurrency)));
        }
        // Reserve edit end: Coin transfer ovehaul

        public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            return args.Length switch
            {
                1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(), Loc.GetString("server-currency-command-completion-1")),
                2 => CompletionResult.FromHint(Loc.GetString("server-currency-command-completion-2")),
                _ => CompletionResult.Empty
            };
        }
    }

    [AdminCommand(AdminFlags.Admin)] // Reserve edit: Coin transfer ovehaul
    public sealed partial class RemoveServerCurrencyCommand : IConsoleCommand
    {
        private const string Operation = "server-currency-operation-remove"; // Reserve edit: Coin transfer ovehaul

        [Dependency] private ICommonCurrencyManager _currencyMan = default!;
        [Dependency] private IAdminGuardManager _adminGuard = default!; // Reserve edit: Reserve Red - AdminGuard

        public string Command => Loc.GetString("server-currency-remove-command");
        public string Description => Loc.GetString("server-currency-remove-command-description");
        public string Help => Loc.GetString("server-currency-remove-command-help");
        // Reserve edit start: Coin transfer ovehaul

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            CurrencyCommandHelper.Run(shell, Operation, () => ExecuteAsync(shell, args));
        }

        private async Task ExecuteAsync(IConsoleShell shell, string[] args)
        {
            if (args.Length != 2)
            {
                CurrencyCommandHelper.Fail(shell, Operation, $"{Loc.GetString("shell-wrong-arguments-number")} {Help}");
                return;
            }

            if (!int.TryParse(args[1], out var currency))
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("server-currency-command-error-2"));
                return;
            }

            var target = await CurrencyCommandHelper.ResolveTarget(shell, Operation, args[0]);
            if (target is not { } resolved)
                return;

            var newBalance = _currencyMan.RemoveCurrency(resolved.UserId, currency);
            var newCurrency = _currencyMan.Stringify(newBalance);
            _adminGuard.AdminCurrencyChanged(resolved.UserId, AdminCurrencyAction.Remove, _currencyMan.Stringify(currency), newCurrency, DiscordLogActor.From(shell.Player)); // Reserve edit: Reserve Red - AdminGuard

            CurrencyCommandHelper.NotifyIfOnline(resolved.UserId,
                Loc.GetString("server-currency-admin-remove-target", ("amount", _currencyMan.Stringify(currency)), ("balance", newCurrency)));
            CurrencyCommandHelper.Succeed(shell,
                Loc.GetString("server-currency-admin-remove-author", ("player", resolved.Name), ("amount", _currencyMan.Stringify(currency)), ("balance", newCurrency)));
        }
        // Reserve edit end: Coin transfer ovehaul

        public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            return args.Length switch
            {
                1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(), Loc.GetString("server-currency-command-completion-1")),
                2 => CompletionResult.FromHint(Loc.GetString("server-currency-command-completion-2")),
                _ => CompletionResult.Empty
            };
        }
    }

    [AdminCommand(AdminFlags.Admin)] // Reserve edit: Coin transfer ovehaul
    public sealed partial class SetServerCurrencyCommand : IConsoleCommand
    {
        private const string Operation = "server-currency-operation-set"; // Reserve edit: Coin transfer ovehaul

        [Dependency] private ICommonCurrencyManager _currencyMan = default!;
        [Dependency] private IAdminGuardManager _adminGuard = default!; // Reserve edit: Reserve Red - AdminGuard

        public string Command => Loc.GetString("server-currency-set-command");
        public string Description => Loc.GetString("server-currency-set-command-description");
        public string Help => Loc.GetString("server-currency-set-command-help");
        // Reserve edit start: Coin transfer ovehaul

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            CurrencyCommandHelper.Run(shell, Operation, () => ExecuteAsync(shell, args));
        }

        private async Task ExecuteAsync(IConsoleShell shell, string[] args)
        {
            if (args.Length != 2)
            {
                CurrencyCommandHelper.Fail(shell, Operation, $"{Loc.GetString("shell-wrong-arguments-number")} {Help}");
                return;
            }

            if (!int.TryParse(args[1], out var currency))
            {
                CurrencyCommandHelper.Fail(shell, Operation, Loc.GetString("server-currency-command-error-2"));
                return;
            }

            var target = await CurrencyCommandHelper.ResolveTarget(shell, Operation, args[0]);
            if (target is not { } resolved)
                return;

            _currencyMan.SetBalance(resolved.UserId, currency);
            var newCurrency = _currencyMan.Stringify(currency);
            _adminGuard.AdminCurrencyChanged(resolved.UserId, AdminCurrencyAction.Set, newCurrency, newCurrency, DiscordLogActor.From(shell.Player)); // Reserve edit: Reserve Red - AdminGuard

            CurrencyCommandHelper.NotifyIfOnline(resolved.UserId,
                Loc.GetString("server-currency-admin-set-target", ("balance", newCurrency)));
            CurrencyCommandHelper.Succeed(shell,
                Loc.GetString("server-currency-admin-set-author", ("player", resolved.Name), ("balance", newCurrency)));
        }
        // Reserve edit end: Coin transfer ovehaul

        public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            return args.Length switch
            {
                1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(), Loc.GetString("server-currency-command-completion-1")),
                2 => CompletionResult.FromHint(Loc.GetString("server-currency-command-completion-2")),
                _ => CompletionResult.Empty
            };
        }
    }

    [AdminCommand(AdminFlags.Admin)] // Reserve edit: Coin transfer ovehaul
    public sealed partial class GetServerCurrencyCommand : IConsoleCommand
    {
        private const string Operation = "server-currency-operation-get"; // Reserve edit: Coin transfer ovehaul

        [Dependency] private ICommonCurrencyManager _currencyMan = default!;

        public string Command => Loc.GetString("server-currency-get-command");
        public string Description => Loc.GetString("server-currency-get-command-description");
        public string Help => Loc.GetString("server-currency-get-command-help");
        // Reserve edit start: Coin transfer ovehaul

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            CurrencyCommandHelper.Run(shell, Operation, () => ExecuteAsync(shell, args));
        }

        private async Task ExecuteAsync(IConsoleShell shell, string[] args)
        {
            if (args.Length != 1)
            {
                CurrencyCommandHelper.Fail(shell, Operation, $"{Loc.GetString("shell-wrong-arguments-number")} {Help}");
                return;
            }

            var target = await CurrencyCommandHelper.ResolveTarget(shell, Operation, args[0]);
            if (target is not { } resolved)
                return;

            var currency = _currencyMan.Stringify(_currencyMan.GetBalance(resolved.UserId));
            CurrencyCommandHelper.Succeed(shell, Loc.GetString("server-currency-command-return", ("player", resolved.Name), ("balance", currency)));
        }
        // Reserve edit end: Coin transfer ovehaul

        public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            return args.Length switch
            {
                1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(), Loc.GetString("server-currency-command-completion-1")),
                _ => CompletionResult.Empty
            };
        }
    }
}
