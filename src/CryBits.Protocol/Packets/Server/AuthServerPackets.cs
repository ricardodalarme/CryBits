using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable] public partial class ConnectPacket : IServerPacket;

[MemoryPackable]
public partial class AlertPacket : IServerPacket
{
    public string Message = string.Empty;
}
