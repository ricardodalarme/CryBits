using SFML.Graphics;
using SFML.System;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

partial class Graphics
{
    // Locais de renderização
    public static RenderWindow Win_Interface;
    public static RenderWindow Win_Tile;
    public static RenderWindow Win_Map;
    public static RenderWindow Win_Map_Tile;
    public static RenderTexture Win_Map_Lighting;
    public static RenderWindow Win_Item;
    public static RenderWindow Win_Class;
    public static RenderWindow Win_NPC;

    // Fonte principal
    public static SFML.Graphics.Font Font_Default;

    // Texturas
    public static Texture[] Tex_Character;
    public static Texture[] Tex_Tile;
    public static Texture[] Tex_Face;
    public static Texture[] Tex_Panel;
    public static Texture[] Tex_Button;
    public static Texture[] Tex_Panorama;
    public static Texture[] Tex_Fog;
    public static Texture[] Tex_Item;
    public static Texture Tex_CheckBox;
    public static Texture Tex_TextBox;
    public static Texture Tex_Grid;
    public static Texture Tex_Weather;
    public static Texture Tex_Blank;
    public static Texture Tex_Directions;
    public static Texture Tex_Transparent;
    public static Texture Tex_Lighting;

    // Formato das texturas
    public const string Format = ".png";

    #region Engine
    private static Texture[] AddTextures(string Directory)
    {
        short i = 1;
        Texture[] TempTex = Array.Empty<Texture>();

        while (File.Exists(Directory + i + Format))
        {
            // Carrega todas do diretório e as adiciona a lista
            Array.Resize(ref TempTex, i + 1);
            TempTex[i] = new Texture(Directory + i + Format);
            i += 1;
        }

        // Retorna o cache da textura
        return TempTex;
    }

    public static Size TSize(Texture Texture)
    {
        // Retorna com o tamanho da textura
        if (Texture != null)
            return new Size((int)Texture.Size.X, (int)Texture.Size.Y);
        else
            return new Size(0, 0);
    }

    // Retorna a cor
    private static SFML.Graphics.Color CColor(byte R = 255, byte G = 255, byte B = 255, byte A = 255) => new SFML.Graphics.Color(R, G, B, A);

    private static void Render(RenderWindow Window, Texture Texture, Rectangle Source, Rectangle Destiny, object Color = null, object Mode = null)
    {
        // Define os dados
        Sprite TmpImage = new Sprite(Texture)
        {
            TextureRect = new IntRect(Source.X, Source.Y, Source.Width, Source.Height),
            Position = new Vector2f(Destiny.X, Destiny.Y),
            Scale = new Vector2f(Destiny.Width / (float)Source.Width, Destiny.Height / (float)Source.Height)
        };
        if (Color != null) TmpImage.Color = (SFML.Graphics.Color)Color;

        // Renderiza a textura em forma de retângulo
        if (Mode == null) Mode = RenderStates.Default;
        Window.Draw(TmpImage, (RenderStates)Mode);
    }

    private static void Render(RenderTexture Window, Texture Texture, Rectangle Destiny, object Color = null, object Mode = null)
    {
        // Define os dados
        Sprite TmpImage = new Sprite(Texture)
        {
            Position = new Vector2f(Destiny.X, Destiny.Y),
            Scale = new Vector2f(Destiny.Width / (float)TSize(Texture).Width, Destiny.Height / (float)TSize(Texture).Height)
        };
        if (Color != null) TmpImage.Color = (SFML.Graphics.Color)Color;

        // Renderiza a textura em forma de retângulo
        if (Mode == null) Mode = RenderStates.Default;
        Window.Draw(TmpImage, (RenderStates)Mode);
    }

    private static void Render(RenderWindow Window, Texture Texture, int X, int Y, int Source_X, int Source_Y, int Source_Width, int Source_Height, object Color = null, object Mode = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(Source_X, Source_Y), new Size(Source_Width, Source_Height));
        Rectangle Destiny = new Rectangle(new Point(X, Y), new Size(Source_Width, Source_Height));

        // Desenha a textura
        Render(Window, Texture, Source, Destiny, Color, Mode);
    }

    private static void Render(RenderWindow Window, Texture Texture, Rectangle Destiny, object Color = null, object Mode = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));

        // Desenha a textura
        Render(Window, Texture, Source, Destiny, Color, Mode);
    }

    private static void Render(RenderWindow Window, Texture Texture, Point Point, object Color = null, object Mode = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));
        Rectangle Destiny = new Rectangle(Point, TSize(Texture));

        // Desenha a textura
        Render(Window, Texture, Source, Destiny, Color, Mode);
    }

    private static void RenderRectangle(RenderWindow Window, Rectangle Rectangle, object Color = null)
    {
        // Desenha a caixa
        Render(Window, Tex_Grid, Rectangle.X, Rectangle.Y, 0, 0, Rectangle.Width, 1, Color);
        Render(Window, Tex_Grid, Rectangle.X, Rectangle.Y, 0, 0, 1, Rectangle.Height, Color);
        Render(Window, Tex_Grid, Rectangle.X, Rectangle.Y + Rectangle.Height - 1, 0, 0, Rectangle.Width, 1, Color);
        Render(Window, Tex_Grid, Rectangle.X + Rectangle.Width - 1, Rectangle.Y, 0, 0, 1, Rectangle.Height, Color);
    }

    private static void RenderRectangle(RenderWindow Window, int x, int y, int Width, int Height, object Color = null)
    {
        // Desenha a caixa
        RenderRectangle(Window, new Rectangle(x, y, Width, Height), Color);
    }

    private static void Render_Box(RenderWindow Window, Texture Texture, byte Margin, Point Position, Size Size)
    {
        int Texture_Width = TSize(Texture).Width;
        int Texture_Height = TSize(Texture).Height;

        // Borda esquerda
        Render(Window, Texture, new Rectangle(new Point(0), new Size(Margin, Texture_Width)), new Rectangle(Position, new Size(Margin, Texture_Height)));
        // Borda direita
        Render(Window, Texture, new Rectangle(new Point(Texture_Width - Margin, 0), new Size(Margin, Texture_Height)), new Rectangle(new Point(Position.X + Size.Width - Margin, Position.Y), new Size(Margin, Texture_Height)));
        // Centro
        Render(Window, Texture, new Rectangle(new Point(Margin, 0), new Size(Margin, Texture_Height)), new Rectangle(new Point(Position.X + Margin, Position.Y), new Size(Size.Width - Margin * 2, Texture_Height)));
    }

    private static void DrawText(RenderWindow Window, string Text, int X, int Y, SFML.Graphics.Color Color)
    {
        Text TempText = new Text(Text, Font_Default);

        // Define os dados
        TempText.CharacterSize = 10;
        TempText.FillColor = Color;
        TempText.Position = new Vector2f(X, Y);
        TempText.OutlineColor = new SFML.Graphics.Color(0, 0, 0, 70);
        TempText.OutlineThickness = 1;

        // Desenha
        Window.Draw(TempText);
    }
    #endregion

    public static void Init()
    {
        // Conjuntos
        Tex_Character = AddTextures(Directories.Tex_Characters.FullName);
        Tex_Tile = AddTextures(Directories.Tex_Tiles.FullName);
        Tex_Face = AddTextures(Directories.Tex_Faces.FullName);
        Tex_Panel = AddTextures(Directories.Tex_Painel.FullName);
        Tex_Button = AddTextures(Directories.Tex_Buttons.FullName);
        Tex_Panorama = AddTextures(Directories.Tex_Panoramas.FullName);
        Tex_Fog = AddTextures(Directories.Tex_Fogs.FullName);
        Tex_Item = AddTextures(Directories.Tex_Items.FullName);

        // Únicas
        Tex_Weather = new Texture(Directories.Tex_Weather.FullName + Format);
        Tex_Blank = new Texture(Directories.Tex_Blanc.FullName + Format);
        Tex_Directions = new Texture(Directories.Tex_Directions.FullName + Format);
        Tex_Transparent = new Texture(Directories.Tex_Transparent.FullName + Format);
        Tex_Grid = new Texture(Directories.Tex_Grid.FullName + Format);
        Tex_CheckBox = new Texture(Directories.Tex_CheckBox.FullName + Format);
        Tex_TextBox = new Texture(Directories.Tex_TextBox.FullName + Format);
        Tex_Lighting = new Texture(Directories.Tex_Lighting.FullName + Format);

        // Fontes
        Font_Default = new SFML.Graphics.Font(Directories.Fonts.FullName + "Georgia.ttf");
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

    private static void Transparent(RenderWindow Window)
    {
        Size Texture_Size = TSize(Tex_Transparent);

        // Desenha uma textura transparente na janela inteira
        for (int x = 0; x <= Window.Size.X / Texture_Size.Width; x++)
            for (int y = 0; y <= Window.Size.Y / Texture_Size.Height; y++)
                Render(Window, Tex_Transparent, new Point(Texture_Size.Width * x, Texture_Size.Height * y));
    }

    #region Map Editor
    private static void Editor_Maps_Tile()
    {
        Editor_Maps Form = Editor_Maps.Form;

        // Somente se necessário
        if (Form == null || Form.IsDisposed || !Form.butMNormal.Checked) return;

        // Reinicia o dispositivo caso haja alguma alteração no tamanho da tela
        if (new Size((int)Win_Map_Tile.Size.X, (int)Win_Map_Tile.Size.Y) != Form.picTile.Size)
        {
            Win_Map_Tile.Dispose();
            Win_Map_Tile = new RenderWindow(Editor_Maps.Form.picTile.Handle);
        }

        // Limpa a área com um fundo preto
        Win_Map_Tile.Clear(SFML.Graphics.Color.Black);

        // Dados
        Texture Texture = Tex_Tile[Form.cmbTiles.SelectedIndex + 1];
        Point Position = new Point(Form.scrlTileX.Value, Form.scrlTileY.Value);

        // Desenha o azulejo e as grades
        Transparent(Win_Map_Tile);
        Render(Win_Map_Tile, Texture, new Rectangle(Position, TSize(Texture)), new Rectangle(new Point(0), TSize(Texture)));
        RenderRectangle(Win_Map_Tile, new Rectangle(new Point(Form.Tile_Source.X - Position.X, Form.Tile_Source.Y - Position.Y), Form.Tile_Source.Size), CColor(165, 42, 42, 250));
        RenderRectangle(Win_Map_Tile, Form.Tile_Mouse.X, Form.Tile_Mouse.Y, Globals.Grid, Globals.Grid, CColor(65, 105, 225, 250));

        // Exibe o que foi renderizado
        Win_Map_Tile.Display();
    }

    private static void Editor_Maps_Map()
    {
        // Previne erros
        if (Editor_Maps.Form == null || Editor_Maps.Form.IsDisposed || Editor_Maps.Form.Selected == null) return;

        // Limpa a área com um fundo preto
        Win_Map.Clear(SFML.Graphics.Color.Black);

        // Desenha o mapa
        Objects.Map Selected = Editor_Maps.Form.Selected;
        Editor_Maps_Map_Panorama(Selected);
        Editor_Maps_Map_Tiles(Selected);
        Editor_Maps_Map_Weather(Selected);
        Editor_Maps_Map_Light(Selected);
        Editor_Maps_Map_Fog(Selected);
        Editor_Maps_Map_Grids(Selected);
        Editor_Maps_Map_NPCs(Selected);

        // Exibe o que foi renderizado
        Win_Map.Display();
    }

    private static void Editor_Maps_Map_Panorama(Objects.Map Map)
    {
        // Desenha o panorama
        if (Editor_Maps.Form.butVisualization.Checked && Map.Panorama > 0)
        {
            Rectangle Destiny = new Rectangle()
            {
                X = Editor_Maps.Form.scrlMapX.Value * -Globals.Grid_Zoom,
                Y = Editor_Maps.Form.scrlMapY.Value * -Globals.Grid_Zoom,
                Size = TSize(Tex_Panorama[Map.Panorama])
            };
            Render(Win_Map, Tex_Panorama[Map.Panorama], Editor_Maps.Form.Zoom(Destiny));
        }
    }

    private static void Editor_Maps_Map_Tiles(Objects.Map Map)
    {
        Editor_Maps Form = Editor_Maps.Form;
        Objects.Map_Tile_Data Data;
        int Begin_X = Form.scrlMapX.Value, Begin_Y = Form.scrlMapY.Value;
        SFML.Graphics.Color Color;

        // Desenha todos os azulejos
        for (byte c = 0; c < Map.Layer.Count; c++)
        {
            // Somente se necessário
            if (!Form.lstLayers.Items[c].Checked) continue;

            // Transparência da camada
            Color = CColor(255, 255, 255);
            if (Form.butEdition.Checked && Form.butMNormal.Checked)
            {
                if (Editor_Maps.Form.lstLayers.SelectedIndices.Count > 0)
                    if (c != Editor_Maps.Form.lstLayers.SelectedItems[0].Index)
                        Color = CColor(255, 255, 255, 150);
            }
            else
                Color = CColor(Map.Color.R, Map.Color.G, Map.Color.B);

            // Continua
            for (int x = Begin_X; x < Globals.Map_Width; x++)
                for (int y = Begin_Y; y < Globals.Map_Height; y++)
                    if (Map.Layer[c].Tile[x, y].Tile > 0)
                    {
                        // Dados
                        Data = Map.Layer[c].Tile[x, y];
                        Rectangle Source = new Rectangle(new Point(Data.X * Globals.Grid, Data.Y * Globals.Grid), Globals.Grid_Size);
                        Rectangle Destiny = new Rectangle(new Point((x - Begin_X) * Globals.Grid, (y - Begin_Y) * Globals.Grid), Globals.Grid_Size);

                        // Desenha o azulejo
                        if (!Map.Layer[c].Tile[x, y].Auto)
                            Render(Win_Map, Tex_Tile[Data.Tile], Source, Form.Zoom(Destiny), Color);
                        else
                            Editor_Maps_AutoTile(Destiny.Location, Data, Color);
                    }
        }
    }

    private static void Editor_Maps_AutoTile(Point Position, Objects.Map_Tile_Data Data, SFML.Graphics.Color Color)
    {
        // Desenha todas as partes do azulejo
        for (byte i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 1: Position.X += 16; break;
                case 2: Position.Y += 16; break;
                case 3: Position.X += 16; Position.Y += 16; break;
            }
            Render(Win_Map, Tex_Tile[Data.Tile], new Rectangle(Data.Mini[i].X, Data.Mini[i].Y, 16, 16), Editor_Maps.Form.Zoom(new Rectangle(Position, new Size(16, 16))), Color);
        }
    }

    private static void Editor_Maps_Map_Fog(Objects.Map Map)
    {
        // Somente se necessário
        if (Map.Fog.Texture <= 0) return;
        if (!Editor_Maps.Form.butVisualization.Checked) return;

        // Desenha a fumaça
        Size Texture_Size = TSize(Tex_Fog[Map.Fog.Texture]);
        for (int x = -1; x <= Globals.Map_Width * Globals.Grid / Texture_Size.Width; x++)
            for (int y = -1; y <= Globals.Map_Height * Globals.Grid / Texture_Size.Height; y++)
            {
                Point Position = new Point(x * Texture_Size.Width + Globals.Fog_X, y * Texture_Size.Height + Globals.Fog_Y);
                Render(Win_Map, Tex_Fog[Map.Fog.Texture], Editor_Maps.Form.Zoom(new Rectangle(Position, Texture_Size)), CColor(255, 255, 255, Map.Fog.Alpha));
            }
    }

    private static void Editor_Maps_Map_Weather(Objects.Map Map)
    {
        // Somente se necessário
        if (!Editor_Maps.Form.butVisualization.Checked || Map.Weather.Type == Globals.Weathers.Normal) return;

        // Dados
        byte x = 0;
        if (Map.Weather.Type == Globals.Weathers.Snowing) x = 32;

        // Desenha as partículas
        for (int i = 0; i < Lists.Weather.Length; i++)
            if (Lists.Weather[i].Visible)
                Render(Win_Map, Tex_Weather, new Rectangle(x, 0, 32, 32), Editor_Maps.Form.Zoom(new Rectangle(Lists.Weather[i].x, Lists.Weather[i].y, 32, 32)), CColor(255, 255, 255, 150));
    }

    private static void Editor_Maps_Map_Light(Objects.Map Map)
    {
        Editor_Maps Form = Editor_Maps.Form;
        byte Light = (byte)((255 * ((decimal)Map.Lighting / 100) - 255) * -1);

        // Somente se necessário
        if (!Editor_Maps.Form.butVisualization.Checked) return;

        // Escuridão
        Win_Map_Lighting.Clear(CColor(0, 0, 0, Light));

        // Desenha o ponto iluminado
        if (Map.Light.Count > 0)
            for (byte i = 0; i < Map.Light.Count; i++)
                Render(Win_Map_Lighting, Tex_Lighting, Form.Zoom_Grid(Map.Light[i].Rec), null, new RenderStates(BlendMode.Multiply));

        // Pré visualização
        if (Editor_Maps.Form.butMLighting.Checked)
            Render(Win_Map_Lighting, Tex_Lighting, Form.Zoom_Grid(Form.Map_Selection), null, new RenderStates(BlendMode.Multiply));

        // Apresenta o que foi renderizado
        Win_Map_Lighting.Display();
        Win_Map.Draw(new Sprite(Win_Map_Lighting.Texture));

        // Ponto de remoção da luz
        if (Form.butMLighting.Checked)
            if (Map.Light.Count > 0)
                for (byte i = 0; i < Map.Light.Count; i++)
                    RenderRectangle(Win_Map, Form.Zoom_Grid(new Rectangle(Map.Light[i].Rec.X, Map.Light[i].Rec.Y, 1, 1)), CColor(175, 42, 42, 175));

        // Trovoadas
        Render(Win_Map, Tex_Blank, 0, 0, 0, 0, Form.picMap.Width, Form.picMap.Height, CColor(255, 255, 255, Globals.Lightning));
    }

    private static void Editor_Maps_Map_Grids(Objects.Map Map)
    {
        Editor_Maps Form = Editor_Maps.Form;
        Rectangle Source = Form.Tile_Source, Destiny = new Rectangle();
        Point Begin = new Point(Form.Map_Selection.X - Form.scrlMapX.Value, Form.Map_Selection.Y - Form.scrlMapY.Value);

        // Dados
        Destiny.Location = Globals.Zoom(Begin.X, Begin.Y);
        Destiny.Size = new Size(Source.Width / Form.Zoom(), Source.Height / Form.Zoom());

        // Desenha as grades
        if (Form.butGrid.Checked || !Form.butGrid.Enabled)
            for (byte x = 0; x < Globals.Map_Width; x++)
                for (byte y = 0; y < Globals.Map_Height; y++)
                {
                    RenderRectangle(Win_Map, x * Globals.Grid_Zoom, y * Globals.Grid_Zoom, Globals.Grid_Zoom, Globals.Grid_Zoom, CColor(25, 25, 25, 70));
                    Editor_Maps_Map_Zones(Map, x, y);
                    Editor_Maps_Map_Attributes(Map, x, y);
                    Editor_Maps_Map_DirBlock(Map, x, y);
                }

        if (!Form.chkAuto.Checked && Form.butMNormal.Checked)
            // Normal
            if (Form.butPencil.Checked)
                Render(Win_Map, Tex_Tile[Form.cmbTiles.SelectedIndex + 1], Source, Destiny);
            // Retângulo
            else if (Form.butRectangle.Checked)
                for (int x = Begin.X; x < Begin.X + Form.Map_Selection.Width; x++)
                    for (int y = Begin.Y; y < Begin.Y + Form.Map_Selection.Height; y++)
                        Render(Win_Map, Tex_Tile[Form.cmbTiles.SelectedIndex + 1], Source, new Rectangle(Globals.Zoom(x, y), Destiny.Size));

        // Desenha a grade
        if (!Form.butMAttributes.Checked || !Form.optA_DirBlock.Checked)
            RenderRectangle(Win_Map, Destiny.X, Destiny.Y, Form.Map_Selection.Width * Globals.Grid_Zoom, Form.Map_Selection.Height * Globals.Grid_Zoom);
    }

    private static void Editor_Maps_Map_Zones(Objects.Map Map, byte x, byte y)
    {
        Point Position = new Point((x - Editor_Maps.Form.scrlMapX.Value) * Globals.Grid_Zoom, (y - Editor_Maps.Form.scrlMapY.Value) * Globals.Grid_Zoom);
        byte Zone_Num = Map.Attribute[x, y].Zone;
        SFML.Graphics.Color Color;

        // Apenas se necessário
        if (!Editor_Maps.Form.butMZones.Checked) return;
        if (Zone_Num == 0) return;

        // Define a cor
        if (Zone_Num % 2 == 0)
            Color = CColor((byte)((Zone_Num * 42) ^ 3), (byte)(Zone_Num * 22), (byte)(Zone_Num * 33), 150);
        else
            Color = CColor((byte)(Zone_Num * 33), (byte)(Zone_Num * 22), (byte)(Zone_Num * 42), 150 ^ 3);

        // Desenha as zonas
        Render(Win_Map, Tex_Blank, new Rectangle(Position, new Size(Globals.Grid_Zoom, Globals.Grid_Zoom)), Color);
        DrawText(Win_Map, Zone_Num.ToString(), Position.X, Position.Y, SFML.Graphics.Color.White);
    }

    private static void Editor_Maps_Map_Attributes(Objects.Map Map, byte x, byte y)
    {
        Point Position = new Point((x - Editor_Maps.Form.scrlMapX.Value) * Globals.Grid_Zoom, (y - Editor_Maps.Form.scrlMapY.Value) * Globals.Grid_Zoom);
        Globals.Tile_Attributes Attribute = (Globals.Tile_Attributes)Map.Attribute[x, y].Type;
        SFML.Graphics.Color Color;
        string Letter;

        // Apenas se necessário
        if (!Editor_Maps.Form.butMAttributes.Checked) return;
        if (Editor_Maps.Form.optA_DirBlock.Checked) return;
        if (Attribute == Globals.Tile_Attributes.None) return;

        // Define a cor e a letra
        switch (Attribute)
        {
            case Globals.Tile_Attributes.Block: Letter = "B"; Color = SFML.Graphics.Color.Red; break;
            case Globals.Tile_Attributes.Warp: Letter = "T"; Color = SFML.Graphics.Color.Blue; break;
            case Globals.Tile_Attributes.Item: Letter = "I"; Color = SFML.Graphics.Color.Green; break;
            default: return;
        }
        Color = new SFML.Graphics.Color(Color.R, Color.G, Color.B, 100);

        // Desenha as Atributos
        Render(Win_Map, Tex_Blank, new Rectangle(Position, new Size(Globals.Grid_Zoom, Globals.Grid_Zoom)), Color);
        DrawText(Win_Map, Letter, Position.X, Position.Y, SFML.Graphics.Color.White);
    }

    private static void Editor_Maps_Map_DirBlock(Objects.Map Map, byte x, byte y)
    {
        Point Tile = new Point(Editor_Maps.Form.scrlMapX.Value + x, Editor_Maps.Form.scrlMapY.Value + y);
        byte Y;

        // Apenas se necessário
        if (!Editor_Maps.Form.butMAttributes.Checked) return;
        if (!Editor_Maps.Form.optA_DirBlock.Checked) return;

        // Previne erros
        if (Tile.X > Map.Attribute.GetUpperBound(0)) return;
        if (Tile.Y > Map.Attribute.GetUpperBound(1)) return;

        for (byte i = 0; i < (byte)Globals.Directions.Count; i++)
        {
            // Estado do bloqueio
            if (Map.Attribute[Tile.X, Tile.Y].Block[i])
                Y = 8;
            else
                Y = 0;

            // Renderiza
            Render(Win_Map, Tex_Directions, x * Globals.Grid + Globals.Block_Position(i).X, y * Globals.Grid + Globals.Block_Position(i).Y, i * 8, Y, 6, 6);
        }
    }

    private static void Editor_Maps_Map_NPCs(Objects.Map Map)
    {
        if (Editor_Maps.Form.butMNPCs.Checked)
            for (byte i = 0; i < Map.NPC.Count; i++)
                if (Map.NPC[i].Spawn)
                {
                    Point Position = new Point((Map.NPC[i].X - Editor_Maps.Form.scrlMapX.Value) * Globals.Grid_Zoom, (Map.NPC[i].Y - Editor_Maps.Form.scrlMapY.Value) * Globals.Grid_Zoom);

                    // Desenha uma sinalização de onde os NPCs estão
                    Render(Win_Map, Tex_Blank, new Rectangle(Position, new Size(Globals.Grid_Zoom, Globals.Grid_Zoom)), CColor(0, 220, 0, 150));
                    DrawText(Win_Map, (i + 1).ToString(), Position.X + 10, Position.Y + 10, SFML.Graphics.Color.White);
                }
    }
    #endregion

    #region Tile Editor
    public static void Editor_Tile()
    {
        Editor_Tiles Form = Editor_Tiles.Form;

        // Somente se necessário
        if (Form == null) return;

        // Limpa a tela e desenha um fundo transparente
        Win_Tile.Clear();
        Transparent(Win_Tile);

        // Desenha o azulejo e as grades
        Texture Texture = Tex_Tile[Form.scrlTile.Value];
        Point Position = new Point(Form.scrlTileX.Value * Globals.Grid, Form.scrlTileY.Value * Globals.Grid);
        Render(Win_Tile, Texture, new Rectangle(Position, TSize(Texture)), new Rectangle(new Point(0), TSize(Texture)));

        for (byte x = 0; x <= Form.picTile.Width / Globals.Grid; x++)
            for (byte y = 0; y <= Form.picTile.Height / Globals.Grid; y++)
            {
                // Desenha os atributos
                if (Form.optAttributes.Checked)
                    Editor_Tile_Attributes(x, y);
                // Bloqueios direcionais
                else if (Form.optDirBlock.Checked)
                    Editor_Tile_DirBlock(x, y);

                // Grades
                RenderRectangle(Win_Tile, x * Globals.Grid, y * Globals.Grid, Globals.Grid, Globals.Grid, CColor(25, 25, 25, 70));
            }

        // Exibe o que foi renderizado
        Win_Tile.Display();
    }

    private static void Editor_Tile_Attributes(byte x, byte y)
    {
        Editor_Tiles Form = Editor_Tiles.Form;
        Point Tile = new Point(Form.scrlTileX.Value + x, Form.scrlTileY.Value + y);
        Point Point = new Point(x * Globals.Grid + Globals.Grid / 2 - 5, y * Globals.Grid + Globals.Grid / 2 - 6);

        // Previne erros
        if (Tile.X > Lists.Tile[Form.scrlTile.Value].Data.GetUpperBound(0)) return;
        if (Tile.Y > Lists.Tile[Form.scrlTile.Value].Data.GetUpperBound(1)) return;

        // Desenha uma letra e colore o azulejo referente ao atributo
        switch ((Globals.Tile_Attributes)Lists.Tile[Form.scrlTile.Value].Data[Tile.X, Tile.Y].Attribute)
        {
            case Globals.Tile_Attributes.Block:
                Render(Win_Tile, Tex_Blank, x * Globals.Grid, y * Globals.Grid, 0, 0, Globals.Grid, Globals.Grid, CColor(225, 0, 0, 75));
                DrawText(Win_Tile, "B", Point.X, Point.Y, SFML.Graphics.Color.Red);
                break;
        }
    }

    private static void Editor_Tile_DirBlock(byte x, byte y)
    {
        Editor_Tiles Form = Editor_Tiles.Form;
        Point Tile = new Point(Form.scrlTileX.Value + x, Form.scrlTileY.Value + y);
        byte Y;

        // Previne erros
        if (Tile.X > Lists.Tile[Form.scrlTile.Value].Data.GetUpperBound(0)) return;
        if (Tile.Y > Lists.Tile[Form.scrlTile.Value].Data.GetUpperBound(1)) return;

        // Bloqueio total
        if (Lists.Tile[Form.scrlTile.Value].Data[x, y].Attribute == (byte)Globals.Tile_Attributes.Block)
        {
            Editor_Tile_Attributes(x, y);
            return;
        }

        for (byte i = 0; i < (byte)Globals.Directions.Count; i++)
        {
            // Estado do bloqueio
            if (Lists.Tile[Form.scrlTile.Value].Data[Tile.X, Tile.Y].Block[i])
                Y = 8;
            else
                Y = 0;

            // Renderiza
            Render(Win_Tile, Tex_Directions, x * Globals.Grid + Globals.Block_Position(i).X, y * Globals.Grid + Globals.Block_Position(i).Y, i * 8, Y, 6, 6);
        }
    }
    #endregion

    #region Item Editor
    private static void Editor_Item()
    {
        // Somente se necessário
        if (Editor_Items.Form == null) return;

        // Desenha o item
        short Texture_Num = (short)Editor_Items.Form.numTexture.Value;
        Win_Item.Clear();
        Transparent(Win_Item);
        if (Texture_Num > 0 && Texture_Num < Tex_Item.Length) Render(Win_Item, Tex_Item[Texture_Num], new Point(0));
        Win_Item.Display();
    }
    #endregion

    #region NPC Editor
    private static void Editor_NPC()
    {
        // Somente se necessário
        if (Editor_NPCs.Form == null) return;

        // Desenha o NPC
        Character(Win_NPC, (short)Editor_NPCs.Form.numTexture.Value);
    }
    #endregion

    #region Class Editor
    private static void Editor_Class()
    {
        // Somente se necessário
        if (Editor_Classes.Form == null) return;

        // Desenha o NPC
        Character(Win_Class, (short)Editor_Classes.Form.numTexture.Value);
    }
    #endregion

    #region Character
    private static void Character(RenderWindow Window, short Texture_Num)
    {
        Texture Texture = Tex_Character[Texture_Num];
        Size Size = new Size(TSize(Texture).Width / 4, TSize(Texture).Height / 4);

        // Desenha o item
        Window.Clear();
        Transparent(Window);
        if (Texture_Num > 0 && Texture_Num < Tex_Character.Length) Render(Window, Texture, (int)(Window.Size.X - Size.Width) / 2, (int)(Window.Size.Y - Size.Height) / 2, 0, 0, Size.Width, Size.Height);
        Window.Display();
    }
    #endregion

    #region Interface Editor
    public static void Interface()
    {
        // Apenas se necessário
        if (Editor_Interface.Form == null) return;

        // Desenha as ferramentas
        Win_Interface.Clear();
        Interface_Order(Lists.Tool.Nodes[(byte)Editor_Interface.Form.cmbWindows.SelectedIndex]);
        Win_Interface.Display();
    }

    private static void Interface_Order(TreeNode Node)
    {
        for (byte i = 0; i < Node.Nodes.Count; i++)
        {
            // Desenha a ferramenta
            Objects.Tool Tool = (Objects.Tool)Node.Nodes[i].Tag;
            if (Tool.Visible)
            {
                if (Tool is Objects.Panel) Panel((Objects.Panel)Tool);
                else if (Tool is Objects.TextBox) TextBox((Objects.TextBox)Tool);
                else if (Tool is Objects.Button) Button((Objects.Button)Tool);
                else if (Tool is Objects.CheckBox) CheckBox((Objects.CheckBox)Tool);

                // Pula pra próxima
                Interface_Order(Node.Nodes[i]);
            }
        }
    }

    private static void Button(Objects.Button Tool)
    {
        // Desenha o botão
        if (Tool.Texture_Num < Tex_Button.Length)
            Render(Win_Interface, Tex_Button[Tool.Texture_Num], Tool.Position, new SFML.Graphics.Color(255, 255, 225, 225));
    }

    private static void Panel(Objects.Panel Tool)
    {
        // Desenha o painel
        if (Tool.Texture_Num < Tex_Panel.Length)
            Render(Win_Interface, Tex_Panel[Tool.Texture_Num], Tool.Position);
    }

    private static void CheckBox(Objects.CheckBox Tool)
    {
        // Define as propriedades dos retângulos
        Rectangle Rec_Source = new Rectangle(new Point(), new Size(TSize(Tex_CheckBox).Width / 2, TSize(Tex_CheckBox).Height));
        Rectangle Rec_Destiny = new Rectangle(Tool.Position, Rec_Source.Size);

        // Desenha a textura do marcador pelo seu estado 
        if (Tool.Checked)
            Rec_Source.Location = new Point(TSize(Tex_CheckBox).Width / 2, 0);

        // Desenha o marcador 
        byte Margin = 4;
        Render(Win_Interface, Tex_CheckBox, Rec_Source, Rec_Destiny);
        DrawText(Win_Interface, Tool.Text, Rec_Destiny.Location.X + TSize(Tex_CheckBox).Width / 2 + Margin, Rec_Destiny.Location.Y + 1, SFML.Graphics.Color.White);
    }

    private static void TextBox(Objects.TextBox Tool)
    {
        // Desenha a ferramenta
        Render_Box(Win_Interface, Tex_TextBox, 3, Tool.Position, new Size(Tool.Width, TSize(Tex_TextBox).Height));
    }
    #endregion
}