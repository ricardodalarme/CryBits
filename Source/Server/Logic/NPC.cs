using System;

class NPC
{
    public enum Behaviour
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked,
        ShopKeeper
    }

    public enum Movements
    {
        MoveRandomly,
        TurnRandomly,
        StandStill
    }
}