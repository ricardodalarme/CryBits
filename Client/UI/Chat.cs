using System;
using System.Collections.Generic;
using CryBits.Client.Logic;
using CryBits.Client.Media.Graphics;
using CryBits.Client.Network;
using CryBits.Enums;
using SFML.Graphics;
using static CryBits.Client.Logic.Utils;

namespace CryBits.Client.UI
{
    internal static class Chat
    {
        // Ordem de renderização
        public static List<Structure> Order = new();

        // Chat
        public const byte LinesVisible = 9;
        public static byte LinesFirst;
        private const byte MaxLines = 50;
        public const short SleepTimer = 10000;

        // Estrutura do chat
        public class Structure
        {
            public string Text;
            public Color Color;
        }

        private static void AddLine(string text, Color color)
        {
            Order.Add(new Structure());
            int i = Order.Count - 1;

            // Adiciona a mensagem em uma linha vazia
            Order[i].Text = text;
            Order[i].Color = color;

            // Remove uma linha se necessário
            if (Order.Count > MaxLines) Order.Remove(Order[0]);
            if (i + LinesFirst > LinesVisible + LinesFirst)
                LinesFirst = (byte)(i - LinesVisible);

            // Torna as linhas visíveis
            Loop.ChatTimer = Environment.TickCount + 10000;
        }

        public static void AddText(string message, Color color)
        {
            int messageWidth, boxWidth = Textures.Panels[Panels.List["Chat"].TextureNum].ToSize().Width - 16;
            string tempMessage;

            // Remove os espaços
            message = message.Trim();
            messageWidth = MeasureString(message);

            // Caso couber, adiciona a mensagem normalmente
            if (messageWidth < boxWidth)
                AddLine(message, color);
            else
                for (int i = 0; i <= message.Length; i++)
                {
                    tempMessage = message.Substring(0, i);

                    // Adiciona o texto à caixa
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
            var tool = TextBoxes.List["Chat"];
            var panel = Panels.List["Chat"];

            // Altera a visiblidade da caixa
            panel.Visible = !panel.Visible;

            // Altera o foco do digitalizador
            if (panel.Visible)
            {
                Loop.ChatTimer = Environment.TickCount + SleepTimer;
                TextBoxes.Focused = Tools.Get(tool);
                return;
            }

            TextBoxes.Focused = null;

            // Dados
            string message = tool.Text;

            // Somente se necessário
            if (message.Length < 3)
            {
                tool.Text = string.Empty;
                return;
            }

            // Limpa a caixa de texto
            tool.Text = string.Empty;

            // Separa as mensagens em partes
            string[] parts = message.Split(' ');

            // Comandos
            switch (parts[0].ToLower())
            {
                case "/party":
                    if (parts.Length > 1) Send.PartyInvite(parts[1]);
                    break;
                case "/partyleave":
                    Send.PartyLeave();
                    break;
                case "/trade":
                    if (parts.Length > 1) Send.TradeInvite(parts[1]);
                    break;
                default:
                    // Mensagem lobal
                    if (message.Substring(0, 1) == "'")
                        Send.Message(message.Substring(1), Message.Global);
                    // Mensagem particular
                    else if (message.Substring(0, 1) == "!")
                    {
                        // Previne erros 
                        if (parts.GetUpperBound(0) < 1)
                            AddText("Use: '!' + Addressee + 'Message'", Color.White);
                        else
                        {
                            // Dados
                            string destiny = message.Substring(1, parts[0].Length - 1);
                            message = message.Substring(parts[0].Length + 1);

                            // Envia a mensagem
                            Send.Message(message, Message.Private, destiny);
                        }
                    }
                    // Mensagem mapa
                    else
                        Send.Message(message, Message.Map);
                    break;
            }
        }
    }
}