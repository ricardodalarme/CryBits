using Editors;
using Library;
using Network;
using System;
using System.Windows.Forms;

namespace Logic
{
    static class Program
    {
        // Usado para manter a aplicação aberta
        public static bool Working = true;

        // Medida de calculo do atraso do jogo
        public static short FPS;

        [STAThread]
        static void Main()
        {
            // Abre a janela de seleção de diretório do cliente caso não houver um
            if (!Directories.Options.Exists && !Directories.Select()) return;

            // Lê os dados
            Read.Options();

            // Inicia o dispositivo de rede
            Socket.Init();

            // Abre a janela
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Login.Form = new Login();
            Login.Form.Show();

            // Inicia o laço
            Loop.Init();
        }

        public static void Close()
        {
            int Wait_Timer = Environment.TickCount;

            // Desconecta da rede
            Socket.Disconnect();

            // Espera até que o jogador seja desconectado
            while (Socket.IsConnected() && Environment.TickCount <= Wait_Timer + 1000)
                Application.DoEvents();

            // Fecha a aplicação
            Application.Exit();
        }
    }
}