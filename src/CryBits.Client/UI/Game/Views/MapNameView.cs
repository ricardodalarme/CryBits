using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using Iguina.Entities;
using Color = Iguina.Defs.Color;

namespace CryBits.Client.UI.Game.Views;

internal class MapNameView(UiContext uiContext, GameContext context) : ViewBase
{
    private Label MapNameLabel => uiContext.Get<Label>("MapName");

    public override void Bind()
    {
        Track(
            () => uiContext.PostDraw += OnPostDraw,
            () => uiContext.PostDraw -= OnPostDraw
        );
    }

    private void OnPostDraw()
    {
        var map = context.CurrentMap;
        if (map == null) { MapNameLabel.Visible = false; return; }

        MapNameLabel.Visible = true;
        MapNameLabel.Text = map.Name;

        MapNameLabel.OverrideStyles.TextFillColor = map.Moral switch
        {
            Moral.Dangerous => new Color(255, 0, 0, 255),
            _ => new Color(255, 255, 255, 255)
        };
    }
}
