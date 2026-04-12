// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Harmony.BloodBrothers.EntitySystems;

namespace Content.Server._Harmony.Objectives.Components;

/// <summary>
/// Marker component to show that an objective should be removed when the blood brother is deconverted.
/// </summary>
[RegisterComponent, Access(typeof(BloodBrotherSystem))]
public sealed partial class ConvertedBloodBrotherObjectiveComponent : Component;
