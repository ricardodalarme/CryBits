using CryBits.Definitions.Common;

namespace CryBits.Simulation.Components;

public sealed record PathFollow(List<Direction> Steps, int NextIndex = 0)
{
    public bool IsComplete => NextIndex >= Steps.Count;
    public Direction? CurrentStep => IsComplete ? null : Steps[NextIndex];
}
