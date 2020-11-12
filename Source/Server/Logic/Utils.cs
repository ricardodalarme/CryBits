namespace CryBits.Server.Logic
{
    internal static class Utils
    {
        // Configurações
        public static string Game_Name = "CryBits";
        public static string Welcome_Message = "Welcome to CryBits.";
        public static short Port = 7001;
        public static byte Max_Players = 15;
        public static byte Max_Characters = 3;
        public static byte Max_Party_Members = 3;
        public static byte Max_Map_Items = 100;
        public static byte Num_Points = 3;
        public static byte Max_Name_Length = 12;
        public static byte Min_Name_Length = 3;
        public static byte Max_Password_Length = 12;
        public static byte Min_Password_Length = 3;

        // Limites fixos
        public const byte MaxInventory = 30;
        public const byte MaxHotbar = 10;
    }
}