using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
public partial class ConnectPacket : IClientPacket
{
    public string Username = string.Empty;
    public string Password = string.Empty;
    public bool IsClientAccess;
}

[MemoryPackable]
public partial class RegisterPacket : IClientPacket
{
    public string Username = string.Empty;
    public string Password = string.Empty;
}
