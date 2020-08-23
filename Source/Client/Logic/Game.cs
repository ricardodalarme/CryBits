namespace Logic
{
    static class Game
    {
        // Dimensão das grades 
        public const byte Grid = 32;

        // Medida de calculo do atraso do jogo
        public static short FPS;

        // Ataque
        public const short Attack_Speed = 750;

        // Animação
        public const byte Animation_Amount = 4;
        public const byte Animation_Stopped = 1;
        public const byte Animation_Right = 0;
        public const byte Animation_Left = 2;
        public const byte Animation_Attack = 2;

        // Movimentação
        public const byte Movement_Up = 3;
        public const byte Movement_Down = 0;
        public const byte Movement_Left = 1;
        public const byte Movement_Right = 2;

        // Bloqueio direcional
        public const byte Max_DirBlock = 3;

        // Tamanho da tela
        public const short Screen_Width = Map_Width * Grid;
        public const short Screen_Height = Map_Height * Grid;

        // Limites em geral
        public const byte Max_Inventory = 30;
        public const byte Max_Hotbar = 10;

        // Limitações dos mapas
        public const byte Map_Width = 25;
        public const byte Map_Height = 19;
    }
}
