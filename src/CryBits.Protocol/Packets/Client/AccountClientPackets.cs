using CryBits.Definitions.Characters;
using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
public partial class CreateCharacterPacket : IClientPacket
{
    public string Name = string.Empty;
    public string ClassId = string.Empty;
    public Gender Gender;
    public short TextureNum;
}

[MemoryPackable]
public partial class CharacterUsePacket : IClientPacket
{
    public int CharacterIndex;
}

[MemoryPackable] public partial class CharacterCreatePacket : IClientPacket;

[MemoryPackable]
public partial class CharacterDeletePacket : IClientPacket
{
    public int CharacterIndex;
}
