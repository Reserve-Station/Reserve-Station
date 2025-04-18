using Robust.Shared.Configuration;

namespace Content.Shared._White;

[CVarDefs]
public sealed class WhiteCVars
{
    #region Character Customization

    /// <summary>
    /// Players to set their own clown names.
    /// </summary>
    public static readonly CVarDef<bool> AllowCustomClownName =
        CVarDef.Create("customize.allow_custom_clown_name", true, CVar.REPLICATED);

    #endregion
    public static readonly CVarDef<bool> PMMEnabled =
        CVarDef.Create("pmm.enabled", true, CVar.SERVER | CVar.ARCHIVE);
}
