// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server._Funkystation.Atmos.Events
{
    [ByRefEvent]
    public readonly record struct TileExposedEvent(Vector2i Tile, float Temperature, float Volume, EntityUid? SparkSource);
}
