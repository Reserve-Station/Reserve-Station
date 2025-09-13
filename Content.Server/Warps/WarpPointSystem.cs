// SPDX-FileCopyrightText: 2022 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kill_Me_I_Noobs <118206719+Vonsant@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 nazrin <tikufaev@outlook.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.Examine;
using Content.Shared.Ghost;

namespace Content.Server.Warps;

public sealed class WarpPointSystem : EntitySystem
{
    private Dictionary<string, EntityUid> warpPoints = new Dictionary<string, EntityUid>(); // Corvax-Next-Warper

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<WarpPointComponent, ExaminedEvent>(OnWarpPointExamine);
    }

    // Corvax-Next-Warper-Start
	public EntityUid? FindWarpPoint(string id) => IoCManager.Resolve<IEntityManager>().EntityQuery<WarpPointComponent>(true).FirstOrDefault(p => p.ID == id)?.Owner;
	// Corvax-Next-Warper-End

    private void OnWarpPointExamine(EntityUid uid, WarpPointComponent component, ExaminedEvent args)
    {
        if (!HasComp<GhostComponent>(args.Examiner))
            return;

        var loc = component.Location == null ? "<null>" : $"'{component.Location}'";
        args.PushText(Loc.GetString("warp-point-component-on-examine-success", ("location", loc)));
    }
}
