using MemoryPack;

namespace CryBits.Transport.Packets.Server;

[MemoryPackable] public partial class CreateCharacterPacket : IServerPacket;

[MemoryPackable]
public partial class CharactersPacket : IServerPacket
{
    public PacketsTempCharacter[] Characters;
}

[MemoryPackable]
public partial struct PacketsTempCharacter
{
    public string Name;
    public short TextureNum;
}
