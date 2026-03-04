// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading.Tasks;

namespace Content.Server._Reserve.LenaApi;

public sealed record User
{
    public int Id;
    public string Ss14Id;
    public int DonateCoins;
    public int ReserveCoins;
    public int CurrentSubTier;
    public List<ApiWrapper.ItemRead> UsableItems;
    public Color? UsernameColor;
    public DateTime? SubExpiresAt;

    public User(ApiWrapper.UserRead userRead, ApiWrapper.InventoryRead inventory)
    {
        UsableItems = inventory.Items.ConvertAll(e => e.Item).FindAll(item => item.CanBeUsedIngame);

        if (userRead.SubExpiresAt != null)
            SubExpiresAt = DateTime.Parse(userRead.SubExpiresAt);

        Id = userRead.Id;
        Ss14Id = userRead.Ss14Id;
        DonateCoins = userRead.DonateCoins;
        ReserveCoins = userRead.ReserveCoins;
        CurrentSubTier = userRead.CurrentSubTier;
        CurrentSubTier = userRead.CurrentSubTier;
        UsernameColor = Color.TryFromHex(userRead.UsernameColor);
    }

    public void UpdateFromUserRead(ApiWrapper.UserRead userRead)
    {
        Id = userRead.Id;
        Ss14Id = userRead.Ss14Id;
        DonateCoins = userRead.DonateCoins;
        ReserveCoins = userRead.ReserveCoins;
        CurrentSubTier = userRead.CurrentSubTier;
        CurrentSubTier = userRead.CurrentSubTier;
        UsernameColor = Color.TryFromHex(userRead.UsernameColor);
    }

    public bool HasActiveSub(out int subLevel)
    {
        if (SubExpiresAt == null || SubExpiresAt > DateTime.Now)
        {
            subLevel = CurrentSubTier;
            return true;
        }
        subLevel = 0;
        return false;
    }

    public async Task<bool> ModifyBalance(ApiWrapper wrapper, int? reserveCoins = null, int? donateCoins = null, string? comment = null)
    {
        var response = await wrapper.PostEditBalance(Id,
            new()
        {
            ReserveCoins = reserveCoins,
            DonateCoins = donateCoins,
            Comment = comment
        });

        if (response.Value != null)
        {
            DonateCoins = response.Value.DonateCoins ?? DonateCoins;
            ReserveCoins = response.Value.ReserveCoins ?? ReserveCoins;
        }

        return response.IsSuccess;
    }

    public async Task<bool> ModifyUser(ApiWrapper wrapper, ApiWrapper.UserAdminModify userModify)
    {
        var response = await wrapper.PatchUser(Id, userModify);

        if (response.Value != null)
        {
            UpdateFromUserRead(response.Value);
        }

        return response.IsSuccess;
    }
}
