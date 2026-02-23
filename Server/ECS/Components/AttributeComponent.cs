using CryBits.Enums;

namespace CryBits.Server.ECS.Components;

/// <summary>Player attribute points (Strength, Vitality, Intelligence, etc.).</summary>
internal sealed class AttributeComponent : ECS.IComponent
{
    public short[] Values = new short[(byte)Attribute.Count];
}
