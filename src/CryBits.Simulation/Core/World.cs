using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Spatial;

namespace CryBits.Simulation.Core;

public sealed partial class World
{
    public DefinitionCatalog Catalog { get; }
    public Dictionary<Guid, Map> MapDefs { get; } = [];
    public ChunkGrid SpatialGrid { get; set; } = new();
    public long TickCount { get; set; }
    public EntityRegistry Entities { get; } = new();
    public WorldEvents Events { get; } = new();
    public DirtyTracking? Dirty { get; }

    public World(DefinitionCatalog catalog, bool enableDirtyTracking = true)
    {
        Catalog = catalog;
        if (enableDirtyTracking) Dirty = new DirtyTracking();

        SetupSpatialGrid();
    }

    private void SetupSpatialGrid()
    {
        Events.On<Position>()
            .OnAdded(e => SpatialGrid.Add(e.Entity, e.Component.X, e.Component.Y));
        Events.On<Position>()
            .OnChanged(e => SpatialGrid.Move(e.Entity, e.Previous.X, e.Previous.Y, e.Component.X, e.Component.Y));
        Events.On<Position>()
            .OnRemoved(e => SpatialGrid.Remove(e.Entity));
    }

    public EntityId? FindPlayer(string name)
    {
        foreach (var entity in Entities.All)
        {
            if (!Has<PlayerTag>(entity)) continue;
            var appearance = Get<PlayerAppearance>(entity)!;
            if (appearance.Name.Equals(name))
                return entity;
        }

        return null;
    }

    public void Destroy(EntityId id)
    {
        foreach (var (type, value) in Entities.GetAllComponents(id)) Events.Raise(this, id, type, value, null, ComponentAction.Removed);

        SpatialGrid.Remove(id);
        Entities.Destroy(id);
    }

    public void Clear()
    {
        Entities.Clear();
        SpatialGrid = new ChunkGrid();
    }

    public SpawnBuilder SpawnBuilder()
    {
        return new SpawnBuilder(this);
    }
}

public sealed class SpawnBuilder(World world)
{
    public SpawnBuilder With<T>(T component) where T : class
    {
        world.Set(Id, component);
        return this;
    }

    public EntityId Id { get; } = world.Create();
}
