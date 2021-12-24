using System;
using System.Collections.Generic;
using CryBits.Enums;
using CryBits.Extensions;

namespace CryBits.Entities.Npc;

[Serializable]
public class Npc : Entity
{
    // Lista de dados
    public static Dictionary<Guid, Npc> List = new();

    // Dados
    public string SayMsg { get; set; }
    public short Texture { get; set; }
    public Behaviour Behaviour { get; set; }
    public byte SpawnTime { get; set; }
    public byte Sight { get; set; }
    public int Experience { get; set; }
    public short[] Vital { get; set; } = new short[(byte)Enums.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
    public IList<NpcDrop> Drop { get; set; } = new List<NpcDrop>();
    public bool AttackNpc { get; set; }
    public IList<Npc> Allie { get; set; } = new List<Npc>();
    public MovementStyle Movement { get; set; }
    public byte FleeHealth { get; set; }
    private Guid _shop;

    public Shop.Shop Shop
    {
        get => Entities.Shop.Shop.List.Get(_shop);
        set => _shop = value.GetId();
    }

    public Npc()
    {
        Name = "New Npc";
    }

    public bool IsAllied(Npc npc) => Allie.Contains(npc);
}