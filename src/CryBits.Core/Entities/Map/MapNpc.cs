using System;

namespace CryBits.Entities.Map;

[Serializable]
public class MapNpc
{
    public Npc.Npc Npc { get; set; }
    public byte Zone { get; set; }
    public bool Spawn { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
}
