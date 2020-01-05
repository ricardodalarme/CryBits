using System;
using System.Windows.Forms;

class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    [STAThread]
    public static void Main()
    {
        // Verifica se todos os diretórios existem, se não existirem então criá-los
        Directories.Create();

        // Carrega todos os dados
        Read.Data();

        // Inicializa todos os dispositivos
        Graphics.LoadTextures();
        Audio.Sound.Load();
        Socket.Init();

        // Abre a janela
        Game.OpenMenu();
        Window.Objects.Text = Lists.Options.GameName;
        Window.Objects.Visible = true;

        // Inicia a aplicação
        Loop.Main();
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