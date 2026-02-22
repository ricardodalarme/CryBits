using System.IO;
using CryBits.Enums;
using CryBits.Server.World;

namespace CryBits.Server.Persistence.Repositories;

internal static class AccountRepository
{
    public static void Read(GameSession session, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, name, "Data") + Directories.Format);

        using var data = new BinaryReader(file.OpenRead());
        session.Username = data.ReadString();
        session.PasswordHash = data.ReadString();
        session.AccessLevel = (Access)data.ReadByte();
    }

    public static void ReadCharacters(GameSession session)
    {
        var directory = new DirectoryInfo(Path.Combine(Directories.Accounts.FullName, session.Username, "Characters"));

        if (!directory.Exists) directory.Create();

        var file = directory.GetFiles();
        session.Characters = [];
        for (byte i = 0; i < file.Length; i++)
            using (var data = new BinaryReader(file[i].OpenRead()))
                session.Characters.Add(new GameSession.CharacterSlot
                {
                    Name = data.ReadString(),
                    TextureNum = data.ReadInt16()
                });
    }

    public static void Write(GameSession session)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, session.Username, "Data") + Directories.Format);

        if (!file.Directory!.Exists) file.Directory.Create();

        using var data = new BinaryWriter(file.OpenWrite());
        data.Write(session.Username);
        data.Write(session.PasswordHash);
        data.Write((byte)session.AccessLevel);
    }
}
