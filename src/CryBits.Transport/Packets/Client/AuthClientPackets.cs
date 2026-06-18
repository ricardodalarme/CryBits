using MemoryPack;

namespace CryBits.Transport.Packets.Client;

[MemoryPackable]
public partial class ConnectPacket : IClientPacket
{
    public string Username;
    public string Password;
    public bool IsClientAccess;
}

[MemoryPackable]
public partial class RegisterPacket : IClientPacket
{
    public string Username;
    public string Password;
}
