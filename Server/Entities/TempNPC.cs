using CryBits.Entities.Npc;
using CryBits.Enums;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Entities;

internal class TempNpc : Character
{
    // Dados básicos
    public readonly byte Index;
    public readonly Npc Data;
    public bool Alive;
    public Character Target;
    public long SpawnTimer;
    public long AttackTimer;

    public short Regeneration(byte vital)
    {
        // Cálcula o máximo de vital que o Npc possui
        return (Vital)vital switch
        {
            Enums.Vital.Hp => (short)(Data.Vital[vital] * 0.05 + Data.Attribute[(byte)Attribute.Vitality] * 0.3),
            Enums.Vital.Mp => (short)(Data.Vital[vital] * 0.05 + Data.Attribute[(byte)Attribute.Intelligence] * 0.1),
            _ => 0
        };
    }

    // Construtor
    public TempNpc(byte index, TempMap map, Npc data)
    {
        Index = index;
        Map = map;
        Data = data;
    }
}