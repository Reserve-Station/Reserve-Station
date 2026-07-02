// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.ReagentFires
{
    [Serializable, NetSerializable]
    public enum ReagentPuddleFireVisuals : byte
    {
        OnFire,
        FireState,
        FireColor
    }

    [RegisterComponent, NetworkedComponent]
    public sealed partial class ReagentPuddleFireEffectComponent : Component
    {
    }
}
