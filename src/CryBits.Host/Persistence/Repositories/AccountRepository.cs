using CryBits.Definitions.Common;
using CryBits.Host.Core;
using System.IO;

namespace CryBits.Host.Persistence.Repositories;

internal sealed class AccountRepository
{
    public static AccountRepository Instance { get; } = new();

    public Account Read(string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, name, "Data") + ".dat");

        using var data = new BinaryReader(file.OpenRead());
        return new Account
        {
            Username = data.ReadString(),
            PasswordHash = data.ReadString(),
            AccessLevel = (Access)data.ReadByte()
        };
    }

    public void ReadCharacters(Account account)
    {
        var directory = new DirectoryInfo(Path.Combine(Directories.Accounts.FullName, account.Username, "Characters"));

        if (!directory.Exists) directory.Create();

        var files = directory.GetFiles();
        account.Characters = [];
        for (byte i = 0; i < files.Length; i++)
            using (var data = new BinaryReader(files[i].OpenRead()))
                account.Characters.Add(new Account.CharacterSlot
                {
                    Name = data.ReadString(),
                    TextureNum = data.ReadInt16()
                });
    }

    public void Write(Account account)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, account.Username, "Data") + ".dat");

        if (!file.Directory!.Exists) file.Directory.Create();

        using var data = new BinaryWriter(file.OpenWrite());
        data.Write(account.Username);
        data.Write(account.PasswordHash);
        data.Write((byte)account.AccessLevel);
    }
}
