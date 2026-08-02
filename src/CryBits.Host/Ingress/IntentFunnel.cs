using CryBits.Simulation.Intents;

namespace CryBits.Host.Ingress;

public sealed class IntentFunnel
{
    private readonly Lock _lock = new();
    private readonly List<Intent> _pending = [];

    public void Submit(Intent intent)
    {
        lock (_lock)
        {
            _pending.Add(intent);
        }
    }

    public IntentBuffer Drain()
    {
        var buffer = new IntentBuffer();
        lock (_lock)
        {
            foreach (var intent in _pending)
                buffer.Enqueue(intent);
            _pending.Clear();
        }

        return buffer;
    }
}
