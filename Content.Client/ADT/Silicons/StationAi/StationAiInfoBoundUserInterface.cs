// SPDX-FileCopyrightText: 2025 Kutosss <162154227+Kutosss@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Svarshik <96281939+lexaSvarshik@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later
using Robust.Client.UserInterface;
using JetBrains.Annotations;
using Content.Shared.CrewManifest;
using Content.Shared.ADT.Silicons.StationAi;

namespace Content.Client.ADT.Silicons.StationAi;

[UsedImplicitly]
public sealed class StationAiInfoBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private StationAiInfo? _window;

    public StationAiInfoBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<StationAiInfo>();
        _window.CrewManifestButton.OnPressed += _ => SendMessage(new CrewManifestOpenUiMessage());
        _window.RoboticsControlButton.OnPressed += _ => SendMessage(new RoboticsControlOpenUiMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not StationAiInfoUpdateState updateState || _window == null)
            return;

        _window.UpdateState(updateState);
    }

}
