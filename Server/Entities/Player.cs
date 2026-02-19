using System.Collections.Generic;
using System.Linq;
using CryBits.Entities;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Server.Entities;

internal class Player : Character
{
    // Dados permantes
    public string Name { get; set; } = string.Empty;
    public Class Class { get; set; }
    public short TextureNum { get; set; }
    public bool Genre { get; set; }
    public short Level { get; set; }
    public int Experience { get; set; }
    public byte Points { get; set; }
    public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
    public ItemSlot[] Inventory { get; } = new ItemSlot[MaxInventory];
    public Item[] Equipment { get; } = new Item[(byte)Enums.Equipment.Count];
    public HotbarSlot[] Hotbar { get; } = new HotbarSlot[MaxHotbar];

    // Dados temporários
    public bool GettingMap;
    public List<Player> Party = new();
    public string PartyRequest;
    public Player Trade;
    public string TradeRequest;
    public TradeSlot[] TradeOffer;
    public Shop Shop;
    public Account Account;
    public long AttackTimer;

    // Constutor
    public Player(Account account)
    {
        Account = account;
        for (byte i = 0; i < Inventory.Length; i++)
            Inventory[i] = new ItemSlot(null, 0);
    }

    // Cálcula o dano do jogador
    public short Damage
    {
        get
        {
            var value = Attribute[(byte)Enums.Attribute.Strength];
            if (Equipment[(byte)Enums.Equipment.Weapon] != null)
                value += Equipment[(byte)Enums.Equipment.Weapon].WeaponDamage;
            return value;
        }
    }

    // Cálcula a defesa do jogador
    public short PlayerDefense => Attribute[(byte)Enums.Attribute.Resistance];

    public short MaxVital(byte vital)
    {
        var @base = Class.Vital;

        // Cálcula o máximo de vital que um jogador possui
        return (Vital)vital switch
        {
            Enums.Vital.Hp => (short)(@base[vital] + Attribute[(byte)Enums.Attribute.Vitality] * 1.50 * (Level * 0.75) +
                                      1),
            Enums.Vital.Mp => (short)(@base[vital] +
                                      Attribute[(byte)Enums.Attribute.Intelligence] * 1.25 * (Level * 0.5) + 1),
            _ => 1
        };
    }

    public short Regeneration(byte vital)
    {
        // Cálcula o máximo de vital que um jogador possui
        return (Vital)vital switch
        {
            Enums.Vital.Hp => (short)(MaxVital(vital) * 0.05 + Attribute[(byte)Enums.Attribute.Vitality] * 0.3),
            Enums.Vital.Mp => (short)(MaxVital(vital) * 0.05 + Attribute[(byte)Enums.Attribute.Intelligence] * 0.1),
            _ => 1
        };
    }

    // Quantidade de experiência para passar para o próximo level
    public int ExpNeeded
    {
        get
        {
            short total = 0;
            for (byte i = 0; i < (byte)Enums.Attribute.Count; i++) total += Attribute[i];
            return (int)((Level + 1) * 2.5 + (total + Points) / 2);
        }
    }

    public HotbarSlot FindHotbar(SlotType type, short slot) =>
        Hotbar.FirstOrDefault(x => x.Type == type && x.Slot == slot);

    public HotbarSlot FindHotbar(SlotType type, ItemSlot slot) =>
        Hotbar.FirstOrDefault(x => x.Type == type && Inventory[x.Slot] == slot);

    public ItemSlot FindInventory(Item item) => Inventory.First(x => x.Item == item);

    public byte TotalInventoryFree => (byte)Inventory.Count(x => x.Item != null);

    public byte TotalTradeItems => (byte)TradeOffer.Count(x => x.SlotNum != 0);

    public static Player Find(string name) =>
        Account.List.Find(x => x.IsPlaying && x.Character.Name.Equals(name))?.Character;
}