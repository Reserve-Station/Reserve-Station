// SPDX-FileCopyrightText: 2025 Kill_Me_I_Noobs <118206719+Vonsant@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * All right reserved to CrystallEdge.
 *
 * BUT this file is sublicensed under MIT License
 *
 */

using Content.Server._CorvaxNext.BiomeSpawner.EntitySystems;
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Prototypes;

namespace Content.Server._CorvaxNext.BiomeSpawner.Components;

/// <summary>
/// fills the tile in which it is located with the contents of the biome. Includes: tile, decals and entities
/// </summary>
[RegisterComponent, Access(typeof(BiomeSpawnerSystem))]
public sealed partial class BiomeSpawnerComponent : Component
{
    [DataField]
    public ProtoId<BiomeTemplatePrototype> Biome = "Grasslands";
}
