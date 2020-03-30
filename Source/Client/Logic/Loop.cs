using System;
using System.Windows.Forms;

class Loop
{
    // Contagens
    public static int TextBox_Timer = 0;
    public static int Chat_Timer = 0;

    public static void Init()
    {
        int Count;
        int Timer_1000 = 0;
        int Timer_30 = 0;
        short FPS = 0;

        while (Program.Working)
        {
            Count = Environment.TickCount;

            // Manuseia os dados recebidos
            Socket.HandleData();

            // Apresenta os gráficos à tela
            Graphics.Present();

            // Processa os eventos da janela
            Graphics.RenderWindow.DispatchEvents();

            // Eventos
            TextBox();

            if (Player.MyIndex > 0 && Tools.CurrentWindow == Tools.Windows.Game)
            {
                Map.Logic();
                if (Timer_30 < Environment.TickCount)
                {
                    // Lógicas
                    Player.Logic();
                    NPC.Logic();

                    // Reinicia a contagem
                    Timer_30 = Environment.TickCount + 30;
                }

                // Verifica se é necessário mostrar o painel de informações
                Tools.CheckInformations();
            }

            // Faz com que a aplicação se mantenha estável
            Application.DoEvents();
            while (Environment.TickCount < Count + 15) System.Threading.Thread.Sleep(1);

            // Cálcula o FPS
            if (Timer_1000 < Environment.TickCount)
            {
                Send.Latency();
                Game.FPS = FPS;
                FPS = 0;
                Timer_1000 = Environment.TickCount + 1000;
            }
            else
                FPS += 1;
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