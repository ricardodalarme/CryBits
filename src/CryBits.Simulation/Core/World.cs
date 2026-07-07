using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
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
    public WorldEvents Events { get; } = new();

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
        var state = Entities.Get(id);
        if (state != null)
        {
            foreach (var (type, value) in state.GetAllComponents())
                Events.Raise(this, id, type, value, null, ComponentAction.Removed);
        }

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

    public bool Has(EntityId id, Type type)
    {
        return Entities.Get(id)?.Has(type) ?? false;
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
        var oldValue = state.Get(type);
        state.Set(type, component, TickCount);
        Dirty?.Mark(id);

        if (oldValue == null)
            Events.Raise(this, id, type, null, component, ComponentAction.Added);
        else if (!Equals(oldValue, component))
            Events.Raise(this, id, type, oldValue, component, ComponentAction.Changed);
    }

    public void Remove<T>(EntityId id) where T : class
    {
        var state = Entities.Get(id);
        if (state == null) return;
        var oldValue = state.Get<T>();
        state.Remove<T>(TickCount);
        if (oldValue != null)
        {
            Dirty?.Mark(id);
            Events.Raise(this, id, typeof(T), oldValue, null, ComponentAction.Removed);
        }
    }

    public void Remove(EntityId id, Type type)
    {
        var state = Entities.Get(id);
        if (state == null) return;
        var oldValue = state.Get(type);
        state.Remove(type, TickCount);
        if (oldValue != null)
        {
            Dirty?.Mark(id);
            Events.Raise(this, id, type, oldValue, null, ComponentAction.Removed);
        }
    }

    public T? AddOrGet<T>(EntityId id) where T : class, new()
    {
        var state = Entities.Get(id);
        if (state == null) return null;
        var c = state.Get<T>();
        if (c == null)
        {
            c = new T();
            state.Set(c, TickCount);
            Dirty?.Mark(id);
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
