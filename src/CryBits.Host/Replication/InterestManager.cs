using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.State;

namespace CryBits.Host.Replication;

public sealed class InterestManager(World world)
{
    private readonly ChunkGrid _grid = world.SpatialGrid;
    private readonly Dictionary<EntityId, HashSet<ChunkCoord>> _subscriptions = [];
    private const int AoiRadius = 2;

    public SubscriptionDiff Update(EntityId observer)
    {
        var pos = world.Get<Position>(observer);
        if (pos == null) return new SubscriptionDiff();

        var center = ChunkGrid.FromPosition(pos.X, pos.Y);
        var desired = _grid.GetNeighborhood(center, AoiRadius);
        var current = _subscriptions.GetValueOrDefault(observer) ?? [];

        var diff = new SubscriptionDiff
        {
            Entered = desired.Except(current).ToHashSet(),
            Left = current.Except(desired).ToHashSet()
        };

        _subscriptions[observer] = desired;
        return diff;
    }

    public void RemoveObserver(EntityId observer)
    {
        _subscriptions.Remove(observer);
    }

    public IEnumerable<EntityId> GetObservableEntities(EntityId observer)
    {
        if (!_subscriptions.TryGetValue(observer, out var chunks))
            return [];
        return _grid.GetEntities(chunks);
    }
}

public sealed record SubscriptionDiff
{
    public HashSet<ChunkCoord> Entered { get; init; } = [];
    public HashSet<ChunkCoord> Left { get; init; } = [];
}
