using CryBits.Client.UI.Game.ViewModels;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class StatsView(UiContext uiContext, StatsViewModel viewModel) : ViewBase
{
    private Label HpValueLabel => uiContext.Get<Label>("HP_Value");
    private Label MpValueLabel => uiContext.Get<Label>("MP_Value");
    private Label ExpValueLabel => uiContext.Get<Label>("EXP_Value");
    private ProgressBar HpBar => uiContext.Get<ProgressBar>("HP_Bar");
    private ProgressBar MpBar => uiContext.Get<ProgressBar>("MP_Bar");
    private ProgressBar ExpBar => uiContext.Get<ProgressBar>("EXP_Bar");

    public override void Bind()
    {
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        uiContext.PostDraw -= OnPostDraw;
    }

    private void OnPostDraw()
    {
        HpBar.ValueSafe = viewModel.HpPercent;
        MpBar.ValueSafe = viewModel.MpPercent;
        ExpBar.ValueSafe = viewModel.ExpPercent;

        HpValueLabel.Text = $"HP: {viewModel.Hp}/{viewModel.MaxHp}";
        MpValueLabel.Text = $"MP: {viewModel.Mp}/{viewModel.MaxMp}";
        ExpValueLabel.Text = $"EXP: {viewModel.Experience}/{viewModel.ExpNeeded}";
    }
}
