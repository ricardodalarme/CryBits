using Iguina;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CryBits.Client.Managers;

public class InputManager
{
    private UISystem? _uiSystem;
    private RenderWindow? _renderWindow;

    /// <summary>
    /// Tracks window focus state. Set by LostFocus/GainedFocus events in Renderer.Init().
    /// </summary>
    public bool IsFocused { get; set; } = true;

    public event EventHandler<MouseButtonEventArgs>? MouseButtonPressed;
    public event EventHandler<MouseButtonEventArgs>? MouseButtonReleased;
    public event EventHandler<MouseMoveEventArgs>? MouseMoved;
    public event EventHandler<KeyEventArgs>? KeyPressed;
    public event EventHandler<KeyEventArgs>? KeyReleased;
    public event EventHandler<TextEventArgs>? TextEntered;

    private readonly HashSet<Keyboard.Key> _pressedThisFrame = [];
    private readonly HashSet<Keyboard.Key> _releasedThisFrame = [];

    public void Initialize(UISystem system, RenderWindow window)
    {
        _uiSystem = system;
        _renderWindow = window;
    }

    public void BeginFrame()
    {
        _pressedThisFrame.Clear();
        _releasedThisFrame.Clear();
    }

    public void BindEvents(RenderWindow window)
    {
        window.MouseButtonPressed += (s, e) => MouseButtonPressed?.Invoke(s, e);
        window.MouseButtonReleased += (s, e) => MouseButtonReleased?.Invoke(s, e);
        window.MouseMoved += (s, e) => MouseMoved?.Invoke(s, e);
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

    /// <summary>
    /// Checks if a key is currently held down using layout-independent scancodes.
    /// Preferred for movement and game actions where physical key position matters.
    /// </summary>
    public bool IsScancodePressed(Keyboard.Scancode scancode)
    {
        if (!IsFocused) return false;

        if (_uiSystem?.FocusedEntity != null) return false;

        return Keyboard.IsScancodePressed(scancode);
    }

    public bool IsKeyPressed(Keyboard.Key key)
    {
        if (!IsFocused) return false;

        if (_uiSystem?.FocusedEntity != null) return false;

        return Keyboard.IsKeyPressed(key);
    }

    public bool WasKeyPressed(Keyboard.Key key)
    {
        if (!IsFocused) return false;
        if (_uiSystem?.FocusedEntity != null) return false;

        return _pressedThisFrame.Contains(key);
    }

    public bool WasKeyReleased(Keyboard.Key key)
    {
        if (!IsFocused) return false;
        if (_uiSystem?.FocusedEntity != null) return false;

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
            if (_renderWindow == null) return new Vector2i();
            return Mouse.GetPosition(_renderWindow);
        }
    }
}
