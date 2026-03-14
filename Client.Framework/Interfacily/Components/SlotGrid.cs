using CryBits.Client.Framework.Interfacily.Interfaces;
using SFML.Window;
using System.Drawing;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Framework.Interfacily.Components;

/// <summary>
/// A layout-only component that encodes a uniform grid of slots.
/// <para>
/// The renderer calls <see cref="GetSlotPosition"/> with a zero-based index and
/// draws whatever game data belongs there — completely data-blind.
/// </para>
/// </summary>
public class SlotGrid : Component, IMouseDown, IMouseUp, IMouseDoubleClick, IMouseMoved
{
    /// <summary>Number of columns in the grid.</summary>
    public byte Columns { get; set; } = 1;

    /// <summary>Pixel size of each square slot cell.</summary>
    public byte SlotSize { get; set; } = 32;

    /// <summary>Gap between cells in pixels.</summary>
    public byte Padding { get; set; } = 4;

    /// <summary>Number of rows in the grid.</summary>
    public byte Rows { get; set; } = 1;

    /// <summary>Total number of slots this grid can hold (Rows × Columns).</summary>
    public int SlotCount => Rows * Columns;

    /// <summary>Fires when a mouse button is pressed over a valid slot. Args: event, zero-based slot index.</summary>
    public event Action<MouseButtonEventArgs, short>? OnMouseDown;

    /// <summary>Fires when a mouse button is released over a valid slot. Arg: zero-based slot index.</summary>
    public event Action<short>? OnMouseUp;

    /// <summary>Fires on double-click over a valid slot. Args: event, zero-based slot index.</summary>
    public event Action<MouseButtonEventArgs, short>? OnMouseDoubleClick;

    /// <summary>Fires when the hovered slot changes to a new valid slot. Arg: zero-based slot index.</summary>
    public event Action<short>? OnSlotHover;

    /// <summary>Fires when the mouse leaves a previously hovered slot. Arg: zero-based slot index that was left.</summary>
    public event Action<short>? OnSlotLeave;

    /// <summary>Fires each render frame with the component's top-left position so the subscriber can draw content.</summary>
    public event Action<int, Point>? OnRenderSlot;

    private short _hoveredSlot = -1;

    /// <summary>
    /// Iterates every slot and invokes <see cref="OnRenderSlot"/> so subscribers can draw their content.
    /// Called by the renderer during the component render pass.
    /// </summary>
    public void RenderSlots()
    {
        if (!Visible) return;
        for (var i = 0; i < SlotCount; i++)
            OnRenderSlot?.Invoke(i, GetSlotPosition(i));
    }

    /// <summary>
    /// Returns the zero-based index of the slot under the current mouse position,
    /// or -1 if the mouse is outside the grid or the component is not visible.
    /// </summary>
    public short GetSlotIndex()
    {
        if (!Visible) return -1;
        var size = SlotSize + Padding;
        var col = (MyMouse.X - Position.X) / size;
        var row = (MyMouse.Y - Position.Y) / size;
        if (col < 0 || col >= Columns || row < 0 || row >= Rows) return -1;
        var slot = (short)(row * Columns + col);
        var pos = GetSlotPosition(slot);
        if (!IsAbove(new Rectangle(pos.X, pos.Y, SlotSize, SlotSize))) return -1;
        return slot;
    }

    public void MouseDown(MouseButtonEventArgs e)
    {
        var slot = GetSlotIndex();
        if (slot < 0) return;
        OnMouseDown?.Invoke(e, slot);
    }

    public void MouseUp()
    {
        var slot = GetSlotIndex();
        if (slot < 0) return;
        OnMouseUp?.Invoke(slot);
    }

    public void MouseDoubleClick(MouseButtonEventArgs e)
    {
        var slot = GetSlotIndex();
        if (slot < 0) return;
        OnMouseDoubleClick?.Invoke(e, slot);
    }

    /// <summary>
    /// Returns the top-left screen position for the slot at
    /// <paramref name="zeroBasedIndex"/>.
    /// </summary>
    public Point GetSlotPosition(int zeroBasedIndex)
    {
        var row = zeroBasedIndex / Columns;
        var col = zeroBasedIndex % Columns;
        return new Point(
            Position.X + col * (SlotSize + Padding),
            Position.Y + row * (SlotSize + Padding));
    }

    public override string ToString() => "[SlotGrid] " + Name;

    /// <summary>Detects hover changes and fires <see cref="OnSlotHover"/> / <see cref="OnSlotLeave"/>.</summary>
    public void MouseMoved()
    {
        var slot = GetSlotIndex();
        if (slot == _hoveredSlot) return;
        if (_hoveredSlot >= 0) OnSlotLeave?.Invoke(_hoveredSlot);
        _hoveredSlot = slot;
        if (slot >= 0) OnSlotHover?.Invoke(slot);
    }

    /// <summary>Fires <see cref="OnSlotLeave"/> for the current slot and resets hover state.</summary>
    public void ResetHover()
    {
        if (_hoveredSlot < 0) return;
        var prev = _hoveredSlot;
        _hoveredSlot = -1;
        OnSlotLeave?.Invoke(prev);
    }
}
