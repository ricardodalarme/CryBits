using System.Collections.Generic;

namespace CryBits.Editors.Entities;

/// <summary>
/// Lightweight tree-node that replaces System.Windows.Forms.TreeNode
/// so the Interface editor data model is cross-platform.
/// </summary>
internal sealed class InterfaceNode
{
    public string Text { get; set; } = string.Empty;
    public object? Tag  { get; set; }
    public InterfaceNode? Parent { get; private set; }

    private readonly List<InterfaceNode> _nodes = new();

    public InterfaceNodeCollection Nodes { get; }

    public InterfaceNode? LastNode => _nodes.Count > 0 ? _nodes[^1] : null;

    public InterfaceNode(string text = "")
    {
        Text = text;
        Nodes = new InterfaceNodeCollection(_nodes, this);
    }

    // ── Inner collection ─────────────────────────────────────────────────────

    internal sealed class InterfaceNodeCollection
    {
        private readonly List<InterfaceNode> _list;
        private readonly InterfaceNode _owner;

        internal InterfaceNodeCollection(List<InterfaceNode> list, InterfaceNode owner)
        {
            _list = list;
            _owner = owner;
        }

        public int Count => _list.Count;

        public InterfaceNode this[int index] => _list[index];

        public InterfaceNode Add(string text)
        {
            var node = new InterfaceNode(text) { Parent = _owner };
            _list.Add(node);
            node.Parent = _owner;
            return node;
        }

        public void Add(InterfaceNode node)
        {
            node.Parent = _owner;
            _list.Add(node);
        }

        public void Remove(InterfaceNode node)
        {
            _list.Remove(node);
            node.Parent = null;
        }

        public System.Collections.Generic.IEnumerator<InterfaceNode> GetEnumerator() => _list.GetEnumerator();
    }
}
