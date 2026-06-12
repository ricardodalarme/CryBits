using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using System;
using System.Collections.Generic;

namespace CryBits.Definitions.Catalog;

public class DefinitionCatalog
{
    public static DefinitionCatalog Instance { get; } = new();

    public Dictionary<Guid, Class> Classes { get; set; } = [];
    public Dictionary<Guid, Item> Items { get; set; } = [];
    public Dictionary<Guid, Map> Maps { get; set; } = [];
    public Dictionary<Guid, Npc> Npcs { get; set; } = [];
    public Dictionary<Guid, Shop> Shops { get; set; } = [];
}
