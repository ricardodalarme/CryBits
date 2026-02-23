using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace CryBits.Client.ECS;

/// <summary>
/// Entity registry and component manager.
///
/// Entities are plain <c>int</c> ids — no wrapper struct, no allocations.
/// Components are class instances stored per-type in <see cref="ComponentStore{T}"/>.
///
/// Scale rationale: this is a 2D MMORPG client with at most a few hundred
/// live entities (players, NPCs, blood splats, dropped items on one map).
/// A simple dictionary-backed store matches that domain perfectly — no need
/// for archetype / chunk layouts.
/// </summary>
public sealed class World
{
    // ─── Entity lifecycle ───────────────────────────────────────────────────

    private int _next;
    private readonly HashSet<int> _alive = [];
    private readonly Dictionary<Type, IComponentStore> _stores = [];

    /// <summary>Allocate a new entity and return its id.</summary>
    public int Create()
    {
        var id = Interlocked.Increment(ref _next);
        _alive.Add(id);
        return id;
    }

    /// <summary>Destroy an entity and remove all its components.</summary>
    public void Destroy(int id)
    {
        if (!_alive.Remove(id)) return;
        foreach (var store in _stores.Values)
            store.Remove(id);
    }

    /// <summary>Returns true when the entity is still alive.</summary>
    public bool IsAlive(int id) => _alive.Contains(id);

    /// <summary>Destroy all entities and wipe every component store.</summary>
    public void Clear()
    {
        _alive.Clear();
        foreach (var store in _stores.Values)
            store.Clear();
    }

    // ─── Component access ───────────────────────────────────────────────────

    private ComponentStore<T> StoreOf<T>() where T : class, IComponent
    {
        var type = typeof(T);
        if (!_stores.TryGetValue(type, out var raw))
            _stores[type] = raw = new ComponentStore<T>();
        return (ComponentStore<T>)raw;
    }

    /// <summary>Attach (or replace) a component on an existing entity.</summary>
    public void Add<T>(int id, T component) where T : class, IComponent =>
        StoreOf<T>().Set(id, component);

    /// <summary>Try to retrieve a component; returns false when absent.</summary>
    public bool TryGet<T>(int id, [NotNullWhen(true)] out T? component) where T : class, IComponent =>
        StoreOf<T>().TryGet(id, out component);

    /// <summary>
    /// Retrieve a component; throws if the entity does not have it.
    /// Prefer <see cref="TryGet{T}"/> in hot paths.
    /// </summary>
    public T Get<T>(int id) where T : class, IComponent =>
        TryGet<T>(id, out var c)
            ? c
            : throw new InvalidOperationException(
                $"Entity {id} has no {typeof(T).Name} component.");

    /// <summary>Returns true when the entity has the component type.</summary>
    public bool Has<T>(int id) where T : class, IComponent => StoreOf<T>().Has(id);

    /// <summary>Remove a specific component type from an entity.</summary>
    public void Remove<T>(int id) where T : class, IComponent => ((IComponentStore)StoreOf<T>()).Remove(id);

    // ─── Queries ────────────────────────────────────────────────────────────

    /// <summary>Enumerate all (id, T) pairs.</summary>
    public IEnumerable<(int Id, T Component)> Query<T>() where T : class, IComponent =>
        StoreOf<T>().All();

    /// <summary>Enumerate all entities that have both T1 and T2.</summary>
    public IEnumerable<(int Id, T1 C1, T2 C2)> Query<T1, T2>()
        where T1 : class, IComponent
        where T2 : class, IComponent =>
        StoreOf<T1>().All()
            .Where(x => Has<T2>(x.Id))
            .Select(x => (x.Id, x.Component, Get<T2>(x.Id)));

    /// <summary>Enumerate all entities that have T1, T2 and T3.</summary>
    public IEnumerable<(int Id, T1 C1, T2 C2, T3 C3)> Query<T1, T2, T3>()
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent =>
        StoreOf<T1>().All()
            .Where(x => Has<T2>(x.Id) && Has<T3>(x.Id))
            .Select(x => (x.Id, x.Component, Get<T2>(x.Id), Get<T3>(x.Id)));

    /// <summary>
    /// Return the single entity that has T, or -1 when none exists.
    /// Useful for singleton tags like <c>LocalPlayerTag</c>.
    /// </summary>
    public int FindSingle<T>() where T : class, IComponent
    {
        foreach (var (id, _) in StoreOf<T>().All())
            return id;
        return -1;
    }
}
