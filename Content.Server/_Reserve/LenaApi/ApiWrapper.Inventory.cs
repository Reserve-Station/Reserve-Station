// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Content.Server._Reserve.LenaApi;

public sealed partial class ApiWrapper
{
    public Task<Result<InventoryRead>> GetInventory(int id)
    {
        return Send<InventoryRead>(() => _httpClient.GetAsync("v1/inventory/get/" + id));
    }

    public Task<Result<InventoryModify>> PostInventoryModify(int userId, InventoryModify inventoryModify)
    {
        return Send<InventoryModify>(() =>
            _httpClient.PostAsJsonAsync($"v1/inventory/take/{userId}", inventoryModify, _jsonSerializerOptions));
    }

    public Task<Result<BalanceModify>> PostEditBalance(int id, BalanceModify balanceModify)
    {
        return Send<BalanceModify>(() =>
            _httpClient.PostAsJsonAsync($"v1/inventory/editBalance/{id}", balanceModify, _jsonSerializerOptions));
    }

    public Task<Result<ItemRarityList>> GetInventoryRarities()
    {
        return Send<ItemRarityList>(() => _httpClient.GetAsync("v1/inventory/rarities"));
    }

    public record InventoryRead(
        int UserId,
        List<InventoryEntry> Items,
        int TotalItems
    );

    public record InventoryEntry(
        int Id,
        int Amount,
        string CreatedAt,
        string UpdatedAt,
        ItemRead Item
    );

    public record ItemRead(
        int Id,
        string ItemId,
        string ItemName,
        string? ItemImageUrl,
        int Rarity,
        string? Description,
        bool AvailableForPurchase,
        int? Price,
        string? Currency,
        bool CanBeUsedIngame
    );

    public record BalanceModify
    {
        public int? ReserveCoins { get; init; }
        public int? DonateCoins { get; init; }
        public string? Comment { get; init; }
    }

    public record InventoryModify
    {
        public int ItemId { get; init; }
        public int Amount { get; init; }
        public string? Comment { get; init; }
    }

    public record ItemRarityList(List<ItemRarityList.Entry> Rarities)
    {
        public record Entry(int Id, string Value, string Label);

        public Dictionary<int, Entry> AsDictionary()
        {
            return Rarities.ToDictionary(entry => entry.Id);
        }
    }
}
