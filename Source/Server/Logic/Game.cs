using System;

class Game
{
    // Números aleatórios
    public static Random Random = new Random();

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
    public const byte Min_Map_Width = 25;
    public const byte Min_Map_Height = 19;

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

    public enum Map_Morals
    {
        Pacific,
        Dangerous,
        Amount
    }

    public enum NPC_Behaviour
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked,
        ShopKeeper
    }

    public enum NPC_Movements
    {
        MoveRandomly,
        TurnRandomly,
        StandStill
    }

    public enum Tile_Attributes
    {
        None,
        Block,
        Warp,
        Item,
        Amount
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

    public static void NextTile(Directions Direction, ref short X, ref short Y)
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

    public static Objects.Player FindPlayer(string Name)
    {
        // Encontra o usuário
        for (byte i = 0; i < Lists.Account.Count; i++)
            if (Lists.Account[i].IsPlaying)
                if (Lists.Account[i].Character.Name.Equals(Name))
                    return Lists.Account[i].Character;

        return null;
    }
}