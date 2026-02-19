using CryBits.Client.Framework.Interfacily.Interfaces;
using SFML.Window;

namespace CryBits.Client.Framework.Interfacily.Components;

public class Screen : IMouseMoved, IMouseDown, IMouseUp, IMouseDoubleClick, IKeyReleased
{
    public string Name { get; set; } = string.Empty;
    public List<Component> Body { get; set; } = [];

    // Eventos
    public event Action? OnMouseUp;
    public event Action? OnMouseDown;
    public event Action<KeyEventArgs>? OnKeyReleased;

    // Janela atual
    public static Screen? Current;

    public void MouseMoved()
    {
        // Percorre toda a árvore de ordem para executar o comando
        var stack = new Stack<List<Component>>();
        stack.Push(Current.Body);
        while (stack.Count != 0)
        {
            var top = stack.Pop();

            for (byte i = 0; i < top.Count; i++)
                if (top[i].Visible)
                {
                    // Executa o comando
                    if (top[i] is IMouseMoved component) component.MouseMoved();
                    stack.Push(top[i].Children);
                }
        }

        OnMouseUp?.Invoke();
    }

    public void MouseDown(MouseButtonEventArgs e)
    {
        // Percorre toda a árvore de ordem para executar o comando
        var stack = new Stack<List<Component>>();
        stack.Push(Current.Body);
        while (stack.Count != 0)
        {
            var top = stack.Pop();

            for (byte i = 0; i < top.Count; i++)
                if (top[i].Visible)
                {
                    // Executa o comando
                    if (top[i] is IMouseDown component) component.MouseDown(e);

                    stack.Push(top[i].Children);
                }
        }

        OnMouseDown?.Invoke();
    }

    public void MouseUp()
    {
        // Percorre toda a árvore de ordem para executar o comando
        var stack = new Stack<List<Component>>();
        stack.Push(Current.Body);
        while (stack.Count != 0)
        {
            var top = stack.Pop();

            for (byte i = 0; i < top.Count; i++)
                if (top[i].Visible)
                {
                    // Executa o comando
                    if (top[i] is IMouseUp component) component.MouseUp();
                    stack.Push(top[i].Children);
                }
        }
    }

    public void MouseDoubleClick(MouseButtonEventArgs e)
    {
        // Percorre toda a árvore de ordem para executar o comando
        var stack = new Stack<List<Component>>();
        stack.Push(Current.Body);
        while (stack.Count != 0)
        {
            var top = stack.Pop();

            for (byte i = 0; i < top.Count; i++)
                if (top[i].Visible)
                {
                    // Executa o comando
                    if (top[i] is IMouseDoubleClick component) component.MouseDoubleClick(e);
                    stack.Push(top[i].Children);
                }
        }
    }

    public void KeyReleased(KeyEventArgs e)
    {
        OnKeyReleased?.Invoke(e);
    }
}