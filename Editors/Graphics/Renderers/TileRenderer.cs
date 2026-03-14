using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using SFML.Graphics;
using System.Drawing;
using static CryBits.Editors.Logic.Utils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Editors.Graphics.Renderers;

internal class TileRenderer(Renderer renderer)
{
    public static TileRenderer Instance { get; } = new(Renderer.Instance);

    /// <summary>
    /// Render targets used by the editor windows.
    /// </summary>
    public RenderTexture? WinTile;

    /// <summary>
    /// Render the Tile editor preview.
    /// </summary>
    public void Tile(int textureNum, int scrollX, int scrollY, bool modeAttributes)
    {
        if (WinTile == null || Textures.Tiles.Count == 0) return;
        if (textureNum < 0 || textureNum >= Textures.Tiles.Count) return;

        WinTile.Clear();
        Transparent(WinTile);

        var texture = Textures.Tiles[textureNum];
        var position = new Point(scrollX * Grid, scrollY * Grid);
        renderer.Draw(WinTile, texture, new Rectangle(position, texture.ToSize()),
            new Rectangle(new Point(0), texture.ToSize()));

        for (byte x = 0; x <= 298 / Grid; x++)
            for (byte y = 0; y <= 443 / Grid; y++)
            {
                if (modeAttributes)
                    TileAttributes(textureNum, scrollX, scrollY, x, y);
                else
                    TileDirBlock(textureNum, scrollX, scrollY, x, y);

                renderer.DrawRectangle(WinTile, x * Grid, y * Grid, Grid, Grid, new Color(25, 25, 25, 70));
            }

        WinTile.Display();
    }

    private void TileAttributes(int textureNum, int scrollX, int scrollY, byte x, byte y)
    {
        var tile = new Point(scrollX + x, scrollY + y);
        var point = new Point(x * Grid + Grid / 2 - 5, y * Grid + Grid / 2 - 6);
        if (tile.X > Client.Framework.Entities.Tile.Tile.List[textureNum].Data.GetUpperBound(0)) return;
        if (tile.Y > Client.Framework.Entities.Tile.Tile.List[textureNum].Data.GetUpperBound(1)) return;

        switch ((TileAttribute)Client.Framework.Entities.Tile.Tile.List[textureNum].Data[tile.X, tile.Y].Attribute)
        {
            case TileAttribute.Block:
                renderer.Draw(WinTile, Textures.Blank, x * Grid, y * Grid, 0, 0, Grid, Grid, new Color(225, 0, 0, 75));
                renderer.DrawText(WinTile, "B", point.X, point.Y, Color.Red);
                break;
        }
    }

    private void TileDirBlock(int textureNum, int scrollX, int scrollY, byte x, byte y)
    {
        var tile = new Point(scrollX + x, scrollY + y);
        if (tile.X > Client.Framework.Entities.Tile.Tile.List[textureNum].Data.GetUpperBound(0)) return;
        if (tile.Y > Client.Framework.Entities.Tile.Tile.List[textureNum].Data.GetUpperBound(1)) return;

        if (Client.Framework.Entities.Tile.Tile.List[textureNum].Data[x, y].Attribute == (byte)TileAttribute.Block)
        {
            TileAttributes(textureNum, scrollX, scrollY, x, y);
            return;
        }

        for (byte i = 0; i < (byte)Direction.Count; i++)
        {
            var sourceY = Client.Framework.Entities.Tile.Tile.List[textureNum].Data[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;
            renderer.Draw(WinTile, Textures.Directions, x * Grid + Block_Position(i).X, y * Grid + Block_Position(i).Y,
                i * 8, sourceY, 6, 6);
        }
    }

    private void Transparent(RenderTexture target)
    {
        var textureSize = Textures.Transparent.ToSize();
        for (var x = 0; x <= target.Size.X / textureSize.Width; x++)
            for (var y = 0; y <= target.Size.Y / textureSize.Height; y++)
                renderer.Draw(target, Textures.Transparent, new Point(textureSize.Width * x, textureSize.Height * y));
    }
}
