using System.Drawing;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Entities.Tools;
using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using static CryBits.Globals;
using static CryBits.Editors.Logic.Utils;
using Button = CryBits.Editors.Entities.Tools.Button;
using CheckBox = CryBits.Editors.Entities.Tools.CheckBox;
using Color = SFML.Graphics.Color;
using Panel = CryBits.Editors.Entities.Tools.Panel;
using TextBox = CryBits.Editors.Entities.Tools.TextBox;

namespace CryBits.Editors.Media.Graphics;

internal static class Renders
{
    // Locais de renderização
    public static RenderWindow WinInterface;
    public static RenderWindow WinTile;
    public static RenderWindow WinMap;
    public static RenderWindow WinMapTile;
    public static RenderWindow WinItem;
    public static RenderWindow WinClass;
    public static RenderWindow WinNpc;

    #region Engine

    private static void Render(RenderWindow window, Texture texture, Rectangle source, Rectangle destiny, object color = null, object mode = null)
    {
        // Define os dados
        var tmpImage = new Sprite(texture)
        {
            TextureRect = new IntRect(source.X, source.Y, source.Width, source.Height),
            Position = new Vector2f(destiny.X, destiny.Y),
            Scale = new Vector2f(destiny.Width / (float)source.Width, destiny.Height / (float)source.Height)
        };
        if (color != null) tmpImage.Color = (Color)color;

        // Renderiza a textura em forma de retângulo
        if (mode == null) mode = RenderStates.Default;
        window.Draw(tmpImage, (RenderStates)mode);
    }

    private static void Render(RenderWindow window, Texture texture, int x, int y, int sourceX, int sourceY, int sourceWidth, int sourceHeight, object color = null, object mode = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(new Point(sourceX, sourceY), new Size(sourceWidth, sourceHeight));
        var destiny = new Rectangle(new Point(x, y), new Size(sourceWidth, sourceHeight));

        // Desenha a textura
        Render(window, texture, source, destiny, color, mode);
    }

    private static void Render(RenderWindow window, Texture texture, Rectangle destiny, object color = null, object mode = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(new Point(0), texture.ToSize());

        // Desenha a textura
        Render(window, texture, source, destiny, color, mode);
    }

    private static void Render(RenderWindow window, Texture texture, Point point, object color = null, object mode = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(new Point(0), texture.ToSize());
        var destiny = new Rectangle(point, texture.ToSize());

        // Desenha a textura
        Render(window, texture, source, destiny, color, mode);
    }

    private static void RenderRectangle(RenderWindow window, Rectangle rectangle, object color = null)
    {
        // Desenha a caixa
        Render(window, Textures.Grid, rectangle.X, rectangle.Y, 0, 0, rectangle.Width, 1, color);
        Render(window, Textures.Grid, rectangle.X, rectangle.Y, 0, 0, 1, rectangle.Height, color);
        Render(window, Textures.Grid, rectangle.X, rectangle.Y + rectangle.Height - 1, 0, 0, rectangle.Width, 1, color);
        Render(window, Textures.Grid, rectangle.X + rectangle.Width - 1, rectangle.Y, 0, 0, 1, rectangle.Height, color);
    }

    private static void RenderRectangle(RenderWindow window, int x, int y, int width, int height, object color = null)
    {
        // Desenha a caixa
        RenderRectangle(window, new Rectangle(x, y, width, height), color);
    }

    private static void Render_Box(RenderWindow window, Texture texture, byte margin, Point position, Size size)
    {
        var textureWidth = texture.ToSize().Width;
        var textureHeight = texture.ToSize().Height;

        // Borda esquerda
        Render(window, texture, new Rectangle(new Point(0), new Size(margin, textureWidth)), new Rectangle(position, new Size(margin, textureHeight)));
        // Borda direita
        Render(window, texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
        // Centro
        Render(window, texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + margin, position.Y), new Size(size.Width - (margin * 2), textureHeight)));
    }

    private static void DrawText(RenderWindow window, string text, int x, int y, Color color)
    {
        var tempText = new Text(text, Fonts.Default);

        // Define os dados
        tempText.CharacterSize = 10;
        tempText.FillColor = color;
        tempText.Position = new Vector2f(x, y);
        tempText.OutlineColor = new Color(0, 0, 0, 70);
        tempText.OutlineThickness = 1;

        // Desenha
        window.Draw(tempText);
    }
    #endregion

    public static void Present()
    {
        // Desenha 
        EditorMapsTile();
        EditorMapsMap();
        EditorTile();
        EditorClass();
        EditorItem();
        EditorNPC();
        Interface();
    }

    private static void Transparent(RenderWindow window)
    {
        var textureSize = Textures.Transparent.ToSize();

        // Desenha uma textura transparente na janela inteira
        for (var x = 0; x <= window.Size.X / textureSize.Width; x++)
        for (var y = 0; y <= window.Size.Y / textureSize.Height; y++)
            Render(window, Textures.Transparent, new Point(textureSize.Width * x, textureSize.Height * y));
    }

    #region Map Editor
    private static void EditorMapsTile()
    {
        var form = EditorMaps.Form;

        // Somente se necessário
        if (WinMap == null || !form.butMNormal.Checked) return;

        // Reinicia o dispositivo caso haja alguma alteração no tamanho da tela
        if (new Size((int)WinMapTile.Size.X, (int)WinMapTile.Size.Y) != form.picTile.Size)
        {
            WinMapTile.Dispose();
            WinMapTile = new RenderWindow(EditorMaps.Form.picTile.Handle);
        }

        // Limpa a área com um fundo preto
        WinMapTile.Clear(Color.Black);

        // Dados
        var texture = Textures.Tiles[form.cmbTiles.SelectedIndex + 1];
        var position = new Point(form.scrlTileX.Value, form.scrlTileY.Value);

        // Desenha o azulejo e as grades
        Transparent(WinMapTile);
        Render(WinMapTile, texture, new Rectangle(position, texture.ToSize()), new Rectangle(new Point(0), texture.ToSize()));
        RenderRectangle(WinMapTile, new Rectangle(new Point(form.TileSource.X - position.X, form.TileSource.Y - position.Y), form.TileSource.Size), new Color(165, 42, 42, 250));
        RenderRectangle(WinMapTile, form.TileMouse.X, form.TileMouse.Y, Grid, Grid, new Color(65, 105, 225, 250));

        // Exibe o que foi renderizado
        WinMapTile.Display();
    }

    private static void EditorMapsMap()
    {
        // Previne erros
        if (EditorMaps.Form?.IsDisposed != false || EditorMaps.Form.Selected == null) return;

        // Limpa a área com um fundo preto
        WinMap.Clear(Color.Black);

        // Desenha o mapa
        var selected = EditorMaps.Form.Selected;
        EditorMapsMapPanorama(selected);
        EditorMapsMapTiles(selected);
        EditorMapsMapWeather(selected);
        EditorMapsMapFog(selected);
        EditorMapsMapGrids(selected);
        EditorMapsMapNpcs(selected);

        // Exibe o que foi renderizado
        WinMap.Display();
    }

    private static void EditorMapsMapPanorama(Map map)
    {
        var form = EditorMaps.Form;

        // Desenha o panorama
        if (form.butVisualization.Checked && map.Panorama > 0)
        {
            var destiny = new Rectangle
            {
                X = form.scrlMapX.Value * -form.GridZoom,
                Y = form.scrlMapY.Value * -form.GridZoom,
                Size = Textures.Panoramas[map.Panorama].ToSize()
            };
            Render(WinMap, Textures.Panoramas[map.Panorama], EditorMaps.Form.Zoom(destiny));
        }
    }

    private static void EditorMapsMapTiles(Map map)
    {
        var form = EditorMaps.Form;
        MapTileData data;
        int beginX = form.scrlMapX.Value, beginY = form.scrlMapY.Value;
        Color color;

        // Desenha todos os azulejos
        for (byte c = 0; c < map.Layer.Count; c++)
        {
            // Somente se necessário
            if (!form.lstLayers.Items[c].Checked) continue;

            // Transparência da camada
            color = new Color();
            if (form.butEdition.Checked && form.butMNormal.Checked)
            {
                if (EditorMaps.Form.lstLayers.SelectedIndices.Count > 0)
                    if (c != EditorMaps.Form.lstLayers.SelectedItems[0].Index)
                        color = new Color(255, 255, 255, 150);
            }
            else
                color = new Color(map.Color.R, map.Color.G, map.Color.B);

            // Continua
            for (var x = beginX; x < Map.Width; x++)
            for (var y = beginY; y < Map.Height; y++)
                if (map.Layer[c].Tile[x, y].Texture > 0)
                {
                    // Dados
                    data = map.Layer[c].Tile[x, y];
                    var source = new Rectangle(new Point(data.X * Grid, data.Y * Grid), GridSize);
                    var destiny = new Rectangle(new Point((x - beginX) * Grid, (y - beginY) * Grid), GridSize);

                    // Desenha o azulejo
                    if (!data.IsAutoTile)
                        Render(WinMap, Textures.Tiles[data.Texture], source, form.Zoom(destiny), color);
                    else
                        EditorMapsAutoTile(destiny.Location, data, color);
                }
        }
    }

    private static void EditorMapsAutoTile(Point position, MapTileData data, Color color)
    {
        // Desenha todas as partes do azulejo
        for (byte i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 1: position.X += 16; break;
                case 2: position.Y += 16; break;
                case 3: position.X += 16; position.Y += 16; break;
            }
            Render(WinMap, Textures.Tiles[data.Texture], new Rectangle(data.Mini[i].X, data.Mini[i].Y, 16, 16), EditorMaps.Form.Zoom(new Rectangle(position, new Size(16, 16))), color);
        }
    }

    private static void EditorMapsMapFog(Map map)
    {
        // Somente se necessário
        if (map.Fog.Texture <= 0) return;
        if (!EditorMaps.Form.butVisualization.Checked) return;

        // Desenha a fumaça
        var textureSize = Textures.Fogs[map.Fog.Texture].ToSize();
        for (var x = -1; x <= Map.Width * Grid / textureSize.Width; x++)
        for (var y = -1; y <= Map.Height * Grid / textureSize.Height; y++)
        {
            var position = new Point((x * textureSize.Width) + TempMap.FogX, (y * textureSize.Height) + TempMap.FogY);
            Render(WinMap, Textures.Fogs[map.Fog.Texture], EditorMaps.Form.Zoom(new Rectangle(position, textureSize)), new Color(255, 255, 255, map.Fog.Alpha));
        }
    }

    private static void EditorMapsMapWeather(Map map)
    {
        // Somente se necessário
        if (!EditorMaps.Form.butVisualization.Checked || map.Weather.Type == Weather.Normal) return;

        // Dados
        byte x = 0;
        if (map.Weather.Type == Weather.Snowing) x = 32;

        // Desenha as partículas
        for (var i = 0; i < TempMap.Weather.Length; i++)
            if (TempMap.Weather[i].Visible)
                Render(WinMap, Textures.Weather, new Rectangle(x, 0, 32, 32), EditorMaps.Form.Zoom(new Rectangle(TempMap.Weather[i].X, TempMap.Weather[i].Y, 32, 32)), new Color(255, 255, 255, 150));
    }

    private static void EditorMapsMapGrids(Map map)
    {
        var form = EditorMaps.Form;
        Rectangle source = form.TileSource, destiny = new();
        var begin = new Point(form.MapSelection.X - form.scrlMapX.Value, form.MapSelection.Y - form.scrlMapY.Value);

        // Dados
        destiny.Location = form.Zoom_Grid(begin.X, begin.Y);
        destiny.Size = new Size(source.Width / form.Zoom(), source.Height / form.Zoom());

        // Desenha as grades
        if (form.butGrid.Checked || !form.butGrid.Enabled)
            for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
            {
                RenderRectangle(WinMap, x * form.GridZoom, y * form.GridZoom, form.GridZoom, form.GridZoom, new Color(25, 25, 25, 70));
                EditorMapsMapZones(map, x, y);
                EditorMapsMapAttributes(map, x, y);
                EditorMapsMapDirBlock(map, x, y);
            }

        if (!form.chkAuto.Checked && form.butMNormal.Checked)
            // Normal
            if (form.butPencil.Checked)
                Render(WinMap, Textures.Tiles[form.cmbTiles.SelectedIndex + 1], source, destiny);
            // Retângulo
            else if (form.butRectangle.Checked)
                for (var x = begin.X; x < begin.X + form.MapSelection.Width; x++)
                for (var y = begin.Y; y < begin.Y + form.MapSelection.Height; y++)
                    Render(WinMap, Textures.Tiles[form.cmbTiles.SelectedIndex + 1], source, new Rectangle(form.Zoom_Grid(x, y), destiny.Size));

        // Desenha a grade
        if (!form.butMAttributes.Checked || !form.optA_DirBlock.Checked)
            RenderRectangle(WinMap, destiny.X, destiny.Y, form.MapSelection.Width * form.GridZoom, form.MapSelection.Height * form.GridZoom);
    }

    private static void EditorMapsMapZones(Map map, byte x, byte y)
    {
        var form = EditorMaps.Form;
        var position = new Point((x - form.scrlMapX.Value) * form.GridZoom, (y - form.scrlMapY.Value) * form.GridZoom);
        var zoneNum = map.Attribute[x, y].Zone;
        Color color;

        // Apenas se necessário
        if (!EditorMaps.Form.butMZones.Checked) return;
        if (zoneNum == 0) return;

        // Define a cor
        if (zoneNum % 2 == 0)
            color = new Color((byte)((zoneNum * 42) ^ 3), (byte)(zoneNum * 22), (byte)(zoneNum * 33), 150);
        else
            color = new Color((byte)(zoneNum * 33), (byte)(zoneNum * 22), (byte)(zoneNum * 42), 150 ^ 3);

        // Desenha as zonas
        Render(WinMap, Textures.Blank, new Rectangle(position, new Size(form.GridZoom, form.GridZoom)), color);
        DrawText(WinMap, zoneNum.ToString(), position.X, position.Y, Color.White);
    }

    private static void EditorMapsMapAttributes(Map map, byte x, byte y)
    {
        var form = EditorMaps.Form;
        var position = new Point((x - form.scrlMapX.Value) * form.GridZoom, (y - EditorMaps.Form.scrlMapY.Value) * form.GridZoom);
        var attribute = (TileAttribute)map.Attribute[x, y].Type;
        Color color;
        string letter;

        // Apenas se necessário
        if (!EditorMaps.Form.butMAttributes.Checked) return;
        if (EditorMaps.Form.optA_DirBlock.Checked) return;
        if (attribute == TileAttribute.None) return;

        // Define a cor e a letra
        switch (attribute)
        {
            case TileAttribute.Block: letter = "B"; color = Color.Red; break;
            case TileAttribute.Warp: letter = "T"; color = Color.Blue; break;
            case TileAttribute.Item: letter = "I"; color = Color.Green; break;
            default: return;
        }
        color = new Color(color.R, color.G, color.B, 100);

        // Desenha as Atributos
        Render(WinMap, Textures.Blank, new Rectangle(position, new Size(form.GridZoom, form.GridZoom)), color);
        DrawText(WinMap, letter, position.X, position.Y, Color.White);
    }

    private static void EditorMapsMapDirBlock(Map map, byte x, byte y)
    {
        var tile = new Point(EditorMaps.Form.scrlMapX.Value + x, EditorMaps.Form.scrlMapY.Value + y);
        byte sourceY;

        // Apenas se necessário
        if (!EditorMaps.Form.butMAttributes.Checked) return;
        if (!EditorMaps.Form.optA_DirBlock.Checked) return;

        // Previne erros
        if (tile.X > map.Attribute.GetUpperBound(0)) return;
        if (tile.Y > map.Attribute.GetUpperBound(1)) return;

        for (byte i = 0; i < (byte)Direction.Count; i++)
        {
            // Estado do bloqueio
            sourceY = map.Attribute[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;

            // Renderiza
            Render(WinMap, Textures.Directions, (x * Grid) + Block_Position(i).X, (y * Grid) + Block_Position(i).Y, i * 8, sourceY, 6, 6);
        }
    }

    private static void EditorMapsMapNpcs(Map map)
    {
        var form = EditorMaps.Form;

        if (EditorMaps.Form.butMNPCs.Checked)
            for (byte i = 0; i < map.Npc.Count; i++)
                if (map.Npc[i].Spawn)
                {
                    var position = new Point((map.Npc[i].X - form.scrlMapX.Value) * form.GridZoom, (map.Npc[i].Y - form.scrlMapY.Value) * form.GridZoom);

                    // Desenha uma sinalização de onde os NPCs estão
                    Render(WinMap, Textures.Blank, new Rectangle(position, new Size(form.GridZoom, form.GridZoom)), new Color(0, 220, 0, 150));
                    DrawText(WinMap, (i + 1).ToString(), position.X + 10, position.Y + 10, Color.White);
                }
    }
    #endregion

    #region Tile Editor
    public static void EditorTile()
    {
        var form = EditorTiles.Form;

        // Somente se necessário
        if (WinTile == null) return;

        // Limpa a tela e desenha um fundo transparente
        WinTile.Clear();
        Transparent(WinTile);

        // Desenha o azulejo e as grades
        var texture = Textures.Tiles[form.scrlTile.Value];
        var position = new Point(form.scrlTileX.Value * Grid, form.scrlTileY.Value * Grid);
        Render(WinTile, texture, new Rectangle(position, texture.ToSize()), new Rectangle(new Point(0), texture.ToSize()));

        for (byte x = 0; x <= form.picTile.Width / Grid; x++)
        for (byte y = 0; y <= form.picTile.Height / Grid; y++)
        {
            // Desenha os atributos
            if (form.optAttributes.Checked)
                EditorTileAttributes(x, y);
            // Bloqueios direcionais
            else if (form.optDirBlock.Checked)
                EditorTileDirBlock(x, y);

            // Grades
            RenderRectangle(WinTile, x * Grid, y * Grid, Grid, Grid, new Color(25, 25, 25, 70));
        }

        // Exibe o que foi renderizado
        WinTile.Display();
    }

    private static void EditorTileAttributes(byte x, byte y)
    {
        var form = EditorTiles.Form;
        var tile = new Point(form.scrlTileX.Value + x, form.scrlTileY.Value + y);
        var point = new Point((x * Grid) + (Grid / 2) - 5, (y * Grid) + (Grid / 2) - 6);

        // Previne erros
        if (tile.X > Tile.List[form.scrlTile.Value].Data.GetUpperBound(0)) return;
        if (tile.Y > Tile.List[form.scrlTile.Value].Data.GetUpperBound(1)) return;

        // Desenha uma letra e colore o azulejo referente ao atributo
        switch ((TileAttribute)Tile.List[form.scrlTile.Value].Data[tile.X, tile.Y].Attribute)
        {
            case TileAttribute.Block:
                Render(WinTile, Textures.Blank, x * Grid, y * Grid, 0, 0, Grid, Grid, new Color(225, 0, 0, 75));
                DrawText(WinTile, "B", point.X, point.Y, Color.Red);
                break;
        }
    }

    private static void EditorTileDirBlock(byte x, byte y)
    {
        var form = EditorTiles.Form;
        var tile = new Point(form.scrlTileX.Value + x, form.scrlTileY.Value + y);
        byte sourceY;

        // Previne erros
        if (tile.X > Tile.List[form.scrlTile.Value].Data.GetUpperBound(0)) return;
        if (tile.Y > Tile.List[form.scrlTile.Value].Data.GetUpperBound(1)) return;

        // Bloqueio total
        if (Tile.List[form.scrlTile.Value].Data[x, y].Attribute == (byte)TileAttribute.Block)
        {
            EditorTileAttributes(x, y);
            return;
        }

        for (byte i = 0; i < (byte)Direction.Count; i++)
        {
            // Estado do bloqueio
            sourceY = Tile.List[form.scrlTile.Value].Data[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;

            // Renderiza
            Render(WinTile, Textures.Directions, (x * Grid) + Block_Position(i).X, (y * Grid) + Block_Position(i).Y, i * 8, sourceY, 6, 6);
        }
    }
    #endregion

    #region Item Editor
    private static void EditorItem()
    {
        // Somente se necessário
        if (WinItem == null) return;

        // Desenha o item
        var textureNum = (short)EditorItems.Form.numTexture.Value;
        WinItem.Clear();
        Transparent(WinItem);
        if (textureNum > 0 && textureNum < Textures.Items.Count) Render(WinItem, Textures.Items[textureNum], new Point(0));
        WinItem.Display();
    }
    #endregion

    #region NPC Editor
    private static void EditorNPC()
    {
        // Somente se necessário
        if (WinNpc == null) return;

        // Desenha o NPC
        Character(WinNpc, (short)EditorNpcs.Form.numTexture.Value);
    }
    #endregion

    #region Class Editors
    private static void EditorClass()
    {
        // Somente se necessário
        if (WinClass == null) return;

        // Desenha o NPC
        Character(WinClass, (short)EditorClasses.Form.numTexture.Value);
    }
    #endregion

    #region Character
    private static void Character(RenderWindow window, short textureNum)
    {
        var texture = Textures.Characters[textureNum];
        var size = new Size(texture.ToSize().Width / 4, texture.ToSize().Height / 4);

        // Desenha o item
        window.Clear();
        Transparent(window);
        if (textureNum > 0 && textureNum < Textures.Characters.Count) Render(window, texture, (int)(window.Size.X - size.Width) / 2, (int)(window.Size.Y - size.Height) / 2, 0, 0, size.Width, size.Height);
        window.Display();
    }
    #endregion

    #region Interface Editor
    public static void Interface()
    {
        // Apenas se necessário
        if (WinInterface == null) return;

        // Desenha as ferramentas
        WinInterface.Clear();
        InterfaceOrder(Tool.Tree.Nodes[(byte)EditorInterface.Form.cmbWindows.SelectedIndex]);
        WinInterface.Display();
    }

    private static void InterfaceOrder(TreeNode node)
    {
        for (byte i = 0; i < node.Nodes.Count; i++)
        {
            // Desenha a ferramenta
            var tool = (Tool)node.Nodes[i].Tag;
            if (tool.Visible)
            {
                if (tool is Panel panel) Panel(panel);
                else if (tool is TextBox textBox) TextBox(textBox);
                else if (tool is Button button) Button(button);
                else if (tool is CheckBox checkBox) CheckBox(checkBox);

                // Pula pra próxima
                InterfaceOrder(node.Nodes[i]);
            }
        }
    }

    private static void Button(Button tool)
    {
        // Desenha o botão
        if (tool.TextureNum < Textures.Buttons.Count)
            Render(WinInterface, Textures.Buttons[tool.TextureNum], tool.Position, new Color(255, 255, 225, 225));
    }

    private static void Panel(Panel tool)
    {
        // Desenha o painel
        if (tool.TextureNum < Textures.Panels.Count)
            Render(WinInterface, Textures.Panels[tool.TextureNum], tool.Position);
    }

    private static void CheckBox(CheckBox tool)
    {
        // Define as propriedades dos retângulos
        var recSource = new Rectangle(new Point(), new Size(Textures.CheckBox.ToSize().Width / 2, Textures.CheckBox.ToSize().Height));
        var recDestiny = new Rectangle(tool.Position, recSource.Size);

        // Desenha a textura do marcador pelo seu estado 
        if (tool.Checked)
            recSource.Location = new Point(Textures.CheckBox.ToSize().Width / 2, 0);

        // Desenha o marcador 
        byte margin = 4;
        Render(WinInterface, Textures.CheckBox, recSource, recDestiny);
        DrawText(WinInterface, tool.Text, recDestiny.Location.X + (Textures.CheckBox.ToSize().Width / 2) + margin, recDestiny.Location.Y + 1, Color.White);
    }

    private static void TextBox(TextBox tool)
    {
        // Desenha a ferramenta
        Render_Box(WinInterface, Textures.TextBox, 3, tool.Position, new Size(tool.Width, Textures.TextBox.ToSize().Height));
    }
    #endregion
}