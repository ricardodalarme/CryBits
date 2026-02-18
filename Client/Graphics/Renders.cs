using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal static class Renders
{
    // Locais de renderização
    public static RenderWindow RenderWindow;

    public static void Init()
    {
        // Inicia a janela
        RenderWindow = new RenderWindow(new VideoMode(new Vector2u(800, 608)), GameName, Styles.Titlebar | Styles.Close,
            State.Windowed);
        RenderWindow.Closed += UI.Window.OnClosed;
        RenderWindow.MouseButtonPressed += UI.Window.OnMouseButtonPressed;
        RenderWindow.MouseMoved += UI.Window.OnMouseMoved;
        RenderWindow.MouseButtonReleased += UI.Window.OnMouseButtonReleased;
        RenderWindow.KeyPressed += UI.Window.OnKeyPressed;
        RenderWindow.KeyReleased += UI.Window.OnKeyReleased;
        RenderWindow.TextEntered += UI.Window.OnTextEntered;
    }

    public static void Render(Texture texture, Rectangle recSource, Rectangle recDestiny, object color = null,
        object mode = null)
    {
        var tmpImage = new Sprite(texture)
        {
            // Define os dados
            TextureRect = new IntRect(new Vector2i(recSource.X, recSource.Y),
                new Vector2i(recSource.Width, recSource.Height)),
            Position = new Vector2f(recDestiny.X, recDestiny.Y),
            Scale = new Vector2f(recDestiny.Width / (float)recSource.Width, recDestiny.Height / (float)recSource.Height)
        };
        if (color != null) tmpImage.Color = (Color)color;

        // Renderiza a textura em forma de retângulo
        mode ??= RenderStates.Default;
        RenderWindow.Draw(tmpImage, (RenderStates)mode);
    }

    public static void Render(Texture texture, int x, int y, int sourceX, int sourceY, int sourceWidth,
        int sourceHeight, object color = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
        var destiny = new Rectangle(x, y, sourceWidth, sourceHeight);

        // Desenha a textura
        Render(texture, source, destiny, color);
    }

    public static void Render(Texture texture, Rectangle destiny, object color = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(new Point(0), texture.ToSize());

        // Desenha a textura
        Render(texture, source, destiny, color);
    }

    public static void Render(Texture texture, Point position, object color = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(new Point(0), texture.ToSize());
        var destiny = new Rectangle(position, texture.ToSize());

        // Desenha a textura
        Render(texture, source, destiny, color);
    }

    public static void DrawText(string text, int x, int y, Color color, TextAlign alignment = TextAlign.Left)
    {
        // Alinhamento do texto
        switch (alignment)
        {
            case TextAlign.Center: x -= MeasureString(text) / 2; break;
            case TextAlign.Right: x -= MeasureString(text); break;
        }

        // Define os dados
        var tempText = new Text(Fonts.Default, text)
        {
            CharacterSize = 10,
            FillColor = color,
            Position = new Vector2f(x, y),
            OutlineColor = new Color(0, 0, 0, 70),
            OutlineThickness = 1
        };

        // Desenha
        RenderWindow.Draw(tempText);
    }

    public static void DrawText(string text, int x, int y, Color color, int maxWidth, bool cut = true)
    {
        int messageWidth = MeasureString(text), split = -1;

        // Caso couber, adiciona a mensagem normalmente
        if (messageWidth < maxWidth)
            DrawText(text, x, y, color);
        else
            for (var i = 0; i < text.Length; i++)
            {
                // Verifica se o caráctere é um separável 
                split = text[i] switch
                {
                    '-' or '_' or ' ' => i,
                    _ => split
                };

                // Desenha a parte do texto que cabe
                var tempText = text.Substring(0, i);
                if (MeasureString(tempText) > maxWidth)
                {
                    // Divide o texto novamente caso tenha encontrado um ponto de divisão
                    if (cut && split != -1) tempText = text.Substring(0, split + 1);

                    // Desenha o texto cortado
                    DrawText(tempText, x, y, color);
                    DrawText(text.Substring(tempText.Length), x, y + 12, color, maxWidth);
                    return;
                }
            }
    }

    public static void Render_Box(Texture texture, byte margin, Point position, Size size)
    {
        var textureWidth = texture.ToSize().Width;
        var textureHeight = texture.ToSize().Height;

        // Borda esquerda
        Render(texture, new Rectangle(new Point(0), new Size(margin, textureWidth)),
            new Rectangle(position, new Size(margin, textureHeight)));
        // Borda direita
        Render(texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
        // Centro
        Render(texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)),
            new Rectangle(new Point(position.X + margin, position.Y),
                new Size(size.Width - margin * 2, textureHeight)));
    }
}