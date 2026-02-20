using System.Collections.Generic;
using System.IO;
using CryBits.Enums;
using CryBits.Server.Entities;

namespace CryBits.Server.Persistence.Repositories;

internal static class AccountRepository
{
    public static void Read(Account account, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, name, "Data") + Directories.Format);

        // Load account data.
        using var data = new BinaryReader(file.OpenRead());
        account.User = data.ReadString();
        account.PasswordHash = data.ReadString();
        account.Access = (Access)data.ReadByte();
    }

    public static void ReadCharacters(Account account)
    {
        var directory = new DirectoryInfo(Path.Combine(Directories.Accounts.FullName, account.User, "Characters"));

        // Ensure characters directory exists.
        if (!directory.Exists) directory.Create();

        // Read all characters for the account.
        var file = directory.GetFiles();
        account.Characters = [];
        for (byte i = 0; i < file.Length; i++)
            using (var data = new BinaryReader(file[i].OpenRead()))
                // Add character metadata to the list.
                account.Characters.Add(new Account.TempCharacter
                {
                    Name = data.ReadString(),
                    TextureNum = data.ReadInt16()
                });
    }

    public static void Write(Account account)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, account.User, "Data") + Directories.Format);

        // Ensure account directory exists.
        if (!file.Directory.Exists) file.Directory.Create();

        // Write account data to file.
        using var data = new BinaryWriter(file.OpenWrite());
        data.Write(account.User);
        data.Write(account.PasswordHash);
        data.Write((byte)account.Access);
    }
}
