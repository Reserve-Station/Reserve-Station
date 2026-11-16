// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Pinpointer;
using Content.Goobstation.Shared.Supermatter.Components;
using Content.Goobstation.Shared.Supermatter.Consoles;
using Content.Goobstation.Shared.Supermatter.Monitor;
using Content.Shared.Atmos;
using Content.Shared.Pinpointer;
using Content.Shared.Radiation.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Map.Components;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Content.Goobstation.Server.Supermatter.Console.Systems;

public sealed partial class SupermatterConsoleSystem : SharedSupermatterConsoleSystem
{
    [Dependency] private UserInterfaceSystem _userInterfaceSystem = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    // [Dependency] private NavMapSystem _navMapSystem = default!; // Reserve edit: Fix warnings

    private const float UpdateTime = 1.0f;

    // Note: this data does not need to be saved
    private float _updateTimer = UpdateTime; // Reserve: update throttle

    public override void Initialize()
    {
        base.Initialize();

        // Console events
        SubscribeLocalEvent<SupermatterConsoleComponent, ComponentInit>(OnConsoleInit);
        SubscribeLocalEvent<SupermatterConsoleComponent, EntParentChangedMessage>(OnConsoleParentChanged);
        SubscribeLocalEvent<SupermatterConsoleComponent, SupermatterConsoleFocusChangeMessage>(OnFocusChangedMessage);
        SubscribeLocalEvent<SupermatterConsoleComponent, BoundUIOpenedEvent>(OnUIOpened); // Reserve: ui refresh

        // Grid events
        SubscribeLocalEvent<GridSplitEvent>(OnGridSplit);
    }

    #region Event handling

    private void OnConsoleInit(EntityUid uid, SupermatterConsoleComponent component, ComponentInit args) =>
        InitalizeConsole(uid, component);

    private void OnConsoleParentChanged(EntityUid uid, SupermatterConsoleComponent component, EntParentChangedMessage args) =>
        InitalizeConsole(uid, component);

    // Reserve start
    private void OnFocusChangedMessage(EntityUid uid, SupermatterConsoleComponent component, SupermatterConsoleFocusChangeMessage args)
    {
        if (component.FocusSupermatter == args.FocusSupermatter)
            return;

        component.FocusSupermatter = args.FocusSupermatter;
        UpdateUIState(uid, component);
    }

    private void OnUIOpened(EntityUid uid, SupermatterConsoleComponent component, BoundUIOpenedEvent args)
    {
        if (args.UiKey is not SupermatterConsoleUiKey.Key)
            return;

        UpdateUIState(uid, component);
    }
    // Reserve end

    private void OnGridSplit(ref GridSplitEvent args)
    {
        // Collect grids
        var allGrids = args.NewGrids.ToList();

        if (!allGrids.Contains(args.Grid))
            allGrids.Add(args.Grid);

        // Update supermatter monitoring consoles that stand upon an updated grid
        var query = AllEntityQuery<SupermatterConsoleComponent, TransformComponent>();
        while (query.MoveNext(out var ent, out var entConsole, out var entXform))
        {
            if (entXform.GridUid == null
                || !allGrids.Contains(entXform.GridUid.Value))
                continue;

            InitalizeConsole(ent, entConsole);
        }
    }

    #endregion

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Reserve start: update throttle
        _updateTimer += frameTime;
        if (_updateTimer < UpdateTime)
            return;

        _updateTimer -= UpdateTime;
        // Reserve end

        // Keep a list of UI entries for each gridUid, in case multiple consoles stand on the same grid
        var supermatterEntriesForEachGrid = new Dictionary<EntityUid, SupermatterConsoleEntry[]>();

        var query = AllEntityQuery<SupermatterConsoleComponent, TransformComponent>();
        while (query.MoveNext(out var ent, out var entConsole, out var entXform))
        {
            // Reserve start: off-grid console
            if (entXform.GridUid == null)
            {
                if (TryComp<AppearanceComponent>(ent, out var offGridAppearance))
                    _appearance.SetData(ent, SupermatterConsoleVisuals.ComputerLayerScreen, (int)SupermatterStatusType.Inactive, offGridAppearance);

                UpdateUIState(ent, Array.Empty<SupermatterConsoleEntry>(), entConsole, entXform);
                continue;
            }
            // Reserve end

            // Make a list of alarm state data for all the supermatters on the grid
            if (!supermatterEntriesForEachGrid.TryGetValue(entXform.GridUid.Value, out var supermatterEntries))
            {
                supermatterEntries = GetSupermatterStateData(entXform.GridUid.Value).ToArray();
                supermatterEntriesForEachGrid[entXform.GridUid.Value] = supermatterEntries;
            }

            // Determine the highest level of status for the console
            var highestStatus = SupermatterStatusType.Inactive;

            foreach (var entry in supermatterEntries)
            {
                var status = entry.EntityStatus;

                if (status > highestStatus)
                    highestStatus = status;
            }

            // Update the appearance of the console based on the highest recorded level of alert
            if (TryComp<AppearanceComponent>(ent, out var entAppearance))
                _appearance.SetData(ent, SupermatterConsoleVisuals.ComputerLayerScreen, (int)highestStatus, entAppearance);

            // If the console UI is open, send UI data to each subscribed session
            UpdateUIState(ent, supermatterEntries, entConsole, entXform);
        }
    }

    public void UpdateUIState
        (EntityUid uid,
        SupermatterConsoleEntry[] supermatterStateData,
        SupermatterConsoleComponent component,
        TransformComponent xform)
    {
        if (!_userInterfaceSystem.IsUiOpen(uid, SupermatterConsoleUiKey.Key))
            return;

        // Reserve start: off-grid console
        SupermatterFocusData? focusSupermatterData = null;
        if (xform.GridUid != null)
        {
            var gridUid = xform.GridUid.Value;
            focusSupermatterData = GetFocusSupermatterData(uid, GetEntity(component.FocusSupermatter), gridUid);
        }
        // Reserve end

        // Set the UI state
        _userInterfaceSystem.SetUiState(uid, SupermatterConsoleUiKey.Key,
            new SupermatterConsoleBoundInterfaceState(supermatterStateData, focusSupermatterData));
    }

    // Reserve start: ui refresh
    public void UpdateUIState(EntityUid uid, SupermatterConsoleComponent? component = null, TransformComponent? xform = null)
    {
        if (!_userInterfaceSystem.IsUiOpen(uid, SupermatterConsoleUiKey.Key)
            || !Resolve(uid, ref component, ref xform))
            return;

        var supermatterEntries = xform.GridUid != null
            ? GetSupermatterStateData(xform.GridUid.Value).ToArray()
            : Array.Empty<SupermatterConsoleEntry>();

        UpdateUIState(uid, supermatterEntries, component, xform);
    }
    // Reserve end

    private List<SupermatterConsoleEntry> GetSupermatterStateData(EntityUid gridUid)
    {
        var supermatterStateData = new List<SupermatterConsoleEntry>();

        var querySupermatters = AllEntityQuery<SupermatterComponent, TransformComponent>();
        while (querySupermatters.MoveNext(out var ent, out var entSupermatter, out var entXform))
        {
            if (entXform.GridUid != gridUid
                || !entXform.Anchored)
                continue;

            // Create entry
            var netEnt = GetNetEntity(ent);

            var entry = new SupermatterConsoleEntry
                (netEnt,
                MetaData(ent).EntityName,
                entSupermatter.Status);

            supermatterStateData.Add(entry);
        }

        return supermatterStateData;
    }

    private SupermatterFocusData? GetFocusSupermatterData(EntityUid uid, EntityUid? focusSupermatter, EntityUid gridUid)
    {
        if (focusSupermatter == null)
            return null;

        // Reserve start: focus transform
        if (!TryComp(focusSupermatter.Value, out TransformComponent? focusSupermatterXform))
            return null;
        // Reserve end

        if (!focusSupermatterXform.Anchored
            || focusSupermatterXform.GridUid != gridUid
            || !TryComp<SupermatterComponent>(focusSupermatter.Value, out var sm)
            || !TryComp<RadiationSourceComponent>(focusSupermatter.Value, out var radiationComp))
            return null;

        var gases = sm.GasStorage;
        var tempThreshold = Atmospherics.T0C + sm.HeatPenaltyThreshold;

        return new SupermatterFocusData(
            GetNetEntity(focusSupermatter.Value),
            GetIntegrity(sm),
            sm.Power,
            radiationComp.Intensity,
            gases.Sum(gas => gases[gas.Key]),
            sm.Temperature,
            tempThreshold * sm.DynamicHeatResistance,
            sm.WasteMultiplier,
            sm.GasEfficiency * 100,
            sm.GasStorage);
    }

    public float GetIntegrity(SupermatterComponent sm)
    {
        var integrity = sm.Damage / sm.DelaminationPoint;
        integrity = (float)Math.Round(100 - integrity * 100, 2);
        integrity = integrity < 0 ? 0 : integrity;
        return integrity;
    }

    private void InitalizeConsole(EntityUid uid, SupermatterConsoleComponent component)
    {
        var xform = Transform(uid);

        if (xform.GridUid == null)
            return;

        Dirty(uid, component);
    }
}
