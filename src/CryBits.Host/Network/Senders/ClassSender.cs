using CryBits.Definitions.Catalog;
using CryBits.Network.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class ClassSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ClassSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Classes(Session session)
    {
        packageSender.ToPlayer(session, new ClassesPacket { List = _catalog.Classes });
    }
}
