using CryBits.Entities;
using System;

namespace CryBits
{
    static class Utils
    {
        // Números aleatórios
        public static Random MyRandom = new Random();

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