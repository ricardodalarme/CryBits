using System;

namespace CryBits.Server.ECS.Components;

/// <summary>World position of an entity. Shared by players, NPCs, and map items.</summary>
internal sealed class PositionComponent : ECS.IComponent
{
    public byte X;
    public byte Y;
    public Guid MapId;
}
