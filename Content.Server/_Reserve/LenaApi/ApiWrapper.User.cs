// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Content.Server._Reserve.LenaApi;

public sealed partial class ApiWrapper
{
    public Task<Result<UserRead>> GetUser(string ss14Id)
    {
        return Send<UserRead>(() => _httpClient.GetAsync("v1/user/get/" + ss14Id));
    }

    public Task<Result<UserRead>> PatchUser(int id, UserAdminModify data)
    {
        return Send<UserRead>(() => _httpClient.PatchAsJsonAsync($"v1/user/{id}", data, _jsonSerializerOptions));
    }

    public record UserAdminModify
    {
        public int? CurrentSubTier { get; init; }
        public Optional<string> UsernameColor { get; init; }
        public int? ReserveCoins { get; init; }
        public int? DonateCoins { get; init; }
    }

    public record UserRead(
        string Ss14Id,
        string Name,
        int CurrentSubTier,
        string? SubExpiresAt,
        int Permissions,
        string? UsernameColor,
        int ReserveCoins,
        int DonateCoins,
        int Id,
        string CreatedAt,
        DiscordUserRead? DiscordProfile,
        ProfileRead? Profile,
        List<string> PermissionsList
    );

    public record DiscordUserRead(
        string DiscordId,
        string Username,
        string GlobalName,
        string Avatar
    );

    public record ProfileRead(
        string Status,
        bool IsPrivate,
        bool IsPostsAllowed,
        bool IsInventoryHidden,
        bool IsCharactersHidden
    );
}
