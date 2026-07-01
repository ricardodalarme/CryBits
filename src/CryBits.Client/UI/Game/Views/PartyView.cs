using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Client.Worlds;
using CryBits.Simulation.Components;

namespace CryBits.Client.UI.Game.Views;

internal class PartyView(IguinaContext uiContext, GameContext context) : ViewBase
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
        if (context.LocalPlayer.Entity == null) return;

        var world = context.World;
        var party = context.LocalPlayer.GetParty();
        if (party == null) return;

        for (byte i = 0; i < party.Members.Count; i++)
        {
            var entity = context.GetNetworkEntity(party.Members[i].Value);
            Renderer.Instance.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 0, 82, 8);
            Renderer.Instance.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 0, 82, 8);
            if (entity != null)
            {
                var vitals = world.Get<Vitals>(entity.Value);
                if (vitals != null)
                {
                    if (vitals.Hp > 0)
                        Renderer.Instance.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 8,
                            vitals.Hp * 82 / vitals.MaxHp, 8);
                    if (vitals.Mp > 0)
                        Renderer.Instance.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 16,
                            vitals.Mp * 82 / vitals.MaxMp, 8);
                }
            }
            var name = entity != null
                ? world.Get<PlayerAppearance>(entity.Value)?.Name ?? string.Empty
                : string.Empty;
            Renderer.Instance.DrawText(name, 10, 79 + 27 * i, SFML.Graphics.Color.White);
        }
    }
}
