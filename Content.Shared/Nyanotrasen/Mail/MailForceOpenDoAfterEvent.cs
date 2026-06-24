// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Mail;

[Serializable, NetSerializable]
public sealed partial class MailForceOpenDoAfterEvent : SimpleDoAfterEvent;
