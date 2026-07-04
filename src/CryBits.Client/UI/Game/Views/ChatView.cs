using Iguina.Defs;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class ChatView(UiContext uiContext, Chat chat) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("ChatPanel");
    internal TextInput MessageTextInput => uiContext.Get<TextInput>("ChatInput");
    private Panel MessagesPanel => uiContext.Get<Panel>("ChatMessagesPanel");
    private Paragraph ChatHint => uiContext.Get<Paragraph>("ChatHint");

    private readonly List<Paragraph> _messageParagraphs = [];
    private bool _scrollbarCreated;

    public override void Bind()
    {
        MessageTextInput.Events.OnClick += OnMessagePressed;
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        MessageTextInput.Events.OnClick -= OnMessagePressed;
        uiContext.PostDraw -= OnPostDraw;
    }

    private void OnMessagePressed(Entity _)
    {
        chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;
        Panel.Visible = true;
    }

    private void OnPostDraw()
    {
        var focused = uiContext.UISystem?.FocusedEntity;
        var chatFocused = focused == MessageTextInput;

        Panel.Visible = chatFocused;
        ChatHint.Visible = !chatFocused;

        if (!_scrollbarCreated && MessagesPanel.Parent != null)
        {
            MessagesPanel.OverflowMode = OverflowMode.HideOverflow;
            MessagesPanel.CreateVerticalScrollbar(true);
            _scrollbarCreated = true;
        }

        while (_messageParagraphs.Count < chat.Order.Count)
        {
            var i = _messageParagraphs.Count;
            var line = chat.Order[i];
            var paragraph = new Paragraph(uiContext.UISystem!)
            {
                Text = line.Text,
                TextOverflowMode = TextOverflowMode.WrapWords,
                ShrinkHeightToMinimalSize = true
            };
            paragraph.Size.SetPixels(330, 0);
            paragraph.OverrideStyles.TextFillColor = new Color(
                line.Color.R, line.Color.G, line.Color.B, line.Color.A);
            MessagesPanel.AddChild(paragraph);
            _messageParagraphs.Add(paragraph);

            if (MessagesPanel.VerticalScrollbar != null && i == chat.Order.Count - 1)
                MessagesPanel.VerticalScrollbar.Value = MessagesPanel.VerticalScrollbar.MaxValue;
        }

        while (_messageParagraphs.Count > chat.Order.Count)
        {
            var last = _messageParagraphs[^1];
            last.RemoveSelf();
            _messageParagraphs.RemoveAt(_messageParagraphs.Count - 1);
        }
    }
}
