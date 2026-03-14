using CryBits.Client.Commands;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Enums;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using static CryBits.Client.Framework.Utils.TextUtils;

namespace CryBits.Client.UI.Game;

internal class Chat
{
    public static Chat Instance { get; } = new Chat(ChatSender.Instance);

    private readonly ChatCommandDispatcher _dispatcher;

    private readonly ChatSender chatSender;

    // Rendering order for chat lines
    public static List<Structure> Order = [];

    public const byte LinesVisible = 9;
    public static byte LinesFirst;
    private const byte MaxLines = 50;
    public const short SleepTimer = 10000;

    public Chat(ChatSender chatSender)
    {
        this.chatSender = chatSender;
        _dispatcher = new ChatCommandDispatcher(AddText)
            .Register(new PartyInviteCommand(PartySender.Instance, AddText))
            .Register(new PartyLeaveCommand(PartySender.Instance))
            .Register(new TradeInviteCommand(TradeSender.Instance, AddText));
    }

    /// <summary>Chat line record containing the displayed text and color.</summary>
    public class Structure
    {
        public string Text = string.Empty;
        public Color Color;
    }

    private void AddLine(string text, Color color)
    {
        Order.Add(new Structure());
        var i = Order.Count - 1;

        Order[i].Text = text;
        Order[i].Color = color;

        if (Order.Count > MaxLines) Order.Remove(Order[0]);
        if (i + LinesFirst > LinesVisible + LinesFirst)
            LinesFirst = (byte)(i - LinesVisible);

        // Reset chat visibility timer
        GameLoop.ChatTimer = Environment.TickCount64 + 10000;
    }

    public void AddText(string message, Color color)
    {
        var boxWidth = Textures.Panels[ChatView.Panel.TextureNum].ToSize().Width - 16;

        // Trim whitespace and measure
        message = message.Trim();
        int messageWidth = MeasureString(message);

        if (messageWidth < boxWidth)
            AddLine(message, color);
        else
            for (var i = 0; i <= message.Length; i++)
            {
                var tempMessage = message.Substring(0, i);

                if (MeasureString(tempMessage) > boxWidth)
                {
                    AddLine(tempMessage, color);
                    AddText(message.Substring(tempMessage.Length), color);
                    return;
                }
            }
    }

    public void Type()
    {
        var tool = ChatView.MessageTextBox;
        var panel = ChatView.Panel;

        panel.Visible = !panel.Visible;

        if (panel.Visible)
        {
            GameLoop.ChatTimer = Environment.TickCount64 + SleepTimer;
            TextBox.Focused = tool;
            return;
        }

        TextBox.Focused = null;

        var message = tool.Text;

        if (message.Length < 3)
        {
            tool.Text = string.Empty;
            return;
        }

        tool.Text = string.Empty;

        if (!_dispatcher.TryDispatch(message))
            SendMessage(message);
    }

    private void SendMessage(string message)
    {
        switch (message[0])
        {
            case '\'':
                chatSender.Message(message[1..], Message.Global);
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
                chatSender.Message(content, Message.Private, addressee);
                return;
            default:
                // Default: map message
                chatSender.Message(message, Message.Map);
                break;
        }
    }
}
