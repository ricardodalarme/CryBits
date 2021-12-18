using System.IO;
using CryBits.Editors.Forms;
using CryBits.Editors.Library;

namespace CryBits.Editors.Media.Audio;

public static class Music
{
    // Formato em o dispositivo irá ler as músicas
    private const string Format = ".ogg";

    // Lista das músicas
    public static SFML.Audio.Music Device;

    // Index da música reproduzida atualmente
    public static Enums.Music Current;

    public static void Play(Enums.Music index, bool loop = false)
    {
        FileInfo file = new FileInfo(Directories.Musics.FullName + (byte)index + Format);

        // Apenas se necessário
        if (Device != null) return;
        if (EditorMaps.Form.Visible && !EditorMaps.Form.butAudio.Checked) return;
        if (!file.Exists) return;

        // Carrega o áudio
        Device = new SFML.Audio.Music(Directories.Musics.FullName + (byte)index + Format)
        {
            Loop = loop,
            Volume = 20
        };

        // Reproduz
        Device.Play();
        Current = index;
    }

    public static void Stop()
    {
        if (Device == null || Current == 0) return;

        // Para a música que está tocando
        Device.Stop();
        Device.Dispose();
        Device = null;
    }
}