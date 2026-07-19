using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Input;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using Iguina.Entities;
using SFML.Window;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Combat;

internal sealed class AttackSystem(
    GameContext context,
    InputManager inputManager,
    IntentSender intentSender,
    UiContext uiContext
) : IClientSystem
{
    private const float ThrottleInterval = 0.030f;
    private float _inputThrottle;

    public void Update(World world, float t)
    {
        var entity = context.LocalPlayerEntity;
        if (entity is null || !world.IsAlive(entity.Value)) return;

        _inputThrottle += t;
        if (_inputThrottle < ThrottleInterval) return;
        _inputThrottle = 0f;

        if (!inputManager.IsKeyPressed(Keyboard.Key.LControl)) return;

        var attack = world.Get<AttackComponent>(entity.Value);
        if (attack == null || attack.AttackCountdown > 0f) return;
        if (uiContext.TryGet<Panel>("Trade", out var tradePanel) && tradePanel.Visible) return;
        if (uiContext.TryGet<Panel>("Shop", out var shopPanel) && shopPanel.Visible) return;

        world.Set(entity.Value, new AttackComponent(AttackSpeed / 1000f));
        intentSender.Send(new AttackIntent(default, null));
    }
}
