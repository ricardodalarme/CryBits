namespace CryBits.Client.Components;

/// <summary>
/// Experience and level-up state for the local player.
/// </summary>
internal struct ExperienceComponent(int current, int needed, short points)
{
    public int Current = current;
    public int Needed = needed;
    public short Points = points;
}
