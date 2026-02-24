namespace CryBits.Client.Components;

/// <summary>
/// World-pixel position of an entity.
/// </summary>
internal struct TransformComponent(int x, int y)
{
    public int X = x;
    public int Y = y;
}
