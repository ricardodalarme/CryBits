using System.Text.Json;
using CryBits.Client.Framework.Constants;

namespace CryBits.Client.Framework.Library.Repositories;

public static class OptionsRepository
{
    public static void Read()
    {
        // Cria o arquivo se ele não existir
        if (!Directories.Options.Exists)
        {
            Write();
            return;
        }

        // Carrega as configurações a partir de JSON
        using var stream = Directories.Options.OpenRead();
        var opts = JsonSerializer.Deserialize<OptionsDto>(stream, JsonConfig.Options) ?? new OptionsDto();
        Options.SaveUsername = opts.SaveUsername;
        Options.Username = opts.Username;
        Options.Sounds = opts.Sounds;
        Options.Musics = opts.Musics;
        Options.Chat = opts.Chat;
        Options.Fps = opts.Fps;
        Options.Latency = opts.Latency;
        Options.Party = opts.Party;
        Options.Trade = opts.Trade;
        Options.PreMapGrid = opts.PreMapGrid;
        Options.PreMapView = opts.PreMapView;
        Options.PreMapAudio = opts.PreMapAudio;
    }

    public static void Write()
    {
        // Escreve as configurações em JSON
        Directories.Options.Directory?.Create();
        var opts = new OptionsDto
        {
            SaveUsername = Options.SaveUsername,
            Username = Options.Username,
            Sounds = Options.Sounds,
            Musics = Options.Musics,
            Chat = Options.Chat,
            Fps = Options.Fps,
            Latency = Options.Latency,
            Party = Options.Party,
            Trade = Options.Trade,
            PreMapGrid = Options.PreMapGrid,
            PreMapView = Options.PreMapView,
            PreMapAudio = Options.PreMapAudio
        };
        using var stream = Directories.Options.Open(FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(stream, opts, JsonConfig.Options);
    }
}
