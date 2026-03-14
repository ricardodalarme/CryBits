using CryBits.Client.Framework.Constants;
using SFML.Audio;

namespace CryBits.Client.Framework.Audio;

public class AudioManager
{
    public static AudioManager Instance { get; } = new();

    /// <summary>Loaded sound instances keyed by filename.</summary>
    public readonly Dictionary<string, Sound> Sounds = [];

    // Current music playback device.
    public Music? CurrentMusicDevice { get; private set; }

    // Currently playing music name.
    public string CurrentMusicName { get; private set; } = string.Empty;

    public void LoadSounds()
    {
        var files = Directories.Sounds.GetFiles();

        // Load all files and add them to the list.
        foreach (var file in files)
            Sounds.Add(file.Name, new Sound(new SoundBuffer(file.FullName)));
    }

    public void PlaySound(string soundName, bool loop = false)
    {
        if (!Options.Instance.Sounds) return;
        if (!Sounds.TryGetValue(soundName, out var sound)) return;

        // Play sound.
        sound.Volume = 20;
        sound.IsLooping = loop;
        sound.Play();

    }

    public bool IsPlaying(string soundName)
    {
        if (!Sounds.TryGetValue(soundName, out var sound)) return false;
        return sound.Status == SoundStatus.Playing;
    }

    public void StopAllSounds()
    {
        // Stop all sounds.
        foreach (var sound in Sounds)
            sound.Value.Stop();
    }

    public void PlayMusic(string musicName, bool loop = false)
    {
        if (!Options.Instance.Musics) return;
        var directory = Path.Combine(Directories.Musics.FullName, musicName);

        // Return early if a music device already exists or file missing.
        if (CurrentMusicDevice != null) return;
        if (!File.Exists(directory)) return;

        // Load audio file into SFML Music.
        CurrentMusicDevice = new Music(directory)
        {
            Volume = 20,
            IsLooping = loop
        };

        // Start playback.
        CurrentMusicDevice.Play();
        CurrentMusicName = musicName;
    }

    public void StopMusic()
    {
        // Stop and dispose current music.
        CurrentMusicName = string.Empty;
        CurrentMusicDevice?.Stop();
        CurrentMusicDevice?.Dispose();
        CurrentMusicDevice = null;
    }
}
