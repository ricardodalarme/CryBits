using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable] public partial class CreateCharacterPacket : IServerPacket;

[MemoryPackable]
public partial class CharactersPacket : IServerPacket
{
    public PacketsTempCharacter[] Characters = [];
}

[MemoryPackable]
public partial struct PacketsTempCharacter
{
    public string Name = string.Empty;
    public short TextureNum;

    public PacketsTempCharacter()
    {
    }
}

[MemoryPackable]
public partial class JoinPacket : IServerPacket
{
    public long PlayerId;
}

[MemoryPackable] public partial class JoinGamePacket : IServerPacket;
