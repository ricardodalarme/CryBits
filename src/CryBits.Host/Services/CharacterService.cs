using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Network.Senders;
using CryBits.Host.Persistence;
using CryBits.Host.Persistence.Repositories;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Simulation.Spawners;
using System;
using System.Drawing;
using System.IO;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.State;
using CryBits.Host.Core;

namespace CryBits.Host.Services;

internal sealed class CharacterService(
    CharacterRepository characterRepository,
    AuthSender authSender,
    PlayerSender playerSender,
    ItemSender itemSender,
    NpcSender npcSender,
    ShopSender shopSender,
    MapSender mapSender,
    AccountSender accountSender,
    ClassSender classSender,
    ChatSender chatSender,
    DefinitionCatalog catalog)
{
    public static CharacterService Instance { get; } = new(
        CharacterRepository.Instance,
        AuthSender.Instance,
        PlayerSender.Instance,
        ItemSender.Instance,
        NpcSender.Instance,
        ShopSender.Instance,
        MapSender.Instance,
        AccountSender.Instance,
        ClassSender.Instance,
        ChatSender.Instance,
        DefinitionCatalog.Instance);

    [PacketHandler]
    internal void CreateCharacter(Session session, CreateCharacterPacket packet)
    {
        var name = packet.Name.Trim();

        if (name.Length < Config.MinNameLength || name.Length > Config.MaxNameLength)
        {
            authSender.Alert(session,
                "The character name must contain between " + Config.MinNameLength + " and " + Config.MaxNameLength +
                " characters.",
                false);
            return;
        }

        if (name.Contains(';') || name.Contains(':'))
        {
            authSender.Alert(session, "Can't contain ';' and ':' in the character name.", false);
            return;
        }

        if (characterRepository.ReadAllNames().Contains(";" + name + ":"))
        {
            authSender.Alert(session, "A character with this name already exists", false);
            return;
        }

        var @class = catalog.Classes.Get(new Guid(packet.ClassId));
        if (@class == null) return;

        var data = new Character
        {
            Name = name,
            ClassId = @class.Id,
            Gender = packet.Gender,
            TextureNum = packet.TextureNum,
            Level = 1,
            Experience = 0,
            Points = 0,
            Attributes = (short[])@class.Attribute.Clone(),
            MapId = @class.SpawnMapId,
            X = @class.SpawnX,
            Y = @class.SpawnY,
            Direction = @class.SpawnDirection,
            Hp = 0,
            Mp = 0,
            InventoryIds = new Guid[MaxInventory],
            InventoryAmounts = new short[MaxInventory],
            Equipment = new Guid[(byte)Equipment.Count],
            HotbarTypes = new byte[MaxHotbar],
            HotbarSlots = new byte[MaxHotbar]
        };

        byte slotIndex = 0;
        for (byte i = 0; i < (byte)@class.Item.Count; i++)
        {
            var item = catalog.Items.Get(@class.Item[i].ItemId);
            if (item == null) continue;
            if (item.Type == ItemType.Equipment && data.Equipment[(byte)item.EquipType] == Guid.Empty)
                data.Equipment[(byte)item.EquipType] = item.Id;
            else if (slotIndex < MaxInventory)
            {
                data.InventoryIds[slotIndex] = item.Id;
                data.InventoryAmounts[slotIndex] = item.Stackable ? @class.Item[i].Amount : (byte)1;
                slotIndex++;
            }
        }

        characterRepository.WriteName(name);
        characterRepository.Write(session.Account!, data);

        Join(session, data);
    }

    [PacketHandler]
    internal void CharacterUse(Session session, CharacterUsePacket packet)
    {
        if (packet.CharacterIndex < 0 || packet.CharacterIndex >= session.Account!.Characters.Count) return;

        var data = characterRepository.Read(session.Account!,
            session.Account!.Characters[packet.CharacterIndex].Name);
        if (data == null) return;

        Join(session, data);
    }

    [PacketHandler]
    internal void CharacterCreate(Session session, CharacterCreatePacket packet)
    {
        if (session.Account!.Characters.Count == Config.MaxCharacters)
        {
            authSender.Alert(session, "You can only have " + Config.MaxCharacters + " characters.", false);
            return;
        }

        classSender.Classes(session);
        accountSender.CreateCharacter(session);
    }

    [PacketHandler]
    internal void CharacterDelete(Session session, CharacterDeletePacket packet)
    {
        if (packet.CharacterIndex < 0 || packet.CharacterIndex >= session.Account!.Characters.Count) return;

        var name = session.Account!.Characters[packet.CharacterIndex].Name;
        authSender.Alert(session, "The character '" + name + "' has been deleted.", false);
        characterRepository.WriteAllNames(characterRepository.ReadAllNames().Replace(":;" + name + ":", ":"));
        session.Account!.Characters.RemoveAt(packet.CharacterIndex);
        File.Delete(Path.Combine(Directories.Accounts.FullName, session.Account!.Username, "Characters", name) + Directories.Format);

        accountSender.Characters(session);
        AccountRepository.Instance.Write(session.Account!);
    }

    internal void Leave(EntityId entityId)
    {
        var world = WorldHost.Current;
        var session = world.Sessions.Get(entityId);
        if (session?.Account == null) return;

        var entity = world.Entities.Get(entityId);
        if (entity != null)
        {
            var pos = entity.Get<Position>();
            var appearance = entity.Get<PlayerAppearance>();
            var stats = entity.Get<StatBlock>();
            var vitals = entity.Get<Vitals>();
            var inv = entity.Get<InventoryState>();
            var equip = entity.Get<EquipmentState>();
            var hotbar = entity.Get<HotbarState>();

            if (pos != null && appearance != null && stats != null && vitals != null &&
                inv != null && equip != null && hotbar != null)
            {
                var data = new Character
                {
                    Name = appearance.Name,
                    ClassId = appearance.ClassId,
                    Gender = appearance.Gender,
                    TextureNum = appearance.TextureNum,
                    Level = stats.Level,
                    Experience = stats.Experience,
                    Points = stats.Points,
                    Attributes = (short[])stats.Attribute.Clone(),
                    MapId = pos.MapId,
                    X = pos.X,
                    Y = pos.Y,
                    Direction = (byte)pos.Direction,
                    Hp = vitals.Hp,
                    Mp = vitals.Mp,
                    InventoryIds = new Guid[MaxInventory],
                    InventoryAmounts = new short[MaxInventory],
                    Equipment = new Guid[(byte)Equipment.Count],
                    HotbarTypes = new byte[MaxHotbar],
                    HotbarSlots = new byte[MaxHotbar],
                };

                for (byte i = 0; i < MaxInventory; i++)
                {
                    data.InventoryIds[i] = inv.Slots[i].ItemId;
                    data.InventoryAmounts[i] = inv.Slots[i].Amount;
                }

                for (byte i = 0; i < (byte)Equipment.Count; i++)
                    data.Equipment[i] = equip.Slots[i];

                for (byte i = 0; i < MaxHotbar; i++)
                {
                    data.HotbarTypes[i] = (byte)hotbar.Slots[i].Type;
                    data.HotbarSlots[i] = (byte)hotbar.Slots[i].Slot;
                }

                characterRepository.Write(session.Account, data);
            }
        }

        playerSender.PlayerLeave(entityId);

        world.CurrentTick?.Events.Emit(new PlayerDisconnectedEvent { PlayerId = entityId });

        world.Sessions.Unregister(entityId);
        world.Entities.Destroy(entityId);
        session.Character = null;
    }

    private void Join(Session session, Character data)
    {
        var world = WorldHost.Current;
        var entityId = PlayerSpawner.Spawn(world.Simulation, catalog, data);
        var state = world.Entities.Get(entityId)!;
        var pos = state.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId);
        if (map == null) return;

        world.Sessions.Register(entityId, session);
        session.Character = entityId;

        playerSender.Join(entityId);
        itemSender.Items(session);
        npcSender.Npcs(session);
        shopSender.Shops(session);
        mapSender.Map(session, map.Data);
        mapSender.MapPlayers(entityId);
        playerSender.PlayerExperience(entityId);
        playerSender.PlayerInventory(entityId);
        playerSender.PlayerHotbar(entityId);

        WorldHost.Current.CurrentTick?.Events.Emit(new PlayerWarpedEvent
        {
            PlayerId = entityId,
            OldMapId = pos.MapId,
            NewMapId = pos.MapId,
            NeedsMapData = true
        });

        playerSender.JoinGame(entityId);
        chatSender.Message(entityId, Config.WelcomeMessage, Color.Blue);
    }
}
