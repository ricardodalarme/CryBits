using CryBits.Client.Framework.Constants;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace CryBits.Client.Framework.Audio;

public class AudioManager
{
    /// <summary>Loaded sound effect instances keyed by filename.</summary>
    public readonly Dictionary<string, SoundEffectInstance> Sounds = [];

    // Currently playing music track name.
    public string? CurrentMusic { get; private set; }

    // Music volume
    private const float MusicVolume = 0.20f;
    private const float SoundVolume = 0.20f;

    /// <summary>Whitelisted audio extensions for SoundEffect.</summary>
    private static readonly HashSet<string> SoundExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".wav", ".xma", ".aiff", ".aif"
    };

    /// <summary>Whitelisted audio extensions for Song / MediaPlayer.</summary>
    private static readonly HashSet<string> MusicExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".wma", ".mp3", ".m4a", ".aiff", ".aif", ".wav"
    };

    public void LoadSounds()
    {
        if (!Directory.Exists(Directories.Sounds.FullName)) return;

        foreach (var file in Directories.Sounds.GetFiles())
        {
            if (!SoundExtensions.Contains(file.Extension)) continue;

            try
            {
                var effect = SoundEffect.FromFile(file.FullName);
                Sounds.Add(file.Name, effect.CreateInstance());
            }
            catch
            {
                // Skip malformed / unsupported files rather than crashing the game.
            }
        }
    }

    public void PlaySound(string soundName, bool loop = false)
    {
        if (!Options.Instance.Sounds) return;
        if (!Sounds.TryGetValue(soundName, out var sound)) return;

        sound.Volume = SoundVolume;
        sound.IsLooped = loop;
        sound.Play();
    }

    public bool IsPlaying(string soundName)
    {
        if (!Sounds.TryGetValue(soundName, out var sound)) return false;
        return sound.State == SoundState.Playing;
    }

    public void StopAllSounds()
    {
        foreach (var sound in Sounds)
            sound.Value.Stop();
    }

    public void PlayMusic(string musicName, bool loop = false)
    {
        if (!Options.Instance.Musics) return;
        if (!string.IsNullOrEmpty(CurrentMusic)) return;

        var path = Path.Combine(Directories.Musics.FullName, musicName);
        if (!File.Exists(path)) return;
        if (!MusicExtensions.Contains(Path.GetExtension(path))) return;

        // Load audio file into a Song and start playback.
        try
        {
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Volume = MusicVolume;
            MediaPlayer.Play(Song.FromUri(musicName, new Uri(path)));
            CurrentMusic = musicName;
        }
        catch
        {
            // Skip unsupported formats silently.
        }
    }

    public void StopMusic()
    {
        CurrentMusic = string.Empty;
        try
        {
            MediaPlayer.Stop();
        }
        catch
        {
            // MediaPlayer may throw if no song is loaded.
        }
    }
}
