using CryBits.Definitions.Catalog;
using CryBits.Host.Core;
using Microsoft.Extensions.Logging;

namespace CryBits.Server.Commands;

internal static class ServerContext
{
    public static WorldHost? Host { get; set; }
    public static DefinitionCatalog? Catalog { get; set; }
    public static ILoggerFactory? LoggerFactory { get; set; }
}
