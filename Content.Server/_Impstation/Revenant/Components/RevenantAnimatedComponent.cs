using Content.Goobstation.Maths.FixedPoint;
using Content.Server.Revenant.EntitySystems;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Revenant.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Server.Revenant.Components;

// Reserve edit - reworked together with RevenantAnimatedSystem
/// <summary>
/// An item possessed by a revenant. It hovers in place and hurls itself at nearby living beings,
/// or, for guns, handcuffs and grenades, walks around and uses itself on them.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(RevenantAnimatedSystem))]
[AutoGenerateComponentPause]
public sealed partial class RevenantAnimatedComponent : Component
{
    /// <summary>
    /// The revenant that animated this item.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public Entity<RevenantComponent>? Revenant;

    /// <summary>
    /// Guns, handcuffs and grenades get an NPC brain and act on their own instead of being thrown.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public bool HasBrain;

    /// <summary>
    /// Components added to give this item a brain.
    /// Removed when the item becomes inanimate.
    /// </summary>
    public List<Component> AddedComponents = new();

    /// <summary>
    /// When the item should become inanimate. If null,
    /// the item never becomes inanimate.
    /// </summary>
    [AutoPausedField]
    public TimeSpan? EndTime;

    /// <summary>
    /// When the item can throw itself again.
    /// </summary>
    [AutoPausedField]
    public TimeSpan NextThrow;

    /// <summary>
    /// Delay between throws.
    /// </summary>
    [DataField]
    public TimeSpan ThrowCooldown = TimeSpan.FromSeconds(2.5);

    /// <summary>
    /// How close a living being has to get before the item attacks it.
    /// </summary>
    [DataField]
    public float DetectRange = 5f;

    [DataField]
    public float ThrowSpeed = 15f;

    /// <summary>
    /// Damage dealt when the item hits someone after throwing itself.
    /// </summary>
    [DataField]
    public FixedPoint2 HitDamage = 8;

    [DataField]
    public ProtoId<DamageTypePrototype> HitDamageType = "Blunt";
}
