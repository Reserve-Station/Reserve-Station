namespace Content.Server._Freakystation;


[RegisterComponent]
public sealed partial class SecondChanceComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Brr = "Brr brr patapim";
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int Uses = 1;
}
