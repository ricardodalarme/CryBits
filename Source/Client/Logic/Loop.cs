using System;
using System.Threading;
using System.Windows.Forms;
using CryBits.Client.Entities;
using CryBits.Client.Media;
using CryBits.Client.Network;
using CryBits.Client.UI;

namespace CryBits.Client.Logic
{
    internal static class Loop
    {
        // Contagens
        public static int TextBox_Timer;
        public static int Chat_Timer = 0;

        public static void Init()
        {
            int timer1000 = 0;
            int timer30 = 0;
            short fps = 0;

            while (Program.Working)
            {
                // Manuseia os dados recebidos
                Socket.HandleData();

                // Apresenta os gráficos à tela
                Graphics.Present();

                // Processa os eventos da janela
                Graphics.RenderWindow.DispatchEvents();

                // Eventos
                TextBox();

                if (Windows.Current == WindowsTypes.Game)
                {
                    Mapper.Logic();
                    if (timer30 < Environment.TickCount)
                    {
                        // Lógica dos jogadores
                        for (byte i = 0; i < Player.List.Count; i++)
                            Player.List[i].Logic();

                        // Lógica dos NPCBehaviour
                        for (byte i = 0; i < Mapper.Current.NPC.Length; i++)
                            if (Mapper.Current.NPC[i].Data != null)
                                Mapper.Current.NPC[i].Logic();

                        // Reinicia a contagem
                        timer30 = Environment.TickCount + 30;
                    }

                    // Verifica se é necessário mostrar o painel de informações
                    Panels.CheckInformation();
                }

                // Faz com que a aplicação se mantenha estável
                Application.DoEvents();
                Thread.Sleep(1);

                // Cálcula o FPS
                if (timer1000 < Environment.TickCount)
                {
                    Send.Latency();
                    Game.FPS = fps;
                    fps = 0;
                    timer1000 = Environment.TickCount + 1000;
                }
                else
                    fps += 1;
            }

            // Fecha o jogo
            Program.Close();
        }

        private static void TextBox()
        {
            // Contagem para a renderização da referência do último texto
            if (TextBox_Timer < Environment.TickCount)
            {
                TextBox_Timer = Environment.TickCount + 500;
                TextBoxes.Signal = !TextBoxes.Signal;

                // Se necessário foca o digitalizador de novo
                TextBoxes.Focus();
            }
        }
    }
}