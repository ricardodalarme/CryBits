namespace Logic
{
    public enum Tools_Types
    {
        Button,
        Panel,
        CheckBox,
        TextBox,
        Count
    }

    public enum Tile_Attributes
    {
        None,
        Block,
        Warp,
        Item,
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

    public enum Map_Morals
    {
        Pacific,
        Dangerous
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

    public enum Windows
    {
        Menu,
        Game
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
}
