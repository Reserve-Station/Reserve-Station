using Robust.Client.UserInterface.RichText;

namespace Content.Goobstation.UIKit.UserInterface.RichText;

public sealed class ExamineBorderTag : IMarkupTagHandler
{
    // [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!; // Reserve edit: Fix warnings

    public const string TagName = "examineborder";

    public string Name => TagName;
}
