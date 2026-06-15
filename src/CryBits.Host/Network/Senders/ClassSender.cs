using CryBits.Definitions.Catalog;
using CryBits.Transport.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class ClassSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    public void Classes(Session session)
    {
        packageSender.ToPlayer(session, new ClassesPacket { List = catalog.Classes });
    }
}
