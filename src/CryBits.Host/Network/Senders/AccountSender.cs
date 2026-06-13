using CryBits.Network.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class AccountSender(PackageSender packageSender)
{
    public static AccountSender Instance { get; } = new(PackageSender.Instance);

    public void Characters(Session session)
    {
        var packet = new CharactersPacket
        {
            Characters = new PacketsTempCharacter[session.Account!.Characters.Count]
        };

        for (byte i = 0; i < session.Account!.Characters.Count; i++)
        {
            packet.Characters[i] = new PacketsTempCharacter
            {
                Name = session.Account!.Characters[i].Name,
                TextureNum = session.Account!.Characters[i].TextureNum
            };
        }

        packageSender.ToPlayer(session, packet);
    }

    public void CreateCharacter(Session session)
    {
        packageSender.ToPlayer(session, new CreateCharacterPacket());
    }
}
