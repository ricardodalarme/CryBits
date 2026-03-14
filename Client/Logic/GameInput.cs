using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.Views;
using SFML.Window;

namespace CryBits.Client.Logic;

/// <summary>
/// Registers and handles keyboard shortcuts that are active during gameplay.
/// Separated from UI Window handling so game-logic bindings live in the logic layer.
/// </summary>
internal class GameInput(PlayerSender playerSender, Chat chat)
{
    public static GameInput Instance { get; } = new(PlayerSender.Instance, Chat.Instance);

    /// <summary>
    /// Subscribe to the game screen's key-released event.
    /// Call once at startup alongside other Bind() calls.
    /// </summary>
    public void Bind() =>
        Screens.Game.OnKeyReleased += OnKeyReleased;

    private void OnKeyReleased(KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Enter: chat.Type(); break;
            case Keyboard.Key.Num1: UseHotbar(1); break;
            case Keyboard.Key.Num2: UseHotbar(2); break;
            case Keyboard.Key.Num3: UseHotbar(3); break;
            case Keyboard.Key.Num4: UseHotbar(4); break;
            case Keyboard.Key.Num5: UseHotbar(5); break;
            case Keyboard.Key.Num6: UseHotbar(6); break;
            case Keyboard.Key.Num7: UseHotbar(7); break;
            case Keyboard.Key.Num8: UseHotbar(8); break;
            case Keyboard.Key.Num9: UseHotbar(9); break;
            case Keyboard.Key.Num0: UseHotbar(0); break;
        }
    }

    private void UseHotbar(byte slot)
    {
        if (TextBox.Focused != null) return;

        playerSender.HotbarUse(slot);
        DropItemView.Panel.Visible = false;
    }
}
