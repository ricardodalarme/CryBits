using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public readonly struct ComponentAdded<T>(EntityId entity, T component) where T : class
{
    public EntityId Entity { get; } = entity;
    public T Component { get; } = component;
}

public readonly struct ComponentChanged<T>(EntityId entity, T component, T previous) where T : class
{
    public EntityId Entity { get; } = entity;
    public T Component { get; } = component;
    public T Previous { get; } = previous;
}

public readonly struct ComponentRemoved<T>(EntityId entity, T previous) where T : class
{
    public EntityId Entity { get; } = entity;
    public T Previous { get; } = previous;
}
