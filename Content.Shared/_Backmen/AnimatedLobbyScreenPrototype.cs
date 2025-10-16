// SPDX-FileCopyrightText: 2025 Egorql <Egorkashilkin@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Shared._Backmen;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype]
public sealed partial class AnimatedLobbyScreenPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("path", required: true)]
    public string Path { get; private set; } = default!;
}
