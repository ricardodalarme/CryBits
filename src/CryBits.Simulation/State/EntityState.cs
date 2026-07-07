namespace CryBits.Simulation.State;

public sealed class EntityState(EntityId id)
{
    public EntityId Id { get; } = id;
    private readonly Dictionary<Type, object> _components = [];
    private readonly Dictionary<Type, long> _componentVersions = [];
    private readonly Dictionary<Type, long> _removalVersions = [];

    public T? Get<T>() where T : class => _components.TryGetValue(typeof(T), out var v) ? (T)v : null;
    public object? Get(Type type) => _components.TryGetValue(type, out var v) ? v : null;
    public void Set<T>(T c, long currentTick) where T : class
    {
        _components[typeof(T)] = c;
        _componentVersions[typeof(T)] = currentTick;
        _removalVersions.Remove(typeof(T));
    }
    public void Set(Type type, object component, long currentTick)
    {
        _components[type] = component;
        _componentVersions[type] = currentTick;
        _removalVersions.Remove(type);
    }
    public long GetVersion(Type type) =>
        _componentVersions.TryGetValue(type, out var tick) ? tick : 0;
    public long GetRemovalVersion(Type type) =>
        _removalVersions.TryGetValue(type, out var tick) ? tick : 0;
    public bool Has<T>() where T : class => _components.ContainsKey(typeof(T));
    public bool Has(Type type) => _components.ContainsKey(type);
    public void Remove<T>(long currentTick) where T : class
    {
        var type = typeof(T);
        if (_components.Remove(type))
        {
            _componentVersions.Remove(type);
            _removalVersions[type] = currentTick;
        }
    }
    public void Remove(Type type, long currentTick)
    {
        if (_components.Remove(type))
        {
            _componentVersions.Remove(type);
            _removalVersions[type] = currentTick;
        }
    }
    public IEnumerable<(Type Type, object Value)> GetAllComponents() =>
        _components.Select(kvp => (kvp.Key, kvp.Value));

    public IEnumerable<(Type Type, long RemovalVersion)> GetRemovals() =>
        _removalVersions.Select(kvp => (kvp.Key, kvp.Value));
}
