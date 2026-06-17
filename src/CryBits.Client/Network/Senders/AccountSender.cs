using CryBits.Client.Framework.Network;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Transport.Packets.Client;

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
            Gender = isMale ? Gender.Male : Gender.Female,
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
