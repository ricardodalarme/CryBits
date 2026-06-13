using System.Collections.Generic;

namespace CryBits.Server.Simulation.State;

internal sealed class EntityRegistry
{
    private readonly Dictionary<EntityId, EntityState> _entities = [];

    public EntityId Create()
    {
        var id = new EntityId(System.Guid.NewGuid());
        _entities[id] = new EntityState(id);
        return id;
    }

    public void Destroy(EntityId id) => _entities.Remove(id);

    public EntityState? Get(EntityId id) => _entities.GetValueOrDefault(id);

    public IEnumerable<EntityState> All => _entities.Values;
}
