using CryBits.Definitions.Catalog;
using CryBits.Host.Core;
using CryBits.Host.Persistence.Repositories;

namespace CryBits.Server.Commands;

internal static class ServerContext
{
    public static WorldHost? Host { get; set; }
    public static DefinitionCatalog? Catalog { get; set; }
    public static AccountRepository? AccountRepository { get; set; }
}
