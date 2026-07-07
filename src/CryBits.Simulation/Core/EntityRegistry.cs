namespace CryBits.Simulation.Core;

public sealed class EntityRegistry
{
    private readonly HashSet<EntityId> _aliveEntities = [];
    private readonly Dictionary<Type, Dictionary<EntityId, object>> _components = [];
    private readonly Dictionary<Type, Dictionary<EntityId, long>> _componentVersions = [];
    private readonly Dictionary<Type, Dictionary<EntityId, long>> _removalVersions = [];

    private long _nextId = 1;

    public EntityId Create()
    {
        var id = new EntityId(_nextId++);
        _aliveEntities.Add(id);
        return id;
    }

    public void Destroy(EntityId id)
    {
        if (_aliveEntities.Remove(id))
        {
            foreach (var type in _components.Keys)
            {
                if (_components[type].Remove(id))
                {
                    _componentVersions[type].Remove(id);
                    if (!_removalVersions.TryGetValue(type, out var rDict))
                    {
                        rDict = [];
                        _removalVersions[type] = rDict;
                    }
                    rDict[id] = 0; // Destroy doesn't really have a tick, we can just use 0 or current tick. Let's just track it loosely.
                }
            }
        }
    }

    public void Clear()
    {
        _aliveEntities.Clear();
        _components.Clear();
        _componentVersions.Clear();
        _removalVersions.Clear();
    }

    public IEnumerable<EntityId> All => _aliveEntities;

    // Component Methods
    public void Set(EntityId id, Type type, object component, long currentTick)
    {
        if (!_aliveEntities.Contains(id)) return;

        if (!_components.TryGetValue(type, out var cDict))
        {
            cDict = [];
            _components[type] = cDict;
        }
        cDict[id] = component;

        if (!_componentVersions.TryGetValue(type, out var vDict))
        {
            vDict = [];
            _componentVersions[type] = vDict;
        }
        vDict[id] = currentTick;

        if (_removalVersions.TryGetValue(type, out var rDict))
        {
            rDict.Remove(id);
        }
    }

    public void Set<T>(EntityId id, T component, long currentTick) where T : class
    {
        Set(id, typeof(T), component, currentTick);
    }

    public object? Get(EntityId id, Type type)
    {
        if (_components.TryGetValue(type, out var cDict) && cDict.TryGetValue(id, out var comp))
            return comp;
        return null;
    }

    public T? Get<T>(EntityId id) where T : class
    {
        return Get(id, typeof(T)) as T;
    }

    public bool Has(EntityId id, Type type)
    {
        return _components.TryGetValue(type, out var cDict) && cDict.ContainsKey(id);
    }

    public bool Has<T>(EntityId id) where T : class
    {
        return Has(id, typeof(T));
    }

    public void Remove(EntityId id, Type type, long currentTick)
    {
        if (_components.TryGetValue(type, out var cDict) && cDict.Remove(id))
        {
            if (_componentVersions.TryGetValue(type, out var vDict))
                vDict.Remove(id);

            if (!_removalVersions.TryGetValue(type, out var rDict))
            {
                rDict = [];
                _removalVersions[type] = rDict;
            }
            rDict[id] = currentTick;
        }
    }

    public void Remove<T>(EntityId id, long currentTick) where T : class
    {
        Remove(id, typeof(T), currentTick);
    }

    public long GetVersion(EntityId id, Type type)
    {
        if (_componentVersions.TryGetValue(type, out var vDict) && vDict.TryGetValue(id, out var tick))
            return tick;
        return 0;
    }

    public long GetRemovalVersion(EntityId id, Type type)
    {
        if (_removalVersions.TryGetValue(type, out var rDict) && rDict.TryGetValue(id, out var tick))
            return tick;
        return 0;
    }

    public IEnumerable<(Type Type, object Value)> GetAllComponents(EntityId id)
    {
        foreach (var kvp in _components)
        {
            if (kvp.Value.TryGetValue(id, out var comp))
                yield return (kvp.Key, comp);
        }
    }

    public IEnumerable<(Type Type, long RemovalVersion)> GetRemovals(EntityId id)
    {
        foreach (var kvp in _removalVersions)
        {
            if (kvp.Value.TryGetValue(id, out var version))
                yield return (kvp.Key, version);
        }
    }
}
