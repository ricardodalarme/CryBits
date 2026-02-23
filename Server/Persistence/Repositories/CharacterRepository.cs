using System;
using System.IO;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.World;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Persistence.Repositories;

internal static class CharacterRepository
{
    /// <summary>
    /// Loads the named character file, creates a new ECS entity with all player
    /// components, and assigns a thin <see cref="Player"/> handle to the session.
    /// </summary>
    public static void Read(GameSession session, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", name) +
                                Directories.Format);

        if (!file.Directory!.Exists) return;

        var world = ServerContext.Instance.World;
        var entityId = world.Create();
        session.Character = new Player(entityId, session);

        // Component initialisation order mirrors the binary layout on disk.
        var pd = new PlayerDataComponent();
        var pos = new PositionComponent();
        var dir = new DirectionComponent();
        var vitals = new VitalsComponent();
        var attr = new AttributeComponent();
        var inv = new InventoryComponent();
        var equip = new EquipmentComponent();
        var hotbar = new HotbarComponent();

        using var data = new BinaryReader(file.OpenRead());

        pd.Name = data.ReadString();
        pd.TextureNum = data.ReadInt16();
        pd.Level = data.ReadInt16();
        pd.Class = Class.List.Get(new Guid(data.ReadString()));
        pd.Genre = data.ReadBoolean();
        pd.Experience = data.ReadInt32();
        pd.Points = data.ReadByte();
        pos.MapId = new Guid(data.ReadString());
        pos.X = data.ReadByte();
        pos.Y = data.ReadByte();
        dir.Value = (Direction)data.ReadByte();

        for (byte n = 0; n < (byte)Vital.Count; n++)
            vitals.Values[n] = data.ReadInt16();

        for (byte n = 0; n < (byte)Attribute.Count; n++)
            attr.Values[n] = data.ReadInt16();

        for (byte n = 0; n < MaxInventory; n++)
        {
            inv.Slots[n].Item = Item.List.Get(new Guid(data.ReadString()));
            inv.Slots[n].Amount = data.ReadInt16();
        }

        for (byte n = 0; n < (byte)Equipment.Count; n++)
            equip.Slots[n] = Item.List.Get(new Guid(data.ReadString()));

        for (byte n = 0; n < MaxHotbar; n++)
            hotbar.Slots[n] = new HotbarSlot((SlotType)data.ReadByte(), data.ReadByte());

        // Attach all components.
        world.Add(entityId, pd);
        world.Add(entityId, pos);
        world.Add(entityId, dir);
        world.Add(entityId, vitals);
        world.Add(entityId, attr);
        world.Add(entityId, inv);
        world.Add(entityId, equip);
        world.Add(entityId, hotbar);
        world.Add(entityId, new TradeComponent());
        world.Add(entityId, new PartyComponent());
        world.Add(entityId, new TimerComponent());
        world.Add(entityId, new SessionComponent(session));
    }

    public static string ReadAllNames()
    {
        if (!Directories.Characters.Exists)
        {
            WriteAllNames(string.Empty);
            return string.Empty;
        }

        using var data = new StreamReader(Directories.Characters.FullName);
        return data.ReadToEnd();
    }

    /// <summary>
    /// Serialises the active character's ECS components to disk.
    /// </summary>
    public static void Write(GameSession session)
    {
        var player = session.Character!;
        var world = ServerContext.Instance.World;
        var pd = world.Get<PlayerDataComponent>(player.EntityId);
        var pos = world.Get<PositionComponent>(player.EntityId);
        var dir = world.Get<DirectionComponent>(player.EntityId);
        var vitals = world.Get<VitalsComponent>(player.EntityId);
        var attr = world.Get<AttributeComponent>(player.EntityId);
        var inv = world.Get<InventoryComponent>(player.EntityId);
        var equip = world.Get<EquipmentComponent>(player.EntityId);
        var hotbar = world.Get<HotbarComponent>(player.EntityId);

        var file = new FileInfo(
            Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", pd.Name) +
            Directories.Format);

        if (!file.Directory!.Exists) file.Directory.Create();

        using var data = new BinaryWriter(file.OpenWrite());

        data.Write(pd.Name);
        data.Write(pd.TextureNum);
        data.Write(pd.Level);
        data.Write(pd.Class.GetId());
        data.Write(pd.Genre);
        data.Write(pd.Experience);
        data.Write(pd.Points);
        data.Write(pos.MapId.ToString());
        data.Write(pos.X);
        data.Write(pos.Y);
        data.Write((byte)dir.Value);

        for (byte n = 0; n < (byte)Vital.Count; n++) data.Write(vitals.Values[n]);
        for (byte n = 0; n < (byte)Attribute.Count; n++) data.Write(attr.Values[n]);

        for (byte n = 0; n < MaxInventory; n++)
        {
            data.Write(inv.Slots[n].Item.GetId());
            data.Write(inv.Slots[n].Amount);
        }

        for (byte n = 0; n < (byte)Equipment.Count; n++)
            data.Write(equip.Slots[n].GetId());

        for (byte n = 0; n < MaxHotbar; n++)
        {
            data.Write((byte)hotbar.Slots[n].Type);
            data.Write(hotbar.Slots[n].Slot);
        }
    }

    public static void WriteName(string name)
    {
        using var data = new StreamWriter(Directories.Characters.FullName, true);
        data.Write(";" + name + ":");
    }

    public static void WriteAllNames(string charactersName)
    {
        using var data = new StreamWriter(Directories.Characters.FullName);
        data.Write(charactersName);
    }
}
