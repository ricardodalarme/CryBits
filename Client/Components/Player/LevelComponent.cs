namespace CryBits.Client.Components.Player;

/// <summary>
/// Level, experience, and unspent points for a player character.
/// </summary>
internal struct LevelComponent
{
    public short Level;
    public int Experience;
    public int ExpNeeded;
    public short Points;
}
