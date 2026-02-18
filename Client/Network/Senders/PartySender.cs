using CryBits.Enums;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Senders;

internal static class PartySender
{
    public static void PartyInvite(string playerName)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyInvite);
        data.Put(playerName);
        Send.Packet(data);
    }

    public static void PartyAccept()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyAccept);
        Send.Packet(data);
    }

    public static void PartyDecline()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyDecline);
        Send.Packet(data);
    }

    public static void PartyLeave()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyLeave);
        Send.Packet(data);
    }
}
