using System;
using System.Collections.Generic;

class Map
{
    ////////////////
    // Numerações //
    ////////////////
    public enum Morals
    {
        Pacific,
        Dangerous,
        Amount
    }

    public enum Attributes
    {
        None,
        Block,
        Warp,
        Item,
        Amount
    }

    public static void Logic()
    {
        foreach (Objects.TMap Temp_Map in Lists.Temp_Map.Values) Temp_Map.Logic();

        // Reseta as contagens
        if (Environment.TickCount > Loop.Timer_NPC_Regen + 5000) Loop.Timer_NPC_Regen = Environment.TickCount;
        if (Environment.TickCount > Loop.Timer_Map_Items + 300000) Loop.Timer_Map_Items = Environment.TickCount;
    }
}