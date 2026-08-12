using CryBits.Client.Framework.Network;
using CryBits.Definitions.Characters;
using CryBits.Protocol.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class AccountSender(Connection connection)
{
    public void CreateCharacter(string name, bool isMale, Guid classId, short textureNum) =>
        connection.SendPacket(new CreateCharacterPacket
        {
            Name = name,
            ClassId = classId.ToString(),
            Gender = isMale ? Gender.Male : Gender.Female,
            TextureNum = textureNum
        });

    public void CharacterUse(int characterIndex) =>
        connection.SendPacket(new CharacterUsePacket { CharacterIndex = characterIndex });

    public void CharacterCreate() => connection.SendPacket(new CharacterCreatePacket());

    public void CharacterDelete(int characterIndex) =>
        connection.SendPacket(new CharacterDeletePacket { CharacterIndex = characterIndex });
}
