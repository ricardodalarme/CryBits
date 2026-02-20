using System;
using System.Drawing;
using System.IO;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Persistence;
using CryBits.Server.Persistence.Repositories;
using CryBits.Server.Network.Senders;
using CryBits.Server.Persistence.Repositories;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>Owns character creation, selection, deletion, and session enter/exit.</summary>
internal static class CharacterSystem
{
    /// <summary>Validates and creates a new character for <paramref name="account"/>, then joins the game.</summary>
    internal static void Create(Account account, NetDataReader data)
    {
        var name = data.GetString().Trim();

        if (name.Length < Config.MinNameLength || name.Length > Config.MaxNameLength)
        {
            AuthSender.Alert(account,
                "The character name must contain between " + Config.MinNameLength + " and " + Config.MaxNameLength +
                " characters.",
                false);
            return;
        }

        if (name.Contains(';') || name.Contains(':'))
        {
            AuthSender.Alert(account, "Can't contain ';' and ':' in the character name.", false);
            return;
        }

        if (CharacterRepository.ReadAllNames().Contains(";" + name + ":"))
        {
            AuthSender.Alert(account, "A character with this name already exists", false);
            return;
        }

        Class @class;
        account.Character = new Player(account);
        account.Character.Name = name;
        account.Character.Level = 1;
        account.Character.Class = @class = Class.List.Get(new Guid(data.GetString()));
        account.Character.Genre = data.GetBool();
        account.Character.TextureNum = account.Character.Genre
            ? @class.TextureMale[data.GetByte()]
            : @class.TextureFemale[data.GetByte()];
        account.Character.Attribute = @class.Attribute;
        account.Character.Map = TempMap.List.Get(@class.SpawnMap.Id);
        account.Character.Direction = (Direction)@class.SpawnDirection;
        account.Character.X = @class.SpawnX;
        account.Character.Y = @class.SpawnY;
        for (byte i = 0; i < (byte)Vital.Count; i++) account.Character.Vital[i] = account.Character.MaxVital(i);
        for (byte i = 0; i < (byte)@class.Item.Count; i++)
            if (@class.Item[i].Item.Type == ItemType.Equipment &&
                account.Character.Equipment[@class.Item[i].Item.EquipType] == null)
                account.Character.Equipment[@class.Item[i].Item.EquipType] = @class.Item[i].Item;
            else
                InventorySystem.GiveItem(account.Character, @class.Item[i].Item, @class.Item[i].Amount);
        for (byte i = 0; i < MaxHotbar; i++) account.Character.Hotbar[i] = new HotbarSlot(SlotType.None, 0);

        CharacterRepository.WriteName(name);
        CharacterRepository.Write(account);

        Join(account.Character);
    }

    /// <summary>Loads the selected character for <paramref name="account"/> and joins the game.</summary>
    internal static void Use(Account account, int index)
    {
        if (index < 0 || index >= account.Characters.Count) return;

        CharacterRepository.Read(account, account.Characters[index].Name);
        Join(account.Character);
    }

    /// <summary>Opens the character-creation screen, after verifying the slot limit.</summary>
    internal static void OpenCreation(Account account)
    {
        if (account.Characters.Count == Config.MaxCharacters)
        {
            AuthSender.Alert(account, "You can only have " + Config.MaxCharacters + " characters.", false);
            return;
        }

        ClassSender.Classes(account);
        AccountSender.CreateCharacter(account);
    }

    /// <summary>Deletes the character at <paramref name="index"/> from <paramref name="account"/>.</summary>
    internal static void Delete(Account account, int index)
    {
        if (index < 0 || index >= account.Characters.Count) return;

        var name = account.Characters[index].Name;
        AuthSender.Alert(account, "The character '" + name + "' has been deleted.", false);
        CharacterRepository.WriteAllNames(CharacterRepository.ReadAllNames().Replace(":;" + name + ":", ":"));
        account.Characters.RemoveAt(index);
        File.Delete(Path.Combine(Directories.Accounts.FullName, account.User, "Characters", name) + Directories.Format);

        AccountSender.Characters(account);
        AccountRepository.Write(account);
    }

    /// <summary>Sends all required game data and places <paramref name="player"/> into the world.</summary>
    internal static void Join(Player player)
    {
        player.Account.Characters = null;

        PlayerSender.Join(player);
        ItemSender.Items(player.Account);
        NpcSender.Npcs(player.Account);
        ShopSender.Shops(player.Account);
        MapSender.Map(player.Account, player.Map.Data);
        MapSender.MapPlayers(player);
        PlayerSender.PlayerExperience(player);
        PlayerSender.PlayerInventory(player);
        PlayerSender.PlayerHotbar(player);

        MovementSystem.Warp(player, player.Map, player.X, player.Y, true);

        PlayerSender.JoinGame(player);
        ChatSender.Message(player, Config.WelcomeMessage, Color.Blue);
    }

    /// <summary>Saves the character, notifies the map, and cleans up active sessions.</summary>
    internal static void Leave(Player player)
    {
        CharacterRepository.Write(player.Account);
        PlayerSender.PlayerLeave(player);

        PartySystem.Leave(player);
        TradeSystem.Leave(player);
    }
}
