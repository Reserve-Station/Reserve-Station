// SPDX-FileCopyrightText: 2025 Egorql <Egorkashilkin@gmail.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
namespace Content.Shared.ERP;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ERPComponent : Component
{
    [DataField, AutoNetworkedField] public bool Erp;
    [DataField, AutoNetworkedField] public float ActualLove = 0;
    [DataField, AutoNetworkedField] public float Love = 0;
    [DataField, AutoNetworkedField] public TimeSpan LoveDelay;
    [DataField, AutoNetworkedField] public TimeSpan TimeFromLastErp;
}

[Serializable, NetSerializable]
public enum InteractionKey
{
    Key
}