using System;
using System.Threading;
using CryBits.Client.Framework.Audio;
using CryBits.Editors.AvaloniaUI.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Graphics;
using CryBits.Editors.Network;

namespace CryBits.Editors.Logic;

internal static class Loop
{
    /// <summary>
    /// Start the editor main loop: process incoming data, update state and present render targets.
    /// </summary>
    public static void Init()
    {
        var timer1000 = 0;
        short fps = 0;

        while (Program.Working)
        {
            var count = Environment.TickCount;

            Socket.HandleData();

            TempMap.UpdateFog();
            TempMap.UpdateWeather();
            MapsMusic();

            Renders.Present();

            // Throttle loop to ~10ms per iteration.
            while (Environment.TickCount < count + 10) Thread.Sleep(1);

            if (timer1000 < Environment.TickCount)
            {
                Program.Fps = fps;
                fps = 0;
                timer1000 = Environment.TickCount + 1000;
            }
            else
            {
                fps++;
            }
        }

        Program.Close();
    }

    private static void MapsMusic()
    {
        // Return early when the selected map is unavailable or audio is disabled.
        var win = EditorMapsWindow.Instance;
        if (win == null) return;
        if (win.SelectedMap == null) return;
        if (!win.IsOpen) { Music.Stop(); return; }
        if (!win.ShowAudioSafe) { Music.Stop(); return; }
        if (!win.ShowVisualizationSafe) { Music.Stop(); return; }
        if (string.IsNullOrEmpty(win.SelectedMap?.Music)) { Music.Stop(); return; }

        // Start the map music if not already playing.
        if (Music.Device == null || Music.Current != win.SelectedMap?.Music)
            Music.Play(win.SelectedMap!.Music);
    }
}
