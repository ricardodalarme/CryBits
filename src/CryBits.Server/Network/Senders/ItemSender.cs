using CryBits.Definitions.Catalog;
using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class ItemSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ItemSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Items(GameSession session)
    {
        packageSender.ToPlayer(session, new ItemsPacket { List = _catalog.Items });
    }
}
