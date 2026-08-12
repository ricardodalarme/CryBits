using CryBits.Client.Network.Senders;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Simulation.Intents;
using Microsoft.Xna.Framework.Input;

namespace CryBits.Client.Input;

/// <summary>
/// Registers and handles keyboard shortcuts that are active during gameplay.
/// Separated from UI Window handling so game-logic bindings live in the logic layer.
/// </summary>
internal class GameInput(IntentSender intentSender, Chat chat, InputManager input, UiContext uiContext)
{
    public void Bind()
    {
        input.OnKeyReleased += OnKeyReleased;
    }

    public void Unbind()
    {
        input.OnKeyReleased -= OnKeyReleased;
    }

    private void OnKeyReleased(Keys key)
    {
        switch (key)
        {
            case Keys.Enter: chat.Type(); break;
            case Keys.D1: UseHotbar(1); break;
            case Keys.D2: UseHotbar(2); break;
            case Keys.D3: UseHotbar(3); break;
            case Keys.D4: UseHotbar(4); break;
            case Keys.D5: UseHotbar(5); break;
            case Keys.D6: UseHotbar(6); break;
            case Keys.D7: UseHotbar(7); break;
            case Keys.D8: UseHotbar(8); break;
            case Keys.D9: UseHotbar(9); break;
            case Keys.D0: UseHotbar(0); break;
        }
    }

    private void UseHotbar(byte slot)
    {
        if (uiContext.UISystem?.FocusedEntity != null) return;
        intentSender.Send(new HotbarUseIntent(default, slot));
    }
}
