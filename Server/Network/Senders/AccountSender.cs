using CryBits.Enums;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class AccountSender
{
    public static void Characters(Account account)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.Characters);
        data.Put((byte)account.Characters.Count);

        for (byte i = 0; i < account.Characters.Count; i++)
        {
            data.Put(account.Characters[i].Name);
            data.Put(account.Characters[i].TextureNum);
        }

        Send.ToPlayer(account, data);
    }

    public static void CreateCharacter(Account account)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.CreateCharacter);
        Send.ToPlayer(account, data);
    }
}
