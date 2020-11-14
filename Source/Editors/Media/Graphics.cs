using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Entities.Tools;
using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using CryBits.Entities;
using SFML.Graphics;
using SFML.System;
using static CryBits.Editors.Logic.Utils;
using Button = CryBits.Editors.Entities.Tools.Button;
using CheckBox = CryBits.Editors.Entities.Tools.CheckBox;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;
using Panel = CryBits.Editors.Entities.Tools.Panel;
using TextBox = CryBits.Editors.Entities.Tools.TextBox;

namespace CryBits.Editors.Media
{
    internal static class Graphics
    {
        // Locais de renderização
        public static RenderWindow WinInterface;
        public static RenderWindow WinTile;
        public static RenderWindow WinMap;
        public static RenderWindow WinMapTile;
        public static RenderTexture WinMapLighting;
        public static RenderWindow WinItem;
        public static RenderWindow WinClass;
        public static RenderWindow WinNPC;

        // Fonte principal
        public static Font FontDefault;

        // Texturas
        public static Texture[] TexCharacter;
        public static Texture[] TexTile;
        public static Texture[] TexFace;
        public static Texture[] TexPanel;
        public static Texture[] TexButton;
        public static Texture[] TexPanorama;
        public static Texture[] TexFog;
        public static Texture[] TexItem;
        public static Texture TexCheckBox;
        public static Texture TexTextBox;
        public static Texture TexGrid;
        public static Texture TexWeather;
        public static Texture TexBlank;
        public static Texture TexDirections;
        public static Texture TexTransparent;
        public static Texture TexLighting;

        // Formato das texturas
        public const string Format = ".png";

        #region Engine
        private static Texture[] AddTextures(string directory)
        {
            short i = 1;
            Texture[] tempTex = Array.Empty<Texture>();

            while (File.Exists(directory + i + Format))
            {
                // Carrega todas do diretório e as adiciona a lista
                Array.Resize(ref tempTex, i + 1);
                tempTex[i] = new Texture(directory + i + Format);
                i += 1;
            }

            // Retorna o cache da textura
            return tempTex;
        }

        public static Size Size(Texture texture)
        {
            // Retorna com o tamanho da textura
            if (texture != null)
                return new Size((int)texture.Size.X, (int)texture.Size.Y);
            return new Size(0, 0);
        }

        // Retorna a cor
        private static Color CColor(byte r = 255, byte g = 255, byte b = 255, byte a = 255) => new Color(r, g, b, a);

        private static void Render(RenderWindow window, Texture texture, Rectangle source, Rectangle destiny, object color = null, object mode = null)
        {
            // Define os dados
            Sprite tmpImage = new Sprite(texture)
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

        private static void Render(RenderTexture window, Texture texture, Rectangle destiny, object color = null, object mode = null)
        {
            // Define os dados
            Sprite tmpImage = new Sprite(texture)
            {
                Position = new Vector2f(destiny.X, destiny.Y),
                Scale = new Vector2f(destiny.Width / (float)Size(texture).Width, destiny.Height / (float)Size(texture).Height)
            };
            if (color != null) tmpImage.Color = (Color)color;

            // Renderiza a textura em forma de retângulo
            if (mode == null) mode = RenderStates.Default;
            window.Draw(tmpImage, (RenderStates)mode);
        }

        private static void Render(RenderWindow window, Texture texture, int x, int y, int sourceX, int sourceY, int sourceWidth, int sourceHeight, object color = null, object mode = null)
        {
            // Define as propriedades dos retângulos
            Rectangle source = new Rectangle(new Point(sourceX, sourceY), new Size(sourceWidth, sourceHeight));
            Rectangle destiny = new Rectangle(new Point(x, y), new Size(sourceWidth, sourceHeight));

            // Desenha a textura
            Render(window, texture, source, destiny, color, mode);
        }

        private static void Render(RenderWindow window, Texture texture, Rectangle destiny, object color = null, object mode = null)
        {
            // Define as propriedades dos retângulos
            Rectangle source = new Rectangle(new Point(0), Size(texture));

            // Desenha a textura
            Render(window, texture, source, destiny, color, mode);
        }

        private static void Render(RenderWindow window, Texture texture, Point point, object color = null, object mode = null)
        {
            // Define as propriedades dos retângulos
            Rectangle source = new Rectangle(new Point(0), Size(texture));
            Rectangle destiny = new Rectangle(point, Size(texture));

            // Desenha a textura
            Render(window, texture, source, destiny, color, mode);
        }

        private static void RenderRectangle(RenderWindow window, Rectangle rectangle, object color = null)
        {
            // Desenha a caixa
            Render(window, TexGrid, rectangle.X, rectangle.Y, 0, 0, rectangle.Width, 1, color);
            Render(window, TexGrid, rectangle.X, rectangle.Y, 0, 0, 1, rectangle.Height, color);
            Render(window, TexGrid, rectangle.X, rectangle.Y + rectangle.Height - 1, 0, 0, rectangle.Width, 1, color);
            Render(window, TexGrid, rectangle.X + rectangle.Width - 1, rectangle.Y, 0, 0, 1, rectangle.Height, color);
        }

        private static void RenderRectangle(RenderWindow window, int x, int y, int width, int height, object color = null)
        {
            // Desenha a caixa
            RenderRectangle(window, new Rectangle(x, y, width, height), color);
        }

        private static void Render_Box(RenderWindow window, Texture texture, byte margin, Point position, Size size)
        {
            int textureWidth = Size(texture).Width;
            int textureHeight = Size(texture).Height;

            // Borda esquerda
            Render(window, texture, new Rectangle(new Point(0), new Size(margin, textureWidth)), new Rectangle(position, new Size(margin, textureHeight)));
            // Borda direita
            Render(window, texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
            // Centro
            Render(window, texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + margin, position.Y), new Size(size.Width - margin * 2, textureHeight)));
        }

        private static void DrawText(RenderWindow window, string text, int x, int y, Color color)
        {
            Text tempText = new Text(text, FontDefault);

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

        public static void Init()
        {
            // Conjuntos
            TexCharacter = AddTextures(Directories.TexCharacters.FullName);
            TexTile = AddTextures(Directories.TexTiles.FullName);
            TexFace = AddTextures(Directories.TexFaces.FullName);
            TexPanel = AddTextures(Directories.TexPanels.FullName);
            TexButton = AddTextures(Directories.TexButtons.FullName);
            TexPanorama = AddTextures(Directories.TexPanoramas.FullName);
            TexFog = AddTextures(Directories.TexFogs.FullName);
            TexItem = AddTextures(Directories.TexItems.FullName);

            // Únicas
            TexWeather = new Texture(Directories.TexWeather.FullName + Format);
            TexBlank = new Texture(Directories.TexBlanc.FullName + Format);
            TexDirections = new Texture(Directories.TexDirections.FullName + Format);
            TexTransparent = new Texture(Directories.TexTransparent.FullName + Format);
            TexGrid = new Texture(Directories.TexGrid.FullName + Format);
            TexCheckBox = new Texture(Directories.TexCheckBox.FullName + Format);
            TexTextBox = new Texture(Directories.TexTextBox.FullName + Format);
            TexLighting = new Texture(Directories.TexLighting.FullName + Format);

            // Fontes
            FontDefault = new Font(Directories.Fonts.FullName + "Georgia.ttf");
        }

        public static void Present()
        {
            // Desenha 
            Editor_Maps_Tile();
            Editor_Maps_Map();
            Editor_Tile();
            Editor_Class();
            Editor_Item();
            Editor_NPC();
            Interface();
        }

        private static void Transparent(RenderWindow window)
        {
            Size textureSize = Size(TexTransparent);

            // Desenha uma textura transparente na janela inteira
            for (int x = 0; x <= window.Size.X / textureSize.Width; x++)
                for (int y = 0; y <= window.Size.Y / textureSize.Height; y++)
                    Render(window, TexTransparent, new Point(textureSize.Width * x, textureSize.Height * y));
        }

        #region Map Editor
        private static void Editor_Maps_Tile()
        {
            EditorMaps form = EditorMaps.Form;

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
            Texture texture = TexTile[form.cmbTiles.SelectedIndex + 1];
            Point position = new Point(form.scrlTileX.Value, form.scrlTileY.Value);

            // Desenha o azulejo e as grades
            Transparent(WinMapTile);
            Render(WinMapTile, texture, new Rectangle(position, Size(texture)), new Rectangle(new Point(0), Size(texture)));
            RenderRectangle(WinMapTile, new Rectangle(new Point(form.TileSource.X - position.X, form.TileSource.Y - position.Y), form.TileSource.Size), CColor(165, 42, 42, 250));
            RenderRectangle(WinMapTile, form.TileMouse.X, form.TileMouse.Y, Grid, Grid, CColor(65, 105, 225, 250));

            // Exibe o que foi renderizado
            WinMapTile.Display();
        }

        private static void Editor_Maps_Map()
        {
            // Previne erros
            if (EditorMaps.Form == null || EditorMaps.Form.IsDisposed || EditorMaps.Form.Selected == null) return;

            // Limpa a área com um fundo preto
            WinMap.Clear(Color.Black);

            // Desenha o mapa
            Map selected = EditorMaps.Form.Selected;
            Editor_Maps_Map_Panorama(selected);
            Editor_Maps_Map_Tiles(selected);
            Editor_Maps_Map_Weather(selected);
            Editor_Maps_Map_Light(selected);
            Editor_Maps_Map_Fog(selected);
            Editor_Maps_Map_Grids(selected);
            Editor_Maps_Map_NPCs(selected);

            // Exibe o que foi renderizado
            WinMap.Display();
        }

        private static void Editor_Maps_Map_Panorama(Map map)
        {
            EditorMaps form = EditorMaps.Form;

            // Desenha o panorama
            if (form.butVisualization.Checked && map.Panorama > 0)
            {
                Rectangle destiny = new Rectangle
                {
                    X = form.scrlMapX.Value * -form.GridZoom,
                    Y = form.scrlMapY.Value * -form.GridZoom,
                    Size = Size(TexPanorama[map.Panorama])
                };
                Render(WinMap, TexPanorama[map.Panorama], EditorMaps.Form.Zoom(destiny));
            }
        }

        private static void Editor_Maps_Map_Tiles(Map map)
        {
            EditorMaps form = EditorMaps.Form;
            MapTileData data;
            int beginX = form.scrlMapX.Value, beginY = form.scrlMapY.Value;
            Color color;

            // Desenha todos os azulejos
            for (byte c = 0; c < map.Layer.Count; c++)
            {
                // Somente se necessário
                if (!form.lstLayers.Items[c].Checked) continue;

                // Transparência da camada
                color = CColor();
                if (form.butEdition.Checked && form.butMNormal.Checked)
                {
                    if (EditorMaps.Form.lstLayers.SelectedIndices.Count > 0)
                        if (c != EditorMaps.Form.lstLayers.SelectedItems[0].Index)
                            color = CColor(255, 255, 255, 150);
                }
                else
                    color = CColor(map.Color.R, map.Color.G, map.Color.B);

                // Continua
                for (int x = beginX; x < Map.Width; x++)
                    for (int y = beginY; y < Map.Height; y++)
                        if (map.Layer[c].Tile[x, y].Texture > 0)
                        {
                            // Dados
                            data = map.Layer[c].Tile[x, y];
                            Rectangle source = new Rectangle(new Point(data.X * Grid, data.Y * Grid), GridSize);
                            Rectangle destiny = new Rectangle(new Point((x - beginX) * Grid, (y - beginY) * Grid), GridSize);

                            // Desenha o azulejo
                            if (!data.IsAutoTile)
                                Render(WinMap, TexTile[data.Texture], source, form.Zoom(destiny), color);
                            else
                                Editor_Maps_AutoTile(destiny.Location, data, color);
                        }
            }
        }

        private static void Editor_Maps_AutoTile(Point position, MapTileData data, Color color)
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
                Render(WinMap, TexTile[data.Texture], new Rectangle(data.Mini[i].X, data.Mini[i].Y, 16, 16), EditorMaps.Form.Zoom(new Rectangle(position, new Size(16, 16))), color);
            }
        }

        private static void Editor_Maps_Map_Fog(Map map)
        {
            // Somente se necessário
            if (map.Fog.Texture <= 0) return;
            if (!EditorMaps.Form.butVisualization.Checked) return;

            // Desenha a fumaça
            Size textureSize = Size(TexFog[map.Fog.Texture]);
            for (int x = -1; x <= Map.Width * Grid / textureSize.Width; x++)
                for (int y = -1; y <= Map.Height * Grid / textureSize.Height; y++)
                {
                    Point position = new Point(x * textureSize.Width + TempMap.FogX, y * textureSize.Height + TempMap.FogY);
                    Render(WinMap, TexFog[map.Fog.Texture], EditorMaps.Form.Zoom(new Rectangle(position, textureSize)), CColor(255, 255, 255, map.Fog.Alpha));
                }
        }

        private static void Editor_Maps_Map_Weather(Map map)
        {
            // Somente se necessário
            if (!EditorMaps.Form.butVisualization.Checked || map.Weather.Type == Weathers.Normal) return;

            // Dados
            byte x = 0;
            if (map.Weather.Type == Weathers.Snowing) x = 32;

            // Desenha as partículas
            for (int i = 0; i < Lists.Weather.Length; i++)
                if (Lists.Weather[i].Visible)
                    Render(WinMap, TexWeather, new Rectangle(x, 0, 32, 32), EditorMaps.Form.Zoom(new Rectangle(Lists.Weather[i].X, Lists.Weather[i].Y, 32, 32)), CColor(255, 255, 255, 150));
        }

        private static void Editor_Maps_Map_Light(Map map)
        {
            EditorMaps form = EditorMaps.Form;
            byte light = (byte)((255 * ((decimal)map.Lighting / 100) - 255) * -1);

            // Somente se necessário
            if (!form.butVisualization.Checked) return;

            // Escuridão
            WinMapLighting.Clear(CColor(0, 0, 0, light));

            // Desenha o ponto iluminado
            if (map.Light.Count > 0)
                for (byte i = 0; i < map.Light.Count; i++)
                {
                    var destiny = new Rectangle
                    {
                        X = map.Light[i].Rec.X - form.scrlMapX.Value,
                        Y = map.Light[i].Rec.Y - form.scrlMapY.Value,
                        Width = map.Light[i].Width,
                        Height = map.Light[i].Height
                    };
                    Render(WinMapLighting, TexLighting, form.Zoom_Grid(destiny), null, new RenderStates(BlendMode.Multiply));
                }

            // Pré visualização
            if (form.butMLighting.Checked)
                Render(WinMapLighting, TexLighting, form.Zoom_Grid(form.MapSelection), null, new RenderStates(BlendMode.Multiply));

            // Apresenta o que foi renderizado
            WinMapLighting.Display();
            WinMap.Draw(new Sprite(WinMapLighting.Texture));

            // Ponto de remoção da luz
            if (form.butMLighting.Checked)
                if (map.Light.Count > 0)
                    for (byte i = 0; i < map.Light.Count; i++)
                        RenderRectangle(WinMap, form.Zoom_Grid(new Rectangle(map.Light[i].Rec.X - form.scrlMapX.Value, map.Light[i].Rec.Y - form.scrlMapY.Value, 1, 1)), CColor(175, 42, 42, 175));

            // Trovoadas
            Render(WinMap, TexBlank, 0, 0, 0, 0, form.picMap.Width, form.picMap.Height, CColor(255, 255, 255, TempMap.Lightning));
        }

        private static void Editor_Maps_Map_Grids(Map map)
        {
            EditorMaps form = EditorMaps.Form;
            Rectangle source = form.TileSource, destiny = new Rectangle();
            Point begin = new Point(form.MapSelection.X - form.scrlMapX.Value, form.MapSelection.Y - form.scrlMapY.Value);

            // Dados
            destiny.Location = form.Zoom_Grid(begin.X, begin.Y);
            destiny.Size = new Size(source.Width / form.Zoom(), source.Height / form.Zoom());

            // Desenha as grades
            if (form.butGrid.Checked || !form.butGrid.Enabled)
                for (byte x = 0; x < Map.Width; x++)
                    for (byte y = 0; y < Map.Height; y++)
                    {
                        RenderRectangle(WinMap, x * form.GridZoom, y * form.GridZoom, form.GridZoom, form.GridZoom, CColor(25, 25, 25, 70));
                        Editor_Maps_Map_Zones(map, x, y);
                        Editor_Maps_Map_Attributes(map, x, y);
                        Editor_Maps_Map_DirBlock(map, x, y);
                    }

            if (!form.chkAuto.Checked && form.butMNormal.Checked)
                // Normal
                if (form.butPencil.Checked)
                    Render(WinMap, TexTile[form.cmbTiles.SelectedIndex + 1], source, destiny);
                // Retângulo
                else if (form.butRectangle.Checked)
                    for (int x = begin.X; x < begin.X + form.MapSelection.Width; x++)
                        for (int y = begin.Y; y < begin.Y + form.MapSelection.Height; y++)
                            Render(WinMap, TexTile[form.cmbTiles.SelectedIndex + 1], source, new Rectangle(form.Zoom_Grid(x, y), destiny.Size));

            // Desenha a grade
            if (!form.butMAttributes.Checked || !form.optA_DirBlock.Checked)
                RenderRectangle(WinMap, destiny.X, destiny.Y, form.MapSelection.Width * form.GridZoom, form.MapSelection.Height * form.GridZoom);
        }

        private static void Editor_Maps_Map_Zones(Map map, byte x, byte y)
        {
            EditorMaps form = EditorMaps.Form;
            Point position = new Point((x - form.scrlMapX.Value) * form.GridZoom, (y - form.scrlMapY.Value) * form.GridZoom);
            byte zoneNum = map.Attribute[x, y].Zone;
            Color color;

            // Apenas se necessário
            if (!EditorMaps.Form.butMZones.Checked) return;
            if (zoneNum == 0) return;

            // Define a cor
            if (zoneNum % 2 == 0)
                color = CColor((byte)((zoneNum * 42) ^ 3), (byte)(zoneNum * 22), (byte)(zoneNum * 33), 150);
            else
                color = CColor((byte)(zoneNum * 33), (byte)(zoneNum * 22), (byte)(zoneNum * 42), 150 ^ 3);

            // Desenha as zonas
            Render(WinMap, TexBlank, new Rectangle(position, new Size(form.GridZoom, form.GridZoom)), color);
            DrawText(WinMap, zoneNum.ToString(), position.X, position.Y, Color.White);
        }

        private static void Editor_Maps_Map_Attributes(Map map, byte x, byte y)
        {
            EditorMaps form = EditorMaps.Form;
            Point position = new Point((x - form.scrlMapX.Value) * form.GridZoom, (y - EditorMaps.Form.scrlMapY.Value) * form.GridZoom);
            TileAttributes attribute = (TileAttributes)map.Attribute[x, y].Type;
            Color color;
            string letter;

            // Apenas se necessário
            if (!EditorMaps.Form.butMAttributes.Checked) return;
            if (EditorMaps.Form.optA_DirBlock.Checked) return;
            if (attribute == TileAttributes.None) return;

            // Define a cor e a letra
            switch (attribute)
            {
                case TileAttributes.Block: letter = "B"; color = Color.Red; break;
                case TileAttributes.Warp: letter = "T"; color = Color.Blue; break;
                case TileAttributes.Item: letter = "I"; color = Color.Green; break;
                default: return;
            }
            color = new Color(color.R, color.G, color.B, 100);

            // Desenha as Atributos
            Render(WinMap, TexBlank, new Rectangle(position, new Size(form.GridZoom, form.GridZoom)), color);
            DrawText(WinMap, letter, position.X, position.Y, Color.White);
        }

        private static void Editor_Maps_Map_DirBlock(Map map, byte x, byte y)
        {
            Point tile = new Point(EditorMaps.Form.scrlMapX.Value + x, EditorMaps.Form.scrlMapY.Value + y);
            byte sourceY;

            // Apenas se necessário
            if (!EditorMaps.Form.butMAttributes.Checked) return;
            if (!EditorMaps.Form.optA_DirBlock.Checked) return;

            // Previne erros
            if (tile.X > map.Attribute.GetUpperBound(0)) return;
            if (tile.Y > map.Attribute.GetUpperBound(1)) return;

            for (byte i = 0; i < (byte)Directions.Count; i++)
            {
                // Estado do bloqueio
                sourceY = map.Attribute[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;

                // Renderiza
                Render(WinMap, TexDirections, x * Grid + Block_Position(i).X, y * Grid + Block_Position(i).Y, i * 8, sourceY, 6, 6);
            }
        }

        private static void Editor_Maps_Map_NPCs(Map map)
        {
            EditorMaps form = EditorMaps.Form;

            if (EditorMaps.Form.butMNPCs.Checked)
                for (byte i = 0; i < map.NPC.Count; i++)
                    if (map.NPC[i].Spawn)
                    {
                        Point position = new Point((map.NPC[i].X - form.scrlMapX.Value) * form.GridZoom, (map.NPC[i].Y - form.scrlMapY.Value) * form.GridZoom);

                        // Desenha uma sinalização de onde os NPCs estão
                        Render(WinMap, TexBlank, new Rectangle(position, new Size(form.GridZoom, form.GridZoom)), CColor(0, 220, 0, 150));
                        DrawText(WinMap, (i + 1).ToString(), position.X + 10, position.Y + 10, Color.White);
                    }
        }
        #endregion

        #region Tile Editor
        public static void Editor_Tile()
        {
            EditorTiles form = EditorTiles.Form;

            // Somente se necessário
            if (WinTile == null) return;

            // Limpa a tela e desenha um fundo transparente
            WinTile.Clear();
            Transparent(WinTile);

            // Desenha o azulejo e as grades
            Texture texture = TexTile[form.scrlTile.Value];
            Point position = new Point(form.scrlTileX.Value * Grid, form.scrlTileY.Value * Grid);
            Render(WinTile, texture, new Rectangle(position, Size(texture)), new Rectangle(new Point(0), Size(texture)));

            for (byte x = 0; x <= form.picTile.Width / Grid; x++)
                for (byte y = 0; y <= form.picTile.Height / Grid; y++)
                {
                    // Desenha os atributos
                    if (form.optAttributes.Checked)
                        Editor_TileAttributes(x, y);
                    // Bloqueios direcionais
                    else if (form.optDirBlock.Checked)
                        Editor_Tile_DirBlock(x, y);

                    // Grades
                    RenderRectangle(WinTile, x * Grid, y * Grid, Grid, Grid, CColor(25, 25, 25, 70));
                }

            // Exibe o que foi renderizado
            WinTile.Display();
        }

        private static void Editor_TileAttributes(byte x, byte y)
        {
            EditorTiles form = EditorTiles.Form;
            Point tile = new Point(form.scrlTileX.Value + x, form.scrlTileY.Value + y);
            Point point = new Point(x * Grid + Grid / 2 - 5, y * Grid + Grid / 2 - 6);

            // Previne erros
            if (tile.X > Lists.Tile[form.scrlTile.Value].Data.GetUpperBound(0)) return;
            if (tile.Y > Lists.Tile[form.scrlTile.Value].Data.GetUpperBound(1)) return;

            // Desenha uma letra e colore o azulejo referente ao atributo
            switch ((TileAttributes)Lists.Tile[form.scrlTile.Value].Data[tile.X, tile.Y].Attribute)
            {
                case TileAttributes.Block:
                    Render(WinTile, TexBlank, x * Grid, y * Grid, 0, 0, Grid, Grid, CColor(225, 0, 0, 75));
                    DrawText(WinTile, "B", point.X, point.Y, Color.Red);
                    break;
            }
        }

        private static void Editor_Tile_DirBlock(byte x, byte y)
        {
            EditorTiles form = EditorTiles.Form;
            Point tile = new Point(form.scrlTileX.Value + x, form.scrlTileY.Value + y);
            byte sourceY;

            // Previne erros
            if (tile.X > Lists.Tile[form.scrlTile.Value].Data.GetUpperBound(0)) return;
            if (tile.Y > Lists.Tile[form.scrlTile.Value].Data.GetUpperBound(1)) return;

            // Bloqueio total
            if (Lists.Tile[form.scrlTile.Value].Data[x, y].Attribute == (byte)TileAttributes.Block)
            {
                Editor_TileAttributes(x, y);
                return;
            }

            for (byte i = 0; i < (byte)Directions.Count; i++)
            {
                // Estado do bloqueio
                sourceY = Lists.Tile[form.scrlTile.Value].Data[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;

                // Renderiza
                Render(WinTile, TexDirections, x * Grid + Block_Position(i).X, y * Grid + Block_Position(i).Y, i * 8, sourceY, 6, 6);
            }
        }
        #endregion

        #region Item Editor
        private static void Editor_Item()
        {
            // Somente se necessário
            if (WinItem == null) return;

            // Desenha o item
            short textureNum = (short)EditorItems.Form.numTexture.Value;
            WinItem.Clear();
            Transparent(WinItem);
            if (textureNum > 0 && textureNum < TexItem.Length) Render(WinItem, TexItem[textureNum], new Point(0));
            WinItem.Display();
        }
        #endregion

        #region NPC Editor
        private static void Editor_NPC()
        {
            // Somente se necessário
            if (WinNPC == null) return;

            // Desenha o NPC
            Character(WinNPC, (short)EditorNPCs.Form.numTexture.Value);
        }
        #endregion

        #region Class Editors
        private static void Editor_Class()
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
            Texture texture = TexCharacter[textureNum];
            Size size = new Size(Size(texture).Width / 4, Size(texture).Height / 4);

            // Desenha o item
            window.Clear();
            Transparent(window);
            if (textureNum > 0 && textureNum < TexCharacter.Length) Render(window, texture, (int)(window.Size.X - size.Width) / 2, (int)(window.Size.Y - size.Height) / 2, 0, 0, size.Width, size.Height);
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
            Interface_Order(Lists.Tool.Nodes[(byte)EditorInterface.Form.cmbWindows.SelectedIndex]);
            WinInterface.Display();
        }

        private static void Interface_Order(TreeNode node)
        {
            for (byte i = 0; i < node.Nodes.Count; i++)
            {
                // Desenha a ferramenta
                Tool tool = (Tool)node.Nodes[i].Tag;
                if (tool.Visible)
                {
                    if (tool is Panel) Panel((Panel)tool);
                    else if (tool is TextBox) TextBox((TextBox)tool);
                    else if (tool is Button) Button((Button)tool);
                    else if (tool is CheckBox) CheckBox((CheckBox)tool);

                    // Pula pra próxima
                    Interface_Order(node.Nodes[i]);
                }
            }
        }

        private static void Button(Button tool)
        {
            // Desenha o botão
            if (tool.TextureNum < TexButton.Length)
                Render(WinInterface, TexButton[tool.TextureNum], tool.Position, new Color(255, 255, 225, 225));
        }

        private static void Panel(Panel tool)
        {
            // Desenha o painel
            if (tool.TextureNum < TexPanel.Length)
                Render(WinInterface, TexPanel[tool.TextureNum], tool.Position);
        }

        private static void CheckBox(CheckBox tool)
        {
            // Define as propriedades dos retângulos
            Rectangle recSource = new Rectangle(new Point(), new Size(Size(TexCheckBox).Width / 2, Size(TexCheckBox).Height));
            Rectangle recDestiny = new Rectangle(tool.Position, recSource.Size);

            // Desenha a textura do marcador pelo seu estado 
            if (tool.Checked)
                recSource.Location = new Point(Size(TexCheckBox).Width / 2, 0);

            // Desenha o marcador 
            byte margin = 4;
            Render(WinInterface, TexCheckBox, recSource, recDestiny);
            DrawText(WinInterface, tool.Text, recDestiny.Location.X + Size(TexCheckBox).Width / 2 + margin, recDestiny.Location.Y + 1, Color.White);
        }

        private static void TextBox(TextBox tool)
        {
            // Desenha a ferramenta
            Render_Box(WinInterface, TexTextBox, 3, tool.Position, new Size(tool.Width, Size(TexTextBox).Height));
        }
        #endregion
    }
}