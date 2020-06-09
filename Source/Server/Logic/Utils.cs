using System;

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

    public static Game.Directions ReverseDirection(Game.Directions Direction)
    {
        // Retorna a direção inversa
        switch (Direction)
        {
            case Game.Directions.Up: return Game.Directions.Down;
            case Game.Directions.Down: return Game.Directions.Up;
            case Game.Directions.Left: return Game.Directions.Right;
            case Game.Directions.Right: return Game.Directions.Left;
            default: return Game.Directions.Count;
        }
    }

    public static void NextTile(Game.Directions Direction, ref byte X, ref byte Y)
    {
        // Próximo azulejo
        switch (Direction)
        {
            case Game.Directions.Up: Y -= 1; break;
            case Game.Directions.Down: Y += 1; break;
            case Game.Directions.Right: X += 1; break;
            case Game.Directions.Left: X -= 1; break;
        }
    }
}