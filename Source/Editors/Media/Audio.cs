using SFML.Audio;
using System;

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
        // Formato em que o dispositivo irá ler os sons
        private const string Format = ".wav";

        // Dispositivo sonoro
        public static SFML.Audio.Sound[] List;

        public static void Load()
        {
            // Redimensiona a lista
            Array.Resize(ref List, (byte)Sounds.Count);

            // Carrega todos os arquivos e os adiciona a lista
            for (int i = 1; i < List.Length; i++)
                List[i] = new SFML.Audio.Sound(new SoundBuffer(Directories.Sounds.FullName + i + Format));
        }

        public static void Play(Sounds Index, bool Loop = false)
        {
            // Somente se necessário
            if ((byte)Index == 0) return; 
            if (Editor_Maps.Objects.Visible && !Editor_Maps.Objects.butAudio.Checked) return;

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
            for (byte i = 1; i < (byte)Sounds.Count; i++)
                List[i].Stop();
        }
    }

    public class Music
    {
        // Formato em o dispositivo irá ler as músicas
        private const string Format = ".ogg";

        // Lista das músicas
        public static SFML.Audio.Music Device;

        // Índice da música reproduzida atualmente
        public static byte Current;

        public static void Play(Musics Index, bool Loop = false)
        {
            System.IO.FileInfo File = new System.IO.FileInfo(Directories.Musics.FullName + (byte)Index + Format);

            // Apenas se necessário
            if (Device != null) return;
            if (Editor_Maps.Objects.Visible && !Editor_Maps.Objects.butAudio.Checked) return;
            if (!File.Exists) return;

            // Carrega o áudio
            Device = new SFML.Audio.Music(Directories.Musics.FullName + (byte)Index + Format);
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