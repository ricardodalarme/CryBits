using CryBits.Client.Framework.Assets;
using CryBits.Client.UI.Game.ViewModels;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        for (byte i = 0; i < viewModel.Members.Count; i++)
        {
            var member = viewModel.Members[i];
            var topY = 92 + (27 * i);
            var bottomY = 99 + (27 * i);
            var nameY = 79 + (27 * i);

            spriteBatch.Draw(Textures.PartyBars, new Rectangle(10, topY, 82, 8), Color.White);
            spriteBatch.Draw(Textures.PartyBars, new Rectangle(10, bottomY, 82, 8), Color.White);

            if (member.Hp > 0)
            {
                var fillHp = member.Hp * 82 / member.MaxHp;
                spriteBatch.Draw(Textures.PartyBars,
                    new Rectangle(10, topY, fillHp, 8),
                    new Rectangle(0, 8, fillHp, 8), Color.White);
            }
            if (member.Mp > 0)
            {
                var fillMp = member.Mp * 82 / member.MaxMp;
                spriteBatch.Draw(Textures.PartyBars,
                    new Rectangle(10, bottomY, fillMp, 8),
                    new Rectangle(0, 16, fillMp, 8), Color.White);
            }

            spriteBatch.DrawString(Fonts.Default, member.Name, new Vector2(10, nameY), Color.White);
        }
    }
}
