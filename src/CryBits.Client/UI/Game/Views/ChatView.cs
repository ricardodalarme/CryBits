using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class ChatView(UiContext uiContext, Chat chat) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("ChatPanel");
    internal TextBox MessageTextInput => uiContext.Get<TextBox>("ChatInput");
    private Panel MessagesPanel => uiContext.Get<Panel>("ChatMessagesPanel");
    private Label ChatHint => uiContext.Get<Label>("ChatHint");

    private readonly List<Label> _messageLabels = [];

    public override void Bind()
    {
        MessageTextInput.TouchDown += OnMessagePressed;
    }

    public override void Unbind()
    {
        MessageTextInput.TouchDown -= OnMessagePressed;
    }

    private void OnMessagePressed(object? sender, MyraEventArgs e)
    {
        chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;
        Panel.Visible = true;
    }

    public void UpdateChat()
    {
        var focused = uiContext.Desktop.FocusedKeyboardWidget;
        var chatFocused = focused == MessageTextInput;

        Panel.Visible = chatFocused;
        ChatHint.Visible = !chatFocused;

        while (_messageLabels.Count < chat.Order.Count)
        {
            var i = _messageLabels.Count;
            var line = chat.Order[i];
            var label = new Label
            {
                Text = line.Text,
                Wrap = true,
                Width = 330,
                TextColor = line.Color
            };
            MessagesPanel.Widgets.Add(label);
            _messageLabels.Add(label);
        }

        while (_messageLabels.Count > chat.Order.Count)
        {
            var last = _messageLabels[^1];
            MessagesPanel.Widgets.Remove(last);
            _messageLabels.RemoveAt(_messageLabels.Count - 1);
        }
    }
}
