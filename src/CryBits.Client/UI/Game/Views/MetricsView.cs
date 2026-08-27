using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class MetricsView(UiContext uiContext, Connection connection, Func<short> getFps) : ViewBase
{
    private Label FpsLabel => uiContext.Get<Label>("FpsLabel");
    private Label LatencyLabel => uiContext.Get<Label>("LatencyLabel");

    public override void Bind()
    {
    }

    public override void Unbind()
    {
    }

    public void UpdateMetrics()
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
