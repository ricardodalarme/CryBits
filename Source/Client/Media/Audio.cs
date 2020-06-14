using SFML.Audio;
using System;
using System.IO;

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
        Amount
    }

    // Listas das músicas
    public enum Musics
    {
        Menu = 1,
        Amount
    }

    public class Sound
    {
        // Formato em o dispositivo irá ler os sons
        public const string Format = ".wav";

        // Dispositivo sonoro
        private static SFML.Audio.Sound[] List;

        public static void Load()
        {
            // Redimensiona a lista
            Array.Resize(ref List, (byte)Sounds.Amount);

            // Carrega todos os arquivos e os adiciona a lista
            for (int i = 1; i < List.Length; i++)
                List[i] = new SFML.Audio.Sound(new SoundBuffer(Directories.Sounds.FullName + i + Format));
        }

        public static void Play(Sounds Index, bool Loop = false)
        {
            // Apenas se necessário
            if (!Game.Option.Sounds) return;

            // Reproduz o áudio
            List[(byte)Index].Volume = 20;
            List[(byte)Index].Loop = Loop;
            List[(byte)Index].Play();
        }

        public static void Stop_All()
        {
            // Apenas se necessário
            if (List == null) return;

            // Para todos os sons
            for (byte i = 1; i < (byte)Sounds.Amount; i++)
                List[i].Stop();
        }
    }

    public class Music
    {
        // Formato em o dispositivo irá ler as músicas
        public const string Format = ".ogg";

        // Lista das músicas
        private static SFML.Audio.Music Device;

        // Index da música reproduzida atualmente
        private static byte Current;

        public static void Play(Musics Index, bool Loop = false)
        {
            string Directory = Directories.Musics.FullName + (byte)Index + Format;

            // Apenas se necessário
            if (Device != null) return;
            if (!Game.Option.Musics) return;
            if (!File.Exists(Directory)) return;

            // Carrega o áudio
            Device = new SFML.Audio.Music(Directory);
            Device.Loop = true;
            Device.Volume = 20;
            Device.Loop = Loop;

            // Reproduz
            Device.Play();
            Current = (byte)Index;
        }

        public static void Stop()
        {
            // Para a música que está tocando
            if (Device != null && Current != 0)
            {
                Device.Stop();
                Device.Dispose();
                Device = null;
            }
        }
    }
}