using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Combat;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.Systems.Combat;

/// <summary>
/// Polls keyboard state to drive local-player attacks.
/// </summary>
internal class AttackSystem(
    GameContext context,
    InputManager inputManager,
    PlayerSender playerSender
) : BaseSystem<World, float>(context.World)
{
    private const float ThrottleInterval = 0.030f;
    private float _inputThrottle;

    public override void Update(in float t)
    {
        var entity = context.LocalPlayer.Entity;
        if (entity == Entity.Null || !World.IsAlive(entity)) return;

        _inputThrottle += t;
        if (_inputThrottle < ThrottleInterval) return;
        _inputThrottle = 0f;

        if (!inputManager.IsKeyPressed(Keyboard.Key.LControl)) return;

        ref var state = ref World.Get<AttackComponent>(entity);
        if (state.AttackCountdown > 0f) return;
        if (TradeView.Panel.Visible) return;
        if (ShopView.Panel.Visible) return;

        state.AttackCountdown = AttackSpeed / 1000f;
        playerSender.PlayerAttack();
    }
}
