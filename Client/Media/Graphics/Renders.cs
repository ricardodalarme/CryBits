using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Client.UI;
using CryBits.Entities;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static CryBits.Client.Logic.Utils;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;
using Color = SFML.Graphics.Color;
using Window = CryBits.Enums.Window;

namespace CryBits.Client.Media.Graphics;

internal static class Renders
{
    // Locais de renderização
    public static RenderWindow RenderWindow;

    #region Engine
    public static void Init()
    {
        Fonts.LoadAll();
        // Carrega as texturas
        Textures.LoadAll();

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

    private static void Render(Texture texture, Rectangle recSource, Rectangle recDestiny, object color = null, object mode = null)
    {
        var tmpImage = new Sprite(texture);

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
        var source = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
        var destiny = new Rectangle(x, y, sourceWidth, sourceHeight);

        // Desenha a textura
        Render(texture, source, destiny, color);
    }

    private static void Render(Texture texture, Rectangle destiny, object color = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(new Point(0), texture.ToSize());

        // Desenha a textura
        Render(texture, source, destiny, color);
    }

    private static void Render(Texture texture, Point position, object color = null)
    {
        // Define as propriedades dos retângulos
        var source = new Rectangle(new Point(0), texture.ToSize());
        var destiny = new Rectangle(position, texture.ToSize());

        // Desenha a textura
        Render(texture, source, destiny, color);
    }

    private static void DrawText(string text, int x, int y, Color color, TextAlign alignment = TextAlign.Left)
    {
        var tempText = new Text(text, Fonts.Default);

        // Alinhamento do texto
        switch (alignment)
        {
            case TextAlign.Center: x -= MeasureString(text) / 2; break;
            case TextAlign.Right: x -= MeasureString(text); break;
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
        int messageWidth = MeasureString(text), split = -1;

        // Caso couber, adiciona a mensagem normalmente
        if (messageWidth < maxWidth)
            DrawText(text, x, y, color);
        else
            for (var i = 0; i < text.Length; i++)
            {
                // Verifica se o caráctere é um separável 
                switch (text[i])
                {
                    case '-':
                    case '_':
                    case ' ': split = i; break;
                }

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

    private static void Render_Box(Texture texture, byte margin, Point position, Size size)
    {
        var textureWidth = texture.ToSize().Width;
        var textureHeight = texture.ToSize().Height;

        // Borda esquerda
        Render(texture, new Rectangle(new Point(0), new Size(margin, textureWidth)), new Rectangle(position, new Size(margin, textureHeight)));
        // Borda direita
        Render(texture, new Rectangle(new Point(textureWidth - margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + size.Width - margin, position.Y), new Size(margin, textureHeight)));
        // Centro
        Render(texture, new Rectangle(new Point(margin, 0), new Size(margin, textureHeight)), new Rectangle(new Point(position.X + margin, position.Y), new Size(size.Width - (margin * 2), textureHeight)));
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
        if (Windows.Current == Window.Game) Chat();

        // Exibe o que foi renderizado
        RenderWindow.Display();
    }

    private static void InGame()
    {
        // Não desenhar se não estiver em jogo
        if (Windows.Current != Window.Game) return;

        // Atualiza a câmera
        Camera.Update();

        // Desenhos abaixo do jogador
        MapPanorama();
        MapTiles((byte)Layer.Ground);
        MapBlood();
        MapItems();

        // Desenha os Npcs
        for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
            if (TempMap.Current.Npc[i].Data != null)
                Npc(TempMap.Current.Npc[i]);

        // Desenha os jogadores
        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                if (Player.List[i].Map == Player.Me.Map)
                    PlayerCharacter(Player.List[i]);

        // Desenha o próprio jogador
        PlayerCharacter(Player.Me);

        // Desenhos acima do jogador
        MapTiles((byte)Layer.Fringe);
        MapWeather();
        MapFog();
        MapName();

        // Desenha os membros da party
        Party();

        // Desenha os dados do jogo
        if (Options.FPS) DrawText("FPS: " + Loop.FPS, 176, 7, Color.White);
        if (Options.Latency) DrawText("Latency: " + Socket.Latency, 176, 19, Color.White);
    }

    #region Tools
    private static void Interface(List<Tools.OrderStructure> node)
    {
        for (byte i = 0; i < node.Count; i++)
            if (node[i].Data.Visible)
            {
                // Desenha a ferramenta
                if (node[i].Data is Panels panel) Panel(panel);
                else if (node[i].Data is TextBoxes textBox) TextBox(textBox);
                else if (node[i].Data is Buttons button) Button(button);
                else if (node[i].Data is CheckBoxes checkBox) CheckBox(checkBox);

                // Desenha algumas coisas mais específicas da interface
                InterfaceSpecific(node[i].Data);

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
        Render(Textures.Buttons[tool.TextureNum], tool.Position, new Color(255, 255, 225, alpha));
    }

    private static void Panel(Panels tool)
    {
        // Desenha o painel
        Render(Textures.Panels[tool.TextureNum], tool.Position);
    }

    private static void CheckBox(CheckBoxes tool)
    {
        // Define as propriedades dos retângulos
        var recSource = new Rectangle(new Point(), new Size(Textures.CheckBox.ToSize().Width / 2, Textures.CheckBox.ToSize().Height));
        var recDestiny = new Rectangle(tool.Position, recSource.Size);

        // Desenha a textura do marcador pelo seu estado 
        if (tool.Checked) recSource.Location = new Point(Textures.CheckBox.ToSize().Width / 2, 0);

        // Desenha o marcador 
        Render(Textures.CheckBox, recSource, recDestiny);
        DrawText(tool.Text, recDestiny.Location.X + (Textures.CheckBox.ToSize().Width / 2) + CheckBoxes.Margin, recDestiny.Location.Y + 1, Color.White);
    }

    private static void TextBox(TextBoxes tool)
    {
        var position = tool.Position;
        var text = tool.Text;

        // Desenha a ferramenta
        Render_Box(Textures.TextBox, 3, tool.Position, new Size(tool.Width, Textures.TextBox.ToSize().Height));

        // Altera todos os caracteres do texto para um em especifico, se for necessário
        if (tool.Password && !string.IsNullOrEmpty(text)) text = new string('•', text.Length);

        // Quebra o texto para que caiba no digitalizador, se for necessário
        text = TextBreak(text, tool.Width - 10);

        // Desenha o texto do digitalizador
        if (TextBoxes.Focused != null && (TextBoxes)TextBoxes.Focused.Data == tool && TextBoxes.Signal) text += "|";
        DrawText(text, position.X + 4, position.Y + 2, Color.White);
    }

    private static void InterfaceSpecific(Tools.Structure tool)
    {
        // Interações especificas
        if (!(tool is Panels)) return;
        switch (tool.Name)
        {
            case "SelectCharacter": SelectCharacterClass(); break;
            case "CreateCharacter": CreateCharacterClass(); break;
            case "Hotbar": Hotbar((Panels)tool); break;
            case "Menu_Character": MenuCharacter((Panels)tool); break;
            case "Menu_Inventory": MenuInventory((Panels)tool); break;
            case "Bars": Bars((Panels)tool); break;
            case "Information": Information((Panels)tool); break;
            case "Party_Invitation": PartyInvitation((Panels)tool); break;
            case "Trade_Invitation": Trade_Invitation((Panels)tool); break;
            case "Trade": Trade((Panels)tool); break;
            case "Shop": Shop((Panels)tool); break;
        }
    }
    #endregion

    #region Menu
    private static void SelectCharacterClass()
    {
        var textPosition = new Point(399, 425);
        var text = "(" + (Panels.SelectCharacter + 1) + ") None";

        // Somente se necessário
        if (!Buttons.Characters_Change_Buttons())
        {
            DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        // Verifica se o personagem existe
        if (Panels.SelectCharacter >= Panels.Characters.Length)
        {
            DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        // Desenha o personagem
        var textureNum = Panels.Characters[Panels.SelectCharacter].TextureNum;
        if (textureNum > 0)
        {
            Render(Textures.Faces[textureNum], new Point(353, 442));
            Character(textureNum, new Point(356, 534 - (Textures.Characters[textureNum].ToSize().Height / 4)), Direction.Down, AnimationStopped);
        }

        // Desenha o nome da classe
        text = "(" + (Panels.SelectCharacter + 1) + ") " + Panels.Characters[Panels.SelectCharacter].Name;
        DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
    }

    private static void CreateCharacterClass()
    {
        short textureNum = 0;
        var @class = Class.List.ElementAt(Panels.CreateCharacterClass).Value;

        // Textura do personagem
        if (CheckBoxes.List["GenderMale"].Checked && @class.TexMale.Count > 0)
            textureNum = @class.TexMale[Panels.CreateCharacterTex];
        else if (@class.TexFemale.Count > 0)
            textureNum = @class.TexFemale[Panels.CreateCharacterTex];

        // Desenha o personagem
        if (textureNum > 0)
        {
            Render(Textures.Faces[textureNum], new Point(425, 440));
            Character(textureNum, new Point(433, 501), Direction.Down, AnimationStopped);
        }

        // Desenha o nome da classe
        var text = @class.Name;
        DrawText(text, 347, 509, Color.White, TextAlign.Center);

        // Descrição
        DrawText(@class.Description, 282, 526, Color.White, 123);
    }
    #endregion

    private static void Bars(Panels tool)
    {
        var hpPercentage = Player.Me.Vital[(byte)Vital.HP] / (decimal)Player.Me.MaxVital[(byte)Vital.HP];
        var mpPercentage = Player.Me.Vital[(byte)Vital.MP] / (decimal)Player.Me.MaxVital[(byte)Vital.MP];
        var expPercentage = Player.Me.Experience / (decimal)Player.Me.ExpNeeded;

        // Barras
        Render(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 6, 0, 0, (int)(Textures.BarsPanel.Size.X * hpPercentage), 17);
        Render(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 24, 0, 18, (int)(Textures.BarsPanel.Size.X * mpPercentage), 17);
        Render(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 42, 0, 36, (int)(Textures.BarsPanel.Size.X * expPercentage), 17);

        // Textos 
        DrawText("HP", tool.Position.X + 10, tool.Position.Y + 3, Color.White);
        DrawText("MP", tool.Position.X + 10, tool.Position.Y + 21, Color.White);
        DrawText("Exp", tool.Position.X + 10, tool.Position.Y + 39, Color.White);

        // Indicadores
        DrawText(Player.Me.Vital[(byte)Vital.HP] + "/" + Player.Me.MaxVital[(byte)Vital.HP], tool.Position.X + 76, tool.Position.Y + 7, Color.White, TextAlign.Center);
        DrawText(Player.Me.Vital[(byte)Vital.MP] + "/" + Player.Me.MaxVital[(byte)Vital.MP], tool.Position.X + 76, tool.Position.Y + 25, Color.White, TextAlign.Center);
        DrawText(Player.Me.Experience + "/" + Player.Me.ExpNeeded, tool.Position.X + 76, tool.Position.Y + 43, Color.White, TextAlign.Center);
    }

    private static void Chat()
    {
        var tool = Panels.List["Chat"];
        tool.Visible = TextBoxes.Focused != null && ((TextBoxes)TextBoxes.Focused.Data).Name.Equals("Chat");

        // Renderiza as mensagens
        if (tool.Visible || (Loop.ChatTimer >= Environment.TickCount && Options.Chat))
            for (var i = UI.Chat.LinesFirst; i <= UI.Chat.LinesVisible + UI.Chat.LinesFirst; i++)
                if (UI.Chat.Order.Count > i)
                    DrawText(UI.Chat.Order[i].Text, 16, 461 + (11 * (i - UI.Chat.LinesFirst)), UI.Chat.Order[i].Color);

        // Dica de como abrir o chat
        if (!tool.Visible) DrawText("Press [Enter] to open chat.", TextBoxes.List["Chat"].Position.X + 5, TextBoxes.List["Chat"].Position.Y + 3, Color.White);
    }

    private static void Information(Panels tool)
    {
        var item = CryBits.Entities.Item.List.Get(Panels.InformationID);
        Color textColor;
        var data = new List<string>();

        // Apenas se necessário
        if (item == null) return;

        // Define a cor de acordo com a raridade
        switch (item.Rarity)
        {
            case Rarity.Uncommon: textColor = new Color(204, 255, 153); break; // Verde
            case Rarity.Rare: textColor = new Color(102, 153, 255); break; // Azul
            case Rarity.Epic: textColor = new Color(153, 0, 204); break; // Roxo
            case Rarity.Legendary: textColor = new Color(255, 255, 77); break; // Amarelo
            default: textColor = new Color(); break; // Branco
        }

        // Nome, descrição e icone do item
        DrawText(item.Name, tool.Position.X + 41, tool.Position.Y + 6, textColor, TextAlign.Center);
        DrawText(item.Description, tool.Position.X + 82, tool.Position.Y + 20, Color.White, 86);
        Render(Textures.Items[item.Texture], new Rectangle(tool.Position.X + 9, tool.Position.Y + 21, 64, 64));

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
            case ItemType.Potion:
                for (byte n = 0; n < (byte)Vital.Count; n++)
                    if (item.PotionVital[n] != 0)
                        data.Add((Vital)n + ": " + item.PotionVital[n]);

                if (item.PotionExperience != 0) data.Add("Experience: " + item.PotionExperience);
                break;
            // Equipamentos
            case ItemType.Equipment:
                if (item.EquipType == (byte)Equipment.Weapon)
                    if (item.WeaponDamage != 0)
                        data.Add("Damage: " + item.WeaponDamage);

                for (byte n = 0; n < (byte)Attribute.Count; n++)
                    if (item.EquipAttribute[n] != 0)
                        data.Add((Attribute)n + ": " + item.EquipAttribute[n]);
                break;
        }

        // Desenha todos os dados necessários
        Point[] positions = { new(tool.Position.X + 10, tool.Position.Y + 90), new(tool.Position.X + 10, tool.Position.Y + 102), new(tool.Position.X + 10, tool.Position.Y + 114), new(tool.Position.X + 96, tool.Position.Y + 90), new(tool.Position.X + 96, tool.Position.Y + 102), new(tool.Position.X + 96, tool.Position.Y + 114), new(tool.Position.X + 96, tool.Position.Y + 126) };
        for (byte i = 0; i < data.Count; i++) DrawText(data[i], positions[i].X, positions[i].Y, Color.White);
    }

    private static void Hotbar(Panels tool)
    {
        var indicator = string.Empty;

        // Desenha os objetos da hotbar
        for (byte i = 0; i < MaxHotbar; i++)
        {
            var slot = Player.Me.Hotbar[i].Slot;
            if (slot > 0)
                switch (Player.Me.Hotbar[i].Type)
                {
                    // Itens
                    case SlotType.Item: Item(Player.Me.Inventory[slot].Item, 1, tool.Position + new Size(8, 6), (byte)(i + 1), 10); break;
                }

            // Desenha os números de cada slot
            if (i < 10) indicator = (i + 1).ToString();
            else if (i == 9) indicator = "0";
            DrawText(indicator, tool.Position.X + 16 + (36 * i), tool.Position.Y + 22, Color.White);
        }

        // Movendo slot
        if (Panels.HotbarChange >= 0)
            if (Player.Me.Hotbar[Panels.HotbarChange].Type == SlotType.Item)
                Render(Textures.Items[Player.Me.Inventory[Player.Me.Hotbar[Panels.HotbarChange].Slot].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
    }

    private static void MenuCharacter(Panels tool)
    {
        // Dados básicos
        DrawText(Player.Me.Name, tool.Position.X + 18, tool.Position.Y + 52, Color.White);
        DrawText(Player.Me.Level.ToString(), tool.Position.X + 18, tool.Position.Y + 79, Color.White);
        Render(Textures.Faces[Player.Me.TextureNum], new Point(tool.Position.X + 82, tool.Position.Y + 37));

        // Atributos
        DrawText("Strength: " + Player.Me.Attribute[(byte)Attribute.Strength], tool.Position.X + 32, tool.Position.Y + 146, Color.White);
        DrawText("Resistance: " + Player.Me.Attribute[(byte)Attribute.Resistance], tool.Position.X + 32, tool.Position.Y + 162, Color.White);
        DrawText("Intelligence: " + Player.Me.Attribute[(byte)Attribute.Intelligence], tool.Position.X + 32, tool.Position.Y + 178, Color.White);
        DrawText("Agility: " + Player.Me.Attribute[(byte)Attribute.Agility], tool.Position.X + 32, tool.Position.Y + 194, Color.White);
        DrawText("Vitality: " + Player.Me.Attribute[(byte)Attribute.Vitality], tool.Position.X + 32, tool.Position.Y + 210, Color.White);
        DrawText("Points: " + Player.Me.Points, tool.Position.X + 14, tool.Position.Y + 228, Color.White);

        // Equipamentos 
        for (byte i = 0; i < (byte)Equipment.Count; i++)
            if (Player.Me.Equipment[i] == null)
                Render(Textures.Equipments, tool.Position.X + 7 + (i * 34), tool.Position.Y + 247, i * 34, 0, 32, 32);
            else
                Render(Textures.Items[Player.Me.Equipment[i].Texture], tool.Position.X + 8 + (i * 35), tool.Position.Y + 247, 0, 0, 34, 34);
    }

    private static void MenuInventory(Panels tool)
    {
        byte numColumns = 5;

        // Desenha todos os itens do inventário
        for (byte i = 0; i < MaxInventory; i++)
            Item(Player.Me.Inventory[i].Item, Player.Me.Inventory[i].Amount, tool.Position + new Size(7, 30), i, numColumns);

        // Movendo item
        if (Panels.InventoryChange > 0) Render(Textures.Items[Player.Me.Inventory[Panels.InventoryChange].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
    }

    private static void PartyInvitation(Panels tool)
    {
        DrawText(Panels.PartyInvitation + " has invite you to a party. Would you like to join?", tool.Position.X + 14, tool.Position.Y + 33, Color.White, 160);
    }

    private static void Party()
    {
        for (byte i = 0; i < Player.Me.Party.Length; i++)
        {
            // Barras do membro
            Render(Textures.PartyBars, 10, 92 + (27 * i), 0, 0, 82, 8); // HP Cinza
            Render(Textures.PartyBars, 10, 99 + (27 * i), 0, 0, 82, 8); // MP Cinza
            if (Player.Me.Party[i].Vital[(byte)Vital.HP] > 0)
                Render(Textures.PartyBars, 10, 92 + (27 * i), 0, 8, Player.Me.Party[i].Vital[(byte)Vital.HP] * 82 / Player.Me.Party[i].MaxVital[(byte)Vital.HP], 8); // HP 
            if (Player.Me.Party[i].Vital[(byte)Vital.MP] > 0)
                Render(Textures.PartyBars, 10, 99 + (27 * i), 0, 16, Player.Me.Party[i].Vital[(byte)Vital.MP] * 82 / Player.Me.Party[i].MaxVital[(byte)Vital.MP], 8); // MP 

            // Nome do membro
            DrawText(Player.Me.Party[i].Name, 10, 79 + (27 * i), Color.White);
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
        var name = Panels.ShopOpen.Name;
        DrawText(name, tool.Position.X + 131, tool.Position.Y + 28, Color.White, TextAlign.Center);
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
        var line = (slot - 1) / columns;
        var column = slot - (line * 5) - 1;
        var position = start + new Size(column * (grid + gap), line * (grid + gap));

        // Desenha o item e sua quantidade
        Render(Textures.Items[item.Texture], position);
        if (amount > 1) DrawText(amount.ToString(), position.X + 2, position.Y + 17, Color.White);
    }

    private static void Character(short textureNum, Point position, Direction direction, byte column, bool hurt = false)
    {
        Rectangle recSource = new();
        var size = Textures.Characters[textureNum].ToSize();
        var color = new Color(255, 255, 255);
        byte line = 0;

        // Direção
        switch (direction)
        {
            case Direction.Up: line = MovementUp; break;
            case Direction.Down: line = MovementDown; break;
            case Direction.Left: line = MovementLeft; break;
            case Direction.Right: line = MovementRight; break;
        }

        // Define as propriedades dos retângulos
        recSource.X = column * size.Width / AnimationAmount;
        recSource.Y = line * size.Height / AnimationAmount;
        recSource.Width = size.Width / AnimationAmount;
        recSource.Height = size.Height / AnimationAmount;
        var recDestiny = new Rectangle(position, recSource.Size);

        // Demonstra que o personagem está sofrendo dano
        if (hurt) color = new Color(205, 125, 125);

        // Desenha o personagem e sua sombra
        Render(Textures.Shadow, recDestiny.Location.X, recDestiny.Location.Y + (size.Height / AnimationAmount) - Textures.Shadow.ToSize().Height + 5, 0, 0, size.Width / AnimationAmount, Textures.Shadow.ToSize().Height);
        Render(Textures.Characters[textureNum], recSource, recDestiny, color);
    }

    private static void PlayerCharacter(Player player)
    {
        // Desenha o jogador
        PlayerTexture(player);
        PlayerName(player);
        PlayerBars(player);
    }

    private static void PlayerTexture(Player player)
    {
        var column = AnimationStopped;
        var hurt = false;

        // Previne sobrecargas
        if (player.TextureNum <= 0 || player.TextureNum > Textures.Characters.Count) return;

        // Define a animação
        if (player.Attacking && player.AttackTimer + (AttackSpeed / 2) > Environment.TickCount)
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

    private static void PlayerBars(Player player)
    {
        var value = player.Vital[(byte)Vital.HP];

        // Apenas se necessário
        if (value <= 0 || value >= player.MaxVital[(byte)Vital.HP]) return;

        // Cálcula a largura da barra
        var characterSize = Textures.Characters[player.TextureNum].ToSize();
        var fullWidth = characterSize.Width / AnimationAmount;
        var width = value * fullWidth / player.MaxVital[(byte)Vital.HP];

        // Posição das barras
        var position = new Point
        {
            X = ConvertX(player.PixelX),
            Y = ConvertY(player.PixelY) + (characterSize.Height / AnimationAmount) + 4
        };

        // Desenha as barras 
        Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }

    private static void PlayerName(Player player)
    {
        var texture = Textures.Characters[player.TextureNum];
        int nameSize = MeasureString(player.Name);

        // Posição do texto
        var position = new Point
        {
            X = player.PixelX + (texture.ToSize().Width / AnimationAmount / 2) - (nameSize / 2),
            Y = player.PixelY - (texture.ToSize().Height / AnimationAmount / 2)
        };

        // Cor do texto
        var color = player == Player.Me ? Color.Yellow : Color.White;

        // Desenha o texto
        DrawText(player.Name, ConvertX(position.X), ConvertY(position.Y), color);
    }

    private static void Npc(TempNpc npc)
    {
        byte column = 0;
        var hurt = false;

        // Previne sobrecargas
        if (npc.Data.Texture <= 0 || npc.Data.Texture > Textures.Characters.Count) return;

        // Define a animação
        if (npc.Attacking && npc.AttackTimer + (AttackSpeed / 2) > Environment.TickCount)
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
        NpcName(npc);
        NpcBars(npc);
    }

    private static void NpcName(TempNpc npc)
    {
        var position = new Point();
        Color color;
        int nameSize = MeasureString(npc.Data.Name);
        var texture = Textures.Characters[npc.Data.Texture];

        // Posição do texto
        position.X = npc.PixelX + (texture.ToSize().Width / AnimationAmount / 2) - (nameSize / 2);
        position.Y = npc.PixelY - (texture.ToSize().Height / AnimationAmount / 2);

        // Cor do texto
        switch (npc.Data.Behaviour)
        {
            case Behaviour.Friendly: color = Color.White; break;
            case Behaviour.AttackOnSight: color = Color.Red; break;
            case Behaviour.AttackWhenAttacked: color = new Color(228, 120, 51); break;
            default: color = Color.White; break;
        }

        // Desenha o texto
        DrawText(npc.Data.Name, ConvertX(position.X), ConvertY(position.Y), color);
    }

    private static void NpcBars(TempNpc npc)
    {
        var texture = Textures.Characters[npc.Data.Texture];
        var value = npc.Vital[(byte)Vital.HP];

        // Apenas se necessário
        if (value <= 0 || value >= npc.Data.Vital[(byte)Vital.HP]) return;

        // Posição
        var position = new Point(ConvertX(npc.PixelX), ConvertY(npc.PixelY) + (texture.ToSize().Height / AnimationAmount) + 4);
        var fullWidth = texture.ToSize().Width / AnimationAmount;
        var width = value * fullWidth / npc.Data.Vital[(byte)Vital.HP];

        // Desenha a barra 
        Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }

    private static void MapTiles(byte layerType)
    {
        // Previne erros
        if (TempMap.Current.Data.Name == null) return;

        // Dados
        var tempColor = TempMap.Current.Data.Color;
        var color = new Color(tempColor.R, tempColor.G, tempColor.B);
        var map = TempMap.Current.Data;

        // Desenha todas as camadas dos azulejos
        for (byte c = 0; c < map.Layer.Count; c++)
            if (map.Layer[c].Type == layerType)
                for (var x = Camera.TileSight.X; x <= Camera.TileSight.Width; x++)
                for (var y = Camera.TileSight.Y; y <= Camera.TileSight.Height; y++)
                    if (!Map.OutLimit((short)x, (short)y))
                    {
                        var data = map.Layer[c].Tile[x, y];
                        if (data.Texture > 0)
                        {
                            var x2 = data.X * Grid;
                            var y2 = data.Y * Grid;

                            // Desenha o azulejo
                            if (!map.Layer[c].Tile[x, y].IsAutoTile)
                                Render(Textures.Tiles[data.Texture], ConvertX(x * Grid), ConvertY(y * Grid), x2, y2, Grid, Grid, color);
                            else
                                MapAutoTile(new Point(ConvertX(x * Grid), ConvertY(y * Grid)), data, color);
                        }
                    }
    }

    private static void MapAutoTile(Point position, MapTileData data, Color cor)
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
            Render(Textures.Tiles[data.Texture], new Rectangle(source.X, source.Y, 16, 16), new Rectangle(destiny, new Size(16, 16)), cor);
        }
    }

    private static void MapPanorama()
    {
        // Desenha o panorama
        if (TempMap.Current.Data.Panorama > 0)
            Render(Textures.Panoramas[TempMap.Current.Data.Panorama], new Point(0));
    }

    private static void MapFog()
    {
        var data = TempMap.Current.Data.Fog;
        var textureSize = Textures.Fogs[data.Texture].ToSize();

        // Previne erros
        if (data.Texture <= 0) return;

        // Desenha a fumaça
        for (var x = -1; x <= Map.Width * Grid / textureSize.Width; x++)
        for (var y = -1; y <= Map.Height * Grid / textureSize.Height; y++)
            Render(Textures.Fogs[data.Texture], new Point((x * textureSize.Width) + TempMap.Current.FogX, (y * textureSize.Height) + TempMap.Current.FogY), new Color(255, 255, 255, data.Alpha));
    }

    private static void MapWeather()
    {
        byte x = 0;

        // Somente se necessário
        if (TempMap.Current.Data.Weather.Type == 0) return;

        // Textura
        switch (TempMap.Current.Data.Weather.Type)
        {
            case Weather.Snowing: x = 32; break;
        }

        // Desenha as partículas
        foreach (var weather in TempMap.Current.Weather)
            if (weather.Visible)
                Render(Textures.Weather, new Rectangle(x, 0, 32, 32), new Rectangle(weather.X, weather.Y, 32, 32), new Color(255, 255, 255, 150));

        // Trovoadas
        Render(Textures.Blank, 0, 0, 0, 0, ScreenWidth, ScreenHeight, new Color(255, 255, 255, TempMap.Current.Lightning));
    }

    private static void MapName()
    {
        Color color;

        // Somente se necessário
        if (string.IsNullOrEmpty(TempMap.Current.Data.Name)) return;

        // A cor do texto vária de acordo com a moral do mapa
        switch (TempMap.Current.Data.Moral)
        {
            case Moral.Dangerous: color = Color.Red; break;
            default: color = Color.White; break;
        }

        // Desenha o nome do mapa
        DrawText(TempMap.Current.Data.Name, 426, 48, color);
    }

    private static void MapItems()
    {
        // Desenha todos os itens que estão no chão
        for (byte i = 0; i < TempMap.Current.Item.Length; i++)
        {
            var data = TempMap.Current.Item[i];

            // Somente se necessário
            if (data.Item == null) continue;

            // Desenha o item
            var position = new Point(ConvertX(data.X * Grid), ConvertY(data.Y * Grid));
            Render(Textures.Items[data.Item.Texture], position);
        }
    }

    private static void MapBlood()
    {
        // Desenha todos os sangues
        for (byte i = 0; i < TempMap.Current.Blood.Count; i++)
        {
            var data = TempMap.Current.Blood[i];
            Render(Textures.Blood, ConvertX(data.X * Grid), ConvertY(data.Y * Grid), data.TextureNum * 32, 0, 32, 32, new Color(255, 255, 255, data.Opacity));
        }
    }
}