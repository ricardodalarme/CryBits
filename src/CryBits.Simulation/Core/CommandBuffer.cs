namespace CryBits.Simulation.Core;

public sealed class CommandBuffer(World world)
{
    private readonly List<Action<World>> _commands = [];

    public EntityId Create()
    {
        return world.Create();
    }

    public void Set<T>(EntityId id, T component) where T : class
    {
        _commands.Add(w => w.Set(id, component));
    }

    public void Update<T>(EntityId id, Func<T, T> transform) where T : class
    {
        _commands.Add(w =>
        {
            var current = w.Get<T>(id);
            if (current != null)
                w.Set(id, transform(current));
        });
    }

    public void Remove<T>(EntityId id) where T : class
    {
        _commands.Add(w => w.Remove<T>(id));
    }

    public void Remove(EntityId id, Type type)
    {
        _commands.Add(w => w.Remove(id, type));
    }

    public void Destroy(EntityId id)
    {
        _commands.Add(w => w.Destroy(id));
    }

    public void Flush()
    {
        foreach (var cmd in _commands)
            cmd(world);
        _commands.Clear();
    }
}
