namespace CryBits.Simulation.State;

public sealed class EntityRegistry
{
    private readonly Dictionary<EntityId, EntityState> _entities = [];
    private long _nextId = 1;

    public EntityId Create()
    {
        var id = new EntityId(_nextId++);
        _entities[id] = new EntityState(id);
        return id;
    }

    public void Destroy(EntityId id) => _entities.Remove(id);

    public EntityState? Get(EntityId id) => _entities.GetValueOrDefault(id);

    public IEnumerable<EntityState> All => _entities.Values;
}
