// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared._EinsteinEngines.Paint;

public abstract class SharedPaintSystem : EntitySystem
{
    public virtual void UpdateAppearance(EntityUid uid, PaintedComponent? component = null) { }
}
