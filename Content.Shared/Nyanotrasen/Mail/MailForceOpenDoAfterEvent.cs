using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Mail;

[Serializable, NetSerializable]
public sealed partial class MailForceOpenDoAfterEvent : SimpleDoAfterEvent;
