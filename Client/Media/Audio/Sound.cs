using System;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using SFML.Audio;

namespace CryBits.Client.Media.Audio
{
    public static class Sound
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
            for (int i = 1; i < _list.Length; i++) _list[i] = new SFML.Audio.Sound(new SoundBuffer(Directories.Sounds.FullName + i + Format));
        }

        public static void Play(Sounds index, bool loop = false)
        {
            // Apenas se necessário
            if (!Options.Sounds) return;

            // Reproduz o áudio
            _list[(byte)index].Volume = 20;
            _list[(byte)index].Loop = loop;
            _list[(byte)index].Play();
        }

        public static void StopAll()
        {
            // Apenas se necessário
            if (_list == null) return;

            // Para todos os sons
            for (byte i = 1; i < (byte)Sounds.Count; i++) _list[i].Stop();
        }
    }
}