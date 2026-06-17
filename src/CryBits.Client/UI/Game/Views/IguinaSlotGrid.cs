using CryBits.Client.Managers;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game.Views;

internal sealed class IguinaSlotGrid
{
    public Panel Container { get; }
    public int Columns { get; }
    public int Rows { get; }
    public int SlotSize { get; }
    public int Padding { get; }
    public int TotalSlots => Columns * Rows;

    private readonly Panel[] _slots;
    private const int DoubleClickIntervalMs = 142;
    private long _lastClickTime;
    private int _lastClickSlot = -1;

    public event Action<int>? SlotRender;
    public event Action<int>? SlotLeftClick;
    public event Action<int>? SlotRightClick;
    public event Action<int>? SlotDoubleClick;
    public event Action<int>? SlotHover;
    public event Action<int>? SlotLeave;

    public IguinaSlotGrid(UISystem ui, int columns, int rows, int slotSize, int padding, int startX, int startY, Panel parent)
    {
        Columns = columns;
        Rows = rows;
        SlotSize = slotSize;
        Padding = padding;
        Container = new Panel(ui);
        Container.Anchor = Anchor.TopLeft;
        Container.Offset.SetPixels(startX, startY);
        parent.AddChild(Container);

        _slots = new Panel[TotalSlots];

        for (var i = 0; i < TotalSlots; i++)
        {
            var col = i % columns;
            var row = i / columns;
            var x = col * (slotSize + padding);
            var y = row * (slotSize + padding);
            var slotIndex = i;

            var slot = new Panel(ui);
            slot.Size.SetPixels(slotSize, slotSize);
            slot.Anchor = Anchor.TopLeft;
            slot.Offset.SetPixels(x, y);
            slot.OverrideStyles.BackgroundColor = new Color { R = 0, G = 0, B = 0, A = 0 };

            slot.Events.AfterDraw += _ => SlotRender?.Invoke(slotIndex);
            slot.Events.OnLeftMousePressed += _ => OnSlotLeftMousePressed(slotIndex);
            slot.Events.OnRightMousePressed += _ => SlotRightClick?.Invoke(slotIndex);
            slot.Events.WhileMouseHover += _ =>
            {
                SlotHover?.Invoke(slotIndex);
                _lastHovered = slotIndex;
            };

            Container.AddChild(slot);
            _slots[i] = slot;
        }

        Container.Events.AfterDraw += OnContainerAfterDraw;
    }

    private int _lastHovered = -1;

    private void OnSlotLeftMousePressed(int slotIndex)
    {
        SlotLeftClick?.Invoke(slotIndex);

        var now = Environment.TickCount64;
        if (slotIndex == _lastClickSlot && now - _lastClickTime <= DoubleClickIntervalMs)
        {
            SlotDoubleClick?.Invoke(slotIndex);
            _lastClickSlot = -1;
            _lastClickTime = 0;
        }
        else
        {
            _lastClickSlot = slotIndex;
            _lastClickTime = now;
        }
    }

    private void OnContainerAfterDraw(Ent _)
    {
        var mousePos = InputManager.Instance.MousePosition;
        var hovering = false;

        for (var i = 0; i < _slots.Length; i++)
        {
            if (!_slots[i].Visible) continue;
            var rect = GetSlotRect(i);
            if (mousePos.X >= rect.X && mousePos.X <= rect.X + SlotSize &&
                mousePos.Y >= rect.Y && mousePos.Y <= rect.Y + SlotSize)
            {
                hovering = true;
                break;
            }
        }

        if (!hovering && _lastHovered >= 0)
        {
            SlotLeave?.Invoke(_lastHovered);
            _lastHovered = -1;
        }
    }

    public void SetSlotVisible(int index, bool visible)
    {
        if (index >= 0 && index < _slots.Length)
            _slots[index].Visible = visible;
    }

    public void ClearSlots()
    {
        for (var i = 0; i < _slots.Length; i++)
            _slots[i].Visible = false;
    }

    public Rectangle GetSlotRect(int index)
    {
        if (index >= 0 && index < _slots.Length)
            return _slots[index].LastVisibleBoundingRect;
        return new Rectangle();
    }
}
