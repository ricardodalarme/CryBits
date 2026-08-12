using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class MetricsView(UiContext uiContext, Connection connection, Func<short> getFps) : ViewBase
{
    private Label FpsLabel => uiContext.Get<Label>("FpsLabel");
    private Label LatencyLabel => uiContext.Get<Label>("LatencyLabel");

    public override void Bind()
    {
        Track(
            () => uiContext.PostDraw += OnPostDraw,
            () => uiContext.PostDraw -= OnPostDraw
        );
    }

    private void OnPostDraw()
    {
        if (Options.Instance.ShowMetrics)
        {
            FpsLabel.Text = "FPS: " + getFps();
            LatencyLabel.Text = "Latency: " + connection.Latency;
        }

        FpsLabel.Visible = Options.Instance.ShowMetrics;
        LatencyLabel.Visible = Options.Instance.ShowMetrics;
    }
}
