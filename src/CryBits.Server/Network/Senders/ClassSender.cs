using CryBits.Definitions.Catalog;
using CryBits.Network.Packets.Server;
using CryBits.Server.Core;

namespace CryBits.Server.Network.Senders;

internal sealed class ClassSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ClassSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Classes(GameSession session)
    {
        packageSender.ToPlayer(session, new ClassesPacket { List = _catalog.Classes });
    }
}
