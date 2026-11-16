// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis; // Reserve edit: Vent crawl air
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Systems;
using Content.Shared.NodeContainer;
// using Content.Server.NodeContainer.EntitySystems; // Reserve edit: Vent crawl air
using Content.Server.NodeContainer.Nodes;
using Content.Shared._Starlight.VentCrawling.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Atmos; // Reserve edit: Vent crawl air
using Content.Shared.Interaction.Events;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Throwing;

namespace Content.Server._Starlight.VentCrawling;

public sealed class BeingVentCrawSystem : EntitySystem
{
    // [Dependency] private readonly NodeContainerSystem _nodeContainer = default!; // Reserve edit: Vent crawl air

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BeingVentCrawlerComponent, InhaleLocationEvent>(OnInhaleLocation);
        SubscribeLocalEvent<BeingVentCrawlerComponent, ExhaleLocationEvent>(OnExhaleLocation);
        SubscribeLocalEvent<BeingVentCrawlerComponent, AtmosExposedGetAirEvent>(OnGetAir);

        // List of all actions you cant do while in the pipe
        SubscribeLocalEvent<BeingVentCrawlerComponent, ActionAttemptEvent>(OnActionAttempt);
        SubscribeLocalEvent<BeingVentCrawlerComponent, AttackAttemptEvent>(OnAttackAttempt);
        SubscribeLocalEvent<BeingVentCrawlerComponent, PickupAttemptEvent>(OnPickupAttempt);
        SubscribeLocalEvent<BeingVentCrawlerComponent, ThrowAttemptEvent>(OnThrowAttempt);
        SubscribeLocalEvent<BeingVentCrawlerComponent, DropAttemptEvent>(OnDropAttempt);
        SubscribeLocalEvent<BeingVentCrawlerComponent, IsUnequippingAttemptEvent>(OnUnequiptAttempt);
        SubscribeLocalEvent<BeingVentCrawlerComponent, IsEquippingAttemptEvent>(OnEquiptAttempt);

    }

    // Reserve edit start: Vent crawl air - crawlers don't suffocate in thin pipes
    private void OnGetAir(EntityUid uid, BeingVentCrawlerComponent component, ref AtmosExposedGetAirEvent args)
    {
        if (!TryGetPipeAir(component, out var holder, out var pipeAir))
            return;

        args.Gas = GetFeltAir(holder, pipeAir);
        args.Handled = true;
    }

    private void OnInhaleLocation(EntityUid uid, BeingVentCrawlerComponent component, ref InhaleLocationEvent args)
    {
        if (TryGetPipeAir(component, out _, out var pipeAir))
            args.Gas = TakeBreath(pipeAir, args.Respirator.BreathVolume);
    }

    private void OnExhaleLocation(EntityUid uid, BeingVentCrawlerComponent component, ref ExhaleLocationEvent args)
    {
        if (TryGetPipeAir(component, out _, out var pipeAir))
            args.Gas = pipeAir;
    }

    private bool TryGetPipeAir(
        BeingVentCrawlerComponent component,
        [NotNullWhen(true)] out VentCrawlerHolderComponent? holder,
        [NotNullWhen(true)] out GasMixture? air)
    {
        air = null;

        if (!TryComp(component.Holder, out holder) || holder.CurrentTube is not { } tube)
            return false;

        if (!TryComp(tube, out NodeContainerComponent? nodeContainer))
            return false;

        foreach (var node in nodeContainer.Nodes.Values)
        {
            if (node is not PipeNode pipe)
                continue;

            air = pipe.Air;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Pipes are often near vacuum, which would suffocate anything small enough to crawl in them.
    /// So a thin pipe's gas mix is felt at one atmosphere. High pressure and an empty pipe are felt as they are.
    /// </summary>
    private static GasMixture GetFeltAir(VentCrawlerHolderComponent holder, GasMixture pipeAir)
    {
        if (!TryGetThinAirVolume(pipeAir, out var volume))
            return pipeAir;

        holder.Air.CopyFrom(pipeAir);
        holder.Air.Volume = volume;
        return holder.Air;
    }

    /// <summary>
    /// Takes a breath worth of gas out of a thin pipe, as if the pipe was at one atmosphere.
    /// </summary>
    private static GasMixture TakeBreath(GasMixture pipeAir, float breathVolume)
    {
        if (!TryGetThinAirVolume(pipeAir, out var volume))
            return pipeAir;

        var breath = pipeAir.RemoveRatio(breathVolume / volume);
        breath.Volume = breathVolume;
        return breath;
    }

    /// <summary>
    /// Gets the volume the gas would take up at one atmosphere, if it's thinner than that and not empty.
    /// </summary>
    private static bool TryGetThinAirVolume(GasMixture air, out float volume)
    {
        volume = air.TotalMoles * Atmospherics.R * air.Temperature / Atmospherics.OneAtmosphere;
        return volume > 0f && volume < air.Volume;
    }
    // Reserve edit end: Vent crawl air

    private void OnActionAttempt(EntityUid uid, BeingVentCrawlerComponent component, ref ActionAttemptEvent args)
        => args.Cancelled = true;

    private void OnAttackAttempt(EntityUid uid, BeingVentCrawlerComponent component, ref AttackAttemptEvent args)
        => args.Cancel();

    private void OnPickupAttempt(EntityUid uid, BeingVentCrawlerComponent component, ref PickupAttemptEvent args)
        => args.Cancel();

    private void OnThrowAttempt(EntityUid uid, BeingVentCrawlerComponent component, ref ThrowAttemptEvent args)
        => args.Cancel();

    private void OnDropAttempt(EntityUid uid, BeingVentCrawlerComponent component, ref DropAttemptEvent args)
        => args.Cancel();

    private void OnUnequiptAttempt(EntityUid uid, BeingVentCrawlerComponent component, ref IsUnequippingAttemptEvent args)
        => args.Cancel();

    private void OnEquiptAttempt(EntityUid uid, BeingVentCrawlerComponent component, ref IsEquippingAttemptEvent args)
        => args.Cancel();
}
