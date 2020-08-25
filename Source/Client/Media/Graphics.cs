using Entities;
using Interface;
using Library;
using Logic;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static Logic.Utils;

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
        RenderWindow = new RenderWindow(new VideoMode(800, 608), Option.Game_Name, Styles.Close);
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

    private static Texture[] LoadTextures(string Directory)
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

    private static SFML.Graphics.Color CColor(byte R = 255, byte G = 255, byte B = 255, byte A = 255) => new SFML.Graphics.Color(R, G, B, A);

    private static void Render(Texture Texture, Rectangle Rec_Source, Rectangle Rec_Destiny, object Color = null, object Mode = null)
    {
        Sprite TmpImage = new Sprite(Texture);

        // Define os dados
        TmpImage.TextureRect = new IntRect(Rec_Source.X, Rec_Source.Y, Rec_Source.Width, Rec_Source.Height);
        TmpImage.Position = new SFML.System.Vector2f(Rec_Destiny.X, Rec_Destiny.Y);
        TmpImage.Scale = new SFML.System.Vector2f(Rec_Destiny.Width / (float)Rec_Source.Width, Rec_Destiny.Height / (float)Rec_Source.Height);
        if (Color != null)
            TmpImage.Color = (SFML.Graphics.Color)Color;

        // Renderiza a textura em forma de retângulo
        if (Mode == null) Mode = RenderStates.Default;
        RenderWindow.Draw(TmpImage, (RenderStates)Mode);
    }

    private static void Render(Texture Texture, int X, int Y, int Source_X, int Source_Y, int Source_Width, int Source_Height, object Color = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(Source_X, Source_Y, Source_Width, Source_Height);
        Rectangle Destiny = new Rectangle(X, Y, Source_Width, Source_Height);

        // Desenha a textura
        Render(Texture, Source, Destiny, Color);
    }

    private static void Render(Texture Texture, Rectangle Destiny, object Color = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));

        // Desenha a textura
        Render(Texture, Source, Destiny, Color);
    }

    private static void Render(Texture Texture, Point Position, object Color = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));
        Rectangle Destiny = new Rectangle(Position, TSize(Texture));

        // Desenha a textura
        Render(Texture, Source, Destiny, Color);
    }

    private static void DrawText(string Text, int X, int Y, SFML.Graphics.Color Color, Alignments Alignment = Alignments.Left)
    {
        Text TempText = new Text(Text, Font_Default);

        // Alinhamento do texto
        switch (Alignment)
        {
            case Alignments.Center: X -= MeasureString(Text) / 2; break;
            case Alignments.Right: X -= MeasureString(Text); break;
        }

        // Define os dados
        TempText.CharacterSize = 10;
        TempText.FillColor = Color;
        TempText.Position = new SFML.System.Vector2f(X, Y);
        TempText.OutlineColor = new SFML.Graphics.Color(0, 0, 0, 70);
        TempText.OutlineThickness = 1;

        // Desenha
        RenderWindow.Draw(TempText);
    }

    private static void DrawText(string Text, int X, int Y, SFML.Graphics.Color Color, int Max_Width, bool Cut = true)
    {
        string Temp_Text;
        int Message_Width = MeasureString(Text), Split = -1;

        // Caso couber, adiciona a mensagem normalmente
        if (Message_Width < Max_Width)
            DrawText(Text, X, Y, Color);
        else
            for (int i = 0; i < Text.Length; i++)
            {
                // Verifica se o caráctere é um separável 
                switch (Text[i])
                {
                    case '-':
                    case '_':
                    case ' ': Split = i; break;
                }

                // Desenha a parte do texto que cabe
                Temp_Text = Text.Substring(0, i);
                if (MeasureString(Temp_Text) > Max_Width)
                {
                    // Divide o texto novamente caso tenha encontrado um ponto de divisão
                    if (Cut && Split != -1) Temp_Text = Text.Substring(0, Split + 1);

                    // Desenha o texto cortado
                    DrawText(Temp_Text, X, Y, Color);
                    DrawText(Text.Substring(Temp_Text.Length), X, Y + 12, Color, Max_Width);
                    return;
                }
            }
    }

    private static void Render_Box(Texture Texture, byte Margin, Point Position, Size Size)
    {
        int Texture_Width = TSize(Texture).Width;
        int Texture_Height = TSize(Texture).Height;

        // Borda esquerda
        Render(Texture, new Rectangle(new Point(0), new Size(Margin, Texture_Width)), new Rectangle(Position, new Size(Margin, Texture_Height)));
        // Borda direita
        Render(Texture, new Rectangle(new Point(Texture_Width - Margin, 0), new Size(Margin, Texture_Height)), new Rectangle(new Point(Position.X + Size.Width - Margin, Position.Y), new Size(Margin, Texture_Height)));
        // Centro
        Render(Texture, new Rectangle(new Point(Margin, 0), new Size(Margin, Texture_Height)), new Rectangle(new Point(Position.X + Margin, Position.Y), new Size(Size.Width - Margin * 2, Texture_Height)));
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
        if (Windows.Current == Windows.Types.Game) Chat();

        // Exibe o que foi renderizado
        RenderWindow.Display();
    }

    private static void InGame()
    {
        // Não desenhar se não estiver em jogo
        if (Windows.Current != Windows.Types.Game) return;

        // Atualiza a câmera
        Camera.Update();

        // Desenhos abaixo do jogador
        Map_Panorama();
        Map_Tiles((byte)Mapper.Layers.Ground);
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
        Map_Tiles((byte)Mapper.Layers.Fringe);
        Map_Weather();
        Map_Fog();
        Map_Name();

        // Desenha os membros da party
        Party();

        // Desenha os dados do jogo
        if (Option.FPS) DrawText("FPS: " + FPS.ToString(), 176, 7, SFML.Graphics.Color.White);
        if (Option.Latency) DrawText("Latency: " + Network.Socket.Latency.ToString(), 176, 19, SFML.Graphics.Color.White);
    }

    #region Tools
    private static void Interface(List<Tools.Order_Structure> Node)
    {
        for (byte i = 0; i < Node.Count; i++)
            if (Node[i].Data.Visible)
            {
                // Desenha a ferramenta
                if (Node[i].Data is Panels) Panel((Panels)Node[i].Data);
                else if (Node[i].Data is TextBoxes) TextBox((TextBoxes)Node[i].Data);
                else if (Node[i].Data is Buttons) Button((Buttons)Node[i].Data);
                else if (Node[i].Data is CheckBoxes) CheckBox((CheckBoxes)Node[i].Data);

                // Desenha algumas coisas mais específicas da interface
                Interface_Specific(Node[i].Data);

                // Pula pra próxima
                Interface(Node[i].Nodes);
            }
    }

    private static void Button(Buttons Tool)
    {
        byte Alpha = 225;

        // Define a transparência do botão pelo seu estado
        switch (Tool.State)
        {
            case Buttons.States.Above: Alpha = 250; break;
            case Buttons.States.Click: Alpha = 200; break;
        }

        // Desenha o botão
        Render(Tex_Button[Tool.Texture_Num], Tool.Position, new SFML.Graphics.Color(255, 255, 225, Alpha));
    }

    private static void Panel(Panels Tool)
    {
        // Desenha o painel
        Render(Tex_Panel[Tool.Texture_Num], Tool.Position);
    }

    private static void CheckBox(CheckBoxes Tool)
    {
        // Define as propriedades dos retângulos
        Rectangle Rec_Source = new Rectangle(new Point(), new Size(TSize(Tex_CheckBox).Width / 2, TSize(Tex_CheckBox).Height));
        Rectangle Rec_Destiny = new Rectangle(Tool.Position, Rec_Source.Size);

        // Desenha a textura do marcador pelo seu estado 
        if (Tool.Checked) Rec_Source.Location = new Point(TSize(Tex_CheckBox).Width / 2, 0);

        // Desenha o marcador 
        Render(Tex_CheckBox, Rec_Source, Rec_Destiny);
        DrawText(Tool.Text, Rec_Destiny.Location.X + TSize(Tex_CheckBox).Width / 2 + CheckBoxes.Margin, Rec_Destiny.Location.Y + 1, SFML.Graphics.Color.White);
    }

    private static void TextBox(TextBoxes Tool)
    {
        Point Position = Tool.Position;
        string Text = Tool.Text;

        // Desenha a ferramenta
        Render_Box(Tex_TextBox, 3, Tool.Position, new Size(Tool.Width, TSize(Tex_TextBox).Height));

        // Altera todos os caracteres do texto para um em especifico, se for necessário
        if (Tool.Password && !string.IsNullOrEmpty(Text)) Text = new string('•', Text.Length);

        // Quebra o texto para que caiba no digitalizador, se for necessário
        Text = TextBreak(Text, Tool.Width - 10);

        // Desenha o texto do digitalizador
        if (TextBoxes.Focused != null && (TextBoxes)TextBoxes.Focused.Data == Tool && TextBoxes.Signal) Text += "|";
        DrawText(Text, Position.X + 4, Position.Y + 2, SFML.Graphics.Color.White);
    }


    private static void Interface_Specific(Tools.Structure Tool)
    {
        // Interações especificas
        if (!(Tool is Panels)) return;
        switch (Tool.Name)
        {
            case "SelectCharacter": SelectCharacter_Class(); break;
            case "CreateCharacter": CreateCharacter_Class(); break;
            case "Hotbar": Hotbar((Panels)Tool); break;
            case "Menu_Character": Menu_Character((Panels)Tool); break;
            case "Menu_Inventory": Menu_Inventory((Panels)Tool); break;
            case "Bars": Bars((Panels)Tool); break;
            case "Information": Informations((Panels)Tool); break;
            case "Party_Invitation": Party_Invitation((Panels)Tool); break;
            case "Trade_Invitation": Trade_Invitation((Panels)Tool); break;
            case "Trade": Trade((Panels)Tool); break;
            case "Shop": Shop((Panels)Tool); break;
        }
    }
    #endregion

    #region Menu
    private static void SelectCharacter_Class()
    {
        Point Text_Position = new Point(399, 425);
        string Text = "(" + (Panels.SelectCharacter + 1) + ") None";

        // Somente se necessário
        if (!Buttons.Characters_Change_Buttons())
        {
            DrawText(Text, Text_Position.X, Text_Position.Y, SFML.Graphics.Color.White, Alignments.Center);
            return;
        }

        // Verifica se o personagem existe
        if (Panels.SelectCharacter >= Panels.Characters.Length)
        {
            DrawText(Text, Text_Position.X, Text_Position.Y, SFML.Graphics.Color.White, Alignments.Center);
            return;
        }

        // Desenha o personagem
        short Texture_Num = Panels.Characters[Panels.SelectCharacter].Texture_Num;
        if (Texture_Num > 0)
        {
            Render(Tex_Face[Texture_Num], new Point(353, 442));
            Character(Texture_Num, new Point(356, 534 - TSize(Tex_Character[Texture_Num]).Height / 4), Directions.Down, Animation_Stopped);
        }

        // Desenha o nome da classe
        Text = "(" + (Panels.SelectCharacter + 1) + ") " + Panels.Characters[Panels.SelectCharacter].Name;
        DrawText(Text, Text_Position.X, Text_Position.Y, SFML.Graphics.Color.White, Alignments.Center);
    }

    private static void CreateCharacter_Class()
    {
        short Texture_Num = 0;
        Class Class = Entities.Class.List.ElementAt(Panels.CreateCharacter_Class).Value;

        // Textura do personagem
        if (CheckBoxes.List["GenderMale"].Checked && Class.Tex_Male.Length > 0)
            Texture_Num = Class.Tex_Male[Panels.CreateCharacter_Tex];
        else if (Class.Tex_Female.Length > 0)
            Texture_Num = Class.Tex_Female[Panels.CreateCharacter_Tex];

        // Desenha o personagem
        if (Texture_Num > 0)
        {
            Render(Tex_Face[Texture_Num], new Point(425, 440));
            Character(Texture_Num, new Point(433, 501), Directions.Down, Animation_Stopped);
        }

        // Desenha o nome da classe
        string Text = Class.Name;
        DrawText(Text, 347, 509, SFML.Graphics.Color.White, Alignments.Center);

        // Descrição
        DrawText(Class.Description, 282, 526, SFML.Graphics.Color.White, 123);
    }
    #endregion

    private static void Bars(Panels Tool)
    {
        decimal HP_Percentage = Player.Me.Vital[(byte)Vitals.HP] / (decimal)Player.Me.Max_Vital[(byte)Vitals.HP];
        decimal MP_Percentage = Player.Me.Vital[(byte)Vitals.MP] / (decimal)Player.Me.Max_Vital[(byte)Vitals.MP];
        decimal Exp_Percentage = Player.Me.Experience / (decimal)Player.Me.ExpNeeded;

        // Barras
        Render(Tex_Bars_Panel, Tool.Position.X + 6, Tool.Position.Y + 6, 0, 0, (int)(Tex_Bars_Panel.Size.X * HP_Percentage), 17);
        Render(Tex_Bars_Panel, Tool.Position.X + 6, Tool.Position.Y + 24, 0, 18, (int)(Tex_Bars_Panel.Size.X * MP_Percentage), 17);
        Render(Tex_Bars_Panel, Tool.Position.X + 6, Tool.Position.Y + 42, 0, 36, (int)(Tex_Bars_Panel.Size.X * Exp_Percentage), 17);

        // Textos 
        DrawText("HP", Tool.Position.X + 10, Tool.Position.Y + 3, SFML.Graphics.Color.White);
        DrawText("MP", Tool.Position.X + 10, Tool.Position.Y + 21, SFML.Graphics.Color.White);
        DrawText("Exp", Tool.Position.X + 10, Tool.Position.Y + 39, SFML.Graphics.Color.White);

        // Indicadores
        DrawText(Player.Me.Vital[(byte)Vitals.HP] + "/" + Player.Me.Max_Vital[(byte)Vitals.HP], Tool.Position.X + 76, Tool.Position.Y + 7, SFML.Graphics.Color.White, Alignments.Center);
        DrawText(Player.Me.Vital[(byte)Vitals.MP] + "/" + Player.Me.Max_Vital[(byte)Vitals.MP], Tool.Position.X + 76, Tool.Position.Y + 25, SFML.Graphics.Color.White, Alignments.Center);
        DrawText(Player.Me.Experience + "/" + Player.Me.ExpNeeded, Tool.Position.X + 76, Tool.Position.Y + 43, SFML.Graphics.Color.White, Alignments.Center);
    }

    private static void Chat()
    {
        Panels Tool = Panels.List["Chat"];
        Tool.Visible = TextBoxes.Focused != null && ((TextBoxes)TextBoxes.Focused.Data).Name.Equals("Chat");

        // Renderiza as mensagens
        if (Tool.Visible || (Loop.Chat_Timer >= Environment.TickCount && Option.Chat))
            for (byte i = global::Interface.Chat.Lines_First; i <= global::Interface.Chat.Lines_Visible + global::Interface.Chat.Lines_First; i++)
                if (global::Interface.Chat.Order.Count > i)
                    DrawText(global::Interface.Chat.Order[i].Text, 16, 461 + 11 * (i - global::Interface.Chat.Lines_First), global::Interface.Chat.Order[i].Color);

        // Dica de como abrir o chat
        if (!Tool.Visible) DrawText("Press [Enter] to open chat.", TextBoxes.List["Chat"].Position.X + 5, TextBoxes.List["Chat"].Position.Y + 3, SFML.Graphics.Color.White);
    }

    private static void Informations(Panels Tool)
    {
        Item Item = Item.Get(Panels.Infomation_ID);
        SFML.Graphics.Color Text_Color;
        List<string> Data = new List<string>();

        // Apenas se necessário
        if (Item == null) return;

        // Define a cor de acordo com a raridade
        switch ((Rarity)Item.Rarity)
        {
            case Rarity.Uncommon: Text_Color = CColor(204, 255, 153); break; // Verde
            case Rarity.Rare: Text_Color = CColor(102, 153, 255); break; // Azul
            case Rarity.Epic: Text_Color = CColor(153, 0, 204); break; // Roxo
            case Rarity.Legendary: Text_Color = CColor(255, 255, 77); break; // Amarelo
            default: Text_Color = CColor(255, 255, 255); break; // Branco
        }

        // Nome, descrição e icone do item
        DrawText(Item.Name, Tool.Position.X + 41, Tool.Position.Y + 6, Text_Color, Alignments.Center);
        DrawText(Item.Description, Tool.Position.X + 82, Tool.Position.Y + 20, SFML.Graphics.Color.White, 86);
        Render(Tex_Item[Item.Texture], new Rectangle(Tool.Position.X + 9, Tool.Position.Y + 21, 64, 64));

        // Informações da Loja
        if (Panels.List["Shop"].Visible)
            if (Panels.Shop_Slot >= 0)
                Data.Add("Price: " + Panels.Shop_Open.Sold[Panels.Shop_Slot].Price);
            else if (Panels.Inventory_Slot > 0)
                if (Panels.Shop_Open.FindBought(Item) != null)
                    Data.Add("Sale price: " + Panels.Shop_Open.FindBought(Item).Price);

        // Informações específicas dos itens
        switch ((Items)Item.Type)
        {
            // Poção
            case Items.Potion:
                for (byte n = 0; n < (byte)Vitals.Count; n++)
                    if (Item.Potion_Vital[n] != 0)
                        Data.Add(((Vitals)n).ToString() + ": " + Item.Potion_Vital[n]);

                if (Item.Potion_Experience != 0) Data.Add("Experience: " + Item.Potion_Experience);
                break;
            // Equipamentos
            case Items.Equipment:
                if (Item.Equip_Type == (byte)Equipments.Weapon)
                    if (Item.Weapon_Damage != 0)
                        Data.Add("Damage: " + Item.Weapon_Damage);

                for (byte n = 0; n < (byte)Attributes.Count; n++)
                    if (Item.Equip_Attribute[n] != 0)
                        Data.Add(((Attributes)n).ToString() + ": " + Item.Equip_Attribute[n]);
                break;
        }

        // Desenha todos os dados necessários
        Point[] Positions = { new Point(Tool.Position.X + 10, Tool.Position.Y + 90), new Point(Tool.Position.X + 10, Tool.Position.Y + 102), new Point(Tool.Position.X + 10, Tool.Position.Y + 114), new Point(Tool.Position.X + 96, Tool.Position.Y + 90), new Point(Tool.Position.X + 96, Tool.Position.Y + 102), new Point(Tool.Position.X + 96, Tool.Position.Y + 114), new Point(Tool.Position.X + 96, Tool.Position.Y + 126) };
        for (byte i = 0; i < Data.Count; i++) DrawText(Data[i], Positions[i].X, Positions[i].Y, SFML.Graphics.Color.White);
    }

    private static void Hotbar(Panels Tool)
    {
        string Indicator = string.Empty;

        // Desenha os objetos da hotbar
        for (byte i = 0; i < Max_Hotbar; i++)
        {
            byte Slot = Player.Me.Hotbar[i].Slot;
            if (Slot > 0)
                switch ((Utils.Hotbar)Player.Me.Hotbar[i].Type)
                {
                    // Itens
                    case Utils.Hotbar.Item: Item(Player.Me.Inventory[Slot].Item, 1, Tool.Position + new Size(8, 6), (byte)(i + 1), 10); break;
                }

            // Desenha os números de cada slot
            if (i < 10) Indicator = (i + 1).ToString();
            else if (i == 9) Indicator = "0";
            DrawText(Indicator, Tool.Position.X + 16 + 36 * i, Tool.Position.Y + 22, SFML.Graphics.Color.White);
        }

        // Movendo slot
        if (Panels.Hotbar_Change >= 0)
            if (Player.Me.Hotbar[Panels.Hotbar_Change].Type == (byte)Utils.Hotbar.Item)
                Render(Tex_Item[Player.Me.Inventory[Player.Me.Hotbar[Panels.Hotbar_Change].Slot].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
    }

    private static void Menu_Character(Panels Tool)
    {
        // Dados básicos
        DrawText(Player.Me.Name, Tool.Position.X + 18, Tool.Position.Y + 52, SFML.Graphics.Color.White);
        DrawText(Player.Me.Level.ToString(), Tool.Position.X + 18, Tool.Position.Y + 79, SFML.Graphics.Color.White);
        Render(Tex_Face[Player.Me.Texture_Num], new Point(Tool.Position.X + 82, Tool.Position.Y + 37));

        // Atributos
        DrawText("Strength: " + Player.Me.Attribute[(byte)Attributes.Strength], Tool.Position.X + 32, Tool.Position.Y + 146, SFML.Graphics.Color.White);
        DrawText("Resistance: " + Player.Me.Attribute[(byte)Attributes.Resistance], Tool.Position.X + 32, Tool.Position.Y + 162, SFML.Graphics.Color.White);
        DrawText("Intelligence: " + Player.Me.Attribute[(byte)Attributes.Intelligence], Tool.Position.X + 32, Tool.Position.Y + 178, SFML.Graphics.Color.White);
        DrawText("Agility: " + Player.Me.Attribute[(byte)Attributes.Agility], Tool.Position.X + 32, Tool.Position.Y + 194, SFML.Graphics.Color.White);
        DrawText("Vitality: " + Player.Me.Attribute[(byte)Attributes.Vitality], Tool.Position.X + 32, Tool.Position.Y + 210, SFML.Graphics.Color.White);
        DrawText("Points: " + Player.Me.Points, Tool.Position.X + 14, Tool.Position.Y + 228, SFML.Graphics.Color.White);

        // Equipamentos 
        for (byte i = 0; i < (byte)Equipments.Count; i++)
            if (Player.Me.Equipment[i] == null)
                Render(Tex_Equipments, Tool.Position.X + 7 + i * 34, Tool.Position.Y + 247, i * 34, 0, 32, 32);
            else
                Render(Tex_Item[Player.Me.Equipment[i].Texture], Tool.Position.X + 8 + i * 35, Tool.Position.Y + 247, 0, 0, 34, 34);
    }

    private static void Menu_Inventory(Panels Tool)
    {
        byte NumColumns = 5;

        // Desenha todos os itens do inventário
        for (byte i = 1; i <= Max_Inventory; i++)
            Item(Player.Me.Inventory[i].Item, Player.Me.Inventory[i].Amount, Tool.Position + new Size(7, 30), i, NumColumns);

        // Movendo item
        if (Panels.Inventory_Change > 0) Render(Tex_Item[Player.Me.Inventory[Panels.Inventory_Change].Item.Texture], new Point(Windows.Mouse.X + 6, Windows.Mouse.Y + 6));
    }

    private static void Party_Invitation(Panels Tool)
    {
        DrawText(Panels.Party_Invitation + " has invite you to a party. Would you like to join?", Tool.Position.X + 14, Tool.Position.Y + 33, SFML.Graphics.Color.White, 160);
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

    private static void Trade_Invitation(Panels Tool)
    {
        DrawText(Panels.Trade_Invitation + " has invite you to a trade. Would you like to join?", Tool.Position.X + 14, Tool.Position.Y + 33, SFML.Graphics.Color.White, 160);
    }

    private static void Trade(Panels Tool)
    {
        // Desenha os itens das ofertas
        for (byte i = 1; i <= Max_Inventory; i++)
        {
            Item(Player.Me.Trade_Offer[i].Item, Player.Me.Trade_Offer[i].Amount, Tool.Position + new Size(7, 50), i, 5);
            Item(Player.Me.Trade_Their_Offer[i].Item, Player.Me.Trade_Their_Offer[i].Amount, Tool.Position + new Size(192, 50), i, 5);
        }
    }

    private static void Shop(Panels Tool)
    {
        // Dados da loja
        string Name = Panels.Shop_Open.Name;
        DrawText(Name, Tool.Position.X + 131, Tool.Position.Y + 28, SFML.Graphics.Color.White, Alignments.Center);
        DrawText("Currency: " + Panels.Shop_Open.Currency.Name, Tool.Position.X + 10, Tool.Position.Y + 195, SFML.Graphics.Color.White);

        // Desenha os itens
        for (byte i = 0; i < Panels.Shop_Open.Sold.Length; i++)
            Item(Panels.Shop_Open.Sold[i].Item, Panels.Shop_Open.Sold[i].Amount, Tool.Position + new Size(7, 50), (byte)(i + 1), 7);
    }

    private static void Item(Item Item, short Amount, Point Start, byte Slot, byte Columns, byte Grid = 32, byte Gap = 4)
    {
        // Somente se necessário
        if (Item == null) return;

        // Posição do item baseado no slot
        int Line = (Slot - 1) / Columns;
        int Column = Slot - (Line * 5) - 1;
        Point Position = Start + new Size(Column * (Grid + Gap), Line * (Grid + Gap));

        // Desenha o item e sua quantidade
        Render(Tex_Item[Item.Texture], Position);
        if (Amount > 1) DrawText(Amount.ToString(), Position.X + 2, Position.Y + 17, SFML.Graphics.Color.White);
    }

    private static void Character(short Texture_Num, Point Position, Directions Direction, byte Column, bool Hurt = false)
    {
        Rectangle Rec_Source = new Rectangle(), Rec_Destiny;
        Size Size = TSize(Tex_Character[Texture_Num]);
        SFML.Graphics.Color Color = new SFML.Graphics.Color(255, 255, 255);
        byte Line = 0;

        // Direção
        switch (Direction)
        {
            case Directions.Up: Line = Movement_Up; break;
            case Directions.Down: Line = Movement_Down; break;
            case Directions.Left: Line = Movement_Left; break;
            case Directions.Right: Line = Movement_Right; break;
        }

        // Define as propriedades dos retângulos
        Rec_Source.X = Column * Size.Width / Animation_Amount;
        Rec_Source.Y = Line * Size.Height / Animation_Amount;
        Rec_Source.Width = Size.Width / Animation_Amount;
        Rec_Source.Height = Size.Height / Animation_Amount;
        Rec_Destiny = new Rectangle(Position, Rec_Source.Size);

        // Demonstra que o personagem está sofrendo dano
        if (Hurt) Color = new SFML.Graphics.Color(205, 125, 125);

        // Desenha o personagem e sua sombra
        Render(Tex_Shadow, Rec_Destiny.Location.X, Rec_Destiny.Location.Y + Size.Height / Animation_Amount - TSize(Tex_Shadow).Height + 5, 0, 0, Size.Width / Animation_Amount, TSize(Tex_Shadow).Height);
        Render(Tex_Character[Texture_Num], Rec_Source, Rec_Destiny, Color);
    }

    private static void Player_Character(Player Player)
    {
        // Desenha o jogador
        Player_Texture(Player);
        Player_Name(Player);
        Player_Bars(Player);
    }

    private static void Player_Texture(Player Player)
    {
        byte Column = Animation_Stopped;
        bool Hurt = false;

        // Previne sobrecargas
        if (Player.Texture_Num <= 0 || Player.Texture_Num > Tex_Character.GetUpperBound(0)) return;

        // Define a animação
        if (Player.Attacking && Player.Attack_Timer + Attack_Speed / 2 > Environment.TickCount)
            Column = Animation_Attack;
        else
        {
            if (Player.X2 > 8 && Player.X2 < Grid) Column = Player.Animation;
            if (Player.X2 < -8 && Player.X2 > Grid * -1) Column = Player.Animation;
            if (Player.Y2 > 8 && Player.Y2 < Grid) Column = Player.Animation;
            if (Player.Y2 < -8 && Player.Y2 > Grid * -1) Column = Player.Animation;
        }

        // Demonstra que o personagem está sofrendo dano
        if (Player.Hurt > 0) Hurt = true;

        // Desenha o jogador
        Character(Player.Texture_Num, new Point(ConvertX(Player.Pixel_X), ConvertY(Player.Pixel_Y)), Player.Direction, Column, Hurt);
    }

    private static void Player_Bars(Player Player)
    {
        short Value = Player.Vital[(byte)Vitals.HP];

        // Apenas se necessário
        if (Value <= 0 || Value >= Player.Max_Vital[(byte)Vitals.HP]) return;

        // Cálcula a largura da barra
        Size Chracater_Size = TSize(Tex_Character[Player.Texture_Num]);
        int FullWidth = Chracater_Size.Width / Animation_Amount;
        int Width = (Value * FullWidth) / Player.Max_Vital[(byte)Vitals.HP];

        // Posição das barras
        Point Position = new Point
        {
            X = ConvertX(Player.Pixel_X),
            Y = ConvertY(Player.Pixel_Y) + Chracater_Size.Height / Animation_Amount + 4
        };

        // Desenha as barras 
        Render(Tex_Bars, Position.X, Position.Y, 0, 4, FullWidth, 4);
        Render(Tex_Bars, Position.X, Position.Y, 0, 0, Width, 4);
    }

    private static void Player_Name(Player Player)
    {
        Texture Texture = Tex_Character[Player.Texture_Num];
        int Name_Size = MeasureString(Player.Name);

        // Posição do texto
        Point Position = new Point
        {
            X = Player.Pixel_X + TSize(Texture).Width / Animation_Amount / 2 - Name_Size / 2,
            Y = Player.Pixel_Y - TSize(Texture).Height / Animation_Amount / 2
        };

        // Cor do texto
        SFML.Graphics.Color Color;
        if (Player == Player.Me)
            Color = SFML.Graphics.Color.Yellow;
        else
            Color = SFML.Graphics.Color.White;

        // Desenha o texto
        DrawText(Player.Name, ConvertX(Position.X), ConvertY(Position.Y), Color);
    }

    private static void NPC(TempNPC NPC)
    {
        byte Column = 0;
        bool Hurt = false;

        // Previne sobrecargas
        if (NPC.Data.Texture <= 0 || NPC.Data.Texture > Tex_Character.GetUpperBound(0)) return;

        // Define a animação
        if (NPC.Attacking && NPC.Attack_Timer + Attack_Speed / 2 > Environment.TickCount)
            Column = Animation_Attack;
        else
        {
            if (NPC.X2 > 8 && NPC.X2 < Grid) Column = NPC.Animation;
            else if (NPC.X2 < -8 && NPC.X2 > Grid * -1) Column = NPC.Animation;
            else if (NPC.Y2 > 8 && NPC.Y2 < Grid) Column = NPC.Animation;
            else if (NPC.Y2 < -8 && NPC.Y2 > Grid * -1) Column = NPC.Animation;
        }

        // Demonstra que o personagem está sofrendo dano
        if (NPC.Hurt > 0) Hurt = true;

        // Desenha o jogador
        Character(NPC.Data.Texture, new Point(ConvertX(NPC.Pixel_X), ConvertY(NPC.Pixel_Y)), NPC.Direction, Column, Hurt);
        NPC_Name(NPC);
        NPC_Bars(NPC);
    }

    private static void NPC_Name(TempNPC NPC)
    {
        Point Position = new Point();
        SFML.Graphics.Color Color;
        int Name_Size = MeasureString(NPC.Data.Name);
        Texture Texture = Tex_Character[NPC.Data.Texture];

        // Posição do texto
        Position.X = NPC.Pixel_X + TSize(Texture).Width / Animation_Amount / 2 - Name_Size / 2;
        Position.Y = NPC.Pixel_Y - TSize(Texture).Height / Animation_Amount / 2;

        // Cor do texto
        switch ((NPCs)NPC.Data.Type)
        {
            case NPCs.Friendly: Color = SFML.Graphics.Color.White; break;
            case NPCs.AttackOnSight: Color = SFML.Graphics.Color.Red; break;
            case NPCs.AttackWhenAttacked: Color = new SFML.Graphics.Color(228, 120, 51); break;
            default: Color = SFML.Graphics.Color.White; break;
        }

        // Desenha o texto
        DrawText(NPC.Data.Name, ConvertX(Position.X), ConvertY(Position.Y), Color);
    }

    private static void NPC_Bars(TempNPC NPC)
    {
        Texture Texture = Tex_Character[NPC.Data.Texture];
        short Value = NPC.Vital[(byte)Vitals.HP];

        // Apenas se necessário
        if (Value <= 0 || Value >= NPC.Data.Vital[(byte)Vitals.HP]) return;

        // Posição
        Point Position = new Point(ConvertX(NPC.Pixel_X), ConvertY(NPC.Pixel_Y) + TSize(Texture).Height / Animation_Amount + 4);
        int FullWidth = TSize(Texture).Width / Animation_Amount;
        int Width = (Value * FullWidth) / NPC.Data.Vital[(byte)Vitals.HP];

        // Desenha a barra 
        Render(Tex_Bars, Position.X, Position.Y, 0, 4, FullWidth, 4);
        Render(Tex_Bars, Position.X, Position.Y, 0, 0, Width, 4);
    }

    private static void Map_Tiles(byte c)
    {
        // Previne erros
        if (Mapper.Current.Data.Name == null) return;

        // Dados
        System.Drawing.Color TempColor = System.Drawing.Color.FromArgb(Mapper.Current.Data.Color);
        SFML.Graphics.Color Color = CColor(TempColor.R, TempColor.G, TempColor.B);

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
                                Render(Tex_Tile[Mapper.Current.Data.Tile[x, y].Data[c, q].Tile], ConvertX(x * Grid), ConvertY(y * Grid), x2, y2, Grid, Grid, Color);
                            else
                                Map_Autotile(new Point(ConvertX(x * Grid), ConvertY(y * Grid)), Mapper.Current.Data.Tile[x, y].Data[c, q], Color);
                        }
    }

    private static void Map_Autotile(Point Position, Map_Tile_Data Data, SFML.Graphics.Color Cor)
    {
        // Desenha os 4 mini azulejos
        for (byte i = 0; i < 4; i++)
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
            Render(Tex_Tile[Data.Tile], new Rectangle(Source.X, Source.Y, 16, 16), new Rectangle(Destiny, new Size(16, 16)), Cor);
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
        Map_Fog Data = Mapper.Current.Data.Fog;
        Size Texture_Size = TSize(Tex_Fog[Data.Texture]);

        // Previne erros
        if (Data.Texture <= 0) return;

        // Desenha a fumaça
        for (int x = -1; x <= Map_Width * Grid / Texture_Size.Width; x++)
            for (int y = -1; y <= Map_Height * Grid / Texture_Size.Height; y++)
                Render(Tex_Fog[Data.Texture], new Point(x * Texture_Size.Width + Mapper.Fog_X, y * Texture_Size.Height + Mapper.Fog_Y), new SFML.Graphics.Color(255, 255, 255, Data.Alpha));
    }

    private static void Map_Weather()
    {
        byte x = 0;

        // Somente se necessário
        if (Mapper.Current.Data.Weather.Type == 0) return;

        // Textura
        switch ((Mapper.Weathers)Mapper.Current.Data.Weather.Type)
        {
            case Mapper.Weathers.Snowing: x = 32; break;
        }

        // Desenha as partículas
        for (int i = 0; i < TempMap.Weather.Length; i++)
            if (TempMap.Weather[i].Visible)
                Render(Tex_Weather, new Rectangle(x, 0, 32, 32), new Rectangle(TempMap.Weather[i].x, TempMap.Weather[i].y, 32, 32), CColor(255, 255, 255, 150));

        // Trovoadas
        Render(Tex_Blanc, 0, 0, 0, 0, Screen_Width, Screen_Height, new SFML.Graphics.Color(255, 255, 255, Mapper.Lightning));
    }

    private static void Map_Name()
    {
        SFML.Graphics.Color Color;

        // Somente se necessário
        if (string.IsNullOrEmpty(Mapper.Current.Data.Name)) return;

        // A cor do texto vária de acordo com a moral do mapa
        switch (Mapper.Current.Data.Moral)
        {
            case (byte)Mapper.Morals.Danger: Color = SFML.Graphics.Color.Red; break;
            default: Color = SFML.Graphics.Color.White; break;
        }

        // Desenha o nome do mapa
        DrawText(Mapper.Current.Data.Name, 426, 48, Color);
    }

    private static void Map_Items()
    {
        // Desenha todos os itens que estão no chão
        for (byte i = 0; i < Mapper.Current.Item.Length; i++)
        {
            TMap_Items Data = Mapper.Current.Item[i];

            // Somente se necessário
            if (Data.Item == null) continue;

            // Desenha o item
            Point Position = new Point(ConvertX(Data.X * Grid), ConvertY(Data.Y * Grid));
            Render(Tex_Item[Data.Item.Texture], Position);
        }
    }

    private static void Map_Blood()
    {
        // Desenha todos os sangues
        for (byte i = 0; i < Mapper.Current.Blood.Count; i++)
        {
            TMap_Blood Data = Mapper.Current.Blood[i];
            Render(Tex_Blood, ConvertX(Data.X * Grid), ConvertY(Data.Y * Grid), Data.Texture_Num * 32, 0, 32, 32, CColor(255, 255, 255, Data.Opacity));
        }
    }
}