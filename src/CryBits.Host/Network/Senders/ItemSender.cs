using CryBits.Definitions.Catalog;
using CryBits.Protocol.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class ItemSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    public void Items(Session session)
    {
        packageSender.ToPlayer(session, new ItemsPacket { List = catalog.Items });
    }
}
