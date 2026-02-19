using System.Drawing;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Editors.Entities;
using CryBits.Editors.AvaloniaUI.Forms;
using CryBits.Entities.Map;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using static CryBits.Globals;
using static CryBits.Editors.Logic.Utils;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Color = SFML.Graphics.Color;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using RenderTarget = SFML.Graphics.IRenderTarget;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.Graphics;

internal static class Renders
{
    /// <summary>
    /// Render targets used by the editor windows.
    /// </summary>
    public static RenderTexture WinInterfaceRT;

    public static RenderTexture WinTileRT;
    public static RenderTexture? WinMapRT;
    public static RenderTexture? WinMapTileRT;
    public static RenderTexture WinNpcRT;

    #region Engine

    private static void Render(RenderTarget window, Texture texture, Rectangle source, Rectangle destiny,
        object color = null, object mode = null)
    {
        var tmpImage = new Sprite(texture)
        {
            TextureRect = new IntRect(new Vector2i(source.X, source.Y), new Vector2i(source.Width, source.Height)),
            Position = new Vector2f(destiny.X, destiny.Y),
            Scale = new Vector2f(destiny.Width / (float)source.Width, destiny.Height / (float)source.Height)
        };
        if (color != null) tmpImage.Color = (Color)color;

        if (mode == null) mode = RenderStates.Default;
        window.Draw(tmpImage, (RenderStates)mode);
    }

    private static void Render(RenderTarget window, Texture texture, int x, int y, int sourceX, int sourceY,
        int sourceWidth, int sourceHeight, object color = null, object mode = null)
    {
        var source = new Rectangle(new Point(sourceX, sourceY), new Size(sourceWidth, sourceHeight));
        var destiny = new Rectangle(new Point(x, y), new Size(sourceWidth, sourceHeight));

        Render(window, texture, source, destiny, color, mode);
    }

    private static void Render(RenderTarget window, Texture texture, Rectangle destiny, object color = null,
        object mode = null)
    {
        var source = new Rectangle(new Point(0), texture.ToSize());
        Render(window, texture, source, destiny, color, mode);
    }

    private static void Render(RenderTarget window, Texture texture, Point point, object color = null,
        object mode = null)
    {
        var source = new Rectangle(new Point(0), texture.ToSize());
        var destiny = new Rectangle(point, texture.ToSize());

        Render(window, texture, source, destiny, color, mode);
    }

    private static void RenderRectangle(RenderTarget window, Rectangle rectangle, object color = null)
    {
        Render(window, Textures.Grid, rectangle.X, rectangle.Y, 0, 0, rectangle.Width, 1, color);
        Render(window, Textures.Grid, rectangle.X, rectangle.Y, 0, 0, 1, rectangle.Height, color);
        Render(window, Textures.Grid, rectangle.X, rectangle.Y + rectangle.Height - 1, 0, 0, rectangle.Width, 1, color);
        Render(window, Textures.Grid, rectangle.X + rectangle.Width - 1, rectangle.Y, 0, 0, 1, rectangle.Height, color);
    }

    private static void RenderRectangle(RenderTarget window, int x, int y, int width, int height, object color = null)
    {
        RenderRectangle(window, new Rectangle(x, y, width, height), color);
    }

    private static void Render_Box(RenderTarget window, Texture texture, byte margin, Point position, Size size)
    {
        var textureWidth = texture.ToSize().Width;
        var textureHeight = texture.ToSize().Height;

        Render(window, texture, new Rectangle(new Point(0), new Size(margin, textureWidth)),
            new Rectangle(position, new Size(margin, textureHeight)));
        Render(window, texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
        Render(window, texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + margin, position.Y),
                new Size(size.Width - margin * 2, textureHeight)));
    }

    private static void DrawText(RenderTarget window, string text, int x, int y, Color color)
    {
        var tempText = new Text(Fonts.Default, text);

        tempText.CharacterSize = 10;
        tempText.FillColor = color;
        tempText.Position = new Vector2f(x, y);
        tempText.OutlineColor = new Color(0, 0, 0, 70);
        tempText.OutlineThickness = 1;

        window.Draw(tempText);
    }

    #endregion

    /// <summary>
    /// No-op presentation hook; editors render on Avalonia timers.
    /// </summary>
    public static void Present()
    {
    }

    private static void Transparent(RenderTarget window)
    {
        var textureSize = Textures.Transparent.ToSize();

        for (var x = 0; x <= window.Size.X / textureSize.Width; x++)
        for (var y = 0; y <= window.Size.Y / textureSize.Height; y++)
            Render(window, Textures.Transparent, new Point(textureSize.Width * x, textureSize.Height * y));
    }

    #region Map Editor

    /// <summary>
    /// Render the tile-sheet preview for the Maps editor.
    /// </summary>
    public static void EditorMapsTileRT()
    {
        var win = EditorMapsWindow.Instance;
        if (WinMapTileRT == null || win == null || !win.ModeNormal) return;

        WinMapTileRT.Clear(Color.Black);

        var texture = Textures.Tiles[win.TileSheetIndex + 1];
        var position = new Point(win.TileScrollX, win.TileScrollY);

        Transparent(WinMapTileRT);
        Render(WinMapTileRT, texture, new Rectangle(position, texture.ToSize()),
            new Rectangle(new Point(0), texture.ToSize()));
        RenderRectangle(WinMapTileRT,
            new Rectangle(new Point(win.TileSource.X - position.X, win.TileSource.Y - position.Y), win.TileSource.Size),
            new Color(165, 42, 42, 250));
        RenderRectangle(WinMapTileRT, win.TileMouse.X, win.TileMouse.Y, Grid, Grid, new Color(65, 105, 225, 250));

        WinMapTileRT.Display();
    }

    /// <summary>
    /// Render the map view for the Maps editor.
    /// </summary>
    public static void EditorMapsMapRT()
    {
        var win = EditorMapsWindow.Instance;
        if (WinMapRT == null || win == null) return;

        var selected = win.SelectedMap;
        if (selected == null) return;

        WinMapRT.Clear(Color.Black);

        EditorMapsMapPanorama(selected);
        EditorMapsMapTiles(selected);
        EditorMapsMapWeather(selected);
        EditorMapsMapFog(selected);
        EditorMapsMapGrids(selected);
        EditorMapsMapNpcs(selected);

        WinMapRT.Display();
    }

    private static void EditorMapsMapPanorama(Map map)
    {
        var win = EditorMapsWindow.Instance!;

        if (win.ShowVisualizationSafe && map.Panorama > 0)
        {
            var destiny = new Rectangle
            {
                X = win.MapScrollX * -win.GridZoom,
                Y = win.MapScrollY * -win.GridZoom,
                Size = Textures.Panoramas[map.Panorama].ToSize()
            };
            Render(WinMapRT!, Textures.Panoramas[map.Panorama], win.ZoomRect(destiny));
        }
    }

    private static void EditorMapsMapTiles(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        int beginX = win.MapScrollX, beginY = win.MapScrollY;

        for (byte c = 0; c < map.Layer.Count; c++)
        {
            if (!win.IsLayerVisible(c)) continue;

            var color = new Color();
            if (win.ShowEdition && win.ModeNormal)
            {
                if (win.SelectedLayerIndex() >= 0 && c != win.SelectedLayerIndex())
                    color = new Color(255, 255, 255, 150);
            }
            else
                color = new Color(map.Color.R, map.Color.G, map.Color.B);

            for (var x = beginX; x < Map.Width; x++)
            for (var y = beginY; y < Map.Height; y++)
                if (map.Layer[c].Tile[x, y].Texture > 0)
                {
                    var data = map.Layer[c].Tile[x, y];
                    var source = new Rectangle(new Point(data.X * Grid, data.Y * Grid), GridSize);
                    var destiny = new Rectangle(new Point((x - beginX) * Grid, (y - beginY) * Grid), GridSize);

                    if (!data.IsAutoTile)
                        Render(WinMapRT!, Textures.Tiles[data.Texture], source, win.ZoomRect(destiny), color);
                    else
                        EditorMapsAutoTile(destiny.Location, data, color);
                }
        }
    }

    private static void EditorMapsAutoTile(Point position, MapTileData data, Color color)
    {
        var win = EditorMapsWindow.Instance!;
        for (byte i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 1: position.X += 16; break;
                case 2: position.Y += 16; break;
                case 3:
                    position.X += 16;
                    position.Y += 16;
                    break;
            }

            Render(WinMapRT!, Textures.Tiles[data.Texture], new Rectangle(data.Mini[i].X, data.Mini[i].Y, 16, 16),
                win.ZoomRect(new Rectangle(position, new Size(16, 16))), color);
        }
    }

    private static void EditorMapsMapFog(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (map.Fog.Texture <= 0 || !win.ShowVisualizationSafe) return;

        var textureSize = Textures.Fogs[map.Fog.Texture].ToSize();
        for (var x = -1; x <= Map.Width * Grid / textureSize.Width; x++)
        for (var y = -1; y <= Map.Height * Grid / textureSize.Height; y++)
        {
            var position = new Point(x * textureSize.Width + TempMap.FogX, y * textureSize.Height + TempMap.FogY);
            Render(WinMapRT!, Textures.Fogs[map.Fog.Texture], win.ZoomRect(new Rectangle(position, textureSize)),
                new Color(255, 255, 255, map.Fog.Alpha));
        }
    }

    private static void EditorMapsMapWeather(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (!win.ShowVisualizationSafe || map.Weather.Type == Weather.Normal) return;

        byte x = 0;
        if (map.Weather.Type == Weather.Snowing) x = 32;

        for (var i = 0; i < TempMap.Weather.Length; i++)
            if (TempMap.Weather[i].Visible)
                Render(WinMapRT!, Textures.Weather, new Rectangle(x, 0, 32, 32),
                    win.ZoomRect(new Rectangle(TempMap.Weather[i].X, TempMap.Weather[i].Y, 32, 32)),
                    new Color(255, 255, 255, 150));
    }

    private static void EditorMapsMapGrids(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        Rectangle source = win.TileSource, destiny = new();
        var begin = new Point(win.MapSelection.X - win.MapScrollX, win.MapSelection.Y - win.MapScrollY);

        destiny.Location = win.ZoomGrid(begin.X, begin.Y);
        destiny.Size = new Size(source.Width / win.Zoom(), source.Height / win.Zoom());

        if (win.ShowGrid)
            for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
            {
                RenderRectangle(WinMapRT!, x * win.GridZoom, y * win.GridZoom, win.GridZoom, win.GridZoom,
                    new Color(25, 25, 25, 70));
                EditorMapsMapZones(map, x, y);
                EditorMapsMapAttributes(map, x, y);
                EditorMapsMapDirBlock(map, x, y);
            }

        if (!win.AutoTile && win.ModeNormal)
        {
            if (win.ToolPencil)
                Render(WinMapRT!, Textures.Tiles[win.TileSheetIndex + 1], source, destiny);
            else if (win.ToolRectangle)
                for (var x = begin.X; x < begin.X + win.MapSelection.Width; x++)
                for (var y = begin.Y; y < begin.Y + win.MapSelection.Height; y++)
                    Render(WinMapRT!, Textures.Tiles[win.TileSheetIndex + 1], source,
                        new Rectangle(win.ZoomGrid(x, y), destiny.Size));
        }

        if (!win.ModeAttributes || !win.DirBlockMode)
            RenderRectangle(WinMapRT!, destiny.X, destiny.Y, win.MapSelection.Width * win.GridZoom,
                win.MapSelection.Height * win.GridZoom);
    }

    private static void EditorMapsMapZones(Map map, byte x, byte y)
    {
        var win = EditorMapsWindow.Instance!;
        var position = new Point((x - win.MapScrollX) * win.GridZoom, (y - win.MapScrollY) * win.GridZoom);
        var zoneNum = map.Attribute[x, y].Zone;
        Color color;

        if (!win.ModeZones || zoneNum == 0) return;

        if (zoneNum % 2 == 0)
            color = new Color((byte)((zoneNum * 42) ^ 3), (byte)(zoneNum * 22), (byte)(zoneNum * 33), 150);
        else
            color = new Color((byte)(zoneNum * 33), (byte)(zoneNum * 22), (byte)(zoneNum * 42), 150 ^ 3);

        Render(WinMapRT!, Textures.Blank, new Rectangle(position, new Size(win.GridZoom, win.GridZoom)), color);
        DrawText(WinMapRT!, zoneNum.ToString(), position.X, position.Y, Color.White);
    }

    private static void EditorMapsMapAttributes(Map map, byte x, byte y)
    {
        var win = EditorMapsWindow.Instance!;
        var position = new Point((x - win.MapScrollX) * win.GridZoom, (y - win.MapScrollY) * win.GridZoom);
        var attribute = (TileAttribute)map.Attribute[x, y].Type;
        Color color;
        string letter;

        if (!win.ModeAttributes || win.DirBlockMode || attribute == TileAttribute.None) return;

        switch (attribute)
        {
            case TileAttribute.Block:
                letter = "B";
                color = Color.Red;
                break;
            case TileAttribute.Warp:
                letter = "T";
                color = Color.Blue;
                break;
            case TileAttribute.Item:
                letter = "I";
                color = Color.Green;
                break;
            default: return;
        }

        color = new Color(color.R, color.G, color.B, 100);

        Render(WinMapRT!, Textures.Blank, new Rectangle(position, new Size(win.GridZoom, win.GridZoom)), color);
        DrawText(WinMapRT!, letter, position.X, position.Y, Color.White);
    }

    private static void EditorMapsMapDirBlock(Map map, byte x, byte y)
    {
        var win = EditorMapsWindow.Instance!;
        var tile = new Point(win.MapScrollX + x, win.MapScrollY + y);

        if (!win.ModeAttributes || !win.DirBlockMode) return;
        if (tile.X > map.Attribute.GetUpperBound(0)) return;
        if (tile.Y > map.Attribute.GetUpperBound(1)) return;

        for (byte i = 0; i < (byte)Direction.Count; i++)
        {
            var sourceY = map.Attribute[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;
            Render(WinMapRT!, Textures.Directions, x * Grid + Block_Position(i).X, y * Grid + Block_Position(i).Y,
                i * 8, sourceY, 6, 6);
        }
    }

    private static void EditorMapsMapNpcs(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (!win.ModeNPCs) return;

        for (byte i = 0; i < map.Npc.Count; i++)
            if (map.Npc[i].Spawn)
            {
                var position = new Point((map.Npc[i].X - win.MapScrollX) * win.GridZoom,
                    (map.Npc[i].Y - win.MapScrollY) * win.GridZoom);
                Render(WinMapRT!, Textures.Blank, new Rectangle(position, new Size(win.GridZoom, win.GridZoom)),
                    new Color(0, 220, 0, 150));
                DrawText(WinMapRT!, (i + 1).ToString(), position.X + 10, position.Y + 10, Color.White);
            }
    }

    #endregion

    #region Tile Editor

    /// <summary>
    /// Render the Tile editor preview.
    /// </summary>
    public static void EditorTileRT()
    {
        if (WinTileRT == null || Textures.Tiles.Count == 0) return;
        var idx = EditorTilesWindow.ScrollTile;
        if (idx < 0 || idx >= Textures.Tiles.Count) return;

        WinTileRT.Clear();
        TransparentRT(WinTileRT);

        var texture = Textures.Tiles[idx];
        var position = new Point(EditorTilesWindow.ScrollX * Grid, EditorTilesWindow.ScrollY * Grid);
        Render(WinTileRT, texture, new Rectangle(position, texture.ToSize()),
            new Rectangle(new Point(0), texture.ToSize()));

        for (byte x = 0; x <= 298 / Grid; x++)
        for (byte y = 0; y <= 443 / Grid; y++)
        {
            if (EditorTilesWindow.ModeAttributes)
                EditorTileAttributesRT(idx, x, y);
            else
                EditorTileDirBlockRT(idx, x, y);

            RenderRectangle(WinTileRT, x * Grid, y * Grid, Grid, Grid, new Color(25, 25, 25, 70));
        }

        WinTileRT.Display();
    }

    private static void EditorTileAttributesRT(int idx, byte x, byte y)
    {
        var tile = new Point(EditorTilesWindow.ScrollX + x, EditorTilesWindow.ScrollY + y);
        var point = new Point(x * Grid + Grid / 2 - 5, y * Grid + Grid / 2 - 6);
        if (tile.X > Tile.List[idx].Data.GetUpperBound(0)) return;
        if (tile.Y > Tile.List[idx].Data.GetUpperBound(1)) return;

        switch ((TileAttribute)Tile.List[idx].Data[tile.X, tile.Y].Attribute)
        {
            case TileAttribute.Block:
                Render(WinTileRT, Textures.Blank, x * Grid, y * Grid, 0, 0, Grid, Grid, new Color(225, 0, 0, 75));
                DrawText(WinTileRT, "B", point.X, point.Y, Color.Red);
                break;
        }
    }

    private static void EditorTileDirBlockRT(int idx, byte x, byte y)
    {
        var tile = new Point(EditorTilesWindow.ScrollX + x, EditorTilesWindow.ScrollY + y);
        if (tile.X > Tile.List[idx].Data.GetUpperBound(0)) return;
        if (tile.Y > Tile.List[idx].Data.GetUpperBound(1)) return;

        if (Tile.List[idx].Data[x, y].Attribute == (byte)TileAttribute.Block)
        {
            EditorTileAttributesRT(idx, x, y);
            return;
        }

        for (byte i = 0; i < (byte)Direction.Count; i++)
        {
            var sourceY = Tile.List[idx].Data[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;
            Render(WinTileRT, Textures.Directions, x * Grid + Block_Position(i).X, y * Grid + Block_Position(i).Y,
                i * 8, sourceY, 6, 6);
        }
    }

    private static void TransparentRT(RenderTexture target)
    {
        var textureSize = Textures.Transparent.ToSize();
        for (var x = 0; x <= target.Size.X / textureSize.Width; x++)
        for (var y = 0; y <= target.Size.Y / textureSize.Height; y++)
            Render(target, Textures.Transparent, new Point(textureSize.Width * x, textureSize.Height * y));
    }

    #endregion

    #region NPC Editor

    /// <summary>
    /// Render the NPC preview in the NPC editor.
    /// </summary>
    public static void EditorNpcRT()
    {
        if (WinNpcRT == null || EditorNpcsWindow.CurrentTextureIndex <= 0) return;
        CharacterRT(WinNpcRT, EditorNpcsWindow.CurrentTextureIndex);
    }

    #endregion

    #region Character

    private static void CharacterRT(RenderTexture target, short textureNum)
    {
        var texture = Textures.Characters[textureNum];
        var size = new Size(texture.ToSize().Width / 4, texture.ToSize().Height / 4);

        target.Clear();
        if (textureNum > 0 && textureNum < Textures.Characters.Count)
            Render(target, texture, (int)(target.Size.X - size.Width) / 2, (int)(target.Size.Y - size.Height) / 2, 0, 0,
                size.Width, size.Height);
        target.Display();
    }

    #endregion

    #region Interface Editor

    /// <summary>
    /// Render the editor interface tree to the interface render target.
    /// </summary>
    public static void Interface()
    {
        if (WinInterfaceRT == null) return;
        if (InterfaceData.Tree.Nodes.Count == 0) return;

        WinInterfaceRT.Clear();
        InterfaceOrder(WinInterfaceRT, InterfaceData.Tree.Nodes[EditorInterfaceWindow.SelectedWindowIndex]);
        WinInterfaceRT.Display();
    }

    private static void InterfaceOrder(RenderTarget target, InterfaceNode node)
    {
        for (byte i = 0; i < node.Nodes.Count; i++)
        {
            var tool = (Component)node.Nodes[i].Tag!;
            if (tool.Visible)
            {
                if (tool is Panel panel) Panel(target, panel);
                else if (tool is TextBox textBox) TextBox(target, textBox);
                else if (tool is Button button) Button(target, button);
                else if (tool is CheckBox checkBox) CheckBox(target, checkBox);

                InterfaceOrder(target, node.Nodes[i]);
            }
        }
    }

    private static void Button(RenderTarget target, Button tool)
    {
        if (tool.TextureNum < Textures.Buttons.Count)
            Render(target, Textures.Buttons[tool.TextureNum], tool.Position, new Color(255, 255, 225, 225));
    }

    private static void Panel(RenderTarget target, Panel tool)
    {
        if (tool.TextureNum < Textures.Panels.Count)
            Render(target, Textures.Panels[tool.TextureNum], tool.Position);
    }

    private static void CheckBox(RenderTarget target, CheckBox tool)
    {
        // Configure source/destination rectangles.
        var recSource = new Rectangle(new Point(),
            new Size(Textures.CheckBox.ToSize().Width / 2, Textures.CheckBox.ToSize().Height));
        var recDestiny = new Rectangle(tool.Position, recSource.Size);

        // Use checked state to select marker sprite.
        if (tool.Checked)
            recSource.Location = new Point(Textures.CheckBox.ToSize().Width / 2, 0);

        byte margin = 4;
        Render(target, Textures.CheckBox, recSource, recDestiny);
        DrawText(target, tool.Text, recDestiny.Location.X + Textures.CheckBox.ToSize().Width / 2 + margin,
            recDestiny.Location.Y + 1, Color.White);
    }

    private static void TextBox(RenderTarget target, TextBox tool)
    {
        Render_Box(target, Textures.TextBox, 3, tool.Position, new Size(tool.Width, Textures.TextBox.ToSize().Height));
    }

    #endregion
}