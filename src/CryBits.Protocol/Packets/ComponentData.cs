using MemoryPack;

namespace CryBits.Protocol.Packets;

[MemoryPackable]
public partial class ComponentData
{
    public byte Tag;
    public byte[] Data = [];
}
