using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class AlertPacket : IServerPacket
{
    public string Message = string.Empty;
}

[MemoryPackable]
public partial class MessagePacket : IServerPacket
{
    public string Text = string.Empty;
    public int ColorArgb;
}
