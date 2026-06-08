// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Shared.Roles
{
    [Prototype]
    public sealed class AntagCategoryPrototype : IPrototype
    {
        [IdDataField]
        public string ID { get; set; } = string.Empty;
        [DataField]
        public string Name { get; set; } = string.Empty;
    }
}
