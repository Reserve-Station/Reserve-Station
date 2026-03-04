// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Reserve.LenaApi;

/// <summary>
///     Reserve API cvars
/// </summary>
[CVarDefs]
public sealed class LenaApiCVars : CVars
{
    public static readonly CVarDef<string> ApiKey =
        CVarDef.Create("lena.api_key", "", CVar.CONFIDENTIAL | CVar.SERVERONLY);

    public static readonly CVarDef<bool> ApiIntegration =
        CVarDef.Create("lena.api_integration", false, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<bool> RequireAuth =
        CVarDef.Create("lena.require_auth", true, CVar.SERVERONLY);

    public static readonly CVarDef<string> AuthUri =
        CVarDef.Create("lena.auth_uri", "https://lena.reserve-station.space/v1/auth/login", CVar.SERVERONLY);

    public static readonly CVarDef<string> ShopUri =
        CVarDef.Create("lena.shop_uri", "https://reserve-station.space/shop", CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<string> BaseUri =
        CVarDef.Create("lena.base_uri", "https://lena.reserve-station.space/v1", CVar.SERVERONLY);

    public static readonly CVarDef<bool> IgnoreSubName =
        CVarDef.Create("lena.ignore_sub_name", true, CVar.SERVERONLY);

    public static readonly CVarDef<bool> DoocEnabled =
        CVarDef.Create("lena.dooc_enabled", true, CVar.SERVER);
}
