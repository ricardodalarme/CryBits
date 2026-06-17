using CryBits.Client.Worlds;
using Iguina.Entities;
using Ent = global::Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game.Views;

internal sealed class ChatView
{
    private readonly GameContext _context;
    private Panel? _chatPanel;
    private TextInput? _messageInput;
    private Label? _chatMessages;

    public static bool IsInputVisible => Instance?._messageInput?.Visible ?? false;
    public static TextInput? ChatInput => Instance?._messageInput;
    private static ChatView? Instance { get; set; }

    public ChatView(GameContext context)
    {
        _context = context;
        Instance = this;
    }

    public void Wire(Dictionary<string, Ent> reg)
    {
        _chatPanel = reg["ChatPanel"] as Panel;
        _chatMessages = reg["ChatMessages"] as Label;
        _messageInput = reg["ChatInput"] as TextInput;
    }

    public void Toggle()
    {
        if (_chatPanel == null || _messageInput == null) return;
        _chatPanel.Visible = !_chatPanel.Visible;
        _messageInput.Visible = _chatPanel.Visible;
        if (_chatPanel.Visible)
        {
            UI.Game.Chat.VisibilityTimer = Environment.TickCount64 + UI.Game.Chat.SleepTimer;
            Refresh();
        }
    }

    public void Refresh()
    {
        var lines = UI.Game.Chat.Order;
        if (lines.Count == 0 || _chatMessages == null) return;
        var first = UI.Game.Chat.LinesFirst;
        var visible = UI.Game.Chat.LinesVisible;
        var text = "";
        for (var i = first; i <= first + visible && i < lines.Count; i++)
            if (i >= 0 && i < lines.Count)
                text += lines[i].Text + "\n";
        _chatMessages.Text = text.TrimEnd('\n');
    }
}
