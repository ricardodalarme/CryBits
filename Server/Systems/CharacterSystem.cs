using System;
using System.Drawing;
using System.IO;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Persistence;
using CryBits.Server.Persistence.Repositories;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>Owns character creation, selection, deletion, and session enter/exit.</summary>
internal sealed class CharacterSystem(
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
    PartySystem partySystem,
    TradeSystem tradeSystem)
{
    public static CharacterSystem Instance { get; } = new(
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
        PartySystem.Instance,
        TradeSystem.Instance);

    private readonly CharacterRepository _characterRepository = characterRepository;
    private readonly AuthSender _authSender = authSender;
    private readonly PlayerSender _playerSender = playerSender;
    private readonly ItemSender _itemSender = itemSender;
    private readonly NpcSender _npcSender = npcSender;
    private readonly ShopSender _shopSender = shopSender;
    private readonly MapSender _mapSender = mapSender;
    private readonly AccountSender _accountSender = accountSender;
    private readonly ClassSender _classSender = classSender;
    private readonly ChatSender _chatSender = chatSender;
    private readonly MovementSystem _movementSystem = movementSystem;
    private readonly InventorySystem _inventorySystem = inventorySystem;
    private readonly PartySystem _partySystem = partySystem;
    private readonly TradeSystem _tradeSystem = tradeSystem;

    /// <summary>Validates and creates a new character for <paramref name="session"/>, then joins the game.</summary>
    internal void Create(GameSession session, CreateCharacterPacket packet)
    {
        var name = packet.Name.Trim();

        if (name.Length < Config.MinNameLength || name.Length > Config.MaxNameLength)
        {
            _authSender.Alert(session,
                "The character name must contain between " + Config.MinNameLength + " and " + Config.MaxNameLength +
                " characters.",
                false);
            return;
        }

        if (name.Contains(';') || name.Contains(':'))
        {
            _authSender.Alert(session, "Can't contain ';' and ':' in the character name.", false);
            return;
        }

        if (_characterRepository.ReadAllNames().Contains(";" + name + ":"))
        {
            _authSender.Alert(session, "A character with this name already exists", false);
            return;
        }

        Class @class;
        session.Character = new Player(session);
        session.Character.Name = name;
        session.Character.Level = 1;
        session.Character.Class = @class = Class.List.Get(new Guid(packet.ClassId));
        session.Character.Genre = packet.GenderMale;
        session.Character.TextureNum = session.Character.Genre
            ? @class.TextureMale[packet.TextureNum]
            : @class.TextureFemale[packet.TextureNum];
        session.Character.Attribute = @class.Attribute;
        session.Character.MapInstance = GameWorld.Current.Maps.Get(@class.SpawnMap.Id);
        session.Character.Direction = (Direction)@class.SpawnDirection;
        session.Character.X = @class.SpawnX;
        session.Character.Y = @class.SpawnY;
        for (byte i = 0; i < (byte)Vital.Count; i++) session.Character.Vital[i] = session.Character.MaxVital(i);
        for (byte i = 0; i < (byte)@class.Item.Count; i++)
            if (@class.Item[i].Item.Type == ItemType.Equipment &&
                session.Character.Equipment[@class.Item[i].Item.EquipType] == null)
                session.Character.Equipment[@class.Item[i].Item.EquipType] = @class.Item[i].Item;
            else
                _inventorySystem.GiveItem(session.Character, @class.Item[i].Item, @class.Item[i].Amount);
        for (byte i = 0; i < MaxHotbar; i++) session.Character.Hotbar[i] = new HotbarSlot(SlotType.None, 0);

        _characterRepository.WriteName(name);
        _characterRepository.Write(session);

        Join(session.Character);
    }

    /// <summary>Loads the selected character for <paramref name="session"/> and joins the game.</summary>
    internal void Use(GameSession session, int index)
    {
        if (index < 0 || index >= session.Characters.Count) return;

        _characterRepository.Read(session, session.Characters[index].Name);
        Join(session.Character);
    }

    internal void OpenCreation(GameSession session)
    {
        if (session.Characters.Count == Config.MaxCharacters)
        {
            _authSender.Alert(session, "You can only have " + Config.MaxCharacters + " characters.", false);
            return;
        }

        _classSender.Classes(session);
        _accountSender.CreateCharacter(session);
    }

    internal void Delete(GameSession session, int index)
    {
        if (index < 0 || index >= session.Characters.Count) return;

        var name = session.Characters[index].Name;
        _authSender.Alert(session, "The character '" + name + "' has been deleted.", false);
        _characterRepository.WriteAllNames(_characterRepository.ReadAllNames().Replace(":;" + name + ":", ":"));
        session.Characters.RemoveAt(index);
        File.Delete(Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", name) + Directories.Format);

        _accountSender.Characters(session);
        AccountRepository.Instance.Write(session);
    }

    /// <summary>Sends all required game data and places <paramref name="player"/> into the world.</summary>
    internal void Join(Player player)
    {
        player.Session.Characters = [];

        _playerSender.Join(player);
        _itemSender.Items(player.Session);
        _npcSender.Npcs(player.Session);
        _shopSender.Shops(player.Session);
        _mapSender.Map(player.Session, player.MapInstance.Data);
        _mapSender.MapPlayers(player);
        _playerSender.PlayerExperience(player);
        _playerSender.PlayerInventory(player);
        _playerSender.PlayerHotbar(player);

        _movementSystem.Warp(player, player.MapInstance, player.X, player.Y, true);

        _playerSender.JoinGame(player);
        _chatSender.Message(player, Config.WelcomeMessage, Color.Blue);
    }

    /// <summary>Saves the character, notifies the map, and cleans up active sessions.</summary>
    internal void Leave(Player player)
    {
        _characterRepository.Write(player.Session);
        _playerSender.PlayerLeave(player);

        _partySystem.Leave(player);
        _tradeSystem.Leave(player);
    }
}
