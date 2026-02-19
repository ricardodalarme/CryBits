using System;
using System.Collections.Generic;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using SFML.Graphics;
using static CryBits.Client.Utils.TextUtils;

namespace CryBits.Client.UI;

internal static class Chat
{
    // Rendering order for chat lines
    public static List<Structure> Order = [];

    public const byte LinesVisible = 9;
    public static byte LinesFirst;
    private const byte MaxLines = 50;
    public const short SleepTimer = 10000;

    /// <summary>Chat line record containing the displayed text and color.</summary>
    public class Structure
    {
        public string Text;
        public Color Color;
    }

    private static void AddLine(string text, Color color)
    {
        Order.Add(new Structure());
        var i = Order.Count - 1;

        Order[i].Text = text;
        Order[i].Color = color;

        if (Order.Count > MaxLines) Order.Remove(Order[0]);
        if (i + LinesFirst > LinesVisible + LinesFirst)
            LinesFirst = (byte)(i - LinesVisible);

        // Reset chat visibility timer
        Loop.ChatTimer = Environment.TickCount + 10000;
    }

    public static void AddText(string message, Color color)
    {
        var boxWidth = Textures.Panels[Panels.Chat.TextureNum].ToSize().Width - 16;

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

    public static void Type()
    {
        var tool = TextBoxes.Chat;
        var panel = Panels.Chat;

        panel.Visible = !panel.Visible;

        if (panel.Visible)
        {
            Loop.ChatTimer = Environment.TickCount + SleepTimer;
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

        var parts = message.Split(' ');

        switch (parts[0].ToLower())
        {
            case "/party":
                if (parts.Length > 1) PartySender.PartyInvite(parts[1]);
                break;
            case "/partyleave":
                PartySender.PartyLeave();
                break;
            case "/trade":
                if (parts.Length > 1) TradeSender.TradeInvite(parts[1]);
                break;
            default:
                if (message.Substring(0, 1) == "'")
                    ChatSender.Message(message.Substring(1), Message.Global);
                else if (message.Substring(0, 1) == "!")
                {
                    if (parts.GetUpperBound(0) < 1)
                        AddText("Use: '!' + Addressee + 'Message'", Color.White);
                    else
                    {
                        var destiny = message.Substring(1, parts[0].Length - 1);
                        message = message.Substring(parts[0].Length + 1);

                        ChatSender.Message(message, Message.Private, destiny);
                    }
                }
                // Map message
                else
                    ChatSender.Message(message, Message.Map);

                break;
        }
    }
}