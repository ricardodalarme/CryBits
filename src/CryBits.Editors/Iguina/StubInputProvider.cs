using Iguina.Defs;
using Iguina.Drivers;

namespace CryBits.Editors.Iguina;

internal sealed class StubInputProvider : IInputProvider
{
    public Point GetMousePosition() => new();
    public bool IsMouseButtonDown(MouseButton btn) => false;
    public int GetMouseWheelChange() => 0;
    public int[] GetTextInput() => [];
    public TextInputCommands[] GetTextInputCommands() => [];
    public KeyboardInteractions? GetKeyboardInteraction() => null;
}
