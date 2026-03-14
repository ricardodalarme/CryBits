using CryBits.Client.Framework.Interfacily.Components;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace CryBits.Client.Managers;

public class InputManager
{
    public static InputManager Instance { get; } = new();

    /// <summary>
    /// Tracks window focus state. Set by LostFocus/GainedFocus events in Renderer.Init().
    /// More efficient than polling HasFocus() on every frame.
    /// </summary>
    public bool IsFocused { get; set; } = true;

    public event EventHandler<MouseButtonEventArgs>? MouseButtonPressed;
    public event EventHandler<MouseButtonEventArgs>? MouseButtonReleased;
    public event EventHandler<MouseMoveEventArgs>? MouseMoved;
    public event EventHandler<KeyEventArgs>? KeyPressed;
    public event EventHandler<KeyEventArgs>? KeyReleased;
    public event EventHandler<TextEventArgs>? TextEntered;

    public void BindEvents(RenderWindow window)
    {
        window.MouseButtonPressed += (s, e) => MouseButtonPressed?.Invoke(s, e);
        window.MouseButtonReleased += (s, e) => MouseButtonReleased?.Invoke(s, e);
        window.MouseMoved += (s, e) => MouseMoved?.Invoke(s, e);
        window.KeyPressed += (s, e) => KeyPressed?.Invoke(s, e);
        window.KeyReleased += (s, e) => KeyReleased?.Invoke(s, e);
        window.TextEntered += (s, e) => TextEntered?.Invoke(s, e);
    }

    /// <summary>
    /// Checks if a key is currently held down using layout-independent scancodes.
    /// Preferred for movement and game actions where physical key position matters.
    /// </summary>
    public bool IsScancodePressed(Keyboard.Scancode scancode)
    {
        if (!IsFocused) return false;

        // Disable game keyboard inputs when a text box is focused.
        if (TextBox.Focused != null) return false;

        return Keyboard.IsScancodePressed(scancode);
    }

    /// <summary>
    /// Checks if a key is currently held down using layout-dependent key codes.
    /// Use for keys whose label matters (e.g. Enter, Escape, Tab, number row).
    /// </summary>
    public bool IsKeyPressed(Keyboard.Key key)
    {
        if (!IsFocused) return false;

        // Disable game keyboard inputs when a text box is focused.
        if (TextBox.Focused != null) return false;

        return Keyboard.IsKeyPressed(key);
    }

    /// <summary>
    /// Checks if a mouse button is currently held down.
    /// </summary>
    public bool IsMouseButtonPressed(Mouse.Button button)
    {
        if (!IsFocused) return false;

        return Mouse.IsButtonPressed(button);
    }

    /// <summary>
    /// Current mouse position relative to the game window in screen pixels.
    /// Use this for UI hit-testing.
    /// </summary>
    public Vector2i MousePosition
    {
        get
        {
            if (Graphics.Renderer.Instance.RenderWindow == null) return new Vector2i();
            return Mouse.GetPosition(Graphics.Renderer.Instance.RenderWindow);
        }
    }
}
