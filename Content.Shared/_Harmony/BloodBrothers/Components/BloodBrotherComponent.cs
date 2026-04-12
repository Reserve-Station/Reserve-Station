// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.BloodBrothers.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class BloodBrotherComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? Brother;

    [DataField]
    public ProtoId<FactionIconPrototype> BloodBrotherIcon = "BloodBrotherFaction";

    [DataField]
    public TimeSpan? DeconversionStunTime = TimeSpan.FromSeconds(3);

    public override bool SessionSpecific => true;
}
