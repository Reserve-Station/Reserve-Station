// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;

namespace Content.Shared._Starlight.Sound;

public readonly struct SoundEvent(SoundSpecifier soundSpecifier, EntityUid source, EntityUid target, AudioParams audioParams)
{
    public readonly SoundSpecifier SoundSpecifier = soundSpecifier;
    public readonly EntityUid Source = source;
    public readonly EntityUid User = target;
    public readonly AudioParams AudioParams = audioParams;
}
