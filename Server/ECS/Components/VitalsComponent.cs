using CryBits.Enums;

namespace CryBits.Server.ECS.Components;

/// <summary>Current vitals (HP, MP, …). Shared by players and NPCs.</summary>
internal sealed class VitalsComponent : ECS.IComponent
{
    public short[] Values = new short[(byte)Vital.Count];
}
