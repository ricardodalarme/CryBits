using System.IO;
using CryBits.Editors.Forms;
using CryBits.Editors.Library;

namespace CryBits.Editors.Media.Audio
{
    public class Music
    {
        // Formato em o dispositivo irá ler as músicas
        private const string Format = ".ogg";

        // Lista das músicas
        public static SFML.Audio.Music Device;

        // Index da música reproduzida atualmente
        public static Musics Current;

        public static void Play(Musics index, bool loop = false)
        {
            FileInfo file = new FileInfo(Directories.Musics.FullName + (byte)index + Format);

            // Apenas se necessário
            if (Device != null) return;
            if (EditorMaps.Form.Visible && !EditorMaps.Form.butAudio.Checked) return;
            if (!file.Exists) return;

            // Carrega o áudio
            Device = new SFML.Audio.Music(Directories.Musics.FullName + (byte)index + Format);
            Device.Loop = true;
            Device.Volume = 20;
            Device.Loop = loop;

            // Reproduz
            Device.Play();
            Current = index;
        }

        public static void Stop()
        {
            // Para a música que está tocando
            if (Device == null || Current == 0) return;
            Device.Stop();
            Device.Dispose();
            Device = null;
        }
    }
}