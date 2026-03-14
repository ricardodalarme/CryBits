using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class AccountSender(PackageSender packageSender)
{
    public static AccountSender Instance { get; } = new(PackageSender.Instance);

    public void Characters(GameSession session)
    {
        var packet = new CharactersPacket
        {
            Characters = new PacketsTempCharacter[session.Characters.Count]
        };

        for (byte i = 0; i < session.Characters.Count; i++)
        {
            packet.Characters[i] = new PacketsTempCharacter
            {
                Name = session.Characters[i].Name,
                TextureNum = session.Characters[i].TextureNum
            };
        }

        packageSender.ToPlayer(session, packet);
    }

    public void CreateCharacter(GameSession session)
    {
        packageSender.ToPlayer(session, new CreateCharacterPacket());
    }
}
