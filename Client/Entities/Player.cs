using System.Collections.Generic;

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
    public MapInstance MapInstance;
}
