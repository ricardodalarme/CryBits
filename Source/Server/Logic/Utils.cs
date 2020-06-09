using System;

static class Utils
{
    // Números aleatórios
    public static Random Random = new Random();

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