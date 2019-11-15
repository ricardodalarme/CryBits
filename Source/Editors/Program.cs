using System;
using System.Windows.Forms;

class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    [STAThread]
    static void Main()
    {
        // Carrega as opções
        Read.Options();

        // Inicia a aplicação
        Selection.Objects.Visible = true;
        Application.EnableVisualStyles();
        Loop.Main();
    }

    public static void Close()
    {
        // Fecha a aplicação
        //Gráficos.Destruir();
        Application.Exit();
    }
}