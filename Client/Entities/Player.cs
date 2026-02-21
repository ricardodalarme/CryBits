using System;
using System.Collections.Generic;
using CryBits.Entities;

namespace CryBits.Client.Entities;

internal class Player(string name) : Character
{
    // Player collection
    public static List<Player> List;

    /// <summary>Find a player by name.</summary>
    public static Player Get(string name) => List.Find(x => x.Name.Equals(name));

    // Local player instance
    public static Me Me;

    // Player data
    public string Name { get; set; } = name;
    public short TextureNum { get; set; }
    public short Level { get; set; }
    public short[] MaxVital { get; set; } = new short[(byte)Enums.Vital.Count];
    public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
    public Item[] Equipment { get; set; } = new Item[(byte)Enums.Equipment.Count];
    public MapInstance MapInstance;

    public virtual void Logic()
    {
        // Reset hurt flag when damage timer expires.
        if (Hurt + 325 < Environment.TickCount) Hurt = 0;

        // Process movement.
        ProcessMovement();
    }
}
