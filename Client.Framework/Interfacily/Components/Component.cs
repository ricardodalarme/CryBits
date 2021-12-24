using System.Drawing;

namespace CryBits.Client.Framework.Interfacily.Components;

public abstract class Component
{
    public string Name { get; set; } = string.Empty;
    public Point Position { get; set; }
    public bool Visible { get; set; }
    public Component? Parent { get; set; }
    public List<Component> Children { get; set; } = new ();
}