namespace CryBits.Simulation.State;

public sealed class EntityState(EntityId id)
{
    public EntityId Id { get; } = id;
    private readonly Dictionary<Type, object> _components = [];

    public T? Get<T>() where T : class => _components.TryGetValue(typeof(T), out var v) ? (T)v : null;
    public void Set<T>(T c) where T : class => _components[typeof(T)] = c;
    public void Set(Type type, object component) => _components[type] = component;
    public bool Has<T>() where T : class => _components.ContainsKey(typeof(T));
    public void Remove<T>() where T : class => _components.Remove(typeof(T));
    public IEnumerable<(Type Type, object Value)> GetAllComponents() =>
        _components.Select(kvp => (kvp.Key, kvp.Value));
}
