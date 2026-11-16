// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.Antag;

[RegisterComponent, NetworkedComponent]
public sealed partial class GlobalAntagonistComponent : Component
{
    [DataField(required: true)]
    public ProtoId<AntagonistPrototype>? AntagonistPrototype;
}
