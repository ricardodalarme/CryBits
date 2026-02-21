using System;

namespace CryBits.Packets.Server;

[Serializable] public struct CreateCharacterPacket : IServerPacket;

[Serializable]
public struct CharactersPacket : IServerPacket
{
    public PacketsTempCharacter[] Characters;
}

[Serializable]
public struct PacketsTempCharacter
{
    public string Name;
    public short TextureNum;
}
