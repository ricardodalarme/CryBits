using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Client.UI.Game;
using SFML.Window;

namespace CryBits.Client.Logic;

/// <summary>
/// Registers and handles keyboard shortcuts that are active during gameplay.
/// Separated from UI Window handling so game-logic bindings live in the logic layer.
/// </summary>
internal static class GameInput
{
    private static readonly GameContext _context = GameContext.Instance;
    private static readonly PlayerSender _playerSender = PlayerSender.Instance;

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
            case Keyboard.Key.Space: _context.LocalPlayer.CollectItem(); break;
            case Keyboard.Key.Num1: _playerSender.HotbarUse(1); break;
            case Keyboard.Key.Num2: _playerSender.HotbarUse(2); break;
            case Keyboard.Key.Num3: _playerSender.HotbarUse(3); break;
            case Keyboard.Key.Num4: _playerSender.HotbarUse(4); break;
            case Keyboard.Key.Num5: _playerSender.HotbarUse(5); break;
            case Keyboard.Key.Num6: _playerSender.HotbarUse(6); break;
            case Keyboard.Key.Num7: _playerSender.HotbarUse(7); break;
            case Keyboard.Key.Num8: _playerSender.HotbarUse(8); break;
            case Keyboard.Key.Num9: _playerSender.HotbarUse(9); break;
            case Keyboard.Key.Num0: _playerSender.HotbarUse(0); break;
        }
    }
}
