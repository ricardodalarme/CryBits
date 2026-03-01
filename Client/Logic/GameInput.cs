using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI;
using SFML.Window;

namespace CryBits.Client.Logic;

/// <summary>
/// Registers and handles keyboard shortcuts that are active during gameplay.
/// Separated from UI Window handling so game-logic bindings live in the logic layer.
/// </summary>
internal static class GameInput
{
    /// <summary>
    /// Subscribe to the game screen's key-released event.
    /// Call once at startup alongside other Bind() calls.
    /// </summary>
    public static void Bind() =>
        Screens.Game.OnKeyReleased += OnKeyReleased;

    private static void OnKeyReleased(KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Enter: Chat.Type(); break;
            case Keyboard.Key.Space: Player.Me.CollectItem(); break;
            case Keyboard.Key.Num1: PlayerSender.Instance.HotbarUse(1); break;
            case Keyboard.Key.Num2: PlayerSender.Instance.HotbarUse(2); break;
            case Keyboard.Key.Num3: PlayerSender.Instance.HotbarUse(3); break;
            case Keyboard.Key.Num4: PlayerSender.Instance.HotbarUse(4); break;
            case Keyboard.Key.Num5: PlayerSender.Instance.HotbarUse(5); break;
            case Keyboard.Key.Num6: PlayerSender.Instance.HotbarUse(6); break;
            case Keyboard.Key.Num7: PlayerSender.Instance.HotbarUse(7); break;
            case Keyboard.Key.Num8: PlayerSender.Instance.HotbarUse(8); break;
            case Keyboard.Key.Num9: PlayerSender.Instance.HotbarUse(9); break;
            case Keyboard.Key.Num0: PlayerSender.Instance.HotbarUse(0); break;
        }
    }
}
