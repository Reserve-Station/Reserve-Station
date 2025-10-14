// SPDX-FileCopyrightText: 2025 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Shared.GameTicking;
using Content.Server.Destructible;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.GameTicking;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Goobstation.Server.Destructible.Thresholds.Behaviors
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class AddGameRuleBehavior : IThresholdBehavior
    {
        [DataField(required: true)]
        public EntProtoId Rule;

        public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
        {
            var ev = new AddGameRuleItemEvent(cause);
            system.EntityManager.EventBus.RaiseLocalEvent(owner, ref ev);

            system.EntityManager.System<GameTicker>().StartGameRule(Rule);
        }
    }
}
