using System;
using System.IO;
using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using SFML.Audio;

namespace CryBits.Editors.Media
{
    internal class Audio
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
            None,
            Menu
        }

        public class Sound
        {
            // Formato em que o dispositivo irá ler os sons
            public const string Format = ".wav";

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

            public static void Play(Sounds index, bool loop = false)
            {
                // Somente se necessário
                if (EditorMaps.Form.Visible && !EditorMaps.Form.butAudio.Checked) return;

                // Reproduz o áudio
                List[(byte)index].Volume = 20;
                List[(byte)index].Loop = loop;
                List[(byte)index].Play();
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
}