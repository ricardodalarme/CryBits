using System;

namespace CryBits.Client.Logic
{
    internal static class Game
    {
        // Medida de calculo do atraso do jogo
        public static short FPS;



        // Opções
        public static Options Option = new Options();

        [Serializable]
        public class Options
        {
            public bool SaveUsername = true;
            public string Username = string.Empty;
            public bool Sounds = true;
            public bool Musics = true;
            public bool Chat = true;
            public bool FPS = false;
            public bool Latency = false;
            public bool Party = true;
            public bool Trade = true;
        }
    }
}
