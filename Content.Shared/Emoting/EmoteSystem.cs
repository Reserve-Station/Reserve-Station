// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Damage.Components;  // Reserve edit: Flip & spin antispam

namespace Content.Shared.Emoting;

public sealed class EmoteSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EmoteAttemptEvent>(OnEmoteAttempt);
    }

    public void SetEmoting(EntityUid uid, bool value, EmotingComponent? component = null)
    {
        if (value && !Resolve(uid, ref component))
            return;

        component = EnsureComp<EmotingComponent>(uid);

        if (component.Enabled == value)
            return;

        Dirty(uid, component);
    }

    private void OnEmoteAttempt(EmoteAttemptEvent args)
    {
        if (!TryComp(args.Uid, out EmotingComponent? emote) || !emote.Enabled)
            args.Cancel();

        // Reserve edit start: Flip & spin antispam - can't emote while stamina-stunned
        if (TryComp(args.Uid, out StaminaComponent? stamina) && stamina.Critical)
            args.Cancel();
        // Reserve edit end: Flip & spin antispam
    }
}
