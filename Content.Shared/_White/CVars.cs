using Robust.Shared.Configuration;

namespace Content.Shared._White;

[CVarDefs]
public sealed class WhiteCVars
{
    public static readonly CVarDef<bool> PMMEnabled =
        CVarDef.Create("pmm.enabled", true, CVar.SERVER | CVar.ARCHIVE);
}
