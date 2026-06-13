using CryBits.Definitions.Catalog;
using CryBits.Network.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class ItemSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ItemSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Items(Session session)
    {
        packageSender.ToPlayer(session, new ItemsPacket { List = _catalog.Items });
    }
}
