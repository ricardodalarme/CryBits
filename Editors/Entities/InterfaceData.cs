using System.Collections.Generic;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Editors.Entities;

/// <summary>
/// Cross-platform replacement for the static EditorInterface.Tree field
/// that used to live in the WinForms EditorInterface form.
/// </summary>
internal class InterfaceData
{
    public static InterfaceData Instance { get; } = new();

    /// <summary>Root node whose children are the top-level Screen windows.</summary>
    public readonly InterfaceNode Tree = new();

    /// <summary>
    /// Populate the InterfaceNode tree from <see cref="Screens.List"/> after
    /// <c>ToolsRepository.Instance.Read()</c> has loaded all screens and components.
    /// </summary>
    public void BuildFromScreens()
    {
        foreach (var screen in Screens.List.Values)
        {
            var node = Tree.Nodes.Add("[Window] " + screen.Name);
            node.Tag = screen;
            FillNodes(node, screen.Body);
        }
    }

    private static void FillNodes(InterfaceNode parentNode, List<Component> components)
    {
        foreach (var comp in components)
        {
            var childNode = parentNode.Nodes.Add($"[{comp.GetType().Name}] {comp.Name}");
            childNode.Tag = comp;
            FillNodes(childNode, comp.Children);
        }
    }
}
