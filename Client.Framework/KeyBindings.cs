using SFML.Window;

namespace CryBits.Client.Framework;

/// <summary>
/// Holds the current runtime key bindings for all game actions.
/// Values are loaded from <c>Data/Keybinds.json</c> at startup;
/// defaults match the original hardcoded layout.
/// </summary>
public static class KeyBindings
{
    // ── Movement (scancode-based — physical key position, layout-independent) ─

    /// <summary>Move the player upward.</summary>
    public static Keyboard.Scancode MoveUp = Keyboard.Scancode.Up;

    /// <summary>Move the player downward.</summary>
    public static Keyboard.Scancode MoveDown = Keyboard.Scancode.Down;

    /// <summary>Move the player to the left.</summary>
    public static Keyboard.Scancode MoveLeft = Keyboard.Scancode.Left;

    /// <summary>Move the player to the right.</summary>
    public static Keyboard.Scancode MoveRight = Keyboard.Scancode.Right;

    // ── Held-action keys (key-code-based — logical/label keys) ───────────────

    /// <summary>Hold to run instead of walk.</summary>
    public static Keyboard.Key Run = Keyboard.Key.LShift;

    /// <summary>Hold to perform a melee attack.</summary>
    public static Keyboard.Key Attack = Keyboard.Key.LControl;

    // ── Discrete-action keys (fired on key-release) ───────────────────────────

    /// <summary>Open / send chat message.</summary>
    public static Keyboard.Key Chat = Keyboard.Key.Enter;

    /// <summary>Collect a ground item.</summary>
    public static Keyboard.Key Collect = Keyboard.Key.Space;

    /// <summary>Use hotbar slot 1.</summary>
    public static Keyboard.Key Hotbar1 = Keyboard.Key.Num1;

    /// <summary>Use hotbar slot 2.</summary>
    public static Keyboard.Key Hotbar2 = Keyboard.Key.Num2;

    /// <summary>Use hotbar slot 3.</summary>
    public static Keyboard.Key Hotbar3 = Keyboard.Key.Num3;

    /// <summary>Use hotbar slot 4.</summary>
    public static Keyboard.Key Hotbar4 = Keyboard.Key.Num4;

    /// <summary>Use hotbar slot 5.</summary>
    public static Keyboard.Key Hotbar5 = Keyboard.Key.Num5;

    /// <summary>Use hotbar slot 6.</summary>
    public static Keyboard.Key Hotbar6 = Keyboard.Key.Num6;

    /// <summary>Use hotbar slot 7.</summary>
    public static Keyboard.Key Hotbar7 = Keyboard.Key.Num7;

    /// <summary>Use hotbar slot 8.</summary>
    public static Keyboard.Key Hotbar8 = Keyboard.Key.Num8;

    /// <summary>Use hotbar slot 9.</summary>
    public static Keyboard.Key Hotbar9 = Keyboard.Key.Num9;

    /// <summary>Use hotbar slot 0.</summary>
    public static Keyboard.Key Hotbar0 = Keyboard.Key.Num0;
}
