using CryBits.Definitions.Catalog;
using CryBits.Protocol.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class ContentSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    public void Classes(Session session)
    {
        packageSender.ToPlayer(session, new ClassesPacket { List = catalog.Classes });
    }

    public void Items(Session session)
    {
        packageSender.ToPlayer(session, new ItemsPacket { List = catalog.Items });
    }

    public void Npcs(Session session)
    {
        packageSender.ToPlayer(session, new NpcsPacket { List = catalog.Npcs });
    }

    public void Shops(Session session)
    {
        packageSender.ToPlayer(session, new ShopsPacket { List = catalog.Shops });
    }
}
