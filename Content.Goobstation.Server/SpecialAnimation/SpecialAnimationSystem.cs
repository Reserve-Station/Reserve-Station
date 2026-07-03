// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Roudenn <romabond091@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Shared.SpecialAnimation;
using Content.Shared._EstacaoPirata.Cards.Card;  // Reserve edit: Fix special animation for cards
using Content.Shared.Interaction.Events;  // Reserve edit: Fix special animation for cards
using JetBrains.Annotations;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Goobstation.Server.SpecialAnimation;

public sealed class SpecialAnimationSystem : SharedSpecialAnimationSystem
{
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IEntityManager _entity = default!;  // Reserve edit: Fix special animation for cards

    // Reserve edit start: Fix special animation for cards
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpecialAnimationOnUseComponent, UseInHandEvent>(OnUsed);
    }
    // Reserve edit end: Fix special animation for cards

    // Reserve edit start: Fix special animation for cards
    private void OnUsed(Entity<SpecialAnimationOnUseComponent> ent, ref UseInHandEvent args)
    {
        var xform = Transform(args.User);
        args.ApplyDelay = true;

        if (_entity.TryGetComponent<CardComponent>(ent.Owner, out var card) && card.Flipped)  // For cards, only proceed if the card is face-up.
            return;

        var animation = SpecialAnimationData.DefaultAnimation;
        if (_protoMan.TryIndex(ent.Comp.AnimationDataId, out var animationProto))
            animation = animationProto.Animation;

        if (ent.Comp.OverrideText != null)
            animation = animation.WithText(ent.Comp.OverrideText);

        switch (ent.Comp.BroadcastType)
        {
            case SpecialAnimationBroadcastType.Local:
                PlayAnimationOnUse(args.User, Filter.Entities([args.User]), animation);
                break;
            case SpecialAnimationBroadcastType.Pvs:
                PlayAnimationOnUse(args.User, Filter.Pvs(args.User, entityManager: EntityManager), animation);
                break;
            case SpecialAnimationBroadcastType.Grid:
                PlayAnimationOnUse(args.User, Filter.BroadcastGrid(xform.ParentUid), animation);
                break;
            case SpecialAnimationBroadcastType.Map:
                PlayAnimationOnUse(args.User, Filter.BroadcastMap(xform.MapID), animation);
                break;
            case SpecialAnimationBroadcastType.Global:
                PlayAnimationOnUse(args.User, Filter.Broadcast(), animation);
                break;
        }
    }
    // Reserve edit end: Fix special animation for cards

    /// <summary>
    /// Plays a special attack animation.
    /// </summary>
    /// <param name="sprite">Entity to take the sprite from</param>
    /// <param name="player">Entity to show the animation</param>
    /// <param name="overrideText">If specified, will override the name that is located inside animation data</param>
    /// <param name="animationData">Options to show the animation</param>
    [PublicAPI]
    public override void PlayAnimationForEntity(SpriteSpecifier sprite, EntityUid player, SpecialAnimationData? animationData = null, string? overrideText = null)
    {
        animationData ??= SpecialAnimationData.DefaultAnimation;
        animationData.Sprite = sprite;

        if (overrideText != null)
            animationData = animationData.WithText(overrideText);

        var ev = new SpecialAnimationEvent { AnimationData = animationData };
        RaiseNetworkEvent(ev, player);
    }

    /// <summary>
    /// Plays a special attack animation, and loads the sprite entity
    /// in PVS for the filter for a small amount of time.
    /// </summary>
    /// <param name="sprite">Entity to take the sprite from</param>
    /// <param name="filter">Entities to show the animation for</param>
    /// <param name="overrideText">If specified, will override the name that is located inside animation data</param>
    /// <param name="animationData">Options to show the animation</param>
    [PublicAPI]
    public override void PlayAnimationFiltered(SpriteSpecifier sprite, Filter filter, SpecialAnimationData? animationData = null, string? overrideText = null)
    {
        animationData ??= SpecialAnimationData.DefaultAnimation;
        animationData.Sprite = sprite;

        if (overrideText != null)
            animationData = animationData.WithText(overrideText);

        var ev = new SpecialAnimationEvent { AnimationData = animationData };
        RaiseNetworkEvent(ev, filter);
    }

    public override void PlayAnimationFiltered(
        SpriteSpecifier sprite,
        Filter filter,
        ProtoId<SpecialAnimationPrototype>? animationDataId = null,
        string? overrideText = null)
    {
        if (!_protoMan.TryIndex(animationDataId, out var animationPrototype))
            return;

        PlayAnimationFiltered(sprite, filter, animationPrototype.Animation, overrideText);
    }

    // Reserve edit start: Fix special animation for cards

    /// <summary>
    /// Plays a special attack animation, and loads the sprite entity
    /// in PVS for the filter for a small amount of time.
    /// </summary>
    /// <param name="user">Entity to take the sprite from</param>
    /// <param name="filter">Entities to show the animation for</param>
    /// <param name="overrideText">If specified, will override the name that is located inside animation data</param>
    /// <param name="animationData">Options to show the animation</param>
    [PublicAPI]
    public override void PlayAnimationOnUse(EntityUid user, Filter filter, SpecialAnimationData? animationData = null, string? overrideText = null)
    {
        animationData ??= SpecialAnimationData.DefaultAnimation;
        animationData.Source = _entity.GetNetEntity(user);

        if (overrideText != null)
            animationData = animationData.WithText(overrideText);

        var ev = new SpecialAnimationEvent { AnimationData = animationData };
        RaiseNetworkEvent(ev, filter);
    }

    public override void PlayAnimationOnUse(
        EntityUid user,
        Filter filter,
        ProtoId<SpecialAnimationPrototype>? animationDataId = null,
        string? overrideText = null)
    {
        if (!_protoMan.TryIndex(animationDataId, out var animationPrototype))
            return;

        PlayAnimationOnUse(user, filter, animationPrototype.Animation, overrideText);
    }

    // Reserve edit end: Fix special animation for cards
}
