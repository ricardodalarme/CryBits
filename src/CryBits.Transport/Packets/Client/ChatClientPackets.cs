using MemoryPack;

namespace CryBits.Transport.Packets.Client;

[MemoryPackable]
public partial class MessagePacket : IClientPacket
{
    public string Text;
    public byte Type;
    public string Addressee;
}
