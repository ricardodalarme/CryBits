using CryBits.Definitions.Common;
using MemoryPack;

namespace CryBits.Definitions.Npcs;

/// <summary>NPC metadata definition used by the game.</summary>
[MemoryPackable]
public partial record class Npc : Definition
{
    public string SayMsg { get; set; } = string.Empty;
    public short Texture { get; set; }
    public Behaviour Behaviour { get; set; }
    public byte SpawnTime { get; set; }
    public byte Sight { get; set; }
    public int Experience { get; set; }
    public short[] Vital { get; set; } = new short[(byte)CryBits.Definitions.Characters.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)CryBits.Definitions.Characters.Attribute.Count];
    public IList<NpcDrop> Drop { get; set; } = [];
    public bool AttackNpc { get; set; }

    public List<Guid> AllieIds { get; set; } = [];

    public MovementStyle Movement { get; set; }
    public byte FleeHealth { get; set; }
    public Guid ShopId { get; set; }

    public Npc()
    {
        Name = "New Npc";
    }

    public bool IsAllied(Guid npcId) => AllieIds.Contains(npcId);
}
