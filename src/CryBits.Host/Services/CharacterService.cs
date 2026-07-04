using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Host.Core;
using CryBits.Host.Network.Senders;
using CryBits.Host.Replication;
using CryBits.Host.Services.Party;
using CryBits.Persistence.Repositories;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.Spawners;
using CryBits.Simulation.State;
using CryBits.Transport;
using CryBits.Transport.Abstractions;
using MemoryPack;
using Microsoft.Extensions.Logging;
using System.Drawing;
using ZLogger;
using static CryBits.Definitions.Globals;

namespace CryBits.Host.Services;

internal sealed class CharacterService(
    ILogger<CharacterService> logger,
    CharacterRepository characterRepository,
    AuthSender authSender,
    ContentSender contentSender,
    AccountSender accountSender,
    ChatSender chatSender,
    DefinitionCatalog catalog,
    WorldHost host,
    KeyframeEncoder keyframeEncoder,
    InterestManager interestManager,
    ITransport transport,
    PartyService partyService)
{
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

        if (characterRepository.NameExists(name))
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

        characterRepository.Save(session.Account!.Username, data);

        logger.ZLogInformation($"Character {data.Name} created for account {session.Account.Username}");
        Join(session, data);
    }

    [PacketHandler]
    internal void CharacterUse(Session session, CharacterUsePacket packet)
    {
        if (packet.CharacterIndex < 0 || packet.CharacterIndex >= session.Account!.Characters.Count) return;

        var data = characterRepository.Find(session.Account!.Username,
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

        contentSender.Classes(session);
        accountSender.CreateCharacter(session);
    }

    [PacketHandler]
    internal void CharacterDelete(Session session, CharacterDeletePacket packet)
    {
        if (packet.CharacterIndex < 0 || packet.CharacterIndex >= session.Account!.Characters.Count) return;

        var name = session.Account!.Characters[packet.CharacterIndex].Name;
        logger.ZLogInformation($"Character {name} deleted for account {session.Account.Username}");
        authSender.Alert(session, "The character '" + name + "' has been deleted.", false);
        characterRepository.Delete(session.Account!.Username, name);
        session.Account!.Characters.RemoveAt(packet.CharacterIndex);

        accountSender.Characters(session);
    }

    internal void Leave(EntityId entityId)
    {
        var session = host.Sessions.Get(entityId);
        if (session?.Account == null) return;

        var entity = host.Entities.Get(entityId);
        var playerName = entity?.Get<PlayerAppearance>()?.Name ?? "unknown";
        if (entity != null)
        {
            WriteCharacterSave(session, entity);
            logger.ZLogDebug($"Player {playerName} saved to database");
        }

        logger.ZLogInformation($"Player {playerName} left world");

        interestManager.RemoveObserver(entityId);

        var tickNum = host.CurrentTick?.TickNumber ?? 0;
        host.CurrentTick?.Events.Emit(new PlayerDisconnectedEvent(tickNum, entityId));

        partyService.HandleDisconnect(entityId);
        host.Sessions.Unregister(entityId);
        host.Simulation.Destroy(entityId);
        session.Character = null;
    }

    private void WriteCharacterSave(Session session, EntityState entity)
    {
        var pos = entity.Get<Position>();
        var appearance = entity.Get<PlayerAppearance>();
        var level = entity.Get<LevelComponent>();
        var attrs = entity.Get<AttributesComponent>();
        var vitals = entity.Get<Vitals>();
        var inv = entity.Get<InventoryState>();
        var equip = entity.Get<EquipmentState>();
        var hotbar = entity.Get<HotbarState>();

        if (pos == null || appearance == null || level == null || attrs == null || vitals == null ||
            inv == null || equip == null || hotbar == null) return;

        var data = new Character
        {
            Name = appearance.Name,
            ClassId = appearance.ClassId,
            Gender = appearance.Gender,
            TextureNum = appearance.TextureNum,
            Level = level.Level,
            Experience = level.Experience,
            Points = (byte)level.Points,
            Attributes = (short[])attrs.Values.Clone(),
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
            HotbarSlots = new byte[MaxHotbar]
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

        characterRepository.Save(session.Account!.Username, data);
    }

    private void Join(Session session, Character data)
    {
        logger.ZLogInformation($"Player {data.Name} joined world on map {data.MapId}");
        var entityId = PlayerSpawner.Spawn(host.Simulation, data);
        var state = host.Entities.Get(entityId)!;
        var pos = state.Get<Position>()!;
        if (!host.Maps.TryGetValue(pos.MapId, out var mapDef)) return;

        host.Sessions.Register(entityId, session);
        session.Character = entityId;

        accountSender.Join(session, entityId);
        contentSender.Items(session);
        contentSender.Npcs(session);
        contentSender.Shops(session);
        contentSender.Map(session, mapDef.Id);

        // Send initial AOI chunk payloads
        var world = host.Simulation;
        var center = ChunkGrid.FromPosition(pos.X, pos.Y);
        foreach (var chunkCoord in world.SpatialGrid.GetNeighborhood(center, 2))
        {
            var payload = ChunkPayloadBuilder.Build(world, pos.MapId, chunkCoord.X, chunkCoord.Y);
            if (payload != null)
            {
                var chunkBytes = MemoryPackSerializer.Serialize<Protocol.Packets.Server.IServerPacket>(payload);
                transport.Send(session.Id, chunkBytes, DeliveryChannel.ReliableOrdered);
            }
        }

        var allOnMap = GetAllEntitiesOnMap(host.Entities, pos.MapId);
        var keyframe = keyframeEncoder.Encode(pos.MapId, allOnMap);
        var bytes = MemoryPackSerializer.Serialize<Protocol.Packets.Server.IServerPacket>(keyframe);
        transport.Send(session.Id, bytes, DeliveryChannel.ReliableOrdered);

        host.Entities.Get(entityId)?.Remove<MapLoadingTag>();
        accountSender.JoinGame(session);
        chatSender.Message(entityId, Config.WelcomeMessage, Color.Blue);
    }

    private static List<EntityId> GetAllEntitiesOnMap(EntityRegistry entities, Guid mapId)
    {
        var result = new List<EntityId>();
        foreach (var state in entities.All)
        {
            var pos = state.Get<Position>();
            if (pos != null && pos.MapId == mapId)
                result.Add(state.Id);
        }
        return result;
    }
}
