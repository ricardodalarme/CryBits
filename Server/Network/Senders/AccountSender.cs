using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class AccountSender
{
    public static void Characters(Account account)
    {
        var packet = new CharactersPacket
        {
            Characters = new PacketsTempCharacter[account.Characters.Count]
        };

        for (byte i = 0; i < account.Characters.Count; i++)
        {
            packet.Characters[i] = new PacketsTempCharacter
            {
                Name = account.Characters[i].Name,
                TextureNum = account.Characters[i].TextureNum
            };
        }

        Send.ToPlayer(account, packet);
    }

    public static void CreateCharacter(Account account)
    {
        Send.ToPlayer(account, new CreateCharacterPacket());
    }
}
