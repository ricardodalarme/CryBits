using CryBits.Definitions.Catalog;
using CryBits.Client.Framework.Network;
using CryBits.Packets.Client;
using System.Linq;

namespace CryBits.Client.Network.Senders;

internal class AccountSender(PacketSender packetSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static AccountSender Instance { get; } = new(PacketSender.Instance, DefinitionCatalog.Instance);

    public void CreateCharacter(string name, bool isMale, short @class, short textureNum) =>
        packetSender.Packet(new CreateCharacterPacket
        {
            Name = name,
            ClassId = _catalog.Classes.ElementAt(@class).Value.Id.ToString(),
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
