using System.IO;
using static CryBits.Globals;

namespace CryBits.Server.Library.Repositories;

internal static class SettingsRepository
{
    public static void Read()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Defaults.Exists)
        {
            Write();
            return;
        }

        // Carrega as configurações
        using var data = new BinaryReader(Directories.Defaults.OpenRead());
        GameName = data.ReadString();
        WelcomeMessage = data.ReadString();
        Port = data.ReadInt16();
        MaxPlayers = data.ReadByte();
        MaxCharacters = data.ReadByte();
        MaxPartyMembers = data.ReadByte();
        MaxMapItems = data.ReadByte();
        NumPoints = data.ReadByte();
        MaxNameLength = data.ReadByte();
        MinNameLength = data.ReadByte();
        MaxPasswordLength = data.ReadByte();
        MinPasswordLength = data.ReadByte();
    }

    public static void Write()
    {
        // Escreve as configurações
        using var data = new BinaryWriter(Directories.Defaults.OpenWrite());
        data.Write(GameName);
        data.Write(WelcomeMessage);
        data.Write(Port);
        data.Write(MaxPlayers);
        data.Write(MaxCharacters);
        data.Write(MaxPartyMembers);
        data.Write(MaxMapItems);
        data.Write(NumPoints);
        data.Write(MaxNameLength);
        data.Write(MinNameLength);
        data.Write(MaxPasswordLength);
        data.Write(MinPasswordLength);
    }
}
