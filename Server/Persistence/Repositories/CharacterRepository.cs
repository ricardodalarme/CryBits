using System;
using System.IO;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Persistence.Repositories;

internal static class CharacterRepository
{
    public static void Read(Account account, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, account.User, "Characters", name) +
                                Directories.Format);

        // Return if the character file directory doesn't exist.
        if (!file.Directory.Exists) return;

        // Read character data and populate Player instance.
        using var data = new BinaryReader(file.OpenRead());
        account.Character = new Player(account);
        account.Character.Name = data.ReadString();
        account.Character.TextureNum = data.ReadInt16();
        account.Character.Level = data.ReadInt16();
        account.Character.Class = Class.List.Get(new Guid(data.ReadString()));
        account.Character.Genre = data.ReadBoolean();
        account.Character.Experience = data.ReadInt32();
        account.Character.Points = data.ReadByte();
        account.Character.Map = TempMap.List.Get(new Guid(data.ReadString()));
        account.Character.X = data.ReadByte();
        account.Character.Y = data.ReadByte();
        account.Character.Direction = (Direction)data.ReadByte();
        for (byte n = 0; n < (byte)Vital.Count; n++) account.Character.Vital[n] = data.ReadInt16();
        for (byte n = 0; n < (byte)Attribute.Count; n++) account.Character.Attribute[n] = data.ReadInt16();
        for (byte n = 0; n < MaxInventory; n++)
        {
            account.Character.Inventory[n].Item = Item.List.Get(new Guid(data.ReadString()));
            account.Character.Inventory[n].Amount = data.ReadInt16();
        }

        for (byte n = 0; n < (byte)Equipment.Count; n++)
            account.Character.Equipment[n] = Item.List.Get(new Guid(data.ReadString()));
        for (byte n = 0; n < MaxHotbar; n++)
            account.Character.Hotbar[n] = new HotbarSlot((SlotType)data.ReadByte(), data.ReadByte());
    }

    public static string ReadAllNames()
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

    public static void Write(Account account)
    {
        var file = new FileInfo(
            Path.Combine(Directories.Accounts.FullName, account.User, "Characters", account.Character.Name) +
            Directories.Format);

        // Ensure character directory exists.
        if (!file.Directory.Exists) file.Directory.Create();

        // Save character data to file.
        using var data = new BinaryWriter(file.OpenWrite());
        data.Write(account.Character.Name);
        data.Write(account.Character.TextureNum);
        data.Write(account.Character.Level);
        data.Write(account.Character.Class.GetId());
        data.Write(account.Character.Genre);
        data.Write(account.Character.Experience);
        data.Write(account.Character.Points);
        data.Write(account.Character.Map.GetId());
        data.Write(account.Character.X);
        data.Write(account.Character.Y);
        data.Write((byte)account.Character.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Write(account.Character.Vital[n]);
        for (byte n = 0; n < (byte)Attribute.Count; n++) data.Write(account.Character.Attribute[n]);
        for (byte n = 0; n < MaxInventory; n++)
        {
            data.Write(account.Character.Inventory[n].Item.GetId());
            data.Write(account.Character.Inventory[n].Amount);
        }

        for (byte n = 0; n < (byte)Equipment.Count; n++) data.Write(account.Character.Equipment[n].GetId());
        for (byte n = 0; n < MaxHotbar; n++)
        {
            data.Write((byte)account.Character.Hotbar[n].Type);
            data.Write(account.Character.Hotbar[n].Slot);
        }
    }

    public static void WriteName(string name)
    {
        // Append a character name to names file.
        using var data = new StreamWriter(Directories.Characters.FullName, true);
        data.Write(";" + name + ":");
    }

    public static void WriteAllNames(string charactersName)
    {
        // Overwrite names file with the provided list.
        using var data = new StreamWriter(Directories.Characters.FullName);
        data.Write(charactersName);
    }
}
