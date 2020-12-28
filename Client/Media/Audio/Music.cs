using System.IO;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using CryBits.Enums;

namespace CryBits.Client.Media.Audio
{
    internal static class Music
    {
        // Formato em o dispositivo irá ler as músicas
        private const string Format = ".ogg";

        // Lista das músicas
        private static SFML.Audio.Music _device;

        // Index da música reproduzida atualmente
        private static byte _current;

        public static void Play(Musics index, bool loop = false)
        {
            string directory = Directories.Musics.FullName + (byte)index + Format;

            // Apenas se necessário
            if (_device != null) return;
            if (!Options.Musics) return;
            if (!File.Exists(directory)) return;

            // Carrega o áudio
            _device = new SFML.Audio.Music(directory)
            {
                Volume = 20,
                Loop = loop
            };

            // Reproduz
            _device.Play();
            _current = (byte)index;
        }

        public static void Stop()
        {
            if (_device == null || _current == 0) return;

            // Para a música que está tocando
            _device.Stop();
            _device.Dispose();
            _device = null;
        }
    }
}