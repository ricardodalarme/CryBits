using SFML.Graphics;
using SFML.Window;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

partial class Graphics
{
    // Locais de renderização
    public static RenderWindow Win_Preview;
    public static RenderWindow Win_Interface = new RenderWindow(Editor_Interface.Objects.picWindow.Handle);
    public static RenderWindow Win_Tile = new RenderWindow(Editor_Tiles.Objects.picTile.Handle);
    public static RenderWindow Win_Map = new RenderWindow(Editor_Maps.Objects.picMap.Handle);
    public static RenderWindow Win_Map_Tile = new RenderWindow(Editor_Maps.Objects.picTile.Handle);
    public static RenderTexture Win_Map_Lighting = new RenderTexture((uint)Editor_Maps.Objects.Width, (uint)Editor_Maps.Objects.Height);

    // Fonte principal
    public static SFML.Graphics.Font GameFont;

    // Texturas
    public static Texture[] Tex_Character;
    public static Texture[] Tex_Tile;
    public static Texture[] Tex_Face;
    public static Texture[] Tex_Panel;
    public static Texture[] Tex_Button;
    public static Texture[] Tex_Panorama;
    public static Texture[] Tex_Fog;
    public static Texture[] Tex_Light;
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
    public static Texture[] AddTextures(string Directory)
    {
        short i = 1;
        Texture[] TempTex = new Texture[0];

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

    public static SFML.Graphics.Color CColor(byte R = 255, byte G = 255, byte B = 255, byte A = 255)
    {
        // Retorna com a cor
        return new SFML.Graphics.Color(R, G, B, A);
    }

    public static void Render(RenderWindow Window, Texture Texture, Rectangle Source, Rectangle Destiny, object Color = null, object Mode = null)
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

    public static void Render(RenderTexture Window, Texture Texture, Rectangle Destiny, object Color = null, object Mode = null)
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

    public static void Render(RenderWindow Window, Texture Texture, int X, int Y, int Source_X, int Source_Y, int Source_Width, int Source_Height, object Color = null, object Mode = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(Source_X, Source_Y), new Size(Source_Width, Source_Height));
        Rectangle Destiny = new Rectangle(new Point(X, Y), new Size(Source_Width, Source_Height));

        // Desenha a textura
        Render(Window, Texture, Source, Destiny, Color, Mode);
    }

    public static void Render(RenderWindow Window, Texture Texture, Rectangle Destiny, object Color = null, object Mode = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));

        // Desenha a textura
        Render(Window, Texture, Source, Destiny, Color, Mode);
    }

    public static void Render(RenderWindow Window, Texture Texture, Point Point, object Color = null, object Mode = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));
        Rectangle Destiny = new Rectangle(Point, TSize(Texture));

        // Desenha a textura
        Render(Window, Texture, Source, Destiny, Color, Mode);
    }

    public static void RenderRectangle(RenderWindow Window, Rectangle Rectangle, object Color = null)
    {
        // Desenha a caixa
        Render(Window, Tex_Grid, Rectangle.X, Rectangle.Y, 0, 0, Rectangle.Width, 1, Color);
        Render(Window, Tex_Grid, Rectangle.X, Rectangle.Y, 0, 0, 1, Rectangle.Height, Color);
        Render(Window, Tex_Grid, Rectangle.X, Rectangle.Y + Rectangle.Height - 1, 0, 0, Rectangle.Width, 1, Color);
        Render(Window, Tex_Grid, Rectangle.X + Rectangle.Width - 1, Rectangle.Y, 0, 0, 1, Rectangle.Height, Color);
    }

    public static void RenderRectangle(RenderWindow Window, int x, int y, int Width, int Height, object Color = null)
    {
        // Desenha a caixa
        RenderRectangle(Window, new Rectangle(x, y, Width, Height), Color);
    }

    public static void Render_Box(RenderWindow Window, Texture Texture, byte Margin, Point Position, Size Size)
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
        // Define os dados
        Text TempText = new Text(Text, GameFont)
        {
            CharacterSize = 10,
            Color = Color,
            Position = new Vector2f(X, Y)
        };

        // Desenha
        Window.Draw(TempText);
    }
    #endregion

    public static void LoadTextures()
    {
        // Conjuntos
        Tex_Character = AddTextures(Directories.Tex_Characters.FullName);
        Tex_Tile = AddTextures(Directories.Tex_Tiles.FullName);
        Tex_Face = AddTextures(Directories.Tex_Faces.FullName);
        Tex_Panel = AddTextures(Directories.Tex_Painel.FullName);
        Tex_Button = AddTextures(Directories.Tex_Buttons.FullName);
        Tex_Panorama = AddTextures(Directories.Tex_Panoramas.FullName);
        Tex_Fog = AddTextures(Directories.Tex_Fogs.FullName);
        Tex_Light = AddTextures(Directories.Tex_Lights.FullName);
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
        GameFont = new SFML.Graphics.Font(Directories.Fonts.FullName + "Georgia.ttf");
    }

    public static void Present()
    {
        // Desenha 
        Preview_Image();
        Editor_Tile();
        Editor_Maps_Tile();
        Editor_Maps_Map();
        Interface();
    }

    public static void Transparent(RenderWindow Window)
    {
        Size Tamanho = TSize(Tex_Transparent);

        // Desenha uma textura transparente na janela inteira
        for (int x = 0; x <= Window.Size.X / Tamanho.Width; x++)
            for (int y = 0; y <= Window.Size.Y / Tamanho.Height; y++)
                Render(Window, Tex_Transparent, new Point(Tamanho.Width * x, Tamanho.Height * y));
    }

    #region Preview
    public static void Preview_Image()
    {
        Preview Objects = Preview.Objects;

        // Apenas se necessário
        if (!Objects.Visible) return;
        if (Objects.List.SelectedIndex < 0) return;

        // Dados
        Texture Texture = Preview.Texture[Objects.List.SelectedIndex];

        // Limpa a área
        Win_Preview.Clear();

        // Desenha a textura
        if (Objects.chkTransparent.Checked) Transparent(Win_Preview);
        if (Objects.List.SelectedIndex > 0) Render(Win_Preview, Texture, new Rectangle(new Point(Objects.scrlImageX.Value, Objects.scrlImageY.Value), TSize(Texture)), new Rectangle(new Point(0), TSize(Texture)));

        // Exibe o que foi renderizado
        Win_Preview.Display();
    }
    #endregion

    #region Tile Editor
    public static void Editor_Tile()
    {
        Editor_Tiles Objects = Editor_Tiles.Objects;

        // Somente se necessário
        if (!Objects.Visible) return;

        // Limpa a área com um fundo preto
        Win_Tile.Clear(SFML.Graphics.Color.Black);

        // Dados
        Texture Texture = Tex_Tile[Objects.scrlTile.Value];
        Size Size = TSize(Texture);
        Point Point = new Point(Objects.scrlTileX.Value * Globals.Grid, Objects.scrlTileY.Value * Globals.Grid);

        // Desenha o azulejo e as grades
        Transparent(Win_Tile);
        Render(Win_Tile, Texture, new Rectangle(Point, Size), new Rectangle(new Point(0), Size));

        for (byte x = 0; x <= Objects.picTile.Width / Globals.Grid; x++)
            for (byte y = 0; y <= Objects.picTile.Height / Globals.Grid; y++)
            {
                // Desenha os atributos
                if (Objects.optAttributes.Checked)
                    Editor_Tile_Attributes(x, y);
                // Bloqueios direcionais
                else if (Objects.optDirBlock.Checked)
                    Editor_Tile_DirBlock(x, y);

                // Grades
                RenderRectangle(Win_Tile, x * Globals.Grid, y * Globals.Grid, Globals.Grid, Globals.Grid, CColor(25, 25, 25, 70));
            }

        // Exibe o que foi renderizado
        Win_Tile.Display();
    }

    public static void Editor_Tile_Attributes(byte x, byte y)
    {
        Editor_Tiles Objects = Editor_Tiles.Objects;
        Point Tile = new Point(Objects.scrlTileX.Value + x, Objects.scrlTileY.Value + y);
        Point Point = new Point(x * Globals.Grid + Globals.Grid / 2 - 5, y * Globals.Grid + Globals.Grid / 2 - 6);

        // Previne erros
        if (Tile.X > Lists.Tile[Objects.scrlTile.Value].Data.GetUpperBound(0)) return;
        if (Tile.Y > Lists.Tile[Objects.scrlTile.Value].Data.GetUpperBound(1)) return;

        // Desenha uma letra e colore o azulejo referente ao atributo
        switch ((Globals.Tile_Attributes)Lists.Tile[Objects.scrlTile.Value].Data[Tile.X, Tile.Y].Attribute)
        {
            case Globals.Tile_Attributes.Block:
                Render(Win_Tile, Tex_Blank, x * Globals.Grid, y * Globals.Grid, 0, 0, Globals.Grid, Globals.Grid, CColor(225, 0, 0, 75));
                DrawText(Win_Tile, "B", Point.X, Point.Y, SFML.Graphics.Color.Red);
                break;
        }
    }

    public static void Editor_Tile_DirBlock(byte x, byte y)
    {
        Editor_Tiles Objects = Editor_Tiles.Objects;
        Point Tile = new Point(Objects.scrlTileX.Value + x, Objects.scrlTileY.Value + y);
        byte Y;

        // Previne erros
        if (Tile.X > Lists.Tile[Objects.scrlTile.Value].Data.GetUpperBound(0)) return;
        if (Tile.Y > Lists.Tile[Objects.scrlTile.Value].Data.GetUpperBound(1)) return;

        for (byte i = 0; i < (byte)Globals.Directions.Count; i++)
        {
            // Estado do bloqueio
            if (Lists.Tile[Objects.scrlTile.Value].Data[Tile.X, Tile.Y].Block[i])
                Y = 8;
            else
                Y = 0;

            // Renderiza
            Render(Win_Tile, Tex_Directions, x * Globals.Grid + Globals.Block_Position(i).X, y * Globals.Grid + Globals.Block_Position(i).Y, i * 8, Y, 6, 6);
        }
    }
    #endregion

    #region Map Editor
    public static void Editor_Maps_Tile()
    {
        Editor_Maps Objects = Editor_Maps.Objects;

        // Somente se necessário
        if (!Objects.Visible || !Editor_Maps.Objects.butMNormal.Checked) return;

        // Reinicia o dispositivo caso haja alguma alteração no tamanho da tela
        if (new Size((int)Win_Map_Tile.Size.X, (int)Win_Map_Tile.Size.Y) != Objects.picTile.Size)
        {
            Win_Map_Tile.Dispose();
            Win_Map_Tile = new RenderWindow(Editor_Maps.Objects.picTile.Handle);
        }

        // Limpa a área com um fundo preto
        Win_Map_Tile.Clear(SFML.Graphics.Color.Black);

        // Dados
        Texture Texture = Tex_Tile[Objects.cmbTiles.SelectedIndex + 1];
        Size Size = TSize(Texture);
        Point Position = new Point(Objects.scrlTileX.Value, Objects.scrlTileY.Value);

        // Desenha o azulejo e as grades
        Transparent(Win_Map_Tile);
        Render(Win_Map_Tile, Texture, new Rectangle(Position, Size), new Rectangle(new Point(0), Size));
        RenderRectangle(Win_Map_Tile, new Rectangle(new Point(Editor_Maps.Tile_Source.X - Position.X, Editor_Maps.Tile_Source.Y - Position.Y), Editor_Maps.Tile_Source.Size), CColor(165, 42, 42, 250));
        RenderRectangle(Win_Map_Tile, Editor_Maps.Tile_Mouse.X, Editor_Maps.Tile_Mouse.Y, Globals.Grid, Globals.Grid, CColor(65, 105, 225, 250));

        // Exibe o que foi renderizado
        Win_Map_Tile.Display();
    }

    public static void Editor_Maps_Map()
    {
        short Index = Editor_Maps.Selected;

        // Previne erros
        if (!Editor_Maps.Objects.Visible) return;
        if (Lists.Map.GetUpperBound(0) <= 0) return;

        // Limpa a área com um fundo preto
        Win_Map.Clear(SFML.Graphics.Color.Black);

        // Desenha o mapa
        Editor_Maps_Map_Panorama(Index);
        Editor_Maps_Map_Tiles(Index);
        Editor_Maps_Map_Weather(Index);
        Editor_Maps_Map_Light(Index);
        Editor_Maps_Map_Fog(Index);
        Editor_Maps_Map_Grids(Index);
        Editor_Maps_Map_NPCs(Index);

        // Exibe o que foi renderizado
        Win_Map.Display();
    }

    public static void Editor_Maps_Map_Panorama(short Index)
    {
        short Texture = Lists.Map[Index].Panorama;
        Size Size = TSize(Tex_Panorama[Texture]);

        // Limitações
        Size TempSize = new Size
        {
            Width = (Lists.Map[Index].Width + 1 - Editor_Maps.Objects.scrlMapX.Value) * Globals.Grid_Zoom,
            Height = (Lists.Map[Index].Height + 1 - Editor_Maps.Objects.scrlMapY.Value) * Globals.Grid_Zoom
        };

        // Não permite que o tamanho ultrapasse a tela de jogo
        if (Size.Width > TempSize.Width) Size.Width = TempSize.Width;
        if (Size.Height > TempSize.Height) Size.Height = TempSize.Height;

        // Desenha o panorama
        if (Editor_Maps.Objects.butVisualization.Checked && Lists.Map[Index].Panorama > 0)
            Render(Win_Map, Tex_Panorama[Texture], Editor_Maps.Zoom(new Rectangle(new Point(0), Size)));
    }

    public static void Editor_Maps_Map_Tiles(short Index)
    {
        Editor_Maps Objects = Editor_Maps.Objects;
        Lists.Structures.Map Map = Lists.Map[Index];
        Lists.Structures.Map_Tile_Data Data;
        int Begin_X = Objects.scrlMapX.Value, Begin_Y = Objects.scrlMapY.Value;
        SFML.Graphics.Color Color; System.Drawing.Color TempCor = System.Drawing.Color.FromArgb(Map.Color);

        // Desenha todos os azulejos
        for (byte c = 0; c < Map.Layer.Count; c++)
        {
            // Somente se necessário
            if (!Objects.lstLayers.Items[c].Checked) continue;

            // Transparência da camada
            Color = CColor(255, 255, 255);
            if (Objects.butEdition.Checked && Objects.butMNormal.Checked)
            {
                if (Editor_Maps.Objects.lstLayers.SelectedIndices.Count > 0)
                    if (c != Editor_Maps.Objects.lstLayers.SelectedItems[0].Index)
                        Color = CColor(255, 255, 255, 150);
            }
            else
                Color = CColor(TempCor.R, TempCor.G, TempCor.B);

            // Continua
            for (int x = Begin_X; x <= Editor_Maps.Map_Size.Width; x++)
                for (int y = Begin_Y; y <= Editor_Maps.Map_Size.Height; y++)
                    if (Map.Layer[c].Tile[x, y].Tile > 0)
                    {
                        // Dados
                        Data = Map.Layer[c].Tile[x, y];
                        Rectangle Source = new Rectangle(new Point(Data.X * Globals.Grid, Data.Y * Globals.Grid), Globals.Grid_Size);
                        Rectangle Destiny = new Rectangle(new Point((x - Begin_X) * Globals.Grid, (y - Begin_Y) * Globals.Grid), Globals.Grid_Size);

                        // Desenha o azulejo
                        if (!Map.Layer[c].Tile[x, y].Auto)
                            Render(Win_Map, Tex_Tile[Data.Tile], Source, Editor_Maps.Zoom(Destiny), Color);
                        else
                            Editor_Maps_AutoTile(Destiny.Location, Data, Color);
                    }
        }
    }

    public static void Editor_Maps_AutoTile(Point Position, Lists.Structures.Map_Tile_Data Data, SFML.Graphics.Color Color)
    {
        // Desenha os 4 mini azulejos
        for (byte i = 0; i <= 3; i++)
        {
            Point Destiny = Position, Source = Data.Mini[i];

            // Partes do azulejo
            switch (i)
            {
                case 1: Destiny.X += 16; break;
                case 2: Destiny.Y += 16; break;
                case 3: Destiny.X += 16; Destiny.Y += 16; break;
            }

            // Renderiza o mini azulejo
            Render(Win_Map, Tex_Tile[Data.Tile], new Rectangle(Source.X, Source.Y, 16, 16), Editor_Maps.Zoom(new Rectangle(Destiny, new Size(16, 16))), Color);
        }
    }

    public static void Editor_Maps_Map_Fog(short Index)
    {
        Lists.Structures.Map_Fog Data = Lists.Map[Index].Fog;
        Point Position;

        // Somente se necessário
        if (Data.Texture <= 0) return;
        if (!Editor_Maps.Objects.butVisualization.Checked) return;

        // Dados
        Size Textura_Tamanho = TSize(Tex_Fog[Data.Texture]);
        for (int x = -1; x <= Editor_Maps.Map_Size.Width * Globals.Grid / Textura_Tamanho.Width + 1; x++)
            for (int y = -1; y <= Editor_Maps.Map_Size.Height * Globals.Grid / Textura_Tamanho.Height + 1; y++)
            {
                // Desenha a fumaça
                Position = new Point(x * Textura_Tamanho.Width + Globals.Fog_X, y * Textura_Tamanho.Height + Globals.Fog_Y);
                Render(Win_Map, Tex_Fog[Data.Texture], Editor_Maps.Zoom(new Rectangle(Position, Textura_Tamanho)), CColor(255, 255, 255, Data.Alpha));
            }
    }

    public static void Editor_Maps_Map_Weather(short Index)
    {
        // Somente se necessário
        if (!Editor_Maps.Objects.butVisualization.Checked || Lists.Map[Index].Weather.Type == (byte)Globals.Weathers.Normal) return;

        // Dados
        byte x = 0;
        if (Lists.Map[Index].Weather.Type == (byte)Globals.Weathers.Snowing) x = 32;

        // Desenha as partículas
        for (int i = 1; i < Lists.Weather.Length; i++)
            if (Lists.Weather[i].Visible)
                Render(Win_Map, Tex_Weather, new Rectangle(x, 0, 32, 32), Editor_Maps.Zoom(new Rectangle(Lists.Weather[i].x, Lists.Weather[i].y, 32, 32)), CColor(255, 255, 255, 150));
    }

    public static void Editor_Maps_Map_Light(short Index)
    {
        Editor_Maps Objects = Editor_Maps.Objects;
        byte GlobalLight_Tex = Lists.Map[Index].Light_Global;
        Point Begin = Globals.Zoom(Editor_Maps.Objects.scrlMapX.Value, Editor_Maps.Objects.scrlMapY.Value);
        byte Light = (byte)((255 * ((decimal)Lists.Map[Index].Lighting / 100) - 255) * -1);

        // Somente se necessário
        if (!Editor_Maps.Objects.butVisualization.Checked) return;

        // Escuridão
        Win_Map_Lighting.Clear(CColor(0, 0, 0, Light));

        // Desenha o ponto iluminado
        if (Lists.Map[Index].Light.Count > 0)
            for (byte i = 0; i < Lists.Map[Index].Light.Count; i++)
                Render(Win_Map_Lighting, Tex_Lighting, Editor_Maps.Zoom_Grid(Lists.Map[Index].Light[i].Rec), null, new RenderStates(BlendMode.Multiply));

        // Pré visualização
        if (Editor_Maps.Objects.butMLighting.Checked)
            Render(Win_Map_Lighting, Tex_Lighting, Globals.Zoom(Editor_Maps.Map_Selection), null, new RenderStates(BlendMode.Multiply));

        // Apresenta o que foi renderizado
        Win_Map_Lighting.Display();
        Win_Map.Draw(new Sprite(Win_Map_Lighting.Texture));

        // Luz global
        if (GlobalLight_Tex > 0)
            Render(Win_Map, Tex_Light[GlobalLight_Tex], Editor_Maps.Zoom(new Rectangle(new Point(-Begin.X, -Begin.Y), TSize(Tex_Light[GlobalLight_Tex]))), CColor(255, 255, 255, 175), new RenderStates(BlendMode.Add));

        // Ponto de remoção da luz
        if (Objects.butMLighting.Checked)
            if (Lists.Map[Index].Light.Count > 0)
                for (byte i = 0; i < Lists.Map[Index].Light.Count; i++)
                    RenderRectangle(Win_Map, Lists.Map[Index].Light[i].Rec.X * Globals.Grid_Zoom, Lists.Map[Index].Light[i].Rec.Y * Globals.Grid_Zoom, Globals.Grid_Zoom, Globals.Grid_Zoom, CColor(175, 42, 42, 175));

        // Trovoadas
        Size Size = new Size(Editor_Maps.Zoom((Lists.Map[Index].Width + 1) * Globals.Grid), Editor_Maps.Zoom((Lists.Map[Index].Height + 1) * Globals.Grid));
        Render(Win_Map, Tex_Blank, 0, 0, 0, 0, Size.Width, Size.Height, CColor(255, 255, 255, Globals.Lightning));
    }

    public static void Editor_Maps_Map_Grids(short Index)
    {
        Editor_Maps Objects = Editor_Maps.Objects;
        Rectangle Source = Editor_Maps.Tile_Source, Destiny = new Rectangle();
        Point Begin = new Point(Editor_Maps.Map_Selection.X - Objects.scrlMapX.Value, Editor_Maps.Map_Selection.Y - Objects.scrlMapY.Value);

        // Dados
        Destiny.Location = Globals.Zoom(Begin.X, Begin.Y);
        Destiny.Size = new Size(Source.Width / Editor_Maps.Zoom(), Source.Height / Editor_Maps.Zoom());

        // Desenha as grades
        if (Objects.butGrid.Checked || !Objects.butGrid.Enabled)
            for (byte x = 0; x <= Editor_Maps.Map_Size.Width; x++)
                for (byte y = 0; y <= Editor_Maps.Map_Size.Height; y++)
                {
                    RenderRectangle(Win_Map, x * Globals.Grid_Zoom, y * Globals.Grid_Zoom, Globals.Grid_Zoom, Globals.Grid_Zoom, CColor(25, 25, 25, 70));
                    Editor_Maps_Map_Zones(Index, x, y);
                    Editor_Maps_Map_Attributes(Index, x, y);
                    Editor_Maps_Map_DirBlock(Index, x, y);
                }

        if (!Objects.chkAuto.Checked && Objects.butMNormal.Checked)
            // Normal
            if (Objects.butPencil.Checked)
                Render(Win_Map, Tex_Tile[Objects.cmbTiles.SelectedIndex + 1], Source, Destiny);
            // Retângulo
            else if (Objects.butRectangle.Checked)
                for (int x = Begin.X; x < Begin.X + Editor_Maps.Map_Selection.Width; x++)
                    for (int y = Begin.Y; y < Begin.Y + Editor_Maps.Map_Selection.Height; y++)
                        Render(Win_Map, Tex_Tile[Objects.cmbTiles.SelectedIndex + 1], Source, new Rectangle(Globals.Zoom(x, y), Destiny.Size));

        // Desenha a grade
        if (!Objects.butMAttributes.Checked || !Editor_Maps.Objects.optA_DirBlock.Checked)
            RenderRectangle(Win_Map, Destiny.X, Destiny.Y, Editor_Maps.Map_Selection.Width * Globals.Grid_Zoom, Editor_Maps.Map_Selection.Height * Globals.Grid_Zoom);
    }

    public static void Editor_Maps_Map_Zones(short Index, byte x, byte y)
    {
        Point Position = new Point((x - Editor_Maps.Objects.scrlMapX.Value) * Globals.Grid_Zoom, (y - Editor_Maps.Objects.scrlMapY.Value) * Globals.Grid_Zoom);
        byte Zone_Num = Lists.Map[Index].Tile[x, y].Zone;
        SFML.Graphics.Color Color;

        // Apenas se necessário
        if (!Editor_Maps.Objects.butMZones.Checked) return;
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

    public static void Editor_Maps_Map_Attributes(short Index, byte x, byte y)
    {
        Point Position = new Point((x - Editor_Maps.Objects.scrlMapX.Value) * Globals.Grid_Zoom, (y - Editor_Maps.Objects.scrlMapY.Value) * Globals.Grid_Zoom);
        Globals.Tile_Attributes Attribute = (Globals.Tile_Attributes)Lists.Map[Index].Tile[x, y].Attribute;
        SFML.Graphics.Color Color;
        string Letter;

        // Apenas se necessário
        if (!Editor_Maps.Objects.butMAttributes.Checked) return;
        if (Editor_Maps.Objects.optA_DirBlock.Checked) return;
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

    public static void Editor_Maps_Map_DirBlock(short Index, byte x, byte y)
    {
        Point Tile = new Point(Editor_Maps.Objects.scrlMapX.Value + x, Editor_Maps.Objects.scrlMapY.Value + y);
        byte Y;

        // Apenas se necessário
        if (!Editor_Maps.Objects.butMAttributes.Checked) return;
        if (!Editor_Maps.Objects.optA_DirBlock.Checked) return;

        // Previne erros
        if (Tile.X > Lists.Map[Index].Tile.GetUpperBound(0)) return;
        if (Tile.Y > Lists.Map[Index].Tile.GetUpperBound(1)) return;

        for (byte i = 0; i < (byte)Globals.Directions.Count; i++)
        {
            // Estado do bloqueio
            if (Lists.Map[Index].Tile[Tile.X, Tile.Y].Block[i])
                Y = 8;
            else
                Y = 0;

            // Renderiza
            Render(Win_Map, Tex_Directions, x * Globals.Grid + Globals.Block_Position(i).X, y * Globals.Grid + Globals.Block_Position(i).Y, i * 8, Y, 6, 6);
        }
    }

    public static void Editor_Maps_Map_NPCs(short Index)
    {
        if (Editor_Maps.Objects.butMNPCs.Checked)
            for (byte i = 0; i < Lists.Map[Index].NPC.Count; i++)
                if (Lists.Map[Index].NPC[i].Spawn)
                {
                    Point Position = new Point((Lists.Map[Index].NPC[i].X - Editor_Maps.Objects.scrlMapX.Value) * Globals.Grid_Zoom, (Lists.Map[Index].NPC[i].Y - Editor_Maps.Objects.scrlMapY.Value) * Globals.Grid_Zoom);

                    // Desenha uma sinalização de onde os NPCs estão
                    Render(Win_Map, Tex_Blank, new Rectangle(Position, new Size(Globals.Grid_Zoom, Globals.Grid_Zoom)), CColor(0, 220, 0, 150));
                    DrawText(Win_Map, (i + 1).ToString(), Position.X + 10, Position.Y + 10, SFML.Graphics.Color.White);
                }
    }
    #endregion

    #region Interface Editor
    public static void Interface()
    {
        // Apenas se necessário
        if (!Editor_Interface.Objects.Visible) return;

        // Desenha as ferramentas
        Win_Interface.Clear();
        Interface_Order(Lists.Tool.Nodes[(byte)Editor_Interface.Objects.cmbWindows.SelectedIndex]);
        Win_Interface.Display();
    }

    private static void Interface_Order(TreeNode Node)
    {
        for (byte i = 0; i < Node.Nodes.Count; i++)
        {
            // Desenha a ferramenta
            Lists.Structures.Tool Tool = (Lists.Structures.Tool)Node.Nodes[i].Tag;
            if (Tool.Visible)
            {
                if (Tool is Lists.Structures.Panel) Panel((Lists.Structures.Panel)Tool);
                else if (Tool is Lists.Structures.TextBox) TextBox((Lists.Structures.TextBox)Tool);
                else if (Tool is Lists.Structures.Button) Button((Lists.Structures.Button)Tool);
                else if (Tool is Lists.Structures.CheckBox) CheckBox((Lists.Structures.CheckBox)Tool);

                // Pula pra próxima
                Interface_Order(Node.Nodes[i]);
            }
        }
    }

    public static void Button(Lists.Structures.Button Tool)
    {
        // Desenha o botão
        if (Tool.Texture_Num < Tex_Button.Length)
            Render(Win_Interface, Tex_Button[Tool.Texture_Num], Tool.Position, new SFML.Graphics.Color(255, 255, 225, 225));
    }

    public static void Panel(Lists.Structures.Panel Tool)
    {
        // Desenha o painel
        if (Tool.Texture_Num < Tex_Button.Length)
            Render(Win_Interface, Tex_Panel[Tool.Texture_Num], Tool.Position);
    }

    public static void CheckBox(Lists.Structures.CheckBox Tool)
    {
        byte Margin = 4;

        // Define as propriedades dos retângulos
        Rectangle Rec_Source = new Rectangle(new Point(), new Size(TSize(Tex_CheckBox).Width / 2, TSize(Tex_CheckBox).Height));
        Rectangle Rec_Destiny = new Rectangle(Tool.Position, Rec_Source.Size);

        // Desenha a textura do marcador pelo seu estado 
        if (Tool.State)
            Rec_Source.Location = new Point(TSize(Tex_CheckBox).Width / 2, 0);

        // Desenha o marcador 
        Render(Win_Interface, Tex_CheckBox, Rec_Source, Rec_Destiny);
        DrawText(Win_Interface, Tool.Text, Rec_Destiny.Location.X + TSize(Tex_CheckBox).Width / 2 + Margin, Rec_Destiny.Location.Y + 1, SFML.Graphics.Color.White);
    }

    public static void TextBox(Lists.Structures.TextBox Tool)
    {
        // Desenha a ferramenta
        Render_Box(Win_Interface, Tex_TextBox, 3, Tool.Position, new Size(Tool.Width, TSize(Tex_TextBox).Height));
    }
    #endregion
}