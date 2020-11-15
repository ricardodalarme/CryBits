using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Client.UI;
using CryBits.Entities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static CryBits.Client.Logic.Game;
using static CryBits.Client.Logic.Utils;
using static CryBits.Utils;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;

namespace CryBits.Client.Media
{
    internal static class Graphics
    {
        // Locais de renderização
        public static RenderWindow RenderWindow;

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
        public static Texture[] TexLight;
        public static Texture[] TexItem;
        public static Texture TexCheckBox;
        public static Texture TexTextBox;
        public static Texture TexWeather;
        public static Texture TexBlanc;
        public static Texture TexShadow;
        public static Texture TexBars;
        public static Texture TexBarsPanel;
        public static Texture TexEquipments;
        public static Texture TexBlood;
        public static Texture TexPartyBars;

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
            FontDefault = new Font(Directories.Fonts.FullName + "Georgia.ttf");

            // Carrega as texturas
            LoadTextures();

            // Inicia a janela
            RenderWindow = new RenderWindow(new VideoMode(800, 608), GameName, Styles.Close);
            RenderWindow.Closed += Windows.OnClosed;
            RenderWindow.MouseButtonPressed += Windows.OnMouseButtonPressed;
            RenderWindow.MouseMoved += Windows.OnMouseMoved;
            RenderWindow.MouseButtonReleased += Windows.OnMouseButtonReleased;
            RenderWindow.KeyPressed += Windows.OnKeyPressed;
            RenderWindow.KeyReleased += Windows.OnKeyReleased;
            RenderWindow.TextEntered += Windows.OnTextEntered;
        }

        private static void LoadTextures()
        {
            // Conjuntos
            TexCharacter = LoadTextures(Directories.TexCharacters.FullName);
            TexTile = LoadTextures(Directories.TexTiles.FullName);
            TexFace = LoadTextures(Directories.TexFaces.FullName);
            TexPanel = LoadTextures(Directories.TexPanels.FullName);
            TexButton = LoadTextures(Directories.TexButtons.FullName);
            TexPanorama = LoadTextures(Directories.TexPanoramas.FullName);
            TexFog = LoadTextures(Directories.TexFogs.FullName);
            TexLight = LoadTextures(Directories.TexLights.FullName);
            TexItem = LoadTextures(Directories.TexItems.FullName);

            // Únicas
            TexWeather = new Texture(Directories.TexWeather.FullName + Format);
            TexBlanc = new Texture(Directories.TexBlank.FullName + Format);
            TexCheckBox = new Texture(Directories.TexCheckBox.FullName + Format);
            TexTextBox = new Texture(Directories.TexTextBox.FullName + Format);
            TexShadow = new Texture(Directories.TexShadow.FullName + Format);
            TexBars = new Texture(Directories.TexBars.FullName + Format);
            TexBarsPanel = new Texture(Directories.TexBarsPanel.FullName + Format);
            TexEquipments = new Texture(Directories.TexEquipments.FullName + Format);
            TexBlood = new Texture(Directories.TexBlood.FullName + Format);
            TexPartyBars = new Texture(Directories.TexPartyBars.FullName + Format);
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
            return new Size(0, 0);
        }

        private static Color CColor(byte r = 255, byte g = 255, byte b = 255, byte a = 255) => new Color(r, g, b, a);

        private static void Render(Texture texture, Rectangle recSource, Rectangle recDestiny, object color = null, object mode = null)
        {
            Sprite tmpImage = new Sprite(texture);

            // Define os dados
            tmpImage.TextureRect = new IntRect(recSource.X, recSource.Y, recSource.Width, recSource.Height);
            tmpImage.Position = new Vector2f(recDestiny.X, recDestiny.Y);
            tmpImage.Scale = new Vector2f(recDestiny.Width / (float)recSource.Width, recDestiny.Height / (float)recSource.Height);
            if (color != null)
                tmpImage.Color = (Color)color;

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

        private static void DrawText(string text, int x, int y, Color color, Alignments alignment = Alignments.Left)
        {
            Text tempText = new Text(text, FontDefault);

            // Alinhamento do texto
            switch (alignment)
            {
                case Alignments.Center: x -= MeasureString(text) / 2; break;
                case Alignments.Right: x -= MeasureString(text); break;
            }

            // Define os dados
            tempText.CharacterSize = 10;
            tempText.FillColor = color;
            tempText.Position = new Vector2f(x, y);
            tempText.OutlineColor = new Color(0, 0, 0, 70);
            tempText.OutlineThickness = 1;

            // Desenha
            RenderWindow.Draw(tempText);
        }

        private static void DrawText(string text, int x, int y, Color color, int maxWidth, bool cut = true)
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
            int textureWidth = Size(texture).Width;
            int textureHeight = Size(texture).Height;

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
            RenderWindow.Clear(Color.Black);

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
            if (Option.FPS) DrawText("FPS: " + FPS, 176, 7, Color.White);
            if (Option.Latency) DrawText("Latency: " + Socket.Latency, 176, 19, Color.White);
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
            Render(TexButton[tool.TextureNum], tool.Position, new Color(255, 255, 225, alpha));
        }

        private static void Panel(Panels tool)
        {
            // Desenha o painel
            Render(TexPanel[tool.TextureNum], tool.Position);
        }

        private static void CheckBox(CheckBoxes tool)
        {
            // Define as propriedades dos retângulos
            Rectangle recSource = new Rectangle(new Point(), new Size(Size(TexCheckBox).Width / 2, Size(TexCheckBox).Height));
            Rectangle recDestiny = new Rectangle(tool.Position, recSource.Size);

            // Desenha a textura do marcador pelo seu estado 
            if (tool.Checked) recSource.Location = new Point(Size(TexCheckBox).Width / 2, 0);

            // Desenha o marcador 
            Render(TexCheckBox, recSource, recDestiny);
            DrawText(tool.Text, recDestiny.Location.X + Size(TexCheckBox).Width / 2 + CheckBoxes.Margin, recDestiny.Location.Y + 1, Color.White);
        }

        private static void TextBox(TextBoxes tool)
        {
            Point position = tool.Position;
            string text = tool.Text;

            // Desenha a ferramenta
            Render_Box(TexTextBox, 3, tool.Position, new Size(tool.Width, Size(TexTextBox).Height));

            // Altera todos os caracteres do texto para um em especifico, se for necessário
            if (tool.Password && !string.IsNullOrEmpty(text)) text = new string('•', text.Length);

            // Quebra o texto para que caiba no digitalizador, se for necessário
            text = TextBreak(text, tool.Width - 10);

            // Desenha o texto do digitalizador
            if (TextBoxes.Focused != null && (TextBoxes)TextBoxes.Focused.Data == tool && TextBoxes.Signal) text += "|";
            DrawText(text, position.X + 4, position.Y + 2, Color.White);
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
                case "Information": Information((Panels)tool); break;
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
                DrawText(text, textPosition.X, textPosition.Y, Color.White, Alignments.Center);
                return;
            }

            // Verifica se o personagem existe
            if (Panels.SelectCharacter >= Panels.Characters.Length)
            {
                DrawText(text, textPosition.X, textPosition.Y, Color.White, Alignments.Center);
                return;
            }

            // Desenha o personagem
            short textureNum = Panels.Characters[Panels.SelectCharacter].TextureNum;
            if (textureNum > 0)
            {
                Render(TexFace[textureNum], new Point(353, 442));
                Character(textureNum, new Point(356, 534 - Size(TexCharacter[textureNum]).Height / 4), Directions.Down, AnimationStopped);
            }

            // Desenha o nome da classe
            text = "(" + (Panels.SelectCharacter + 1) + ") " + Panels.Characters[Panels.SelectCharacter].Name;
            DrawText(text, textPosition.X, textPosition.Y, Color.White, Alignments.Center);
        }

        private static void CreateCharacter_Class()
        {
            short textureNum = 0;
            Class @class = Class.List.ElementAt(Panels.CreateCharacterClass).Value;

            // Textura do personagem
            if (CheckBoxes.List["GenderMale"].Checked && @class.TexMale.Count > 0)
                textureNum = @class.TexMale[Panels.CreateCharacterTex];
            else if (@class.TexFemale.Count > 0)
                textureNum = @class.TexFemale[Panels.CreateCharacterTex];

            // Desenha o personagem
            if (textureNum > 0)
            {
                Render(TexFace[textureNum], new Point(425, 440));
                Character(textureNum, new Point(433, 501), Directions.Down, AnimationStopped);
            }

            // Desenha o nome da classe
            string text = @class.Name;
            DrawText(text, 347, 509, Color.White, Alignments.Center);

            // Descrição
            DrawText(@class.Description, 282, 526, Color.White, 123);
        }
        #endregion

        private static void Bars(Panels tool)
        {
            decimal hpPercentage = Player.Me.Vital[(byte)Vitals.HP] / (decimal)Player.Me.MaxVital[(byte)Vitals.HP];
            decimal mpPercentage = Player.Me.Vital[(byte)Vitals.MP] / (decimal)Player.Me.MaxVital[(byte)Vitals.MP];
            decimal expPercentage = Player.Me.Experience / (decimal)Player.Me.ExpNeeded;

            // Barras
            Render(TexBarsPanel, tool.Position.X + 6, tool.Position.Y + 6, 0, 0, (int)(TexBarsPanel.Size.X * hpPercentage), 17);
            Render(TexBarsPanel, tool.Position.X + 6, tool.Position.Y + 24, 0, 18, (int)(TexBarsPanel.Size.X * mpPercentage), 17);
            Render(TexBarsPanel, tool.Position.X + 6, tool.Position.Y + 42, 0, 36, (int)(TexBarsPanel.Size.X * expPercentage), 17);

            // Textos 
            DrawText("HP", tool.Position.X + 10, tool.Position.Y + 3, Color.White);
            DrawText("MP", tool.Position.X + 10, tool.Position.Y + 21, Color.White);
            DrawText("Exp", tool.Position.X + 10, tool.Position.Y + 39, Color.White);

            // Indicadores
            DrawText(Player.Me.Vital[(byte)Vitals.HP] + "/" + Player.Me.MaxVital[(byte)Vitals.HP], tool.Position.X + 76, tool.Position.Y + 7, Color.White, Alignments.Center);
            DrawText(Player.Me.Vital[(byte)Vitals.MP] + "/" + Player.Me.MaxVital[(byte)Vitals.MP], tool.Position.X + 76, tool.Position.Y + 25, Color.White, Alignments.Center);
            DrawText(Player.Me.Experience + "/" + Player.Me.ExpNeeded, tool.Position.X + 76, tool.Position.Y + 43, Color.White, Alignments.Center);
        }

        private static void Chat()
        {
            Panels tool = Panels.List["Chat"];
            tool.Visible = TextBoxes.Focused != null && ((TextBoxes)TextBoxes.Focused.Data).Name.Equals("Chat");

            // Renderiza as mensagens
            if (tool.Visible || Loop.ChatTimer >= Environment.TickCount && Option.Chat)
                for (byte i = UI.Chat.LinesFirst; i <= UI.Chat.LinesVisible + UI.Chat.LinesFirst; i++)
                    if (UI.Chat.Order.Count > i)
                        DrawText(UI.Chat.Order[i].Text, 16, 461 + 11 * (i - UI.Chat.LinesFirst), UI.Chat.Order[i].Color);

            // Dica de como abrir o chat
            if (!tool.Visible) DrawText("Press [Enter] to open chat.", TextBoxes.List["Chat"].Position.X + 5, TextBoxes.List["Chat"].Position.Y + 3, Color.White);
        }

        private static void Information(Panels tool)
        {
            Item item = CryBits.Entities.Item.Get(Panels.InformationID);
            Color textColor;
            List<string> data = new List<string>();

            // Apenas se necessário
            if (item == null) return;

            // Define a cor de acordo com a raridade
            switch (item.Rarity)
            {
                case Rarity.Uncommon: textColor = CColor(204, 255, 153); break; // Verde
                case Rarity.Rare: textColor = CColor(102, 153); break; // Azul
                case Rarity.Epic: textColor = CColor(153, 0, 204); break; // Roxo
                case Rarity.Legendary: textColor = CColor(255, 255, 77); break; // Amarelo
                default: textColor = CColor(); break; // Branco
            }

            // Nome, descrição e icone do item
            DrawText(item.Name, tool.Position.X + 41, tool.Position.Y + 6, textColor, Alignments.Center);
            DrawText(item.Description, tool.Position.X + 82, tool.Position.Y + 20, Color.White, 86);
            Render(TexItem[item.Texture], new Rectangle(tool.Position.X + 9, tool.Position.Y + 21, 64, 64));

            // Informações da Loja
            if (Panels.List["Shop"].Visible)
                if (Panels.ShopSlot >= 0)
                    data.Add("Price: " + Panels.ShopOpen.Sold[Panels.ShopSlot].Price);
                else if (Panels.InventorySlot > 0)
                    if (Panels.ShopOpen.FindBought(item) != null)
                        data.Add("Sale price: " + Panels.ShopOpen.FindBought(item).Price);

            // Informações específicas dos itens
            switch (item.Type)
            {
                // Poção
                case Items.Potion:
                    for (byte n = 0; n < (byte)Vitals.Count; n++)
                        if (item.PotionVital[n] != 0)
                            data.Add((Vitals)n + ": " + item.PotionVital[n]);

                    if (item.PotionExperience != 0) data.Add("Experience: " + item.PotionExperience);
                    break;
                // Equipamentos
                case Items.Equipment:
                    if (item.EquipType == (byte)Equipments.Weapon)
                        if (item.WeaponDamage != 0)
                            data.Add("Damage: " + item.WeaponDamage);

                    for (byte n = 0; n < (byte)Attributes.Count; n++)
                        if (item.EquipAttribute[n] != 0)
                            data.Add((Attributes)n + ": " + item.EquipAttribute[n]);
                    break;
            }

            // Desenha todos os dados necessários
            Point[] positions = { new Point(tool.Position.X + 10, tool.Position.Y + 90), new Point(tool.Position.X + 10, tool.Position.Y + 102), new Point(tool.Position.X + 10, tool.Position.Y + 114), new Point(tool.Position.X + 96, tool.Position.Y + 90), new Point(tool.Position.X + 96, tool.Position.Y + 102), new Point(tool.Position.X + 96, tool.Position.Y + 114), new Point(tool.Position.X + 96, tool.Position.Y + 126) };
            for (byte i = 0; i < data.Count; i++) DrawText(data[i], positions[i].X, positions[i].Y, Color.White);
        }

        private static void Hotbar(Panels tool)
        {
            string indicator = string.Empty;

            // Desenha os objetos da hotbar
            for (byte i = 0; i < MaxHotbar; i++)
            {
                byte slot = Player.Me.Hotbar[i].Slot;
                if (slot > 0)
                    switch ((Hotbars)Player.Me.Hotbar[i].Type)
                    {
                        // Itens
                        case Hotbars.Item: Item(Player.Me.Inventory[slot].Item, 1, tool.Position + new Size(8, 6), (byte)(i + 1), 10); break;
                    }

                // Desenha os números de cada slot
                if (i < 10) indicator = (i + 1).ToString();
                else if (i == 9) indicator = "0";
                DrawText(indicator, tool.Position.X + 16 + 36 * i, tool.Position.Y + 22, Color.White);
            }

            // Movendo slot
            if (Panels.HotbarChange >= 0)
                if (Player.Me.Hotbar[Panels.HotbarChange].Type == (byte)Hotbars.Item)
                    Render(TexItem[Player.Me.Inventory[Player.Me.Hotbar[Panels.HotbarChange].Slot].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
        }

        private static void Menu_Character(Panels tool)
        {
            // Dados básicos
            DrawText(Player.Me.Name, tool.Position.X + 18, tool.Position.Y + 52, Color.White);
            DrawText(Player.Me.Level.ToString(), tool.Position.X + 18, tool.Position.Y + 79, Color.White);
            Render(TexFace[Player.Me.TextureNum], new Point(tool.Position.X + 82, tool.Position.Y + 37));

            // Atributos
            DrawText("Strength: " + Player.Me.Attribute[(byte)Attributes.Strength], tool.Position.X + 32, tool.Position.Y + 146, Color.White);
            DrawText("Resistance: " + Player.Me.Attribute[(byte)Attributes.Resistance], tool.Position.X + 32, tool.Position.Y + 162, Color.White);
            DrawText("Intelligence: " + Player.Me.Attribute[(byte)Attributes.Intelligence], tool.Position.X + 32, tool.Position.Y + 178, Color.White);
            DrawText("Agility: " + Player.Me.Attribute[(byte)Attributes.Agility], tool.Position.X + 32, tool.Position.Y + 194, Color.White);
            DrawText("Vitality: " + Player.Me.Attribute[(byte)Attributes.Vitality], tool.Position.X + 32, tool.Position.Y + 210, Color.White);
            DrawText("Points: " + Player.Me.Points, tool.Position.X + 14, tool.Position.Y + 228, Color.White);

            // Equipamentos 
            for (byte i = 0; i < (byte)Equipments.Count; i++)
                if (Player.Me.Equipment[i] == null)
                    Render(TexEquipments, tool.Position.X + 7 + i * 34, tool.Position.Y + 247, i * 34, 0, 32, 32);
                else
                    Render(TexItem[Player.Me.Equipment[i].Texture], tool.Position.X + 8 + i * 35, tool.Position.Y + 247, 0, 0, 34, 34);
        }

        private static void Menu_Inventory(Panels tool)
        {
            byte numColumns = 5;

            // Desenha todos os itens do inventário
            for (byte i = 0; i < MaxInventory; i++)
                Item(Player.Me.Inventory[i].Item, Player.Me.Inventory[i].Amount, tool.Position + new Size(7, 30), i, numColumns);

            // Movendo item
            if (Panels.InventoryChange > 0) Render(TexItem[Player.Me.Inventory[Panels.InventoryChange].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
        }

        private static void Party_Invitation(Panels tool)
        {
            DrawText(Panels.PartyInvitation + " has invite you to a party. Would you like to join?", tool.Position.X + 14, tool.Position.Y + 33, Color.White, 160);
        }

        private static void Party()
        {
            for (byte i = 0; i < Player.Me.Party.Length; i++)
            {
                // Barras do membro
                Render(TexPartyBars, 10, 92 + 27 * i, 0, 0, 82, 8); // HP Cinza
                Render(TexPartyBars, 10, 99 + 27 * i, 0, 0, 82, 8); // MP Cinza
                if (Player.Me.Party[i].Vital[(byte)Vitals.HP] > 0)
                    Render(TexPartyBars, 10, 92 + 27 * i, 0, 8, Player.Me.Party[i].Vital[(byte)Vitals.HP] * 82 / Player.Me.Party[i].MaxVital[(byte)Vitals.HP], 8); // HP 
                if (Player.Me.Party[i].Vital[(byte)Vitals.MP] > 0)
                    Render(TexPartyBars, 10, 99 + 27 * i, 0, 16, Player.Me.Party[i].Vital[(byte)Vitals.MP] * 82 / Player.Me.Party[i].MaxVital[(byte)Vitals.MP], 8); // MP 

                // Nome do membro
                DrawText(Player.Me.Party[i].Name, 10, 79 + 27 * i, Color.White);
            }
        }

        private static void Trade_Invitation(Panels tool)
        {
            DrawText(Panels.TradeInvitation + " has invite you to a trade. Would you like to join?", tool.Position.X + 14, tool.Position.Y + 33, Color.White, 160);
        }

        private static void Trade(Panels tool)
        {
            // Desenha os itens das ofertas
            for (byte i = 0; i < MaxInventory; i++)
            {
                Item(Player.Me.TradeOffer[i].Item, Player.Me.TradeOffer[i].Amount, tool.Position + new Size(7, 50), i, 5);
                Item(Player.Me.TradeTheirOffer[i].Item, Player.Me.TradeTheirOffer[i].Amount, tool.Position + new Size(192, 50), i, 5);
            }
        }

        private static void Shop(Panels tool)
        {
            // Dados da loja
            string name = Panels.ShopOpen.Name;
            DrawText(name, tool.Position.X + 131, tool.Position.Y + 28, Color.White, Alignments.Center);
            DrawText("Currency: " + Panels.ShopOpen.Currency.Name, tool.Position.X + 10, tool.Position.Y + 195, Color.White);

            // Desenha os itens
            for (byte i = 0; i < Panels.ShopOpen.Sold.Count; i++)
                Item(Panels.ShopOpen.Sold[i].Item, Panels.ShopOpen.Sold[i].Amount, tool.Position + new Size(7, 50), (byte)(i + 1), 7);
        }

        private static void Item(Item item, short amount, Point start, byte slot, byte columns, byte grid = 32, byte gap = 4)
        {
            // Somente se necessário
            if (item == null) return;

            // Posição do item baseado no slot
            int line = (slot - 1) / columns;
            int column = slot - line * 5 - 1;
            Point position = start + new Size(column * (grid + gap), line * (grid + gap));

            // Desenha o item e sua quantidade
            Render(TexItem[item.Texture], position);
            if (amount > 1) DrawText(amount.ToString(), position.X + 2, position.Y + 17, Color.White);
        }

        private static void Character(short textureNum, Point position, Directions direction, byte column, bool hurt = false)
        {
            Rectangle recSource = new Rectangle(), recDestiny;
            Size size = Size(TexCharacter[textureNum]);
            Color color = new Color(255, 255, 255);
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
            if (hurt) color = new Color(205, 125, 125);

            // Desenha o personagem e sua sombra
            Render(TexShadow, recDestiny.Location.X, recDestiny.Location.Y + size.Height / AnimationAmount - Size(TexShadow).Height + 5, 0, 0, size.Width / AnimationAmount, Size(TexShadow).Height);
            Render(TexCharacter[textureNum], recSource, recDestiny, color);
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
            if (player.TextureNum <= 0 || player.TextureNum > TexCharacter.GetUpperBound(0)) return;

            // Define a animação
            if (player.Attacking && player.AttackTimer + AttackSpeed / 2 > Environment.TickCount)
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
            Character(player.TextureNum, new Point(ConvertX(player.PixelX), ConvertY(player.PixelY)), player.Direction, column, hurt);
        }

        private static void Player_Bars(Player player)
        {
            short value = player.Vital[(byte)Vitals.HP];

            // Apenas se necessário
            if (value <= 0 || value >= player.MaxVital[(byte)Vitals.HP]) return;

            // Cálcula a largura da barra
            Size characterSize = Size(TexCharacter[player.TextureNum]);
            int fullWidth = characterSize.Width / AnimationAmount;
            int width = value * fullWidth / player.MaxVital[(byte)Vitals.HP];

            // Posição das barras
            Point position = new Point
            {
                X = ConvertX(player.PixelX),
                Y = ConvertY(player.PixelY) + characterSize.Height / AnimationAmount + 4
            };

            // Desenha as barras 
            Render(TexBars, position.X, position.Y, 0, 4, fullWidth, 4);
            Render(TexBars, position.X, position.Y, 0, 0, width, 4);
        }

        private static void Player_Name(Player player)
        {
            Texture texture = TexCharacter[player.TextureNum];
            int nameSize = MeasureString(player.Name);

            // Posição do texto
            Point position = new Point
            {
                X = player.PixelX + Size(texture).Width / AnimationAmount / 2 - nameSize / 2,
                Y = player.PixelY - Size(texture).Height / AnimationAmount / 2
            };

            // Cor do texto
            Color color = player == Player.Me ? Color.Yellow : Color.White;

            // Desenha o texto
            DrawText(player.Name, ConvertX(position.X), ConvertY(position.Y), color);
        }

        private static void NPC(TempNPC npc)
        {
            byte column = 0;
            bool hurt = false;

            // Previne sobrecargas
            if (npc.Data.Texture <= 0 || npc.Data.Texture > TexCharacter.GetUpperBound(0)) return;

            // Define a animação
            if (npc.Attacking && npc.AttackTimer + AttackSpeed / 2 > Environment.TickCount)
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
            Character(npc.Data.Texture, new Point(ConvertX(npc.PixelX), ConvertY(npc.PixelY)), npc.Direction, column, hurt);
            NPC_Name(npc);
            NPC_Bars(npc);
        }

        private static void NPC_Name(TempNPC npc)
        {
            Point position = new Point();
            Color color;
            int nameSize = MeasureString(npc.Data.Name);
            Texture texture = TexCharacter[npc.Data.Texture];

            // Posição do texto
            position.X = npc.PixelX + Size(texture).Width / AnimationAmount / 2 - nameSize / 2;
            position.Y = npc.PixelY - Size(texture).Height / AnimationAmount / 2;

            // Cor do texto
            switch (npc.Data.Behaviour)
            {
                case NPCs.Friendly: color = Color.White; break;
                case NPCs.AttackOnSight: color = Color.Red; break;
                case NPCs.AttackWhenAttacked: color = new Color(228, 120, 51); break;
                default: color = Color.White; break;
            }

            // Desenha o texto
            DrawText(npc.Data.Name, ConvertX(position.X), ConvertY(position.Y), color);
        }

        private static void NPC_Bars(TempNPC npc)
        {
            Texture texture = TexCharacter[npc.Data.Texture];
            short value = npc.Vital[(byte)Vitals.HP];

            // Apenas se necessário
            if (value <= 0 || value >= npc.Data.Vital[(byte)Vitals.HP]) return;

            // Posição
            Point position = new Point(ConvertX(npc.PixelX), ConvertY(npc.PixelY) + Size(texture).Height / AnimationAmount + 4);
            int fullWidth = Size(texture).Width / AnimationAmount;
            int width = value * fullWidth / npc.Data.Vital[(byte)Vitals.HP];

            // Desenha a barra 
            Render(TexBars, position.X, position.Y, 0, 4, fullWidth, 4);
            Render(TexBars, position.X, position.Y, 0, 0, width, 4);
        }

        private static void Map_Tiles(byte layerType)
        {
            // Previne erros
            if (Mapper.Current.Data.Name == null) return;

            // Dados
            System.Drawing.Color tempColor = Mapper.Current.Data.Color;
            Color color = CColor(tempColor.R, tempColor.G, tempColor.B);
            Map map = Mapper.Current.Data;

            // Desenha todas as camadas dos azulejos
            for (byte c = 0; c < map.Layer.Count; c++)
                if (c == layerType)
                    for (int x = Camera.TileSight.X; x <= Camera.TileSight.Width; x++)
                        for (int y = Camera.TileSight.Y; y <= Camera.TileSight.Height; y++)
                            if (!Mapper.OutOfLimit(x, y))
                            {
                                MapTileData data = map.Layer[c].Tile[x, y];
                                if (data.Texture > 0)
                                {
                                    int x2 = data.X * Grid;
                                    int y2 = data.Y * Grid;

                                    // Desenha o azulejo
                                    if (!map.Layer[c].Tile[x, y].IsAutoTile)
                                        Render(TexTile[data.Texture], ConvertX(x * Grid), ConvertY(y * Grid), x2, y2, Grid, Grid, color);
                                    else
                                        Map_AutoTile(new Point(ConvertX(x * Grid), ConvertY(y * Grid)), data, color);
                                }
                            }
        }

        private static void Map_AutoTile(Point position, MapTileData data, Color cor)
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
                Render(TexTile[data.Texture], new Rectangle(source.X, source.Y, 16, 16), new Rectangle(destiny, new Size(16, 16)), cor);
            }
        }

        private static void Map_Panorama()
        {
            // Desenha o panorama
            if (Mapper.Current.Data.Panorama > 0)
                Render(TexPanorama[Mapper.Current.Data.Panorama], new Point(0));
        }

        private static void Map_Fog()
        {
            MapFog data = Mapper.Current.Data.Fog;
            Size textureSize = Size(TexFog[data.Texture]);

            // Previne erros
            if (data.Texture <= 0) return;

            // Desenha a fumaça
            for (int x = -1; x <= MapWidth * Grid / textureSize.Width; x++)
                for (int y = -1; y <= MapHeight * Grid / textureSize.Height; y++)
                    Render(TexFog[data.Texture], new Point(x * textureSize.Width + Mapper.FogX, y * textureSize.Height + Mapper.FogY), new Color(255, 255, 255, data.Alpha));
        }

        private static void Map_Weather()
        {
            byte x = 0;

            // Somente se necessário
            if (Mapper.Current.Data.Weather.Type == 0) return;

            // Textura
            switch (Mapper.Current.Data.Weather.Type)
            {
                case Weathers.Snowing: x = 32; break;
            }

            // Desenha as partículas
            foreach (var weather in TempMap.Weather)
                if (weather.Visible)
                    Render(TexWeather, new Rectangle(x, 0, 32, 32), new Rectangle(weather.X, weather.Y, 32, 32), CColor(255, 255, 255, 150));

            // Trovoadas
            Render(TexBlanc, 0, 0, 0, 0, ScreenWidth, ScreenHeight, new Color(255, 255, 255, Mapper.Lightning));
        }

        private static void Map_Name()
        {
            Color color;

            // Somente se necessário
            if (string.IsNullOrEmpty(Mapper.Current.Data.Name)) return;

            // A cor do texto vária de acordo com a moral do mapa
            switch (Mapper.Current.Data.Moral)
            {
                case Morals.Dangerous: color = Color.Red; break;
                default: color = Color.White; break;
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
                Render(TexItem[data.Item.Texture], position);
            }
        }

        private static void Map_Blood()
        {
            // Desenha todos os sangues
            for (byte i = 0; i < Mapper.Current.Blood.Count; i++)
            {
                MapBlood data = Mapper.Current.Blood[i];
                Render(TexBlood, ConvertX(data.X * Grid), ConvertY(data.Y * Grid), data.TextureNum * 32, 0, 32, 32, CColor(255, 255, 255, data.Opacity));
            }
        }
    }
}