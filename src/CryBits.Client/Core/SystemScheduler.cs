namespace CryBits.Client.Core;

public sealed class SystemScheduler
{
    private readonly List<IClientSystem> _simSystems = [];
    private readonly List<IClientRenderSystem> _renderSystems = [];

    public SystemScheduler AddSimulation(IClientSystem s) { _simSystems.Add(s); return this; }
    public SystemScheduler AddRender(IClientRenderSystem s) { _renderSystems.Add(s); return this; }

    public void Update(float dt) { foreach (var s in _simSystems) s.Update(dt); }
    public void Render() { foreach (var s in _renderSystems) s.Render(); }
}
