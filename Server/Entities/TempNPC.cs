using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Server.Formulas;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Entities;

internal class TempNpc : Character
{
    // Basic NPC runtime fields.
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

    public TempNpc(byte index, TempMap map, Npc data)
    {
        Index = index;
        Map = map;
        Data = data;
    }
}
