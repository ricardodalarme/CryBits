using System.Collections.Generic;
using System.Linq;
using CryBits.Entities;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.Formulas;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Entities;

internal class Player : Character
{
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

    public bool GettingMap;
    public List<Player> Party = [];
    public string PartyRequest;
    public Player Trade;
    public string TradeRequest;
    public TradeSlot[] TradeOffer;
    public Shop Shop;
    public GameSession Session;
    public long AttackTimer;

    public Player(GameSession session)
    {
        Session = session;
        for (byte i = 0; i < Inventory.Length; i++)
            Inventory[i] = new ItemSlot(null, 0);
    }

    /// <summary>Gets the player's computed damage (strength plus weapon damage).</summary>
    public short Damage => CombatFormulas.PlayerDamage(
        Attribute[(byte)Enums.Attribute.Strength],
        Equipment[(byte)Enums.Equipment.Weapon]?.WeaponDamage ?? 0);

    /// <summary>Gets the player's defense value (Resistance attribute).</summary>
    public short PlayerDefense => CombatFormulas.PlayerDefense(Attribute[(byte)Enums.Attribute.Resistance]);

    /// <summary>Returns the player's maximum amount for the specified vital (HP or MP).</summary>
    /// <param name="vital">Index of the vital to query.</param>
    /// <returns>Maximum amount for the specified vital.</returns>
    public short MaxVital(byte vital) => VitalFormulas.MaxVital(
        (Vital)vital,
        Class.Vital[vital],
        Attribute[(byte)Enums.Attribute.Vitality],
        Attribute[(byte)Enums.Attribute.Intelligence],
        Level);

    /// <summary>Calculates the player's regeneration amount for the specified vital.</summary>
    /// <param name="vital">Index of the vital to query.</param>
    /// <returns>Amount the player regenerates for the specified vital.</returns>
    public short Regeneration(byte vital) => VitalFormulas.PlayerRegeneration(
        (Vital)vital,
        MaxVital(vital),
        Attribute[(byte)Enums.Attribute.Vitality],
        Attribute[(byte)Enums.Attribute.Intelligence]);

    /// <summary>Gets the experience required to reach the next level.</summary>
    public int ExpNeeded
    {
        get
        {
            short total = 0;
            for (byte i = 0; i < (byte)Enums.Attribute.Count; i++) total += Attribute[i];
            return LevelingFormulas.ExperienceNeeded(Level, total, Points);
        }
    }

    public HotbarSlot FindHotbar(SlotType type, short slot) =>
        Hotbar.FirstOrDefault(x => x.Type == type && x.Slot == slot);

    public HotbarSlot FindHotbar(SlotType type, ItemSlot slot) =>
        Hotbar.FirstOrDefault(x => x.Type == type && Inventory[x.Slot] == slot);

    public ItemSlot FindInventory(Item item) => Inventory.First(x => x.Item == item);

    /// <summary>Number of occupied inventory slots.</summary>
    public byte TotalInventoryFree => (byte)Inventory.Count(x => x.Item != null);

    /// <summary>Number of items currently offered in the active trade.</summary>
    public byte TotalTradeItems => (byte)TradeOffer.Count(x => x.SlotNum != 0);

    /// <summary>Finds a playing player by name.</summary>
    /// <param name="name">Player name to search for.</param>
    /// <returns>The Player instance if found; otherwise null.</returns>
    public static Player Find(string name) =>
        GameWorld.Current.Sessions.Find(x => x.IsPlaying && x.Character!.Name.Equals(name))?.Character;
}
