using System;
using CryBits.Entities;

namespace CryBits.Server.ECS.Components;

/// <summary>A dropped or spawned item lying on a map tile.</summary>
internal sealed class MapItemComponent : ECS.IComponent
{
    public Item Item = null!;
    public short Amount;
    public byte X;
    public byte Y;
    public Guid MapId;
}
