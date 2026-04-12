// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Harmony.Objectives.Systems;

namespace Content.Server._Harmony.Objectives.Components;

[RegisterComponent, Access(typeof(SelfAndTargetSurviveConditionSystem))]
public sealed partial class SelfAndTargetSurviveConditionComponent : Component;
