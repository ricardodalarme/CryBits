using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Host.Network.Senders;
using CryBits.Host.Persistence;
using CryBits.Host.Persistence.Repositories;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using System;
using System.Drawing;
using System.IO;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.State;
using CryBits.Host.Core;
using CryBits.Simulation.Spawners;

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

        var world = WorldHost.Current;
        var @class = catalog.Classes.Get(new Guid(packet.ClassId));

        var entityId = PlayerSpawner.Spawn(WorldHost.Current.Simulation, catalog, name, @class, packet.GenderMale, packet.TextureNum);
        var state = world.Entities.Get(entityId)!;
        var inv = state.Get<InventoryState>()!;
        var equip = state.Get<EquipmentState>()!;

        world.Sessions.Register(entityId, session);
        session.Character = entityId;

        byte slotIndex = 0;
        for (byte i = 0; i < (byte)@class.Item.Count; i++)
        {
            var item = catalog.Items.Get(@class.Item[i].ItemId);
            if (item == null) continue;
            if (item.Type == ItemType.Equipment && equip.Slots[(byte)item.EquipType] == Guid.Empty)
                equip.Slots[(byte)item.EquipType] = item.Id;
            else if (slotIndex < MaxInventory)
            {
                inv.Slots[slotIndex].ItemId = item.Id;
                inv.Slots[slotIndex].Amount = item.Stackable ? @class.Item[i].Amount : (byte)1;
                slotIndex++;
            }
        }

        world.Dirty.Mark<InventoryState>(entityId);

        characterRepository.WriteName(name);
        characterRepository.Write(session.Account!, entityId);

        Join(entityId);
    }

    [PacketHandler]
    internal void CharacterUse(Session session, CharacterUsePacket packet)
    {
        if (packet.CharacterIndex < 0 || packet.CharacterIndex >= session.Account!.Characters.Count) return;

        var entityId = characterRepository.Read(session.Account!, session.Account!.Characters[packet.CharacterIndex].Name);
        session.Character = entityId;
        WorldHost.Current.Sessions.Register(entityId, session);
        Join(entityId);
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
        var session = world.Sessions.Get(entityId)!;
        if (session == null) return;

        characterRepository.Write(session.Account!, entityId);
        playerSender.PlayerLeave(entityId);

        world.CurrentTick?.Events.Emit(new PlayerDisconnectedEvent { PlayerId = entityId.Value });

        world.Sessions.Unregister(entityId);
        world.Entities.Destroy(entityId);
        session.Character = null;
    }

    private void Join(EntityId entityId)
    {
        var world = WorldHost.Current;
        var session = world.Sessions.Get(entityId)!;
        var state = world.Entities.Get(entityId)!;
        var pos = state.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId);
        if (map == null) return;

        playerSender.Join(entityId);
        itemSender.Items(session);
        npcSender.Npcs(session);
        shopSender.Shops(session);
        mapSender.Map(session, map.Data);
        mapSender.MapPlayers(entityId);
        playerSender.PlayerExperience(entityId);
        playerSender.PlayerInventory(entityId);
        playerSender.PlayerHotbar(entityId);

        WorldHost.Current.CurrentTick?.Events.Emit(new PlayerRespawnEvent
        {
            PlayerId = entityId.Value,
            MapId = pos.MapId,
            X = pos.X,
            Y = pos.Y
        });

        playerSender.JoinGame(entityId);
        chatSender.Message(entityId, Config.WelcomeMessage, Color.Blue);
    }
}
