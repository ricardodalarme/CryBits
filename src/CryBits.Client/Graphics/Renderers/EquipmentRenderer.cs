using CryBits.Client.Framework.Graphics;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using System.Drawing;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class EquipmentRenderer(Renderer renderer, GameContext context)
{
    public static EquipmentRenderer Instance { get; } = new(Renderer.Instance, GameContext.Instance);

    public void DrawSlot(int slot, Point pos)
    {
        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        var itemId = equipment.Slots[slot];
        if (itemId == Guid.Empty)
            renderer.Draw(Textures.Equipments, pos.X, pos.Y, slot * 32, 0, 32, 32);
        else
        {
            var item = DefinitionCatalog.Instance.Items.Get(itemId);
            if (item != null)
                renderer.Draw(Textures.Items[item.Texture], pos);
        }
    }
}
