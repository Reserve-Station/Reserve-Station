// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared.Mail
{
    /// <summary>
    /// Stores the visuals for mail.
    /// </summary>
    [Serializable, NetSerializable]
    public enum MailVisuals : byte
    {
        IsLocked,
        IsTrash,
        IsBroken,
        IsFragile,
        IsPriority,
        IsPriorityInactive,
        JobIcon,
    }
}
