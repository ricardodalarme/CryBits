using System;

class Game
{
    // Números aleatórios
    public static Random Random = new Random();

    // O maior índice dos jogadores conectados
    public static byte HigherIndex;

    // CPS do servidor
    public static int CPS;

    // Bloqueio direcional
    public const byte Max_DirBlock = 3;

    // Maximo e mínimo de caracteres permidos em alguns texto
    public const byte Max_Name_Length = 12;
    public const byte Min_Name_Length = 3;

    // Limites em geral
    public const byte Max_Inventory = 30;
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
        Count
    }

    public enum Accesses
    {
        None,
        Moderator,
        Editor,
        Administrator
    }

    public enum Vitals
    {
        HP,
        MP,
        Count
    }

    public enum Attributes
    {
        Strength,
        Resistance,
        Intelligence,
        Agility,
        Vitality,
        Count
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
        Count
    }

    public enum Hotbar
    {
        None,
        Item
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
            default: return Directions.Count;
        }
    }

    public static Lists.Structures.Shop_Item Shop_Buy(short Index, short Item_Num)
    {
        // Verifica se a loja vende determinado item
        for (byte i = 0; i < Lists.Shop[Index].Bought.Length; i++)
            if (Lists.Shop[Index].Bought[i].Item_Num == Item_Num)
                return Lists.Shop[Index].Bought[i];
        return null;
    }
}