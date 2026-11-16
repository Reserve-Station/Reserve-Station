// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Alert;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.OfferItem;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedOfferItemSystem))]
public sealed partial class OfferItemComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public bool IsInOfferMode;

    [DataField, AutoNetworkedField]
    public bool IsInReceiveMode;

    [DataField, AutoNetworkedField]
    public string? Hand;

    [DataField, AutoNetworkedField]
    public EntityUid? Item;

    [DataField, AutoNetworkedField]
    public EntityUid? Target;

    [DataField]
    public float MaxOfferDistance = 2f;

    [DataField]
    public ProtoId<AlertPrototype> OfferAlert = "Offer";

    [DataField, AutoNetworkedField]
    public float Cooldown = 0.25f;

    [DataField, AutoNetworkedField]
    public TimeSpan TargetTime;

    /// <summary>
    /// Calculates cooldown for offering an item.
    /// </summary>
    public void RefreshOfferCooldown(TimeSpan curTime)
    {
        TargetTime = curTime + TimeSpan.FromSeconds(Cooldown);
    }

    /// <summary>
    /// Checks if the offer cooldown has expired.
    /// </summary>
    /// <returns>True if the cooldown has expired, false otherwise.</returns>
    public bool IsOfferCooldownExpired(TimeSpan curTime)
    {
        if (TargetTime == default)
            return true;
        return curTime >= TargetTime;
    }
}

public sealed partial class AcceptOfferAlertEvent : BaseAlertEvent;

