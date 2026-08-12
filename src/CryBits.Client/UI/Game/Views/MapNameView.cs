using CryBits.Client.Core;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class MapNameView(UiContext uiContext, World world) : ViewBase
{
    private Label MapNameLabel => uiContext.Get<Label>("MapName");

    public override void Bind()
    {
    }

    public override void Unbind()
    {
    }

    public void UpdateMapName()
    {
        var map = world.CurrentMap;
        if (map == null)
        {
            MapNameLabel.Visible = false;
            return;
        }

        MapNameLabel.Visible = true;
        MapNameLabel.Text = map.Name;
        MapNameLabel.TextColor = map.Moral switch
        {
            Moral.Dangerous => Color.Red,
            _ => Color.White
        };
    }
}
