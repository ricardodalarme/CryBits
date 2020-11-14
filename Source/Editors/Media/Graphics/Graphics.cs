using System;
using System.Drawing;
using System.IO;
using CryBits.Editors.Library;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;

namespace CryBits.Editors.Media
{
    internal partial class Graphics
    {
        // Fonte principal
        public static Font Font_Default;

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

        private static void Render(RenderTexture window, Texture texture, Vec4 destiny, object color = null, object mode = null)
        {
            // Define os dados
            Sprite tmpImage = new Sprite(texture)
            {
                Position = new Vector2f(destiny.X, destiny.Y),
                Scale = new Vector2f(destiny.Width / (float)texture.Size.X, destiny.Height / (float)texture.Size.Y)
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

        private static void Render(RenderWindow window, Texture texture, Vector2f point, object color = null, object mode = null)
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
            Render(window, Tex_Grid, rectangle.X, rectangle.Y, 0, 0, rectangle.Width, 1, color);
            Render(window, Tex_Grid, rectangle.X, rectangle.Y, 0, 0, 1, rectangle.Height, color);
            Render(window, Tex_Grid, rectangle.X, rectangle.Y + rectangle.Height - 1, 0, 0, rectangle.Width, 1, color);
            Render(window, Tex_Grid, rectangle.X + rectangle.Width - 1, rectangle.Y, 0, 0, 1, rectangle.Height, color);
        }

        private static void RenderRectangle(RenderWindow window, int x, int y, int width, int height, object color = null)
        {
            // Desenha a caixa
            RenderRectangle(window, new Rectangle(x, y, width, height), color);
        }

        private static void Render_Box(RenderWindow window, Texture texture, byte margin, Vector2f position, Vector2u size)
        {
            uint textureWidth = texture.Size.X;
            uint textureHeight = texture.Size.Y;

            // Borda esquerda
            Render(window, texture, new Rectangle(new Point(0), new Size(margin, textureWidth)), new Rectangle(position, new Size(margin, textureHeight)));
            // Borda direita
            Render(window, texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
            // Centro
            Render(window, texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + margin, position.Y), new Size(size.Width - margin * 2, textureHeight)));
        }

        private static void DrawText(RenderWindow window, string text, int x, int y, Color color)
        {
            Text tempText = new Text(text, Font_Default);

            // Define os dados
            tempText.CharacterSize = 10;
            tempText.FillColor = color;
            tempText.Position = new Vector2f(x, y);
            tempText.OutlineColor = new Color(0, 0, 0, 70);
            tempText.OutlineThickness = 1;

            // Desenha
            window.Draw(tempText);
        }

        public static void Init()
        {
            // Conjuntos
            Tex_Character = AddTextures(Directories.Tex_Characters.FullName);
            Tex_Tile = AddTextures(Directories.Tex_Tiles.FullName);
            Tex_Face = AddTextures(Directories.Tex_Faces.FullName);
            Tex_Panel = AddTextures(Directories.Tex_Panels.FullName);
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
            Font_Default = new Font(Directories.Fonts.FullName + "Georgia.ttf");
        }
    }
}