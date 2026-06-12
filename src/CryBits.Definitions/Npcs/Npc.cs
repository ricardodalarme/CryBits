using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CryBits.Definitions.Npcs;

/// <summary>NPC metadata definition used by the game.</summary>
[Serializable]
public class Npc : Entity
{
    public string SayMsg { get; set; } = string.Empty;
    public short Texture { get; set; }
    public Behaviour Behaviour { get; set; }
    public byte SpawnTime { get; set; }
    public byte Sight { get; set; }
    public int Experience { get; set; }
    public short[] Vital { get; set; } = new short[(byte)CryBits.Definitions.Characters.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)CryBits.Definitions.Characters.Attribute.Count];
    public IList<NpcDrop> Drop { get; set; } = [];
    public bool AttackNpc { get; set; }

    [JsonIgnore]
    public IList<Npc> Allie { get; set; } = [];

    [JsonInclude]
    private List<Guid> AllieIds
    {
        get => Allie.Select(n => n.GetId()).ToList();
        set => Allie = value.Select(id => DefinitionCatalog.Instance.Npcs.Get(id)).ToList();
    }

    public MovementStyle Movement { get; set; }
    public byte FleeHealth { get; set; }
    private Guid _shop;

    [JsonIgnore]
    public Shops.Shop Shop
    {
        get => DefinitionCatalog.Instance.Shops.Get(_shop);
        set => _shop = value.GetId();
    }

    [JsonInclude]
    private Guid ShopId { get => _shop; set => _shop = value; }

    public Npc()
    {
        Name = "New Npc";
    }

    public bool IsAllied(Npc npc) => Allie.Contains(npc);
}
