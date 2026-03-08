using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.Network;

namespace CryBits.Server.Network.Senders;

internal sealed class PartySender(PackageSender packageSender)
{
    public static PartySender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void Party(Player player)
    {
        var packet = new PartyPacket { Members = new string[player.Party.Count] };
        for (var i = 0; i < player.Party.Count; i++) packet.Members[i] = player.Party[i].Name;
        _packageSender.ToPlayer(player, packet);
    }

    public void PartyInvitation(Player player, string playerInvitation)
    {
        _packageSender.ToPlayer(player, new PartyInvitationPacket { PlayerInvitation = playerInvitation });
    }
}
