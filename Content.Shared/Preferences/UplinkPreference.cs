// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
    /// <summary>
    /// Type of uplink device used by the traitor
    /// </summary>
    [Serializable, NetSerializable]
    public enum UplinkPreference : byte
    {
        /// <summary>
        /// Standard PDA uplink (20 TC)
        /// </summary>
        PDA,

        /// <summary>
        /// Implanted uplink (18 TC)
        /// </summary>
        Implant,

        /// <summary>
        /// Radio uplink (21 TC)
        /// </summary>
        Radio,

        /// <summary>
        /// Direct telecrystals (25 TC)
        /// </summary>
        Telecrystals
    }
}
