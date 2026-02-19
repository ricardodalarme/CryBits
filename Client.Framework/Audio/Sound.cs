using CryBits.Client.Framework.Constants;
using SFML.Audio;

namespace CryBits.Client.Framework.Audio;

public static class Sound
{
    // Dispositivo sonoro
    public static readonly Dictionary<string, SFML.Audio.Sound> List = [];

    public static void Load()
    {
        var files = Directories.Sounds.GetFiles();

        // Carrega todos os arquivos e os adiciona a lista
        foreach (var file in files)
            List.Add(file.Name, new SFML.Audio.Sound(new SoundBuffer(file.FullName)));
    }

    public static void Play(string sound, bool loop = false)
    {
        if (!List.ContainsKey(sound)) return;

        // Reproduz o áudio
        List[sound].Volume = 20;
        List[sound].IsLooping = loop;
        List[sound].Play();
    }

    public static void StopAll()
    {
        // Para todos os sons
        foreach (var sound in List)
            sound.Value.Stop();
    }
}