using System;
using System.Windows.Forms;

class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    [STAThread]
    static void Main()
    {
        // Inicia o dispositivo de rede
        Socket.Init();

        // Carrega as opções
        Read.Options();

        // Inicia a aplicação
        Login.Objects.Visible = true;
        Application.EnableVisualStyles();
        Loop.Main();
    }

    public static void Close()
    {
        // Fecha a aplicação
        //Graphics.Destruir();
        Application.Exit();
    }
}