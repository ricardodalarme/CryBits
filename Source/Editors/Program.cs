using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;
using System;
using System.Windows.Forms;

static class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    // Medida de calculo do atraso do jogo
    public static short FPS;

    [STAThread]
    static void Main()
    {
        // Verifica se todos os diretórios existem, se não existirem então criá-os
        Directories.Create();

        // Carrega as preferências
        Read.Options();

        // Inicializa todos os dispositivos
        Socket.Init();
        Audio.Sound.Load();
        Graphics.Init();

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