// SPDX-License-Identifier: MIT

using Content.Shared.Botany.Components;

namespace Content.Client.Botany.Components;

[RegisterComponent]
public sealed partial class ProduceComponent : SharedProduceComponent
{
    /// <summary>
    ///     Seed prototype ID, read from the entity prototype by the food guide.
    ///     <c>SeedPrototype</c> is server-only, so this is a plain string on the client.
    /// </summary>
    [DataField]
    public string? SeedId;
}