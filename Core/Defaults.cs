using CryBits.Entities;

namespace CryBits
{
    public static class Defaults
    {
        // Configurações
        public static string GameName = "CryBits";
        public static string WelcomeMessage = "Welcome to CryBits.";
        public static short Port = 7001;
        public static byte MaxPlayers = 15;
        public static byte MaxCharacters = 3;
        public static byte MaxPartyMembers = 3;
        public static byte MaxMapItems = 100;
        public static byte NumPoints = 3;
        public static byte MaxNameLength = 12;
        public static byte MinNameLength = 3;
        public static byte MaxPasswordLength = 12;
        public static byte MinPasswordLength = 3;

        // Tamanho da grade do jogo
        public const byte Grid = 32;

        // Limites fixos
        public const byte MaxInventory = 30;
        public const byte MaxHotbar = 10;

        // Ataque
        public const short AttackSpeed = 750;

        // Animação
        public const byte AnimationRight = 0;
        public const byte AnimationStopped = 1;
        public const byte AnimationLeft = 2;
        public const byte AnimationAttack = 2;
        public const byte AnimationAmount = 4;

        // Movimentação
        public const byte MovementUp = 3;
        public const byte MovementDown = 0;
        public const byte MovementLeft = 1;
        public const byte MovementRight = 2;

        // Tamanho da tela
        public const short ScreenWidth = Map.Width * Grid;
        public const short ScreenHeight = Map.Height * Grid;

        // Clima
        public const byte MaxRain = 100;
        public const short MaxSnow = 635;
        public const byte MaxWeatherIntensity = 10;
        public const byte SnowMovement = 10;
    }
}