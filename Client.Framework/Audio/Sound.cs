using CryBits.Client.Framework.Constants;
using SFML.Audio;

namespace CryBits.Client.Framework.Audio;

public static class Sound
{
    /// <summary>Loaded sound instances keyed by filename.</summary>
    public static readonly Dictionary<string, SFML.Audio.Sound> List = [];

    public static void Load()
    {
        var files = Directories.Sounds.GetFiles();

        // Load all files and add them to the list.
        foreach (var file in files)
            List.Add(file.Name, new SFML.Audio.Sound(new SoundBuffer(file.FullName)));
    }

    public static void Play(string sound, bool loop = false)
    {
        if (!List.ContainsKey(sound)) return;

        // Play sound.
        List[sound].Volume = 20;
        List[sound].IsLooping = loop;
        List[sound].Play();
    }

    public static void StopAll()
    {
        // Stop all sounds.
        foreach (var sound in List)
            sound.Value.Stop();
    }
}
