// SPDX-FileCopyrightText: 2025 CSH <menelectro3@gmail.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._FreakyStation.CyberPunk
{
    [RegisterComponent]
    public partial class ActionSpawnOnUseComponent : Component
    {
        [DataField("spawnPrototype")]
        public string SpawnPrototype { get; set; } = string.Empty;
        // Впереди гремучий лес - шкибиди доп доп ес ес
        [DataField("spawnSlot")]
        public string SpawnSlot { get; set; } = string.Empty;

        [DataField("removeOnSecondUse")]
        public bool RemoveOnSecondUse { get; set; } = false;

        [DataField("dropPreviousItem")]
        public bool DropPreviousItem { get; set; } = false;

        [DataField("spawnedItem")]
        public EntityUid? SpawnedItem { get; set; }
    }
}
