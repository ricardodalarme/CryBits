using System;
using System.Drawing;
using System.IO;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Client;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
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
    /// <summary>Validates and creates a new character for the session, then joins the game.</summary>
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

        var @class = Class.List.Get(new Guid(packet.ClassId));
        if (@class == null)
        {
            AuthSender.Alert(session, "Invalid class selection.", false);
            return;
        }

        // Create the ECS entity and attach all player components.
        var world = ServerContext.Instance.World;
        var entityId = world.Create();
        session.Character = new Player(entityId, session);

        var pd = new PlayerDataComponent
        {
            Name      = name,
            Level     = 1,
            Class     = @class,
            Genre     = packet.GenderMale,
            TextureNum = (short)(packet.GenderMale
                ? @class.TextureMale[packet.TextureNum]
                : @class.TextureFemale[packet.TextureNum])
        };

        var attr = new AttributeComponent();
        Array.Copy(@class.Attribute, attr.Values, @class.Attribute.Length);

        var pos = new PositionComponent
        {
            MapId = @class.SpawnMap.Id,
            X     = @class.SpawnX,
            Y     = @class.SpawnY
        };

        var dir = new DirectionComponent { Value = (Direction)@class.SpawnDirection };

        var vitals = new VitalsComponent();
        for (byte i = 0; i < (byte)Vital.Count; i++)
            vitals.Values[i] = session.Character.MaxVital(i);

        var equip  = new EquipmentComponent();
        var inv    = new InventoryComponent();
        var hotbar = new HotbarComponent();

        // Distribute starting class items.
        // Temporarily attach components so the helpers work during init.
        world.Add(entityId, pd);
        world.Add(entityId, attr);
        world.Add(entityId, pos);
        world.Add(entityId, dir);
        world.Add(entityId, vitals);
        world.Add(entityId, equip);
        world.Add(entityId, inv);
        world.Add(entityId, hotbar);
        world.Add(entityId, new TradeComponent());
        world.Add(entityId, new PartyComponent());
        world.Add(entityId, new TimerComponent());
        world.Add(entityId, new SessionComponent(session));

        for (byte i = 0; i < @class.Item.Count; i++)
        {
            var classItem = @class.Item[i];
            if (classItem.Item.Type == ItemType.Equipment &&
                equip.Slots[classItem.Item.EquipType] == null)
                equip.Slots[classItem.Item.EquipType] = classItem.Item;
            else
                InventorySystem.GiveItem(session.Character, classItem.Item, classItem.Amount);
        }

        CharacterRepository.WriteName(name);
        CharacterRepository.Write(session);

        Join(session.Character);
    }

    /// <summary>Loads the selected character and joins the game.</summary>
    internal static void Use(GameSession session, int index)
    {
        if (index < 0 || index >= session.Characters.Count) return;

        CharacterRepository.Read(session, session.Characters[index].Name);
        Join(session.Character!);
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

    /// <summary>Sends all required game data and places the player into the world.</summary>
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

        MovementSystem.Warp(player, player.MapInstance, player.Get<PositionComponent>().X, player.Get<PositionComponent>().Y, true);

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

        // Destroy the ECS entity so resources are freed.
        ServerContext.Instance.World.Destroy(player.EntityId);
        player.Session.Character = null;
    }
}
