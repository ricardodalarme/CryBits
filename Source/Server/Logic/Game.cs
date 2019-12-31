using System;

class Game
{
    // Números aleatórios
    public static Random Random = new Random();

    // O maior índice dos jogadores conectados
    public static byte HigherIndex;

    // CPS do servidor
    public static int CPS;
    public static bool CPS_Lock;

    // Bloqueio direcional
    public const byte Max_DirBlock = 3;

    // Maximo e mínimo de caracteres permidos em alguns texto
    public const byte Max_Name_Length = 12;
    public const byte Min_Name_Length = 3;

    // Limites em geral
    public const byte Max_NPC_Drop = 4;
    public const byte Max_Inventory = 30;
    public const byte Max_Map_Items = 100;
    public const byte Max_Hotbar = 10;
    public const byte Min_Map_Width = 24;
    public const byte Min_Map_Height = 18;


    #region Nums
    public enum Directions
    {
        Up,
        Down,
        Left,
        Right,
        Amount
    }

    public enum Accesses
    {
        None,
        Moderator,
        Editor,
        Administrator
    }

    public enum Genres
    {
        Male,
        Female
    }

    public enum Vitals
    {
        HP,
        MP,
        Amount
    }

    public enum Attributes
    {
        Strength,
        Resistance,
        Intelligence,
        Agility,
        Vitality,
        Amount
    }

    public enum Messages
    {
        Game,
        Map,
        Global,
        Private
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
        Amount
    }

    public enum Hotbar
    {
        None,
        Item
    }
    #endregion

    public static void SetHigherIndex()
    {
        // Redefine o índice máximo de jogadores
        HigherIndex = 0;

        for (byte i = (byte)Lists.Player.GetUpperBound(0); i >= 1; i -= 1)
            if (Socket.IsConnected(i))
            {
                HigherIndex = i;
                break;
            }

        // Envia os dados para os jogadores
        Send.HigherIndex();
    }

    public static Directions ReverseDirection(Directions Direction)
    {
        // Retorna a direção inversa
        switch (Direction)
        {
            case Directions.Up: return Directions.Down;
            case Directions.Down: return Directions.Up;
            case Directions.Left: return Directions.Right;
            case Directions.Right: return Directions.Left;
            default: return Directions.Amount;
        }
    }
}