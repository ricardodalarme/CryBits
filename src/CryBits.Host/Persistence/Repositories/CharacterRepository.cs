using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.State;
using System;
using System.IO;
using static CryBits.Definitions.Globals;
using Attribute = CryBits.Definitions.Characters.Attribute;
using CryBits.Host.Core;

namespace CryBits.Host.Persistence.Repositories;

internal sealed class CharacterRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static CharacterRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public EntityId Read(Account account, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, account.Username, "Characters", name) +
                                ".dat");

        if (!file.Directory.Exists) return default;

        var world = WorldHost.Current;
        var entityId = world.Entities.Create();
        var state = world.Entities.Get(entityId)!;

        using var data = new BinaryReader(file.OpenRead());

        var appearance = new PlayerAppearance { Name = data.ReadString() };
        appearance.TextureNum = data.ReadInt16();

        var stats = new StatBlock();
        stats.Level = data.ReadInt16();
        appearance.ClassId = new Guid(data.ReadString());
        appearance.Genre = data.ReadBoolean();
        stats.Experience = data.ReadInt32();
        stats.Points = data.ReadByte();

        var pos = new Position { MapId = new Guid(data.ReadString()) };
        pos.X = data.ReadByte();
        pos.Y = data.ReadByte();
        pos.Direction = (Direction)data.ReadByte();

        var vitals = new Vitals();
        vitals.Hp = data.ReadInt16();
        vitals.Mp = data.ReadInt16();

        for (byte n = 0; n < (byte)Attribute.Count; n++) stats.Attribute[n] = data.ReadInt16();

        var inv = new InventoryState();
        for (byte n = 0; n < MaxInventory; n++)
        {
            inv.Slots[n] = new ItemSlot(new Guid(data.ReadString()), data.ReadInt16());
        }

        var equip = new EquipmentState();
        for (byte n = 0; n < (byte)Equipment.Count; n++)
            equip.Slots[n] = new Guid(data.ReadString());

        var hotbar = new HotbarState();
        for (byte n = 0; n < MaxHotbar; n++)
            hotbar.Slots[n] = new HotbarSlot((SlotType)data.ReadByte(), data.ReadByte());

        state.Set(pos);
        state.Set(appearance);
        state.Set(stats);
        state.Set(vitals);
        state.Set(inv);
        state.Set(equip);
        state.Set(hotbar);
        state.Set(new CombatState());
        state.Set(new TradeState());
        state.Set(new PartyState());
        state.Set(new ShopState());
        state.Set(new PlayerTag());

        return entityId;
    }

    public string ReadAllNames()
    {
        if (!Directories.Characters.Exists)
        {
            WriteAllNames(string.Empty);
            return string.Empty;
        }

        using var data = new StreamReader(Directories.Characters.FullName);
        return data.ReadToEnd();
    }

    public void Write(Account account, EntityId entityId)
    {
        var state = WorldHost.Current.Entities.Get(entityId)!;
        var pos = state.Get<Position>()!;
        var appearance = state.Get<PlayerAppearance>()!;
        var stats = state.Get<StatBlock>()!;
        var vitals = state.Get<Vitals>()!;
        var inv = state.Get<InventoryState>()!;
        var equip = state.Get<EquipmentState>()!;
        var hotbar = state.Get<HotbarState>()!;

        var file = new FileInfo(
            Path.Combine(Directories.Accounts.FullName, account.Username, "Characters", appearance.Name) +
            ".dat");

        if (!file.Directory.Exists) file.Directory.Create();

        using var data = new BinaryWriter(file.OpenWrite());
        data.Write(appearance.Name);
        data.Write(appearance.TextureNum);
        data.Write(stats.Level);
        data.Write(appearance.ClassId.ToString());
        data.Write(appearance.Genre);
        data.Write(stats.Experience);
        data.Write(stats.Points);
        data.Write(pos.MapId.ToString());
        data.Write(pos.X);
        data.Write(pos.Y);
        data.Write((byte)pos.Direction);
        data.Write(vitals.Hp);
        data.Write(vitals.Mp);
        for (byte n = 0; n < (byte)Attribute.Count; n++) data.Write(stats.Attribute[n]);
        for (byte n = 0; n < MaxInventory; n++)
        {
            data.Write(inv.Slots[n].ItemId.ToString());
            data.Write(inv.Slots[n].Amount);
        }

        for (byte n = 0; n < (byte)Equipment.Count; n++) data.Write(equip.Slots[n].ToString());
        for (byte n = 0; n < MaxHotbar; n++)
        {
            data.Write((byte)hotbar.Slots[n].Type);
            data.Write(hotbar.Slots[n].Slot);
        }
    }

    public void WriteName(string name)
    {
        using var data = new StreamWriter(Directories.Characters.FullName, true);
        data.Write(";" + name + ":");
    }

    public void WriteAllNames(string charactersName)
    {
        using var data = new StreamWriter(Directories.Characters.FullName);
        data.Write(charactersName);
    }
}
