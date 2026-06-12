using System;

namespace CryBits.Definitions.Maps;

[Serializable]
public class MapNpc
{
    public Npcs.Npc Npc { get; set; }
    public byte Zone { get; set; }
    public bool Spawn { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
}
