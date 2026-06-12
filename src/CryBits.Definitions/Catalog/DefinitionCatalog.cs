using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using System.Collections.Generic;

namespace CryBits.Definitions.Catalog;

public static class DefinitionCatalog
{
    public static Dictionary<System.Guid, Class> Classes { get; set; } = [];
    public static Dictionary<System.Guid, Item> Items { get; set; } = [];
    public static Dictionary<System.Guid, Map> Maps { get; set; } = [];
    public static Dictionary<System.Guid, Npc> Npcs { get; set; } = [];
    public static Dictionary<System.Guid, Shop> Shops { get; set; } = [];
}
