using SFML.Graphics;
using SFML.System;
using SFML.Window;

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
    public event EventHandler<MouseWheelScrollEventArgs>? MouseWheelScrolled;

    // Edge-detection: keys pressed or released during the current frame.
    private readonly HashSet<Keyboard.Key> _pressedThisFrame = [];
    private readonly HashSet<Keyboard.Key> _releasedThisFrame = [];

    private int _mouseWheelDelta;

    /// <summary>
    /// Clears per-frame edge state. Must be called once per frame, before DispatchEvents.
    /// </summary>
    public void BeginFrame()
    {
        _pressedThisFrame.Clear();
        _releasedThisFrame.Clear();
        _mouseWheelDelta = 0;
    }

    public void BindEvents(RenderWindow window)
    {
        window.MouseButtonPressed += (s, e) => MouseButtonPressed?.Invoke(s, e);
        window.MouseButtonReleased += (s, e) => MouseButtonReleased?.Invoke(s, e);
        window.MouseMoved += (s, e) => MouseMoved?.Invoke(s, e);
        window.MouseWheelScrolled += (s, e) =>
        {
            _mouseWheelDelta += (int)e.Delta;
            MouseWheelScrolled?.Invoke(s, e);
        };
        window.KeyPressed += (s, e) =>
        {
            _pressedThisFrame.Add(e.Code);
            KeyPressed?.Invoke(s, e);
        };
        window.KeyReleased += (s, e) =>
        {
            _releasedThisFrame.Add(e.Code);
            KeyReleased?.Invoke(s, e);
        };
        window.TextEntered += (s, e) => TextEntered?.Invoke(s, e);
    }

    public int ConsumeMouseWheelDelta()
    {
        var delta = _mouseWheelDelta;
        _mouseWheelDelta = 0;
        return delta;
    }

    /// <summary>
    /// Checks if a key is currently held down using layout-independent scancodes.
    /// Preferred for movement and game actions where physical key position matters.
    /// </summary>
    public bool IsScancodePressed(Keyboard.Scancode scancode)
    {
        if (!IsFocused) return false;

        // Disable game keyboard inputs when a text box is focused.

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
    /// Returns true if the key was pressed (went down) during the current frame.
    /// Use for one-shot actions triggered on key press.
    /// </summary>
    public bool WasKeyPressed(Keyboard.Key key)
    {
        if (!IsFocused) return false;

        return _pressedThisFrame.Contains(key);
    }

    /// <summary>
    /// Returns true if the key was pressed this frame, ignoring focus and textbox checks.
    /// Use for UI systems (Iguina) that manage their own focus state.
    /// </summary>
    public bool WasKeyPressedRaw(Keyboard.Key key) =>
        _pressedThisFrame.Contains(key);

    /// <summary>
    /// Returns true if the key was released (went up) during the current frame.
    /// Use for one-shot actions triggered on key release.
    /// </summary>
    public bool WasKeyReleased(Keyboard.Key key)
    {
        if (!IsFocused) return false;

        return _releasedThisFrame.Contains(key);
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
