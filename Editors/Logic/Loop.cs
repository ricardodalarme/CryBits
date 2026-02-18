using System;
using System.Threading;
using System.Windows.Forms;
using CryBits.Client.Framework.Audio;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms;
using CryBits.Editors.Graphics;
using CryBits.Editors.Network;

namespace CryBits.Editors.Logic;

internal static class Loop
{
    public static void Init()
    {
        var timer1000 = 0;
        short fps = 0;

        while (Program.Working)
        {
            var count = Environment.TickCount;

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
                Program.Fps = fps;
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
        if (EditorMaps.Form == null) return;
        if (EditorMaps.Form?.Selected == null) return;
        if (EditorMaps.Form?.Visible != true) Music.Stop();
        if (!EditorMaps.Form.butAudio.Checked) Music.Stop();
        if (!EditorMaps.Form.butVisualization.Checked) Music.Stop();
        if (string.IsNullOrEmpty(EditorMaps.Form?.Selected.Music)) Music.Stop();

        // Inicia a música-
        if (Music.Device == null || Music.Current != EditorMaps.Form?.Selected.Music)
            Music.Play(EditorMaps.Form.Selected.Music);
    }
}