namespace Logic
{
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

    public enum MapMorals
    {
        Pacific,
        Dangerous,
    }

    public enum NPCBehaviour
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked,
        ShopKeeper
    }

    public enum NPCMovements
    {
        MoveRandomly,
        TurnRandomly,
        StandStill
    }

    public enum TileAttributes
    {
        None,
        Block,
        Warp,
        Item,
    }

    public enum Targets
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

    public enum Hotbars
    {
        None,
        Item
    }

    public enum BindOn
    {
        None,
        Pickup,
        Equip
    }

    public enum TradeStatus
    {
        Waiting,
        Confirmed,
        Accepted,
        Declined
    }
}
