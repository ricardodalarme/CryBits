using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
public partial class MessagePacket : IClientPacket
{
    public string Text = string.Empty;
    public byte Type;
    public string Addressee = string.Empty;
}
