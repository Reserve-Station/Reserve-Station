// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared._White.Bark;

[Serializable, NetSerializable]
public record struct BarkData(float Pitch, float Volume, float Pause, bool Enabled = true);
