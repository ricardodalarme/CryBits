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
using CryBits.Host.Systems.Inventory;
using CryBits.Host.Systems.Movement;
using CryBits.Simulation.Events;
using CryBits.Simulation.Formulas;
using System;
using System.Drawing;
using System.IO;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.State;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Handlers;

internal sealed class AccountHandler(
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
    MovementSystem movementSystem,
    InventorySystem inventorySystem,
    DefinitionCatalog catalog)
{
    public static AccountHandler Instance { get; } = new(
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
        MovementSystem.Instance,
        InventorySystem.Instance,
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

        var entityId = world.Entities.Create();
        var state = world.Entities.Get(entityId)!;

        var maxHp = VitalFormulas.MaxVital(Vital.Hp, @class.Vital[(byte)Vital.Hp], @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality], @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], 1);
        var maxMp = VitalFormulas.MaxVital(Vital.Mp, @class.Vital[(byte)Vital.Mp], @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality], @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], 1);

        state.Set(new Position { MapId = @class.SpawnMapId, X = @class.SpawnX, Y = @class.SpawnY, Direction = (Direction)@class.SpawnDirection });
        state.Set(new PlayerAppearance { Name = name, ClassId = @class.Id, TextureNum = packet.GenderMale ? @class.TextureMale[packet.TextureNum] : @class.TextureFemale[packet.TextureNum], Genre = packet.GenderMale });
        state.Set(new StatBlock { Level = 1, Attribute = (short[])@class.Attribute.Clone() });
        state.Set(new Vitals { Hp = maxHp, Mp = maxMp, MaxHp = maxHp, MaxMp = maxMp });

        var inv = new InventoryState();
        for (byte i = 0; i < MaxInventory; i++) inv.Slots[i] = new ItemSlot(Guid.Empty, 0);
        state.Set(inv);

        var equip = new EquipmentState();
        state.Set(equip);

        var hotbar = new HotbarState();
        for (byte i = 0; i < MaxHotbar; i++) hotbar.Slots[i] = new HotbarSlot(SlotType.None, 0);
        state.Set(hotbar);

        state.Set(new CombatState());
        state.Set(new TradeState());
        state.Set(new PartyState());
        state.Set(new ShopState());
        state.Set(new PlayerTag());

        world.Sessions.Register(entityId, session);
        session.Character = entityId;

        for (byte i = 0; i < (byte)@class.Item.Count; i++)
        {
            var item = catalog.Items.Get(@class.Item[i].ItemId);
            if (item == null) continue;
            if (item.Type == ItemType.Equipment && equip.Slots[(byte)item.EquipType] == Guid.Empty)
                equip.Slots[(byte)item.EquipType] = item.Id;
            else
                inventorySystem.GiveItem(WorldHost.Current.Simulation, entityId, item, @class.Item[i].Amount);
        }

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

        movementSystem.Warp(WorldHost.Current.Simulation, entityId, pos.MapId, pos.X, pos.Y, true);

        playerSender.JoinGame(entityId);
        chatSender.Message(entityId, Config.WelcomeMessage, Color.Blue);
    }
}
