using CryBits.Client.Framework;
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
    /// <summary>
    /// Subscribe to the game screen's key-released event.
    /// Call once at startup alongside other Bind() calls.
    /// </summary>
    public static void Bind() =>
        Screens.Game.OnKeyReleased += OnKeyReleased;

    private static void OnKeyReleased(KeyEventArgs e)
    {
        // Each condition is checked in order; if multiple actions share the same key
        // the first match wins.  Duplicate bindings can be avoided by editing Keybinds.json.
        var key = e.Code;

        if (key == KeyBindings.Chat)         { Chat.Type();                                   return; }
        if (key == KeyBindings.Collect)      { GameContext.Instance.LocalPlayer.CollectItem(); return; }
        if (key == KeyBindings.Hotbar1)      { PlayerSender.Instance.HotbarUse(1);            return; }
        if (key == KeyBindings.Hotbar2)      { PlayerSender.Instance.HotbarUse(2);            return; }
        if (key == KeyBindings.Hotbar3)      { PlayerSender.Instance.HotbarUse(3);            return; }
        if (key == KeyBindings.Hotbar4)      { PlayerSender.Instance.HotbarUse(4);            return; }
        if (key == KeyBindings.Hotbar5)      { PlayerSender.Instance.HotbarUse(5);            return; }
        if (key == KeyBindings.Hotbar6)      { PlayerSender.Instance.HotbarUse(6);            return; }
        if (key == KeyBindings.Hotbar7)      { PlayerSender.Instance.HotbarUse(7);            return; }
        if (key == KeyBindings.Hotbar8)      { PlayerSender.Instance.HotbarUse(8);            return; }
        if (key == KeyBindings.Hotbar9)      { PlayerSender.Instance.HotbarUse(9);            return; }
        if (key == KeyBindings.Hotbar0)      { PlayerSender.Instance.HotbarUse(0);            return; }
    }
}
