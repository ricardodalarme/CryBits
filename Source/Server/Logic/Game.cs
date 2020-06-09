using System;

class Game
{
    // CPS do servidor
    public static int CPS;

    // Bloqueio direcional
    public const byte Max_DirBlock = 3;

    // Limites em geral
    public const byte Max_Inventory = 30;
    public const byte Max_Hotbar = 10;

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
}