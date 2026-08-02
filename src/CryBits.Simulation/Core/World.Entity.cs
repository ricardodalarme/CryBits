namespace CryBits.Simulation.Core;

public sealed partial class World
{
    public IEnumerable<EntityId> All => Entities.All;

    public EntityId Create()
    {
        return Entities.Create();
    }

    public bool IsAlive(EntityId id)
    {
        return Entities.All.Contains(id);
    }

    public T? Get<T>(EntityId id) where T : class
    {
        return Entities.Get<T>(id);
    }

    public bool Has<T>(EntityId id) where T : class
    {
        return Entities.Has<T>(id);
    }

    public bool Has(EntityId id, Type type)
    {
        return Entities.Has(id, type);
    }

    public void Set<T>(EntityId id, T component) where T : class
    {
        Set(id, (object)component);
    }

    public void Update<T>(EntityId id, Func<T, T> transform) where T : class
    {
        var current = Entities.Get<T>(id);
        if (current != null) Set(id, transform(current));
    }

    public void Set(EntityId id, object component)
    {
        if (!IsAlive(id)) return;

        var type = component.GetType();
        var oldValue = Entities.Get(id, type);
        Entities.Set(id, type, component, TickCount);
        Dirty?.Mark(id);

        if (oldValue == null)
            Events.Raise(this, id, type, null, component, ComponentAction.Added);
        else if (!Equals(oldValue, component))
            Events.Raise(this, id, type, oldValue, component, ComponentAction.Changed);
    }

    public void Remove<T>(EntityId id) where T : class
    {
        if (!IsAlive(id)) return;
        var oldValue = Entities.Get<T>(id);
        Entities.Remove<T>(id, TickCount);
        if (oldValue != null)
        {
            Dirty?.Mark(id);
            Events.Raise(this, id, typeof(T), oldValue, null, ComponentAction.Removed);
        }
    }

    public void Remove(EntityId id, Type type)
    {
        if (!IsAlive(id)) return;
        var oldValue = Entities.Get(id, type);
        Entities.Remove(id, type, TickCount);
        if (oldValue != null)
        {
            Dirty?.Mark(id);
            Events.Raise(this, id, type, oldValue, null, ComponentAction.Removed);
        }
    }

    public void DestroyWhere(Func<EntityId, bool> predicate)
    {
        var toDestroy = new List<EntityId>();
        foreach (var id in Entities.All)
            if (predicate(id))
                toDestroy.Add(id);

        foreach (var id in toDestroy)
            Destroy(id);
    }
}
