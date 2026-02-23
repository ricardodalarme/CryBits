using System;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics;
using CryBits.Client.Network.Senders;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Handles attack-key input for the locally controlled entity and resets the
/// attacking flag once the attack cooldown expires.
/// </summary>
internal sealed class AttackSystem : IUpdateSystem
{
    public void Update(GameContext ctx)
    {
        var localId = ctx.GetLocalPlayer();
        if (localId < 0) return;

        if (!ctx.World.TryGet<AnimationComponent>(localId, out var animation)) return;

        // Reset attacking state when cooldown expires.
        var now = Environment.TickCount;
        if (animation.IsAttacking && animation.AttackTimer + AttackSpeed < now)
        {
            animation.AttackTimer = 0;
            animation.IsAttacking = false;
        }

        // Only process new attacks in the Game screen when the window is focused.
        if (Screen.Current != Screens.Game) return;
        if (!Renders.RenderWindow.HasFocus()) return;
        if (!Keyboard.IsKeyPressed(Keyboard.Key.LControl)) return;
        if (animation.AttackTimer > 0) return;

        // Block attacks while UI panels are open.
        if (Panels.Trade.Visible || Panels.Shop.Visible) return;

        animation.AttackTimer = now;
        PlayerSender.PlayerAttack();
    }
}
