using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Simulation.Intents;
using SFML.Window;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Combat;

internal sealed class AttackSystem(
    GameContext context,
    InputManager inputManager,
    IntentSender intentSender
) : IClientSystem
{
    private const float ThrottleInterval = 0.030f;
    private float _inputThrottle;

    public void Update(float t)
    {
        var localPlayer = context.LocalPlayer;
        if (localPlayer is null) return;

        var entity = localPlayer.Entity;
        if (entity is null || !context.World.IsAlive(entity.Value)) return;

        _inputThrottle += t;
        if (_inputThrottle < ThrottleInterval) return;
        _inputThrottle = 0f;

        if (!inputManager.IsKeyPressed(Keyboard.Key.LControl)) return;

        var state = context.World.Get<AttackComponent>(entity.Value);
        if (state == null || state.AttackCountdown > 0f) return;
        if (TradeView.Panel.Visible) return;
        if (ShopView.Panel.Visible) return;

        state.AttackCountdown = AttackSpeed / 1000f;
        intentSender.Send(new AttackIntent(default, null));
    }
}
