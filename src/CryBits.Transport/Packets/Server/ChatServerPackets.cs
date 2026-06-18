using MemoryPack;

namespace CryBits.Transport.Packets.Server;

[MemoryPackable]
public partial class AlertPacket : IServerPacket
{
    public string Message;
}

[MemoryPackable]
public partial class MessagePacket : IServerPacket
{
    public string Text;
    public int ColorArgb;
}
