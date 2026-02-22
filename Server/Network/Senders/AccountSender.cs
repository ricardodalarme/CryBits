using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class AccountSender
{
    public static void Characters(GameSession session)
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

        Send.ToPlayer(session, packet);
    }

    public static void CreateCharacter(GameSession session)
    {
        Send.ToPlayer(session, new CreateCharacterPacket());
    }
}
