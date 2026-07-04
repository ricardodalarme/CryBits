using CryBits.Simulation.State;
using System.Collections;

namespace CryBits.Host.Core;

internal sealed class SessionManager : IEnumerable<Session>
{
    private readonly List<Session> _sessions = [];
    private readonly Dictionary<EntityId, Session> _entityMap = [];

    public void Add(Session session) => _sessions.Add(session);
    public void Remove(Session session) => _sessions.Remove(session);
    public Session this[int index] => _sessions[index];
    public Session? Find(Predicate<Session> match) => _sessions.Find(match);

    public void Register(EntityId id, Session s) => _entityMap[id] = s;
    public void Unregister(EntityId id) => _entityMap.Remove(id);
    public Session? Get(EntityId id) => _entityMap.GetValueOrDefault(id);

    public IEnumerator<Session> GetEnumerator() => _sessions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_sessions).GetEnumerator();
}
