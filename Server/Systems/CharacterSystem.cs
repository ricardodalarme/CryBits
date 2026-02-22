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
internal static class CharacterSystem
{
    /// <summary>Validates and creates a new character for <paramref name="account"/>, then joins the game.</summary>
    internal static void Create(GameSession session, CreateCharacterPacket packet)
    {
        var name = packet.Name.Trim();

        if (name.Length < Config.MinNameLength || name.Length > Config.MaxNameLength)
        {
            AuthSender.Alert(session,
                "The character name must contain between " + Config.MinNameLength + " and " + Config.MaxNameLength +
                " characters.",
                false);
            return;
        }

        if (name.Contains(';') || name.Contains(':'))
        {
            AuthSender.Alert(session, "Can't contain ';' and ':' in the character name.", false);
            return;
        }

        if (CharacterRepository.ReadAllNames().Contains(";" + name + ":"))
        {
            AuthSender.Alert(session, "A character with this name already exists", false);
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
                InventorySystem.GiveItem(session.Character, @class.Item[i].Item, @class.Item[i].Amount);
        for (byte i = 0; i < MaxHotbar; i++) session.Character.Hotbar[i] = new HotbarSlot(SlotType.None, 0);

        CharacterRepository.WriteName(name);
        CharacterRepository.Write(session);

        Join(session.Character);
    }

    /// <summary>Loads the selected character for <paramref name="account"/> and joins the game.</summary>
    internal static void Use(GameSession session, int index)
    {
        if (index < 0 || index >= session.Characters.Count) return;

        CharacterRepository.Read(session, session.Characters[index].Name);
        Join(session.Character);
    }

    internal static void OpenCreation(GameSession session)
    {
        if (session.Characters.Count == Config.MaxCharacters)
        {
            AuthSender.Alert(session, "You can only have " + Config.MaxCharacters + " characters.", false);
            return;
        }

        ClassSender.Classes(session);
        AccountSender.CreateCharacter(session);
    }

    internal static void Delete(GameSession session, int index)
    {
        if (index < 0 || index >= session.Characters.Count) return;

        var name = session.Characters[index].Name;
        AuthSender.Alert(session, "The character '" + name + "' has been deleted.", false);
        CharacterRepository.WriteAllNames(CharacterRepository.ReadAllNames().Replace(":;" + name + ":", ":"));
        session.Characters.RemoveAt(index);
        File.Delete(Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", name) + Directories.Format);

        AccountSender.Characters(session);
        AccountRepository.Write(session);
    }

    /// <summary>Sends all required game data and places <paramref name="player"/> into the world.</summary>
    internal static void Join(Player player)
    {
        player.Session.Characters = [];

        PlayerSender.Join(player);
        ItemSender.Items(player.Session);
        NpcSender.Npcs(player.Session);
        ShopSender.Shops(player.Session);
        MapSender.Map(player.Session, player.MapInstance.Data);
        MapSender.MapPlayers(player);
        PlayerSender.PlayerExperience(player);
        PlayerSender.PlayerInventory(player);
        PlayerSender.PlayerHotbar(player);

        MovementSystem.Warp(player, player.MapInstance, player.X, player.Y, true);

        PlayerSender.JoinGame(player);
        ChatSender.Message(player, Config.WelcomeMessage, Color.Blue);
    }

    /// <summary>Saves the character, notifies the map, and cleans up active sessions.</summary>
    internal static void Leave(Player player)
    {
        CharacterRepository.Write(player.Session);
        PlayerSender.PlayerLeave(player);

        PartySystem.Leave(player);
        TradeSystem.Leave(player);
    }
}
