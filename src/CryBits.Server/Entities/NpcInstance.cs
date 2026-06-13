using CryBits.Definitions.Characters;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Formulas;
using System;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Server.Entities;

internal class NpcInstance : Character
{
    // Basic NPC runtime fields.
    public override Guid Id { get; } = Guid.NewGuid();
    public readonly byte Index;
    public readonly Npc Data;
    public bool Alive;
    public Character Target;
    public long SpawnTimer;
    public long AttackTimer;

    /// <summary>Calculates NPC regeneration for the specified vital.</summary>
    /// <param name="vital">Index of the vital to query.</param>
    /// <returns>Regeneration amount for the specified vital.</returns>
    public short Regeneration(byte vital) => VitalFormulas.NpcRegeneration(
        (Vital)vital,
        Data.Vital[vital],
        Data.Attribute[(byte)Attribute.Vitality],
        Data.Attribute[(byte)Attribute.Intelligence]);

    public NpcInstance(byte index, MapInstance mapInstance, Npc data)
    {
        Index = index;
        MapInstance = mapInstance;
        Data = data;
    }
}
