using CryBits.Protocol.Packets.Server;
using CryBits.Host.Core;
using CryBits.Simulation.State;

namespace CryBits.Host.Network.Senders;

internal sealed class AccountSender(PackageSender packageSender)
{
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

    public void Join(Session session, EntityId entityId)
    {
        packageSender.ToPlayer(session, new JoinPacket { PlayerId = entityId.Value });
    }

    public void JoinGame(Session session)
    {
        packageSender.ToPlayer(session, new JoinGamePacket());
    }
}
