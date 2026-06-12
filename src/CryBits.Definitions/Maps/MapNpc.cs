using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using System;
using System.Text.Json.Serialization;

namespace CryBits.Definitions.Maps;

[Serializable]
public class MapNpc
{
    private Guid _npc;

    [JsonIgnore]
    public Npcs.Npc Npc
    {
        get => DefinitionCatalog.Instance.Npcs.Get(_npc);
        set => _npc = value.GetId();
    }

    [JsonInclude]
    private Guid NpcId { get => _npc; set => _npc = value; }

    public byte Zone { get; set; }
    public bool Spawn { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
}
