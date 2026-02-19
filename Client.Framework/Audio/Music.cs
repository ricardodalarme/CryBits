using CryBits.Client.Framework.Constants;

namespace CryBits.Client.Framework.Audio;

public static class Music
{
    // Current music playback device.
    public static SFML.Audio.Music? Device;

    // Currently playing music name.
    public static string Current = string.Empty;

    public static void Play(string music, bool loop = false)
    {
        var directory = Path.Combine(Directories.Musics.FullName, music);

        // Return early if a music device already exists or file missing.
        if (Device != null) return;
        if (!File.Exists(directory)) return;

        // Load audio file into SFML Music.
        Device = new SFML.Audio.Music(directory)
        {
            Volume = 20,
            IsLooping = loop
        };

        // Start playback.
        Device.Play();
        Current = music;
    }

    public static void Stop()
    {
        // Stop and dispose current music.
        Current = string.Empty;
        Device?.Stop();
        Device?.Dispose();
        Device = null;
    }
}
