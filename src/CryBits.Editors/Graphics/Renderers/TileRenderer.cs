using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Maps;
using Direction = CryBits.Definitions.Common.Direction;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SysRect = System.Drawing.Rectangle;
using static CryBits.Definitions.Globals;
using static CryBits.Editors.Logic.Utils;

namespace CryBits.Editors.Graphics.Renderers;

internal class TileRenderer(Renderer renderer)
{
    public RenderTarget2D? WinTile { get; set; }

    /// <summary>Render the Tile editor preview.</summary>
    public void Tile(int textureNum, int scrollX, int scrollY, bool modeAttributes)
    {
        if (Textures.Tiles.Count == 0) return;
        if (textureNum < 0 || textureNum >= Textures.Tiles.Count) return;

        renderer.DrawTransparentBackground();

        if (Textures.Tiles[textureNum] is not { } texture) return;

        var position = new SysRect(scrollX * Grid, scrollY * Grid, texture.Width, texture.Height);
        renderer.Draw(texture, position, new SysRect(0, 0, texture.Width, texture.Height));

        for (var x = 0; x <= 298 / Grid; x++)
            for (var y = 0; y <= 443 / Grid; y++)
            {
                if (modeAttributes)
                    TileAttributes(textureNum, scrollX, scrollY, x, y);
                else
                    TileDirBlock(textureNum, scrollX, scrollY, x, y);

                renderer.DrawRectangle(x * Grid, y * Grid, Grid, Grid, new Color(25, 25, 25, 70));
            }
    }

    private void TileAttributes(int textureNum, int scrollX, int scrollY, int x, int y)
    {
        var tile = new Point(scrollX + x, scrollY + y);
        var point = new Point((x * Grid) + (Grid / 2) - 5, (y * Grid) + (Grid / 2) - 6);
        if (tile.X > Client.Framework.Entities.Tile.Tile.List[textureNum].Data.GetUpperBound(0)) return;
        if (tile.Y > Client.Framework.Entities.Tile.Tile.List[textureNum].Data.GetUpperBound(1)) return;

        switch ((TileAttribute)Client.Framework.Entities.Tile.Tile.List[textureNum].Data[tile.X, tile.Y].Attribute)
        {
            case TileAttribute.Block:
                renderer.Draw(Textures.Blank, x * Grid, y * Grid, 0, 0, Grid, Grid, new Color(225, 0, 0, 75));
                renderer.DrawText((point.X, point.Y), "B", Color.Red);
                break;
        }
    }

    private void TileDirBlock(int textureNum, int scrollX, int scrollY, int x, int y)
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
            var sourceY = Client.Framework.Entities.Tile.Tile.List[textureNum].Data[tile.X, tile.Y].Block[i]
                ? (byte)8
                : (byte)0;
            renderer.Draw(Textures.Directions, (x * Grid) + Block_Position(i).X, (y * Grid) + Block_Position(i).Y,
                i * 8, sourceY, 6, 6);
        }
    }
}
