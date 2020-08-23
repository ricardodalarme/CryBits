using Logic;
using Network;
using SFML.Graphics;
using System.Collections.Generic;
using static Logic.Utils;

namespace Interface
{
    class Chat
    {
        // Ordem de renderização
        public static List<Structure> Order = new List<Structure>();

        // Chat
        public const byte Lines_Visible = 9;
        public static byte Lines_First;
        private const byte Max_Lines = 50;
        public const short Sleep_Timer = 10000;

        // Estrutura do chat
        public class Structure
        {
            public string Text;
            public Color Color;
        }

        private static void AddLine(string Text, Color Color)
        {
            Order.Add(new Structure());
            int i = Order.Count - 1;

            // Adiciona a mensagem em uma linha vazia
            Order[i].Text = Text;
            Order[i].Color = Color;

            // Remove uma linha se necessário
            if (Order.Count > Max_Lines) Order.Remove(Order[0]);
            if (i + Lines_First > Lines_Visible + Lines_First)
                Lines_First = (byte)(i - Lines_Visible);

            // Torna as linhas visíveis
            Loop.Chat_Timer = System.Environment.TickCount + 10000; ;
        }

        public static void AddText(string Message, Color Color)
        {
            int Message_Width, Box_Width = Graphics.TSize(Graphics.Tex_Panel[Panels.List["Chat"].Texture_Num]).Width - 16;
            string Temp_Message;

            // Remove os espaços
            Message = Message.Trim();
            Message_Width = MeasureString(Message);

            // Caso couber, adiciona a mensagem normalmente
            if (Message_Width < Box_Width)
                AddLine(Message, Color);
            else
                for (int i = 0; i <= Message.Length; i++)
                {
                    Temp_Message = Message.Substring(0, i);

                    // Adiciona o texto à caixa
                    if (MeasureString(Temp_Message) > Box_Width)
                    {
                        AddLine(Temp_Message, Color);
                        AddText(Message.Substring(Temp_Message.Length), Color);
                        return;
                    }
                }
        }

        public static void Type()
        {
            var Tool = TextBoxes.List["Chat"];
            var Panel = Panels.List["Chat"];

            // Altera a visiblidade da caixa
            Panel.Visible = !Panel.Visible;

            // Altera o foco do digitalizador
            if (Panel.Visible)
            {
                Loop.Chat_Timer = System.Environment.TickCount + Sleep_Timer;
                TextBoxes.Focused = Tools.Get(Tool);
                return;
            }
            else
                TextBoxes.Focused = null;

            // Dados
            string Message = Tool.Text;

            // Somente se necessário
            if (Message.Length < 3)
            {
                Tool.Text = string.Empty;
                return;
            }

            // Limpa a caixa de texto
            Tool.Text = string.Empty;

            // Separa as mensagens em partes
            string[] Parts = Message.Split(' ');

            // Comandos
            switch (Parts[0].ToLower())
            {
                case "/party":
                    if (Parts.Length > 1) Send.Party_Invite(Parts[1]);
                    break;
                case "/partyleave":
                    Send.Party_Leave();
                    break;
                case "/trade":
                    if (Parts.Length > 1) Send.Trade_Invite(Parts[1]);
                    break;
                default:
                    // Mensagem lobal
                    if (Message.Substring(0, 1) == "'")
                        Send.Message(Message.Substring(1), Messages.Global);
                    // Mensagem particular
                    else if (Message.Substring(0, 1) == "!")
                    {
                        // Previne erros 
                        if (Parts.GetUpperBound(0) < 1)
                            AddText("Use: '!' + Addressee + 'Message'", Color.White);
                        else
                        {
                            // Dados
                            string Destiny = Message.Substring(1, Parts[0].Length - 1);
                            Message = Message.Substring(Parts[0].Length + 1);

                            // Envia a mensagem
                            Send.Message(Message, Messages.Private, Destiny);
                        }
                    }
                    // Mensagem mapa
                    else
                        Send.Message(Message, Messages.Map);
                    break;
            }
        }
    }
}