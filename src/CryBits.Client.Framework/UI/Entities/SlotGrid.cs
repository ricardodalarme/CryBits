using Iguina;
using Iguina.Defs;
using Iguina.Entities;

namespace CryBits.Client.Framework.UI.Entities;

public class SlotGrid : Entity
{
    private int _columns, _rows, _slotSize, _spacing;

    public int Columns
    {
        get => _columns;
        set { _columns = value; RecalculateSize(); }
    }

    public int Rows
    {
        get => _rows;
        set { _rows = value; RecalculateSize(); }
    }

    public int SlotSize
    {
        get => _slotSize;
        set { _slotSize = value; RecalculateSize(); }
    }

    public int Spacing
    {
        get => _spacing;
        set { _spacing = value; RecalculateSize(); }
    }

    public int TotalSlots => Columns * Rows;
    public int? HoveredSlot { get; private set; }

    public event Action<int>? OnSlotLeftDown;
    public event Action<int>? OnSlotLeftUp;
    public event Action<int>? OnSlotRightClick;
    public event Action<int>? OnSlotDoubleClick;
    public event Action<int>? OnSlotHoverEnter;
    public event Action? OnSlotHoverLeave;

    private long _lastClickTime;

    private void RecalculateSize()
    {
        Size.SetPixels(
            _columns * _slotSize + _spacing * (_columns - 1),
            _rows * _slotSize + _spacing * (_rows - 1));
    }

    public SlotGrid(UISystem system, int columns, int rows, int slotSize, int spacing)
        : base(system, null)
    {
        _columns = columns;
        _rows = rows;
        _slotSize = slotSize;
        _spacing = spacing;

        RecalculateSize();

        Events.OnLeftMousePressed += OnLeftMousePressed;
        Events.OnLeftMouseReleased += OnLeftMouseReleased;
        Events.OnRightMousePressed += OnRightMousePressed;
        Events.AfterUpdate += OnAfterUpdate;
    }

    private void OnLeftMousePressed(Entity _)
    {
        var slot = GetSlotIndex(UISystem.CurrentInputState.MousePosition.X,
                                UISystem.CurrentInputState.MousePosition.Y);
        if (slot == null) return;

        var now = Environment.TickCount64;
        if (now - _lastClickTime < 300)
        {
            OnSlotDoubleClick?.Invoke(slot.Value);
            _lastClickTime = 0;
            return;
        }
        _lastClickTime = now;

        OnSlotLeftDown?.Invoke(slot.Value);
    }

    private void OnLeftMouseReleased(Entity _)
    {
        var slot = GetSlotIndex(UISystem.CurrentInputState.MousePosition.X,
                                UISystem.CurrentInputState.MousePosition.Y);
        if (slot != null)
            OnSlotLeftUp?.Invoke(slot.Value);
    }

    private void OnRightMousePressed(Entity _)
    {
        var slot = GetSlotIndex(UISystem.CurrentInputState.MousePosition.X,
                                UISystem.CurrentInputState.MousePosition.Y);
        if (slot != null)
            OnSlotRightClick?.Invoke(slot.Value);
    }

    private void OnAfterUpdate(Entity _)
    {
        var slot = GetSlotIndex(UISystem.CurrentInputState.MousePosition.X,
                                UISystem.CurrentInputState.MousePosition.Y);

        if (slot == HoveredSlot) return;

        if (HoveredSlot.HasValue)
            OnSlotHoverLeave?.Invoke();
        HoveredSlot = slot;
        if (slot != null)
            OnSlotHoverEnter?.Invoke(slot.Value);
    }

    public void ResetHover()
    {
        if (HoveredSlot.HasValue)
        {
            OnSlotHoverLeave?.Invoke();
            HoveredSlot = null;
        }
    }

    public Rectangle GetSlotRect(int slotIndex)
    {
        var bounds = LastBoundingRect;
        var col = slotIndex % Columns;
        var row = slotIndex / Columns;
        var x = bounds.X + col * (SlotSize + Spacing);
        var y = bounds.Y + row * (SlotSize + Spacing);
        return new Rectangle { X = x, Y = y, Width = SlotSize, Height = SlotSize };
    }

    private int? GetSlotIndex(int mouseX, int mouseY)
    {
        var bounds = LastBoundingRect;
        if (mouseX < bounds.X || mouseX >= bounds.X + bounds.Width ||
            mouseY < bounds.Y || mouseY >= bounds.Y + bounds.Height)
            return null;

        var col = (mouseX - bounds.X) / (SlotSize + Spacing);
        var row = (mouseY - bounds.Y) / (SlotSize + Spacing);

        if (col < 0 || col >= Columns || row < 0 || row >= Rows)
            return null;

        var slotX = bounds.X + col * (SlotSize + Spacing);
        var slotY = bounds.Y + row * (SlotSize + Spacing);
        if (mouseX >= slotX + SlotSize || mouseY >= slotY + SlotSize)
            return null;

        return row * Columns + col;
    }
}
