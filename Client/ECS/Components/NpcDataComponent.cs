using CryBits.Entities.Npc;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Links an NPC entity back to its static definition loaded from the server.
/// The <see cref="Npc"/> reference is the shared, read-only data (name, texture,
/// behaviour, base vitals).  Runtime state (position, current health) lives in
/// <see cref="TransformComponent"/> and <see cref="VitalsComponent"/>.
/// </summary>
public sealed class NpcDataComponent : IComponent
{
    /// <summary>
    /// Map-slot index this NPC occupies (mirrors the server-side array index).
    /// Used to correlate server update packets to the right entity.
    /// </summary>
    public byte SlotIndex { get; set; }

    /// <summary>Shared NPC definition; null until the server sends the data.</summary>
    public Npc? Data { get; set; }
}
