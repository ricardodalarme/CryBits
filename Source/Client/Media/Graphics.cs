using SFML.Graphics;
using SFML.Window;
using System;
using System.Drawing;
using System.IO;

partial class Graphics
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
    public static Texture Tex_BackGround;
    public static Texture Tex_Weather;
    public static Texture Tex_Blanc;
    public static Texture Tex_Directions;
    public static Texture Tex_Shadow;
    public static Texture Tex_Bars;
    public static Texture Tex_Bars_Panel;
    public static Texture Tex_Grid;
    public static Texture Tex_Equipments;

    // Formato das texturas
    public const string Format = ".png";

    #region Engine
    public static Texture[] LoadTextures(string Directory)
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

    public static void Render(Texture Texture, Rectangle Rec_Source, Rectangle Rec_Destiny, object Color = null, object Mode = null)
    {
        Sprite TmpImage = new Sprite(Texture);

        // Define os dados
        TmpImage.TextureRect = new IntRect(Rec_Source.X, Rec_Source.Y, Rec_Source.Width, Rec_Source.Height);
        TmpImage.Position = new Vector2f(Rec_Destiny.X, Rec_Destiny.Y);
        TmpImage.Scale = new Vector2f(Rec_Destiny.Width / (float)Rec_Source.Width, Rec_Destiny.Height / (float)Rec_Source.Height);
        if (Color != null)
            TmpImage.Color = (SFML.Graphics.Color)Color;

        // Renderiza a textura em forma de retângulo
        if (Mode == null) Mode = RenderStates.Default;
        RenderWindow.Draw(TmpImage, (RenderStates)Mode);
    }

    public static void Render(Texture Texture, int X, int Y, int Source_X, int Source_Y, int Source_Width, int Source_Height, object Color = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(Source_X, Source_Y), new Size(Source_Width, Source_Height));
        Rectangle Destiny = new Rectangle(new Point(X, Y), new Size(Source_Width, Source_Height));

        // Desenha a textura
        Render(Texture, Source, Destiny, Color);
    }

    public static void Render(Texture Texture, Rectangle Destiny, object Color = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));

        // Desenha a textura
        Render(Texture, Source, Destiny, Color);
    }

    public static void Render(Texture Texture, Point Position, object Color = null)
    {
        // Define as propriedades dos retângulos
        Rectangle Source = new Rectangle(new Point(0), TSize(Texture));
        Rectangle Destiny = new Rectangle(Position, TSize(Texture));

        // Desenha a textura
        Render(Texture, Source, Destiny, Color);
    }

    private static void DrawText(string Text, int X, int Y, SFML.Graphics.Color Color)
    {
        Text TempText = new Text(Text, Font_Default);

        // Define os dados
        TempText.CharacterSize = 10;
        TempText.Color = Color;
        TempText.Position = new Vector2f(X, Y);

        // Desenha
        RenderWindow.Draw(TempText);
    }

    public static void Render_Box(Texture Texture, byte Margin, Point Position, Size Size)
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

    public static void LoadTextures()
    {
        // Inicia os dispositivos
        RenderWindow = new RenderWindow(Window.Objects.Handle);
        Font_Default = new SFML.Graphics.Font(Directories.Fonts.FullName + "Georgia.ttf");

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
        Tex_BackGround = new Texture(Directories.Tex_Background.FullName + Format);
        Tex_Directions = new Texture(Directories.Tex_Directions.FullName + Format);
        Tex_Shadow = new Texture(Directories.Tex_Shadow.FullName + Format);
        Tex_Bars = new Texture(Directories.Tex_Bars.FullName + Format);
        Tex_Bars_Panel = new Texture(Directories.Tex_Bars_Panel.FullName + Format);
        Tex_Grid = new Texture(Directories.Tex_Grid.FullName + Format);
        Tex_Equipments = new Texture(Directories.Tex_Equipments.FullName + Format);
    }

    public static void Present()
    {
        // Limpa a área com um fundo preto
        RenderWindow.Clear(SFML.Graphics.Color.Black);

        // Desenha o menu
        Menu();

        // Desenha as coisas em jogo
        InGame();

        // Desenha os dados do jogo
        DrawText("FPS: " + Game.FPS.ToString(), 8, 73, SFML.Graphics.Color.White);
        DrawText("Latency: " + Game.Latency.ToString(), 8, 83, SFML.Graphics.Color.White);

        // Exibe o que foi renderizado
        RenderWindow.Display();
    }

    public static void InGame()
    {
        // Não desenhar se não estiver em jogo
        if (Tools.CurrentWindow != Tools.Windows.Game)
            return;

        // Atualiza a câmera
        Game.UpdateCamera();

        // Desenhos abaixo do jogador
        Map_Panorama();
        Map_Tiles((byte)Map.Layers.Ground);
        Map_Items();

        // Desenha os NPCs
        for (byte i = 1; i < Lists.Map.Temp_NPC.Length; i++)
            if (Lists.Map.Temp_NPC[i].Index > 0)
                NPC(i);

        // Desenha os jogadores
        for (byte i = 1; i <= Player.HigherIndex; i++)
            if (Player.IsPlaying(i))
                if (i != Player.MyIndex)
                    if (Lists.Player[i].Map == Player.Me.Map)
                        Player_Character(i);

        // Desenha o próprio jogador
        Player_Character(Player.MyIndex);

        // Desenhos acima do jogador
        Map_Tiles((byte)Map.Layers.Fringe);
        Map_Weather();
        Map_Fog();
        Map_Name();

        // Interface do jogo
        Game_Interface();
    }

    #region Tools
    public static void Button(string Name)
    {
        byte Alpha = 225;
        byte Index = Buttons.GetIndex(Name);

        // Lista a ordem de renderização da ferramenta
        Tools.List(Tools.Types.Button, Index);

        // Não desenha a ferramenta se ela não for visível
        if (!Buttons.List[Index].CheckEnable())
            return;

        // Define a transparência do botão pelo seu estado
        switch (Buttons.List[Index].State)
        {
            case Buttons.States.Above:
                Alpha = 250;
                break;
            case Buttons.States.Click:
                Alpha = 200;
                break;
        }

        // Desenha o botão
        Render(Tex_Button[Buttons.List[Index].Texture_Num], Buttons.List[Index].Position, new SFML.Graphics.Color(255, 255, 225, Alpha));
    }

    public static void Panel(string Name)
    {
        byte Index = Panels.GetIndex(Name);

        // Lista a ordem de renderização da ferramenta
        Tools.List(Tools.Types.Painel, Index);

        // Não desenha a ferramenta se ela não for visível
        if (!Panels.List[Index].CheckEnable())
            return;

        // Desenha o painel
        Render(Tex_Panel[Panels.List[Index].Texture], Panels.List[Index].Position);
    }

    public static void CheckBox(string Name)
    {
        Rectangle Rec_Source = new Rectangle(), Rec_Destiny;
        byte Index = CheckBoxes.GetIndex(Name);

        // Lista a ordem de renderização da ferramenta
        Tools.List(Tools.Types.CheckBox, Index);

        // Não desenha a ferramenta se ela não for visível
        if (!CheckBoxes.List[Index].CheckEnable())
            return;

        // Define as propriedades dos retângulos
        Rec_Source.Size = new Size(TSize(Tex_CheckBox).Width / 2, TSize(Tex_CheckBox).Height);
        Rec_Destiny = new Rectangle(CheckBoxes.Get(Name).Position, Rec_Source.Size);

        // Desenha a textura do marcador pelo seu estado 
        if (CheckBoxes.List[Index].State)
            Rec_Source.Location = new Point(TSize(Tex_CheckBox).Width / 2, 0);

        // Desenha o marcador 
        Render(Tex_CheckBox, Rec_Source, Rec_Destiny);
        DrawText(CheckBoxes.Get(Name).Text, Rec_Destiny.Location.X + TSize(Tex_CheckBox).Width / 2 + CheckBoxes.Margin, Rec_Destiny.Location.Y + 1, SFML.Graphics.Color.White);
    }

    public static void TextBox(string Name)
    {
        byte Index = TextBoxes.GetIndex(Name);

        // Lista a ordem de renderização da ferramenta
        Tools.List(Tools.Types.TextBox, Index);

        // Não desenha a ferramenta se ela não for visível
        if (!TextBoxes.List[Index].CheckEnable())
            return;

        // Desenha a ferramenta
        Render_Box(Tex_TextBox, 3, TextBoxes.List[Index].Position, new Size(TextBoxes.List[Index].Width, TSize(Tex_TextBox).Height));

        // Desenha o texto do digitalizador
        TextBox_Text(Index);
    }

    public static void TextBox_Text(byte i)
    {
        Point Position = TextBoxes.List[i].Position;
        string Text = TextBoxes.List[i].Text;

        // Altera todos os caracteres do texto para um em especifico, se for necessário
        if (TextBoxes.List[i].Password && !string.IsNullOrEmpty(Text))
            Text = new String('•', Text.Length);

        // Quebra o texto para que caiba no digitalizador, se for necessário
        Text = Tools.TextBreak(Text, TextBoxes.List[i].Width - 10);

        // Desenha o texto do digitalizador
        if (TextBoxes.TexBox_Focus == i && TextBoxes.Signal)
            DrawText(Text + "|", Position.X + 4, Position.Y + 2, SFML.Graphics.Color.White);
        else
            DrawText(Text, Position.X + 4, Position.Y + 2, SFML.Graphics.Color.White);
    }
    #endregion

    #region Menu
    public static void Menu()
    {
        // Define a habilitação das ferramentas
        Tools.SetEnable(string.Empty, Tools.Windows.Menu);

        // Desenha o menu
        Menu_Tools();
        Menu_Connect();
        Menu_Register();
        Menu_Options();
        Menu_SelectCharacter();
        Menu_CreateCharacter();
    }

    public static void Menu_Tools()
    {
        // Desenha as ferramentas básicas do menu
        if (Tools.Able) Render(Tex_BackGround, new Point(0));
        Button("Opções");
    }

    public static void Menu_Connect()
    {
        // Define a habilitação das ferramentas
        Tools.SetEnable("Conectar", Tools.Windows.Menu);

        // Desenha o conjunto das ferramentas
        Panel("Conectar");
        TextBox("Conectar_Usuário");
        TextBox("Conectar_Senha");
        Button("Conectar_Pronto");
        Button("Registrar");
        CheckBox("SalvarUsuário");
    }

    public static void Menu_Register()
    {
        // Define a habilitação das ferramentas
        Tools.SetEnable("Registrar", Tools.Windows.Menu);

        // Desenha o conjunto das ferramentas
        Panel("Registrar");
        TextBox("Registrar_Usuário");
        TextBox("Registrar_Senha");
        TextBox("Registrar_RepetirSenha");
        Button("Registrar_Pronto");
        Button("Conectar");
    }

    public static void Menu_Options()
    {
        // Define a habilitação das ferramentas
        Tools.SetEnable("Opções", Tools.Windows.Menu);

        // Desenha o conjunto das ferramentas
        Panel("Opções");
        CheckBox("Sons");
        CheckBox("Músicas");
        Button("Opções_Retornar");
    }

    public static void Menu_SelectCharacter()
    {
        // Define a habilitação das ferramentas
        Tools.SetEnable("SelecionarPersonagem", Tools.Windows.Menu);

        // Desenha o conjunto das ferramentas
        Panel("SelecionarPersonagem");
        SelectCharacter_Class();
        Button("Personagem_Criar");
        Button("Personagem_Usar");
        Button("Personagem_Deletar");
        Button("Personagem_TrocarDireita");
        Button("Personagem_TrocarEsquerda");

        // Eventos
        Buttons.Characters_Change_Buttons();
    }

    public static void Menu_CreateCharacter()
    {
        // Define a habilitação das ferramentas
        Tools.SetEnable("CriarPersonagem", Tools.Windows.Menu);

        // Desenha o conjunto das ferramentas
        Panel("CriarPersonagem");
        Button("CriarPersonagem");
        TextBox("CriarPersonagem_Nome");
        CreateCharacter_Class();
        Button("CriarPersonagem_TrocarDireita");
        Button("CriarPersonagem_TrocarEsquerda");
        Button("CriarPersonagem_Retornar");
        CheckBox("GêneroMasculino");
        CheckBox("GêneroFeminino");
    }

    public static void SelectCharacter_Class()
    {
        short Texture;
        string Text = "None";

        // Somente se necessário
        if (!Panels.Get("SelecionarPersonagem").IsAble) return;
        if (Lists.Characters == null) return;

        // Dados
        short Class = Lists.Characters[Game.SelectCharacter].Class;
        Point Text_Position = new Point(399, 425);

        // Verifica se o personagem existe
        if (Class == 0)
        {
            DrawText(Text, Text_Position.X - Tools.MeasureString(Text) / 2, Text_Position.Y, SFML.Graphics.Color.White);
            return;
        }

        // Textura do personagem
        if (Lists.Characters[Game.SelectCharacter].Genre)
            Texture = Lists.Class[Class].Texture_Male;
        else
            Texture = Lists.Class[Class].Texture_Female;

        // Desenha o personagem
        if (Texture > 0)
        {
            Render(Tex_Face[Texture], new Point(353, 442));
            Character(Texture, new Point(356, 534 - TSize(Tex_Character[Texture]).Height / 4), Game.Directions.Down, Game.Animation_Stopped);
        }

        // Desenha o nome da classe
        Text = Lists.Characters[Game.SelectCharacter].Name;
        DrawText(Text, Text_Position.X - Tools.MeasureString(Text) / 2, Text_Position.Y, SFML.Graphics.Color.White);
    }

    public static void CreateCharacter_Class()
    {
        short Texture;

        // Não desenhar se o painel não for visível
        if (!Panels.Get("CriarPersonagem").IsAble)
            return;

        // Textura do personagem
        if (CheckBoxes.Get("GêneroMasculino").State)
            Texture = Lists.Class[Game.CreateCharacter_Class].Texture_Male;
        else
            Texture = Lists.Class[Game.CreateCharacter_Class].Texture_Female;

        // Desenha o personagem
        if (Texture > 0)
        {
            Render(Tex_Face[Texture], new Point(425, 467));
            Character(Texture, new Point(430, 527), Game.Directions.Down, Game.Animation_Stopped);
        }

        // Desenha o nome da classe
        string Text = Lists.Class[Game.CreateCharacter_Class].Name;
        DrawText(Text, 471 - Tools.MeasureString(Text) / 2, 449, SFML.Graphics.Color.White);
    }
    #endregion

    #region Game
    public static void Game_Interface()
    {
        // Define a habilitação das ferramentas
        Tools.SetEnable(string.Empty, Tools.Windows.Game);

        // Desenha o conjunto das ferramentas
        Game_Menu();
        Game_Chat();
        Game_Bars();
        Game_Hotbar();
        Game_Menu_Character();
        Game_Menu_Inventory();
    }

    public static void Game_Hotbar()
    {
        string Indicator = string.Empty;
        Point Panel_Position = Panels.Get("Hotbar").Position;

        // Desenha o painel 
        Panel("Hotbar");

        // Desenha os itens da hotbar
        for (byte i = 1; i <= Game.Max_Hotbar; i++)
        {
            byte Slot = Player.Hotbar[i].Slot;
            if (Slot > 0)
            {
                // Itens
                if (Player.Hotbar[i].Type == (byte)Game.Hotbar.Item)
                {
                    // Desenha as visualizações do item
                    Point Position = new Point(Panel_Position.X + 8 + (i - 1) * 36, Panel_Position.Y + 6);
                    Render(Tex_Item[Lists.Item[Player.Inventory[Slot].Item_Num].Texture], Position);
                    if (Tools.IsAbove(new Rectangle(Position.X, Position.Y, 32, 32))) Painel_Informations(Player.Inventory[Slot].Item_Num, Panel_Position.X, Panel_Position.Y + 42);
                }
            }

            // Números da hotbar
            if (i < 10)
                Indicator = i.ToString();
            else if (i == 10)
                Indicator = "0";

            // Desenha os números
            DrawText(Indicator, Panel_Position.X + 16 + 36 * (i - 1), Panel_Position.Y + 22, SFML.Graphics.Color.White);
        }

        // Movendo slot
        if (Player.Hotbar_Change > 0)
            if (Player.Hotbar[Player.Hotbar_Change].Type == (byte)Game.Hotbar.Item)
                Render(Tex_Item[Lists.Item[Player.Inventory[Player.Hotbar[Player.Hotbar_Change].Slot].Item_Num].Texture], new Point(Tools.Mouse.X + 6, Tools.Mouse.Y + 6));
    }

    public static void Game_Menu()
    {
        // Desenha o conjunto das ferramentas
        Panel("Menu");
        Button("Menu_Personagem");
        Button("Menu_Inventário");
        Button("Menu_Feitiços");
        Button("Menu_1");
        Button("Menu_2");
        Button("Menu_Opções");
    }

    public static void Game_Menu_Character()
    {
        Point Panel_Position = Panels.Get("Menu_Personagem").Position;

        // Somente se necessário
        if (!Panels.Get("Menu_Personagem").Visible) return;

        // Desenha o painel 
        Panel("Menu_Personagem");

        // Dados básicos
        DrawText(Player.Me.Name, Panel_Position.X + 18, Panel_Position.Y + 52, SFML.Graphics.Color.White);
        DrawText(Player.Me.Level.ToString(), Panel_Position.X + 18, Panel_Position.Y + 79, SFML.Graphics.Color.White);
        Render(Tex_Face[Lists.Class[Player.Me.Class].Texture_Male], new Point(Panel_Position.X + 82, Panel_Position.Y + 37));

        // Adicionar atributos
        if (Player.Me.Points > 0)
        {
            Button("Atributos_Força");
            Button("Atributos_Resistência");
            Button("Atributos_Inteligência");
            Button("Atributos_Agilidade");
            Button("Atributos_Vitalidade");
        }

        // Atributos
        DrawText("Strength: " + Player.Me.Attribute[(byte)Game.Attributes.Strength], Panel_Position.X + 32, Panel_Position.Y + 146, SFML.Graphics.Color.White);
        DrawText("Resistance: " + Player.Me.Attribute[(byte)Game.Attributes.Resistance], Panel_Position.X + 32, Panel_Position.Y + 162, SFML.Graphics.Color.White);
        DrawText("Intelligence: " + Player.Me.Attribute[(byte)Game.Attributes.Intelligence], Panel_Position.X + 32, Panel_Position.Y + 178, SFML.Graphics.Color.White);
        DrawText("Agility: " + +Player.Me.Attribute[(byte)Game.Attributes.Agility], Panel_Position.X + 32, Panel_Position.Y + 194, SFML.Graphics.Color.White);
        DrawText("Vitality: " + +Player.Me.Attribute[(byte)Game.Attributes.Vitality], Panel_Position.X + 32, Panel_Position.Y + 210, SFML.Graphics.Color.White);
        DrawText("Points: " + Player.Me.Points, Panel_Position.X + 14, Panel_Position.Y + 228, SFML.Graphics.Color.White);

        // Equipamentos 
        for (byte i = 0; i <= (byte)Game.Equipments.Amount - 1; i++)
        {
            if (Player.Me.Equipment[i] == 0)
                Render(Tex_Equipments, Panel_Position.X + 7 + i * 34, Panel_Position.Y + 247, i * 34, 0, 34, 34);
            else
            {
                Render(Tex_Item[Lists.Item[Player.Me.Equipment[i]].Texture], Panel_Position.X + 8 + i * 35, Panel_Position.Y + 247, 0, 0, 34, 34);
                if (Tools.IsAbove(new Rectangle(Panel_Position.X + 7 + i * 36, Panel_Position.Y + 247, 32, 32))) Painel_Informations(Player.Me.Equipment[i], Panel_Position.X - 186, Panel_Position.Y + 5);
            }
        }
    }

    public static void Game_Menu_Inventory()
    {
        byte NumColumns = 5;
        Point Panel_Position = Panels.Get("Menu_Inventário").Position;

        // Somente se necessário
        if (!Panels.Get("Menu_Inventário").Visible) return;

        // Desenha o painel 
        Panel("Menu_Inventário");

        // Desenha todos os itens do inventário
        for (byte i = 1; i <= Game.Max_Inventory; i++)
            if (Player.Inventory[i].Item_Num > 0)
            {
                byte Line = (byte)((i - 1) / NumColumns);
                int Column = i - (Line * 5) - 1;
                Point Position = new Point(Panel_Position.X + 7 + Column * 36, Panel_Position.Y + 30 + Line * 36);

                // Desenha as visualizações do item
                Render(Tex_Item[Lists.Item[Player.Inventory[i].Item_Num].Texture], Position);
                if (Tools.IsAbove(new Rectangle(Position.X, Position.Y, 32, 32))) Painel_Informations(Player.Inventory[i].Item_Num, Panel_Position.X - 186, Panel_Position.Y + 3);

                // Quantidade
                if (Player.Inventory[i].Amount > 1) DrawText(Player.Inventory[i].Amount.ToString(), Position.X + 2, Position.Y + 17, SFML.Graphics.Color.White);
            }

        // Movendo item
        if (Player.Inventory_Change > 0)
            Render(Tex_Item[Lists.Item[Player.Inventory[Player.Inventory_Change].Item_Num].Texture], new Point(Tools.Mouse.X + 6, Tools.Mouse.Y + 6));
    }

    public static void Painel_Informations(short Item_Num, int X, int Y)
    {
        // Desenha o painel 
        Panels.Get("Menu_Informação").Position.X = X;
        Panels.Get("Menu_Informação").Position.Y = Y;
        Panel("Menu_Informação");

        // Informações
        Point Position = Panels.Get("Menu_Informação").Position;
        DrawText(Lists.Item[Item_Num].Name, Position.X + 9, Position.Y + 6, SFML.Graphics.Color.Yellow);
        Render(Tex_Item[Lists.Item[Item_Num].Texture], new Rectangle(Position.X + 9, Position.Y + 21, 64, 64));

        // Requerimentos
        if (Lists.Item[Item_Num].Type != (byte)Game.Itens.None)
        {
            DrawText("Req level: " + Lists.Item[Item_Num].Req_Level, Position.X + 9, Position.Y + 90, SFML.Graphics.Color.White);
            if (Lists.Item[Item_Num].Req_Class > 0)
                DrawText("Req classe: " + Lists.Class[Lists.Item[Item_Num].Req_Class].Name, Position.X + 9, Position.Y + 102, SFML.Graphics.Color.White);
            else
                DrawText("Req classe: Nenhuma", Position.X + 9, Position.Y + 102, SFML.Graphics.Color.White);
        }

        // Específicas 
        if (Lists.Item[Item_Num].Type == (byte)Game.Itens.Potion)
        {
            for (byte n = 0; n <= (byte)Game.Vitals.Amount - 1; n++)
                DrawText(((Game.Vitals)n).ToString() + ": " + Lists.Item[Item_Num].Potion_Vital[n], Position.X + 100, Position.Y + 18 + 12 * n, SFML.Graphics.Color.White);
            DrawText("Exp: " + Lists.Item[Item_Num].Potion_Experience, Position.X + 100, Position.Y + 42, SFML.Graphics.Color.White);
        }
        else if (Lists.Item[Item_Num].Type == (byte)Game.Itens.Equipment)
        {
            for (byte n = 0; n <= (byte)Game.Attributes.Amount - 1; n++) DrawText(((Game.Attributes)n).ToString() + ": " + Lists.Item[Item_Num].Equip_Attribute[n], Position.X + 100, Position.Y + 18 + 12 * n, SFML.Graphics.Color.White);
            if (Lists.Item[Item_Num].Equip_Type == (byte)Game.Equipments.Weapon) DrawText("Dano: " + Lists.Item[Item_Num].Weapon_Damage, Position.X + 100, Position.Y + 18 + 60, SFML.Graphics.Color.White);
        }
    }

    public static void Game_Bars()
    {
        decimal HP_Percentage = Player.Me.Vital[(byte)Game.Vitals.HP] / (decimal)Player.Me.Max_Vital[(byte)Game.Vitals.HP];
        decimal MP_Percentage = Player.Me.Vital[(byte)Game.Vitals.MP] / (decimal)Player.Me.Max_Vital[(byte)Game.Vitals.MP];
        decimal Exp_Percentage = Player.Me.Experience / (decimal)Player.Me.ExpNeeded;

        // Painel
        Panel("Barras");

        // Barras
        Render(Tex_Bars_Panel, 14, 14, 0, 0, (int)(Tex_Bars_Panel.Size.X * HP_Percentage), 17);
        Render(Tex_Bars_Panel, 14, 32, 0, 18, (int)(Tex_Bars_Panel.Size.X * MP_Percentage), 17);
        Render(Tex_Bars_Panel, 14, 50, 0, 36, (int)(Tex_Bars_Panel.Size.X * Exp_Percentage), 17);

        // Textos 
        DrawText("HP", 18, 11, SFML.Graphics.Color.White);
        DrawText("MP", 18, 29, SFML.Graphics.Color.White);
        DrawText("Exp", 18, 47, SFML.Graphics.Color.White);

        // Indicadores
        DrawText(Player.Me.Vital[(byte)Game.Vitals.HP] + "/" + Player.Me.Max_Vital[(byte)Game.Vitals.HP], 70, 15, SFML.Graphics.Color.White);
        DrawText(Player.Me.Vital[(byte)Game.Vitals.MP] + "/" + Player.Me.Max_Vital[(byte)Game.Vitals.MP], 70, 33, SFML.Graphics.Color.White);
        DrawText(Player.Me.Experience + "/" + Player.Me.ExpNeeded, 70, 51, SFML.Graphics.Color.White);
        DrawText("Position: " + Player.Me.X + "/" + Player.Me.Y, 8, 93, SFML.Graphics.Color.White);
    }

    public static void Game_Chat()
    {
        // Define a bisiblidade da caixa
        Panels.Get("Chat").Visible = TextBoxes.TexBox_Focus == TextBoxes.GetIndex("Chat");

        // Renderiza as caixas
        Panel("Chat");
        TextBox("Chat");

        // Renderiza as mensagens
        if (Tools.Chat_Text_Visible)
            for (byte i = Tools.Chat_Line; i <= Tools.Chat_Lines_Visible + Tools.Chat_Line; i++)
                if (Tools.Chat.Count > i)
                    DrawText(Tools.Chat[i].Text, 16, 461 + 11 * (i - Tools.Chat_Line), Tools.Chat[i].Color);

        // Dica de como abrir o chat
        if (!Panels.Get("Chat").Visible)
            DrawText("Press [Enter] to open chat.", TextBoxes.Get("Chat").Position.X + 5, TextBoxes.Get("Chat").Position.Y + 3, SFML.Graphics.Color.White);
        else
        {
            Button("Chat_Subir");
            Button("Chat_Descer");
        }
    }
    #endregion

    public static void Character(short Textura, Point Position, Game.Directions Direction, byte Column, bool Hurt = false)
    {
        Rectangle Rec_Source = new Rectangle(), Rec_Destiny;
        Size Size = TSize(Tex_Character[Textura]);
        SFML.Graphics.Color Color = new SFML.Graphics.Color(255, 255, 255);
        byte Line = 0;

        // Direção
        switch (Direction)
        {
            case Game.Directions.Up: Line = Game.Movement_Up; break;
            case Game.Directions.Down: Line = Game.Movement_Down; break;
            case Game.Directions.Left: Line = Game.Movement_Left; break;
            case Game.Directions.Right: Line = Game.Movement_Right; break;
        }

        // Define as propriedades dos retângulos
        Rec_Source.X = Column * Size.Width / Game.Animation_Amount;
        Rec_Source.Y = Line * Size.Height / Game.Animation_Amount;
        Rec_Source.Width = Size.Width / Game.Animation_Amount;
        Rec_Source.Height = Size.Height / Game.Animation_Amount;
        Rec_Destiny = new Rectangle(Position, Rec_Source.Size);

        // Demonstra que o personagem está sofrendo dano
        if (Hurt) Color = new SFML.Graphics.Color(205, 125, 125);

        // Desenha o personagem e sua sombra
        Render(Tex_Shadow, Rec_Destiny.Location.X, Rec_Destiny.Location.Y + Size.Height / Game.Animation_Amount - TSize(Tex_Shadow).Height + 5, 0, 0, Size.Width / Game.Animation_Amount, TSize(Tex_Shadow).Height);
        Render(Tex_Character[Textura], Rec_Source, Rec_Destiny, Color);
    }
}