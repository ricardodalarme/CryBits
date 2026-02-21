using System;

namespace CryBits.Packets.Client;

[Serializable]
public struct CreateCharacterPacket : IClientPacket
{
    public string Name;
    public string ClassId;
    public bool GenderMale;
    public short TextureNum;
}

[Serializable]
public struct CharacterUsePacket : IClientPacket
{
    public int CharacterIndex;
}

[Serializable] public struct CharacterCreatePacket : IClientPacket;

[Serializable]
public struct CharacterDeletePacket : IClientPacket
{
    public int CharacterIndex;
}
