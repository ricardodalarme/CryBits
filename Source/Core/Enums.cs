namespace CryBits
{
    public enum ToolsTypes
    {
        Button,
        Panel,
        CheckBox,
        TextBox,
        Count
    }

    public enum Layers
    {
        Ground,
        Fringe,
        Count
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

    public enum Weathers
    {
        Normal,
        Raining,
        Thundering,
        Snowing
    }

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right,
        Count
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

    public enum WindowsTypes
    {
        Menu,
        Game,
        Count
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

    // Formas de adicionar o mini azulejo
    public enum AddMode
    {
        None,
        Inside,
        Exterior,
        Horizontal,
        Vertical,
        Fill
    }

    public enum Accesses
    {
        None,
        Moderator,
        Editor,
        Administrator
    }

    public enum Messages
    {
        Game,
        Map,
        Global,
        Private
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
        Count
    }

    public enum Targets
    {
        Player = 1,
        NPC
    }

    public enum Hotbars
    {
        None,
        Item
    }

    public enum TradeStatus
    {
        Waiting,
        Confirmed,
        Accepted,
        Declined
    }

    public enum Movements
    {
        Stopped,
        Walking,
        Moving
    }

    public enum Target
    {
        Player = 1,
        NPC
    }

    public enum LayerAttributes
    {
        None,
        Block,
        Warp,
        Count
    }

    public enum Morals
    {
        Pacific,
        Dangerous,
        Count
    }
}