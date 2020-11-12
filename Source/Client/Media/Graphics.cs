using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using CryBits.Client.UI;
using SFML.Graphics;
using SFML.Window;
using static CryBits.Client.Logic.Game;
using static CryBits.Client.Logic.Utils;

namespace CryBits.Client.Media
{
    class Graphics
    {
        // Locais de renderização
        public static RenderWindow RenderWindow;

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
        public static Texture[] Tex_Light;
        public static Texture[] Tex_Item;
        public static Texture Tex_CheckBox;
        public static Texture Tex_TextBox;
        public static Texture Tex_Weather;
        public static Texture Tex_Blanc;
        public static Texture Tex_Directions;
        public static Texture Tex_Shadow;
        public static Texture Tex_Bars;
        public static Texture Tex_Bars_Panel;
        public static Texture Tex_Grid;
        public static Texture Tex_Equipments;
        public static Texture Tex_Blood;
        public static Texture Tex_Party_Bars;
        public static Texture Tex_Intro;

        // Formato das texturas
        public const string Format = ".png";

        public enum Alignments
        {
            Left,
            Center,
            Right
        }

        #region Engine
        public static void Init()
        {
            // Carrega a fonte
            Font_Default = new SFML.Graphics.Font(Directories.Fonts.FullName + "Georgia.ttf");

            // Carrega as texturas
            LoadTextures();

            // Inicia a janela
            RenderWindow = new RenderWindow(new VideoMode(800, 608), Game_Name, Styles.Close);
            RenderWindow.Closed += new EventHandler(Windows.OnClosed);
            RenderWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(Windows.OnMouseButtonPressed);
            RenderWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(Windows.OnMouseMoved);
            RenderWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(Windows.OnMouseButtonReleased);
            RenderWindow.KeyPressed += new EventHandler<KeyEventArgs>(Windows.OnKeyPressed);
            RenderWindow.KeyReleased += new EventHandler<KeyEventArgs>(Windows.OnKeyReleased);
            RenderWindow.TextEntered += new EventHandler<TextEventArgs>(Windows.OnTextEntered);
        }

        private static void LoadTextures()
        {
            // Conjuntos
            Tex_Character = LoadTextures(Directories.Tex_Characters.FullName);
            Tex_Tile = LoadTextures(Directories.Tex_Tiles.FullName);
            Tex_Face = LoadTextures(Directories.Tex_Faces.FullName);
            Tex_Panel = LoadTextures(Directories.Tex_Panels.FullName);
            Tex_Button = LoadTextures(Directories.Tex_Buttons.FullName);
            Tex_Panorama = LoadTextures(Directories.Tex_Panoramas.FullName);
            Tex_Fog = LoadTextures(Directories.Tex_Fogs.FullName);
            Tex_Light = LoadTextures(Directories.Tex_Lights.FullName);
            Tex_Item = LoadTextures(Directories.Tex_Items.FullName);

            // Únicas
            Tex_Weather = new Texture(Directories.Tex_Weather.FullName + Format);
            Tex_Blanc = new Texture(Directories.Tex_Blank.FullName + Format);
            Tex_Directions = new Texture(Directories.Tex_Directions.FullName + Format);
            Tex_CheckBox = new Texture(Directories.Tex_CheckBox.FullName + Format);
            Tex_TextBox = new Texture(Directories.Tex_TextBox.FullName + Format);
            Tex_Directions = new Texture(Directories.Tex_Directions.FullName + Format);
            Tex_Shadow = new Texture(Directories.Tex_Shadow.FullName + Format);
            Tex_Bars = new Texture(Directories.Tex_Bars.FullName + Format);
            Tex_Bars_Panel = new Texture(Directories.Tex_Bars_Panel.FullName + Format);
            Tex_Grid = new Texture(Directories.Tex_Grid.FullName + Format);
            Tex_Equipments = new Texture(Directories.Tex_Equipments.FullName + Format);
            Tex_Blood = new Texture(Directories.Tex_Blood.FullName + Format);
            Tex_Party_Bars = new Texture(Directories.Tex_Party_Bars.FullName + Format);
            Tex_Intro = new Texture(Directories.Tex_Intro.FullName + Format);
        }

        private static Texture[] LoadTextures(string directory)
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
            else
                return new Size(0, 0);
        }

        private static SFML.Graphics.Color CColor(byte r = 255, byte g = 255, byte b = 255, byte a = 255) => new SFML.Graphics.Color(r, g, b, a);

        private static void Render(Texture texture, Rectangle recSource, Rectangle recDestiny, object color = null, object mode = null)
        {
            Sprite tmpImage = new Sprite(texture);

            // Define os dados
            tmpImage.TextureRect = new IntRect(recSource.X, recSource.Y, recSource.Width, recSource.Height);
            tmpImage.Position = new SFML.System.Vector2f(recDestiny.X, recDestiny.Y);
            tmpImage.Scale = new SFML.System.Vector2f(recDestiny.Width / (float)recSource.Width, recDestiny.Height / (float)recSource.Height);
            if (color != null)
                tmpImage.Color = (SFML.Graphics.Color)color;

            // Renderiza a textura em forma de retângulo
            if (mode == null) mode = RenderStates.Default;
            RenderWindow.Draw(tmpImage, (RenderStates)mode);
        }

        private static void Render(Texture texture, int x, int y, int sourceX, int sourceY, int sourceWidth, int sourceHeight, object color = null)
        {
            // Define as propriedades dos retângulos
            Rectangle source = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
            Rectangle destiny = new Rectangle(x, y, sourceWidth, sourceHeight);

            // Desenha a textura
            Render(texture, source, destiny, color);
        }

        private static void Render(Texture texture, Rectangle destiny, object color = null)
        {
            // Define as propriedades dos retângulos
            Rectangle source = new Rectangle(new Point(0), Size(texture));

            // Desenha a textura
            Render(texture, source, destiny, color);
        }

        private static void Render(Texture texture, Point position, object color = null)
        {
            // Define as propriedades dos retângulos
            Rectangle source = new Rectangle(new Point(0), Size(texture));
            Rectangle destiny = new Rectangle(position, Size(texture));

            // Desenha a textura
            Render(texture, source, destiny, color);
        }

        private static void DrawText(string text, int x, int y, SFML.Graphics.Color color, Alignments alignment = Alignments.Left)
        {
            Text tempText = new Text(text, Font_Default);

            // Alinhamento do texto
            switch (alignment)
            {
                case Alignments.Center: x -= MeasureString(text) / 2; break;
                case Alignments.Right: x -= MeasureString(text); break;
            }

            // Define os dados
            tempText.CharacterSize = 10;
            tempText.FillColor = color;
            tempText.Position = new SFML.System.Vector2f(x, y);
            tempText.OutlineColor = new SFML.Graphics.Color(0, 0, 0, 70);
            tempText.OutlineThickness = 1;

            // Desenha
            RenderWindow.Draw(tempText);
        }

        private static void DrawText(string text, int x, int y, SFML.Graphics.Color color, int maxWidth, bool cut = true)
        {
            string tempText;
            int messageWidth = MeasureString(text), split = -1;

            // Caso couber, adiciona a mensagem normalmente
            if (messageWidth < maxWidth)
                DrawText(text, x, y, color);
            else
                for (int i = 0; i < text.Length; i++)
                {
                    // Verifica se o caráctere é um separável 
                    switch (text[i])
                    {
                        case '-':
                        case '_':
                        case ' ': split = i; break;
                    }

                    // Desenha a parte do texto que cabe
                    tempText = text.Substring(0, i);
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

        private static void Render_Box(Texture texture, byte margin, Point position, Size size)
        {
            int textureWidth = Graphics.Size(texture).Width;
            int textureHeight = Graphics.Size(texture).Height;

            // Borda esquerda
            Render(texture, new Rectangle(new Point(0), new Size(margin, textureWidth)), new Rectangle(position, new Size(margin, textureHeight)));
            // Borda direita
            Render(texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
            // Centro
            Render(texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + margin, position.Y), new Size(size.Width - margin * 2, textureHeight)));
        }
        #endregion

        public static void Present()
        {
            // Limpa a área com um fundo preto
            RenderWindow.Clear(SFML.Graphics.Color.Black);

            // Desenha as coisas em jogo
            InGame();

            // Interface do jogo
            Interface(Tools.Order);

            // Desenha o chat 
            if (Windows.Current == WindowsTypes.Game) Chat();

            // Exibe o que foi renderizado
            RenderWindow.Display();
        }

        private static void InGame()
        {
            // Não desenhar se não estiver em jogo
            if (Windows.Current != WindowsTypes.Game) return;

            // Atualiza a câmera
            Camera.Update();

            // Desenhos abaixo do jogador
            Map_Panorama();
            Map_Tiles((byte)Layers.Ground);
            Map_Blood();
            Map_Items();

            // Desenha os NPCs
            for (byte i = 0; i < Mapper.Current.NPC.Length; i++)
                if (Mapper.Current.NPC[i].Data != null)
                    NPC(Mapper.Current.NPC[i]);

            // Desenha os jogadores
            for (byte i = 0; i < Player.List.Count; i++)
                if (Player.List[i] != Player.Me)
                    if (Player.List[i].Map == Player.Me.Map)
                        Player_Character(Player.List[i]);

            // Desenha o próprio jogador
            Player_Character(Player.Me);

            // Desenhos acima do jogador
            Map_Tiles((byte)Layers.Fringe);
            Map_Weather();
            Map_Fog();
            Map_Name();

            // Desenha os membros da party
            Party();

            // Desenha os dados do jogo
            if (Option.FPS) DrawText("FPS: " + FPS.ToString(), 176, 7, SFML.Graphics.Color.White);
            if (Option.Latency) DrawText("Latency: " + CryBits.Client.Network.Socket.Latency.ToString(), 176, 19, SFML.Graphics.Color.White);
        }

        #region Tools
        private static void Interface(List<Tools.OrderStructure> node)
        {
            for (byte i = 0; i < node.Count; i++)
                if (node[i].Data.Visible)
                {
                    // Desenha a ferramenta
                    if (node[i].Data is Panels) Panel((Panels)node[i].Data);
                    else if (node[i].Data is TextBoxes) TextBox((TextBoxes)node[i].Data);
                    else if (node[i].Data is Buttons) Button((Buttons)node[i].Data);
                    else if (node[i].Data is CheckBoxes) CheckBox((CheckBoxes)node[i].Data);

                    // Desenha algumas coisas mais específicas da interface
                    Interface_Specific(node[i].Data);

                    // Pula pra próxima
                    Interface(node[i].Nodes);
                }
        }

        private static void Button(Buttons tool)
        {
            byte alpha = 225;

            // Define a transparência do botão pelo seu estado
            switch (tool.State)
            {
                case Buttons.States.Above: alpha = 250; break;
                case Buttons.States.Click: alpha = 200; break;
            }

            // Desenha o botão
            Render(Tex_Button[tool.Texture_Num], tool.Position, new SFML.Graphics.Color(255, 255, 225, alpha));
        }

        private static void Panel(Panels tool)
        {
            // Desenha o painel
            Render(Tex_Panel[tool.Texture_Num], tool.Position);
        }

        private static void CheckBox(CheckBoxes tool)
        {
            // Define as propriedades dos retângulos
            Rectangle recSource = new Rectangle(new Point(), new Size(Size(Tex_CheckBox).Width / 2, Size(Tex_CheckBox).Height));
            Rectangle recDestiny = new Rectangle(tool.Position, recSource.Size);

            // Desenha a textura do marcador pelo seu estado 
            if (tool.Checked) recSource.Location = new Point(Size(Tex_CheckBox).Width / 2, 0);

            // Desenha o marcador 
            Render(Tex_CheckBox, recSource, recDestiny);
            DrawText(tool.Text, recDestiny.Location.X + Size(Tex_CheckBox).Width / 2 + CheckBoxes.Margin, recDestiny.Location.Y + 1, SFML.Graphics.Color.White);
        }

        private static void TextBox(TextBoxes tool)
        {
            Point position = tool.Position;
            string text = tool.Text;

            // Desenha a ferramenta
            Render_Box(Tex_TextBox, 3, tool.Position, new Size(tool.Width, Size(Tex_TextBox).Height));

            // Altera todos os caracteres do texto para um em especifico, se for necessário
            if (tool.Password && !string.IsNullOrEmpty(text)) text = new string('•', text.Length);

            // Quebra o texto para que caiba no digitalizador, se for necessário
            text = TextBreak(text, tool.Width - 10);

            // Desenha o texto do digitalizador
            if (TextBoxes.Focused != null && (TextBoxes)TextBoxes.Focused.Data == tool && TextBoxes.Signal) text += "|";
            DrawText(text, position.X + 4, position.Y + 2, SFML.Graphics.Color.White);
        }


        private static void Interface_Specific(Tools.Structure tool)
        {
            // Interações especificas
            if (!(tool is Panels)) return;
            switch (tool.Name)
            {
                case "SelectCharacter": SelectCharacter_Class(); break;
                case "CreateCharacter": CreateCharacter_Class(); break;
                case "Hotbar": Hotbar((Panels)tool); break;
                case "Menu_Character": Menu_Character((Panels)tool); break;
                case "Menu_Inventory": Menu_Inventory((Panels)tool); break;
                case "Bars": Bars((Panels)tool); break;
                case "Information": Informations((Panels)tool); break;
                case "Party_Invitation": Party_Invitation((Panels)tool); break;
                case "Trade_Invitation": Trade_Invitation((Panels)tool); break;
                case "Trade": Trade((Panels)tool); break;
                case "Shop": Shop((Panels)tool); break;
            }
        }
        #endregion

        #region Menu
        private static void SelectCharacter_Class()
        {
            Point textPosition = new Point(399, 425);
            string text = "(" + (Panels.SelectCharacter + 1) + ") None";

            // Somente se necessário
            if (!Buttons.Characters_Change_Buttons())
            {
                DrawText(text, textPosition.X, textPosition.Y, SFML.Graphics.Color.White, Alignments.Center);
                return;
            }

            // Verifica se o personagem existe
            if (Panels.SelectCharacter >= Panels.Characters.Length)
            {
                DrawText(text, textPosition.X, textPosition.Y, SFML.Graphics.Color.White, Alignments.Center);
                return;
            }

            // Desenha o personagem
            short textureNum = Panels.Characters[Panels.SelectCharacter].Texture_Num;
            if (textureNum > 0)
            {
                Render(Tex_Face[textureNum], new Point(353, 442));
                Character(textureNum, new Point(356, 534 - Size(Tex_Character[textureNum]).Height / 4), Directions.Down, AnimationStopped);
            }

            // Desenha o nome da classe
            text = "(" + (Panels.SelectCharacter + 1) + ") " + Panels.Characters[Panels.SelectCharacter].Name;
            DrawText(text, textPosition.X, textPosition.Y, SFML.Graphics.Color.White, Alignments.Center);
        }

        private static void CreateCharacter_Class()
        {
            short textureNum = 0;
            Class @class = CryBits.Client.Entities.Class.List.ElementAt(Panels.CreateCharacter_Class).Value;

            // Textura do personagem
            if (CheckBoxes.List["GenderMale"].Checked && @class.Tex_Male.Length > 0)
                textureNum = @class.Tex_Male[Panels.CreateCharacter_Tex];
            else if (@class.Tex_Female.Length > 0)
                textureNum = @class.Tex_Female[Panels.CreateCharacter_Tex];

            // Desenha o personagem
            if (textureNum > 0)
            {
                Render(Tex_Face[textureNum], new Point(425, 440));
                Character(textureNum, new Point(433, 501), Directions.Down, AnimationStopped);
            }

            // Desenha o nome da classe
            string text = @class.Name;
            DrawText(text, 347, 509, SFML.Graphics.Color.White, Alignments.Center);

            // Descrição
            DrawText(@class.Description, 282, 526, SFML.Graphics.Color.White, 123);
        }
        #endregion

        private static void Bars(Panels tool)
        {
            decimal hpPercentage = Player.Me.Vital[(byte)Vitals.HP] / (decimal)Player.Me.Max_Vital[(byte)Vitals.HP];
            decimal mpPercentage = Player.Me.Vital[(byte)Vitals.MP] / (decimal)Player.Me.Max_Vital[(byte)Vitals.MP];
            decimal expPercentage = Player.Me.Experience / (decimal)Player.Me.ExpNeeded;

            // Barras
            Render(Tex_Bars_Panel, tool.Position.X + 6, tool.Position.Y + 6, 0, 0, (int)(Tex_Bars_Panel.Size.X * hpPercentage), 17);
            Render(Tex_Bars_Panel, tool.Position.X + 6, tool.Position.Y + 24, 0, 18, (int)(Tex_Bars_Panel.Size.X * mpPercentage), 17);
            Render(Tex_Bars_Panel, tool.Position.X + 6, tool.Position.Y + 42, 0, 36, (int)(Tex_Bars_Panel.Size.X * expPercentage), 17);

            // Textos 
            DrawText("HP", tool.Position.X + 10, tool.Position.Y + 3, SFML.Graphics.Color.White);
            DrawText("MP", tool.Position.X + 10, tool.Position.Y + 21, SFML.Graphics.Color.White);
            DrawText("Exp", tool.Position.X + 10, tool.Position.Y + 39, SFML.Graphics.Color.White);

            // Indicadores
            DrawText(Player.Me.Vital[(byte)Vitals.HP] + "/" + Player.Me.Max_Vital[(byte)Vitals.HP], tool.Position.X + 76, tool.Position.Y + 7, SFML.Graphics.Color.White, Alignments.Center);
            DrawText(Player.Me.Vital[(byte)Vitals.MP] + "/" + Player.Me.Max_Vital[(byte)Vitals.MP], tool.Position.X + 76, tool.Position.Y + 25, SFML.Graphics.Color.White, Alignments.Center);
            DrawText(Player.Me.Experience + "/" + Player.Me.ExpNeeded, tool.Position.X + 76, tool.Position.Y + 43, SFML.Graphics.Color.White, Alignments.Center);
        }

        private static void Chat()
        {
            Panels tool = Panels.List["Chat"];
            tool.Visible = TextBoxes.Focused != null && ((TextBoxes)TextBoxes.Focused.Data).Name.Equals("Chat");

            // Renderiza as mensagens
            if (tool.Visible || (Loop.Chat_Timer >= Environment.TickCount && Option.Chat))
                for (byte i = UI.Chat.Lines_First; i <= global::CryBits.Client.UI.Chat.LinesVisible + global::CryBits.Client.UI.Chat.Lines_First; i++)
                    if (global::CryBits.Client.UI.Chat.Order.Count > i)
                        DrawText(global::CryBits.Client.UI.Chat.Order[i].Text, 16, 461 + 11 * (i - global::CryBits.Client.UI.Chat.Lines_First), global::CryBits.Client.UI.Chat.Order[i].Color);

            // Dica de como abrir o chat
            if (!tool.Visible) DrawText("Press [Enter] to open chat.", TextBoxes.List["Chat"].Position.X + 5, TextBoxes.List["Chat"].Position.Y + 3, SFML.Graphics.Color.White);
        }

        private static void Informations(Panels tool)
        {
            Item item = CryBits.Client.Entities.Item.Get(Panels.Infomation_ID);
            SFML.Graphics.Color textColor;
            List<string> data = new List<string>();

            // Apenas se necessário
            if (item == null) return;

            // Define a cor de acordo com a raridade
            switch ((Rarity)item.Rarity)
            {
                case Rarity.Uncommon: textColor = CColor(204, 255, 153); break; // Verde
                case Rarity.Rare: textColor = CColor(102, 153, 255); break; // Azul
                case Rarity.Epic: textColor = CColor(153, 0, 204); break; // Roxo
                case Rarity.Legendary: textColor = CColor(255, 255, 77); break; // Amarelo
                default: textColor = CColor(255, 255, 255); break; // Branco
            }

            // Nome, descrição e icone do item
            DrawText(item.Name, tool.Position.X + 41, tool.Position.Y + 6, textColor, Alignments.Center);
            DrawText(item.Description, tool.Position.X + 82, tool.Position.Y + 20, SFML.Graphics.Color.White, 86);
            Render(Tex_Item[item.Texture], new Rectangle(tool.Position.X + 9, tool.Position.Y + 21, 64, 64));

            // Informações da Loja
            if (Panels.List["Shop"].Visible)
                if (Panels.Shop_Slot >= 0)
                    data.Add("Price: " + Panels.Shop_Open.Sold[Panels.Shop_Slot].Price);
                else if (Panels.Inventory_Slot > 0)
                    if (Panels.Shop_Open.FindBought(item) != null)
                        data.Add("Sale price: " + Panels.Shop_Open.FindBought(item).Price);

            // Informações específicas dos itens
            switch ((Items)item.Type)
            {
                // Poção
                case Items.Potion:
                    for (byte n = 0; n < (byte)Vitals.Count; n++)
                        if (item.Potion_Vital[n] != 0)
                            data.Add(((Vitals)n).ToString() + ": " + item.Potion_Vital[n]);

                    if (item.Potion_Experience != 0) data.Add("Experience: " + item.Potion_Experience);
                    break;
                // Equipamentos
                case Items.Equipment:
                    if (item.Equip_Type == (byte)Equipments.Weapon)
                        if (item.Weapon_Damage != 0)
                            data.Add("Damage: " + item.Weapon_Damage);

                    for (byte n = 0; n < (byte)Attributes.Count; n++)
                        if (item.Equip_Attribute[n] != 0)
                            data.Add(((Attributes)n).ToString() + ": " + item.Equip_Attribute[n]);
                    break;
            }

            // Desenha todos os dados necessários
            Point[] positions = { new Point(tool.Position.X + 10, tool.Position.Y + 90), new Point(tool.Position.X + 10, tool.Position.Y + 102), new Point(tool.Position.X + 10, tool.Position.Y + 114), new Point(tool.Position.X + 96, tool.Position.Y + 90), new Point(tool.Position.X + 96, tool.Position.Y + 102), new Point(tool.Position.X + 96, tool.Position.Y + 114), new Point(tool.Position.X + 96, tool.Position.Y + 126) };
            for (byte i = 0; i < data.Count; i++) DrawText(data[i], positions[i].X, positions[i].Y, SFML.Graphics.Color.White);
        }

        private static void Hotbar(Panels tool)
        {
            string indicator = string.Empty;

            // Desenha os objetos da hotbar
            for (byte i = 0; i < MaxHotbar; i++)
            {
                byte slot = Player.Me.Hotbar[i].Slot;
                if (slot > 0)
                    switch ((CryBits.Hotbars)Player.Me.Hotbar[i].Type)
                    {
                        // Itens
                        case CryBits.Hotbars.Item: Item(Player.Me.Inventory[slot].Item, 1, tool.Position + new Size(8, 6), (byte)(i + 1), 10); break;
                    }

                // Desenha os números de cada slot
                if (i < 10) indicator = (i + 1).ToString();
                else if (i == 9) indicator = "0";
                DrawText(indicator, tool.Position.X + 16 + 36 * i, tool.Position.Y + 22, SFML.Graphics.Color.White);
            }

            // Movendo slot
            if (Panels.Hotbar_Change >= 0)
                if (Player.Me.Hotbar[Panels.Hotbar_Change].Type == (byte)CryBits.Hotbars.Item)
                    Render(Tex_Item[Player.Me.Inventory[Player.Me.Hotbar[Panels.Hotbar_Change].Slot].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
        }

        private static void Menu_Character(Panels tool)
        {
            // Dados básicos
            DrawText(Player.Me.Name, tool.Position.X + 18, tool.Position.Y + 52, SFML.Graphics.Color.White);
            DrawText(Player.Me.Level.ToString(), tool.Position.X + 18, tool.Position.Y + 79, SFML.Graphics.Color.White);
            Render(Tex_Face[Player.Me.Texture_Num], new Point(tool.Position.X + 82, tool.Position.Y + 37));

            // Atributos
            DrawText("Strength: " + Player.Me.Attribute[(byte)Attributes.Strength], tool.Position.X + 32, tool.Position.Y + 146, SFML.Graphics.Color.White);
            DrawText("Resistance: " + Player.Me.Attribute[(byte)Attributes.Resistance], tool.Position.X + 32, tool.Position.Y + 162, SFML.Graphics.Color.White);
            DrawText("Intelligence: " + Player.Me.Attribute[(byte)Attributes.Intelligence], tool.Position.X + 32, tool.Position.Y + 178, SFML.Graphics.Color.White);
            DrawText("Agility: " + Player.Me.Attribute[(byte)Attributes.Agility], tool.Position.X + 32, tool.Position.Y + 194, SFML.Graphics.Color.White);
            DrawText("Vitality: " + Player.Me.Attribute[(byte)Attributes.Vitality], tool.Position.X + 32, tool.Position.Y + 210, SFML.Graphics.Color.White);
            DrawText("Points: " + Player.Me.Points, tool.Position.X + 14, tool.Position.Y + 228, SFML.Graphics.Color.White);

            // Equipamentos 
            for (byte i = 0; i < (byte)Equipments.Count; i++)
                if (Player.Me.Equipment[i] == null)
                    Render(Tex_Equipments, tool.Position.X + 7 + i * 34, tool.Position.Y + 247, i * 34, 0, 32, 32);
                else
                    Render(Tex_Item[Player.Me.Equipment[i].Texture], tool.Position.X + 8 + i * 35, tool.Position.Y + 247, 0, 0, 34, 34);
        }

        private static void Menu_Inventory(Panels tool)
        {
            byte numColumns = 5;

            // Desenha todos os itens do inventário
            for (byte i = 1; i <= MaxInventory; i++)
                Item(Player.Me.Inventory[i].Item, Player.Me.Inventory[i].Amount, tool.Position + new Size(7, 30), i, numColumns);

            // Movendo item
            if (Panels.Inventory_Change > 0) Render(Tex_Item[Player.Me.Inventory[Panels.Inventory_Change].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
        }

        private static void Party_Invitation(Panels tool)
        {
            DrawText(Panels.Party_Invitation + " has invite you to a party. Would you like to join?", tool.Position.X + 14, tool.Position.Y + 33, SFML.Graphics.Color.White, 160);
        }

        private static void Party()
        {
            for (byte i = 0; i < Player.Me.Party.Length; i++)
            {
                // Barras do membro
                Render(Tex_Party_Bars, 10, 92 + (27 * i), 0, 0, 82, 8); // HP Cinza
                Render(Tex_Party_Bars, 10, 99 + (27 * i), 0, 0, 82, 8); // MP Cinza
                if (Player.Me.Party[i].Vital[(byte)Vitals.HP] > 0)
                    Render(Tex_Party_Bars, 10, 92 + (27 * i), 0, 8, (Player.Me.Party[i].Vital[(byte)Vitals.HP] * 82) / Player.Me.Party[i].Max_Vital[(byte)Vitals.HP], 8); // HP 
                if (Player.Me.Party[i].Vital[(byte)Vitals.MP] > 0)
                    Render(Tex_Party_Bars, 10, 99 + (27 * i), 0, 16, (Player.Me.Party[i].Vital[(byte)Vitals.MP] * 82) / Player.Me.Party[i].Max_Vital[(byte)Vitals.MP], 8); // MP 

                // Nome do membro
                DrawText(Player.Me.Party[i].Name, 10, 79 + (27 * i), SFML.Graphics.Color.White);
            }
        }

        private static void Trade_Invitation(Panels tool)
        {
            DrawText(Panels.Trade_Invitation + " has invite you to a trade. Would you like to join?", tool.Position.X + 14, tool.Position.Y + 33, SFML.Graphics.Color.White, 160);
        }

        private static void Trade(Panels tool)
        {
            // Desenha os itens das ofertas
            for (byte i = 1; i <= MaxInventory; i++)
            {
                Item(Player.Me.Trade_Offer[i].Item, Player.Me.Trade_Offer[i].Amount, tool.Position + new Size(7, 50), i, 5);
                Item(Player.Me.Trade_Their_Offer[i].Item, Player.Me.Trade_Their_Offer[i].Amount, tool.Position + new Size(192, 50), i, 5);
            }
        }

        private static void Shop(Panels tool)
        {
            // Dados da loja
            string name = Panels.Shop_Open.Name;
            DrawText(name, tool.Position.X + 131, tool.Position.Y + 28, SFML.Graphics.Color.White, Alignments.Center);
            DrawText("Currency: " + Panels.Shop_Open.Currency.Name, tool.Position.X + 10, tool.Position.Y + 195, SFML.Graphics.Color.White);

            // Desenha os itens
            for (byte i = 0; i < Panels.Shop_Open.Sold.Length; i++)
                Item(Panels.Shop_Open.Sold[i].Item, Panels.Shop_Open.Sold[i].Amount, tool.Position + new Size(7, 50), (byte)(i + 1), 7);
        }

        private static void Item(Item item, short amount, Point start, byte slot, byte columns, byte grid = 32, byte gap = 4)
        {
            // Somente se necessário
            if (item == null) return;

            // Posição do item baseado no slot
            int line = (slot - 1) / columns;
            int column = slot - (line * 5) - 1;
            Point position = start + new Size(column * (grid + gap), line * (grid + gap));

            // Desenha o item e sua quantidade
            Render(Tex_Item[item.Texture], position);
            if (amount > 1) DrawText(amount.ToString(), position.X + 2, position.Y + 17, SFML.Graphics.Color.White);
        }

        private static void Character(short textureNum, Point position, Directions direction, byte column, bool hurt = false)
        {
            Rectangle recSource = new Rectangle(), recDestiny;
            Size size = Graphics.Size(Tex_Character[textureNum]);
            SFML.Graphics.Color color = new SFML.Graphics.Color(255, 255, 255);
            byte line = 0;

            // Direção
            switch (direction)
            {
                case Directions.Up: line = MovementUp; break;
                case Directions.Down: line = MovementDown; break;
                case Directions.Left: line = MovementLeft; break;
                case Directions.Right: line = MovementRight; break;
            }

            // Define as propriedades dos retângulos
            recSource.X = column * size.Width / AnimationAmount;
            recSource.Y = line * size.Height / AnimationAmount;
            recSource.Width = size.Width / AnimationAmount;
            recSource.Height = size.Height / AnimationAmount;
            recDestiny = new Rectangle(position, recSource.Size);

            // Demonstra que o personagem está sofrendo dano
            if (hurt) color = new SFML.Graphics.Color(205, 125, 125);

            // Desenha o personagem e sua sombra
            Render(Tex_Shadow, recDestiny.Location.X, recDestiny.Location.Y + size.Height / AnimationAmount - Graphics.Size(Tex_Shadow).Height + 5, 0, 0, size.Width / AnimationAmount, Graphics.Size(Tex_Shadow).Height);
            Render(Tex_Character[textureNum], recSource, recDestiny, color);
        }

        private static void Player_Character(Player player)
        {
            // Desenha o jogador
            Player_Texture(player);
            Player_Name(player);
            Player_Bars(player);
        }

        private static void Player_Texture(Player player)
        {
            byte column = AnimationStopped;
            bool hurt = false;

            // Previne sobrecargas
            if (player.Texture_Num <= 0 || player.Texture_Num > Tex_Character.GetUpperBound(0)) return;

            // Define a animação
            if (player.Attacking && player.Attack_Timer + AttackSpeed / 2 > Environment.TickCount)
                column = AnimationAttack;
            else
            {
                if (player.X2 > 8 && player.X2 < Grid) column = player.Animation;
                if (player.X2 < -8 && player.X2 > Grid * -1) column = player.Animation;
                if (player.Y2 > 8 && player.Y2 < Grid) column = player.Animation;
                if (player.Y2 < -8 && player.Y2 > Grid * -1) column = player.Animation;
            }

            // Demonstra que o personagem está sofrendo dano
            if (player.Hurt > 0) hurt = true;

            // Desenha o jogador
            Character(player.Texture_Num, new Point(ConvertX(player.Pixel_X), ConvertY(player.Pixel_Y)), player.Direction, column, hurt);
        }

        private static void Player_Bars(Player player)
        {
            short value = player.Vital[(byte)Vitals.HP];

            // Apenas se necessário
            if (value <= 0 || value >= player.Max_Vital[(byte)Vitals.HP]) return;

            // Cálcula a largura da barra
            Size chracaterSize = Size(Tex_Character[player.Texture_Num]);
            int fullWidth = chracaterSize.Width / AnimationAmount;
            int width = (value * fullWidth) / player.Max_Vital[(byte)Vitals.HP];

            // Posição das barras
            Point position = new Point
            {
                X = ConvertX(player.Pixel_X),
                Y = ConvertY(player.Pixel_Y) + chracaterSize.Height / AnimationAmount + 4
            };

            // Desenha as barras 
            Render(Tex_Bars, position.X, position.Y, 0, 4, fullWidth, 4);
            Render(Tex_Bars, position.X, position.Y, 0, 0, width, 4);
        }

        private static void Player_Name(Player player)
        {
            Texture texture = Tex_Character[player.Texture_Num];
            int nameSize = MeasureString(player.Name);

            // Posição do texto
            Point position = new Point
            {
                X = player.Pixel_X + Size(texture).Width / AnimationAmount / 2 - nameSize / 2,
                Y = player.Pixel_Y - Size(texture).Height / AnimationAmount / 2
            };

            // Cor do texto
            SFML.Graphics.Color color;
            if (player == Player.Me)
                color = SFML.Graphics.Color.Yellow;
            else
                color = SFML.Graphics.Color.White;

            // Desenha o texto
            DrawText(player.Name, ConvertX(position.X), ConvertY(position.Y), color);
        }

        private static void NPC(TempNPC npc)
        {
            byte column = 0;
            bool hurt = false;

            // Previne sobrecargas
            if (npc.Data.Texture <= 0 || npc.Data.Texture > Tex_Character.GetUpperBound(0)) return;

            // Define a animação
            if (npc.Attacking && npc.Attack_Timer + AttackSpeed / 2 > Environment.TickCount)
                column = AnimationAttack;
            else
            {
                if (npc.X2 > 8 && npc.X2 < Grid) column = npc.Animation;
                else if (npc.X2 < -8 && npc.X2 > Grid * -1) column = npc.Animation;
                else if (npc.Y2 > 8 && npc.Y2 < Grid) column = npc.Animation;
                else if (npc.Y2 < -8 && npc.Y2 > Grid * -1) column = npc.Animation;
            }

            // Demonstra que o personagem está sofrendo dano
            if (npc.Hurt > 0) hurt = true;

            // Desenha o jogador
            Character(npc.Data.Texture, new Point(ConvertX(npc.Pixel_X), ConvertY(npc.Pixel_Y)), npc.Direction, column, hurt);
            NPC_Name(npc);
            NPC_Bars(npc);
        }

        private static void NPC_Name(TempNPC npc)
        {
            Point position = new Point();
            SFML.Graphics.Color color;
            int nameSize = MeasureString(npc.Data.Name);
            Texture texture = Tex_Character[npc.Data.Texture];

            // Posição do texto
            position.X = npc.Pixel_X + Size(texture).Width / AnimationAmount / 2 - nameSize / 2;
            position.Y = npc.Pixel_Y - Size(texture).Height / AnimationAmount / 2;

            // Cor do texto
            switch ((NPCs)npc.Data.Type)
            {
                case NPCs.Friendly: color = SFML.Graphics.Color.White; break;
                case NPCs.AttackOnSight: color = SFML.Graphics.Color.Red; break;
                case NPCs.AttackWhenAttacked: color = new SFML.Graphics.Color(228, 120, 51); break;
                default: color = SFML.Graphics.Color.White; break;
            }

            // Desenha o texto
            DrawText(npc.Data.Name, ConvertX(position.X), ConvertY(position.Y), color);
        }

        private static void NPC_Bars(TempNPC npc)
        {
            Texture texture = Tex_Character[npc.Data.Texture];
            short value = npc.Vital[(byte)Vitals.HP];

            // Apenas se necessário
            if (value <= 0 || value >= npc.Data.Vital[(byte)Vitals.HP]) return;

            // Posição
            Point position = new Point(ConvertX(npc.Pixel_X), ConvertY(npc.Pixel_Y) + Size(texture).Height / AnimationAmount + 4);
            int fullWidth = Size(texture).Width / AnimationAmount;
            int width = (value * fullWidth) / npc.Data.Vital[(byte)Vitals.HP];

            // Desenha a barra 
            Render(Tex_Bars, position.X, position.Y, 0, 4, fullWidth, 4);
            Render(Tex_Bars, position.X, position.Y, 0, 0, width, 4);
        }

        private static void Map_Tiles(byte c)
        {
            // Previne erros
            if (Mapper.Current.Data.Name == null) return;

            // Dados
            System.Drawing.Color tempColor = System.Drawing.Color.FromArgb(Mapper.Current.Data.Color);
            SFML.Graphics.Color color = CColor(tempColor.R, tempColor.G, tempColor.B);

            // Desenha todas as camadas dos azulejos
            for (var x = Camera.Tile_Sight.X; x <= Camera.Tile_Sight.Width; x++)
            for (var y = Camera.Tile_Sight.Y; y <= Camera.Tile_Sight.Height; y++)
                if (!Mapper.OutOfLimit(x, y))
                    for (byte q = 0; q <= Mapper.Current.Data.Tile[x, y].Data.GetUpperBound(1); q++)
                        if (Mapper.Current.Data.Tile[x, y].Data[c, q].Tile > 0)
                        {
                            int x2 = Mapper.Current.Data.Tile[x, y].Data[c, q].X * Grid;
                            int y2 = Mapper.Current.Data.Tile[x, y].Data[c, q].Y * Grid;

                            // Desenha o azulejo
                            if (!Mapper.Current.Data.Tile[x, y].Data[c, q].Automatic)
                                Render(Tex_Tile[Mapper.Current.Data.Tile[x, y].Data[c, q].Tile], ConvertX(x * Grid), ConvertY(y * Grid), x2, y2, Grid, Grid, color);
                            else
                                Map_Autotile(new Point(ConvertX(x * Grid), ConvertY(y * Grid)), Mapper.Current.Data.Tile[x, y].Data[c, q], color);
                        }
        }

        private static void Map_Autotile(Point position, MapTileData data, SFML.Graphics.Color cor)
        {
            // Desenha os 4 mini azulejos
            for (byte i = 0; i < 4; i++)
            {
                Point destiny = position, source = data.Mini[i];

                // Partes do azulejo
                switch (i)
                {
                    case 1: destiny.X += 16; break;
                    case 2: destiny.Y += 16; break;
                    case 3: destiny.X += 16; destiny.Y += 16; break;
                }

                // Renderiza o mini azulejo
                Render(Tex_Tile[data.Tile], new Rectangle(source.X, source.Y, 16, 16), new Rectangle(destiny, new Size(16, 16)), cor);
            }
        }

        private static void Map_Panorama()
        {
            // Desenha o panorama
            if (Mapper.Current.Data.Panorama > 0)
                Render(Tex_Panorama[Mapper.Current.Data.Panorama], new Point(0));
        }

        private static void Map_Fog()
        {
            MapFog data = Mapper.Current.Data.Fog;
            Size textureSize = Size(Tex_Fog[data.Texture]);

            // Previne erros
            if (data.Texture <= 0) return;

            // Desenha a fumaça
            for (int x = -1; x <= MapWidth * Grid / textureSize.Width; x++)
            for (int y = -1; y <= MapHeight * Grid / textureSize.Height; y++)
                Render(Tex_Fog[data.Texture], new Point(x * textureSize.Width + Mapper.Fog_X, y * textureSize.Height + Mapper.Fog_Y), new SFML.Graphics.Color(255, 255, 255, data.Alpha));
        }

        private static void Map_Weather()
        {
            byte x = 0;

            // Somente se necessário
            if (Mapper.Current.Data.Weather.Type == 0) return;

            // Textura
            switch ((Weathers)Mapper.Current.Data.Weather.Type)
            {
                case Weathers.Snowing: x = 32; break;
            }

            // Desenha as partículas
            for (int i = 0; i < TempMap.Weather.Length; i++)
                if (TempMap.Weather[i].Visible)
                    Render(Tex_Weather, new Rectangle(x, 0, 32, 32), new Rectangle(TempMap.Weather[i].X, TempMap.Weather[i].Y, 32, 32), CColor(255, 255, 255, 150));

            // Trovoadas
            Render(Tex_Blanc, 0, 0, 0, 0, ScreenWidth, ScreenHeight, new SFML.Graphics.Color(255, 255, 255, Mapper.Lightning));
        }

        private static void Map_Name()
        {
            SFML.Graphics.Color color;

            // Somente se necessário
            if (string.IsNullOrEmpty(Mapper.Current.Data.Name)) return;

            // A cor do texto vária de acordo com a moral do mapa
            switch (Mapper.Current.Data.Moral)
            {
                case (byte)Morals.Danger: color = SFML.Graphics.Color.Red; break;
                default: color = SFML.Graphics.Color.White; break;
            }

            // Desenha o nome do mapa
            DrawText(Mapper.Current.Data.Name, 426, 48, color);
        }

        private static void Map_Items()
        {
            // Desenha todos os itens que estão no chão
            for (byte i = 0; i < Mapper.Current.Item.Length; i++)
            {
                MapItems data = Mapper.Current.Item[i];

                // Somente se necessário
                if (data.Item == null) continue;

                // Desenha o item
                Point position = new Point(ConvertX(data.X * Grid), ConvertY(data.Y * Grid));
                Render(Tex_Item[data.Item.Texture], position);
            }
        }

        private static void Map_Blood()
        {
            // Desenha todos os sangues
            for (byte i = 0; i < Mapper.Current.Blood.Count; i++)
            {
                MapBlood data = Mapper.Current.Blood[i];
                Render(Tex_Blood, ConvertX(data.X * Grid), ConvertY(data.Y * Grid), data.Texture_Num * 32, 0, 32, 32, CColor(255, 255, 255, data.Opacity));
            }
        }
    }
}