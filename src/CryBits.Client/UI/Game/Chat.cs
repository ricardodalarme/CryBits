using CryBits.Client.Commands;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Common;
using CryBits.Simulation.Intents;
using Iguina.Entities;
using SFML.Graphics;

namespace CryBits.Client.UI.Game;

internal class Chat
{
    private readonly ChatCommandDispatcher _dispatcher;
    private readonly IntentSender intentSender;
    private readonly UiContext _uiContext;

    public List<Structure> Order = [];
    public long VisibilityTimer;
    public const byte LinesVisible = 9;
    private const byte MaxLines = 50;
    public const short SleepTimer = 10000;

    public Chat(IntentSender intentSender, UiContext uiContext)
    {
        this.intentSender = intentSender;
        _uiContext = uiContext;
        _dispatcher = new ChatCommandDispatcher(AddText)
            .Register(new PartyInviteCommand(intentSender, AddText))
            .Register(new PartyLeaveCommand(intentSender))
            .Register(new TradeInviteCommand(intentSender, AddText));
    }

    public class Structure
    {
        public string Text = string.Empty;
        public Color Color;
    }

    private void AddLine(string text, Color color)
    {
        Order.Add(new Structure { Text = text, Color = color });
        if (Order.Count > MaxLines) Order.RemoveAt(0);
        VisibilityTimer = Environment.TickCount64 + 10000;
    }

    public void AddText(string message, Color color)
    {
        AddLine(message.Trim(), color);
    }

    public void Type()
    {
        if (!_uiContext.TryGet<Panel>("ChatPanel", out var panel) ||
            !_uiContext.TryGet<TextInput>("ChatInput", out var input))
            return;

        panel.Visible = !panel.Visible;

        if (panel.Visible)
        {
            VisibilityTimer = Environment.TickCount64 + SleepTimer;
            _uiContext.UISystem.FocusedEntity = input;
            return;
        }

        _uiContext.UISystem.FocusedEntity = null;

        var message = input.Value;

        if (message.Length < 3)
        {
            input.Value = string.Empty;
            return;
        }

        input.Value = string.Empty;

        if (!_dispatcher.TryDispatch(message))
            SendMessage(message);
    }

    private void SendMessage(string message)
    {
        switch (message[0])
        {
            case '\'':
                intentSender.Send(new ChatMessageIntent(default, message[1..], Message.Global, null));
                return;
            case '!':
                var parts = message.Split(' ');
                if (parts.GetUpperBound(0) < 1)
                {
                    AddText("Use: '!' + Addressee + ' Message'", Color.White);
                    return;
                }
                var addressee = message.Substring(1, parts[0].Length - 1);
                var content = message.Substring(parts[0].Length + 1);
                intentSender.Send(new ChatMessageIntent(default, content, Message.Private, addressee));
                return;
            default:
                intentSender.Send(new ChatMessageIntent(default, message, Message.Map, null));
                break;
        }
    }
}
