using System;
using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Editors.Forms;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;
using Read = CryBits.Editors.Library.Read;

namespace CryBits.Editors;

internal static class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    // Medida de calculo do atraso do jogo
    public static short Fps;

    [STAThread]
    private static void Main()
    {
        // Verifica se todos os diretórios existem, se não existirem então criá-os
        Directories.Create();

        // Carrega as preferências
        Client.Framework.Library.Read.Options();
        Read.Tools();

        // Inicializa todos os dispositivos
        Socket.Init();
        Sound.Load();
        AvaloniaDataLauncher.Initialize();

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
        var waitTimer = Environment.TickCount;

        // Desconecta da rede
        Socket.Disconnect();

        // Espera até que o jogador seja desconectado
        while (Socket.IsConnected() && Environment.TickCount <= waitTimer + 1000)
            Application.DoEvents();

        // Fecha a aplicação
        Application.Exit();
    }
}
