using System.Collections.Generic;
using System.IO;
using CryBits.Enums;
using CryBits.Server.Entities;

namespace CryBits.Server.Library.Repositories;

internal static class AccountRepository
{
    public static void Read(Account account, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, name, "Data") + Directories.Format);

        // Carrega os dados da conta
        using var data = new BinaryReader(file.OpenRead());
        account.User = data.ReadString();
        account.Password = data.ReadString();
        account.Access = (Access)data.ReadByte();
    }

    public static void ReadCharacters(Account account)
    {
        var directory = new DirectoryInfo(Path.Combine(Directories.Accounts.FullName, account.User, "Characters"));

        // Previne erros
        if (!directory.Exists) directory.Create();

        // Lê todos os personagens
        var file = directory.GetFiles();
        account.Characters = new List<Account.TempCharacter>();
        for (byte i = 0; i < file.Length; i++)
            // Cria um arquivo temporário
            using (var data = new BinaryReader(file[i].OpenRead()))
                // Carrega os dados e os adiciona à lista
                account.Characters.Add(new Account.TempCharacter
                {
                    Name = data.ReadString(),
                    TextureNum = data.ReadInt16()
                });
    }

    public static void Write(Account account)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, account.User, "Data") + Directories.Format);

        // Evita erros
        if (!file.Directory.Exists) file.Directory.Create();

        // Escreve os dados da conta no arquivo
        using var data = new BinaryWriter(file.OpenWrite());
        data.Write(account.User);
        data.Write(account.Password);
        data.Write((byte)account.Access);
    }
}
