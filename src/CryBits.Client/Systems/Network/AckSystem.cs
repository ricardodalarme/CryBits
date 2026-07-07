using CryBits.Client.Network.Senders;

namespace CryBits.Client.Systems.Network;

internal sealed class AckSystem(AckSender ackSender) : IClientSystem
{
    private const float AckInterval = 0.2f;
    private float _timer;

    public void Update(float dt)
    {
        _timer += dt;
        if (_timer < AckInterval) return;
        _timer -= AckInterval;
        ackSender.SendAck();
    }
}
