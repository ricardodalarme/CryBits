using System;
using System.Collections.Generic;

namespace CryBits.Server.Simulation.State;

internal sealed class EntityState(EntityId id)
{
    public EntityId Id { get; } = id;
    private readonly Dictionary<Type, object> _components = [];

    public T? Get<T>() where T : class => _components.TryGetValue(typeof(T), out var v) ? (T)v : null;
    public void Set<T>(T c) where T : class => _components[typeof(T)] = c;
    public bool Has<T>() where T : class => _components.ContainsKey(typeof(T));
    public void Remove<T>() where T : class => _components.Remove(typeof(T));
}
