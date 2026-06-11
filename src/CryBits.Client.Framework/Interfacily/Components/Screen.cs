using CryBits.Client.Framework.Interfacily.Interfaces;
using SFML.Window;

namespace CryBits.Client.Framework.Interfacily.Components;

/// <summary>Represents a UI screen that dispatches input events to its component tree.</summary>
public class Screen : IMouseMoved, IMouseDown, IMouseUp, IMouseDoubleClick, IKeyReleased
{
    public string Name { get; set; } = string.Empty;
    public List<Component> Body { get; set; } = [];

    public event Action<KeyEventArgs>? OnKeyReleased;

    public static Screen? Current;

    private void Traverse<T>(Action<T> invoke) where T : class
    {
        var stack = new Stack<List<Component>>();
        stack.Push(Body);
        while (stack.Count != 0)
        {
            foreach (var child in stack.Pop())
                if (child.Visible)
                {
                    if (child is T t) invoke(t);
                    stack.Push(child.Children);
                }
        }
    }

    public void MouseMoved() => Traverse<IMouseMoved>(c => c.MouseMoved());

    public void MouseDown(MouseButtonEventArgs e) => Traverse<IMouseDown>(c => c.MouseDown(e));

    public void MouseUp() => Traverse<IMouseUp>(c => c.MouseUp());

    public void MouseDoubleClick(MouseButtonEventArgs e) => Traverse<IMouseDoubleClick>(c => c.MouseDoubleClick(e));

    public void KeyReleased(KeyEventArgs e)
    {
        OnKeyReleased?.Invoke(e);
    }
}
