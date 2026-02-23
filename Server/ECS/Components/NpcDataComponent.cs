using System;
using CryBits.Entities.Npc;

namespace CryBits.Server.ECS.Components;

/// <summary>Static definition and map-slot identity of a spawned NPC.</summary>
internal sealed class NpcDataComponent : ECS.IComponent
{
    /// <summary>Reference to the NPC definition loaded from data files.</summary>
    public Npc Data = null!;

    /// <summary>Slot index within the map's NPC list (used in network packets).</summary>
    public byte Index;

    /// <summary>ID of the map this NPC belongs to.</summary>
    public Guid MapId;
}
