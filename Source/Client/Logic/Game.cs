using System;
using System.Drawing;

class Game
{
    // Números aleatórios
    public static Random Random = new Random();

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

    // Visão do jogador
    private static Rectangle Camera;
    public static Rectangle Tile_Sight;

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

    public enum Attributes
    {
        Strength,
        Resistance,
        Intelligence,
        Agility,
        Vitality,
        Count
    }

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right,
        Count
    }

    public enum Movements
    {
        Stopped,
        Walking,
        Moving
    }

    public enum Messages
    {
        Game,
        Map,
        Global,
        Private
    }

    public enum Vitals
    {
        HP,
        MP,
        Count
    }

    public enum NPCs
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked,
        ShopKeeper
    }

    public enum Target
    {
        Player = 1,
        NPC
    }

    public enum Items
    {
        None,
        Equipment,
        Potion
    }

    public enum Equipments
    {
        Weapon,
        Armor,
        Helmet,
        Shield,
        Amulet,
        Count
    }

    public enum Hotbar
    {
        None,
        Item
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Count
    }

    public enum BindOn
    {
        None,
        Pickup,
        Equip,
        Count
    }

    public enum Trade_Status
    {
        Waiting,
        Confirmed,
        Accepted,
        Declined
    }

    // Converte o valor em uma posição adequada à camera
    public static int ConvertX(int x) => x - (Tile_Sight.X * Grid) - Camera.X;
    public static int ConvertY(int y) => y - (Tile_Sight.Y * Grid) - Camera.Y;

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

    public static void UpdateCamera()
    {
        Point End = new Point(), Start = new Point(), Position = new Point();

        // Centro da tela
        Position.X = Player.Me.X2 + Grid;
        Position.Y = Player.Me.Y2 + Grid;

        // Início da tela
        Start.X = Player.Me.X - ((Map_Width + 1) / 2) - 1;
        Start.Y = Player.Me.Y - ((Map_Height + 1) / 2) - 1;

        // Reajusta a posição horizontal da tela
        if (Start.X < 0)
        {
            Position.X = 0;
            if (Start.X == -1 && Player.Me.X2 > 0) Position.X = Player.Me.X2;
            Start.X = 0;
        }

        // Reajusta a posição vertical da tela
        if (Start.Y < 0)
        {
            Position.Y = 0;
            if (Start.Y == -1 && Player.Me.Y2 > 0) Position.Y = Player.Me.Y2;
            Start.Y = 0;
        }

        // Final da tela
        End.X = Start.X + (Map_Width + 1) + 1;
        End.Y = Start.Y + (Map_Height + 1) + 1;

        // Reajusta a posição horizontal da tela
        if (End.X > Map_Width)
        {
            Position.X = Grid;
            if (End.X == Map_Width + 1 && Player.Me.X2 < 0) Position.X = Player.Me.X2 + Grid;
            End.X = Map_Width;
            Start.X = End.X - Map_Width - 1;
        }

        // Reajusta a posição vertical da tela
        if (End.Y > Map_Height)
        {
            Position.Y = Grid;
            if (End.Y == Map_Height + 1 && Player.Me.Y2 < 0) Position.Y = Player.Me.Y2 + Grid;
            End.Y = Map_Height;
            Start.Y = End.Y - Map_Height - 1;
        }

        // Define a dimensão dos azulejos vistos
        Tile_Sight.Y = Start.Y;
        Tile_Sight.Height = End.Y;
        Tile_Sight.X = Start.X;
        Tile_Sight.Width = End.X;

        // Define a posição da câmera
        Camera.Y = Position.Y;
        Camera.Height = Camera.Y + Screen_Height;
        Camera.X = Position.X;
        Camera.Width = Camera.X + Screen_Width;
    }
}