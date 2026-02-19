using System;
using CryBits.Enums;

namespace CryBits.Entities.Map;

[Serializable]
public class MapAttribute
{
    public byte Type { get; set; }
    public string Data1 { get; set; } = string.Empty;
    public short Data2 { get; set; }
    public short Data3 { get; set; }
    public short Data4 { get; set; }
    public byte Zone { get; set; }
    public bool[] Block { get; set; } = new bool[(byte)Direction.Count];
}
