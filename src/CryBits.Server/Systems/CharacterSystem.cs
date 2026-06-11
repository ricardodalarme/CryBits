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
using System;
using System.Drawing;
using System.IO;
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

    /// <summary>Validates and creates a new character for <paramref name="session"/>, then joins the game.</summary>
    internal void Create(GameSession session, CreateCharacterPacket packet)
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
                inventorySystem.GiveItem(session.Character, @class.Item[i].Item, @class.Item[i].Amount);
        for (byte i = 0; i < MaxHotbar; i++) session.Character.Hotbar[i] = new HotbarSlot(SlotType.None, 0);

        characterRepository.WriteName(name);
        characterRepository.Write(session);

        Join(session.Character);
    }

    /// <summary>Loads the selected character for <paramref name="session"/> and joins the game.</summary>
    internal void Use(GameSession session, int index)
    {
        if (index < 0 || index >= session.Characters.Count) return;

        characterRepository.Read(session, session.Characters[index].Name);
        Join(session.Character);
    }

    internal void OpenCreation(GameSession session)
    {
        if (session.Characters.Count == Config.MaxCharacters)
        {
            authSender.Alert(session, "You can only have " + Config.MaxCharacters + " characters.", false);
            return;
        }

        classSender.Classes(session);
        accountSender.CreateCharacter(session);
    }

    internal void Delete(GameSession session, int index)
    {
        if (index < 0 || index >= session.Characters.Count) return;

        var name = session.Characters[index].Name;
        authSender.Alert(session, "The character '" + name + "' has been deleted.", false);
        characterRepository.WriteAllNames(characterRepository.ReadAllNames().Replace(":;" + name + ":", ":"));
        session.Characters.RemoveAt(index);
        File.Delete(Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", name) + Directories.Format);

        accountSender.Characters(session);
        AccountRepository.Instance.Write(session);
    }

    /// <summary>Sends all required game data and places <paramref name="player"/> into the world.</summary>
    internal void Join(Player player)
    {
        player.Session.Characters = [];

        playerSender.Join(player);
        itemSender.Items(player.Session);
        npcSender.Npcs(player.Session);
        shopSender.Shops(player.Session);
        mapSender.Map(player.Session, player.MapInstance.Data);
        mapSender.MapPlayers(player);
        playerSender.PlayerExperience(player);
        playerSender.PlayerInventory(player);
        playerSender.PlayerHotbar(player);

        movementSystem.Warp(player, player.MapInstance, player.X, player.Y, true);

        playerSender.JoinGame(player);
        chatSender.Message(player, Config.WelcomeMessage, Color.Blue);
    }

    /// <summary>Saves the character, notifies the map, and cleans up active sessions.</summary>
    internal void Leave(Player player)
    {
        characterRepository.Write(player.Session);
        playerSender.PlayerLeave(player);

        partySystem.Leave(player);
        tradeSystem.Leave(player);
    }
}
