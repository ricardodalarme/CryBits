using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Core;

public sealed class World
{
    public DefinitionCatalog Catalog { get; }
    public Dictionary<Guid, Map> MapDefs { get; } = [];
    public ChunkGrid SpatialGrid { get; set; } = new();
    public long TickCount { get; set; }
    public EntityRegistry Entities { get; } = new();

    public World(DefinitionCatalog catalog, bool enableDirtyTracking = true)
    {
        Catalog = catalog;
        if (enableDirtyTracking) Dirty = new DirtyTracking();
    }

    public DirtyTracking? Dirty { get; }

    public EntityId? FindPlayer(string name)
    {
        foreach (var state in Entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var appearance = state.Get<PlayerAppearance>()!;
            if (appearance.Name.Equals(name))
                return state.Id;
        }
        return null;
    }

    public EntityId Spawn()
    {
        return Entities.Create();
    }

    public void Destroy(EntityId id)
    {
        SpatialGrid.Remove(id);
        Entities.Destroy(id);
    }

    public bool IsAlive(EntityId id)
    {
        return Entities.Get(id) != null;
    }

    public void Clear()
    {
        Entities.Clear();
        SpatialGrid = new ChunkGrid();
    }

    public T? Get<T>(EntityId id) where T : class
    {
        return Entities.Get(id)?.Get<T>();
    }

    public bool Has<T>(EntityId id) where T : class
    {
        return Entities.Get(id)?.Has<T>() ?? false;
    }

    public void Set<T>(EntityId id, T component) where T : class
    {
        Set(id, (object)component);
    }

    public void Set(EntityId id, object component)
    {
        var state = Entities.Get(id);
        if (state == null) return;

        if (component is Position newPos)
        {
            var oldPos = state.Get<Position>();
            if (oldPos != null)
                SpatialGrid.Move(id, oldPos.X, oldPos.Y, newPos.X, newPos.Y);
            else
                SpatialGrid.Add(id, newPos.X, newPos.Y);
        }

        var type = component.GetType();
        state.Set(type, component);
        Dirty?.Mark(id, type);
    }

    public void Remove<T>(EntityId id) where T : class
    {
        Entities.Get(id)?.Remove<T>();
    }

    public T? AddOrGet<T>(EntityId id) where T : class, new()
    {
        var state = Entities.Get(id);
        if (state == null) return null;
        var c = state.Get<T>();
        if (c == null)
        {
            c = new T();
            state.Set(c);
            Dirty?.Mark<T>(id);
        }
        return c;
    }

    public bool TryGet<T>(EntityId id, out T? component) where T : class
    {
        component = Get<T>(id);
        return component != null;
    }

    public void DestroyWhere(Func<EntityState, bool> predicate)
    {
        var toDestroy = new List<EntityId>();
        foreach (var state in Entities.All)
        {
            if (predicate(state))
                toDestroy.Add(state.Id);
        }
        foreach (var id in toDestroy)
            Destroy(id);
    }

    public IEnumerable<EntityState> All => Entities.All;

    public void Update<T>(EntityId id, Func<T, T> transform) where T : class
    {
        var current = Entities.Get(id)?.Get<T>();
        if (current != null) Set(id, transform(current));
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

    public EntityId Id { get; } = world.Spawn();
}
