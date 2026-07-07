using CryBits.Simulation.Events;

namespace CryBits.Simulation.Core;

enum ComponentAction
{
    Added,
    Changed,
    Removed
}

public sealed class WorldEvents
{
    private readonly Dictionary<Type, List<IComponentSubscription>> _subs = [];

    public ComponentObserver<T> On<T>() where T : class => new(this);

    internal IDisposable Register<T>(EntityId? entityFilter, Type? tagFilter,
        Action<ComponentAdded<T>>? onAdded,
        Action<ComponentChanged<T>>? onChanged,
        Action<ComponentRemoved<T>>? onRemoved) where T : class
    {
        var sub = new ComponentSubscription<T>(this, entityFilter, tagFilter, onAdded, onChanged, onRemoved);
        var type = typeof(T);

        if (!_subs.TryGetValue(type, out var list))
        {
            list = [];
            _subs[type] = list;
        }

        list.Add(sub);
        return sub;
    }

    internal void Unregister(IComponentSubscription sub)
    {
        foreach (var list in _subs.Values)
        {
            if (list.Remove(sub))
                break;
        }
    }

    internal void Raise(World world, EntityId entity, Type type, object? oldValue, object? newValue, ComponentAction action)
    {
        if (!_subs.TryGetValue(type, out var list))
            return;

        foreach (var sub in list)
        {
            if (sub.Matches(world, entity))
                sub.Invoke(entity, oldValue, newValue, action);
        }
    }
}

internal interface IComponentSubscription
{
    bool Matches(World world, EntityId entity);
    void Invoke(EntityId entity, object? oldValue, object? newValue, ComponentAction action);
}

internal sealed class ComponentSubscription<T>(WorldEvents owner, EntityId? entityFilter, Type? tagFilter,
    Action<ComponentAdded<T>>? onAdded,
    Action<ComponentChanged<T>>? onChanged,
    Action<ComponentRemoved<T>>? onRemoved) : IComponentSubscription, IDisposable where T : class
{
    private readonly WorldEvents _owner = owner;
    private readonly EntityId? _entityFilter = entityFilter;
    private readonly Type? _tagFilter = tagFilter;
    private readonly Action<ComponentAdded<T>>? _onAdded = onAdded;
    private readonly Action<ComponentChanged<T>>? _onChanged = onChanged;
    private readonly Action<ComponentRemoved<T>>? _onRemoved = onRemoved;

    public bool Matches(World world, EntityId entity)
    {
        if (_entityFilter.HasValue && _entityFilter.Value != entity) return false;
        if (_tagFilter != null && !world.Has(entity, _tagFilter)) return false;
        return true;
    }

    public void Invoke(EntityId entity, object? oldValue, object? newValue, ComponentAction action)
    {
        switch (action)
        {
            case ComponentAction.Added:
                _onAdded?.Invoke(new ComponentAdded<T>(entity, (T)newValue!));
                break;
            case ComponentAction.Changed:
                _onChanged?.Invoke(new ComponentChanged<T>(entity, (T)newValue!, (T)oldValue!));
                break;
            case ComponentAction.Removed:
                _onRemoved?.Invoke(new ComponentRemoved<T>(entity, (T)oldValue!));
                break;
        }
    }

    public void Dispose() => _owner.Unregister(this);
}

public sealed class ComponentObserver<T> where T : class
{
    private readonly WorldEvents _events;
    private EntityId? _entityFilter;
    private Type? _tagFilter;

    internal ComponentObserver(WorldEvents events) => _events = events;

    public ComponentObserver<T> Entity(EntityId id)
    {
        _entityFilter = id;
        return this;
    }

    public ComponentObserver<T> With<Tag>() where Tag : class
    {
        _tagFilter = typeof(Tag);
        return this;
    }

    public IDisposable OnAdded(Action<ComponentAdded<T>> handler) =>
        _events.Register(_entityFilter, _tagFilter, handler, null, null);

    public IDisposable OnChanged(Action<ComponentChanged<T>> handler) =>
        _events.Register(_entityFilter, _tagFilter, null, handler, null);

    public IDisposable OnRemoved(Action<ComponentRemoved<T>> handler) =>
        _events.Register(_entityFilter, _tagFilter, null, null, handler);
}
