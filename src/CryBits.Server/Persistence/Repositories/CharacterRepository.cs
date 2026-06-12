using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.World;
using System;
using System.IO;
using static CryBits.Globals;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class CharacterRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static CharacterRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public void Read(GameSession session, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", name) +
                                ".dat");

        // Return if the character file directory doesn't exist.
        if (!file.Directory.Exists) return;

        // Read character data and populate Player instance.
        using var data = new BinaryReader(file.OpenRead());
        session.Character = new Player(session);
        session.Character.Name = data.ReadString();
        session.Character.TextureNum = data.ReadInt16();
        session.Character.Level = data.ReadInt16();
        session.Character.Class = _catalog.Classes.Get(new Guid(data.ReadString()));
        session.Character.Genre = data.ReadBoolean();
        session.Character.Experience = data.ReadInt32();
        session.Character.Points = data.ReadByte();
        session.Character.MapInstance = GameWorld.Current.Maps.Get(new Guid(data.ReadString()));
        session.Character.X = data.ReadByte();
        session.Character.Y = data.ReadByte();
        session.Character.Direction = (Direction)data.ReadByte();
        for (byte n = 0; n < (byte)Vital.Count; n++) session.Character.Vital[n] = data.ReadInt16();
        for (byte n = 0; n < (byte)Attribute.Count; n++) session.Character.Attribute[n] = data.ReadInt16();
        for (byte n = 0; n < MaxInventory; n++)
        {
            session.Character.Inventory[n].ItemId = new Guid(data.ReadString());
            session.Character.Inventory[n].Amount = data.ReadInt16();
        }

        for (byte n = 0; n < (byte)Equipment.Count; n++)
            session.Character.Equipment[n] = _catalog.Items.Get(new Guid(data.ReadString()));
        for (byte n = 0; n < MaxHotbar; n++)
            session.Character.Hotbar[n] = new HotbarSlot((SlotType)data.ReadByte(), data.ReadByte());
    }

    public string ReadAllNames()
    {
        // Create the characters names file if it doesn't exist.
        if (!Directories.Characters.Exists)
        {
            WriteAllNames(string.Empty);
            return string.Empty;
        }

        // Return all registered character names.
        using var data = new StreamReader(Directories.Characters.FullName);
        return data.ReadToEnd();
    }

    public void Write(GameSession session)
    {
        var file = new FileInfo(
            Path.Combine(Directories.Accounts.FullName, session.Username, "Characters", session.Character!.Name) +
            ".dat");

        // Ensure character directory exists.
        if (!file.Directory.Exists) file.Directory.Create();

        // Save character data to file.
        using var data = new BinaryWriter(file.OpenWrite());
        data.Write(session.Character!.Name);
        data.Write(session.Character.TextureNum);
        data.Write(session.Character.Level);
        data.Write(session.Character.Class.GetId().ToString());
        data.Write(session.Character.Genre);
        data.Write(session.Character.Experience);
        data.Write(session.Character.Points);
        data.Write(session.Character.MapInstance.GetId().ToString());
        data.Write(session.Character.X);
        data.Write(session.Character.Y);
        data.Write((byte)session.Character.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Write(session.Character.Vital[n]);
        for (byte n = 0; n < (byte)Attribute.Count; n++) data.Write(session.Character.Attribute[n]);
        for (byte n = 0; n < MaxInventory; n++)
        {
            data.Write(session.Character.Inventory[n].ItemId.ToString());
            data.Write(session.Character.Inventory[n].Amount);
        }

        for (byte n = 0; n < (byte)Equipment.Count; n++) data.Write(session.Character.Equipment[n].GetId().ToString());
        for (byte n = 0; n < MaxHotbar; n++)
        {
            data.Write((byte)session.Character.Hotbar[n].Type);
            data.Write(session.Character.Hotbar[n].Slot);
        }
    }

    public void WriteName(string name)
    {
        // Append a character name to names file.
        using var data = new StreamWriter(Directories.Characters.FullName, true);
        data.Write(";" + name + ":");
    }

    public void WriteAllNames(string charactersName)
    {
        // Overwrite names file with the provided list.
        using var data = new StreamWriter(Directories.Characters.FullName);
        data.Write(charactersName);
    }
}
