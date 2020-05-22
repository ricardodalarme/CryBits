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

    public static void Create_Maps()
    {
        foreach (Objects.Map Map in Lists.Map.Values)
        {
            Objects.TMap Temp_Map = new Objects.TMap(Map.ID, Map);

            // NPCs do mapa
            Temp_Map.NPC = new Objects.TNPC[Map.NPC.Length];
            for (byte i = 1; i < Temp_Map.NPC.Length; i++)
            {
                Temp_Map.NPC[i] = new Objects.TNPC(i, Temp_Map, Map.NPC[i].NPC);
                Temp_Map.NPC[i].Spawn();
            }

            // Itens do mapa
            Temp_Map.Spawn_Items();
        }
    }
}