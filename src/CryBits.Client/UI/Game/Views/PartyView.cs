using CryBits.Client.Framework.Assets;
using CryBits.Client.Rendering;
using CryBits.Client.UI.Game.ViewModels;

namespace CryBits.Client.UI.Game.Views;

internal class PartyView(UiContext uiContext, SpriteBatch spriteBatch, PartyViewModel viewModel) : ViewBase
{
    public override void Bind()
    {
        Track(
            () => uiContext.PostDraw += OnPostDraw,
            () => uiContext.PostDraw -= OnPostDraw
        );
    }

    private void OnPostDraw()
    {
        viewModel.Refresh();

        for (byte i = 0; i < viewModel.Members.Count; i++)
        {
            var member = viewModel.Members[i];
            spriteBatch.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 0, 82, 8);
            spriteBatch.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 0, 82, 8);

            if (member.Hp > 0)
                spriteBatch.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 8,
                    member.Hp * 82 / member.MaxHp, 8);
            if (member.Mp > 0)
                spriteBatch.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 16,
                    member.Mp * 82 / member.MaxMp, 8);

            spriteBatch.DrawText(member.Name, 10, 79 + 27 * i, SFML.Graphics.Color.White);
        }
    }
}
