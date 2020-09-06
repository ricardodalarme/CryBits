using Entities;
using System;

namespace Logic
{
    static class Utils
    {
        // Números aleatórios
        public static Random MyRandom = new Random();

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
        public const byte Max_Inventory = 30;
        public const byte Max_Hotbar = 10;
        public const byte Max_DirBlock = 3;

        public static Directions ReverseDirection(Directions Direction)
        {
            // Retorna a direção inversa
            switch (Direction)
            {
                case Directions.Up: return Directions.Down;
                case Directions.Down: return Directions.Up;
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                default: return Directions.Count;
            }
        }

        public static void NextTile(Directions Direction, ref byte X, ref byte Y)
        {
            // Próximo azulejo
            switch (Direction)
            {
                case Directions.Up: Y -= 1; break;
                case Directions.Down: Y += 1; break;
                case Directions.Right: X += 1; break;
                case Directions.Left: X -= 1; break;
            }
        }

        public static void Swap<T>(ref T Item1, ref T Item2)
        {
            // Troca dois elementos
            T Temp = Item1;
            Item1 = Item2;
            Item2 = Temp;
        }

        // Obtém o ID de alguma entidade, caso ela não existir retorna um ID zerado
        public static string GetID(this Entity Object) => Object == null ? Guid.Empty.ToString() : Object.ID.ToString();
    }
}