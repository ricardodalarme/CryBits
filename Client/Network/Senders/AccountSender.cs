using CryBits.Client.Framework.Network;
using CryBits.Entities;
using CryBits.Packets.Client;
using System.Linq;

namespace CryBits.Client.Network.Senders;

internal class AccountSender(PacketSender packetSender)
{
    public static AccountSender Instance { get; } = new(PacketSender.Instance);

    public void CreateCharacter(string name, bool isMale, short @class, short textureNum) =>
        packetSender.Packet(new CreateCharacterPacket
        {
            Name = name,
            ClassId = Class.List.ElementAt(@class).Value.Id.ToString(),
            GenderMale = isMale,
            TextureNum = textureNum
        });

    public void CharacterUse(int characterIndex) =>
        packetSender.Packet(new CharacterUsePacket
        {
            CharacterIndex = characterIndex
        });

    public void CharacterCreate() => packetSender.Packet(new CharacterCreatePacket());

    public void CharacterDelete(int characterIndex) =>
        packetSender.Packet(new CharacterDeletePacket
        {
            CharacterIndex = characterIndex
        });
}
