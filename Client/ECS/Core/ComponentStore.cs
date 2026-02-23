using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CryBits.Client.ECS;

/// <summary>
/// Typed storage for a single component kind, keyed by entity id.
/// One instance exists per component type inside <see cref="World"/>.
/// </summary>
internal sealed class ComponentStore<T> : IComponentStore where T : class, IComponent
{
    private readonly Dictionary<int, T> _data = new();

    public void Set(int id, T component) => _data[id] = component;

    public bool TryGet(int id, [NotNullWhen(true)] out T? component) =>
        _data.TryGetValue(id, out component);

    public bool Has(int id) => _data.ContainsKey(id);

    void IComponentStore.Remove(int id) => _data.Remove(id);

    void IComponentStore.Clear() => _data.Clear();

    /// <summary>Enumerate every (entityId, component) pair in this store.</summary>
    public IEnumerable<(int Id, T Component)> All() =>
        System.Linq.Enumerable.Select(_data, kv => (kv.Key, kv.Value));
}
