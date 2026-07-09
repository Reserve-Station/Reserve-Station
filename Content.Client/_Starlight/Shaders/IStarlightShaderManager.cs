// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.Shaders;

public interface IStarlightShaderManager
{
    ShaderInstance? GetShader(ProtoId<ShaderPrototype>? id);
}
