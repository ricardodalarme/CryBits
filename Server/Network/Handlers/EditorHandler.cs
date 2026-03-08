using System.Linq;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Client;
using CryBits.Server.Network.Senders;
using CryBits.Server.Persistence.Repositories;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal sealed class EditorHandler(
    AuthSender authSender,
    ClassSender classSender,
    MapSender mapSender,
    ItemSender itemSender,
    NpcSender npcSender,
    ShopSender shopSender,
    SettingsSender settingsSender,
    SettingsRepository settingsRepository,
    ClassRepository classRepository,
    MapRepository mapRepository,
    NpcRepository npcRepository,
    ItemRepository itemRepository,
    ShopRepository shopRepository)
{
    public static EditorHandler Instance { get; } = new(
        AuthSender.Instance,
        ClassSender.Instance,
        MapSender.Instance,
        ItemSender.Instance,
        NpcSender.Instance,
        ShopSender.Instance,
        SettingsSender.Instance,
        SettingsRepository.Instance,
        ClassRepository.Instance,
        MapRepository.Instance,
        NpcRepository.Instance,
        ItemRepository.Instance,
        ShopRepository.Instance);

    private readonly AuthSender _authSender = authSender;
    private readonly ClassSender _classSender = classSender;
    private readonly MapSender _mapSender = mapSender;
    private readonly ItemSender _itemSender = itemSender;
    private readonly NpcSender _npcSender = npcSender;
    private readonly ShopSender _shopSender = shopSender;
    private readonly SettingsSender _settingsSender = settingsSender;
    private readonly SettingsRepository _settingsRepository = settingsRepository;
    private readonly ClassRepository _classRepository = classRepository;
    private readonly MapRepository _mapRepository = mapRepository;
    private readonly NpcRepository _npcRepository = npcRepository;
    private readonly ItemRepository _itemRepository = itemRepository;
    private readonly ShopRepository _shopRepository = shopRepository;

    [PacketHandler]
    internal void WriteSettings(GameSession session, WriteSettingsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            _authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        // Apply received settings.
        Config = packet.Config;

        // Persist settings.
        _settingsRepository.Write();
    }

    [PacketHandler]
    internal void WriteClasses(GameSession session, WriteClassesPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            _authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Class.List = packet.Classes;
        _classRepository.WriteAll();

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            _classSender.Classes(t);
    }

    [PacketHandler]
    internal void WriteMaps(GameSession session, WriteMapsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            _authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Map.List = packet.Maps;
        _mapRepository.WriteAll();

        foreach (var tempMap in GameWorld.Current.Maps.Values)
        {
            tempMap.SpawnItems();

            foreach (var t in GameWorld.Current.Sessions.Where(t => t != session).Where(t => t.Character?.MapInstance == tempMap || t.InEditor))
                _mapSender.Map(t, tempMap.Data);
        }
    }

    [PacketHandler]
    internal void WriteNpcs(GameSession session, WriteNpcsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            _authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Npc.List = packet.Npcs;
        _npcRepository.WriteAll();

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            _npcSender.Npcs(t);
    }

    [PacketHandler]
    internal void WriteItems(GameSession session, WriteItemsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            _authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Item.List = packet.Items;
        _itemRepository.WriteAll();

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            _itemSender.Items(t);
    }

    [PacketHandler]
    internal void WriteShops(GameSession session, WriteShopsPacket packet)
    {
        if (session.AccessLevel < Access.Editor)
        {
            _authSender.Alert(session, "You aren't allowed to do this.");
            return;
        }

        Shop.List = packet.Shops;
        _shopRepository.WriteAll();

        foreach (var t in GameWorld.Current.Sessions.Where(t => t != session))
            _shopSender.Shops(t);
    }

    [PacketHandler]
    internal void RequestSetting(GameSession session, RequestSettingPacket _)
    {
        _settingsSender.ServerData(session);
    }

    [PacketHandler]
    internal void RequestClasses(GameSession session, RequestClassesPacket _)
    {
        _classSender.Classes(session);
    }

    [PacketHandler]
    internal void RequestMap(GameSession session, RequestMapPacket packet)
    {
        if (session.InEditor)
            _mapSender.Map(session, Map.List.Get(packet.Id));
        else
        {
            var player = session.Character;

            if (packet.SendMap) _mapSender.Map(player.Session, player.MapInstance.Data);

            _mapSender.MapPlayers(player);

            player.GettingMap = false;
            PlayerSender.Instance.JoinMap(player);
        }
    }

    [PacketHandler]
    internal void RequestMaps(GameSession session, RequestMapsPacket _)
    {
        _mapSender.Maps(session);
    }

    [PacketHandler]
    internal void RequestNpcs(GameSession session, RequestNpcsPacket _)
    {
        _npcSender.Npcs(session);
    }

    [PacketHandler]
    internal void RequestItems(GameSession session, RequestItemsPacket _)
    {
        _itemSender.Items(session);
    }

    [PacketHandler]
    internal void RequestShops(GameSession session, RequestShopsPacket _)
    {
        _shopSender.Shops(session);
    }
}
