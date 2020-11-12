using CryBits.Client.Library;
using SFML.Audio;
using System;
using System.IO;
using static CryBits.Client.Logic.Game;

class Audio
{
    // Lista dos sons
    public enum Sounds
    {
        Click = 1,
        Above,
        Rain,
        Thunder_1,
        Thunder_2,
        Thunder_3,
        Thunder_4,
        Count
    }

    // Listas das músicas
    public enum Musics
    {
        Menu = 1,
        Count
    }

    public class Sound
    {
        // Formato em o dispositivo irá ler os sons
        public const string Format = ".wav";

        // Dispositivo sonoro
        private static SFML.Audio.Sound[] _list;

        public static void Load()
        {
            // Redimensiona a lista
            Array.Resize(ref _list, (byte)Sounds.Count);

            // Carrega todos os arquivos e os adiciona a lista
            for (int i = 1; i < _list.Length; i++)
                _list[i] = new SFML.Audio.Sound(new SoundBuffer(Directories.Sounds.FullName + i + Format));
        }

        public static void Play(Sounds index, bool loop = false)
        {
            // Apenas se necessário
            if (!Option.Sounds) return;

            // Reproduz o áudio
            _list[(byte)index].Volume = 20;
            _list[(byte)index].Loop = loop;
            _list[(byte)index].Play();
        }

        public static void Stop_All()
        {
            // Apenas se necessário
            if (_list == null) return;

            // Para todos os sons
            for (byte i = 1; i < (byte)Sounds.Count; i++)
                _list[i].Stop();
        }
    }

    public class Music
    {
        // Formato em o dispositivo irá ler as músicas
        public const string Format = ".ogg";

        // Lista das músicas
        private static SFML.Audio.Music _device;

        // Index da música reproduzida atualmente
        private static byte _current;

        public static void Play(Musics index, bool loop = false)
        {
            string directory = Directories.Musics.FullName + (byte)index + Format;

            // Apenas se necessário
            if (_device != null) return;
            if (!Option.Musics) return;
            if (!File.Exists(directory)) return;

            // Carrega o áudio
            _device = new SFML.Audio.Music(directory);
            _device.Loop = true;
            _device.Volume = 20;
            _device.Loop = loop;

            // Reproduz
            _device.Play();
            _current = (byte)index;
        }

        public static void Stop()
        {
            // Para a música que está tocando
            if (_device != null && _current != 0)
            {
                _device.Stop();
                _device.Dispose();
                _device = null;
            }
        }
    }
}