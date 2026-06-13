using System.Collections.Generic;

namespace CryBits.Simulation.Intents;

public sealed class IntentBuffer
{
    private readonly List<Intent> _items = [];

    public IReadOnlyList<Intent> All => _items;

    public void Enqueue(Intent intent) => _items.Add(intent);

    public void Clear() => _items.Clear();
}
