using System;
using System.Threading;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms;
using CryBits.Editors.Media.Audio;
using CryBits.Editors.Media.Graphics;
using CryBits.Editors.Network;

namespace CryBits.Editors.Logic;

internal static class Loop
{
    public static void Init()
    {
        int count;
        var timer1000 = 0;
        short fps = 0;

        while (Program.Working)
        {
            count = Environment.TickCount;

            // Manuseia os dados recebidos
            Socket.HandleData();

            // Eventos
            TempMap.UpdateFog();
            TempMap.UpdateWeather();
            MapsMusic();

            // Desenha os gráficos
            Renders.Present();

            // Faz com que a aplicação se mantenha estável
            Application.DoEvents();
            while (Environment.TickCount < count + 10) Thread.Sleep(1);

            // FPS
            if (timer1000 < Environment.TickCount)
            {
                // Cálcula o FPS
                Program.FPS = fps;
                fps = 0;

                // Reinicia a contagem
                timer1000 = Environment.TickCount + 1000;
            }
            else
                fps++;
        }

        // Fecha a aplicação
        Program.Close();
    }

    private static void MapsMusic()
    {
        // Apenas se necessário
        if (EditorMaps.Form?.Visible != true) goto stop;
        if (!EditorMaps.Form.butAudio.Checked) goto stop;
        if (!EditorMaps.Form.butVisualization.Checked) goto stop;
        if (EditorMaps.Form.Selected.Music == 0) goto stop;

        // Inicia a música
        if (Music.Device == null || Music.Current != (Enums.Music)EditorMaps.Form.Selected.Music)
            Music.Play((Enums.Music)EditorMaps.Form.Selected.Music);
        return;
        stop:
        // Para a música
        if (Music.Device != null) Music.Stop();
    }
}