using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Persistence;
using CryBits.Server.Persistence.Repositories;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Inventory;
using CryBits.Server.Systems.Movement;
using CryBits.Server.World;
using System;
using System.Drawing;
using System.IO;
using static CryBits.Definitions.Globals;

namespace CryBits.Server.Network.Handlers;

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
    internal void CreateCharacter(GameSession session, CreateCharacterPacket packet)
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
        session.Character.Class = @class = catalog.Classes.Get(new Guid(packet.ClassId));
        session.Character.Genre = packet.GenderMale;
        session.Character.TextureNum = session.Character.Genre
            ? @class.TextureMale[packet.TextureNum]
            : @class.TextureFemale[packet.TextureNum];
        session.Character.Attribute = @class.Attribute;
        session.Character.MapInstance = GameWorld.Current.Maps.Get(@class.SpawnMapId);
        session.Character.Direction = (Direction)@class.SpawnDirection;
        session.Character.X = @class.SpawnX;
        session.Character.Y = @class.SpawnY;
        for (byte i = 0; i < (byte)Vital.Count; i++) session.Character.Vital[i] = session.Character.MaxVital(i);
        for (byte i = 0; i < (byte)@class.Item.Count; i++)
        {
            var item = catalog.Items.Get(@class.Item[i].ItemId);
            if (item == null) continue;
            if (item.Type == ItemType.Equipment &&
                session.Character.Equipment[item.EquipType] == null)
                session.Character.Equipment[item.EquipType] = item;
            else
                inventorySystem.GiveItem(session.Character, item, @class.Item[i].Amount);
        }
        for (byte i = 0; i < MaxHotbar; i++) session.Character.Hotbar[i] = new HotbarSlot(SlotType.None, 0);

        characterRepository.WriteName(name);
        characterRepository.Write(session);

        Join(session.Character);
    }

    [PacketHandler]
    internal void CharacterUse(GameSession session, CharacterUsePacket packet)
    {
        if (packet.CharacterIndex < 0 || packet.CharacterIndex >= session.Characters.Count) return;

        characterRepository.Read(session, session.Characters[packet.CharacterIndex].Name);
        Join(session.Character);
    }

    [PacketHandler]
    internal void CharacterCreate(GameSession session, CharacterCreatePacket packet)
    {
        if (session.Characters.Count == Config.MaxCharacters)
        {
            authSender.Alert(session, "You can only have " + Config.MaxCharacters + " characters.", false);
            return;
        }

        classSender.Classes(session);
        accountSender.CreateCharacter(session);
    }

    [PacketHandler]
    internal void CharacterDelete(GameSession session, CharacterDeletePacket packet)
    {
        if (packet.CharacterIndex < 0 || packet.CharacterIndex >= session.Characters.Count) return;

        var name = session.Characters[packet.CharacterIndex].Name;
        authSender.Alert(session, "The character '" + name + "' has been deleted.", false);
        characterRepository.WriteAllNames(characterRepository.ReadAllNames().Replace(":;" + name + ":", ":"));
        session.Characters.RemoveAt(packet.CharacterIndex);
        File.Delete(Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", name) + Directories.Format);

        accountSender.Characters(session);
        AccountRepository.Instance.Write(session);
    }

    internal void Leave(Player player)
    {
        characterRepository.Write(player.Session);
        playerSender.PlayerLeave(player);

        GameWorld.Current.CurrentTick?.Events.Emit(new PlayerDisconnectedEvent { PlayerId = player.Id });
    }

    private void Join(Player player)
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
}
