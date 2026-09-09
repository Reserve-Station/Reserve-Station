using System.Collections.ObjectModel;
using System.Linq;  // Reserve edit: Full discord bot integration
using System.Threading.Tasks;
using Content.Server._Reserve.Discord;  // Reserve edit: Full discord bot integration
using Content.Shared.CCVar;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using DiscordInteraction = NetCord.Interaction;  // Reserve edit: Full discord bot integration
using Robust.Shared.Asynchronous;  // Reserve edit: Full discord bot integration
using Robust.Shared.Configuration;
using Robust.Shared.Utility;

namespace Content.Server.Discord.DiscordLink;

/// <summary>
/// Represents the arguments for the <see cref="DiscordLink.OnCommandReceived"/> event.
/// </summary>
public sealed class CommandReceivedEventArgs
{
    /// <summary>
    /// The command that was received. This is the first word in the message, after the bot prefix.
    /// </summary>
    public string Command { get; init; } = string.Empty;

    /// <summary>
    /// The raw arguments to the command. This is everything after the command
    /// </summary>
    public string RawArguments { get; init; } = string.Empty;

    /// <summary>
    /// A list of arguments to the command.
    /// This uses <see cref="CommandParsing.ParseArguments"/> mostly for maintainability.
    /// </summary>
    public List<string> Arguments { get; init; } = [];

    /// <summary>
    /// Information about the message that the command was received from. This includes the message content, author, etc.
    /// Use this to reply to the message, delete it, etc.
    /// </summary>
    public Message Message { get; init; } = default!;
}

/// <summary>
/// Handles the connection to Discord and provides methods to interact with it.
/// </summary>
public sealed class DiscordLink : IPostInjectInit
{
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly IConfigurationManager _configuration = default!;
    [Dependency] private readonly ITaskManager _taskManager = default!;  // Reserve edit: Full discord bot integration

    /// <summary>
    ///    The Discord client. This is null if the bot is not connected.
    /// </summary>
    /// <remarks>
    ///     This should not be used directly outside of DiscordLink. So please do not make it public. Use the methods in this class instead.
    /// </remarks>
    private GatewayClient? _client;
    private ISawmill _sawmill = default!;
    private ISawmill _sawmillLog = default!;
    private DiscordProfileCog? _profileCog;  // Reserve edit: Full discord bot integration
    private DiscordLinkCog? _linkCog;  // Reserve edit: Discord bot account linking

    private ulong _guildId;
    private string _botToken = string.Empty;

    public string BotPrefix = default!;
    /// <summary>
    /// If the bot is currently connected to Discord.
    /// </summary>
    public bool IsConnected => _client != null;

    #region Events

    /// <summary>
    ///     Event that is raised when a command is received from Discord.
    /// </summary>
    public event Action<CommandReceivedEventArgs>? OnCommandReceived;
    /// <summary>
    ///     Event that is raised when a message is received from Discord. This is raised for every message, including commands.
    /// </summary>
    public event Action<Message>? OnMessageReceived;

    // TODO: consider implementing this in a way where we can unregister it in a similar way
    public void RegisterCommandCallback(Action<CommandReceivedEventArgs> callback, string command)
    {
        OnCommandReceived += args =>
        {
            if (args.Command == command)
                callback(args);
        };
    }

    #endregion

    public void Initialize()
    {
        _configuration.OnValueChanged(CCVars.DiscordGuildId, OnGuildIdChanged, true);
        _configuration.OnValueChanged(CCVars.DiscordPrefix, OnPrefixChanged, true);

        if (_configuration.GetCVar(CCVars.DiscordToken) is not { } token || token == string.Empty)
        {
            _sawmill.Info("No Discord token specified, not connecting.");
            return;
        }

        // If the Guild ID is empty OR the prefix is empty, we don't want to connect to Discord.
        if (_guildId == 0 || BotPrefix == string.Empty)
        {
            // This is a warning, not info, because it's a configuration error.
            // It is valid to not have a Discord token set which is why the above check is an info.
            // But if you have a token set, you should also have a guild ID and prefix set.
            _sawmill.Warning("No Discord guild ID or prefix specified, not connecting.");
            return;
        }

        _client = new GatewayClient(new BotToken(token), new GatewayClientConfiguration()
        {
            Intents = GatewayIntents.Guilds
                             | GatewayIntents.GuildUsers
                             | GatewayIntents.GuildMessages
                             | GatewayIntents.MessageContent
                             | GatewayIntents.DirectMessages,
            Logger = new DiscordSawmillLogger(_sawmillLog),
        });
        _client.MessageCreate += OnCommandReceivedInternal;
        _client.MessageCreate += OnMessageReceivedInternal;

        _profileCog = new DiscordProfileCog();  // Reserve edit: Full discord bot integration
        _linkCog = new DiscordLinkCog();  // Reserve edit: Discord bot account linking
        _client.InteractionCreate += OnInteractionCreate;  // Reserve edit: Full discord bot integration

        _botToken = token;
        // Since you cannot change the token while the server is running / the DiscordLink is initialized,
        // we can just set the token without updating it every time the cvar changes.

        _client.Ready += readyArgs =>  // Reserve edit: Full discord bot integration
        {
            _sawmill.Info("Discord client ready.");
            _ = RegisterApplicationCommandsAsync();  // Reserve edit: Full discord bot integration
            return default;
        };

        Task.Run(async () =>
        {
            try
            {
                await _client.StartAsync();
                _sawmill.Info("Connected to Discord.");
            }
            catch (Exception e)
            {
                _sawmill.Error("Failed to connect to Discord!", e);
            }
        });
    }

    public async Task Shutdown()
    {
        if (_client != null)
        {
            _sawmill.Info("Disconnecting from Discord.");

            // Unsubscribe from the events.
            _client.MessageCreate -= OnCommandReceivedInternal;
            _client.MessageCreate -= OnMessageReceivedInternal;
            _client.InteractionCreate -= OnInteractionCreate;  // Reserve edit: Full discord bot integration

            await _client.CloseAsync();
            _client.Dispose();
            _client = null;
        }

        _configuration.UnsubValueChanged(CCVars.DiscordGuildId, OnGuildIdChanged);
        _configuration.UnsubValueChanged(CCVars.DiscordPrefix, OnPrefixChanged);
    }

    void IPostInjectInit.PostInject()
    {
        _sawmill = _logManager.GetSawmill("discord.link");
        _sawmillLog = _logManager.GetSawmill("discord.link.log");
    }

    private void OnGuildIdChanged(string guildId)
    {
        _guildId = ulong.TryParse(guildId, out var id) ? id : 0;
    }

    private void OnPrefixChanged(string prefix)
    {
        BotPrefix = prefix;
    }

    private ValueTask OnCommandReceivedInternal(Message message)
    {
        var content = message.Content;
        // If the message doesn't start with the bot prefix, ignore it.
        if (!content.StartsWith(BotPrefix))
            return ValueTask.CompletedTask;

        // Split the message into the command and the arguments.
        var trimmedInput = content[BotPrefix.Length..].Trim();
        var firstSpaceIndex = trimmedInput.IndexOf(' ');

        string command, rawArguments;

        if (firstSpaceIndex == -1)
        {
            command = trimmedInput;
            rawArguments = string.Empty;
        }
        else
        {
            command = trimmedInput[..firstSpaceIndex];
            rawArguments = trimmedInput[(firstSpaceIndex + 1)..].Trim();
        }

        var argumentList = new List<string>();
        CommandParsing.ParseArguments(rawArguments, argumentList);

        // Raise the event!
        OnCommandReceived?.Invoke(new CommandReceivedEventArgs
        {
            Command = command,
            Arguments = argumentList,
            RawArguments = rawArguments,
            Message = message,
        });
        return ValueTask.CompletedTask;
    }

    private ValueTask OnMessageReceivedInternal(Message message)
    {
        OnMessageReceived?.Invoke(message);
        return ValueTask.CompletedTask;
    }

    // Reserve edit start: Full discord bot integration
    private async ValueTask OnInteractionCreate(DiscordInteraction interaction)
    {
        if (interaction is not SlashCommandInteraction command)
            return;

        if (command.Data.Name == "link")
        {
            await HandleLinkCommand(command);
            return;
        }
        else if (command.Data.Name == "unlink")
        {
            await HandleUnlinkCommand(command);
            return;
        }

        if (_profileCog == null || command.Data.Name is not ("profile" or "balance" or "characters" or "inventory"))
            return;

        var player = command.Data.Options.FirstOrDefault(option => option.Name == "player")?.Value;
        var privately = command.Data.Options.FirstOrDefault(option => option.Name == "privately")?.Value;
        var ephemeral = privately is not null && bool.TryParse(privately, out var parsedPrivately)
            ? parsedPrivately
            : _configuration.GetCVar(CCVars.DiscordProfilePrivatelyDefault);

        var response = await HandleProfileOnMainThread(command.Data.Name, player, command.User.Id, ephemeral);
        await interaction.SendResponseAsync(response);
    }

    // Reserve edit start: Discord bot account linking
    private async Task HandleLinkCommand(SlashCommandInteraction command)
    {
        if (_linkCog == null)
            return;

        var code = command.Data.Options.FirstOrDefault(option => option.Name == "code")?.Value ?? string.Empty;
        var response = await HandleLinkOnMainThread(code, command.User.Id);
        await command.SendResponseAsync(response);
    }

    private async Task HandleUnlinkCommand(SlashCommandInteraction command)
    {
        if (_linkCog == null)
            return;

        var response = await HandleUnlinkOnMainThread(command.User.Id);
        await command.SendResponseAsync(response);
    }

    private Task<InteractionCallback> HandleUnlinkOnMainThread(ulong discordUserId)
    {
        var completion = new TaskCompletionSource<InteractionCallback>(TaskCreationOptions.RunContinuationsAsynchronously);

        _taskManager.RunOnMainThread(async () =>
        {
            try
            {
                completion.SetResult(await _linkCog!.HandleUnlinkAsync(discordUserId));
            }
            catch (Exception e)
            {
                completion.SetException(e);
            }
        });

        return completion.Task;
    }

    private Task<InteractionCallback> HandleLinkOnMainThread(string code, ulong discordUserId)
    {
        var completion = new TaskCompletionSource<InteractionCallback>(TaskCreationOptions.RunContinuationsAsynchronously);

        _taskManager.RunOnMainThread(async () =>
        {
            try
            {
                completion.SetResult(await _linkCog!.HandleLinkAsync(code, discordUserId));
            }
            catch (Exception e)
            {
                completion.SetException(e);
            }
        });

        return completion.Task;
    }
    // Reserve edit end: Discord bot account linking

    private Task<InteractionCallback> HandleProfileOnMainThread(string command, string? player, ulong discordUserId, bool ephemeral)
    {
        var completion = new TaskCompletionSource<InteractionCallback>(TaskCreationOptions.RunContinuationsAsynchronously);

        _taskManager.RunOnMainThread(async () =>
        {
            try
            {
                completion.SetResult(await _profileCog!.HandleAsync(command, player, discordUserId, ephemeral, _client!.Rest, _guildId));
            }
            catch (Exception e)
            {
                completion.SetException(e);
            }
        });

        return completion.Task;
    }

    private async Task RegisterApplicationCommandsAsync()
    {
        if (_client == null)
            return;

        try
        {
            var commandDefinitions = new[]
            {
                CreateProfileCommand("profile", "Show a player's complete profile."),
                CreateProfileCommand("balance", "Show a player's coin balance."),
                CreateProfileCommand("characters", "Show a player's characters."),
                CreateProfileCommand("inventory", "Show a player's bought tokens."),
                CreateLinkCommand(),
                CreateUnlinkCommand(),
            };

            var registeredCommands = await _client.Rest.GetGuildApplicationCommandsAsync(_client.Id, _guildId);
            foreach (var command in commandDefinitions)
            {
                await _client.Rest.DeleteGuildApplicationCommandAsync(_client.Id, _guildId, registeredCommands.FirstOrDefault(c => c.Name == command.Name)?.Id ?? 0);
                await _client.Rest.CreateGuildApplicationCommandAsync(_client.Id, _guildId, command);
            }

            _sawmill.Info("Registered Discord application commands.");
        }
        catch (Exception e)
        {
            _sawmill.Error("Failed to register Discord application commands!", e);
        }
    }

    private static SlashCommandProperties CreateProfileCommand(string name, string description)
    {
        return new SlashCommandProperties(name, description)
            .AddOptions(
                new ApplicationCommandOptionProperties(
                    ApplicationCommandOptionType.String,
                    "player",
                    "Player CKey or UID (defaults to your own linked account)"),
                new ApplicationCommandOptionProperties(
                    ApplicationCommandOptionType.Boolean,
                    "privately",
                    "Only show the response to you. Always true if you specify a player."));
    }
    // Reserve edit end: Full discord bot integration

    // Reserve edit start: Discord bot account linking
    private static SlashCommandProperties CreateLinkCommand()
    {
        return new SlashCommandProperties("link", "Link your Discord account to your in-game account.")
            .AddOptions(new ApplicationCommandOptionProperties(
                ApplicationCommandOptionType.String,
                "code",
                "The linking code shown in-game")
                .WithRequired(true));
    }

    private static SlashCommandProperties CreateUnlinkCommand()
    {
        return new SlashCommandProperties("unlink", "Unlink your Discord account from your in-game account.");
    }
    // Reserve edit end: Discord bot account linking

    #region Proxy methods

    /// <summary>
    /// Sends a message to a Discord channel with the specified ID. Without any mentions.
    /// </summary>
    public async Task SendMessageAsync(ulong channelId, string message)
    {
        if (_client == null)
        {
            return;
        }

        var channel = await _client.Rest.GetChannelAsync(channelId) as TextChannel;
        if (channel == null)
        {
            _sawmill.Error("Tried to send a message to Discord but the channel {Channel} was not found.", channel);
            return;
        }

        await channel.SendMessageAsync(new MessageProperties()
        {
            AllowedMentions = AllowedMentionsProperties.None,
            Content = message,
        });
    }

    #endregion
}
