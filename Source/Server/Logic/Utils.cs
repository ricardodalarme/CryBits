namespace CryBits.Server.Logic
{
    internal static class Utils
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

        // Limites fixos
        public const byte MaxInventory = 30;
        public const byte MaxHotbar = 10;
    }
}