// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading.Tasks;

namespace Content.Server._Reserve.LenaApi;

public sealed partial class ApiWrapper
{
    public Task<Result<SubTierList>> GetDonorsTiers()
    {
        return Send<SubTierList>(() => _httpClient.GetAsync("v1/donors/tiers"));
    }

    public record SubTierList(List<SubTierList.Entry> SubTiers)
    {
        public record Entry(int Id, string Value, string Label);

        public Dictionary<int, Entry> AsDictionary()
        {
            return SubTiers.ToDictionary(entry => entry.Id);
        }
    }
}
