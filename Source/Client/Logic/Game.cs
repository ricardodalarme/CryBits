using System;

namespace CryBits.Client.Logic
{
    static class Game
    {
        // Nome do jogo
        public static string Game_Name = "CryBits";

        // Dimensão das grades 
        public const byte Grid = 32;

        // Medida de calculo do atraso do jogo
        public static short FPS;

        // Ataque
        public const short AttackSpeed = 750;

        // Animação
        public const byte AnimationAmount = 4;
        public const byte AnimationStopped = 1;
        public const byte AnimationRight = 0;
        public const byte AnimationLeft = 2;
        public const byte AnimationAttack = 2;

        // Movimentação
        public const byte MovementUp = 3;
        public const byte MovementDown = 0;
        public const byte MovementLeft = 1;
        public const byte MovementRight = 2;

        // Bloqueio direcional
        public const byte MaxDirBlock = 3;

        // Tamanho da tela
        public const short ScreenWidth = MapWidth * Grid;
        public const short ScreenHeight = MapHeight * Grid;

        // Limites em geral
        public const byte MaxInventory = 30;
        public const byte MaxHotbar = 10;

        // Limitações dos mapas
        public const byte MapWidth = 25;
        public const byte MapHeight = 19;

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
