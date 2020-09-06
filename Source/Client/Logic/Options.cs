using System;

namespace Logic
{
    [Serializable]
    static class Options
    {
        public static string Game_Name = "CryBits";
        public static bool SaveUsername = true;
        public static bool Sounds = true;
        public static bool Musics = true;
        public static bool Chat = true;
        public static bool FPS = false;
        public static bool Latency = false;
        public static bool Party = true;
        public static bool Trade = true;
        public static string Username = string.Empty;
    }
}