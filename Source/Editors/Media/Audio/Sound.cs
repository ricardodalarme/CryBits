using System;
using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using SFML.Audio;

namespace CryBits.Editors.Media.Audio
{
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
            for (int i = 1; i < List.Length; i++) List[i] = new SFML.Audio.Sound(new SoundBuffer(Directories.Sounds.FullName + i + Format));
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
            for (byte i = 1; i < (byte)Sounds.Count; i++) List[i].Stop();
        }
    }
}