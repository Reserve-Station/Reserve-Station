// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading.Tasks;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Content.Shared._Reserve.LenaApi;

namespace Content.Server._Reserve.LenaApi;

public sealed class LenaApiManager
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    private ISawmill _sawmill = default!;

    private ApiWrapper? _wrapper;
    public ApiWrapper? Wrapper => _wrapper;
    [ViewVariables] private readonly Dictionary<string, User> _users = new();
    [ViewVariables] private Dictionary<int, ApiWrapper.ItemRarityList.Entry> _rarityNames = new();
    [ViewVariables] private Dictionary<int, ApiWrapper.SubTierList.Entry> _subLevelNames = new();

    private readonly Dictionary<string, Action<ICommonSession, ApiWrapper.ItemRead>> _itemActions = new();
    private readonly Dictionary<string, string> _itemIcons = new();
    private readonly Dictionary<string, Dictionary<string, AntagRuleConfig>> _antagRules = new();
    private readonly Dictionary<string, TokenConditions> _tokenConditions = new();
    private readonly Dictionary<NetUserId, HashSet<string>> _lockedOutTokens = new();
    private readonly Dictionary<NetUserId, HashSet<string>> _processingTokens = new();
    private readonly HashSet<NetUserId> _globallyLockedPlayers = new();
    private readonly Dictionary<NetUserId, Action<string>> _inventoryRemoveCallbacks = new();
    private readonly Dictionary<string, List<string>> _cosmeticItems = new();

    private string ApiToken => _configurationManager.GetCVar(LenaApiCVars.ApiKey);
    public bool IsIntegrationEnabled => _configurationManager.GetCVar(LenaApiCVars.ApiIntegration);
    public bool IsAuthRequired => _configurationManager.GetCVar(LenaApiCVars.RequireAuth);
    public string BaseUri => _configurationManager.GetCVar(LenaApiCVars.BaseUri);

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("lena-api");
        _wrapper = new ApiWrapper(BaseUri, () => ApiToken);
        _configurationManager.OnValueChanged(LenaApiCVars.BaseUri, newUri => _wrapper.SetBaseUri(newUri), true);
        _configurationManager.OnValueChanged(LenaApiCVars.ApiIntegration, newUri => _ = UpdateData(), true);

        _ = UpdateData();
    }

    public async Task UpdateData()
    {
        if (_wrapper == null)
            return;

        var subLevelNames = await _wrapper.GetDonorsTiers();
        if (subLevelNames is { IsSuccess: true, Value: not null })
        {
            _subLevelNames = subLevelNames.Value.AsDictionary();
        }

        var rarityNames = await _wrapper.GetInventoryRarities();
        if (rarityNames is { IsSuccess: true, Value: not null })
        {
            _rarityNames = rarityNames.Value.AsDictionary();
        }
    }

    public string? GetSubLevelName(int subLevel)
    {
        _subLevelNames.TryGetValue(subLevel, out var entry);
        return entry?.Label;
    }

    public string? GetRarityName(int rarity)
    {
        _rarityNames.TryGetValue(rarity, out var entry);
        return entry?.Label;
    }

    public async Task UpdateUserData(NetUserId netUserId)
    {
        var userRequest = await GetUserFromApi(netUserId.ToString());

        if (!userRequest.IsSuccess || userRequest.Value == null)
            return;

        await UpdateUserData(netUserId, userRequest.Value);
    }

    public async Task UpdateUserData(NetUserId netUserId, ApiWrapper.UserRead userReadData)
    {
        var inventoryRequest = await GetInventoryFromApi(userReadData.Id);

        if (!inventoryRequest.IsSuccess || inventoryRequest.Value == null)
        {
            _sawmill.Error(
                $"Could not read inventory from API for user {userReadData.Id}, Error = {inventoryRequest.Error}");
            return;
        }

        _users[netUserId.ToString()] = new User(userReadData, inventoryRequest.Value);
    }

    public User? GetUser(NetUserId netUserId)
    {
        _users.TryGetValue(netUserId.ToString(), out var user);
        return user;
    }

    public async Task<string?> ShouldDenyConnection(NetUserId netUserId)
    {
        if (IsIntegrationEnabled && IsAuthRequired)
        {
            var response = await GetUserFromApi(netUserId.ToString());
            if (!response.IsSuccess)
            {
                if (response.Error is ApiWrapper.NotFoundError)
                    return Loc.GetString("reserve-auth-required");

                _sawmill.Error($"Got unhandled response while denying user {netUserId}:\n{response.Error}");
                return Loc.GetString("reserve-auth-error");
            }

            if (response.Value != null)
                await UpdateUserData(new NetUserId(netUserId), response.Value);
        }

        return null;
    }

    private async Task<ApiWrapper.Result<T>> Send<T>(Func<ApiWrapper, Task<ApiWrapper.Result<T>>> func)
    {
        if (!IsIntegrationEnabled)
            return ApiWrapper.Result<T>.Failure(new IntegrationDisabledError());

        if (_wrapper == null)
            return ApiWrapper.Result<T>.Failure(new WrapperNotInitializedError());

        return await func(_wrapper);
    }

    #region api methods

    public async Task<ApiWrapper.Result<ApiWrapper.UserRead>> GetUserFromApi(string ss14Id) =>
        await Send<ApiWrapper.UserRead>(wrapper => wrapper.GetUser(ss14Id));

    public async Task<ApiWrapper.Result<ApiWrapper.InventoryRead>> GetInventoryFromApi(int id) =>
        await Send<ApiWrapper.InventoryRead>(wrapper => wrapper.GetInventory(id));

    public async Task<ApiWrapper.Result<ApiWrapper.InventoryModify>> TakeItemFromApi(int userId, int itemId, string? comment = null) =>
        await Send<ApiWrapper.InventoryModify>(wrapper => wrapper.PostInventoryModify(userId,
            new ApiWrapper.InventoryModify { ItemId = itemId, Amount = 1, Comment = comment }));

    #endregion

    #region item icons

    public void RegisterItemIcon(string itemId, string iconPath)
        => _itemIcons[itemId] = iconPath;

    public string? GetItemIcon(string itemId)
    {
        _itemIcons.TryGetValue(itemId, out var path);
        return path;
    }

    #endregion

    #region item actions

    public void RegisterItemAction(string itemId, Action<ICommonSession, ApiWrapper.ItemRead> action)
    {
        _itemActions[itemId] = action;
    }


    public bool TryUseItem(ICommonSession session, string itemId)
    {
        var user = GetUser(session.UserId);
        if (user == null)
            return false;

        var item = user.UsableItems.FirstOrDefault(i => i.ItemId == itemId);
        if (item == null)
            return false;

        if (!_itemActions.TryGetValue(itemId, out var action))
        {
            _sawmill.Warning($"К айтему '{itemId}' не привязан каллбек");
            return false;
        }

        action(session, item);
        return true;
    }

    #endregion

    #region antag rules

    public void RegisterAntagRule(string itemId,
        string ruleId,
        string displayName,
        bool forAlive = false,
        Action<ICommonSession>? forAliveAction = null)
    {
        if (!_antagRules.TryGetValue(itemId, out var rules))
            _antagRules[itemId] = rules = new Dictionary<string, AntagRuleConfig>();
        rules[ruleId] = new AntagRuleConfig(displayName, forAlive, forAliveAction);
    }

    public IReadOnlyDictionary<string, AntagRuleConfig> GetAntagRules(string itemId)
    {
        _antagRules.TryGetValue(itemId, out var rules);
        return rules ?? new Dictionary<string, AntagRuleConfig>();
    }

    #endregion

    #region token conditions

    public void RegisterTokenConditions(string itemId, TokenConditions conditions)
        => _tokenConditions[itemId] = conditions;

    public TokenConditions? GetTokenConditions(string itemId)
    {
        _tokenConditions.TryGetValue(itemId, out var conditions);
        return conditions;
    }

    #endregion

    #region lockout

    public bool IsTokenLockedOut(NetUserId userId, string itemId)
        => _globallyLockedPlayers.Contains(userId)
           || (_lockedOutTokens.TryGetValue(userId, out var tokens) && tokens.Contains(itemId));

    public void LockOutToken(NetUserId userId, string itemId)
    {
        if (!_lockedOutTokens.TryGetValue(userId, out var tokens))
            _lockedOutTokens[userId] = tokens = new HashSet<string>();
        tokens.Add(itemId);
    }

    public void LockOutPlayerGlobally(NetUserId userId)
        => _globallyLockedPlayers.Add(userId);

    public void ClearAllLockouts()
    {
        _lockedOutTokens.Clear();
        _globallyLockedPlayers.Clear();
    }

    public bool TryBeginTokenUse(NetUserId userId, string itemId)
    {
        if (!_processingTokens.TryGetValue(userId, out var set))
            _processingTokens[userId] = set = new HashSet<string>();
        return set.Add(itemId);
    }

    public void EndTokenUse(NetUserId userId, string itemId)
    {
        if (_processingTokens.TryGetValue(userId, out var set))
            set.Remove(itemId);
    }

    #endregion

    #region cosmetic items

    public void RegisterCosmeticItem(string tokenId, string protoId)
    {
        if (!_cosmeticItems.TryGetValue(tokenId, out var list))
            _cosmeticItems[tokenId] = list = new List<string>();
        list.Add(protoId);
    }

    public IReadOnlyList<string> GetCosmeticItems(string tokenId)
    {
        if (_cosmeticItems.TryGetValue(tokenId, out var list))
            return list;
        return Array.Empty<string>();
    }

    #endregion

    #region inventory callbacks

    public void RegisterInventoryRemoveCallback(NetUserId userId, Action<string> callback)
        => _inventoryRemoveCallbacks[userId] = callback;

    public void UnregisterInventoryRemoveCallback(NetUserId userId)
        => _inventoryRemoveCallbacks.Remove(userId);

    public void NotifyItemRemoved(NetUserId userId, string itemId)
    {
        if (_inventoryRemoveCallbacks.TryGetValue(userId, out var callback))
            callback(itemId);
    }

    #endregion

    public sealed record AntagRuleConfig(
        string DisplayName,
        bool ForAlive,
        Action<ICommonSession>? ForAliveAction = null);

    public sealed record TokenConditions(
        CVarDef<int> MinAlive,
        CVarDef<int> MaxAntags,
        CVarDef<float> Chance,
        CVarDef<int>? MinSec = null,
        IReadOnlyList<string>? BlockingRules = null
    );

    public record IntegrationDisabledError : ApiWrapper.ApiError;

    public record WrapperNotInitializedError : ApiWrapper.ApiError;
}
