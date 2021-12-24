using CryBits.Client.Framework.Constants;

namespace CryBits.Client.Framework.Audio;

public static class Music
{
    // Lista das músicas
    public static SFML.Audio.Music? Device;

    // Index da música reproduzida atualmente
    public static string Current = string.Empty;

    public static void Play(string music, bool loop = false)
    {
        var directory = Directories.Musics.FullName + music;

        // Apenas se necessário
        if (Device != null) return;
        if (!File.Exists(directory)) return;

        // Carrega o áudio
        Device = new SFML.Audio.Music(directory)
        {
            Volume = 20,
            Loop = loop
        };

        // Reproduz
        Device.Play();
        Current = music;
    }

    public static void Stop()
    {
        // Para a música que está tocando
        Current = string.Empty;
        Device?.Stop();
        Device?.Dispose();
        Device = null;
    }
}