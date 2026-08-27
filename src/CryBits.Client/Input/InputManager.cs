using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace CryBits.Client.Input;

public class InputManager(Desktop desktop)
{
    private readonly Desktop _desktop = desktop;

    private KeyboardState _previousKeyboardState;
    private KeyboardState _currentKeyboardState;
    private MouseState _currentMouseState;

    /// <summary>Fired when a key is released this frame.</summary>
    public event Action<Keys>? OnKeyReleased;

    /// <summary>
    /// Capture the current input state. Call once per Update frame.
    /// Detects key-release transitions vs the previous frame and invokes OnKeyReleased event.
    /// </summary>
    public void Capture()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
        _currentMouseState = Mouse.GetState();

        foreach (var key in _previousKeyboardState.GetPressedKeys())
            if (_currentKeyboardState.IsKeyUp(key))
                OnKeyReleased?.Invoke(key);
    }

    /// <summary>
    /// Checks if a key is currently held down. Polled from the MonoGame keyboard state.
    /// Skips input when window is unfocused or a UI element has focus.
    /// </summary>
    public bool IsKeyDown(Keys key)
    {
        if (_desktop.FocusedKeyboardWidget != null) return false;
        return _currentKeyboardState.IsKeyDown(key);
    }

    /// <summary>Returns true if the key was released this frame.</summary>
    public bool WasKeyReleased(Keys key)
    {
        if (_desktop.FocusedKeyboardWidget != null) return false;
        return _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);
    }

    /// <summary>
    /// Current mouse position relative to the game window in screen pixels.
    /// </summary>
    public Point MousePosition => _currentMouseState.Position;
}
