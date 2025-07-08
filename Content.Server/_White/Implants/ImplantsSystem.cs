using Content.Server.Administration.Logs;
using Content.Server.Chat.Managers;
using Content.Server.Mind;
using Content.Server.Popups;
using Content.Server.Roles;
using Content.Server.Roles.Jobs;

using Content.Shared._White.Implants.NeuroStabilization;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Mindshield.Components;
using Content.Shared.Revolutionary.Components;
using Content.Shared.Tag;

namespace Content.Server._White.Implants;

public sealed class ImplantsSystem : EntitySystem
{
    [Dependency] private readonly IAdminLogManager _adminLog = default!;
    [Dependency] private readonly RoleSystem _role = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly JobSystem _job = default!;

    [ValidatePrototypeId<TagPrototype>]
    private const string MindShieldTag = "MindShield";

    [ValidatePrototypeId<TagPrototype>]
    private const string NeuroStabilizationTag = "NeuroStabilization";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SubdermalImplantComponent, ImplantImplantedEvent>(OnImplantInserted);
    }

    private void OnImplantInserted(EntityUid uid, SubdermalImplantComponent component, ImplantImplantedEvent args)
    {
        if (_tag.HasTag(uid, NeuroStabilizationTag) && args.Implanted != null)
            EnsureComp<NeuroStabilizationComponent>(args.Implanted.Value);
    }

    // Методы для обработки имплантов будут добавлены по мере необходимости
    // Пока что система нейро-стабилизации работает через компонент
}
