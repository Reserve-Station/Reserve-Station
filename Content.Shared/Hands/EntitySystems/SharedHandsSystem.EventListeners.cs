using Content.Shared.Hands.Components;
using Content.Shared.Stunnable;

namespace Content.Shared.Hands.EntitySystems;

/// <summary>
/// This is for events that don't affect normal hand functions but do care about hands.
/// </summary>
public abstract partial class SharedHandsSystem
{
    private void InitializeEventListeners()
    {
        SubscribeLocalEvent<HandsComponent, GetStandUpTimeEvent>(OnStandupArgs);
        SubscribeLocalEvent<HandsComponent, KnockedDownRefreshEvent>(OnKnockedDownRefresh);
    }

    /// <summary>
    /// Reduces the time it takes to stand up based on the number of hands we have available.
    /// </summary>
    private void OnStandupArgs(Entity<HandsComponent> ent, ref GetStandUpTimeEvent time)
    {
        if (!HasComp<KnockedDownComponent>(ent))
            return;

        var hands = GetEmptyHandCount(ent.Owner);

        if (hands == 0)
            return;

        time.DoAfterTime *= (float) ent.Comp.Count / (hands + ent.Comp.Count);
    }

    private void OnKnockedDownRefresh(Entity<HandsComponent> ent, ref KnockedDownRefreshEvent args)
    {
        // Reserve edit start: Fix crawling - You can crawl with your elbows, or even legs, though slowly.
        /*
        var freeHands = CountFreeHands(ent.AsNullable());
        var totalHands = GetHandCount(ent.AsNullable());

        // Can't crawl around without any hands.
        // Entities without the HandsComponent will always have full crawling speed.
        if (totalHands == 0)
            args.SpeedModifier = 0f;
        else
            args.SpeedModifier *= (float)freeHands / totalHands;
        */
        var totalHands = GetHandCount(ent.AsNullable());
        var totalSizePointsModifier = GetHeldItemSumSizePoints(ent.AsNullable()) * 0.01f;  // 1% for each point
        var totalHandsModifier = 1.0f / (totalHands * 0.5f);  // 1 hand is 2x reduction, 2 hands is 1x, 4 hands 0.5x, etc.

        // Entities without the HandsComponent will always have full crawling speed.
        if (totalHands == 0)
            args.SpeedModifier = 0.15f;
        else
            args.SpeedModifier *= Math.Max(1.0f - totalSizePointsModifier * totalHandsModifier, 0.05f);
        // Reserve edit end: Fix crawling
    }
}
